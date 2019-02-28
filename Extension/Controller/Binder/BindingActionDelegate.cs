namespace Chlorine.Controller
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

		public void AsTransient()
		{
			_binder.BindAction(new TransientProvider<IActionDelegate<TAction>>(_provider));
		}
	}
}