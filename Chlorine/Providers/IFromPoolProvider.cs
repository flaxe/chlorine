namespace Chlorine.Providers
{
	public interface IFromPoolProvider<T> : IProvider<T>
			where T : class
	{
		void Release(T value, bool reset = true);
	}
}