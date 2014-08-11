using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Collections
{
	public partial class ImmutableList<T>
	{
		private class EmptyList : IImmutableList<T>
		{
			#region Properties

			public int Count
			{
				get { return 0; }
			}

			public T this[int index]
			{
				get { throw new ArgumentOutOfRangeException("index"); }
			}

			public T Element { get { throw new InvalidOperationException("List is empty"); } }

			public bool IsEmpty { get { return true; } }
			#endregion

			#region Methods

			public int IndexOf(T item)
			{
				return NOT_FOUND;
			}

			public IImmutableList<T> Insert(T item, int index)
			{
				if (index != 0)
				{
					throw new ArgumentOutOfRangeException("index");
				}

				return new ImmutableList<T>(item);
			}

			public IImmutableList<T> RemoveAt(int index)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			public IImmutableList<T> Add(T item)
			{
				return Insert(item, 0);
			}

			public bool Contains(T item)
			{
				return false;
			}

			public void CopyTo(T[] array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException("array");
				}

				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException("arrayIndex");
				}

				if (array.Length - arrayIndex < 0)
				{
					throw new ArgumentException("Array isn't big enough");
				}
			}

			public IImmutableList<T> Remove(T item, out bool didRemoved)
			{
				didRemoved = false;
				return this;
			}

			public IImmutableList<T> Remove()
			{
				throw new InvalidOperationException("List is empty");
			}

			public IImmutableList<T> Remove(T item)
			{
				bool stub;
				return Remove(item, out stub);
			}

			public IEnumerator<T> GetEnumerator()
			{
				yield break;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public IImmutableList<T> SetItem(int index, T item)
			{
				throw new ArgumentOutOfRangeException("index");
			}

			IImmutableCollection<T> IImmutableCollection<T>.Add(T item)
			{
				return Add(item);
			}

			IImmutableCollection<T> IImmutableCollection<T>.Remove()
			{
				return Remove();
			}

			IImmutableCollection<T> IImmutableCollection<T>.Remove(T item)
			{
				return Remove(item);
			}

			IImmutableCollection<T> IImmutableCollection<T>.Remove(T item, out bool didRemoved)
			{
				return Remove(item, out didRemoved);
			}
			#endregion
		}
	}
}
