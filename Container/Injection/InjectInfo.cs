using System;
using System.Collections.Generic;
using System.Reflection;

namespace Carbone.Injection
{
	internal class InjectInfo
	{
		public readonly Type Type;

		public InjectConstructorInfo? Constructor;
		public List<InjectMethodInfo>? Methods;
		public List<InjectPropertyInfo>? Properties;
		public List<InjectFieldInfo>? Fields;

		public InjectInfo(Type type)
		{
			Type = type;
		}
	}

	internal class InjectConstructorInfo
	{
		public readonly ConstructorInfo Info;
		public readonly List<InjectParameterInfo>? Parameters;

		public InjectConstructorInfo(ConstructorInfo info, List<InjectParameterInfo>? parameters = null)
		{
			Info = info;
			Parameters = parameters;
		}
	}

	internal class InjectMethodInfo
	{
		public readonly MethodInfo Info;
		public readonly List<InjectParameterInfo>? Parameters;

		public InjectMethodInfo(MethodInfo info, List<InjectParameterInfo>? parameters = null)
		{
			Info = info;
			Parameters = parameters;
		}
	}

	internal class InjectPropertyInfo
	{
		public readonly PropertyInfo Info;
		public readonly InjectAttributeInfo Attribute;

		public InjectPropertyInfo(PropertyInfo info, InjectAttributeInfo attribute)
		{
			Info = info;
			Attribute = attribute;
		}
	}

	internal class InjectFieldInfo
	{
		public readonly FieldInfo Info;
		public readonly InjectAttributeInfo Attribute;

		public InjectFieldInfo(FieldInfo info, InjectAttributeInfo attribute)
		{
			Info = info;
			Attribute = attribute;
		}
	}

	internal class InjectParameterInfo
	{
		public readonly ParameterInfo Info;
		public readonly InjectAttributeInfo? Attribute;

		public InjectParameterInfo(ParameterInfo info, InjectAttributeInfo? attribute = null)
		{
			Info = info;
			Attribute = attribute;
		}
	}

	internal class InjectAttributeInfo
	{
		public readonly object? Id;
		public readonly bool Optional;

		public InjectAttributeInfo(object? id, bool optional)
		{
			Id = id;
			Optional = optional;
		}
	}
}