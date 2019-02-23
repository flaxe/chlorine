namespace Chlorine
{
	public struct BindingActionDelegate<TAction>
			where TAction : struct
	{
		private readonly Binder _binder;

		private readonly IProvider<IActionDelegate<TAction>> _provider;

		internal BindingActionDelegate(Binder binder, IProvider<IActionDelegate<TAction>> provider)
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