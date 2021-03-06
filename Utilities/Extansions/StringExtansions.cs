﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extansions.Text
{
	/// <summary>
	/// Extansion methods for string
	/// </summary>
	public static class StringExtansions
	{
		/// <summary>
		/// Indicates whether the specified string is null or an Empty string.
		/// </summary>
		/// <param name="str">The string to test.</param>
		/// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
		public static bool IsNullOrEmpty(this string str)
		{
			return string.IsNullOrEmpty(str);
		}

		/// <summary>
		/// Indicates whether a specified string is null, empty, or consists only of white-space characters.
		/// </summary>
		/// <param name="str">The string to test.</param>
		/// <returns>
		/// true if the value parameter is null or String.Empty, or if value consists exclusively of white-space characters. 
		/// </returns>
		public static bool IsNullOrWhiteSpace(this string str)
		{
			return string.IsNullOrWhiteSpace(str);
		}

		/// <summary>
		/// Returns an IReadOnlyList&lt;char&gt; wrapper for the string.
		/// </summary>
		/// <param name="str">The string to wrap.</param>
		/// <returns>A wrapper which implements IReadOnlyList&lt;char&gt; for the string.</returns>
		public static IReadOnlyList<char> AsList(this string str)
		{
			return new StringWrapper(str);
		}

		private class StringWrapper : IReadOnlyList<char>
		{
			#region Fields

			private string str;
			#endregion

			#region Properties

			public char this[int index]
			{
				get { return this.str[index]; }
			}

			public int Count
			{
				get { return this.str.Length; }
			} 
			#endregion

			#region Ctor

			public StringWrapper(string str)
			{
				if (str == null)
				{
					throw new ArgumentNullException();
				}

				this.str = str;
			}
			#endregion

			#region Methods

			public IEnumerator<char> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			} 
			#endregion
		}

	}
}
