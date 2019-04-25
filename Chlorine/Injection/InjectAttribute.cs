using System;

namespace Chlorine
{
	[AttributeUsage(
			AttributeTargets.Constructor |
			AttributeTargets.Method |
			AttributeTargets.Property |
			AttributeTargets.Field |
			AttributeTargets.Parameter)]
	public class InjectAttribute : Attribute
	{
		public object Id { get; set; }
		public bool Optional { get; set; }
	}
}