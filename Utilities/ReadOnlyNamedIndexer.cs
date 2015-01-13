using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	/// <summary>
	/// Emulates a read only named indexer with one parameter.
	/// </summary>
	/// <typeparam name="TParam">The type of the parameter.</typeparam>
	/// <typeparam name="TType">The type of the idnexer.</typeparam>
	public class ReadOnlyNamedIndexer<TParam, TType> : NamedIndexerBase<TType>
	{
		#region Fields

		private readonly Func<TParam, TType> getter;
		#endregion

		#region Properties

		/// <summary>
		/// Gets the value for the given parameter.
		/// </summary>
		/// <param name="param">The parameter for the value.</param>
		/// <returns>The value for the given parameter.</returns>
		public TType this[TParam param] => this.getter(param); 
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of the indexer
		/// </summary>
		/// <param name="getter">Delegate to the getter function.</param>
		/// <param name="enumerator">
		/// Delegate which returns inumerator for the indexer, or null, 
		/// if enumeration is not supported for this indexer.
		/// </param>
		public ReadOnlyNamedIndexer(
			Func<TParam, TType> getter,
			Func<IEnumerator<TType>> enumerator = null)
			: base(enumerator)
		{
			NamedIndexer.Validate(nameof(getter), getter);
			this.getter = getter;
		}
		#endregion
	}

	/// <summary>
	/// Emulates a read only named indexer with two parameters.
	/// </summary>
	/// <typeparam name="TParam1">The type of the first parameter.</typeparam>
	/// <typeparam name="TParam2">The type of the second parameter.</typeparam>
	/// <typeparam name="TType">The type of the idnexer.</typeparam>
	public class ReadOnlyNamedIndexer<TParam1, TParam2, TType> : NamedIndexerBase<TType>
	{
		#region Fields

		private readonly Func<TParam1, TParam2, TType> getter;
		#endregion

		#region Properties

		/// <summary>
		/// Gets the value for the given parameters.
		/// </summary>
		/// <param name="param1">The first parameter for the value.</param>
		/// <param name="param2">The second parameter for the value.</param>
		/// <returns>The value for the given parameters.</returns>
		public TType this[TParam1 param1, TParam2 param2] => this.getter(param1, param2); 
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of the indexer
		/// </summary>
		/// <param name="getter">Delegate to the getter function.</param>
		/// <param name="enumerator">
		/// Delegate which returns inumerator for the indexer, or null, 
		/// if enumeration is not supported for this indexer.
		/// </param>
		public ReadOnlyNamedIndexer(
			Func<TParam1, TParam2, TType> getter,
			Func<IEnumerator<TType>> enumerator = null)
			: base(enumerator)
		{
			NamedIndexer.Validate(nameof(getter), getter);
			this.getter = getter;
		}
		#endregion
	}

	/// <summary>
	/// Emulates a read only named indexer with three parameters.
	/// </summary>
	/// <typeparam name="TParam1">The type of the first parameter.</typeparam>
	/// <typeparam name="TParam2">The type of the second parameter.</typeparam>
	/// <typeparam name="TParam3">The type of the third parameter.</typeparam>
	/// <typeparam name="TType">The type of the idnexer.</typeparam>
	public class ReadOnlyNamedIndexer<TParam1, TParam2, TParam3, TType> : NamedIndexerBase<TType>
	{
		#region Fields

		private readonly Func<TParam1, TParam2, TParam3, TType> getter;
		#endregion

		#region Properties

		/// <summary>
		/// Gets the value for the given parameters.
		/// </summary>
		/// <param name="param1">The first parameter for the value.</param>
		/// <param name="param2">The second parameter for the value.</param>
		/// <param name="param3">The third parameter for the value.</param>
		/// <returns>The value for the given parameters.</returns>
		public TType this[TParam1 param1, TParam2 param2, TParam3 param3] => this.getter(param1, param2, param3); 
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of the indexer
		/// </summary>
		/// <param name="getter">Delegate to the getter function.</param>
		/// <param name="enumerator">
		/// Delegate which returns inumerator for the indexer, or null, 
		/// if enumeration is not supported for this indexer.
		/// </param>
		public ReadOnlyNamedIndexer(
			Func<TParam1, TParam2, TParam3, TType> getter,
			Func<IEnumerator<TType>> enumerator = null)
			: base(enumerator)
		{
			NamedIndexer.Validate(nameof(getter), getter);
			this.getter = getter;
		}
		#endregion
	}
}
