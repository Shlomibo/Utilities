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
	partial class ServiceControlManager
	{
		/// <summary>
		/// A collection of statuses of the services in a service control manager database.
		/// </summary>
		public class ServiceCollection : ServiceCollectionBase
		{
			#region Fields

			private ServiceControlManager scm;
			#endregion

			#region Ctor

			internal ServiceCollection(ServiceControlManager scm)
			{
				this.scm = scm;
			}
			#endregion

			#region Methods

			/// <summary>
			/// Throws an exception if the collection is disposed.
			/// </summary>
			protected override void ThrowIfDisposed()
			{
				this.scm.ThrowIfDisposed();
			}

			/// <summary>
			/// Queries for services of specific types, states, or under specific groups
			/// </summary>
			/// <param name="type">The type of services to be enumerated.</param>
			/// <param name="state">The state of the services to be enumerated.</param>
			/// <param name="groupName">
			/// The load-order group name. 
			/// The only services enumerated are those that belong to the group that has the name specified by the string. 
			/// If this parameter is an empty string, only services that do not belong to any group are enumerated. 
			/// If this parameter is NULL, group membership is ignored and all services are enumerated.
			/// </param>
			/// <returns>An enumerable that enumerates the corresponding services' status</returns>
			public IEnumerable<ServiceInfo> QueryServices(
				ServiceType type,
				StateQuery state,
				string groupName)
			{
				return QueryServicesInumerator(type, state, groupName);
			}

			/// <summary>
			/// Queries for services of specific types, states, or under specific groups
			/// </summary>
			/// <param name="type">The type of services to be enumerated.</param>
			/// <param name="state">The state of the services to be enumerated.</param>
			/// <param name="groupName">
			/// The load-order group name. 
			/// The only services enumerated are those that belong to the group that has the name specified by the string. 
			/// If this parameter is an empty string, only services that do not belong to any group are enumerated. 
			/// If this parameter is NULL, group membership is ignored and all services are enumerated.
			/// </param>
			/// <returns>An enumerable that enumerates the corresponding services' status</returns>
			protected override unsafe IEnumerable<ServiceInfo> QueryServicesInumerator(
				ServiceType type,
				StateQuery state,
				string groupName)
			{
				ThrowIfDisposed();
				return new Enumerator(this, type, state, groupName);
			}
			#endregion

			#region Enumerator
			
			private new unsafe class Enumerator : ServiceCollectionBase.Enumerator
			{
				#region Consts

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


				private uint resumeHandle = 0;
				private ServiceCollection collection;
				private ServiceType type;
				private string groupName;
				private EnumServiceStatusProcess* pESSP = null;
				#endregion

				#region Properties

				public override ServiceInfo Current
				{
					get
					{
						ThrowIfDisposed();
						return new ServiceInfo(this.collection.scm, this.pESSP[this.index]);
					}
				}
				#endregion

				#region Ctor

				public Enumerator(
					ServiceCollection collection,
					ServiceType type,
					StateQuery state,
					string groupName)
					: base()
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
				}
				#endregion

				#region Methods

				protected override void ThrowIfDisposed()
				{
					this.collection.scm.ThrowIfDisposed();
				}

				protected override void Dispose(bool disposing)
				{
					Marshal.FreeHGlobal((IntPtr)pESSP);
					this.pESSP = null;
				}
				#endregion
			}
			#endregion
		}
	}
}
