using System;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Extansions
{
	/// <summary>
	/// Provides the functionality as System.Enum with generic methods
	/// </summary>
	public static class EnumExtansions
	{
		/// <summary>
		/// Converts the specified value of a specified enumerated type to its equivalent string representation according to the 
		/// specified format.
		/// </summary>
		/// <typeparam name="TEnum">The enumeration type of the value to convert.</typeparam>
		/// <param name="value">The value to convert.</param>
		/// <param name="format">The output format to use.</param>
		/// <returns>A string representation of value.</returns>
		public static string Format<TEnum>(TEnum value, string format) where TEnum : struct =>
			System.Enum.Format(typeof(TEnum), value, format);

		/// <summary>
		/// Retrieves the name of the constant in the specified enumeration that has the specified value.
		/// </summary>
		/// <typeparam name="TEnum">An enumeration type.</typeparam>
		/// <typeparam name="TUnderlying">The enumeration underlying type.</typeparam>
		/// <param name="value">The value of a particular enumerated constant in terms of its underlying type.</param>
		/// <returns>
		/// A string containing the name of the enumerated constant in enumType whose value is value;
		/// or null if no such constant is found.
		/// </returns>
		public static string GetName<TEnum, TUnderlying>(TUnderlying value) 
			where TEnum : struct
			where TUnderlying : struct =>
			System.Enum.GetName(typeof(TEnum), value);

		/// <summary>
		/// Retrieves an array of the names of the constants in a specified enumeration.
		/// </summary>
		/// <typeparam name="TEnum">An enumeration type.</typeparam>
		/// <returns>A string array of the names of the constants in enumType.</returns>
		public static string[] GetNames<TEnum>() where TEnum : struct =>
			System.Enum.GetNames(typeof(TEnum));

		/// <summary>
		/// Returns the underlying type of the specified enumeration.
		/// </summary>
		/// <typeparam name="TEnum">The enumeration type whose underlying type will be retrieved.</typeparam>
		/// <returns>The underlying type of TEnum.</returns>
		public static Type GetUnderlyingType<TEnum>() where TEnum : struct =>
			System.Enum.GetUnderlyingType(typeof(TEnum));

		/// <summary>
		/// Retrieves an array of the values of the constants in a specified enumeration.
		/// </summary>
		/// <typeparam name="TEnum">An enumeration type.</typeparam>
		/// <returns>An array that contains the values of the constants in enumType.</returns>
		public static TEnum[] GetValues<TEnum>() where TEnum : struct =>
			(TEnum[])System.Enum.GetValues(typeof(TEnum));

		/// <summary>
		/// Returns an indication whether a constant with a specified value exists in a specified enumeration.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enum.</typeparam>
		/// <param name="value">The value of the enum.</param>
		/// <returns>true if a constant in TEnum has a value equal to value; otherwise, false.</returns>
		public static bool IsDefined<TEnum>(TEnum value) where TEnum : struct =>
			IsDefined<TEnum>((object)value);

		/// <summary>
		/// Returns an indication whether a constant with a specified value exists in a specified enumeration.
		/// </summary>
		/// <typeparam name="TEnum">An enumeration type.</typeparam>
		/// <param name="value">The value or name of a constant in TEnum.</param>
		/// <returns>true if a constant in TEnum has a value equal to value; otherwise, false.</returns>
		public static bool IsDefined<TEnum>(object value) where TEnum : struct =>
			System.Enum.IsDefined(typeof(TEnum), value);

		/// <summary>
		/// Returns an indication whether a constant with a specified name exists in a specified enumeration.
		/// </summary>
		/// <typeparam name="TEnum">An enumeration type.</typeparam>
		/// <param name="name">The name of a constant in TEnum.</param>
		/// <param name="ignoreCase">true to ignore case; false to regard case.</param>
		/// <returns>true if a constant in TEnum has a name equal to value; otherwise, false.</returns>
		public static bool IsDefined<TEnum>(string name, bool ignoreCase) where TEnum : struct
		{
			if (!ignoreCase)
			{
				return IsDefined<TEnum>(name);
			}
			else
			{
				return GetNames<TEnum>().Any(enumName => 
					enumName.Equals(name, StringComparison.InvariantCultureIgnoreCase));
			}
		}

		/// <summary>
		/// Converts the string representation of the name or numeric value of one or more enumerated constants to 
		/// an equivalent enumerated object.
		/// </summary>
		/// <typeparam name="TEnum">An enumeration type.</typeparam>
		/// <param name="value">A string containing the name or value to convert.</param>
		/// <returns>An object of type TEnum whose value is represented by value.</returns>
		public static TEnum Parse<TEnum>(string value) where TEnum : struct =>
			(TEnum)System.Enum.Parse(typeof(TEnum), value);

		/// <summary>
		/// Converts the string representation of the name or numeric value of one or more enumerated constants to 
		/// an equivalent enumerated object. A parameter specifies whether the operation is case-insensitive.
		/// </summary>
		/// <typeparam name="TEnum">An enumeration type.</typeparam>
		/// <param name="value">A string containing the name or value to convert.</param>
		/// <param name="ignoreCase">true to ignore case; false to regard case.</param>
		/// <returns>An object of type TEnum whose value is represented by value.</returns>
		public static TEnum Parse<TEnum>(string value, bool ignoreCase) where TEnum : struct =>
			(TEnum)System.Enum.Parse(typeof(TEnum), value, ignoreCase);

		/// <summary>
		/// Converts the specified object with an integer value to an enumeration member.
		/// </summary>
		/// <typeparam name="TEnum">The enumeration type to return.</typeparam>
		/// <typeparam name="TUnderlying">The type of the value.</typeparam>
		/// <param name="value">The value convert to an enumeration member.</param>
		/// <returns>An enumeration object whose value is value.</returns>
		public static TEnum ToObject<TEnum, TUnderlying>(TUnderlying value) 
			where TEnum : struct
			where TUnderlying : struct =>
			(TEnum)System.Enum.ToObject(typeof(TEnum), value);

		/// <summary>
		/// Converts the string representation of the name or numeric value of one or more enumerated constants to 
		/// an equivalent enumerated object. The return value indicates whether the conversion succeeded.
		/// </summary>
		/// <typeparam name="TEnum">The enumeration type to which to convert value.</typeparam>
		/// <param name="value">The string representation of the enumeration name or underlying value to convert.</param>
		/// <param name="result">
		/// When this method returns, result contains an object of type TEnum whose value is represented by value if 
		/// the parse operation succeeds. 
		/// If the parse operation fails, result contains the default value of the underlying type of TEnum. 
		/// Note that this value need not be a member of the TEnum enumeration. This parameter is passed uninitialized.
		/// </param>
		/// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
		public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct =>
			System.Enum.TryParse(value, out result);

		/// <summary>
		/// Converts the string representation of the name or numeric value of one or more enumerated constants to 
		/// an equivalent enumerated object. 
		/// A parameter specifies whether the operation is case-sensitive. 
		/// The return value indicates whether the conversion succeeded.
		/// </summary>
		/// <typeparam name="TEnum">The enumeration type to which to convert value.</typeparam>
		/// <param name="value">The string representation of the enumeration name or underlying value to convert.</param>
		/// <param name="ignoreCase">true to ignore case; false to consider case.</param>
		/// <param name="result">
		/// When this method returns, result contains an object of type TEnum whose value is represented by value if 
		/// the parse operation succeeds. 
		/// If the parse operation fails, result contains the default value of the underlying type of TEnum. 
		/// Note that this value need not be a member of the TEnum enumeration. 
		/// This parameter is passed uninitialized.
		/// </param>
		/// <returns>true if the value parameter was converted successfully; otherwise, false.</returns>
		public static bool TryParse<TEnum>(string value, bool ignoreCase, out TEnum result) where TEnum : struct =>
			System.Enum.TryParse(value, ignoreCase, out result);

		/// <summary>
		/// Combine the values of two enums.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		/// <param name="left">The left side of the combination.</param>
		/// <param name="right">The right side of the combination.</param>
		/// <returns>A combination of both enumerations.</returns>
		public static TEnum Combine<TEnum>(TEnum left, TEnum right) 
			where TEnum : struct =>
			(TEnum)((dynamic)left | right);

		/// <summary>
		/// Combine the values of the enums in the enumeration.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		/// <param name="values">The values to combine.</param>
		/// <returns>A combination of the values.</returns>
		public static TEnum Combine<TEnum>(IEnumerable<TEnum> values)
			where TEnum : struct =>
			values.Aggregate(
				default(TEnum), 
				(combination, value) => Combine(combination, value));

		/// <summary>
		/// Combine the given enum values.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		/// <param name="values">The values to combine.</param>
		/// <returns>A combination of the values.</returns>
		public static TEnum Combine<TEnum>(params TEnum[] values)
			where TEnum : struct =>
			Combine((IEnumerable<TEnum>)values);

		/// <summary>
		/// Checks if the value has the flag.
		/// </summary>
		/// <typeparam name="TEnum">The enumeration type.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="flag">The flag to check.</param>
		/// <returns>true if the value has the flag.</returns>
		public static bool HasFlag<TEnum>(TEnum value, TEnum flag)
			where TEnum : struct =>
			((dynamic)value & flag) == flag;

		/// <summary>
		/// Checks if the enumeration is combination of only defined flags.
		/// </summary>
		/// <typeparam name="TEnum">The enumeration type.</typeparam>
		/// <param name="combination">The enumeration to check.</param>
		/// <returns>true if the enumeration is combined from defined values; otherwise false.</returns>
		public static bool IsCombinationDefined<TEnum>(TEnum combination)
			where TEnum : struct 
		{
			var fullCombination = (dynamic)Combine(GetValues<TEnum>());

			combination &= ~fullCombination;

			return combination.Equals(default(TEnum));
		}
	}
}
