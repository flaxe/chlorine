namespace Chlorine
{
	public interface IProvider
	{
		object Provide();
	}

	public interface IProvider<out T> : IProvider
	{
		new T Provide();
	}
}