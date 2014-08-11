using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Collections
{
	/// <summary>
	/// An interface for immutable stack. ie: A stack that any mutating operation on it, returns a new stack, instead of changing it.
	/// </summary>
	/// <typeparam name="T">The type of elements in the list.</typeparam>
	public interface IImmutableStack<T> : IEnumerable<T>
	{
		#region Properties

		/// <summary>
		/// Gets true if the collection is emptry; otherwise, false.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Gets the count of items in the stack.
		/// </summary>
		int Count { get; }
		#endregion

		/// <summary>
		/// Peeks at the top item.
		/// </summary>
		/// <returns>The top item.</returns>
		T Peek();

		/// <summary>
		/// Pops the top item.
		/// </summary>
		/// <returns>New stack, with the top popped.</returns>
		IImmutableStack<T> Pop();

		/// <summary>
		/// Push the given item to the top of the stack.
		/// </summary>
		/// <param name="item">The item to push the the top of the stack.</param>
		/// <returns>New stack, with the given item pushed to its top.</returns>
		IImmutableStack<T> Push(T item);

		/// <summary>
		/// Copies the content of the collection to the given array, to the given array index.
		/// </summary>
		/// <param name="array">The array to copy the content of the collection into.</param>
		/// <param name="arrayIndex">The index in the array, from which item would be copied.</param>
		void CopyTo(T[] array, int arrayIndex);

		/// <summary>
		/// Returns value which indicates if the given item is contained in the collection.
		/// </summary>
		/// <param name="item">The item to seach in the collection.</param>
		/// <returns>true if the item contained in the collection; otherwise, false.</returns>
		bool Contains(T item);
	}
}
