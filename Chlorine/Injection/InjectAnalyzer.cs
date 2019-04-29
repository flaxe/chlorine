using System;
using System.Collections.Generic;
using System.Reflection;
using Chlorine.Exceptions;

namespace Chlorine.Injection
{
	[Flags]
	internal enum InjectFlag
	{
		Construct = 1,
		Inject = 2
	}

	internal sealed class InjectAnalyzer
	{
		private static readonly Type UnityComponentType = typeof(UnityEngine.Component);
		private static readonly Type InjectAttributeType = typeof(InjectAttribute);

		private readonly Dictionary<Type, AnalyzeResult> _resultByType = new Dictionary<Type, AnalyzeResult>();

		public InjectInfo GetInfo(Type type, InjectFlag flags)
		{
			if (_resultByType.TryGetValue(type, out AnalyzeResult result))
			{
				if ((result.Flags & flags) == flags)
				{
					return result.Info;
				}

				InjectFlag missingFlags = (result.Flags ^ flags) & flags;
				Analyze(result.Info, missingFlags);
				_resultByType[type] = new AnalyzeResult(result.Info, result.Flags | missingFlags);
				return result.Info;
			}

			InjectInfo info = new InjectInfo(type);
			Analyze(info, flags);
			_resultByType.Add(type, new AnalyzeResult(info, flags));
			return info;
		}

		private static void Analyze(InjectInfo info, InjectFlag flags)
		{
			Type type = info.Type;
			if (type.IsEnum)
			{
				throw new ArgumentException("Enum types not supported.");
			}
			if (type.IsArray)
			{
				throw new ArgumentException("Array types not supported.");
			}
			if ((flags & InjectFlag.Construct) == InjectFlag.Construct)
			{
				if (type.IsEqualOrDerivesFrom(UnityComponentType))
				{
					throw new ArgumentException($"Can't construct UnityEngine component '{type.Name}'.");
				}
				if (type.IsAbstract)
				{
					throw new ArgumentException($"Can't construct abstract class '{type.Name}'.");
				}
				info.Constructor = GetConstructorInfo(type);
			}
			if ((flags & InjectFlag.Inject) == InjectFlag.Inject)
			{
				info.Methods = GetMethodsInfo(type);
				info.Properties = GetPropertiesInfo(type);
				info.Fields = GetFieldsInfo(type);
			}
		}

		private static InjectConstructorInfo GetConstructorInfo(Type type)
		{
			ConstructorInfo constructorInfo = FindConstructor(type);
			return new InjectConstructorInfo(constructorInfo, GetParametersInfo(type, constructorInfo));
		}

		private static ConstructorInfo FindConstructor(Type type)
		{
			ConstructorInfo[] constructors = type.GetConstructors(
					BindingFlags.Public |
					BindingFlags.NonPublic |
					BindingFlags.Instance);
			int constructorsLength = constructors.Length;
			if (constructorsLength == 0)
			{
				throw new ArgumentException($"Class '{type.Name}' has no constructors.");
			}
			if (constructorsLength > 1)
			{
				ConstructorInfo applicableConstructor = null;

				int explicitConstructorsCount = 0;
				foreach (ConstructorInfo constructor in constructors)
				{
					Attribute[] attributes = Attribute.GetCustomAttributes(constructor, InjectAttributeType, false);
					if (attributes.Length > 0)
					{
						applicableConstructor = constructor;
						explicitConstructorsCount++;
					}
				}
				if (explicitConstructorsCount > 1)
				{
					throw new InjectException($"Class '{type.Name}' has multiple constructors with 'Inject' attribute.");
				}
				if (explicitConstructorsCount == 1)
				{
					return applicableConstructor;
				}

				int publicConstructorsCount = 0;
				foreach (ConstructorInfo constructor in constructors)
				{
					if (constructor.IsPublic)
					{
						applicableConstructor = constructor;
						publicConstructorsCount++;
					}
				}
				if (publicConstructorsCount == 1)
				{
					return applicableConstructor;
				}

				throw new InjectException($"Class '{type.Name}' has multiple public constructors. Specify one with 'Inject' attribute.");
			}

			return constructors[0];
		}

