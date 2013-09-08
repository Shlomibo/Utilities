using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Windows.Services.Interop;

namespace Utilities.Windows.Services
{
	public class ServiceLockStatus
	{
		#region Properties

		public string Owner { get; private set; }
		public int LockDuration { get; private set; }
		#endregion

		#region Ctor

		internal unsafe ServiceLockStatus(QueryServiceLockStatus lockStatus)
		{
			this.Owner = new string(lockStatus.lpLockOwner);
			this.LockDuration = (int)lockStatus.lockDuration;
		}
		#endregion
	}
}
