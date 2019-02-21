using System;
using System.Collections;
using System.Collections.Generic;

namespace Chlorine
{
	public class WeakList<T> : IEnumerable<T>
			where T : class
	{
		private readonly Pool<WeakReference<T>> _referencePool = new Pool<WeakReference<T>>();
		private readonly List<WeakReference<T>> _references = new List<WeakReference<T>>();

		public int Count => _references.Count;

		public Enumerator GetEnumerator()
		{
			return new Enumerator(_references.GetEnumerator());
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Clear()
		{
			foreach (WeakReference<T> reference in _references)
			{
				ReleaseReference(reference);
			}
			_references.Clear();
		}

		public void Add(T target)
		{
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}
			_references.Add(CreateReference(target));
		}

		public bool Remove(T target)
		{
			int index = IndexOf(target);
			if (index < 0)
			{
				return false;
			}
			_references.RemoveAt(index);
			return true;
		}

		private int IndexOf(T target)
		{
			for (int i = 0; i < _references.Count; i++)
			{
				WeakReference<T> reference = _references[i];
				if (reference.TryGetTarget(out T referenceTarget) && target == referenceTarget)
				{
					return i;
				}
			}
			return -1;
		}

		private WeakReference<T> CreateReference(T target)
		{
			if (_referencePool.IsEmpty)
			{
				return new WeakReference<T>(target);
			}
			WeakReference<T> reference = _referencePool.Pull();
			reference.SetTarget(target);
			return reference;
		}

		private void ReleaseReference(WeakReference<T> reference)
		{
			reference.SetTarget(null);
			_referencePool.Release(reference);
		}

		public struct Enumerator : IEnumerator<T>
		{
			private List<WeakReference<T>>.Enumerator _enumerator;
			private T _current;

			internal Enumerator(List<WeakReference<T>>.Enumerator enumerator)
			{
				_enumerator = enumerator;
				_current = default;
			}

			public void Dispose()
			{
				_enumerator.Dispose();
			}

			public bool MoveNext()
			{
				while (_enumerator.MoveNext())
				{
					if (_enumerator.Current.TryGetTarget(out _current))
					{
						return true;
					}
				}
				_current = default;
				return false;
			}

			public T Current => _current;

			object IEnumerator.Current => _current;

			void IEnumerator.Reset()
			{
				((IEnumerator)_enumerator).Reset();
				_current = null;
			}
		}
	}
}