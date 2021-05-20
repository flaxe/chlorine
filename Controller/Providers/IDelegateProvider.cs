namespace Carbone.Providers
{
	internal interface IDelegateProvider : IProvider
	{
		void Release(object value);
	}
}