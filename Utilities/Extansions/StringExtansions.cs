using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extansions.String
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
	}
}
