using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Collections
{
	/// <summary>
	/// An immutable stack.
	/// </summary>
	/// <typeparam name="T">Type of elements in the stack.</typeparam>
	public partial class ImmutableStack<T> : IImmutableStack<T>
	{
		#region Fields

		private static readonly IImmutableStack<T> empty = new ImmutableStack<T>.EmptyStack();

		private readonly IImmutableList<T> itemStore;
		#endregion

		#region Properties

		/// <summary>
		/// Gets true if the collection is emptry; otherwise, false.
		/// </summary>
		public bool IsEmpty => false;

		/// <summary>
		/// Gets the count of items in the stack.
		/// </summary>
		public int Count => this.itemStore.Count;

		/// <summary>
		/// Gets an empty stack.
		/// </summary>
		public static IImmutableStack<T> Empty => empty;
		#endregion

		#region Ctors

		/// <summary>
		/// Create new stack which contains a single item.
		/// </summary>
		/// <param name="item">The item for the stack.</param>
		public ImmutableStack(T item) : this(new ImmutableList<T>(item)) { }

		private ImmutableStack(IImmutableList<T> items)
		{
			Debug.Assert(!items.IsEmpty);

			this.itemStore = items;
		}
		#endregion

		#region Methods

		/// <summary>
		/// Peeks at the top item.
		/// </summary>
		/// <returns>The top item.</returns>
		public T Peek() =>
			this.itemStore.Element;

		/// <summary>
		/// Pops the top item.
		/// </summary>
		/// <returns>New stack, with the top popped.</returns>
		public IImmutableStack<T> Pop()
		{
			IImmutableList<T> removedList = this.itemStore.Remove();

			return removedList.IsEmpty
				? Empty
				: new ImmutableStack<T>(removedList);
		}

		/// <summary>
		/// Push the given item to the top of the stack.
		/// </summary>
		/// <param name="item">The item to push the the top of the stack.</param>
		/// <returns>New stack, with the given item pushed to its top.</returns>
		public IImmutableStack<T> Push(T item) =>
			new ImmutableStack<T>(this.itemStore.Add(item));

		/// <summary>
		/// Copies the content of the collection to the given array, to the given array index.
		/// </summary>
		/// <param name="array">The array to copy the content of the collection into.</param>
		/// <param name="arrayIndex">The index in the array, from which item would be copied.</param>
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

			foreach (var element in this.Select((Item, Index) => new { Item, Index }))
			{
				array[element.Index + arrayIndex] = element.Item;
			}
		}

		/// <summary>
		/// Returns value which indicates if the given item is contained in the collection.
		/// </summary>
		/// <param name="item">The item to seach in the collection.</param>
		/// <returns>true if the item contained in the collection; otherwise, false.</returns>
		public bool Contains(T item) =>
			this.itemStore.Contains(item);

		/// <summary>
		/// Returns an enumerator to enumerate the items in the stack.
		/// </summary>
		/// <returns>An enumerator to enumerate the items in the stack.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			for (var concrete = this.itemStore as ImmutableList<T>;
				concrete != null;
				concrete = concrete.ListAt(concrete.Count - 1) as ImmutableList<T>)
			{
				yield return concrete.Element;
			}
		}

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();
		#endregion
	}
}
