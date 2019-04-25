namespace Chlorine.Providers
{
	internal interface IActionDelegateProvider<TAction>
			where TAction : struct
	{
		IActionDelegate<TAction> Provide(ref TAction action);
		void Release(IActionDelegate<TAction> actionDelegate);
	}
}