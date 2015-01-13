using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extansions.Object;

namespace Utilities
{
	/// <summary>
	/// Provides safe handling for an arbitrary object.
	/// </summary>
	/// <typeparam name="T">The type of the object to handle.</typeparam>
	public class SafeObject<T> : IDisposable
	{
		#region Fields

		private T @object;
		private readonly Action<T> releaser;
		#endregion

		#region Properties

		/// <summary>
		/// Gets the object.
		/// </summary>
		public T Object
		{
			get
			{
				ThrowIfDisposed();

				return this.@object;
			}
			protected set
			{
				ThrowIfDisposed();

				this.@object = value;
			}
		}

		/// <summary>
		/// Gets true if the object has been disposed; otherwise false.
		/// </summary>
		public bool IsDisposed { get; private set; }
		#endregion

		#region Events

		/// <summary>
		/// Raised after the object has been disposed.
		/// </summary>
		public event EventHandler Disposed;
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance of the object
		/// </summary>
		/// <param name="object">The object to handle.</param>
		/// <param name="releaser">Delegate to a function that releases resources of the object.</param>
		public SafeObject(T @object, Action<T> releaser = null)
		{
			@object.ThrowWhen(
				when: obj => obj == null,
				what: new ArgumentNullException(nameof(@object)));

			this.@object = @object;
			this.releaser = releaser;

			if (releaser == null)
			{
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Finalizes the object
		/// </summary>
		~SafeObject()
		{
			Dispose(false);
		}
		#endregion

		#region Methods

		/// <summary>
		/// Return string representation of the object
		/// </summary>
		/// <returns>String representation of the object</returns>
		public override string ToString() =>
			this.Object.ToString();

		/// <summary>
		/// Disposes the object, and release resources.
		/// </summary>
		/// <param name="disposing">
		/// True if the object is disposed in a good fashion (ie. by calling the Dispose() method).
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.IsDisposed)
			{
				GC.SuppressFinalize(this);
				this.IsDisposed = true;
				Release();

				if (disposing)
				{
					OnDisposed();
				}
			}
		}

		/// <summary>
		/// Raise the Disposed event
		/// </summary>
		protected void OnDisposed()
		{
			this.Disposed?.Invoke(this, EventArgs.Empty);
		}

		private void Release()
		{
			this.releaser?.Invoke(this.Object);
		}

		/// <summary>
		/// Throws ObjectDisposedException if the object is disposed.
		/// </summary>
		protected void ThrowIfDisposed()
		{
			if (this.IsDisposed)
			{
				throw new ObjectDisposedException(string.Format("SafeObject<{0}>", typeof(T).FullName));
			}
		}

		/// <summary>
		/// Disposes the object, and release resources.
		/// </summary>
		public void Dispose() =>
			Dispose(true);
		#endregion
	}
}
