namespace Chlorine
{
	public struct BindingActionResult<TAction, TResult>
			where TAction : struct
	{
		private readonly Container _container;
		private readonly Binder _binder;

		internal BindingActionResult(Container container, Binder binder)
		{
			_container = container;
			_binder = binder;
		}

		public BindingActionDelegate<TAction> To<TActionDelegate>()
				where TActionDelegate : class, IActionDelegate<TAction, TResult>
		{
			return new BindingActionDelegate<TAction>(_binder, new ConcreteProvider<TActionDelegate, IActionDelegate<TAction, TResult>>(_container));
		}

		public BindingActionDelegate<TAction> FromFactory<TActionDelegateFactory>()
				where TActionDelegateFactory : class, IFactory<IActionDelegate<TAction, TResult>>
		{
			return new BindingActionDelegate<TAction>(_binder, new FromFactoryProvider<TActionDelegateFactory, IActionDelegate<TAction, TResult>>(_container));
		}
	}
}