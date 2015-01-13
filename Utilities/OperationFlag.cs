using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extansions.Object;

namespace Utilities
{
	/// <summary>
	/// A flag for an operation, which can be set, and reset using 'using' statement.
	/// </summary>
	public sealed class OperationFlag : IEquatable<OperationFlag>
	{
		#region Fields

		private bool value;
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new flag, with the given initial value.
		/// </summary>
		/// <param name="value">The value of the flag.</param>
		public OperationFlag(bool value)
		{
			this.value = value;
		}
		#endregion

		#region Methods

		/// <summary>
		/// Flips the state of the flag, and returns an IDisposable to flip it back to the original state.
		/// </summary>
		/// <returns>IDisposable object to flip the state back.</returns>
		public IDisposable Flip()
		{
			this.value = !this.value;
			return new ReversableState(() => this.value = !this.value);
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			//       
			// See the full list of guidelines at
			//   http://go.microsoft.com/fwlink/?LinkID=85237  
			// and also the guidance for operator== at
			//   http://go.microsoft.com/fwlink/?LinkId=85238
			//

			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			// TODO: write your implementation of Equals() here
			return Equals((OperationFlag)obj);
		}

		/// <summary>
		/// Serves as the default hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode() =>
			ObjectExtansions.CreateHashCode(this.value);

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
		public bool Equals(OperationFlag other) =>
			(other != null) &&
			(this.value == other.value);

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString() =>
			this.value.ToString();
		#endregion

		#region Operators

		/// <summary>
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static implicit operator bool(OperationFlag flag) =>
			flag.value;

		/// <summary>
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static implicit operator OperationFlag(bool flag) =>
			new OperationFlag(flag);

		/// <summary>
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static bool operator true(OperationFlag flag) =>
			flag;

		/// <summary>
		/// </summary>
		/// <param name="flag"></param>
		/// <returns></returns>
		public static bool operator false(OperationFlag flag) =>
			!flag;

		/// <summary>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(OperationFlag left, OperationFlag right) =>
			object.Equals(left, right);

		/// <summary>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(OperationFlag left, bool right) =>
			left.value == right;

		/// <summary>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(OperationFlag left, OperationFlag right) =>
			!(left == right);

		/// <summary>
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(OperationFlag left, bool right) =>
			!(left == right);
		#endregion
	}
}
