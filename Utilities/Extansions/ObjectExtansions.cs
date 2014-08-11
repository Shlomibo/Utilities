using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extansions.Object
{
	/// <summary>
	/// Provides utilities and extansions for all objects
	/// </summary>
	public static class ObjectExtansions
	{
		#region Fields

		private static readonly Dictionary<Type, Delegate> typesRetrievers = new Dictionary<Type, Delegate>();
		#endregion

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
			return CreateHashCode((IEnumerable<object>)objects);
		}

		/// <summary>
		/// Creates hash code from the given objects
		/// </summary>
		/// <param name="objects">
		/// The objects to create the hash for. 
		/// These should be the immutable fields of the object to generate the hash for.
		/// </param>
		/// <returns>Hash code which is calculated from the given values</returns>
		public static int CreateHashCode(IEnumerable<object> objects)
		{
			const int BASE_HASH = 27;
			const int SHIFT = 7;

			int result = 0;

			if ((objects != null) && (objects.Any()))
			{
				unchecked
				{
					result = objects.Select(obj => obj.NullableGetHashCode())
									.Aggregate(
										BASE_HASH,
										(accumulated, hash) =>
											(accumulated << SHIFT) + accumulated + hash);
				}
			}

			return result;
		}

		/// <summary>
		/// Executes the given Func, with the input, if the isn't null. otherwise returns @default.
		/// </summary>
		/// <typeparam name="TIn">The input type. this has to be a class.</typeparam>
		/// <typeparam name="TOut">The output type.</typeparam>
		/// <param name="input">The input object.</param>
		/// <param name="func">The function to execute, with input as the input parameter.</param>
		/// <param name="default">The default value to return, if input is null.</param>
		/// <returns>If the input isn't null, the result of func for the input; otherwise, the default value.</returns>
		public static TOut IgnoreNullFor<TIn, TOut>(
			this TIn input,
			Func<TIn, TOut> func,
			TOut @default = default(TOut))
			where TIn : class
		{
			if (input == null)
			{
				return @default;
			}
			else
			{
				return func(input);
			}
		}

		/// <summary>
		/// Executes the action, if input isn't null.
		/// </summary>
		/// <typeparam name="T">The input type</typeparam>
		/// <param name="input">The input for the action to execute.</param>
		/// <param name="action">The action to execute.</param>
		public static void IgnoreNullFor<T>(this T input, Action<T> action)
			where T : class
		{
			Func<T, object> stubFunc = arg =>
				{
					action(arg);
					return null;
				};

			input.IgnoreNullFor(stubFunc);
		}

		/// <summary>
		/// Returns string for the object, regardless if it is null
		/// </summary>
		/// <param name="object">The object to return string for.</param>
		/// <param name="nullString">The string for null objects.</param>
		/// <returns>
		/// If the object isn't null, the string representation for that object; otherwise nullString would be returned
		/// </returns>
		public static string NullableToString<T>(this T @object, string nullString = "")
			where T : class
		{
			return @object.IgnoreNullFor(obj => obj.ToString(), nullString);
		}

		/// <summary>
		/// Gets the type for the object, or null
		/// </summary>
		/// <typeparam name="T">The type of the object</typeparam>
		/// <param name="object">The object to get it's type.</param>
		/// <returns>The type of the object is it isn't null; otherwise, null.</returns>
		public static Type NullableGetType<T>(this T @object)
			where T : class
		{
			return @object.IgnoreNullFor(obj => obj.GetType());
		}

		/// <summary>
		/// Gets the hush code for the object, or defaultHash, if the object is null.
		/// </summary>
		/// <typeparam name="T">The type of the object</typeparam>
		/// <param name="object">The object to get its hush code.</param>
		/// <param name="defalutHash">The hash to return if the object is null</param>
		/// <returns>The hush code for the object if it isn't null; otherwise defaultHash</returns>
		public static int NullableGetHashCode<T>(this T @object, int defalutHash = 0)
			where T : class
		{
			return @object.IgnoreNullFor(obj => obj.GetHashCode(), defalutHash);
		}

		/// <summary>
		/// Checks if the given object equals to the other object, even if it's null.
		/// </summary>
		/// <typeparam name="T">The type of the objects to compare</typeparam>
		/// <param name="object">The object to compare.</param>
		/// <param name="other">The other object to compare</param>
		/// <returns>True if the object is equals to the other, or if they are both null; otherwise false.</returns>
		public static bool NullableEquals<T>(this T @object, T other)
			where T : class, IEquatable<T>
		{
			return @object.IgnoreNullFor(obj => obj.Equals(other), other == null);
		}

		/// <summary>
		/// Checks if the given object equals to the other object, even if it's null.
		/// </summary>
		/// <typeparam name="T">The type of the objects to compare</typeparam>
		/// <param name="object">The object to compare.</param>
		/// <param name="other">The other object to compare</param>
		/// <returns>True if the object is equals to the other, or if they are both null; otherwise false.</returns>
		public static bool NullableEquals<T>(this T @object, object other)
			where T : class
		{
			return @object.IgnoreNullFor(obj => obj.Equals(other), other == null);
		}

		/// <summary>
		/// Tries to execute the function on the given input
		/// </summary>
		/// <typeparam name="TIn">The input type</typeparam>
		/// <typeparam name="TOut">The output type</typeparam>
		/// <param name="input">The input for the func</param>
		/// <param name="func">The func to execute</param>
		/// <param name="result">Out: the result of the func, if execution succeeded.</param>
		/// <returns>true if execution succeeded; otherwise false.</returns>
		/// <remarks>If you can avoid the exception, do not use this method, and do not throw it just to catch it.</remarks>
		public static bool TryExecute<TIn, TOut>(
			this TIn input,
			Func<TIn, TOut> func,
			out TOut result)
		{
			result = default(TOut);

			try
			{
				result = func(input);
				return true;
			}
#if DEBUG
			catch (Exception ex)
			{
				Debug.WriteLine("Exception caught in IgnoreExceptionFor:");
				Debug.WriteLine("\t" + ex);
			}
#else
			catch { }
#endif
			return false;
		}

		/// <summary>
		/// Tries to action the function on the given input
		/// </summary>
		/// <typeparam name="T">The input type</typeparam>
		/// <param name="input">The input for the func</param>
		/// <param name="action">The action to execute</param>
		/// <returns>true if execution succeeded; otherwise false.</returns>
		/// <remarks>If you can avoid the exception, do not use this method, and do not throw it just to catch it.</remarks>
		public static bool TryExecute<T>(this T input, Action<T> action)
		{
			Func<T, object> stubFunc = arg =>
				{
					action(arg);
					return null;
				};
			object stub;

			return input.TryExecute(stubFunc, out stub);
		}

		/// <summary>
		/// Executes func, with the input as the argument, and ignores any exception that may have been thrown.
		/// </summary>
		/// <typeparam name="TIn">The input parameter type.</typeparam>
		/// <typeparam name="TOut">The output type.</typeparam>
		/// <param name="input">The input argument.</param>
		/// <param name="func">The function to execute.</param>
		/// <param name="default">The default value to return if the execution has been failed.</param>
		/// <returns>The result of func, if no exception was thrown; otherwise, the default value.</returns>
		/// <remarks>If you can avoid the exception, do not use this method, and do not throw it just to catch it.</remarks>
		public static TOut IgnoreExceptionFor<TIn, TOut>(
			this TIn input,
			Func<TIn, TOut> func,
			TOut @default = default(TOut))
		{
			TOut result;

			if (!input.TryExecute(func, out result))
			{
				result = @default;
			}

			return result;
		}

		/// <summary>
		/// Executes action, with the input as the argument, and ignores any exception that may have been thrown.
		/// </summary>
		/// <typeparam name="T">The input parameter type.</typeparam>
		/// <param name="input">The input argument.</param>
		/// <param name="action">The action to execute.</param>
		/// <remarks>If you can avoid the exception, do not use this method, and do not throw it just to catch it.</remarks>
		public static void IgnoreExceptionFor<T>(this T input, Action<T> action)
		{
			input.TryExecute(action);
		}

		/// <summary>
		/// Throws exception, if the when predicate returns true for the item, 
		/// and throws the exception returned from thte what function.
		/// </summary>
		/// <typeparam name="T">The type of the item.</typeparam>
		/// <param name="item">The item to check.</param>
		/// <param name="when">Predicate to check the item.</param>
		/// <param name="what">Function to get the exception to throw.</param>
		public static void ThrowWhen<T>(this T item, Func<T, bool> when, Func<Exception> what)
		{
			if (when(item))
			{
				throw what();
			}
		}

		/// <summary>
		/// Throws exception, if the when predicate returns true for the item, 
		/// and throws the given excpetion.
		/// </summary>
		/// <typeparam name="T">The type of the item.</typeparam>
		/// <param name="item">The item to check.</param>
		/// <param name="when">Predicate to check the item.</param>
		/// <param name="what">The exception to throw.</param>
		public static void ThrowWhen<T>(this T item, Func<T, bool> when, Exception what)
		{
			item.ThrowWhen(when, () => what);
		}
	}
}
