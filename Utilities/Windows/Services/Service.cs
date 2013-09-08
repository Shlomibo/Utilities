using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utilities.Windows.Interop;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	public class Service : IDisposable
	{
		#region Consts

		private static readonly Dictionary<int, string> MSGS_DELETE_SERVICE = new Dictionary<int, string>()
		{
			{ API.ERROR_ACCESS_DENIED, "The handle does not have the DELETE access right." },
			{ API.ERROR_INVALID_HANDLE, "The specified handle is invalid." },
			{ API.ERROR_SERVICE_MARKED_FOR_DELETE, "The specified service has already been marked for deletion." },
		};

		private static readonly Dictionary<int, string> MSGS_CREATE_SERVICE = new Dictionary<int, string>()
		{
			{ API.ERROR_ACCESS_DENIED, 
				"The handle to the SCM database does not have the SC_MANAGER_CREATE_SERVICE access right." },
			{ API.ERROR_CIRCULAR_DEPENDENCY, "A circular service dependency was specified." },
			{ API.ERROR_DUPLICATE_SERVICE_NAME, 
				"The display name already exists in the service control manager database either as a service name or as another display name." },
			{ API.ERROR_INVALID_HANDLE, "The handle to the specified service control manager database is invalid." },
			{ API.ERROR_INVALID_NAME, "The specified service name is invalid." },
			{ API.ERROR_INVALID_PARAMETER, "A parameter that was specified is invalid." },
			{ API.ERROR_INVALID_SERVICE_ACCOUNT, 
				"The user account name specified in the serviceAccount parameter does not exist." },
			{ API.ERROR_SERVICE_EXISTS, "The specified service already exists in this database." },
			{ API.ERROR_SERVICE_MARKED_FOR_DELETE, 
				"The specified service already exists in this database and has been marked for deletion." },
		};

		private static readonly Dictionary<int, string> MSGS_LOAD_CNFG = new Dictionary<int, string>()
		{
			{ API.ERROR_ACCESS_DENIED, "The handle does not have the SERVICE_QUERY_CONFIG access right." },
			{ API.ERROR_INVALID_HANDLE, "The specified handle is invalid." },
		};

		private static readonly Dictionary<int, string> MSGS_SET_CNFG = new Dictionary<int, string>()
		{
			{ API.ERROR_ACCESS_DENIED, "The handle does not have the SERVICE_CHANGE_CONFIG access right." },
			{ API.ERROR_CIRCULAR_DEPENDENCY, "A circular service dependency was specified." },
			{ API.ERROR_DUPLICATE_SERVICE_NAME, "The display name already exists in the service controller manager database, " +
				"either as a service name or as another display name." },
			{ API.ERROR_INVALID_HANDLE, "The specified handle is invalid." },
			{ API.ERROR_INVALID_PARAMETER, "A parameter that was specified is invalid." },
			{ API.ERROR_INVALID_SERVICE_ACCOUNT, "The account name does not exist, " +
				"or a service is specified to share the same binary file as an already installed service but with " +
				"an account name that is not the same as the installed service." },
			{ API.ERROR_SERVICE_MARKED_FOR_DELETE, "The service has been marked for deletion." },
		};
		#endregion

		#region Fields

		internal uint? tagId;
		private bool isInitialized = false;
		private Lazy<string> serviceName;
		private Lazy<string> displayName;
		#endregion

		#region Properties

		public string ServiceName
		{
			get { return this.serviceName.Value; }
			private set { this.serviceName = new Lazy<string>(() => value); }
		}

		public string DisplayName
		{
			get { return this.displayName.Value; }
			set
			{
				this.displayName = new Lazy<string>(() => value);

				if (this.isInitialized)
				{
					SetConfig(
						ServiceType.NoChange,
						StartType.NoChange,
						ErrorControl.NoChange,
						null,
						null,
						null,
						null,
						null,
						value);
				}
			}
		}

		public ServiceType Type { get; set; }
		public StartType StartType { get; set; }
		public ErrorControl ErrorControl { get; set; }
		public string BinaryPath { get; set; }
		public string LoadOrderGroup { get; set; }
		public uint Tag { get; set; }
		public ICollection<string> Dependencies { get; private set; }
		public string AccountName { get; set; }
		public string Password { private get; set; }
		public bool IsAutoStartDelayed { get; set; }
		public string Description { get; set; }
		public ServiceFailureActions FailureActions { get; set; }
		public ushort? PreferedNodeId { get; set; }
		public int PreShutdownTimeout { get; set; }
		public ICollection<string> RequiredPrivileges { get; private set; }
		public SidType SidType { get; set; }
		public ICollection<Trigger> Triggers { get; private set; }
		public LaunchProtected LaunchProtection { get; set; }

		internal IntPtr Handle { get; private set; }
		public ServiceControl Scm { get; private set; }
		#endregion

		#region Ctor

		private Service()
		{
			this.serviceName = new Lazy<string>(() => LoadAndGet(() => this.ServiceName));
			this.displayName = new Lazy<string>(() => LoadAndGet(() => this.DisplayName));
		}

		internal Service(
			ServiceControl scm,
			IntPtr? handle,
			string name = null,
			string displayName = null,
			StartType? startType = null,
			ErrorControl? errorControl = null,
			string binaryPath = null,
			string loadOrderGroup = null,
			uint? Tag = null,
			ICollection<string> dependencies = null,
			string accountName = null,
			string password = null,
			bool? isAutoStartDelayed = null,
			string description = null,
			ServiceFailureActions failureActions = null,
			ushort? preferedNodeId = null,
			int? preShutdownTimeout = null,
			ICollection<string> requiredPrivileges = null,
			SidType? sidType = null,
			ICollection<Trigger> triggers = null,
			LaunchProtected? launchProtection = null)
			: this()
		{
			if (scm == null)
			{
				throw new ArgumentNullException("scm");
			}

			if ((handle == null) && (name == null) && (displayName == null))
			{
				throw new ArgumentNullException(
					"handle, name, displayName",
					"'handle', 'name' and 'displayName' cannot be all null");
			}

			if (handle != null)
			{
				this.Handle = handle.Value;
			}
			else
			{
				if (name == null)
				{
					name = scm.GetServiceName(displayName);
				}

				handle = scm.GetServiceHandle(name);
			}

			this.Scm = scm;

			if (name != null)
			{
				this.ServiceName = name;
			}

			if (displayName != null)
			{
				this.DisplayName = displayName;
			}

			if (startType != null)
			{
				this.StartType = startType.Value;
			}

			if (errorControl != null)
			{
				this.ErrorControl = errorControl.Value;
			}

			if (binaryPath != null)
			{
				this.BinaryPath = binaryPath;
			}

			if (loadOrderGroup != null)
			{
				this.LoadOrderGroup = null;
			}

			if (tagId != null)
			{
				this.Tag = tagId.Value;
			}

			if (dependencies != null)
			{
				this.Dependencies = dependencies;
			}

			if (accountName != null)
			{
				this.AccountName = accountName;
				this.Password = password;
			}

			if (isAutoStartDelayed != null)
			{
				this.IsAutoStartDelayed = isAutoStartDelayed.Value;
			}

			if (description != null)
			{
				this.Description = description;
			}

			if (failureActions != null)
			{
				this.FailureActions = failureActions;
			}

			if (preferedNodeId != null)
			{
				this.PreferedNodeId = preferedNodeId.Value;
			}

			if (preShutdownTimeout != null)
			{
				this.PreShutdownTimeout = preShutdownTimeout.Value;
			}

			if (requiredPrivileges != null)
			{
				this.RequiredPrivileges = requiredPrivileges;
			}

			if (sidType != null)
			{
				this.SidType = sidType.Value;
			}

			if (triggers != null)
			{
				this.Triggers = triggers;
			}

			if (launchProtection != null)
			{
				this.LaunchProtection = launchProtection.Value;
			}

			this.isInitialized = true;
		}
		#endregion

		#region Methods

		private unsafe void SetConfig(
			ServiceType type,
			StartType startType,
			ErrorControl errorControl,
			string binaryPath,
			string loadOrderGroup,
			MultiString dependencies,
			string accountName,
			string password,
			string displayName)
		{
			string dependenciesString = dependencies != null
				? dependencies.ToString()
				: null;
			uint newTagId = 0;

			fixed (char* lpDependencies = dependenciesString)
			{
				if (!API.ChangeServiceConfig(
					this.Handle,
					type,
					startType,
					errorControl,
					binaryPath,
					loadOrderGroup,
					loadOrderGroup != null
						? &newTagId
						: null,
					lpDependencies,
					accountName,
					password,
					displayName))
				{
					throw ExceptionCreator.Create(MSGS_SET_CNFG, Marshal.GetLastWin32Error());
				}

				if (loadOrderGroup != null)
				{
					this.Tag = newTagId;
				}
			}
		}

		private unsafe T LoadAndGet<T>(Func<T> getFunc)
		{
			QueryServiceConfig* pQSC = null;

			try
			{
				uint allocated = 0;
				uint needed = 0;
				int lastError;

				do
				{
					if (needed != 0)
					{
						if (pQSC == null)
						{
							pQSC = (QueryServiceConfig*)Marshal.AllocHGlobal((int)needed);
						}
						else
						{
							pQSC = (QueryServiceConfig*)Marshal.ReAllocHGlobal((IntPtr)pQSC, (IntPtr)needed);
						}

						allocated = needed;
					}

					if (API.QueryServiceConfig(this.Handle, pQSC, allocated, out needed))
					{
						lastError = API.ERROR_SUCCESS;
					}
					else
					{
						lastError = Marshal.GetLastWin32Error();
					}
				}
				while (lastError == API.ERROR_INSUFFICIENT_BUFFER);

				if (lastError != API.ERROR_SUCCESS)
				{
					throw ExceptionCreator.Create(MSGS_LOAD_CNFG, lastError);
				}

				this.Type = pQSC->type;
				this.StartType = pQSC->startType;
				this.ErrorControl = pQSC->errorControl;

				this.BinaryPath = pQSC->lpBinaryPathName != null
					? new string(pQSC->lpBinaryPathName)
					: null;

				this.LoadOrderGroup = pQSC->lpLoadOrderGroup != null
					? new string(pQSC->lpLoadOrderGroup)
					: null;

				this.Tag = pQSC->tagId;

				var multiString = new MultiString(pQSC->lpDependencies);
				multiString.Changed += Dependencies_Changed;
				this.Dependencies = multiString;

				this.AccountName = new string(pQSC->lpServiceStartName);
				this.DisplayName = new string(pQSC->lpDisplayName);
				this.ServiceName = this.Scm.GetServiceName(this.DisplayName);

				return getFunc();
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pQSC);
			}
		}

		public static unsafe Service CreateService(
			ServiceControl scm,
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
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			var dependenciesMultiStr = new MultiString(dependencies);
			uint tagId = 0;

			fixed (char* pDependencies = dependenciesMultiStr.ToString())
			{
				IntPtr hService = API.CreateService(
					scm.Handle,
					name,
					displayName,
					desiredAccess,
					type,
					startType,
					errorControl,
					binaryPath,
					loadOrderGroup,
					&tagId,
					pDependencies,
					serviceAccount,
					password);

				if (hService == IntPtr.Zero)
				{
					throw ExceptionCreator.Create(MSGS_CREATE_SERVICE, Marshal.GetLastWin32Error());
				}

				return new Service(
					scm,
					hService,
					name,
					displayName,
					startType,
					errorControl,
					binaryPath,
					loadOrderGroup,
					tagId,
					dependencies.ToArray(),
					serviceAccount,
					password);
			}
		}

		private void Dependencies_Changed(object sender, EventArgs e)
		{
			SaveDependencies();
		}

		private void SaveDependencies()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
