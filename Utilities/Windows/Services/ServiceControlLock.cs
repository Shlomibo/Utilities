using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	[Obsolete("This functionality has no effect as of Windows Vista.")]
	public class ServiceControlLock : IDisposable
	{
#pragma warning disable 612, 618

		#region consts

		private static readonly Dictionary<int, string> MSGS_LOCK_ERRORS = new Dictionary<int, string>()
		{
			{ API.ERROR_ACCESS_DENIED, "The handle does not have the SC_MANAGER_LOCK access right." },
			{ API.ERROR_INVALID_HANDLE, "The specified handle is not valid." },
			{ API.ERROR_SERVICE_DATABASE_LOCKED, "The database is locked." },
		};

		private static readonly Dictionary<int, string> MSGS_UNLOCK_ERRORS = new Dictionary<int, string>()
		{
			{ API.ERROR_INVALID_SERVICE_LOCK, "The specified lock is invalid." },
		};
		#endregion

		#region Fields

		private IntPtr scLock;
		#endregion

		#region Properties

		public bool IsUnlocked { get; private set; }
		#endregion

		#region Ctor

		public ServiceControlLock(ServiceControl scm)
		{
			this.scLock = API.LockServiceDatabase(scm.Handle);

			if (this.scLock == IntPtr.Zero)
			{
				throw ExceptionCreator.Create(MSGS_LOCK_ERRORS, Marshal.GetLastWin32Error());
			}
		}

		~ServiceControlLock()
		{
			Dispose(false);
		}
		#endregion

		#region Methods

		public static unsafe ServiceLockStatus QueryLockStatus(ServiceControl scm)
		{
			QueryServiceLockStatus* pQSLS = null;

			try
			{
				int allocated = Marshal.SizeOf(typeof(QueryServiceLockStatus));
				pQSLS = (QueryServiceLockStatus*)Marshal.AllocHGlobal(allocated);
				uint needed = 0;
				int lastError;

				do
				{
					if (API.QueryServiceLockStatus(scm.Handle, pQSLS, (uint)allocated, out needed))
					{
						lastError = API.ERROR_SUCCESS;
					}
					else
					{
						lastError = Marshal.GetLastWin32Error();
					}

					if (lastError == API.ERROR_INSUFFICIENT_BUFFER)
					{
						allocated = (int)needed;
						pQSLS = (QueryServiceLockStatus*)Marshal.ReAllocHGlobal((IntPtr)pQSLS, (IntPtr)allocated);
					}
				} while (lastError == API.ERROR_INSUFFICIENT_BUFFER);

				return new ServiceLockStatus(*pQSLS);
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pQSLS);
			}
		}

		void IDisposable.Dispose()
		{
			Unlock();
		}

		public void Unlock()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.IsUnlocked)
			{
				this.IsUnlocked = true;

				if (!API.UnlockServiceDatabase(this.scLock) && disposing)
				{
					throw ExceptionCreator.Create(MSGS_UNLOCK_ERRORS, Marshal.GetLastWin32Error());
				} 
			}
		} 
		#endregion

#pragma warning restore 612, 618
	}
}
