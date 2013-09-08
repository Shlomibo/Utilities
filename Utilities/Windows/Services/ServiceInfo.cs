using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	public class ServiceInfo
	{
		#region Properties

		public string ServiceName { get; private set; }
		public string DisplayName { get; private set; }
		public ServiceType Type { get; private set; }
		public State State { get; private set; }
		public AcceptedControls AcceptedControls { get; private set; }
		public int ProcessId { get; private set; }
		public ProcessFlags ServiceProcFlags { get; private set; }
		internal ServiceControl Scm { get; private set; }
		#endregion

		#region Ctor

		internal unsafe ServiceInfo(ServiceControl scm, EnumServiceStatusProcess essp)
		{
			this.Scm = scm;
			this.ServiceName = new string(essp.lpServiceName);
			this.DisplayName = new string(essp.lpDisplayName);
			this.Type = essp.processStatus.type;
			this.State = essp.processStatus.state;
			this.AcceptedControls = essp.processStatus.acceptedControls;
			this.ProcessId = (int)essp.processStatus.processId;
			this.ServiceProcFlags = essp.processStatus.flags;
		}
		#endregion

		#region Methods

		public Process GetProcess()
		{
			return Process.GetProcessById(this.ProcessId);
		}

		public Service GetService()
		{
			return Scm.OpenService(this.ServiceName);
		}
		#endregion
	}
}
