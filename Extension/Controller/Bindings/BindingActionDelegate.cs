using Chlorine.Providers;
using Chlorine.Supervisors;

namespace Chlorine.Bindings
{
	public struct BindingActionDelegate<TAction>
			where TAction : struct
	{
		private readonly ControllerBinder _binder;

		private readonly IProvider<IActionDelegate<TAction>> _provider;

		internal BindingActionDelegate(ControllerBinder binder, IProvider<IActionDelegate<TAction>> provider)
		{
			_binder = binder;
			_provider = provider;
		}

		public BindingReusableActionDelegate<TAction> Reusable()
		{
			return new BindingReusableActionDelegate<TAction>(_binder, _provider);
		}

		public void AsTransient()
		{
			_binder.BindAction(new TransientActionSupervisor<TAction>(_binder,
					new TransientActionDelegateProvider<IActionDelegate<TAction>>(_provider)));
		}

		public void AsStackable()
		{
			_binder.BindAction(new StackableActionSupervisor<TAction>(_binder,
					new TransientActionDelegateProvider<IActionDelegate<TAction>>(_provider)));
		}
	}

	public struct BindingActionDelegate<TAction, TResult>
			where TAction : struct
	{
		private readonly ControllerBinder _binder;

		private readonly IProvider<IActionDelegate<TAction, TResult>> _provider;

		internal BindingActionDelegate(ControllerBinder binder, IProvider<IActionDelegate<TAction, TResult>> provider)
		{
			_binder = binder;
			_provider = provider;
		}

		public BindingReusableActionDelegate<TAction, TResult> Reusable()
		{
			return new BindingReusableActionDelegate<TAction, TResult>(_binder, _provider);
		}

		public void AsTransient()
		{
			_binder.BindAction(new TransientActionSupervisor<TAction, TResult>(_binder,
					new TransientActionDelegateProvider<IActionDelegate<TAction, TResult>>(_provider)));
		}

		public void AsStackable()
		{
			_binder.BindAction(new StackableActionSupervisor<TAction, TResult>(_binder,
					new TransientActionDelegateProvider<IActionDelegate<TAction, TResult>>(_provider)));
		}
	}
}