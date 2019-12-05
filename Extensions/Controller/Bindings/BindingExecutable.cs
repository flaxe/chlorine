using Chlorine.Controller.Execution;
using Chlorine.Factories;
using Chlorine.Providers;

namespace Chlorine.Controller.Bindings
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
			return new BindingExecutorProvider<TExecutable>(_binder, new TypeProvider(typeof(TExecutor), _container));
		}

		public BindingExecutorProvider<TExecutable> FromFactory<TExecutorFactory>()
				where TExecutorFactory : class, IFactory<IExecutor<TExecutable>>
		{
			return new BindingExecutorProvider<TExecutable>(_binder,
					new FromFactoryTypeProvider<IExecutor<TExecutable>>(typeof(TExecutorFactory), _container));
		}

		public void ToInstance(IExecutor<TExecutable> executor)
		{
			_binder.RegisterExecutable(typeof(TExecutable),
					new ExecutionDelegate<TExecutable>(new InstanceProvider(executor)));
		}
	}
}