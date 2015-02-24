using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extansions
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
		public static bool IsNullOrEmpty(this string str) =>
			string.IsNullOrEmpty(str);

		/// <summary>
		/// Indicates whether a specified string is null, empty, or consists only of white-space characters.
		/// </summary>
		/// <param name="str">The string to test.</param>
		/// <returns>
		/// true if the value parameter is null or String.Empty, or if value consists exclusively of white-space characters. 
		/// </returns>
		public static bool IsNullOrWhiteSpace(this string str) =>
			string.IsNullOrWhiteSpace(str);

		/// <summary>
		/// Returns an IReadOnlyList&lt;char&gt; wrapper for the string.
		/// </summary>
		/// <param name="str">The string to wrap.</param>
		/// <returns>A wrapper which implements IReadOnlyList&lt;char&gt; for the string.</returns>
		public static IReadOnlyList<char> AsList(this string str) =>
			new StringWrapper(str);

		private class StringWrapper : IReadOnlyList<char>
		{
			#region Fields

			private readonly string str;
			#endregion

			#region Properties

			public char this[int index] => this.str[index]; 

			public int Count => this.str.Length; 
			#endregion

			#region Ctor

			public StringWrapper(string str)
			{
				if (str == null)
				{
					throw new ArgumentNullException(nameof(str));
				}

				this.str = str;
			}
			#endregion

			#region Methods

			public IEnumerator<char> GetEnumerator() =>
				this.str.GetEnumerator();

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
				GetEnumerator();
			#endregion
		}

	}
}
