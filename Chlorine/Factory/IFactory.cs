namespace Chlorine
{
	public interface IFactory<out T>
	{
		T Create();
	}
}