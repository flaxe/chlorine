using Chlorine.Controller.Supervisors;
using Chlorine.Providers;

namespace Chlorine.Controller.Bindings
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

		public BindingActionDelegateFromPool<TAction> WithPool()
		{
			return new BindingActionDelegateFromPool<TAction>(_binder,
					new FromPoolProvider<IActionDelegate<TAction>>(_provider));
		}

		public void AsTransient()
		{
			_binder.BindAction(new TransientActionSupervisor<TAction>(_binder, _provider));
		}

		public void AsStackable()
		{
			_binder.BindAction(new StackableActionSupervisor<TAction>(_binder, _provider));
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

		public BindingActionDelegateFromPool<TAction, TResult> WithPool()
		{
			return new BindingActionDelegateFromPool<TAction, TResult>(_binder,
					new FromPoolProvider<IActionDelegate<TAction, TResult>>(_provider));
		}

		public void AsTransient()
		{
			_binder.BindAction(new TransientActionSupervisor<TAction, TResult>(_binder, _provider));
		}

		public void AsStackable()
		{
			_binder.BindAction(new StackableActionSupervisor<TAction, TResult>(_binder, _provider));
		}
	}
}