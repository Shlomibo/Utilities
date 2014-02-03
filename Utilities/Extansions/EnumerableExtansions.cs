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
	}
}
