using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utilities.Windows.Interop;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	public partial class ServiceControl : IDisposable
	{
		#region Consts

		private const AccessRights DEFAULT_ACCESS = AccessRights.ScmGenericRead;
		private const string DESFAULT_MACHINE_NAME = "localhost";

		private static readonly Dictionary<int, string> MSGS_CTOR = new Dictionary<int, string>()
		{
			{ API.ERROR_ACCESS_DENIED, "The requested access was denied." },
			{ API.ERROR_DATABASE_DOES_NOT_EXIST, "The specified database does not exist." },
		};

		private static readonly Dictionary<int, string> MSGS_NTFY_BOOT_CNFG_STTS = new Dictionary<int, string>()
		{
			{ API.ERROR_ACCESS_DENIED, "The user does not have permission to perform this operation. " +
				"Only the system and members of the Administrator's group can do so." },
		};

		private static readonly Dictionary<int, string> MSGS_OPEN_SVC = new Dictionary<int, string>()
		{
			{ API.ERROR_ACCESS_DENIED, "The handle does not have access to the service." },
			{ API.ERROR_INVALID_HANDLE, "The specified handle is invalid." },
			{ API.ERROR_INVALID_NAME, "The specified service name is invalid." },
			{ API.ERROR_SERVICE_DOES_NOT_EXIST, "The specified service does not exist." },
		};
		#endregion

		#region Fields

		private static WeakReference<ServiceControl> current = new WeakReference<ServiceControl>(null);
		#endregion

		#region Properties

		internal IntPtr Handle { get; private set; }
		public AccessRights AccessRights { get; private set; }

		public bool IsDisposed { get; private set; }

		public static ServiceControl Current
		{
			get
			{
				ServiceControl current;

				if (!ServiceControl.current.TryGetTarget(out current))
				{
					current = new ServiceControl();
					ServiceControl.current.SetTarget(current);
				}

				return current;
			}
		}

		public string MachineName { get; private set; }

		public ServiceCollection Services { get; private set; }
		#endregion

		#region Delegates

		private unsafe delegate bool GetServiceUniqueNameDelegate(
			IntPtr hSCManager,
			string inputName,
			char* pOutputName,
			ref uint buffSize);
		#endregion

		#region Ctor

		public ServiceControl(string machineName, AccessRights desiredAccess)
		{
			this.Handle = API.OpenSCManager(machineName, API.SERVICES_ACTIVE_DATABASE, desiredAccess);

			if (this.Handle == IntPtr.Zero)
			{
				throw ExceptionCreator.Create(MSGS_CTOR, Marshal.GetLastWin32Error());
			}

			if (string.IsNullOrEmpty(machineName))
			{
				machineName = DESFAULT_MACHINE_NAME;
			}

			this.MachineName = machineName;
			this.Services = new ServiceCollection(this);
		}

		public ServiceControl(string machineName)
			: this(machineName, DEFAULT_ACCESS) { }

		public ServiceControl(AccessRights desiredAccess)
			: this(null, desiredAccess) { }

		public ServiceControl() : this(DEFAULT_ACCESS) { }

		~ServiceControl()
		{
			Dispose(false);
		}
		#endregion

		#region Methods

		public static void NotifyBootConfigStatus(bool isBootAcceptable)
		{
			if (!API.NotifyBootConfigStatus(isBootAcceptable))
			{
				throw ExceptionCreator.Create(MSGS_NTFY_BOOT_CNFG_STTS, Marshal.GetLastWin32Error());
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.IsDisposed && (this.Handle != IntPtr.Zero))
			{
				this.IsDisposed = true;

				if (!API.CloseServiceHandle(this.Handle))
				{
					var exception = new ServiceException(Marshal.GetLastWin32Error());
					Trace.WriteLine(exception.ToString(), "Error");
				}
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public Service CreateService(
			string name,
			string displayName,
			AccessRights desiredAccess,
			ServiceType type,
			StartType startType,
			ErrorControl errorControl,
			string binaryPath,
			string loadOrderGroup,
			IEnumerable<string> dependencies,
			string serviceAccount,
			string password)
		{
			return Service.CreateService(
				this,
				name,
				displayName,
				desiredAccess,
				type,
				startType,
				errorControl,
				binaryPath,
				loadOrderGroup,
				dependencies,
				serviceAccount,
				password);
		}

		public unsafe string GetServiceDisplayName(string serviceName)
		{
			return GetServiceUniqueName(serviceName, API.GetServiceDisplayName);
		}

		private unsafe string GetServiceUniqueName(string inputName, GetServiceUniqueNameDelegate getServiceUniqueName)
		{
			char* pOutputName = null;
			int lastError;
			uint length = 0;

			try
			{
				do
				{
					if (length != 0)
					{
						length += (uint)Marshal.SizeOf(typeof(char));

						if (pOutputName == null)
						{
							pOutputName = (char*)Marshal.AllocHGlobal((int)length);
						}
						else
						{
							pOutputName = (char*)Marshal.ReAllocHGlobal((IntPtr)pOutputName, (IntPtr)length);
						}
					}

					if (getServiceUniqueName(this.Handle, inputName, pOutputName, ref length))
					{
						lastError = API.ERROR_SUCCESS;
					}
					else
					{
						lastError = Marshal.GetLastWin32Error();
					}
				} while (lastError == API.ERROR_INSUFFICIENT_BUFFER);

				if (lastError != API.ERROR_SUCCESS)
				{
					throw new ServiceException(lastError);
				}

				return new string(pOutputName);
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pOutputName);
			}
		}

		public unsafe string GetServiceName(string displayName)
		{
			return GetServiceUniqueName(displayName, API.GetServiceKeyName);
		}

		internal IntPtr GetServiceHandle(string serviceName)
		{
			return GetServiceHandle(serviceName, GetDefaultAccessRights());
		}

		internal IntPtr GetServiceHandle(string serviceName, AccessRights desiredAccess)
		{
			IntPtr hService = API.OpenService(this.Handle, serviceName, desiredAccess);

			if (hService == IntPtr.Zero)
			{
				throw ExceptionCreator.Create(MSGS_OPEN_SVC, Marshal.GetLastWin32Error());
			}

			return hService;
		}

		public Service OpenService(string serviceName)
		{
			return OpenService(serviceName, GetDefaultAccessRights());
		}

		public Service OpenService(string serviceName, AccessRights desiredAccess)
		{
			return new Service(this, GetServiceHandle(serviceName, desiredAccess), serviceName);
		}

		private AccessRights GetDefaultAccessRights()
		{
			var desiredAccess = AccessRights.SvcGenericRead;

			if ((this.AccessRights & AccessRights.ScmGeenricWrite) == AccessRights.ScmGeenricWrite)
			{
				desiredAccess |= AccessRights.SvcGenericWrite;
			}

			if ((this.AccessRights & AccessRights.ScmGenericExecute) == AccessRights.ScmGenericExecute)
			{
				desiredAccess |= AccessRights.SvcGenericExecute;
			}

			if ((this.AccessRights & AccessRights.ScmAllAccess) == AccessRights.ScmAllAccess)
			{
				desiredAccess |= AccessRights.SvcAllAccess;
			}

			return desiredAccess;
		}
		#endregion
	}
}
