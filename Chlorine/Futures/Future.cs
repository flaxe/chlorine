using System;
using System.Collections.Generic;
using Chlorine.Exceptions;
using Chlorine.Internal;

namespace Chlorine
{
	public sealed class Future : AbstractFuture
	{
		private IPromise _promise;

		internal Future()
		{
		}

		internal Future(IPromise promise)
		{
			Init(promise);
		}

		internal Future(Error reason) : base(reason)
		{
		}

		internal void Init(IPromise promise)
		{
			_promise?.Revoke(this);
			_promise = promise;
			_promise.Fulfill(this);
		}

		public override void Clear()
		{
			base.Clear();
			if (_promise != null)
			{
				_promise.Revoke(this);
				_promise = null;
			}
		}

		public void Resolve()
		{
			if (Status == FutureStatus.Pending)
			{
				Status = FutureStatus.Resolved;
				HandleResolve();
				HandleFinalize();
				Clear();
			}
		}
	}

	public sealed class Future<TResult> : AbstractFuture, IFuture<TResult>
	{
		private IPromise<TResult> _promise;

		private FutureResolved<TResult> _resultResolved;
		private List<IFutureHandler<TResult>> _resultHandlers;

		private TResult _result;

		internal Future()
		{
		}

		internal Future(IPromise<TResult> promise)
		{
			Init(promise);
		}

		internal Future(TResult result)
		{
			Status = FutureStatus.Resolved;
			_result = result;
		}

		internal Future(Error reason) : base(reason)
		{
		}

		internal void Init(IPromise<TResult> promise)
		{
			_promise?.Revoke(this);
			_promise = promise;
			_promise.Fulfill(this);
		}

		public TResult Result
		{
			get
			{
				if (Status != FutureStatus.Resolved)
				{
					throw new ChlorineException(ChlorineErrorCode.InvalidOperation,
							"Invalid operation. Future was not resolved.");
				}
				return _result;
			}
		}

		public bool TryGetResult(out TResult result)
		{
			if (Status == FutureStatus.Resolved)
			{
				result = _result;
				return true;
			}
			result = default;
			return false;
		}

		public override void Reset()
		{
			base.Reset();
			_result = default;
		}

		public override void Clear()
		{
			base.Clear();
			_resultResolved = null;
			if (_promise != null)
			{
				_promise.Revoke(this);
				_promise = null;
			}
		}

		public IFuture Then(FuturePromised<TResult> promised)
		{
			throw new NotImplementedException();
		}

		public IFuture<T> Then<T>(FutureResultPromised<T, TResult> promised)
		{
			throw new NotImplementedException();
		}

		public void Then(FutureResolved<TResult> resolved, FutureRejected rejected)
		{
			if (resolved == null)
			{
				throw new ArgumentNullException(nameof(resolved));
			}
			switch (Status)
			{
				case FutureStatus.Pending:
					_resultResolved += resolved;
					break;
				case FutureStatus.Resolved:
					resolved.Invoke(_result);
					break;
			}
			Catch(rejected);
		}

		public void Finally(IFutureHandler<TResult> handler)
		{
			if (handler == null)
			{
				throw new ArgumentNullException(nameof(handler));
			}
			switch (Status)
			{
				case FutureStatus.Pending:
					if (_resultHandlers == null)
					{
						_resultHandlers = new List<IFutureHandler<TResult>>{ handler };
					}
					else
					{
						_resultHandlers.Add(handler);
					}
					break;
				case FutureStatus.Resolved:
				case FutureStatus.Rejected:
					handler.HandleFuture(this);
					break;
			}
		}

		public void Resolve(TResult result)
		{
			if (Status == FutureStatus.Pending)
			{
				Status = FutureStatus.Resolved;
				_result = result;
				HandleResolve();
				HandleFinalize();
				Clear();
			}
		}

		protected override void HandleResolve()
		{
			base.HandleResolve();
			_resultResolved?.Invoke(_result);
		}

		protected override void HandleFinalize()
		{
			base.HandleFinalize();
			if (_resultHandlers != null && _resultHandlers.Count > 0)
			{
				foreach (IFutureHandler<TResult> handler in _resultHandlers)
				{
					handler.HandleFuture(this);
				}
			}
		}
	}
}