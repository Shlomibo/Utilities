using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Windows.Interop;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	internal unsafe class ServiceEvents : IDisposable
	{
		#region Consts

		private static readonly Dictionary<int, string> MSGS_NTFY_STTS_CHNG = new Dictionary<int, string>()
		{
			{ Win32API.ERROR_SERVICE_MARKED_FOR_DELETE, "The handle to the service must be closed." },
			{ Win32API.ERROR_SERVICE_NOTIFY_CLIENT_LAGGING, 
				"Close the handle to the service control manager, and open a new handle." },
		};
		#endregion
		
		#region Fields

		private ManualResetEvent waitHandle;
		private ServiceNotify* pSN = null;
		private bool isEventRegistered = false;
		private TaskScheduler eventScheduler;
		private IntPtr serviceHandle;
		private Notification registerFor;
		private object syncRoot = new object();
		private EventHandler<ServiceNotificationEventArgs> eventHandler = (s, e) => { };
		private bool isDisposed = false;
		private CallbackDelegate callback;
		private bool isService;
		private Notification lastEvent;
		#endregion

		#region Events

		public event EventHandler<ServiceNotificationEventArgs> ServiceNotification
		{
			add
			{
				if (!isEventRegistered)
				{
					RegisterEventsAsync();
				}

				this.eventHandler += value;
			}
			remove
			{
				this.eventHandler -= value;
			}
		}
		#endregion

		#region Ctor

		public ServiceEvents(IntPtr handle, Notification registerFor, bool isService)
		{
			this.callback = new CallbackDelegate(EventCallback);
			this.serviceHandle = handle;
			this.registerFor = registerFor;
			this.eventScheduler = TaskScheduler.FromCurrentSynchronizationContext();
			this.waitHandle = new ManualResetEvent(false);
			this.isService = isService;
			this.lastEvent = Notification.None;
		}

		~ServiceEvents()
		{
			Dispose(false);
		}
		#endregion

		#region Methods

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				this.waitHandle.Set();

				if (disposing)
				{
					this.waitHandle.Dispose();
				}

				lock (this.syncRoot)
				{
					Marshal.FreeHGlobal((IntPtr)this.pSN);
				}

				this.isDisposed = true;
			}
		}

		private unsafe Task RegisterEventsAsync()
		{
			this.isEventRegistered = true;

			return Task.Run(() =>
			{
				lock (this.syncRoot)
				{
					this.pSN = (ServiceNotify*)Marshal.AllocHGlobal(sizeof(ServiceNotify)); 
				}

				Notification registerFor = this.registerFor;

				if (this.isService)
				{
					registerFor &= ~this.lastEvent;
				}

				(*this.pSN) = new ServiceNotify
				{
					version = ServiceNotify.SERVICE_NOTIFY_STATUS_CHANGE,
					notifyCallback = Marshal.GetFunctionPointerForDelegate(this.callback),
					context = IntPtr.Zero,
				};

				int result = (int)Win32API.NotifyServiceStatusChange(
					this.serviceHandle,
					registerFor,
					this.pSN);

				if (result != Win32API.ERROR_SUCCESS)
				{
					lock (this.syncRoot)
					{
						Marshal.FreeHGlobal((IntPtr)this.pSN);
						this.pSN = null;
						throw ServiceException.Create(MSGS_NTFY_STTS_CHNG, result); 
					}
				}

				this.waitHandle.WaitOne();
			});
		}

		private unsafe void EventCallback(ServiceNotify* pSN)
		{
			try
			{
				this.lastEvent = pSN->notificationTriggered;
				Task.Factory.StartNew(
					param => FireEvent((EventData)param),
					new EventData
					{
						ServiceNames = new MultiString(pSN->serviceName),
						Event = pSN->notificationTriggered,
						Status = new ServiceStatus(pSN->serviceStatus),
					},
					CancellationToken.None,
					TaskCreationOptions.None,
					this.eventScheduler);
			}
			finally 
			{
				lock (this.syncRoot)
				{
					Marshal.FreeHGlobal((IntPtr)this.pSN->serviceName);
					Marshal.FreeHGlobal((IntPtr)this.pSN);
					this.pSN = null; 
				}
			}

			RegisterEventsAsync();
		}

		private void FireEvent(EventData eventData)
		{
			this.eventHandler(
				this,
				new ServiceNotificationEventArgs(
					eventData.ServiceNames,
					eventData.Status,
					eventData.Event));
		}
		#endregion

		#region Event data class

		private class EventData
		{
			public MultiString ServiceNames { get; set; }
			public ServiceStatus Status { get; set; }
			public Notification Event { get; set; }
		}
		#endregion
	}

	internal class ServiceNotificationEventArgs : EventArgs
	{
		#region Properties

		public MultiString ServiceNames { get; private set; }
		public ServiceStatus Status { get; private set; }
		public Notification Event { get; private set; } 
		#endregion

		#region Ctor

		public ServiceNotificationEventArgs(MultiString serviceNames, ServiceStatus status, Notification @event)
		{
			this.ServiceNames = serviceNames;
			this.Status = status;
			this.Event = @event;
		}
		#endregion
	}
}
