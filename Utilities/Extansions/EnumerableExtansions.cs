using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		/// Returns distinct elements from a sequence by using a specified delegates to compare values.
		/// </summary>
		/// <typeparam name="T">The type of the elements of source.</typeparam>
		/// <param name="source">The sequence to remove duplicate elements from</param>
		/// <param name="equalityComparer">A delegate to compare if two elements are equal.</param>
		/// <param name="hasher">A delegate tp calculate an element hash.</param>
		/// <returns>An IEnumerable&lt;T> that contains distinct elements from the source sequence.</returns>
		public static IEnumerable<T> Distinct<T>(
			this IEnumerable<T> source,
			EqualityComparerDelegate<T> equalityComparer,
			HashDelegate<T> hasher = null) =>
			source.Distinct(new DelegateEqualityComparer<T>(equalityComparer, hasher));

		/// <summary>
		/// Produces the set difference of two sequences by using the specified delegates to compare values.
		/// </summary>
		/// <typeparam name="T">The type of the elements of the input sequences.</typeparam>
		/// <param name="first">An IEnumerable&lt;T> whose elements that are not also insecond will be returned.</param>
		/// <param name="second">
		/// An IEnumerable&lt;T> whose elements that also occur in the first sequence will cause those elements to be removed from the returned sequence.
		/// </param>
		/// <param name="equalityComparer">A delegate to compare if two elements are equal.</param>
		/// <param name="hasher">A delegate tp calculate an element hash.</param>
		/// <returns>A sequence that contains the set difference of the elements of two sequences.</returns>
		public static IEnumerable<T> Except<T>(
			this IEnumerable<T> first,
			IEnumerable<T> second,
			EqualityComparerDelegate<T> equalityComparer,
			HashDelegate<T> hasher = null)
		{
			return first.Except(second, new DelegateEqualityComparer<T>(equalityComparer, hasher));
		}

		/// <summary>
		/// Groups the elements of a sequence according to a key selector function. 
		/// The keys are compared by using delegates and each group's elements are projected by using a specified function.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <typeparam name="TElement">The type of the elements in the IGrouping&lt;TKey, TElement>.</typeparam>
		/// <param name="source">An IEnumerable&lt;T> whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <param name="elementSelector">A function to map each source element to an element in an IGrouping&lt;TKey, TElement>.</param>
		/// <param name="keyComparer">A delegate to compare if two keys are equal.</param>
		/// <param name="keyHasher">A delegate tp calculate a key hash.</param>
		/// <returns>
		/// An IEnumerable&lt;IGrouping&lt;TKey, TElement>> in C# or IEnumerable(Of IGrouping(Of TKey, TElement)) in Visual Basic 
		/// where each IGrouping&lt;TKey, TElement> object contains a collection of objects of type TElement and a key.
		/// </returns>
		public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			Func<TSource, TElement> elementSelector, 
			EqualityComparerDelegate<TKey> keyComparer,
			HashDelegate<TKey> keyHasher = null)
		{
			return source.GroupBy(
				keySelector,
				elementSelector,
				new DelegateEqualityComparer<TKey>(keyComparer, keyHasher));
		}

		/// <summary>
		/// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. 
		/// The keys are compared by using a specified delegates.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
		/// <param name="source">An IEnumerable&lt;T> whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <param name="resultSelector">A function to create a result value from each group.</param>
		/// <param name="keyComparer">A delegate to compare if two keys are equal.</param>
		/// <param name="keyHasher">A delegate tp calculate a key hash.</param>
		/// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
		public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			Func<TKey, IEnumerable<TSource>, TResult> resultSelector, 
			EqualityComparerDelegate<TKey> keyComparer,
			HashDelegate<TKey> keyHasher = null)
		{
			return source.GroupBy(
				keySelector,
				resultSelector,
				new DelegateEqualityComparer<TKey>(keyComparer, keyHasher));
		}

		/// <summary>
		/// Groups the elements of a sequence according to a specified key selector function and creates a result value from each group and its key. 
		/// Key values are compared by using a specified delegates, and the elements of each group are projected by using a specified function.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <typeparam name="TElement">The type of the elements in each IGrouping&lt;TKey, TElement>.</typeparam>
		/// <typeparam name="TResult">The type of the result value returned by resultSelector.</typeparam>
		/// <param name="source">An IEnumerable&lt;T> whose elements to group.</param>
		/// <param name="keySelector">A function to extract the key for each element.</param>
		/// <param name="elementSelector">A function to map each source element to an element in an IGrouping&lt;TKey, TElement>.</param>
		/// <param name="resultSelector">A function to create a result value from each group.</param>
		/// <param name="keyComparer">A delegate to compare if two keys are equal.</param>
		/// <param name="keyHasher">A delegate tp calculate a key hash.</param>
		/// <returns>A collection of elements of type TResult where each element represents a projection over a group and its key.</returns>
		public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			Func<TSource, TElement> elementSelector, 
			Func<TKey, IEnumerable<TElement>, TResult> resultSelector, 
			EqualityComparerDelegate<TKey> keyComparer,
			HashDelegate<TKey> keyHasher = null)
		{
			return source.GroupBy(keySelector, elementSelector, resultSelector, new DelegateEqualityComparer<TKey>(keyComparer, keyHasher));
		}

		/// <summary>
		/// Correlates the elements of two sequences based on key equality and groups the results. 
		/// Specified delegates are used to compare keys.
		/// </summary>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
		/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
		/// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
		/// <param name="keyComparer">A delegate to compare if two keys are equal.</param>
		/// <param name="keyHasher">A delegate tp calculate a key hash.</param>
		/// <returns>An IEnumerable&lt;T> that contains elements of type TResult that are obtained by performing a grouped join on two sequences.</returns>
		public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
			this IEnumerable<TOuter> outer, 
			IEnumerable<TInner> inner, 
			Func<TOuter, TKey> outerKeySelector, 
			Func<TInner, TKey> innerKeySelector, 
			Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, 
			EqualityComparerDelegate<TKey> keyComparer,
			HashDelegate<TKey> keyHasher = null)
		{
			return outer.GroupJoin(
				inner,
				outerKeySelector,
				innerKeySelector,
				resultSelector,
				new DelegateEqualityComparer<TKey>(keyComparer, keyHasher));
		}

		/// <summary>
		/// Produces the set intersection of two sequences by using the specified delegates to compare values.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
		/// <param name="first">An IEnumerable&lt;T> whose distinct elements that also appear insecond will be returned.</param>
		/// <param name="second">An IEnumerable&lt;T> whose distinct elements that also appear in the first sequence will be returned.</param>
		/// <param name="equalityComparer">A delegate to compare if two elements are equal.</param>
		/// <param name="hasher">A delegate tp calculate an element hash.</param>
		/// <returns>A sequence that contains the elements that form the set intersection of two sequences.</returns>
		public static IEnumerable<TSource> Intersect<TSource>(
			this IEnumerable<TSource> first, 
			IEnumerable<TSource> second, 
			EqualityComparerDelegate<TSource> equalityComparer,
			HashDelegate<TSource> hasher = null)
		{
			return first.Intersect(
				second,
				new DelegateEqualityComparer<TSource>(equalityComparer, hasher));
		}

		/// <summary>
		/// Correlates the elements of two sequences based on matching keys. Specified delegate are used to compare keys.
		/// </summary>
		/// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
		/// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
		/// <typeparam name="TResult">The type of the result elements.</typeparam>
		/// <param name="outer">The first sequence to join.</param>
		/// <param name="inner">The sequence to join to the first sequence.</param>
		/// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
		/// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
		/// <param name="resultSelector">A function to create a result element from two matching elements.</param>
		/// <param name="keyComparer">A delegate to compare if two keys are equal.</param>
		/// <param name="keyHasher">A delegate tp calculate a key hash.</param>
		/// <returns>An IEnumerable&lt;T> that has elements of type TResult that are obtained by performing an inner join on two sequences.</returns>
		public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(
			this IEnumerable<TOuter> outer, 
			IEnumerable<TInner> inner, 
			Func<TOuter, TKey> outerKeySelector, 
			Func<TInner, TKey> innerKeySelector, 
			Func<TOuter, TInner, TResult> resultSelector, 
			EqualityComparerDelegate<TKey> keyComparer,
			HashDelegate<TKey> keyHasher = null)
		{
			return outer.Join(
				inner,
				outerKeySelector,
				innerKeySelector,
				resultSelector,
				new DelegateEqualityComparer<TKey>(keyComparer, keyHasher));
		}

		/// <summary>
		/// Sorts the elements of a sequence in ascending order by using a specified comparer delegate.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <param name="source">A sequence of values to order.</param>
		/// <param name="keySelector">A function to extract a key from an element.</param>
		/// <param name="comparer">A comparer delegate to compare keys.</param>
		/// <returns>An IOrderedEnumerable&lt;TElement> whose elements are sorted according to a key.</returns>
		public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			ComparerDelegate<TKey> comparer)
		{
			return source.OrderBy(keySelector, new DelegateComparer<TKey>(comparer));
		}

		/// <summary>
		/// Sorts the elements of a sequence in descending order by using a specified comparer delegate.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <param name="source">A sequence of values to order.</param>
		/// <param name="keySelector">A function to extract a key from an element.</param>
		/// <param name="comparer">A comparer delegate to compare keys.</param>
		/// <returns>An IOrderedEnumerable&lt;TElement> whose elements are sorted in descending order according to a key.</returns>
		public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(
			this IEnumerable<TSource> source,
			Func<TSource, TKey> keySelector,
			ComparerDelegate<TKey> comparer)
		{
			return source.OrderByDescending(keySelector, new DelegateComparer<TKey>(comparer));
		}

		/// <summary>
		/// Determines whether two sequences are equal by comparing their elements by using the specified delegates.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
		/// <param name="first">An IEnumerable&lt;T> to compare tosecond.</param>
		/// <param name="second">An IEnumerable&lt;T> to compare to the first sequence.</param>
		/// <param name="equalityComparer">A delegate to compare if two elements are equal.</param>
		/// <param name="hasher">A delegate tp calculate an element hash.</param>
		/// <returns>true if the two source sequences are of equal length and their corresponding elements compare equal according to comparer; otherwise, false.</returns>
		public static bool SequenceEqual<TSource>(
			this IEnumerable<TSource> first, 
			IEnumerable<TSource> second, 
			EqualityComparerDelegate<TSource> equalityComparer,
			HashDelegate<TSource> hasher = null)
		{
			return first.SequenceEqual(second, new DelegateEqualityComparer<TSource>(equalityComparer, hasher));
		}

		/// <summary>
		/// Performs a subsequent ordering of the elements in a sequence in ascending order by using a specified comparer delegate.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <param name="source">An IOrderedEnumerable&lt;TSource> that contains elements to sort.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="comparer">A comparer delegate to compare keys.</param>
		/// <returns>An IOrderedEnumerable&lt;TSource> whose elements are sorted according to a key.</returns>
		public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(
			this IOrderedEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			ComparerDelegate<TKey> comparer)
		{
			return source.ThenBy(keySelector, new DelegateComparer<TKey>(comparer));
		}

		/// <summary>
		/// Performs a subsequent ordering of the elements in a sequence in descending order by using a specified comparer delegate.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <param name="source">An IOrderedEnumerable&lt;TSource> that contains elements to sort.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="comparer">A comparer delegate to compare keys.</param>
		/// <returns>An IOrderedEnumerable&lt;TSource> whose elements are sorted in descending order according to a key.</returns>
		public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(
			this IOrderedEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			ComparerDelegate<TKey> comparer)
		{
			return source.ThenByDescending(keySelector, new DelegateComparer<TKey>(comparer));
		}

		/// <summary>
		/// Creates a Dictionary&lt;TKey, TValue> from an IEnumerable&lt;T> according to a specified key selector function and key comparison delegates.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the keys returned by keySelector.</typeparam>
		/// <param name="source">An IEnumerable&lt;T> to create a Dictionary&lt;TKey, TValue> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="keyComparer">A delegate to compare if two keys are equal.</param>
		/// <param name="keyHasher">A delegate tp calculate a key hash.</param>
		/// <returns>A Dictionary&lt;TKey, TValue> that contains keys and values.</returns>
		public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			EqualityComparerDelegate<TKey> keyComparer,
			HashDelegate<TKey> keyHasher = null)
		{
			return source.ToDictionary(keySelector, new DelegateEqualityComparer<TKey>(keyComparer, keyHasher));
		}

		/// <summary>
		/// Creates a Dictionary&lt;TKey, TValue> from an IEnumerable&lt;T> according to a specified key selector function, a comparison delegates, and an element selector function.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
		/// <param name="source">An IEnumerable&lt;T> to create a Dictionary&lt;TKey, TValue> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
		/// <param name="keyComparer">A delegate to compare if two keys are equal.</param>
		/// <param name="keyHasher">A delegate tp calculate a key hash.</param>
		/// <returns>A Dictionary&lt;TKey, TValue> that contains values of type TElement selected from the input sequence.</returns>
		public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			Func<TSource, TElement> elementSelector, 
			EqualityComparerDelegate<TKey> keyComparer,
			HashDelegate<TKey> keyHasher = null)
		{
			return source.ToDictionary(keySelector, elementSelector, new DelegateEqualityComparer<TKey>(keyComparer, keyHasher));
		}

		/// <summary>
		/// Creates a Lookup&lt;TKey, TElement> from an IEnumerable&lt;T> according to a specified key selector function and key comparison delegates.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <param name="source">The IEnumerable&lt;T> to create a Lookup&lt;TKey, TElement> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="keyComparer">A delegate to compare if two keys are equal.</param>
		/// <param name="keyHasher">A delegate tp calculate a key hash.</param>
		/// <returns>A Lookup&lt;TKey, TElement> that contains keys and values.</returns>
		public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			EqualityComparerDelegate<TKey> keyComparer,
			HashDelegate<TKey> keyHasher = null)
		{
			return source.ToLookup(keySelector, new DelegateEqualityComparer<TKey>(keyComparer, keyHasher));
		}

		/// <summary>
		/// Creates a Lookup&lt;TKey, TElement> from an IEnumerable&lt;T> according to a specified key selector function, comparison delegates and an element selector function.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
		/// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
		/// <param name="source">The IEnumerable&lt;T> to create a Lookup&lt;TKey, TElement> from.</param>
		/// <param name="keySelector">A function to extract a key from each element.</param>
		/// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
		/// <param name="keyComparer">A delegate to compare if two keys are equal.</param>
		/// <param name="keyHasher">A delegate tp calculate a key hash.</param>
		/// <returns>A Lookup&lt;TKey, TElement> that contains values of type TElement selected from the input sequence.</returns>
		public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(
			this IEnumerable<TSource> source, 
			Func<TSource, TKey> keySelector, 
			Func<TSource, TElement> elementSelector, 
			EqualityComparerDelegate<TKey> keyComparer,
			HashDelegate<TKey> keyHasher = null)
		{
			return source.ToLookup(keySelector, elementSelector, new DelegateEqualityComparer<TKey>(keyComparer, keyHasher));
		}

		/// <summary>
		/// Produces the set union of two sequences by using a specified comparison delegates.
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of the input sequences.</typeparam>
		/// <param name="first">An IEnumerable&lt;T> whose distinct elements form the first set for the union.</param>
		/// <param name="second">An IEnumerable&lt;T> whose distinct elements form the second set for the union.</param>
		/// <param name="equalityComparer">A delegate to compare if two elements are equal.</param>
		/// <param name="hasher">A delegate tp calculate an element hash.</param>
		/// <returns>An IEnumerable&lt;T> that contains the elements from both input sequences, excluding duplicates.</returns>
		public static IEnumerable<TSource> Union<TSource>(
			this IEnumerable<TSource> first, 
			IEnumerable<TSource> second, 
			EqualityComparerDelegate<TSource> equalityComparer,
			HashDelegate<TSource> hasher)
		{
			return first.Union(second, new DelegateEqualityComparer<TSource>(equalityComparer, hasher));
		}

		/// <summary>
		/// Returns true if the enumerable is null, or empty.
		/// </summary>
		/// <typeparam name="T">The type if items in the enumerable.</typeparam>
		/// <param name="enumerable">The enumerable to check.</param>
		/// <returns>true if the enumerable is null, or if it contains no items; otherwise false.</returns>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) =>
			enumerable?.Any() ?? false;

		/// <summary>
		/// Returns the list as IReadOnlyList&lt;T&gt;, wrapping it if necessary.
		/// </summary>
		/// <typeparam name="T">The typeparam of the list</typeparam>
		/// <param name="list">The list to return as IReadOnlyList&lt;T&gt;</param>
		/// <returns>An object which implements IReadOnlyList&lt;T&gt; for the list.</returns>
		public static IReadOnlyList<T> AsReadOnly<T>(this IList<T> list) =>
			(list as IReadOnlyList<T>) ?? new ListWrapper<T>(list);

		/// <summary>
		/// Returns the collection as IReadOnlyCollection&lt;T&gt;, wrapping it if necessary.
		/// </summary>
		/// <typeparam name="T">The typeparam of the collection</typeparam>
		/// <param name="collection">The collection to return as IReadOnlyList&lt;T&gt;</param>
		/// <returns>An object which implements IReadOnlyCollection&lt;T&gt; for the collection.</returns>
		public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection) =>
			(collection as IReadOnlyCollection<T>) ?? new CollectionWrapper<T>(collection);

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
				throw new ArgumentNullException(nameof(collection));
			}

			if (operation == null)
			{
				throw new ArgumentNullException(nameof(operation));
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
				throw new ArgumentNullException(nameof(collection));
			}

			if (operations == null)
			{
				throw new ArgumentNullException(nameof(operations));
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
				throw new ArgumentNullException(nameof(source));
			}

			return GetSequencialsInternal(source);
		}

		private static IEnumerable<Pair<T>> GetSequencialsInternal<T>(IEnumerable<T> source)
		{
			Debug.Assert(source != null, nameof(source) + " is null.");
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
		public static void ForEach<T>(this IEnumerable<T> collection, params Action<T>[] operations) =>
			collection.ForEach((IEnumerable<Action<T>>)operations);

		private class CollectionWrapper<T> : IReadOnlyCollection<T>
		{
			#region Fields

			private readonly ICollection<T> collection;
			#endregion

			#region Properties

			public int Count =>
				this.collection.Count; 
			#endregion

			#region Ctor

			public CollectionWrapper(ICollection<T> collection)
			{
				if (collection == null)
				{
					throw new ArgumentNullException(nameof(collection));
				}

				this.collection = collection;
			}
			#endregion

			#region Methods

			public IEnumerator<T> GetEnumerator() =>
				this.collection.GetEnumerator();

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
				GetEnumerator();
			#endregion
		}


		private class ListWrapper<T> : CollectionWrapper<T>, IReadOnlyList<T>
		{
			#region Fields

			private readonly IList<T> list;
			#endregion

			#region Properties

			public T this[int index] =>
				this.list[index]; 
			#endregion

			#region Ctor

			public ListWrapper(IList<T> list)
				: base(list)
			{
				if (list == null)
				{
					throw new ArgumentNullException(nameof(list));
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

			private readonly IDictionary<TKey, TValue> dictionary;
			#endregion

			#region Properties

			public IEnumerable<TKey> Keys =>
				this.dictionary.Keys; 

			public IEnumerable<TValue> Values =>
				this.dictionary.Values; 

			public TValue this[TKey key] =>
				this.dictionary[key]; 
			#endregion

			#region Ctor

			public DictionaryWrapper(IDictionary<TKey, TValue> dictionary)
				: base(dictionary)
			{
				if (dictionary == null)
				{
					throw new ArgumentNullException(nameof(dictionary));
				}

				this.dictionary = dictionary;
			}
			#endregion

			#region Methods

			public bool ContainsKey(TKey key) =>
				this.dictionary.ContainsKey(key);

			public bool TryGetValue(TKey key, out TValue value) =>
				this.dictionary.TryGetValue(key, out value);
			#endregion
		}
	}

	/// <summary>
	/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
	/// </summary>
	/// <typeparam name="T">The type of objects to compare.</typeparam>
	/// <param name="x">The first object to compare.</param>
	/// <param name="y">The second object to compare.</param>
	/// <returns>A signed integer that indicates the relative values of x and y.</returns>
	public delegate int ComparerDelegate<T>(T x, T y);

	/// <summary>
	/// Determines whether the specified objects are equal.
	/// </summary>
	/// <typeparam name="T">The type of objects to compare.</typeparam>
	/// <param name="x">The first object of type T to compare.</param>
	/// <param name="y">The second object of type T to compare.</param>
	/// <returns>true if the specified objects are equal; otherwise, false.</returns>
	public delegate bool EqualityComparerDelegate<T>(T x, T y);

	/// <summary>
	/// Returns a hash code for the specified object.
	/// </summary>
	/// <typeparam name="T">The type of objects to compare.</typeparam>
	/// <param name="object">The Object for which a hash code is to be returned.</param>
	/// <returns>A hash code for the specified object.</returns>
	public delegate int HashDelegate<T>(T @object);

	/// <summary>
	/// Compares objects for equality with specified delegates.
	/// </summary>
	/// <typeparam name="T">The type of objects to compare.</typeparam>
	public sealed class DelegateEqualityComparer<T> : IEqualityComparer<T>
	{
		#region Fields

		private readonly EqualityComparerDelegate<T> equalityComparer;
		private readonly HashDelegate<T> hasher;
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of the class.
		/// </summary>
		/// <param name="equalityComparer">A delegate to compare the equality of two objects.</param>
		/// <param name="hasher">A delegate to calculate hash for a given object.</param>
		public DelegateEqualityComparer(EqualityComparerDelegate<T> equalityComparer, HashDelegate<T> hasher = null)
		{
			if (equalityComparer == null)
			{
				throw new ArgumentNullException(nameof(equalityComparer));
			}

			if (hasher == null)
			{
				hasher = obj => obj.GetHashCode();
			}

			this.hasher = hasher;
			this.equalityComparer = equalityComparer;
		}
		#endregion

		#region Methods

		/// <summary>
		/// Determines whether the specified objects are equal.
		/// </summary>
		/// <param name="x">The first object of type T to compare.</param>
		/// <param name="y">The second object of type T to compare.</param>
		/// <returns>true if the specified objects are equal; otherwise, false.</returns>
		public bool Equals(T x, T y) =>
			this.equalityComparer(x, y);

		/// <summary>
		/// Returns a hash code for the specified object.
		/// </summary>
		/// <param name="obj">The Object for which a hash code is to be returned.</param>
		/// <returns>A hash code for the specified object.</returns>
		public int GetHashCode(T obj) =>
			this.hasher(obj);
		#endregion
	}

	/// <summary>
	/// Compares objects with specified delegates.
	/// </summary>
	/// <typeparam name="T">The type of objects to compare.</typeparam>
	public sealed class DelegateComparer<T> : IComparer<T>
	{
		#region Fields

		private readonly ComparerDelegate<T> comparer;
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of the class with the specified delegate.
		/// </summary>
		/// <param name="comparer">A delegate to compare two objects.</param>
		public DelegateComparer(ComparerDelegate<T> comparer)
		{
			if (comparer == null)
			{
				throw new ArgumentNullException(nameof(comparer));
			}

			this.comparer = comparer;
		}
		#endregion

		#region Methods

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>A signed integer that indicates the relative values of x and y.</returns>
		public int Compare(T x, T y) =>
			this.comparer(x, y);
		#endregion

		#region Operator

		/// <summary>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static implicit operator ComparerDelegate<T>(DelegateComparer<T> obj) =>
			obj.comparer;

		/// <summary>
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static implicit operator DelegateComparer<T>(ComparerDelegate<T> obj) =>
			new DelegateComparer<T>(obj);
		#endregion
	}
}
