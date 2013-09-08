using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			{ Win32API.ERROR_ACCESS_DENIED, "The handle does not have the DELETE access right." },
			{ Win32API.ERROR_INVALID_HANDLE, "The specified handle is invalid." },
			{ Win32API.ERROR_SERVICE_MARKED_FOR_DELETE, "The specified service has already been marked for deletion." },
		};

		private static readonly Dictionary<int, string> MSGS_CREATE_SERVICE = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_ACCESS_DENIED, 
				"The handle to the SCM database does not have the SC_MANAGER_CREATE_SERVICE access right." },
			{ Win32API.ERROR_CIRCULAR_DEPENDENCY, "A circular service dependency was specified." },
			{ Win32API.ERROR_DUPLICATE_SERVICE_NAME, 
				"The display name already exists in the service control manager database either as a service name or as another display name." },
			{ Win32API.ERROR_INVALID_HANDLE, "The handle to the specified service control manager database is invalid." },
			{ Win32API.ERROR_INVALID_NAME, "The specified service name is invalid." },
			{ Win32API.ERROR_INVALID_PARAMETER, "A parameter that was specified is invalid." },
			{ Win32API.ERROR_INVALID_SERVICE_ACCOUNT, 
				"The user account name specified in the serviceAccount parameter does not exist." },
			{ Win32API.ERROR_SERVICE_EXISTS, "The specified service already exists in this database." },
			{ Win32API.ERROR_SERVICE_MARKED_FOR_DELETE, 
				"The specified service already exists in this database and has been marked for deletion." },
		};

		private static readonly Dictionary<int, string> MSGS_LOAD_CNFG = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_ACCESS_DENIED, "The handle does not have the SERVICE_QUERY_CONFIG access right." },
			{ Win32API.ERROR_INVALID_HANDLE, "The specified handle is invalid." },
		};

		private static readonly Dictionary<int, string> MSGS_SET_CNFG = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_ACCESS_DENIED, "The handle does not have the SERVICE_CHANGE_CONFIG access right." },
			{ Win32API.ERROR_CIRCULAR_DEPENDENCY, "A circular service dependency was specified." },
			{ Win32API.ERROR_DUPLICATE_SERVICE_NAME, "The display name already exists in the service controller manager database, " +
				"either as a service name or as another display name." },
			{ Win32API.ERROR_INVALID_HANDLE, "The specified handle is invalid." },
			{ Win32API.ERROR_INVALID_PARAMETER, "A parameter that was specified is invalid." },
			{ Win32API.ERROR_INVALID_SERVICE_ACCOUNT, "The account name does not exist, " +
				"or a service is specified to share the same binary file as an already installed service but with " +
				"an account name that is not the same as the installed service." },
			{ Win32API.ERROR_SERVICE_MARKED_FOR_DELETE, "The service has been marked for deletion." },
		};
		#endregion

		#region Fields

		private bool isInitialized = false;
		private Lazy<string> serviceName;
		private Lazy<string> displayName;
		private Lazy<ServiceType> type;
		private Lazy<StartType> startType;
		private Lazy<ErrorControl> errorControl;
		private Lazy<string> binaryPath;
		private Lazy<string> loadOrderGroup;
		private MultiString dependencies;
		private Lazy<string> accountName;
		private Lazy<bool> isAutoStartDelayed;
		private Lazy<string> description;
		private Lazy<ushort?> preferedNodeId;
		private Lazy<int> preShutdownTimeout;
		private MultiString requiredPrivileges;
		private Lazy<SidType> sidType;
		private Lazy<ReadOnlyCollection<Trigger>> triggers;
		private Lazy<LaunchProtected> launchProtection;
		#endregion

		#region Properties

		public bool IsDisposed { get; private set; }

		public string ServiceName
		{
			get
			{
				ThrowIfDisposed();
				return this.serviceName.Value;
			}
			private set { this.serviceName = new Lazy<string>(() => value); }
		}

		public string DisplayName
		{
			get
			{
				ThrowIfDisposed();
				return this.displayName.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					SetConfig(displayName: value);
				}

				this.displayName = new Lazy<string>(() => value);
			}
		}

		public ServiceType Type
		{
			get
			{
				ThrowIfDisposed();
				return this.type.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					SetConfig(type: value);
				}

				this.type = new Lazy<ServiceType>(() => value);
			}
		}

		public StartType StartType
		{
			get
			{
				ThrowIfDisposed();
				return this.startType.Value;
			}
			set
			{
				ThrowIfDisposed();
				if (this.isInitialized)
				{
					SetConfig(startType: value);
				}

				this.startType = new Lazy<StartType>(() => value);
			}
		}

		public ErrorControl ErrorControl
		{
			get
			{
				ThrowIfDisposed();
				return this.errorControl.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					SetConfig(errorControl: value);
				}

				this.errorControl = new Lazy<ErrorControl>(() => value);
			}
		}

		public string BinaryPath
		{
			get
			{
				ThrowIfDisposed();
				return this.binaryPath.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					SetConfig(binaryPath: value);
				}

				this.binaryPath = new Lazy<string>(() => value);
			}
		}

		public string LoadOrderGroup
		{
			get
			{
				ThrowIfDisposed();
				return this.loadOrderGroup.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					SetConfig(loadOrderGroup: value);
				}

				this.loadOrderGroup = new Lazy<string>(() => value);
			}
		}

		public uint Tag { get; set; }

		public ICollection<string> Dependencies
		{
			get
			{
				ThrowIfDisposed();

				if (this.dependencies == null)
				{
					LoadAndGet<object>(() => null);
				}

				return this.dependencies;
			}
			private set
			{
				if (this.dependencies != null)
				{
					this.dependencies.Changed -= Dependencies_Changed;
				}

				this.dependencies = value != null
					? new MultiString(value)
					: new MultiString();

				this.dependencies.Changed += Dependencies_Changed;
			}
		}

		public string AccountName
		{
			get
			{
				ThrowIfDisposed();
				return this.accountName.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					SetConfig(accountName: value);
				}

				this.accountName = new Lazy<string>(() => value);
			}
		}

		public string Password
		{
			private get { return null; }
			set
			{
				ThrowIfDisposed();

				if (this.isInitialized)
				{
					SetConfig(password: value);
				}
			}
		}
		public bool IsAutoStartDelayed
		{
			get
			{
				ThrowIfDisposed();
				return this.isAutoStartDelayed.Value;
			}
			set
			{
				unsafe
				{
					ThrowIfDisposed();

					ServiceDelayedAutoStartInfo sdasi = new ServiceDelayedAutoStartInfo
					{
						isAutoStartDelayed = value,
					};

					if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.DelayedAutoStartInfo, &sdasi))
					{
						throw new ServiceException(Marshal.GetLastWin32Error());
					}

					this.isAutoStartDelayed = new Lazy<bool>(() => value);
				}
			}
		}

		public string Description
		{
			get
			{
				ThrowIfDisposed();
				return this.description.Value;
			}
			set
			{
				ThrowIfDisposed();

				unsafe
				{
					fixed (char* lpDescription = value)
					{
						ServiceDescription sd = new ServiceDescription
						{
							lpDescription = lpDescription,
						};

						if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.Description, &sd))
						{
							throw new ServiceException(Marshal.GetLastWin32Error());
						}
					}
				}

				this.description = new Lazy<string>(() => value);
			}
		}

		public FailureActions FailureActions { get; private set; }

		public ushort? PreferedNodeId
		{
			get
			{
				ThrowIfDisposed();
				return this.preferedNodeId.Value;
			}
			set
			{
				ThrowIfDisposed();

				ServicePreferedNodeInfo spni = new ServicePreferedNodeInfo
				{
					isDeleted = !value.HasValue,
					preferedNode = value.HasValue
						? value.Value
						: default(ushort),
				};

				unsafe
				{
					if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.PreferredNode, &spni))
					{
						throw new ServiceException(Marshal.GetLastWin32Error());
					}
				}

				this.preferedNodeId = new Lazy<ushort?>(() => value);
			}
		}

		public int PreShutdownTimeout
		{
			get
			{
				ThrowIfDisposed();
				return this.preShutdownTimeout.Value;
			}
			set
			{
				ThrowIfDisposed();

				if (value < 0)
				{
					throw new ArgumentException("PreShutdownTimeout cannot be less than zero", "PreShutdownTimeout");
				}

				var spsi = new ServicePreShutdownInfo
				{
					timeout = (uint)value,
				};

				unsafe
				{
					if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.PreshutdownInfo, &spsi))
					{
						throw new ServiceException(Marshal.GetLastWin32Error());
					}
				}

				this.preShutdownTimeout = new Lazy<int>(() => value);
			}
		}

		public ICollection<string> RequiredPrivileges
		{
			get
			{
				ThrowIfDisposed();

				if (this.requiredPrivileges == null)
				{
					LoadRquiredPrivileges();
				}

				return this.requiredPrivileges;
			}
			private set
			{
				if (this.requiredPrivileges != null)
				{
					this.requiredPrivileges.Changed -= RequiredPrivileges_Changed;
				}

				this.requiredPrivileges = value != null
					? new MultiString(value)
					: new MultiString();

				if (this.requiredPrivileges != null)
				{
					this.requiredPrivileges.Changed += RequiredPrivileges_Changed;
				}
			}
		}

		public SidType SidType
		{
			get
			{
				ThrowIfDisposed();
				return this.sidType.Value;
			}
			set
			{
				ThrowIfDisposed();

				ServiceSidInfo ssi = new ServiceSidInfo
				{
					serviceSidType = value,
				};

				unsafe
				{
					if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.ServiceSidInfo, &ssi))
					{
						throw new ServiceException(Marshal.GetLastWin32Error());
					}
				}

				this.sidType = new Lazy<SidType>(() => value);
			}
		}

		public ReadOnlyCollection<Trigger> Triggers
		{
			get
			{
				ThrowIfDisposed();
				return this.triggers.Value;
			}
			set
			{
				ThrowIfDisposed();

				unsafe
				{
					ServiceTriggerInfo sti = new ServiceTriggerInfo();

					try
					{
						sti.triggers = (ServiceTrigger*)Marshal.AllocHGlobal(
							sizeof(ServiceTrigger) * value.Count);
						sti.triggersCount = (uint)value.Count;

						for (int i = 0; i < value.Count; i++)
						{
							value[i].ToUnmanaged(ref sti.triggers[i]);
						}

						if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.TriggerInfo, &sti))
						{
							throw new ServiceException(Marshal.GetLastWin32Error());
						}

						this.triggers = new Lazy<ReadOnlyCollection<Trigger>>(() => value);
					}
					finally
					{
						if (sti.triggers != null)
						{
							for (int i = 0; i < value.Count; i++)
							{
								Trigger.FreeUnmanaged(ref sti.triggers[i]);
							}

							Marshal.FreeHGlobal((IntPtr)sti.triggers);
						}
					}
				}
			}
		}

		public LaunchProtected LaunchProtection
		{
			get
			{
				ThrowIfDisposed();
				return this.launchProtection.Value;
			}
			set
			{
				ThrowIfDisposed();

				ServiceLaunchProtectedInfo slpi = new ServiceLaunchProtectedInfo
				{
					launchProtected = value,
				};

				unsafe
				{
					if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.LaunchProtected, &slpi))
					{
						throw new ServiceException(Marshal.GetLastWin32Error());
					}
				}

				this.launchProtection = new Lazy<LaunchProtected>(() => value);
			}
		}

		internal IntPtr Handle { get; private set; }
		public ServiceControl Scm { get; private set; }
		#endregion

		#region Ctor

		private Service()
		{
			this.serviceName = new Lazy<string>(() => LoadAndGet(() => this.ServiceName));
			this.displayName = new Lazy<string>(() => LoadAndGet(() => this.DisplayName));
			this.type = new Lazy<ServiceType>(() => LoadAndGet(() => this.Type));
			this.startType = new Lazy<StartType>(() => LoadAndGet(() => this.StartType));
			this.errorControl = new Lazy<ErrorControl>(() => LoadAndGet(() => this.ErrorControl));
			this.binaryPath = new Lazy<string>(() => LoadAndGet(() => this.BinaryPath));
			this.loadOrderGroup = new Lazy<string>(() => LoadAndGet(() => this.LoadOrderGroup));
			this.Tag = 0;
			this.accountName = new Lazy<string>(() => LoadAndGet(() => this.AccountName));
			this.isAutoStartDelayed = new Lazy<bool>(GetIsAutoStartDelayed);
			this.description = new Lazy<string>(GetDescription);
			this.FailureActions = new FailureActions(this);
			this.preferedNodeId = new Lazy<ushort?>(GetPreferedNodeId);
			this.preShutdownTimeout = new Lazy<int>(GetPreShutdownTimeout);
			this.sidType = new Lazy<SidType>(GetSidType);
			this.triggers = new Lazy<ReadOnlyCollection<Trigger>>(GetTriggers);
			this.launchProtection = new Lazy<LaunchProtected>(GetLaunchProtection);
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
			uint? tag = null,
			ICollection<string> dependencies = null,
			string accountName = null,
			string password = null,
			bool? isAutoStartDelayed = null,
			string description = null,
			FailureActions failureActions = null,
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

			if (tag != null)
			{
				this.Tag = tag.Value;
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
				this.Triggers = Array.AsReadOnly((triggers as Trigger[]) ?? triggers.ToArray());
			}

			if (launchProtection != null)
			{
				this.LaunchProtection = launchProtection.Value;
			}

			this.isInitialized = true;
		}

		~Service()
		{
			Dispose(false);
		}
		#endregion

		#region Methods

		private unsafe LaunchProtected GetLaunchProtection()
		{
			ServiceLaunchProtectedInfo slpi = new ServiceLaunchProtectedInfo();
			uint stub;

			if (!Win32API.QueryServiceOptionalConfig(
				this.Handle, 
				Config.LaunchProtected, 
				&slpi, 
				(uint)sizeof(ServiceLaunchProtectedInfo), 
				out stub))
			{
				throw new ServiceException(Marshal.GetLastWin32Error());
			}

			return slpi.launchProtected;
		}

		private unsafe ReadOnlyCollection<Trigger> GetTriggers()
		{
			ServiceTriggerInfo* pST = null;
			uint allocated = 0;
			uint needed = 0;

			try
			{
				int lastError;

				do
				{
					if (needed != 0)
					{
						if (pST == null)
						{
							pST = (ServiceTriggerInfo*)Marshal.AllocHGlobal((int)needed);
						}
						else
						{
							pST = (ServiceTriggerInfo*)Marshal.ReAllocHGlobal((IntPtr)pST, (IntPtr)needed);
						}

						allocated = needed;
					}

					if (Win32API.ChangeServiceOptionalConfig(this.Handle, Config.TriggerInfo, pST))
					{
						lastError = Win32API.ERROR_SUCCESS;
					}
					else
					{
						lastError = Marshal.GetLastWin32Error();
					}
				} while (lastError == Win32API.ERROR_INSUFFICIENT_BUFFER);

				if (lastError != Win32API.ERROR_SUCCESS)
				{
					throw new ServiceException(lastError);
				}

				var triggers = new Trigger[pST->triggersCount];

				for (int i = 0; i < triggers.Length; i++)
				{
					triggers[i] = new Trigger(ref pST->triggers[i]);
				}

				return Array.AsReadOnly(triggers);
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pST);
			}
		}

		private unsafe SidType GetSidType()
		{
			ServiceSidInfo ssi = new ServiceSidInfo();
			uint stub;

			if (!Win32API.QueryServiceOptionalConfig(
				this.Handle,
				Config.ServiceSidInfo,
				&ssi,
				(uint)sizeof(ServiceSidInfo),
				out stub))
			{
				throw new ServiceException(Marshal.GetLastWin32Error());
			}

			return ssi.serviceSidType;
		}

		private void RequiredPrivileges_Changed(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		private unsafe void LoadRquiredPrivileges()
		{
			ServiceRequiredPrivilegesInfo* pSRP = null;
			uint allocated = 0;
			uint needed = 0;

			try
			{
				int lastError;

				do
				{
					if (needed != 0)
					{
						if (pSRP == null)
						{
							pSRP = (ServiceRequiredPrivilegesInfo*)Marshal.AllocHGlobal((int)needed);
						}
						else
						{
							pSRP = (ServiceRequiredPrivilegesInfo*)Marshal.ReAllocHGlobal(
								(IntPtr)pSRP,
								(IntPtr)needed);
						}

						allocated = needed;
					}

					if (Win32API.QueryServiceOptionalConfig(
						this.Handle,
						Config.RequiredPrivilegesInfo,
						pSRP,
						allocated,
						out needed))
					{
						lastError = Win32API.ERROR_SUCCESS;
					}
					else
					{
						lastError = Marshal.GetLastWin32Error();
					}
				} while (lastError == Win32API.ERROR_INSUFFICIENT_BUFFER);

				if (lastError != Win32API.ERROR_SUCCESS)
				{
					throw new ServiceException(lastError);
				}

				this.requiredPrivileges = new MultiString(pSRP->requiredPrivileges);
				this.requiredPrivileges.Changed += RequiredPrivileges_Changed;
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pSRP);
			}
		}

		private unsafe int GetPreShutdownTimeout()
		{
			var spsi = new ServicePreShutdownInfo();
			uint stub;

			if (!Win32API.QueryServiceOptionalConfig(
				this.Handle,
				Config.PreshutdownInfo,
				&spsi,
				(uint)sizeof(ServicePreShutdownInfo),
				out stub))
			{
				throw new ServiceException(Marshal.GetLastWin32Error());
			}

			return (int)spsi.timeout;
		}

		private unsafe ushort? GetPreferedNodeId()
		{
			ServicePreferedNodeInfo spni = new ServicePreferedNodeInfo();
			uint stub;

			if (!Win32API.QueryServiceOptionalConfig(
				this.Handle,
				Config.PreferredNode,
				&spni,
				(uint)sizeof(ServicePreferedNodeInfo),
				out stub))
			{
				throw new ServiceException(Marshal.GetLastWin32Error());
			}

			return !spni.isDeleted
				? (ushort?)spni.preferedNode
				: null;
		}

		private unsafe string GetDescription()
		{
			ServiceDescription* pSD = null;
			uint allocated = 0;
			uint needed = 0;

			try
			{
				int lastError;

				do
				{
					if (needed != 0)
					{
						if (pSD == null)
						{
							pSD = (ServiceDescription*)Marshal.AllocHGlobal((int)needed);
						}
						else
						{
							pSD = (ServiceDescription*)Marshal.ReAllocHGlobal((IntPtr)pSD, (IntPtr)needed);
						}

						allocated = needed;
					}

					if (Win32API.QueryServiceOptionalConfig(this.Handle, Config.Description, pSD, allocated, out needed))
					{
						lastError = Win32API.ERROR_SUCCESS;
					}
					else
					{
						lastError = Marshal.GetLastWin32Error();
					}
				} while (lastError == Win32API.ERROR_INSUFFICIENT_BUFFER);

				if (lastError != Win32API.ERROR_SUCCESS)
				{
					throw new ServiceException(lastError);
				}

				return new string(pSD->lpDescription);
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pSD);
			}
		}

		private unsafe bool GetIsAutoStartDelayed()
		{
			ServiceDelayedAutoStartInfo sdasi = new ServiceDelayedAutoStartInfo();
			uint stab;

			if (!Win32API.QueryServiceOptionalConfig(
				this.Handle,
				Config.DelayedAutoStartInfo,
				&sdasi,
				(uint)sizeof(ServiceDelayedAutoStartInfo),
				out stab))
			{
				throw new ServiceException(Marshal.GetLastWin32Error());
			}

			return sdasi.isAutoStartDelayed;
		}

		private unsafe void SetConfig(
			ServiceType type = ServiceType.NoChange,
			StartType startType = StartType.NoChange,
			ErrorControl errorControl = ErrorControl.NoChange,
			string binaryPath = null,
			string loadOrderGroup = null,
			MultiString dependencies = null,
			string accountName = null,
			string password = null,
			string displayName = null)
		{
			string dependenciesString = dependencies != null
				? dependencies.ToString()
				: null;
			uint newTagId = 0;

			fixed (char* lpDependencies = dependenciesString)
			{
				if (!Win32API.ChangeServiceConfig(
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

		internal void ThrowIfDisposed()
		{
			if (this.IsDisposed)
			{
				throw new ObjectDisposedException(this.serviceName.Value);
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

					if (Win32API.QueryServiceConfig(this.Handle, pQSC, allocated, out needed))
					{
						lastError = Win32API.ERROR_SUCCESS;
					}
					else
					{
						lastError = Marshal.GetLastWin32Error();
					}
				}
				while (lastError == Win32API.ERROR_INSUFFICIENT_BUFFER);

				if (lastError != Win32API.ERROR_SUCCESS)
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
				IntPtr hService = Win32API.CreateService(
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
			Dispose(true);
			this.IsDisposed = true;
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposed)
		{
			Win32API.CloseServiceHandle(this.Handle);
		}
		#endregion
	}
}
