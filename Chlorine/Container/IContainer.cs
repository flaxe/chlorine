using System;

namespace Chlorine
{
	public interface IContainer
	{
		T Resolve<T>(object id = null) where T : class;
		object Resolve(Type type, object id = null);

		T TryResolve<T>(object id = null) where T : class;
		object TryResolve(Type type, object id = null);

		T Instantiate<T>(Argument[] arguments = null);
		object Instantiate(Type type, Argument[] arguments = null);

		void Inject(object instance, Argument[] arguments = null);
	}
}