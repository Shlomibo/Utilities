using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
	/// <summary>
	/// Supports the creation pf a reversable state.
	/// </summary>
	public sealed class ReversableState : IDisposable
	{
		#region Fields

		private readonly Action revereseAction;
		#endregion

		#region Properties

		/// <summary>
		/// Gets true if the state has been reveresed; otherwise, false.
		/// </summary>
		public bool IsDisposed { get; private set; } = false;
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of the class with the specified reverse action
		/// </summary>
		/// <param name="reverseAction">An action to run when the state should be reversed.</param>
		public ReversableState(Action reverseAction)
		{
			if (reverseAction == null)
			{
				throw new ArgumentNullException(nameof(reverseAction));
			}

			this.revereseAction = reverseAction;
		}
		#endregion

		#region Methods

		void IDisposable.Dispose() =>
			ReverseState();

		/// <summary>
		/// Reverse the state.
		/// </summary>
		public void ReverseState()
		{
			if (!this.IsDisposed)
			{
				this.IsDisposed = true;
				this.revereseAction();
			}
		}
		#endregion
	}
}
