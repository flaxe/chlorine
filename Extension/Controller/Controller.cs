using System;
using Chlorine.Binder;

namespace Chlorine
{
	internal class Controller : IController
	{
		private readonly ControllerBinder _binder;

		public Controller(ControllerBinder binder)
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