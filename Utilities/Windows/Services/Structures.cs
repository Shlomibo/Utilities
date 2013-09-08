using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Services
{
	public struct PowerSetting
	{
		public Guid Setting { get; set; }
		public int Data { get; set; }
	}

	public struct TimeChange
	{
		public DateTime NewTime { get; set; }
		public DateTime OldTime { get; set; }
	}
}