		private static List<InjectMethodInfo> GetMethodsInfo(Type type)
		{
			MethodInfo[] methods = type.GetMethods(
					BindingFlags.Public |
					BindingFlags.NonPublic |
					BindingFlags.Instance |
					BindingFlags.DeclaredOnly);
			if (methods.Length > 0)
			{
				List<InjectMethodInfo> methodsInfo = null;
				foreach (MethodInfo method in methods)
				{
					Attribute[] attributes = Attribute.GetCustomAttributes(method, InjectAttributeType, false);
					if (attributes.Length == 1)
					{
						if (methodsInfo == null)
						{
							methodsInfo = new List<InjectMethodInfo>();
						}
						methodsInfo.Add(new InjectMethodInfo(method, GetParametersInfo(type, method)));
					}
					else if (attributes.Length > 1)
					{
						throw new InjectException($"Method '{method.Name}' at class '{type.Name}' has multiple 'Inject' attributes.");
					}
				}
				return methodsInfo;
			}
			return null;
		}

		private static List<InjectPropertyInfo> GetPropertiesInfo(Type type)
		{
			PropertyInfo[] properties = type.GetProperties(
					BindingFlags.Public |
					BindingFlags.NonPublic |
					BindingFlags.Instance |
					BindingFlags.DeclaredOnly);
			if (properties.Length > 0)
			{
				List<InjectPropertyInfo> propertiesInfo = null;
				foreach (PropertyInfo property in properties)
				{
					if (!property.CanWrite)
					{
						continue;
					}
					Attribute[] attributes = Attribute.GetCustomAttributes(property, InjectAttributeType, false);
					if (attributes.Length == 1)
					{
						if (propertiesInfo == null)
						{
							propertiesInfo = new List<InjectPropertyInfo>();
						}
						propertiesInfo.Add(new InjectPropertyInfo(property, GetAttributeInfo(attributes[0])));
					}
					else if (attributes.Length > 1)
					{
						throw new InjectException($"Property '{property.Name}' at class '{type.Name}' has multiple 'Inject' attributes.");
					}
				}
				return propertiesInfo;
			}
			return null;
		}

		private static List<InjectFieldInfo> GetFieldsInfo(Type type)
		{
			FieldInfo[] fields = type.GetFields(
					BindingFlags.Public |
					BindingFlags.NonPublic |
					BindingFlags.Instance |
					BindingFlags.DeclaredOnly);
			if (fields.Length > 0)
			{
				List<InjectFieldInfo> fieldsInfo = null;
				foreach (FieldInfo field in fields)
				{
					Attribute[] attributes = Attribute.GetCustomAttributes(field, InjectAttributeType, false);
					if (attributes.Length == 1)
					{
						if (fieldsInfo == null)
						{
							fieldsInfo = new List<InjectFieldInfo>();
						}
						fieldsInfo.Add(new InjectFieldInfo(field, GetAttributeInfo(attributes[0])));
					}
					else if (attributes.Length > 1)
					{
						throw new InjectException($"Field '{field.Name}' at class '{type.Name}' has multiple 'Inject' attributes.");
					}
				}
				return fieldsInfo;
			}
			return null;
		}

		private static List<InjectParameterInfo> GetParametersInfo(Type type, MethodBase method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			if (parameters.Length > 0)
			{
				List<InjectParameterInfo> parametersInfo = new List<InjectParameterInfo>();
				foreach (ParameterInfo parameter in parameters)
				{
					parametersInfo.Add(new InjectParameterInfo(parameter, GetParameterAttributeInfo(type, method, parameter)));
				}
				return parametersInfo;
			}
			return null;
		}

		private static InjectAttributeInfo GetParameterAttributeInfo(Type type, MethodBase method, ParameterInfo parameter)
		{
			Attribute[] attributes = Attribute.GetCustomAttributes(parameter, InjectAttributeType, false);
			if (attributes.Length == 1)
			{
				return GetAttributeInfo(attributes[0]);
			}
			if (attributes.Length > 1)
			{
				throw new InjectException($"Parameter '{parameter.Name}' in method '{method.Name}' at class '{type.Name}' has multiple 'Inject' attributes.");
			}
			return null;
		}

		private static InjectAttributeInfo GetAttributeInfo(Attribute attribute)
		{
			if (attribute is InjectAttribute injectAttribute)
			{
				return new InjectAttributeInfo(injectAttribute.Id, injectAttribute.Optional);
			}
			throw new InjectException($"Attribute '{attribute.GetType()}' has invalid type.");
		}

		private struct AnalyzeResult
		{
			public readonly InjectInfo Info;
			public readonly InjectFlag Flags;

			public AnalyzeResult(InjectInfo info, InjectFlag flags)
			{
				Info = info;
				Flags = flags;
			}
		}
	}
}