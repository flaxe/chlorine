namespace Chlorine
{
	internal interface IProvider
	{
		object Provide();
	}

	internal interface IProvider<out T> : IProvider
	{
		new T Provide();
	}
}