using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	partial class ServiceControl
	{
		public class ServiceCollection : ICollection<ServiceInfo>
		{
			#region Consts

			private const int NO_ERROR = 0;

			public const string ALL_GROUPS = Win32API.ALL_GROUPS;
			public const string NOT_IN_GROUP = Win32API.NOT_GROUPED;
			#endregion

			#region Fields

			private ServiceControl scm;
			#endregion

			#region Properties

			public int Count
			{
				get { return this.Count(); }
			}

			bool ICollection<ServiceInfo>.IsReadOnly
			{
				get { return true; }
			}
			#endregion

			#region Ctor

			internal ServiceCollection(ServiceControl scm)
			{
				this.scm = scm;
			}
			#endregion

			#region Methods

			void ICollection<ServiceInfo>.Add(ServiceInfo item)
			{
				throw new NotSupportedException();
			}

			void ICollection<ServiceInfo>.Clear()
			{
				throw new NotSupportedException();
			}

			public bool Contains(ServiceInfo service)
			{
				return service.Scm.Handle == scm.Handle;
			}

			public void CopyTo(ServiceInfo[] array, int arrayIndex)
			{
				foreach (ServiceInfo service in this)
				{
					array[arrayIndex++] = service;
				}
			}

			bool ICollection<ServiceInfo>.Remove(ServiceInfo item)
			{
				throw new NotSupportedException();
			}

			public IEnumerator<ServiceInfo> GetEnumerator()
			{
				return QueryServicesInumerator(ServiceType.All, StateQuery.All, ALL_GROUPS).GetEnumerator();
			}

			public IEnumerable<ServiceInfo> QueryServices(
				ServiceType type,
				StateQuery state,
				string groupName)
			{
				return QueryServicesInumerator(type, state, groupName);
			}

			private unsafe IEnumerable<ServiceInfo> QueryServicesInumerator(
				ServiceType type,
				StateQuery state,
				string groupName)
			{
				return new Enumerator(this, type, state, groupName);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
			#endregion

			#region Enumerator

			private unsafe class Enumerator : IEnumerator<ServiceInfo>, IEnumerable<ServiceInfo>
			{
				#region Consts

				private const int RESETED = -1;

				private static readonly Dictionary<int, string> MSGS_ENUM_ERRORS = new Dictionary<int, string>()
				{
					{ Win32API.ERROR_ACCESS_DENIED, "The handle does not have the SC_MANAGER_ENUMERATE_SERVICE access right." },
					{ Win32API.ERROR_INVALID_PARAMETER, "An illegal parameter value was used." },
					{ Win32API.ERROR_INVALID_HANDLE, "The handle is invalid." },
					{ Win32API.ERROR_INVALID_LEVEL, "The InfoLevel parameter contains an unsupported value." },
					{ Win32API.ERROR_SHUTDOWN_IN_PROGRESS, "The system is shutting down; this function cannot be called." },
				};
				#endregion

				#region Fields

				private ServiceType type;
				private StateQuery state;
				private string groupName;
				private int lastError = NO_ERROR;
				private EnumServiceStatusProcess* pESSP = null;
				private uint allocated = 0;
				private uint needed = 0;
				private uint returned = 0;
				private uint resumeHandle = 0;
				private ServiceCollection collection;
				private int index;
				#endregion

				#region Properties

				public ServiceInfo Current
				{
					get { return new ServiceInfo(this.collection.scm, this.pESSP[this.index]); }
				}

				object IEnumerator.Current
				{
					get { return this.Current; }
				}
				#endregion

				#region Ctor

				public Enumerator(
					ServiceCollection collection,
					ServiceType type,
					StateQuery state,
					string groupName)
				{
					this.collection = collection;
					this.type = type;
					this.state = state;
					this.groupName = groupName;

					do
					{
						if (this.needed != 0)
						{
							if (this.pESSP == null)
							{
								this.pESSP = (EnumServiceStatusProcess*)Marshal.AllocHGlobal((int)this.needed);
							}
							else
							{
								this.pESSP = (EnumServiceStatusProcess*)Marshal.ReAllocHGlobal(
									(IntPtr)this.pESSP,
									(IntPtr)this.needed);
							}

							this.allocated = this.needed;
						}

						if (Win32API.EnumServicesStatus(
							this.collection.scm.Handle,
							Win32API.SC_ENUM_PROCESS_INFO,
							this.type,
							this.state,
							this.pESSP,
							this.allocated,
							out this.needed,
							out this.returned,
							ref this.resumeHandle,
							this.groupName))
						{
							this.lastError = NO_ERROR;
						}
						else
						{
							this.lastError = Marshal.GetLastWin32Error();
						}
					} while (this.lastError == Win32API.ERROR_MORE_DATA);

					if (lastError != NO_ERROR)
					{
						throw ExceptionCreator.Create(MSGS_ENUM_ERRORS, lastError);
					}

					Reset();
				}

				~Enumerator()
				{
					Dispose(false);
				}
				#endregion

				#region Methods

				public void Dispose()
				{
					Dispose(true);
					GC.SuppressFinalize(this);
				}

				protected virtual void Dispose(bool disposing)
				{
					Marshal.FreeHGlobal((IntPtr)pESSP);
					this.pESSP = null;
				}

				public bool MoveNext()
				{
					this.index++;

					return this.index < this.returned;
				}

				public void Reset()
				{
					this.index = RESETED;
				}

				public IEnumerator<ServiceInfo> GetEnumerator()
				{
					return this;
				}

				IEnumerator IEnumerable.GetEnumerator()
				{
					return GetEnumerator();
				}
				#endregion
			}
			#endregion
		}
	}
}
