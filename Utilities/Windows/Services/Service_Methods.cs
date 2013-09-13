using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using Utilities.Windows.Interop;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	partial class Service
	{
		#region Methods

		private void HookEvents()
		{
			if (this.events == null)
			{
				this.events = new ServiceEvents(
					this.Handle,
					Notification.ServiceNotification);
				this.events.ServiceNotification += Events_ServiceNotification;
			}
		}

		private void Events_ServiceNotification(object sender, ServiceNotificationEventArgs e)
		{
			OnEvent(e.Event, e.Status);
		}

		private unsafe ServiceStatus GetServiceStatus()
		{
			ServiceStatusProcess ssp = new ServiceStatusProcess();
			uint stub;

			if (!Win32API.QueryServiceStatus(
				this.Handle,
				Win32API.SC_STATUS_PROCESS_INFO,
				&ssp,
				(uint)sizeof(ServiceStatusProcess),
				out stub))
			{
				throw ExceptionCreator.Create(MSGS_SERVICE_STATUS, Marshal.GetLastWin32Error());
			}

			return new ServiceStatus(ssp);
		}

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

		private unsafe void RequiredPrivileges_Changing(object sender, CancelEventArgs e)
		{
			string requiredPrivileges = this.RequiredPrivileges.ToString();

			fixed (char* pmszRequiredPrivileges = requiredPrivileges)
			{
				ServiceRequiredPrivilegesInfo srpi = new ServiceRequiredPrivilegesInfo
				{
					requiredPrivileges = pmszRequiredPrivileges,
				};

				if (!Win32API.ChangeServiceOptionalConfig(this.Handle, Config.RequiredPrivilegesInfo, &srpi))
				{
					e.Canceled = true;
					throw new ServiceException(Marshal.GetLastWin32Error());
				}
			}
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
				this.requiredPrivileges.Changing += RequiredPrivileges_Changing;
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
			uint? tag = null,
			string dependenciesMultiString = null,
			string accountName = null,
			string password = null,
			string displayName = null)
		{
			uint newTagId = tag ?? 0;

			fixed (char* lpDependencies = dependenciesMultiString)
			{
				if (!Win32API.ChangeServiceConfig(
					this.Handle,
					type,
					startType,
					errorControl,
					binaryPath,
					loadOrderGroup,
					tag != null
						? &newTagId
						: null,
					lpDependencies,
					accountName,
					password,
					displayName))
				{
					throw ExceptionCreator.Create(MSGS_SET_CNFG, Marshal.GetLastWin32Error());
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
				multiString.Changing += Dependencies_Changing;
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

		internal static unsafe Service CreateService(
			ServiceControlManager scm,
			string name,
			string displayName,
			AccessRights desiredAccess,
			ServiceType type,
			StartType startType,
			ErrorControl errorControl,
			string binaryPath,
			string loadOrderGroup,
			uint? tagId,
			IEnumerable<string> dependencies,
			string serviceAccount,
			string password)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			var dependenciesMultiStr = new MultiString(dependencies);
			uint newTag = tagId ?? 0;

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
					tagId.HasValue 
						? &newTag
						: null,
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
					newTag,
					dependencies.ToArray(),
					serviceAccount,
					password);
			}
		}

		private void Dependencies_Changing(object sender, CancelEventArgs e)
		{
			string dependencies = this.dependencies.ToString();

			try
			{
				SetConfig(dependenciesMultiString: dependencies);
			}
			catch
			{
				e.Canceled = true;
				throw;
			}
		}

		void IDisposable.Dispose()
		{
			Close();
		}

		/// <summary>
		/// Closes the connection to the service, and releases unmanaged resources.
		/// </summary>
		public void Close()
		{
			if (!IsDisposed)
			{
				Dispose(true);
				this.IsDisposed = true;
				GC.SuppressFinalize(this);
			}
		}

		/// <summary>
		/// Releases unmanaged resources.
		/// </summary>
		/// <param name="disposed">Value indicates if the object is disposed correctly</param>
		protected virtual void Dispose(bool disposed)
		{
			Win32API.CloseServiceHandle(this.Handle);

			if (this.events != null)
			{
				this.events.Dispose();
			}
		}

		/// <summary>
		/// Sends a control code to a service.
		/// </summary>
		/// <param name="control">The control code for the service.</param>
		/// <param name="stopReason">
		/// The reason for changing the service status to Stop. 
		/// If the current control code is not Stop, this member is ignored.
		/// </param>
		/// <param name="stopComment">
		/// An optional string that provides additional information about the service stop. 
		/// </param>
		/// <returns>The reported status of the service after the control was processed.</returns>
		public unsafe ServiceStatus SendControl(
			ServiceControlCode control,
			StopReasonFlag stopReason = StopReasonFlag.NoReason,
			string stopComment = null)
		{
			ThrowIfDisposed();

			fixed (char* lpStopComment = stopComment)
			{
				ServiceControlStatusReasonParams scsrp = new ServiceControlStatusReasonParams
				{
					comment = lpStopComment,
					reason = stopReason,
					serviceStatus = new ServiceStatusProcess(),
				};

				if (!Win32API.ControlService(
					this.Handle,
					control.Code,
					Win32API.SERVICE_CONTROL_STATUS_REASON_INFO,
					&scsrp))
				{
					throw ExceptionCreator.Create(MSGS_SEND_CTRL, Marshal.GetLastWin32Error());
				}

				return new ServiceStatus(scsrp.serviceStatus);
			}
		}

		/// <summary>
		/// Marks the specified service for deletion from the service control manager database.
		/// </summary>
		public void Delete()
		{
			ThrowIfDisposed();

			if (!Win32API.DeleteService(this.Handle))
			{
				throw ExceptionCreator.Create(MSGS_DELETE_SERVICE, Marshal.GetLastWin32Error());
			}
		}

		/// <summary>
		/// Starts a service.
		/// </summary>
		/// <param name="args">
		/// The null-terminated strings to be passed to the ServiceMain function for the service as arguments.
		/// 
		/// If there are no arguments, this parameter can be NULL. 
		/// Otherwise, the first argument (args[0]) is the name of the service, 
		/// followed by any additional arguments 
		/// (args[1] through args[Length-1]).
		/// </param>
		public unsafe void StartService(string[] args = null)
		{
			char** lpszArgs = null;
			int count = 0;

			try
			{
				if ((args != null) && (args.Length > 0))
				{
					var multiStringArgs = new MultiString(args);
					string argsString = multiStringArgs.ToString();
					count = args.Length;

					lpszArgs = (char**)Marshal.AllocHGlobal(sizeof(char*) * args.Length);

					fixed (char* lpArgsString = argsString)
					{
						for (int i = 0, charIndex = 0;
							i < args.Length;
							i++, charIndex += args[i].Length + 1)
						{
							lpszArgs[i] = lpArgsString + charIndex;
						}
					}
				}

				if (!Win32API.StartService(this.Handle, (uint)count, lpszArgs))
				{
					throw ExceptionCreator.Create(MSGS_START_SVC, Marshal.GetLastWin32Error());
				}
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)lpszArgs);
			}
		}
		#endregion
	}
}