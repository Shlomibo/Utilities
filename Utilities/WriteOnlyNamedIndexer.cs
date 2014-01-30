using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	/// <summary>
	/// Emulates a write only named indexer with one parameter.
	/// </summary>
	/// <typeparam name="TParam">The type of the parameter.</typeparam>
	/// <typeparam name="TType">The type of the idnexer.</typeparam>
	public class WriteOnlyNamedIndexer<TParam, TType> : NamedIndexer<TParam, TType>
	{
		#region Properties

		/// <summary>
		/// Sets the value for the given parameter.
		/// </summary>
		/// <param name="param">The parameter for the value.</param>
		/// <returns>Nothing.</returns>
		public new TType this[TParam param]
		{
			set { base[param] = value; }
		}
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of the indexer
		/// </summary>
		/// <param name="setter">Delegate to the setter function.</param>
		public WriteOnlyNamedIndexer(Action<TParam, TType> setter)
			: base(param => NamedIndexer.FailGet<TType>(), setter) { }
		#endregion
	}

	/// <summary>
	/// Emulates a write only named indexer with two parameters.
	/// </summary>
	/// <typeparam name="TParam1">The type of the first parameter.</typeparam>
	/// <typeparam name="TParam2">The type of the second parameter.</typeparam>
	/// <typeparam name="TType">The type of the idnexer.</typeparam>
	public class WriteOnlyNamedIndexer<TParam1, TParam2, TType> : NamedIndexer<TParam1, TParam2, TType>
	{
		#region Properties

		/// <summary>
		/// Sets the value for the given parameters.
		/// </summary>
		/// <param name="param1">The first parameter for the value.</param>
		/// <param name="param2">The second parameter for the value.</param>
		/// <returns>Nothing.</returns>
		public new TType this[TParam1 param1, TParam2 param2]
		{
			set { base[param1, param2] = value; }
		}
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of the indexer
		/// </summary>
		/// <param name="setter">Delegate to the setter function.</param>
		public WriteOnlyNamedIndexer(Action<TParam1, TParam2, TType> setter)
			: base((param1, param2) => NamedIndexer.FailGet<TType>(), setter) { }
		#endregion
	}

	/// <summary>
	/// Emulates a write only named indexer with three parameters.
	/// </summary>
	/// <typeparam name="TParam1">The type of the first parameter.</typeparam>
	/// <typeparam name="TParam2">The type of the second parameter.</typeparam>
	/// <typeparam name="TParam3">The type of the third parameter.</typeparam>
	/// <typeparam name="TType">The type of the idnexer.</typeparam>
	public class WriteOnlyNamedIndexer<TParam1, TParam2, TParam3, TType> : 
		NamedIndexer<TParam1, TParam2, TParam3, TType>
	{
		#region Properties

		/// <summary>
		/// Sets the value for the given parameters.
		/// </summary>
		/// <param name="param1">The first parameter for the value.</param>
		/// <param name="param2">The second parameter for the value.</param>
		/// <param name="param3">The third parameter for the value.</param>
		/// <returns>The value for the given parameters.</returns>
		public new TType this[TParam1 param1, TParam2 param2, TParam3 param3]
		{
			set { base[param1, param2, param3] = value; }
		}
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of the indexer
		/// </summary>
		/// <param name="setter">Delegate to the setter function.</param>
		public WriteOnlyNamedIndexer(Action<TParam1, TParam2, TParam3, TType> setter)
			: base((param1, param2, param3) => NamedIndexer.FailGet<TType>(), setter) { }
		#endregion
	}
}
