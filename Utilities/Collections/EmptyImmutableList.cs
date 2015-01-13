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
		private sealed class EmptyList : IImmutableList<T>
		{
			#region Properties

			public int Count => 0;

			public T this[int index]
			{
				get { throw new ArgumentOutOfRangeException("index"); }
			}

			public T Element { get { throw new InvalidOperationException("List is empty"); } }

			public bool IsEmpty => true;
			#endregion

			#region Methods

			public int IndexOf(T item) => NOT_FOUND;

			public IImmutableList<T> Insert(T item, int index)
			{
				if (index != 0)
				{
					throw new ArgumentOutOfRangeException(nameof(index));
				}

				return new ImmutableList<T>(item);
			}

			public IImmutableList<T> RemoveAt(int index)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			public IImmutableList<T> Add(T item) => Insert(item, 0);

			public bool Contains(T item) => false;

			public void CopyTo(T[] array, int arrayIndex)
			{
				if (array == null)
				{
					throw new ArgumentNullException(nameof(array));
				}

				if (arrayIndex < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(arrayIndex));
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

			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			public IImmutableList<T> SetItem(int index, T item)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			IImmutableCollection<T> IImmutableCollection<T>.Add(T item) =>
				Add(item);

			IImmutableCollection<T> IImmutableCollection<T>.Remove() =>
				Remove();

			IImmutableCollection<T> IImmutableCollection<T>.Remove(T item) =>
				Remove(item);

			IImmutableCollection<T> IImmutableCollection<T>.Remove(T item, out bool didRemoved) =>
				Remove(item, out didRemoved);
			#endregion
		}
	}
}
