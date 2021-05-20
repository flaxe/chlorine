using Carbone.Providers;
using Carbone.Supervisors;

namespace Carbone.Bindings
{
	public readonly struct BindingStackableActionDelegate<TAction>
			where TAction : struct
	{
		private readonly ControllerBinder _binder;
		private readonly IProvider _delegateProvider;

		internal BindingStackableActionDelegate(ControllerBinder binder, IProvider delegateProvider)
		{
			_binder = binder;
			_delegateProvider = delegateProvider;
		}

		public void AsTransient()
		{
			_binder.RegisterAction(typeof(TAction),
					new StackableActionSupervisor<TAction>(_binder, new TransientDelegateProvider(_delegateProvider)));
		}

		public void AsReusable()
		{
			_binder.RegisterAction(typeof(TAction),
					new StackableActionSupervisor<TAction>(_binder, new ReusableDelegateProvider(_delegateProvider)));
		}
	}

	public readonly struct BindingStackableActionDelegate<TAction, TResult>
			where TAction : struct
	{
		private readonly ControllerBinder _binder;
		private readonly IProvider _delegateProvider;

		internal BindingStackableActionDelegate(ControllerBinder binder, IProvider delegateProvider)
		{
			_binder = binder;
			_delegateProvider = delegateProvider;
		}

		public void AsTransient()
		{
			_binder.RegisterAction(typeof(TAction),
					new StackableActionSupervisor<TAction, TResult>(_binder, new TransientDelegateProvider(_delegateProvider)));
		}

		public void AsReusable()
		{
			_binder.RegisterAction(typeof(TAction),
					new StackableActionSupervisor<TAction, TResult>(_binder, new ReusableDelegateProvider(_delegateProvider)));
		}
	}
}