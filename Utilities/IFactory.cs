namespace Carbone
{
	public interface IFactory<out T>
	{
		T Create();
	}
}