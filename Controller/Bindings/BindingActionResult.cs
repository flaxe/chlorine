using Carbone.Providers;

namespace Carbone.Bindings
{
	public readonly struct BindingActionResult<TAction, TResult>
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
					new TypeProvider(typeof(TActionDelegate), _container));
		}

		public BindingStackableActionDelegate<TAction, TResult> ToStackable<TActionDelegate>()
				where TActionDelegate : class, IStackableActionDelegate<TAction, TResult>
		{
			return new BindingStackableActionDelegate<TAction, TResult>(_binder,
					new TypeProvider(typeof(TActionDelegate), _container));
		}

		public BindingActionDelegate<TAction, TResult> FromFactory<TActionDelegateFactory>()
				where TActionDelegateFactory : class, IFactory<IActionDelegate<TAction, TResult>>
		{
			return new BindingActionDelegate<TAction, TResult>(_binder,
					new FromFactoryTypeProvider<IActionDelegate<TAction, TResult>>(typeof(TActionDelegateFactory), _container));
		}

		public BindingStackableActionDelegate<TAction, TResult> FromStackableFactory<TActionDelegateFactory>()
				where TActionDelegateFactory : class, IFactory<IStackableActionDelegate<TAction, TResult>>
		{
			return new BindingStackableActionDelegate<TAction, TResult>(_binder,
					new FromFactoryTypeProvider<IActionDelegate<TAction, TResult>>(typeof(TActionDelegateFactory), _container));
		}
	}
}