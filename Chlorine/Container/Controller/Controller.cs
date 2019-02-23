using System;

namespace Chlorine
{
	internal class Controller : IController
	{
		private readonly Binder _binder;

		public Controller(Binder binder)
		{
			_binder = binder;
		}

		public IFuture Perform<TAction>(TAction action) where TAction : struct
		{
			throw new NotImplementedException();
		}

		public IFuture<TResult> Perform<TAction, TResult>(TAction action) where TAction : struct
		{
			throw new NotImplementedException();
		}

		public IFuture TryPerform<TAction>(TAction action) where TAction : struct
		{
			throw new NotImplementedException();
		}

		public IFuture<TResult> TryPerform<TAction, TResult>(TAction action) where TAction : struct
		{
			throw new NotImplementedException();
		}
	}
}