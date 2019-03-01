namespace Chlorine.Controller
{
	public struct BindingExecutable<TExecutable>
			where TExecutable : class, IExecutable
	{
		private readonly Container _container;
		private readonly ControllerBinder _binder;

		internal BindingExecutable(Container container, ControllerBinder binder)
		{
			_container = container;
			_binder = binder;
		}

		public BindingExecutorProvider<TExecutable> To<TExecutor>()
				where TExecutor : class, IExecutor<TExecutable>
		{
			return new BindingExecutorProvider<TExecutable>(_binder,
					new ConcreteProvider<TExecutor, IExecutor<TExecutable>>(_container));
		}

		public BindingExecutorProvider<TExecutable> FromFactory<TExecutorFactory>()
				where TExecutorFactory : class, IFactory<IExecutor<TExecutable>>
		{
			return new BindingExecutorProvider<TExecutable>(_binder,
					new FromFactoryProvider<TExecutorFactory, IExecutor<TExecutable>>(_container));
		}

		public void ToInstance(IExecutor<TExecutable> executor)
		{
			_binder.BindExecutable(new ExecutionDelegate<TExecutable>(
					new InstanceProvider<IExecutor<TExecutable>>(executor)));
		}
	}
}