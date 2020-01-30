using Chlorine.Factories;
using Chlorine.Providers;

namespace Chlorine.Controller.Bindings
{
	public struct BindingAction<TAction>
			where TAction : struct
	{
		private readonly Container _container;
		private readonly ControllerBinder _binder;

		internal BindingAction(Container container, ControllerBinder binder)
		{
			_container = container;
			_binder = binder;
		}

		public BindingActionDelegate<TAction> To<TActionDelegate>()
				where TActionDelegate : class, IActionDelegate<TAction>
		{
			return new BindingActionDelegate<TAction>(_binder,
					new TypeProvider(typeof(TActionDelegate), _container));
		}

		public BindingStackableActionDelegate<TAction> ToStackable<TActionDelegate>()
				where TActionDelegate : class, IStackableActionDelegate<TAction>
		{
			return new BindingStackableActionDelegate<TAction>(_binder,
					new TypeProvider(typeof(TActionDelegate), _container));
		}

		public BindingActionDelegate<TAction> FromFactory<TActionDelegateFactory>()
				where TActionDelegateFactory : class, IFactory<IActionDelegate<TAction>>
		{
			return new BindingActionDelegate<TAction>(_binder,
					new FromFactoryTypeProvider<IActionDelegate<TAction>>(typeof(TActionDelegateFactory), _container));
		}

		public BindingStackableActionDelegate<TAction> FromStackableFactory<TActionDelegateFactory>()
				where TActionDelegateFactory : class, IFactory<IStackableActionDelegate<TAction>>
		{
			return new BindingStackableActionDelegate<TAction>(_binder,
					new FromFactoryTypeProvider<IActionDelegate<TAction>>(typeof(TActionDelegateFactory), _container));
		}
	}
}