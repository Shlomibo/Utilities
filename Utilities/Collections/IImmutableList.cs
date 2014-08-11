using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Collections
{
	/// <summary>
	/// An interface for immutable list. ie: A list that any mutating operation on it, returns a new list, instead of changing it.
	/// </summary>
	/// <typeparam name="T">The type of elements in the list.</typeparam>
	public interface IImmutableList<T> : IImmutableCollection<T>, IReadOnlyList<T>
	{
		/// <summary>
		/// Returns the index of the given item, or -1 if it couldn't be found.
		/// </summary>
		/// <param name="item">The item to search in the list.</param>
		/// <returns>The index of item in the list, or -1 if it couldn't be found.</returns>
		int IndexOf(T item);

		/// <summary>
		/// Inserts the given item in the given index.
		/// </summary>
		/// <param name="item">The item to insert.</param>
		/// <param name="index">The index which the item would be inserted to.</param>
		/// <returns>New list with the item inserted in the given index.</returns>
		IImmutableList<T> Insert(T item, int index);

		/// <summary>
		/// Removes an item from the given index.
		/// </summary>
		/// <param name="index">The index from which the item would be removed.</param>
		/// <returns>New list with the item removed from the given index.</returns>
		IImmutableList<T> RemoveAt(int index);

		/// <summary>
		/// Sets the item in the given index.
		/// </summary>
		/// <param name="index">The index in which the item would be replaced.</param>
		/// <param name="item">The item to replace with item in the given index.</param>
		/// <returns>New list with the item in the given index replaced with the given one.</returns>
		IImmutableList<T> SetItem(int index, T item);

		/// <summary>
		/// Adds new item to the list.
		/// </summary>
		/// <param name="item">The item to add to the list.</param>
		/// <returns>New list which contains all the item in the current list, and the added item.</returns>
		new IImmutableList<T> Add(T item);

		/// <summary>
		/// Removes the last item from the list.
		/// </summary>
		/// <returns>New list which doesn't contain that item.</returns>
		new IImmutableList<T> Remove();

		/// <summary>
		/// Removes the first instacne of the given item from the list.
		/// </summary>
		/// <param name="item">The item to remove from the list.</param>
		/// <returns>New list which doesn't contain that item.</returns>
		new IImmutableList<T> Remove(T item);

		/// <summary>
		/// Removes the first instacne of the given item from the list, and returning a value which indicates if the item was removed.
		/// </summary>
		/// <param name="item">The item to remove from the list.</param>
		/// <param name="didRemoved">OUT: On return, contains true if the item was removed from the list; otherwise, contains false.</param>
		/// <returns>New list which doesn't contain that item.</returns>
		new IImmutableList<T> Remove(T item, out bool didRemoved);
	}
}
