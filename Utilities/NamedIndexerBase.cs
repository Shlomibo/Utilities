using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	/// <summary>
	/// Base class for named indexers.
	/// </summary>
	/// <typeparam name="TType">The type of the named indexer.</typeparam>
	public abstract class NamedIndexerBase<TType> : IEnumerable<TType>
	{
		#region Fields

		private readonly Func<IEnumerator<TType>> enumerator;
		#endregion

		#region Fields

		/// <summary>
		/// Gets value which is true if the values in the indexer can be enumerated; otherwise false
		/// </summary>
		public bool IsEnumerable { get; private set; }
		#endregion

		#region Ctor

		/// <summary>
		/// Initialize new instance of the class.
		/// </summary>
		/// <param name="enumerator">
		/// Delegate which returns inumerator for the indexer, or null, 
		/// if enumeration is not supported for this indexer.
		/// </param>
		protected NamedIndexerBase(Func<IEnumerator<TType>> enumerator = null)
		{
			this.IsEnumerable = enumerator != null;

			this.enumerator = this.IsEnumerable
				? enumerator
				: () => { throw new InvalidOperationException("Enumeration is not supported."); };
		}
		#endregion

		#region Methods

		IEnumerator<TType> IEnumerable<TType>.GetEnumerator()
		{
			return this.enumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (this as IEnumerable<TType>).GetEnumerator();
		} 
		#endregion
	}
}
