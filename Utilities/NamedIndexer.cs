using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	/// <summary>
	/// Emulates a named indexer with one parameter.
	/// </summary>
	/// <typeparam name="TParam">The type of the parameter.</typeparam>
	/// <typeparam name="TType">The type of the idnexer.</typeparam>
	public class NamedIndexer<TParam, TType> : ReadOnlyNamedIndexer<TParam, TType>
	{
		#region Fields

		private readonly Action<TParam, TType> setter;
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the value for the given parameter.
		/// </summary>
		/// <param name="param">The parameter for the value.</param>
		/// <returns>The value for the given parameter.</returns>
		public new TType this[TParam param]
		{
			get { return base[param]; }
			set { this.setter(param, value); }
		}
		#endregion

		#region CTor

		/// <summary>
		/// Creates new instance of the indexer
		/// </summary>
		/// <param name="getter">Delegate to the getter function.</param>
		/// <param name="setter">Delegate to the setter function.</param>
		/// <param name="enumerator">
		/// Delegate which returns inumerator for the indexer, or null, 
		/// if enumeration is not supported for this indexer.
		/// </param>
		public NamedIndexer(
			Func<TParam, TType> getter,
			Action<TParam, TType> setter,
			Func<IEnumerator<TType>> enumerator = null)
			: base(getter, enumerator)
		{
			this.setter = setter;
		}
		#endregion
	}

	/// <summary>
	/// Emulates a named indexer with one parameter.
	/// </summary>
	/// <typeparam name="TParam1">The type of the first parameter.</typeparam>
	/// <typeparam name="TParam2">The type of the second parameter.</typeparam>
	/// <typeparam name="TType">The type of the idnexer.</typeparam>
	public class NamedIndexer<TParam1, TParam2, TType> :
		ReadOnlyNamedIndexer<TParam1, TParam2, TType>
	{
		#region Fields

		private readonly Action<TParam1, TParam2, TType> setter;
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the value for the given parameter.
		/// </summary>
		/// <param name="param1">The first parameter for the value.</param>
		/// <param name="param2">The second parameter for the value.</param>
		/// <returns>The value for the given parameters.</returns>
		public new TType this[TParam1 param1, TParam2 param2]
		{
			get { return base[param1, param2]; }
			set { this.setter(param1, param2, value); }
		}
		#endregion

		#region CTor

		/// <summary>
		/// Creates new instance of the indexer
		/// </summary>
		/// <param name="getter">Delegate to the getter function.</param>
		/// <param name="setter">Delegate to the setter function.</param>
		/// <param name="enumerator">
		/// Delegate which returns inumerator for the indexer, or null, 
		/// if enumeration is not supported for this indexer.
		/// </param>
		public NamedIndexer(
			Func<TParam1, TParam2, TType> getter,
			Action<TParam1, TParam2, TType> setter,
			Func<IEnumerator<TType>> enumerator = null)
			: base(getter, enumerator)
		{
			this.setter = setter;
		}
		#endregion
	}

	/// <summary>
	/// Emulates a named indexer with one parameter.
	/// </summary>
	/// <typeparam name="TParam1">The type of the first parameter.</typeparam>
	/// <typeparam name="TParam2">The type of the second parameter.</typeparam>
	/// <typeparam name="TParam3">The type of the third parameter.</typeparam>
	/// <typeparam name="TType">The type of the idnexer.</typeparam>
	public class NamedIndexer<TParam1, TParam2, TParam3, TType> :
		ReadOnlyNamedIndexer<TParam1, TParam2, TParam3, TType>
	{
		#region Fields

		private readonly Action<TParam1, TParam2, TParam3, TType> setter;
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the value for the given parameter.
		/// </summary>
		/// <param name="param1">The first parameter for the value.</param>
		/// <param name="param2">The second parameter for the value.</param>
		/// <param name="param3">The third parameter for the value.</param>
		/// <returns>The value for the given parameters.</returns>
		public new TType this[TParam1 param1, TParam2 param2, TParam3 param3]
		{
			get { return base[param1, param2, param3]; }
			set { this.setter(param1, param2, param3, value); }
		}
		#endregion

		#region CTor

		/// <summary>
		/// Creates new instance of the indexer
		/// </summary>
		/// <param name="getter">Delegate to the getter function.</param>
		/// <param name="setter">Delegate to the setter function.</param>
		/// <param name="enumerator">
		/// Delegate which returns inumerator for the indexer, or null, 
		/// if enumeration is not supported for this indexer.
		/// </param>
		public NamedIndexer(
			Func<TParam1, TParam2, TParam3, TType> getter,
			Action<TParam1, TParam2, TParam3, TType> setter,
			Func<IEnumerator<TType>> enumerator = null)
			: base(getter, enumerator)
		{
			this.setter = setter;
		}
		#endregion
	}
}
