using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extansions.Object;

namespace Utilities.Extansions.Enumerable
{
	/// <summary>
	/// Extansions methods for enumerables
	/// </summary>
	public static class EnumerableExtansions
	{
		/// <summary>
		/// Returns true if the enumerable is null, or empty.
		/// </summary>
		/// <typeparam name="T">The type if items in the enumerable.</typeparam>
		/// <param name="enumerable">The enumerable to check.</param>
		/// <returns>true if the enumerable is null, or if it contains no items; otherwise false.</returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
		{
			return (enumerable == null) || !enumerable.Any();
		}

		/// <summary>
		/// Executes the action for each element in the enumerable.
		/// </summary>
		/// <typeparam name="T">The type of items in the enumerable.</typeparam>
		/// <param name="enumerable">The enumerable which contains the item.</param>
		/// <param name="action">The action to execute for each item.</param>
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			enumerable.ThrowWhen(
				when: sequence => sequence == null,
				what: new ArgumentNullException("enumerable"));

			foreach (T item in enumerable)
			{
				action(item);
			}
		}

		/// <summary>
		/// Returns the list as IReadOnlyList&lt;T&gt;, wrapping it if necessary.
		/// </summary>
		/// <typeparam name="T">The typeparam of the list</typeparam>
		/// <param name="list">The list to return as IReadOnlyList&lt;T&gt;</param>
		/// <returns>An object which implements IReadOnlyList&lt;T&gt; for the list.</returns>
		public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list)
		{
			return (list as IReadOnlyList<T>) ?? new ListWrapper<T>(list);
		}

		/// <summary>
		/// Returns the collection as IReadOnlyCollection&lt;T&gt;, wrapping it if necessary.
		/// </summary>
		/// <typeparam name="T">The typeparam of the collection</typeparam>
		/// <param name="collection">The collection to return as IReadOnlyList&lt;T&gt;</param>
		/// <returns>An object which implements IReadOnlyCollection&lt;T&gt; for the collection.</returns>
		public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection)
		{
			return (collection as IReadOnlyCollection<T>) ?? new CollectionWrapper<T>(collection);
		}

		/// <summary>
		/// Returns the dictionary as IReadOnlyDictionary&lt;T&gt;, wrapping it if necessary.
		/// </summary>
		/// <typeparam name="TKey">The type of the dictionary's keys.</typeparam>
		/// <typeparam name="TValue">The type of the dictionary's values.</typeparam>
		/// <param name="dictionary">The dictionary to return as IReadOnlyList&lt;T&gt;</param>
		/// <returns>An object which implements IReadOnlyDictionary&lt;T&gt; for the dictionary.</returns>
		public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(
			this IDictionary<TKey, TValue> dictionary) 
		{
			return (dictionary as IReadOnlyDictionary<TKey, TValue>) ??
				new DictionaryWrapper<TKey, TValue>(dictionary);
		}

		private class CollectionWrapper<T> : IReadOnlyCollection<T>
		{
			#region Fields

			private ICollection<T> collection;
			#endregion

			#region Properties

			public int Count
			{
				get { return this.collection.Count; }
			} 
			#endregion

			#region Ctor

			public CollectionWrapper(ICollection<T> collection)
			{
				if (collection == null)
				{
					throw new ArgumentNullException();
				}

				this.collection = collection;
			}
			#endregion

			#region Methods

			public IEnumerator<T> GetEnumerator()
			{
				return this.collection.GetEnumerator();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			} 
			#endregion
		}


		private class ListWrapper<T> : CollectionWrapper<T>, IReadOnlyList<T>
		{
			#region Fields

			private IList<T> list;
			#endregion

			#region Properties

			public T this[int index]
			{
				get { return this.list[index]; }
			}
			#endregion

			#region Ctor

			public ListWrapper(IList<T> list)
				: base(list)
			{
				if (list == null)
				{
					throw new ArgumentNullException();
				}

				this.list = list;
			}
			#endregion
		}

		private class DictionaryWrapper<TKey, TValue> : 
			CollectionWrapper<KeyValuePair<TKey, TValue>>,
			IReadOnlyDictionary<TKey, TValue>
		{
			#region Fields

			private IDictionary<TKey, TValue> dictionary;
			#endregion

			#region Properties

			public IEnumerable<TKey> Keys
			{
				get { return this.dictionary.Keys; }
			}

			public IEnumerable<TValue> Values
			{
				get { return this.dictionary.Values; }
			}

			public TValue this[TKey key]
			{
				get { return this.dictionary[key]; }
			}
			#endregion

			#region Ctor

			public DictionaryWrapper(IDictionary<TKey, TValue> dictionary)
				: base(dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException();
				}

				this.dictionary = dictionary;
			}
			#endregion

			#region Methods

			public bool ContainsKey(TKey key)
			{
				return this.dictionary.ContainsKey(key);
			}

			public bool TryGetValue(TKey key, out TValue value)
			{
				return this.dictionary.TryGetValue(key, out value);
			}
			#endregion
		}
	}
}
