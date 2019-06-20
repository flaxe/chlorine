using Chlorine.Controller.Supervisors;
using Chlorine.Providers;

namespace Chlorine.Controller.Bindings
{
	public struct BindingActionDelegateFromPool<TAction>
			where TAction : struct
	{
		private readonly ControllerBinder _binder;

		private readonly IFromPoolProvider<IActionDelegate<TAction>> _provider;

		internal BindingActionDelegateFromPool(ControllerBinder binder, IFromPoolProvider<IActionDelegate<TAction>> provider)
		{
			_binder = binder;
			_provider = provider;
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

	public struct BindingActionDelegateFromPool<TAction, TResult>
			where TAction : struct
	{
		private readonly ControllerBinder _binder;

		private readonly IFromPoolProvider<IActionDelegate<TAction, TResult>> _provider;

		internal BindingActionDelegateFromPool(ControllerBinder binder, IFromPoolProvider<IActionDelegate<TAction, TResult>> provider)
		{
			_binder = binder;
			_provider = provider;
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