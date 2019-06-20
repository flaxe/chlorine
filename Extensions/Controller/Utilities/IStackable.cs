namespace Chlorine.Controller
{
	public interface IStackable<in T>
	{
		bool Stack(T value);
	}
}