namespace Chlorine.Providers
{
	internal interface IActionDelegateProvider<T> : IProvider<T> where T : class
	{
		void Release(T actionDelegate);
	}
}