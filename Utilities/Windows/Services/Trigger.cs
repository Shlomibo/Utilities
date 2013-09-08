using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Services
{
	public class Trigger
	{
		#region Properties

		public TriggerType Type { get; private set; }
		public TriggerAction Action { get; private set; }
		public Guid Subtype { get; private set; }
		public TriggerData[] DataItems { get; set; }
		#endregion

		#region Ctor

		public Trigger(TriggerType type, TriggerAction action, Guid subType, IList<TriggerData> dataItems)
		{
			this.Type = type;
			this.Action = action;
			this.Subtype = subType;
			this.DataItems = dataItems != null
				? (dataItems as TriggerData[]) ?? dataItems.ToArray()
				: null;
		}
		#endregion
	}
}
