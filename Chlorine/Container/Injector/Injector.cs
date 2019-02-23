using System;
using System.Collections.Generic;

namespace Chlorine
{
	internal class Injector
	{
		private static readonly Type ObjectType = typeof(object);

		private readonly ArrayPool<Argument> _argumentsPool = new ArrayPool<Argument>();
		private readonly ArrayPool<object> _parametersPool = new ArrayPool<object>();

		private readonly Binder _binder;

		private InjectAnalyzer _analyzer;

		public Injector(Binder binder)
		{
			_binder = binder;
		}

		public object Instantiate(Type type, Argument[] arguments)
		{
			InjectInfo info = GetAnalyzer().GetInfo(type, InjectFlag.Construct | InjectFlag.Inject);
			return InstantiateInternal(info, arguments);
		}

		public void Inject(object instance, Argument[] arguments)
		{
			InjectInfo info = GetAnalyzer().GetInfo(instance.GetType(), InjectFlag.Inject);
			InjectInternal(instance, info, arguments);
		}

		private InjectAnalyzer GetAnalyzer()
		{
			if (_analyzer == null && !_binder.TryResolveType(null, out _analyzer))
			{
				throw new InjectException("Unable to resolve 'InjectAnalyzer'.");
			}
			return _analyzer;
		}

		private object InstantiateInternal(InjectInfo info, Argument[] arguments)
		{
			InjectConstructorInfo constructorInfo = info.Constructor;
			if (constructorInfo == null)
			{
				throw new ArgumentException($"Can't instantiate '{info.Type.Name}'. Has no constructor.");
			}
			object instance;
			object[] parameters = ResolveParameters(info, constructorInfo.Parameters, arguments);
			try
			{
				instance = constructorInfo.Info.Invoke(parameters);
			}
			finally
			{
				_parametersPool.Release(parameters);
			}
			InjectInternal(instance, info, arguments);
			return instance;
		}

		private void InjectInternal(object instance, InjectInfo info, Argument[] arguments)
		{
			Type baseType = info.Type.BaseType;
			if (baseType != null && baseType != ObjectType)
			{
				InjectInternal(instance, GetAnalyzer().GetInfo(baseType, InjectFlag.Inject), arguments);
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
						_parametersPool.Release(parameters);
					}
				}
			}
		}

		private object ResolveType(InjectInfo info, Type type, InjectAttributeInfo attributeInfo)
		{
			if (!_binder.TryResolveType(type, attributeInfo?.Id, out object instance) && (attributeInfo == null || !attributeInfo.Optional))
			{
				throw new InjectException($"Unable to resolve '{type.Name}' while processing '{info.Type.Name}'.");
			}
			return instance;
		}

		private object[] ResolveParameters(InjectInfo info, List<InjectParameterInfo> parametersInfo, Argument[] arguments)
		{
			object[] parameters;
			if (parametersInfo != null && parametersInfo.Count > 0)
			{
				parameters = _parametersPool.Pull(parametersInfo.Count) ?? new object[parametersInfo.Count];
				if (arguments != null && arguments.Length > 0)
				{
					Argument[] unusedArguments = _argumentsPool.Pull(arguments.Length) ?? new Argument[arguments.Length];
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
								Argument argument = unusedArguments[j];
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
						_argumentsPool.Release(unusedArguments);
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
				parameters = _parametersPool.Pull(0) ?? new object[0];
			}
			return parameters;
		}
	}
}