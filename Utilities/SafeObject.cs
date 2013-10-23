using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Provides safe handling for an arbitrary object.
    /// </summary>
    /// <typeparam name="T">The type of the object to handle.</typeparam>
    public class SafeObject<T> : IDisposable
    {
        #region Fields

        private T @object;
        private Action<T> releaser; 
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
        public event EventHandler Disposed = (s, e) => { };
        #endregion

        #region Ctor

        /// <summary>
        /// Creates new instance of the object
        /// </summary>
        /// <param name="object">The object to handle.</param>
        /// <param name="releaser">Delegate to a function that releases resources of the object.</param>
        public SafeObject(T @object, Action<T> releaser)
        {
            this.@object = @object;
            this.releaser = releaser;

            if (releaser == null)
            {
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// Creates new instance of the object
        /// </summary>
        /// <param name="object">The object to handle.</param>
        public SafeObject(T @object) : this(@object, null) { }

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
        /// Disposes the object, and release resources.
        /// </summary>
        /// <param name="disposing">
        /// True if the object is disposed in a good fashion (ie. by calling the Dispose() method).
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            GC.SuppressFinalize(this);
            Release();
            this.IsDisposed = true;

            if (disposing)
            {
                OnDisposed();
            }
        }

        /// <summary>
        /// Raise the Disposed event
        /// </summary>
        protected void OnDisposed()
        {
            this.Disposed(this, EventArgs.Empty);
        }

        private void Release()
        {
            if (this.releaser != null)
            {
                this.releaser(this.@object);
            }
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
        public void Dispose()
        {
            Dispose(true);
        } 
        #endregion
    }
}
