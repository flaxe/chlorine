using System;

namespace Chlorine
{
	public interface IContainer
	{
		T Resolve<T>(object id = null) where T : class;
		object Resolve(Type type, object id = null);

		T Instantiate<T>(object[] arguments = null);
		object Instantiate(Type type, object[] arguments = null);

		void Inject(object instance, object[] arguments = null);
	}
}