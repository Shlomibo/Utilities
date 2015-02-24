using System;
using static Utilities.Extansions.ObjectExtansions;

namespace Utilities.Extansions
{
	/// <summary>
	/// Represents a pair of items.
	/// </summary>
	/// <typeparam name="T">The items type.</typeparam>
	public struct Pair<T> : IEquatable<Pair<T>>
	{
		#region Fields

		/// <summary>
		/// The empty Pair.
		/// </summary>
		public static readonly Pair<T> Empty = new Pair<T>();
		#endregion

		#region Properties

		/// <summary>
		/// Gets the previous/first item.
		/// </summary>
		public T Previous { get; }

		/// <summary>
		/// Gets the current/second item.
		/// </summary>
		public T Current { get; }

		/// <summary>
		/// Gets the previous/first item.
		/// </summary>
		public T First =>
			this.Previous;

		/// <summary>
		/// Gets the current/second item.
		/// </summary>
		public T Second =>
			this.Current;
		#endregion

		#region Ctor

		/// <summary>
		/// Initialize new Pair instance.
		/// </summary>
		/// <param name="current">The current/second item.</param>
		/// <param name="previous">The previous/first item.</param>
		public Pair(T current, T previous = default(T))
			: this()
		{
			this.Previous = previous;
			this.Current = current;
		}
		#endregion

		#region Methods

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="obj">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return Equals((Pair<T>)obj);
		}

		/// <summary>
		/// Gets a hash code.
		/// </summary>
		/// <returns>A hash code calculated from the pair.</returns>
		public override int GetHashCode() =>
			CreateHashCode(this.Previous, this.Current);

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(Pair<T> other) =>
			object.Equals(this.Previous, other.Previous) &&
			object.Equals(this.Current, other.Current);

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString() =>
			$"{{{this.Previous}, {this.Current}}}";
		#endregion
	}
}
