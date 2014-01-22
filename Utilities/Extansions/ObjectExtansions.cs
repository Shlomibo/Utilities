using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extansions
{
	/// <summary>
	/// Provides utilities and extansions for all objects
	/// </summary>
	public static class ObjectExtansions
	{
		/// <summary>
		/// Creates hash code from the given objects
		/// </summary>
		/// <param name="objects">
		/// The objects to create the hash for. 
		/// These should be the immutable fields of the object to generate the hash for.
		/// </param>
		/// <returns>Hash code which is calculated from the given values</returns>
		public static int CreateHashCode(params object[] objects)
		{
			const int BASE_HASH = 27;
			const int SHIFT = 5;

			int result = 0;

			if ((objects != null) && (objects.Length > 0))
			{
				result = (from obj in objects
						  select (obj ?? 0).GetHashCode()).Aggregate(
							BASE_HASH,
							(accumulated, hash) => (accumulated << SHIFT) - accumulated + hash);
			}

			return result;
		}
	}
}
