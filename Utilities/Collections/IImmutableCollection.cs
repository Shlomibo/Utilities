using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Collections
{
	/// <summary>
	/// An interface for immutable collection. ie: A collection that any mutating operation on it, returns a new collection, instead of changing it.
	/// </summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	public interface IImmutableCollection<T> : IReadOnlyCollection<T>
	{
		#region Properties

		/// <summary>
		/// Gets true if the collection is emptry; otherwise, false.
		/// </summary>
		bool IsEmpty { get; }

		/// <summary>
		/// Gets the last element in the collection.
		/// </summary>
		T Element { get; } 
		#endregion

		#region Methods

		/// <summary>
		/// Returns value which indicates if the given item is contained in the collection.
		/// </summary>
		/// <param name="item">The item to seach in the collection.</param>
		/// <returns>true if the item contained in the collection; otherwise, false.</returns>
		bool Contains(T item);

		/// <summary>
		/// Copies the content of the collection to the given array, to the given array index.
		/// </summary>
		/// <param name="array">The array to copy the content of the collection into.</param>
		/// <param name="arrayIndex">The index in the array, from which item would be copied.</param>
		void CopyTo(T[] array, int arrayIndex);

		/// <summary>
		/// Adds new item to the collection.
		/// </summary>
		/// <param name="item">The item to add to the collection.</param>
		/// <returns>New collection which contains all the item in the current collection, and the added item.</returns>
		IImmutableCollection<T> Add(T item);

		/// <summary>
		/// Removes the last item from the collection.
		/// </summary>
		/// <returns>New collection which doesn't contain that item.</returns>
		IImmutableCollection<T> Remove();

		/// <summary>
		/// Removes the first instacne of the given item from the collection.
		/// </summary>
		/// <param name="item">The item to remove from the collection.</param>
		/// <returns>New collection which doesn't contain that item.</returns>
		IImmutableCollection<T> Remove(T item);

		/// <summary>
		/// Removes the first instacne of the given item from the collection, and returning a value which indicates if the item was removed.
		/// </summary>
		/// <param name="item">The item to remove from the collection.</param>
		/// <param name="didRemoved">OUT: On return, contains true if the item was removed from the collection; otherwise, contains false.</param>
		/// <returns>New collection which doesn't contain that item.</returns>
		IImmutableCollection<T> Remove(T item, out bool didRemoved); 
		#endregion
	}
}
