namespace Chlorine
{
	public sealed class PromiseExtension : IExtension
	{
		public void Extend(Container container)
		{
			if (container.TryResolve<PromisePool>() == null)
			{
				container.Bind<PromisePool>().AsSingleton();
			}
			if (container.TryResolve<FuturePool>() == null)
			{
				container.Bind<FuturePool>().AsSingleton();
			}
		}
	}
}