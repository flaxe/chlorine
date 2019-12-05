using Chlorine.Controller.Providers;
using Chlorine.Controller.Supervisors;
using Chlorine.Providers;

namespace Chlorine.Controller.Bindings
{
	public struct BindingActionDelegate<TAction>
			where TAction : struct
	{
		private readonly ControllerBinder _binder;
		private readonly IProvider _delegateProvider;

		internal BindingActionDelegate(ControllerBinder binder, IProvider delegateProvider)
		{
			_binder = binder;
			_delegateProvider = delegateProvider;
		}

		public void AsTransient()
		{
			_binder.RegisterAction(typeof(TAction),
					new ActionSupervisor<TAction>(_binder, new TransientDelegateProvider(_delegateProvider)));
		}

		public void AsReusable()
		{
			_binder.RegisterAction(typeof(TAction),
					new ActionSupervisor<TAction>(_binder, new ReusableDelegateProvider(_delegateProvider)));
		}
	}

	public struct BindingActionDelegate<TAction, TResult>
			where TAction : struct
	{
		private readonly ControllerBinder _binder;
		private readonly IProvider _delegateProvider;

		internal BindingActionDelegate(ControllerBinder binder, IProvider delegateProvider)
		{
			_binder = binder;
			_delegateProvider = delegateProvider;
		}

		public void AsTransient()
		{
			_binder.RegisterAction(typeof(TAction),
					new ActionSupervisor<TAction, TResult>(_binder, new TransientDelegateProvider(_delegateProvider)));
		}

		public void AsReusable()
		{
			_binder.RegisterAction(typeof(TAction),
					new ActionSupervisor<TAction, TResult>(_binder, new ReusableDelegateProvider(_delegateProvider)));
		}
	}
}