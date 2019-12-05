using Chlorine.Providers;

namespace Chlorine.Controller.Providers
{
	internal interface IDelegateProvider : IProvider
	{
		void Release(object value);
	}
}