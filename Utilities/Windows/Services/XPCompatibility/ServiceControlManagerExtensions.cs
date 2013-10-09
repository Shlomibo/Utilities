using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Services.XPCompatibility
{
	public static class ServiceControlManagerExtensions
	{
		public static ServiceControlLock Lock(this ServiceControlManager scm)
		{
			return new ServiceControlLock(scm);
		}

		public static ServiceLockStatus GetLockStatus(this ServiceControlManager scm)
		{
			return ServiceControlLock.QueryLockStatus(scm);
		}
	}
}
