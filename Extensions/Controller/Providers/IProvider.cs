using Chlorine.Providers;

namespace Chlorine.Controller.Providers
{
	public interface IProvider<out T> : IProvider
	{
		new T Provide();
	}
}