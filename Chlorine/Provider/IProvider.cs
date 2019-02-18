namespace Chlorine
{
	public interface IProvider<out T>
	{
		T Provide();
	}
}