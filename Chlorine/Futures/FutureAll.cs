using System.Collections.Generic;
using Chlorine.Futures.Internal;
using Chlorine.Pools;

namespace Chlorine.Futures
{
	internal class FutureAll : AbstractFuture, IFutureHandler
	{
		private List<IFuture> _futures;

		internal FutureAll(IEnumerable<IFuture> futures)
		{
			Init(futures);
		}

		internal void Init(IEnumerable<IFuture> futures)
		{
			base.Init();

			_futures = ListPool<IFuture>.Pull(futures);

			FutureStatus status = FutureStatus.Resolved;
			Error error = default;
			foreach (IFuture future in _futures)
			{
				if (future.IsRejected)
				{
					status = FutureStatus.Rejected;
					error = future.Reason;
					break;
				}
				if (!future.IsResolved)
				{
					future.Finally(this);
					status = FutureStatus.Pending;
				}
			}
			switch (status)
			{
				case FutureStatus.Resolved:
					Resolve();
					break;
				case FutureStatus.Rejected:
					Reject(error);
					break;
			}
		}

		protected override void HandleClear()
		{
			base.HandleClear();
			if (_futures != null)
			{
				foreach (IFuture future in _futures)
				{
					FuturePool.Release(future);
				}
				ListPool<IFuture>.Release(_futures);
				_futures = null;
			}
		}

		void IFutureHandler.HandleFuture(IFuture future)
		{
			if (future.IsRejected)
			{
				Reject(future.Reason);
			}
			else
			{
				TryResolve();
			}
		}

		private void TryResolve()
		{
			foreach (IFuture future in _futures)
			{
				if (!future.IsResolved)
				{
					return;
				}
			}
			Resolve();
		}
	}
}