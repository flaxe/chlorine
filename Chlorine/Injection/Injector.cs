using System;
using System.Collections.Generic;
using Chlorine.Bindings;
using Chlorine.Exceptions;
using Chlorine.Pools;

namespace Chlorine.Injection
{
	internal sealed class Injector
	{
		private static readonly Type ObjectType = typeof(object);

		private readonly InjectAnalyzer _analyzer;
		private readonly Binder _binder;

		public Injector(InjectAnalyzer analyzer, Binder binder)
		{
			_analyzer = analyzer;
			_binder = binder;
		}

		public InjectAnalyzer Analyzer => _analyzer;

		public object Instantiate(Type type, TypeValue[] arguments)
		{
			InjectInfo info = _analyzer.GetInfo(type, InjectFlag.Construct | InjectFlag.Inject);
			return InstantiateInternal(info, arguments);
		}

		public void Inject(object instance, TypeValue[] arguments)
		{
			InjectInfo info = _analyzer.GetInfo(instance.GetType(), InjectFlag.Inject);
			InjectInternal(instance, info, arguments);
		}

		private object InstantiateInternal(InjectInfo info, TypeValue[] arguments)
		{
			InjectConstructorInfo constructorInfo = info.Constructor;
			if (constructorInfo == null)
			{
				throw new InjectException(InjectErrorCode.HasNoConstructor,
						$"Can't instantiate '{info.Type.Name}'. Has no constructor.");
			}
			object instance;
			object[] parameters = ResolveParameters(info, constructorInfo.Parameters, arguments);
			try
			{
				instance = constructorInfo.Info.Invoke(parameters);
			}
			finally
			{
				ArrayPool<object>.Release(parameters);
			}
			InjectInternal(instance, info, arguments);
			return instance;
		}

		private void InjectInternal(object instance, InjectInfo info, TypeValue[] arguments)
		{
			Type baseType = info.Type.BaseType;
			if (baseType != null && baseType != ObjectType)
			{
				InjectInternal(instance, _analyzer.GetInfo(baseType, InjectFlag.Inject), arguments);
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
						ArrayPool<object>.Release(parameters);
					}
				}
			}
		}

		private object ResolveType(InjectInfo info, Type type, InjectAttributeInfo attributeInfo)
		{
			if (!_binder.TryResolveType(type, attributeInfo?.Id, out object instance) && (attributeInfo == null || !attributeInfo.Optional))
			{
				throw new InjectException(InjectErrorCode.TypeNotRegistered,
						$"Unable to resolve '{type.Name}'{(attributeInfo?.Id != null ? $" with attribute '{attributeInfo.Id}'" : "")} while processing '{info.Type.Name}'.");
			}
			return instance;
		}

		private object[] ResolveParameters(InjectInfo info, List<InjectParameterInfo> parametersInfo, TypeValue[] arguments)
		{
			object[] parameters;
			if (parametersInfo != null && parametersInfo.Count > 0)
			{
				parameters = ArrayPool<object>.Pull(parametersInfo.Count);
				if (arguments != null && arguments.Length > 0)
				{
					TypeValue[] unusedArguments = ArrayPool<TypeValue>.Pull(arguments.Length);
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
								TypeValue argument = unusedArguments[j];
								if (argument.Type == parameterType)
								{
									parameter = argument.Value;
									unusedArguments[j] = default;
								}
							}
							parameters[i] = parameter ?? ResolveType(info, parameterType, parameterInfo.Attribute);
						}
					}
					finally
					{
						ArrayPool<TypeValue>.Release(unusedArguments);
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
				parameters = ArrayPool<object>.Pull(0);
			}
			return parameters;
		}
	}
}