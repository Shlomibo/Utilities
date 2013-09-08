using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	public class FailureActions
	{
		#region Fields

		private Service service;
		private Lazy<int> resetPeriod;
		private Lazy<string> rebootMessage;
		private Lazy<string> command;
		private Lazy<ReadOnlyCollection<ServiceControlAction>> actions;
		private Lazy<bool> isFailOnNonCrash;
		#endregion

		#region Properties

		public int ResetPeriod
		{
			get
			{
				this.service.ThrowIfDisposed();
				return this.resetPeriod.Value;
			}
			set
			{
				this.service.ThrowIfDisposed();

				SetFailureActions(resetPeriod: (uint)value);
				this.resetPeriod = new Lazy<int>(() => value);
			}
		}

		public string RebootMessage
		{
			get
			{
				this.service.ThrowIfDisposed();
				return this.rebootMessage.Value;
			}
			set
			{
				this.service.ThrowIfDisposed();

				SetFailureActions(rebootMessage: value);
				this.rebootMessage = new Lazy<string>(() => value);
			}
		}

		public string Comamnd
		{
			get
			{
				this.service.ThrowIfDisposed();
				return this.command.Value;
			}
			set
			{
				this.service.ThrowIfDisposed();

				SetFailureActions(command: value);
				this.command = new Lazy<string>(() => value);
			}
		}

		public ReadOnlyCollection<ServiceControlAction> Actions
		{
			get
			{
				this.service.ThrowIfDisposed();
				return this.actions.Value;
			}
			set
			{
				this.service.ThrowIfDisposed();

				SetFailureActions(actions: value);
				this.actions = new Lazy<ReadOnlyCollection<ServiceControlAction>>(() => value);
			}
		}
		public bool IsFailOnNonCrash
		{
			get
			{
				this.service.ThrowIfDisposed();
				return this.isFailOnNonCrash.Value;
			}
			set
			{
				unsafe
				{
					ServiceFailureActionsFlag sfaf = new ServiceFailureActionsFlag
					{
						failureActionsOnNonCrashFailures = value,
					};

					if (!Win32API.ChangeServiceOptionalConfig(
						this.service.Handle,
						Config.FailureActionsFlag,
						&sfaf))
					{
						throw new ServiceException(Marshal.GetLastWin32Error());
					}

					this.isFailOnNonCrash = new Lazy<bool>(() => value);
				}
			}
		}
		#endregion

		#region Events

		public event EventHandler Changed = (s, e) => { };
		#endregion

		#region Ctor

		internal FailureActions(Service service)
		{
			this.service = service;
			this.resetPeriod = new Lazy<int>(() => LoadAndGet(() => this.ResetPeriod));
			this.rebootMessage = new Lazy<string>(() => LoadAndGet(() => this.RebootMessage));
			this.command = new Lazy<string>(() => LoadAndGet(() => this.Comamnd));
			this.actions = new Lazy<ReadOnlyCollection<ServiceControlAction>>(
				() => LoadAndGet(() => this.Actions));
			this.isFailOnNonCrash = new Lazy<bool>(GetIsFailOnNonCrash);
		}
		#endregion

		#region Methods

		private unsafe bool GetIsFailOnNonCrash()
		{
			ServiceFailureActionsFlag sfaf = new ServiceFailureActionsFlag();
			uint stab;

			if (!Win32API.QueryServiceOptionalConfig(
				this.service.Handle,
				Config.FailureActionsFlag,
				&sfaf,
				(uint)sizeof(ServiceFailureActionsFlag),
				out stab))
			{
				throw new ServiceException(Marshal.GetLastWin32Error());
			}

			return sfaf.failureActionsOnNonCrashFailures;
		}

		private unsafe T LoadAndGet<T>(Func<T> getFunc)
		{
			ServiceFailureActions* pSFA = null;
			uint allocated = 0;
			uint needed = 0;

			try
			{
				int lastError;

				do
				{
					if (needed != 0)
					{
						if (pSFA == null)
						{
							pSFA = (ServiceFailureActions*)Marshal.AllocHGlobal((int)needed);
						}
						else
						{
							pSFA = (ServiceFailureActions*)Marshal.ReAllocHGlobal((IntPtr)pSFA, (IntPtr)needed);
						}

						allocated = needed;
					}

					if (Win32API.QueryServiceOptionalConfig(
						this.service.Handle,
						Config.FailureActions,
						pSFA, allocated,
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

				this.resetPeriod = new Lazy<int>(() => (int)pSFA->resetPeriod);

				this.RebootMessage = pSFA->lpRebootMessage != null
					? new string(pSFA->lpRebootMessage)
					: null;

				this.Comamnd = pSFA->lpCommand != null
					? new string(pSFA->lpCommand)
					: null;

				var actions = new ServiceControlAction[pSFA->actionsCount];

				for (int i = 0; i < actions.Length; i++)
				{
					actions[i] = new ServiceControlAction(
						pSFA->lpActions[i].actionType,
						unchecked((int)pSFA->lpActions[i].delay));
				}

				this.Actions = Array.AsReadOnly(actions);

				return getFunc();
			}
			finally
			{
				Marshal.FreeHGlobal((IntPtr)pSFA);
			}
		}

		private unsafe void SetFailureActions(
			uint resetPeriod = ServiceFailureActions.INFINITE,
			string rebootMessage = null,
			string command = null,
			ReadOnlyCollection<ServiceControlAction> actions = null)
		{
			uint actionsCount = actions != null
				? (uint)actions.Count
				: 0;
			SCAction[] actionsArray = actions != null
				? (from action in actions
				   select new SCAction
				   {
					   actionType = action.Action,
					   delay = (uint)action.Delay,
				   }).ToArray()
				: null;

			fixed (char* lpRebootMessage = rebootMessage)
			fixed (char* lpCommand = command)
			fixed (SCAction* lpActions = actionsArray)
			{
				var sfa = new ServiceFailureActions
				{
					resetPeriod = resetPeriod,
					lpRebootMessage = lpRebootMessage,
					lpCommand = lpCommand,
					actionsCount = actionsCount,
					lpActions = lpActions,
				};

				if (!Win32API.ChangeServiceOptionalConfig(this.service.Handle, Config.FailureActions, &sfa))
				{
					throw new ServiceException(Marshal.GetLastWin32Error());
				}
			}
		}
		#endregion
	}

	public class ServiceControlAction
	{
		#region Propeties

		public ServiceControlActionType Action { get; private set; }
		public int Delay { get; private set; }
		#endregion

		#region Ctor

		public ServiceControlAction(ServiceControlActionType action, int delay)
		{
			this.Action = action;
			this.Delay = delay;
		}
		#endregion
	}
}
