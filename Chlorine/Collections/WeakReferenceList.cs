using System;
using System.Collections;
using System.Collections.Generic;
using Chlorine.Pools;

namespace Chlorine.Collections
{
	public class WeakReferenceList<T> : ICollection<T>
			where T : class
	{
		private readonly List<WeakReference<T>> _list;
		private readonly List<WeakReference<T>> _overdueCache = new List<WeakReference<T>>();

		public WeakReferenceList()
		{
			_list = new List<WeakReference<T>>();
		}

		public WeakReferenceList(int capacity)
		{
			_list = new List<WeakReference<T>>(capacity);
		}

		public bool IsEmpty => _list.Count == 0;
		public bool IsReadOnly => false;

		public int Count => _list.Count;

		public Enumerator GetEnumerator()
		{
			return new Enumerator(_list);
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
			foreach (WeakReference<T> reference in _list)
			{
				WeakReferencePool.Release(reference);
			}
			_overdueCache.Clear();
			_list.Clear();
		}

		public bool Contains(T target)
		{
			return TryFind(target, out int _, out WeakReference<T> _);
		}

		public void Add(T target)
		{
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}
			if (!TryFind(target, out int _, out WeakReference<T> _))
			{
				_list.Add(WeakReferencePool.Pull(target));
			}
			ReleaseOverdue();
		}

		public bool Remove(T target)
		{
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}
			bool found = TryFind(target, out int index, out WeakReference<T> reference);
			if (found)
			{
				_list.RemoveAt(index);
				WeakReferencePool.Release(reference);
			}
			ReleaseOverdue();
			return found;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			foreach (T target in this)
			{
				array[arrayIndex] = target;
				arrayIndex++;
			}
		}

		private bool TryFind(T target, out int index, out WeakReference<T> reference)
		{
			if (_overdueCache.Count > 0)
			{
				_overdueCache.Clear();
			}
			for (int i = 0; i < _list.Count; i++)
			{
				WeakReference<T> current = _list[i];
				if (current.TryGetTarget(out T currentTarget))
				{
					if (target == currentTarget)
					{
						index = i;
						reference = current;
						return true;
					}
				}
				else
				{
					_overdueCache.Add(current);
				}
			}
			index = -1;
			reference = default;
			return false;
		}

		private void ReleaseOverdue()
		{
			if (_overdueCache.Count > 0)
			{
				foreach (WeakReference<T> overdueReference in _overdueCache)
				{
					_list.Remove(overdueReference);
					WeakReferencePool.Release(overdueReference);
				}
				_overdueCache.Clear();
			}
		}

		public struct Enumerator : IEnumerator<T>
		{
			private List<WeakReference<T>>.Enumerator _enumerator;
			private T _current;

			internal Enumerator(List<WeakReference<T>> list)
			{
				_enumerator = list.GetEnumerator();
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