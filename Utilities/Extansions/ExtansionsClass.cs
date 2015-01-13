using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extansions
{
	/// <summary>
	/// Provide general extansion methods
	/// </summary>
	public static class ExtansionsClass
	{
		/// <summary>
		/// Sets the given key, to the given value in the dictionary, 
		/// regardless of if the key already exists in the dictionary
		/// </summary>
		/// <typeparam name="TKey">The key type in the dictionary</typeparam>
		/// <typeparam name="TValue">The value type in the dictionary</typeparam>
		/// <param name="dictionary">The dictionary to set</param>
		/// <param name="key">The key to set</param>
		/// <param name="value">The value to set</param>
		/// <returns>true if new key added to the dictionary; otherwise false.</returns>
		public static bool SetKey<TKey, TValue>(
			this IDictionary<TKey, TValue> dictionary,
			TKey key,
			TValue value)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}

			bool isNew = !dictionary.ContainsKey(key);

			if (isNew)
			{
				dictionary.Add(key, value);
			}
			else
			{
				dictionary[key] = value;
			}

			return isNew;
		}
	}
}
