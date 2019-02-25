namespace Chlorine
{
	public struct BindingExecutorProvider<TExecutable>
			where TExecutable : class, IExecutable
	{
		private readonly Binder _binder;
		private readonly IProvider<IExecutor<TExecutable>> _provider;

		internal BindingExecutorProvider(Binder binder, IProvider<IExecutor<TExecutable>> provider)
		{
			_binder = binder;
			_provider = provider;
		}

		public void AsSingleton()
		{
			_binder.BindExecutable(new ExecutionDelegate<TExecutable>(new SingletonProvider<IExecutor<TExecutable>>(_provider)));
		}

		public void AsTransient()
		{
			_binder.BindExecutable(new ExecutionDelegate<TExecutable>(new TransientProvider<IExecutor<TExecutable>>(_provider)));
		}
	}
}