using Chlorine.Providers;

namespace Chlorine.Bindings
{
	public struct BindingActionResult<TAction, TResult>
			where TAction : struct
	{
		private readonly Container _container;
		private readonly ControllerBinder _binder;

		internal BindingActionResult(Container container, ControllerBinder binder)
		{
			_container = container;
			_binder = binder;
		}

		public BindingActionDelegate<TAction, TResult> To<TActionDelegate>()
				where TActionDelegate : class, IActionDelegate<TAction, TResult>
		{
			return new BindingActionDelegate<TAction, TResult>(_binder,
					new InstanceProvider<TActionDelegate, IActionDelegate<TAction, TResult>>(_container));
		}

		public BindingActionDelegate<TAction, TResult> FromFactory<TActionDelegateFactory>()
				where TActionDelegateFactory : class, IFactory<IActionDelegate<TAction, TResult>>
		{
			return new BindingActionDelegate<TAction, TResult>(_binder,
					new FromFactoryProvider<TActionDelegateFactory, IActionDelegate<TAction, TResult>>(_container));
		}
	}
}