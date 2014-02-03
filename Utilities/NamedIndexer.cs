using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extansions.Object;
using Utilities.Extansions.Enumerable;

namespace Utilities
{
	/// <summary>
	/// Static class for creating named indexers
	/// </summary>
	public static class NamedIndexer
	{
		internal static TType FailGet<TType>()
		{
			throw new InvalidOperationException("Cannot get value for write only indexer.");
		}

		internal static void Validate(IDictionary<string, object> arguments)
		{
			arguments.ForEach(keyValue => keyValue.ThrowWhen(
				when: keyVal => keyVal.Value == null,
				what: new ArgumentNullException(keyValue.Key)));
		}

		internal static void Validate(string argName, object value)
		{
			Validate(new Dictionary<string, object> { { argName, value } });
		}

		#region ReadWrite creation

		/// <summary>
		/// Creates new read-write named indexer with one parameter.
		/// </summary>
		/// <typeparam name="TParam">The type of the parameter.</typeparam>
		/// <typeparam name="TType">The type of the indexer.</typeparam>
		/// <param name="getter">Getter delegate to retrive items.</param>
		/// <param name="setter">Setter delegate to set items.</param>
		/// <param name="enumerator">Optional: Delegate to retrieve enumerator for the inedexer.</param>
		/// <returns>NamedIndexer for the given types.</returns>
		public static NamedIndexer<TParam, TType> CreateReadWrite<TParam, TType>(
			Func<TParam, TType> getter,
			Action<TParam, TType> setter,
			Func<IEnumerator<TType>> enumerator = null)
		{
			return new NamedIndexer<TParam, TType>(getter, setter, enumerator);
		}

		/// <summary>
		/// Creates new read-write named indexer with two parameters.
		/// </summary>
		/// <typeparam name="TParam1">The type of the first parameter.</typeparam>
		/// <typeparam name="TParam2">The type of the second parameter.</typeparam>
		/// <typeparam name="TType">The type of the indexer.</typeparam>
		/// <param name="getter">Getter delegate to retrive items.</param>
		/// <param name="setter">Setter delegate to set items.</param>
		/// <param name="enumerator">Optional: Delegate to retrieve enumerator for the inedexer.</param>
		/// <returns>NamedIndexer for the given types.</returns>
		public static NamedIndexer<TParam1, TParam2, TType> CreateReadWrite<TParam1, TParam2, TType>(
			Func<TParam1, TParam2, TType> getter,
			Action<TParam1, TParam2, TType> setter,
			Func<IEnumerator<TType>> enumerator = null)
		{
			return new NamedIndexer<TParam1, TParam2, TType>(getter, setter, enumerator);
		}

		/// <summary>
		/// Creates new read-write named indexer with three parameters.
		/// </summary>
		/// <typeparam name="TParam1">The type of the first parameter.</typeparam>
		/// <typeparam name="TParam2">The type of the second parameter.</typeparam>
		/// <typeparam name="TParam3">The type of the third parameter.</typeparam>
		/// <typeparam name="TType">The type of the indexer.</typeparam>
		/// <param name="getter">Getter delegate to retrive items.</param>
		/// <param name="setter">Setter delegate to set items.</param>
		/// <param name="enumerator">Optional: Delegate to retrieve enumerator for the inedexer.</param>
		/// <returns>NamedIndexer for the given types.</returns>
		public static NamedIndexer<TParam1, TParam2, TParam3, TType> CreateReadWrite<TParam1, TParam2, TParam3, TType>(
			Func<TParam1, TParam2, TParam3, TType> getter,
			Action<TParam1, TParam2, TParam3, TType> setter,
			Func<IEnumerator<TType>> enumerator = null)
		{
			return new NamedIndexer<TParam1, TParam2, TParam3, TType>(getter, setter, enumerator);
		}
		#endregion

		#region Read only creation

		/// <summary>
		/// Creates new read-only named indexer with one parameter.
		/// </summary>
		/// <typeparam name="TParam">The type of the parameter.</typeparam>
		/// <typeparam name="TType">The type of the indexer.</typeparam>
		/// <param name="getter">Getter delegate to retrive items.</param>
		/// <param name="enumerator">Optional: Delegate to retrieve enumerator for the inedexer.</param>
		/// <returns>ReadOnlyNamedIndexer for the given types.</returns>
		public static ReadOnlyNamedIndexer<TParam, TType> CreateReadOnly<TParam, TType>(
			Func<TParam, TType> getter,
			Func<IEnumerator<TType>> enumerator = null)
		{
			return new ReadOnlyNamedIndexer<TParam, TType>(getter, enumerator);
		}

