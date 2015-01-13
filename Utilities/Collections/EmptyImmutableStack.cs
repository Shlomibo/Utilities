using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Collections
{
	public partial class ImmutableStack<T>
	{
		private sealed class EmptyStack : IImmutableStack<T>
		{
			#region Properties

			public bool IsEmpty => true;

			public int Count => 0;
			#endregion

			#region Methods

			public T Peek()
			{
				throw new InvalidOperationException("Stack is empty");
			}

			public IImmutableStack<T> Pop()
			{
				throw new InvalidOperationException("Stack is empty");
			}

			public IImmutableStack<T> Push(T item)
			{
				return new ImmutableStack<T>(item);
			}

			public bool Contains(T item) =>
				false;

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

			public IEnumerator<T> GetEnumerator()
			{
				yield break;
			}

			IEnumerator IEnumerable.GetEnumerator() =>
				GetEnumerator();
			#endregion
		}
	}
}
