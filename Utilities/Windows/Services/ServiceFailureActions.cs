using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Services
{
	public class ServiceFailureActions
	{
		#region Properties

		public int ResetPeriod { get; set; }
		public string RebootMessage { get; set; }
		public string Comamnd { get; set; }
		public IList<ServiceControlAction> Actions { get; set; }
		public bool IsFailOnNonCrash { get; set; }
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
