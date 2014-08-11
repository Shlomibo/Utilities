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

		/// <summary>
		/// Run for each element in collection, the given action.
		/// </summary>
		/// <typeparam name="T">The type of elements in collection.</typeparam>
		/// <param name="collection">The collection of elements to run their corresponding actions.</param>
		/// <param name="operation">The action to run on each element in collection.</param>
		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> operation)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}

			if (operation == null)
			{
				throw new ArgumentNullException("operation");
			}

			collection.ForEach(collection.Select(item => (Action<T>)(arg => operation(arg))));
		}

		/// <summary>
		/// Run for each element in collection, the corresponding action in operations.  
		/// <remarks>
		/// If operations has fewer elements than collection, only items which have corresponding action would run.  
		/// null actions are treated as `do nothing` actions.
		/// </remarks>
		/// </summary>
		/// <typeparam name="T">The type of elements in collection.</typeparam>
		/// <param name="collection">The collection of elements to run their corresponding actions.</param>
		/// <param name="operations">The collection of actions to run.</param>
		public static void ForEach<T>(this IEnumerable<T> collection, IEnumerable<Action<T>> operations)
		{
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}

			if (operations == null)
			{
				throw new ArgumentNullException("operations");
			}

			operations = operations.Select(action => action ?? (item => { }));

			foreach (var operation in operations.Zip(collection, (Action, Item) => new { Action, Item }))
			{
				operation.Action(operation.Item);
			}
		}

		/// <summary>
		/// Returns a collection of pair which hold reference to the current and previous items.
		/// </summary>
		/// <typeparam name="T">The type of elements in source.</typeparam>
		/// <param name="source">The source sequence.</param>
		/// <returns>A collection of pair which hold reference to the current and previous items.</returns>
		public static IEnumerable<Pair<T>> GetSequencials<T>(this IEnumerable<T> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return GetSequencialsInternal(source);
		}

		private static IEnumerable<Pair<T>> GetSequencialsInternal<T>(IEnumerable<T> source)
		{
			T previous = default(T);

			foreach (T item in source)
			{
				T current = item;
				yield return new Pair<T>(current, previous);
				previous = current;
			}
		}

		/// <summary>
		/// Run for each element in collection, the corresponding action in operations.  
		/// </summary>
		/// <typeparam name="T">The type of elements in collection.</typeparam>
		/// <param name="collection">The collection of elements to run their corresponding actions.</param>
		/// <param name="operations">The collection of actions to run.</param>
		public static void ForEach<T>(this IEnumerable<T> collection, params Action<T>[] operations)
		{
			collection.ForEach((IEnumerable<Action<T>>)operations);
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
