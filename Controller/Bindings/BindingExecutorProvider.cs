using Carbone.Execution;
using Carbone.Providers;

namespace Carbone.Bindings
{
	public readonly struct BindingExecutorProvider<TExecutable>
			where TExecutable : class, IExecutable
	{
		private readonly ControllerBinder _binder;
		private readonly IProvider _executorProvider;

		internal BindingExecutorProvider(ControllerBinder binder, IProvider executorProvider)
		{
			_binder = binder;
			_executorProvider = executorProvider;
		}

		public void AsSingleton()
		{
			_binder.RegisterExecutable(typeof(TExecutable),
					new ExecutionDelegate<TExecutable>(new SingletonProvider(_executorProvider)));
		}

		public void AsTransient()
		{
			_binder.RegisterExecutable(typeof(TExecutable), new ExecutionDelegate<TExecutable>(_executorProvider));
		}
	}
}