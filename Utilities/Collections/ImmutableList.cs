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
	/// An immutable list.
	/// </summary>
	/// <typeparam name="T">Type of elements in the list.</typeparam>
	public sealed partial class ImmutableList<T> : IImmutableList<T>
	{
		#region Consts

		private const int NOT_FOUND = -1;
		#endregion

		#region Fields

		private static readonly IImmutableList<T> empty = new ImmutableList<T>.EmptyList();

		private readonly IImmutableList<T> tail;
		#endregion

		#region Properties

		/// <summary>
		/// Gets the item in the given index.
		/// </summary>
		/// <param name="index">The index for that item.</param>
		/// <returns>The item which is in the given index</returns>
		public T this[int index] => ListAt(index).Element; 

		/// <summary>
		/// Gets an empty list.
		/// </summary>
		public static IImmutableList<T> Empty => empty;

		/// <summary>
		/// Gets the count of items in the list.
		/// </summary>
		public int Count { get; }

		/// <summary>
		/// Gets the last element in the collection.
		/// </summary>
		public T Element { get; }

		/// <summary>
		/// Gets true if the collection is emptry; otherwise, false.
		/// </summary>
		public bool IsEmpty => false;
		#endregion

		#region Ctors

		/// <summary>
		/// Creates new immutable list.
		/// </summary>
		/// <param name="item">An item to add to the list.</param>
		public ImmutableList(T item) : this(item, Empty) { }

		private ImmutableList(T item, IImmutableList<T> tail)
		{
			Debug.Assert(tail != null, nameof(tail) + " is null.");

			this.Element = item;
			this.tail= tail;
			this.Count = tail.Count + 1;
		}
		#endregion

		#region Methods

		/// <summary>
		/// Returns the index of the given item, or -1 if it couldn't be found.
		/// </summary>
		/// <param name="item">The item to search in the list.</param>
		/// <returns>The index of item in the list, or -1 if it couldn't be found.</returns>
		public int IndexOf(T item)
		{
			int index = NOT_FOUND;

			foreach (var element in this.Select((Item, Index) => new { Item, Index }))
			{
				if (object.Equals(item, element.Item))
				{
					index = element.Index;
					break;
				}
			}

			return index;
		}

		/// <summary>
		/// Inserts the given item in the given index.
		/// </summary>
		/// <param name="item">The item to insert.</param>
		/// <param name="index">The index which the item would be inserted to.</param>
		/// <returns>New list with the item inserted in the given index.</returns>
		public IImmutableList<T> Insert(T item, int index)
		{
			if (index < 0 || index > this.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			IImmutableList<T> listAtIndex = ListAt(index - 1);
			var newList = new ImmutableList<T>(item, listAtIndex);

			return CopyDifferntial(newList, this, listAtIndex); 
		}

		private ImmutableList<T> CopyDifferntial(IImmutableList<T> target, ImmutableList<T> source, IImmutableList<T> startFrom)
		{
			Debug.Assert(target != null, nameof(target) + " is null.");
			Debug.Assert(startFrom != null, nameof(startFrom) + " is null.");

			var addedValues = new Stack<IImmutableList<T>>();

			while ((source != null) && !object.ReferenceEquals(source, startFrom))
			{
				addedValues.Push(source);
				source = source.tail as ImmutableList<T>;
			}

			while (addedValues.Any())
			{
				target = new ImmutableList<T>(addedValues.Pop().Element, target);
			}

			return target as ImmutableList<T>;
		}

		internal IImmutableList<T> ListAt(int index)
		{
			Debug.Assert((index >= 0) && (index < this.Count));

			ImmutableList<T> current = this;

			while ((current != null) && (index != current.Count - 1))
			{
				current = current.tail as ImmutableList<T>;
			}

			return current;
		}

		/// <summary>
		/// Removes an item from the given index.
		/// </summary>
		/// <param name="index">The index from which the item would be removed.</param>
		/// <returns>New list with the item removed from the given index.</returns>
		public IImmutableList<T> RemoveAt(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			IImmutableList<T> result;

			if (this.Count == 1)
			{
				result = Empty;
			}
			else
			{
				var notToCopy = ListAt(index + 1) as ImmutableList<T>;
				IImmutableList<T> lowerIndices = notToCopy.ListAt(index - 1);

				result = CopyDifferntial(lowerIndices, this, notToCopy);
			}

			return result;
		}

		/// <summary>
		/// Adds new item to the list.
		/// </summary>
		/// <param name="item">The item to add to the list.</param>
		/// <returns>New list which contains all the item in the current list, and the added item.</returns>
		public IImmutableList<T> Add(T item) =>
			new ImmutableList<T>(item, this);

		/// <summary>
		/// Returns value which indicates if the given item is contained in the collection.
		/// </summary>
		/// <param name="item">The item to seach in the collection.</param>
		/// <returns>true if the item contained in the collection; otherwise, false.</returns>
		public bool Contains(T item) =>
			IndexOf(item) != NOT_FOUND;

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
		/// Sets the item in the given index.
		/// </summary>
		/// <param name="index">The index in which the item would be replaced.</param>
		/// <param name="item">The item to replace with item in the given index.</param>
		/// <returns>New list with the item in the given index replaced with the given one.</returns>
		public IImmutableList<T> SetItem(int index, T item)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(index));
			}

			ImmutableList<T> notToCopy = ListAt(index + 1) as ImmutableList<T>;
			IImmutableList<T> newList = notToCopy.ListAt(index - 1);
			newList = newList.Add(item);

			return CopyDifferntial(newList, this, notToCopy);
		}

		/// <summary>
		/// Removes the first instacne of the given item from the list, and returning a value which indicates if the item was removed.
		/// </summary>
		/// <param name="item">The item to remove from the list.</param>
		/// <param name="didRemoved">OUT: On return, contains true if the item was removed from the list; otherwise, contains false.</param>
		/// <returns>New list which doesn't contain that item.</returns>
		public IImmutableList<T> Remove(T item, out bool didRemoved)
		{
			int itemIndex = IndexOf(item);
			didRemoved = itemIndex != NOT_FOUND;
			IImmutableList<T> result = this;

			if (didRemoved)
			{
				result = RemoveAt(itemIndex);
			}

			return result;
		}

		/// <summary>
		/// Removes the first instacne of the given item from the list.
		/// </summary>
		/// <param name="item">The item to remove from the list.</param>
		/// <returns>New list which doesn't contain that item.</returns>
		public IImmutableList<T> Remove(T item)
		{
			bool stub;
			return Remove(item, out stub);
		}

		/// <summary>
		/// Removes the last item from the list.
		/// </summary>
		/// <returns>New list which doesn't contain that item.</returns>
		public IImmutableList<T> Remove() =>
			this.tail;

		/// <summary>
		/// Returns an enumerator to enumerate the items in the list.
		/// </summary>
		/// <returns>An enumerator to enumerate the items in the list.</returns>
		public IEnumerator<T> GetEnumerator()
		{
			foreach (T item in this.tail)
			{
				yield return item;
			}

			yield return this.Element;
		}

		IEnumerator IEnumerable.GetEnumerator() =>
			GetEnumerator();

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
