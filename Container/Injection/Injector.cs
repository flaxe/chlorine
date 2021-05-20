using System;
using System.Collections.Generic;
using Carbone.Bindings;
using Carbone.Exceptions;
using Carbone.Pools;

namespace Carbone.Injection
{
	internal sealed class Injector
	{
		private static readonly Type ObjectType = typeof(object);

		private readonly InjectAnalyzer _analyzer;
		private readonly Binder _binder;

#if DEBUG
		private readonly HashSet<Type> _instantiationTypes = new HashSet<Type>();
#endif

		public Injector(InjectAnalyzer analyzer, Binder binder)
		{
			_analyzer = analyzer;
			_binder = binder;
		}

		public InjectAnalyzer Analyzer => _analyzer;

		public object Instantiate(Type type, TypeValue[]? arguments)
		{
			InjectInfo info = _analyzer.GetInfo(type, InjectFlag.Construct | InjectFlag.Inject);
			return InstantiateInternal(info, arguments);
		}

		public void Inject(object instance, TypeValue[]? arguments)
		{
			InjectInfo info = _analyzer.GetInfo(instance.GetType(), InjectFlag.Inject);
			InjectInternal(instance, info, arguments);
		}

		private object InstantiateInternal(InjectInfo info, TypeValue[]? arguments)
		{
#if DEBUG
			if (_instantiationTypes.Contains(info.Type))
			{
				throw new InjectException(InjectErrorCode.CircularDependency,
						$"Type \"{info.Type.Name}\" has circular dependency.");
			}
			_instantiationTypes.Add(info.Type);
#endif
			InjectConstructorInfo? constructorInfo = info.Constructor;
			if (constructorInfo == null)
			{
				throw new InjectException(InjectErrorCode.HasNoConstructor,
						$"Can\"t instantiate \"{info.Type.Name}\". Has no constructor.");
			}
			object instance;
			object?[] parameters = ResolveParameters(info, constructorInfo.Parameters, arguments);
			try
			{
				instance = constructorInfo.Info.Invoke(parameters);
			}
			finally
			{
				ArrayPool<object?>.Release(parameters);
			}
			InjectInternal(instance, info, arguments);
#if DEBUG
			_instantiationTypes.Remove(info.Type);
#endif
			return instance;
		}

		private void InjectInternal(object instance, InjectInfo info, TypeValue[]? arguments)
		{
			Type? baseType = info.Type.BaseType;
			if (baseType != null && baseType != ObjectType)
			{
				InjectInternal(instance, _analyzer.GetInfo(baseType, InjectFlag.Inject), arguments);
			}
			List<InjectFieldInfo>? fieldsInfo = info.Fields;
			if (fieldsInfo != null && fieldsInfo.Count > 0)
			{
				foreach (InjectFieldInfo fieldInfo in fieldsInfo)
				{
					fieldInfo.Info.SetValue(instance, ResolveType(new InjectContext(info, fieldInfo)));
				}
			}
			List<InjectPropertyInfo>? propertiesInfo = info.Properties;
			if (propertiesInfo != null && propertiesInfo.Count > 0)
			{
				foreach (InjectPropertyInfo propertyInfo in propertiesInfo)
				{
					propertyInfo.Info.SetValue(instance, ResolveType(new InjectContext(info, propertyInfo)));
				}
			}
			List<InjectMethodInfo>? methodsInfo = info.Methods;
			if (methodsInfo != null && methodsInfo.Count > 0)
			{
				foreach (InjectMethodInfo methodInfo in methodsInfo)
				{
					object?[] parameters = ResolveParameters(info, methodInfo.Parameters, arguments);
					try
					{
						methodInfo.Info.Invoke(instance, parameters);
					}
					finally
					{
						ArrayPool<object?>.Release(parameters);
					}
				}
			}
		}

		private object? ResolveType(in InjectContext context)
		{
			if (_binder.TryResolveType(in context, out object? instance) && !context.Optional)
			{
				string identifier =
						$"\"{context.InjectType.Name}\"{(context.InjectId != null ? $" with attribute \"{context.InjectId}\"" : "")}";
				throw new InjectException(InjectErrorCode.TypeNotRegistered,
						$"Unable to resolve {identifier}{(context.SourceType != null ? $" while processing \"{context.SourceType.Name}\"." : ".")}");
			}
			return instance;
		}

		private object?[] ResolveParameters(InjectInfo info, List<InjectParameterInfo>? parametersInfo, TypeValue[]? arguments)
		{
			object?[] parameters;
			if (parametersInfo != null && parametersInfo.Count > 0)
			{
				parameters = ArrayPool<object?>.Pull(parametersInfo.Count);
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
							object? parameter = null;
							for (int j = 0; j < unusedArguments.Length; j++)
							{
								TypeValue argument = unusedArguments[j];
								if (argument.Type == parameterType)
								{
									parameter = argument.Value;
									unusedArguments[j] = default;
									break;
								}
							}
							parameters[i] = parameter ?? ResolveType(new InjectContext(info, parameterInfo));
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
						parameters[i] = ResolveType(new InjectContext(info, parametersInfo[i]));
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