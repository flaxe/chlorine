namespace Carbone
{
	public interface IStackable<in T>
	{
		bool Stack(T value);
	}
}