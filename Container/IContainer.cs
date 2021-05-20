using System;

namespace Carbone
{
	public interface IContainer
	{
		T Resolve<T>(object? id = null) where T : class;
		object Resolve(Type type, object? id = null);

		T? TryResolve<T>(object? id = null) where T : class;
		object? TryResolve(Type type, object? id = null);

		T Instantiate<T>(TypeValue[]? arguments = null);
		object Instantiate(Type type, TypeValue[]? arguments = null);

		void Inject(object instance, TypeValue[]? arguments = null);
	}
}