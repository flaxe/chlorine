using System;
using System.Collections.Generic;

namespace Chlorine
{
	internal class Injector
	{
		private static readonly Type ObjectType = typeof(object);

		private static readonly InjectAnalyzer Analyzer = new InjectAnalyzer();

		private static readonly ArrayPool<object> ParametersPool = new ArrayPool<object>();
		private static readonly ArrayPool<TypedArgument> ArgumentsPool = new ArrayPool<TypedArgument>();

		private readonly Container _container;

		public Injector(Container container)
		{
			_container = container;
		}

		public object Instantiate(Type type, object[] arguments)
		{
			object instance;
			InjectInfo info = Analyzer.GetInfo(type, InjectFlag.Construct | InjectFlag.Inject);
			if (arguments != null && arguments.Length > 0)
			{
				TypedArgument[] typedArguments = CreateArguments(arguments);
				try
				{
					instance = InstantiateInternal(info, typedArguments);
				}
				finally
				{
					ArgumentsPool.Release(typedArguments);
				}
			}
			else
			{
				instance = InstantiateInternal(info, null);
			}
			return instance;
		}

		public void Inject(object instance, object[] arguments)
		{
			InjectInfo info = Analyzer.GetInfo(instance.GetType(), InjectFlag.Inject);
			if (arguments != null && arguments.Length > 0)
			{
				TypedArgument[] typedArguments = CreateArguments(arguments);
				try
				{
					InjectInternal(instance, info, typedArguments);
				}
				finally
				{
					ArgumentsPool.Release(typedArguments);
				}
			}
			else
			{
				InjectInternal(instance, info, null);
			}
		}

		private object InstantiateInternal(InjectInfo info, TypedArgument[] arguments)
		{
			InjectConstructorInfo constructorInfo = info.Constructor;
			if (constructorInfo == null)
			{
				throw new ArgumentException($"Can't instantiate '{info.Type}'. Type has no constructor.");
			}
			object instance;
			object[] parameters = ResolveParameters(info, constructorInfo.Parameters, arguments);
			try
			{
				instance = constructorInfo.Info.Invoke(parameters);
			}
			finally
			{
				ParametersPool.Release(parameters);
			}
			InjectInternal(instance, info, arguments);
			return instance;
		}

		private void InjectInternal(object instance, InjectInfo info, TypedArgument[] arguments)
		{
			Type baseType = info.Type.BaseType;
			if (baseType != null && baseType != ObjectType)
			{
				InjectInternal(instance, Analyzer.GetInfo(baseType, InjectFlag.Inject), arguments);
			}
			List<InjectFieldInfo> fieldsInfo = info.Fields;
			if (fieldsInfo != null && fieldsInfo.Count > 0)
			{
				foreach (InjectFieldInfo fieldInfo in fieldsInfo)
				{
					fieldInfo.Info.SetValue(instance, ResolveType(info, fieldInfo.Info.FieldType, fieldInfo.Attribute));
				}
			}
			List<InjectPropertyInfo> propertiesInfo = info.Properties;
			if (propertiesInfo != null && propertiesInfo.Count > 0)
			{
				foreach (InjectPropertyInfo propertyInfo in propertiesInfo)
				{
					propertyInfo.Info.SetValue(instance, ResolveType(info, propertyInfo.Info.PropertyType, propertyInfo.Attribute));
				}
			}
			List<InjectMethodInfo> methodsInfo = info.Methods;
			if (methodsInfo != null && methodsInfo.Count > 0)
			{
				foreach (InjectMethodInfo methodInfo in methodsInfo)
				{
					object[] parameters = ResolveParameters(info, methodInfo.Parameters, arguments);
					try
					{
						methodInfo.Info.Invoke(instance, parameters);
					}
					finally
					{
						ParametersPool.Release(parameters);
					}
				}
			}
		}

		private object ResolveType(InjectInfo info, Type type, InjectAttributeInfo attributeInfo)
		{
			object instance = _container.Resolve(type, attributeInfo?.Id);
			if (instance == null && (attributeInfo == null || !attributeInfo.Optional))
			{
				throw new InjectException($"Unable to resolve type '{type.Name}' while processing '{info.Type.Name}'.");
			}
			return instance;
		}

		private object[] ResolveParameters(InjectInfo info, List<InjectParameterInfo> parametersInfo, TypedArgument[] arguments)
		{
			object[] parameters;
			if (parametersInfo != null && parametersInfo.Count > 0)
			{
				parameters = ParametersPool.Pull(parametersInfo.Count) ?? new object[parametersInfo.Count];
				if (arguments != null && arguments.Length > 0)
				{
					TypedArgument[] unusedArguments = ArgumentsPool.Pull(arguments.Length) ?? new TypedArgument[arguments.Length];
					arguments.CopyTo(unusedArguments, 0);
					try
					{
						for (int i = 0; i < parametersInfo.Count; i++)
						{
							InjectParameterInfo parameterInfo = parametersInfo[i];
							Type parameterType = parameterInfo.Info.ParameterType;
							object parameter = null;
							for (int j = 0; j < unusedArguments.Length; j++)
							{
								TypedArgument typedArgument = unusedArguments[j];
								if (typedArgument.Type == parameterType)
								{
									parameter = typedArgument.Value;
									unusedArguments[j] = default;
								}
							}
							parameters[i] = parameter ?? ResolveType(info, parameterType, parameterInfo.Attribute);
						}
					}
					finally
					{
						ArgumentsPool.Release(unusedArguments);
					}
				}
				else
				{
					for (int i = 0; i < parametersInfo.Count; i++)
					{
						InjectParameterInfo parameterInfo = parametersInfo[i];
						Type parameterType = parameterInfo.Info.ParameterType;
						parameters[i] = ResolveType(info, parameterType, parameterInfo.Attribute);
					}
				}
			}
			else
			{
				parameters = ParametersPool.Pull(0) ?? new object[0];
			}
			return parameters;
		}

		private static TypedArgument[] CreateArguments(object[] arguments)
		{
			if (arguments != null && arguments.Length > 0)
			{
				TypedArgument[] typedArguments = ArgumentsPool.Pull(arguments.Length) ?? new TypedArgument[arguments.Length];
				for (int i = 0; i < arguments.Length; i++)
				{
					object value = arguments[i];
					typedArguments[i] = new TypedArgument(value.GetType(), value);
				}
				return typedArguments;
			}
			return null;
		}

		private struct TypedArgument
		{
			public readonly Type Type;
			public readonly object Value;

			public TypedArgument(Type type, object value)
			{
				Type = type;
				Value = value;
			}
		}
	}
}