		/// <summary>
		/// Creates new read-only named indexer with two parameters.
		/// </summary>
		/// <typeparam name="TParam1">The type of the first parameter.</typeparam>
		/// <typeparam name="TParam2">The type of the second parameter.</typeparam>
		/// <typeparam name="TType">The type of the indexer.</typeparam>
		/// <param name="getter">Getter delegate to retrive items.</param>
		/// <param name="enumerator">Optional: Delegate to retrieve enumerator for the inedexer.</param>
		/// <returns>ReadOnlyNamedIndexer for the given types.</returns>
		public static ReadOnlyNamedIndexer<TParam1, TParam2, TType> CreateReadOnly<TParam1, TParam2, TType>(
			Func<TParam1, TParam2, TType> getter,
			Func<IEnumerator<TType>> enumerator = null)
		{
			return new ReadOnlyNamedIndexer<TParam1, TParam2, TType>(getter, enumerator);
		}

		/// <summary>
		/// Creates new read-only named indexer with three parameters.
		/// </summary>
		/// <typeparam name="TParam1">The type of the first parameter.</typeparam>
		/// <typeparam name="TParam2">The type of the second parameter.</typeparam>
		/// <typeparam name="TParam3">The type of the third parameter.</typeparam>
		/// <typeparam name="TType">The type of the indexer.</typeparam>
		/// <param name="getter">Getter delegate to retrive items.</param>
		/// <param name="enumerator">Optional: Delegate to retrieve enumerator for the inedexer.</param>
		/// <returns>ReadOnlyNamedIndexer for the given types.</returns>
		public static ReadOnlyNamedIndexer<TParam1, TParam2, TParam3, TType> CreateReadOnly<TParam1, TParam2, TParam3, TType>(
			Func<TParam1, TParam2, TParam3, TType> getter,
			Func<IEnumerator<TType>> enumerator = null)
		{
			return new ReadOnlyNamedIndexer<TParam1, TParam2, TParam3, TType>(getter, enumerator);
		}
		#endregion

		#region Write only creation

		/// <summary>
		/// Creates new write-only named indexer with one parameter.
		/// </summary>
		/// <typeparam name="TParam">The type of the parameter.</typeparam>
		/// <typeparam name="TType">The type of the indexer.</typeparam>
		/// <param name="setter">Setter delegate to set items.</param>
		/// <returns>WriteOnlyNamedIndexer for the given types.</returns>
		public static WriteOnlyNamedIndexer<TParam, TType> CreateWriteOnly<TParam, TType>(
			Action<TParam, TType> setter)
		{
			return new WriteOnlyNamedIndexer<TParam, TType>(setter);
		}

		/// <summary>
		/// Creates new write-only named indexer with two parameters.
		/// </summary>
		/// <typeparam name="TParam1">The type of the first parameter.</typeparam>
		/// <typeparam name="TParam2">The type of the second parameter.</typeparam>
		/// <typeparam name="TType">The type of the indexer.</typeparam>
		/// <param name="setter">Setter delegate to set items.</param>
		/// <returns>WriteOnlyNamedIndexer for the given types.</returns>
		public static WriteOnlyNamedIndexer<TParam1, TParam2, TType> CreateWriteOnly<TParam1, TParam2, TType>(
			Action<TParam1, TParam2, TType> setter)
		{
			return new WriteOnlyNamedIndexer<TParam1, TParam2, TType>(setter);
		}

		/// <summary>
		/// Creates new write-only named indexer with three parameters.
		/// </summary>
		/// <typeparam name="TParam1">The type of the first parameter.</typeparam>
		/// <typeparam name="TParam2">The type of the second parameter.</typeparam>
		/// <typeparam name="TParam3">The type of the third parameter.</typeparam>
		/// <typeparam name="TType">The type of the indexer.</typeparam>
		/// <param name="setter">Setter delegate to set items.</param>
		/// <returns>WriteOnlyNamedIndexer for the given types.</returns>
		public static WriteOnlyNamedIndexer<TParam1, TParam2, TParam3, TType> CreateWriteOnly<TParam1, TParam2, TParam3, TType>(
			Action<TParam1, TParam2, TParam3, TType> setter)
		{
			return new WriteOnlyNamedIndexer<TParam1, TParam2, TParam3, TType>(setter);
		}
		#endregion
	}

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
			NamedIndexer.Validate("setter", setter);
			this.setter = setter;
		}
		#endregion
	}

	/// <summary>
	/// Emulates a named indexer with two parameters.
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
			NamedIndexer.Validate("setter", setter);
			this.setter = setter;
		}
		#endregion
	}

	/// <summary>
	/// Emulates a named indexer with three parametera.
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
			NamedIndexer.Validate("setter", setter);
			this.setter = setter;
		}
		#endregion
	}
}
