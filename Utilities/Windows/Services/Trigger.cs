using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Utilities.Windows.Services.Interop;

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

		internal unsafe Trigger(ref ServiceTrigger trigger)
		{
			this.DataItems = new TriggerData[trigger.dataItemsCount];

			for (int i = 0; i < DataItems.Length; i++)
			{
				DataItems[i] = TriggerData.Create(trigger.dataItems[i]);
			}
		}
		#endregion

		#region Methods

		internal unsafe void ToUnmanaged(ref ServiceTrigger unmanaged)
		{
			unmanaged.action = this.Action;
			unmanaged.triggerType = this.Type;
			unmanaged.dataItemsCount = (uint)this.DataItems.Length;

			unmanaged.dataItems = (TriggerSpecificDataItem*)Marshal.AllocHGlobal(
				sizeof(TriggerSpecificDataItem) * this.DataItems.Length);

			for (int i = 0; i < this.DataItems.Length; i++)
			{
				this.DataItems[i].ToUnmanaged(ref unmanaged.dataItems[i]);
			}
			
			unmanaged.triggerSubType = (Guid*)Marshal.AllocHGlobal(sizeof(Guid));
			Marshal.StructureToPtr(this.Subtype, (IntPtr)unmanaged.triggerSubType, false);
		}

		internal static unsafe void FreeUnmanaged(ref ServiceTrigger unmanaged)
		{
			for (int i = 0; i < unmanaged.dataItemsCount; i++)
			{
				TriggerData.FreeUnmanaged(ref unmanaged.dataItems[i]);
			}

			Marshal.FreeHGlobal((IntPtr)unmanaged.dataItems);
			unmanaged.dataItems = null;

			Marshal.FreeHGlobal((IntPtr)unmanaged.triggerSubType);
			unmanaged.triggerSubType = null;
		}
		#endregion
	}
}
