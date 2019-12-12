using System;

namespace Chlorine.Injection
{
	public struct InjectContext
	{
		public readonly Type SourceType;
		public readonly string InjectName;
		public readonly Type InjectType;
		public readonly object InjectId;
		public readonly bool Optional;

		internal InjectContext(Type injectType, object injectId, bool optional)
		{
			SourceType = null;
			InjectName = null;
			InjectType = injectType;
			InjectId = injectId;
			Optional = optional;
		}

		internal InjectContext(InjectInfo injectInfo, InjectFieldInfo fieldInfo) :
				this(injectInfo, fieldInfo.Info.Name, fieldInfo.Info.FieldType, fieldInfo.Attribute)
		{
		}

		internal InjectContext(InjectInfo injectInfo, InjectPropertyInfo propertyInfo) :
				this(injectInfo, propertyInfo.Info.Name, propertyInfo.Info.PropertyType, propertyInfo.Attribute)
		{
		}

		internal InjectContext(InjectInfo injectInfo, InjectParameterInfo parameterInfo) :
				this(injectInfo, parameterInfo.Info.Name, parameterInfo.Info.ParameterType, parameterInfo.Attribute)
		{
		}

		private InjectContext(InjectInfo injectInfo, string injectName, Type injectType, InjectAttributeInfo attributeInfo)
		{
			SourceType = injectInfo.Type;
			InjectName = injectName;
			InjectType = injectType;
			if (attributeInfo != null)
			{
				InjectId = attributeInfo.Id;
				Optional = attributeInfo.Optional;
			}
			else
			{
				InjectId = null;
				Optional = false;
			}
		}
	}
}