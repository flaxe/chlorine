namespace Chlorine
{
	public struct BindingAction<TAction>
			where TAction : struct
	{
		private readonly Container _container;
		private readonly Binder _binder;

		internal BindingAction(Container container, Binder binder)
		{
			_container = container;
			_binder = binder;
		}

		public BindingActionResult<TAction, TResult> With<TResult>()
		{
			return new BindingActionResult<TAction, TResult>(_container, _binder);
		}

		public BindingActionDelegate<TAction> To<TActionDelegate>()
				where TActionDelegate : class, IActionDelegate<TAction>
		{
			return new BindingActionDelegate<TAction>(_binder, new ConcreteProvider<TActionDelegate, IActionDelegate<TAction>>(_container));
		}

		public BindingActionDelegate<TAction> FromFactory<TActionDelegateFactory>()
				where TActionDelegateFactory : class, IFactory<IActionDelegate<TAction>>
		{
			return new BindingActionDelegate<TAction>(_binder, new FromFactoryProvider<TActionDelegateFactory, IActionDelegate<TAction>>(_container));
		}
	}
}