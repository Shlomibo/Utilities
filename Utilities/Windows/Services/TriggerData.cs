using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Services
{
	public abstract class TriggerData
	{
		#region Properties

		public DataItemType DataType { get; protected set; }
		public abstract object Data { get; } 
		#endregion

		#region Methods

		public static TriggerData Create(object data)
		{
			if (data is string)
			{
				return new TriggerStringData((string)data);
			}
			else if (data is byte[])
			{
				return new TriggerBinaryData((byte[])data);
			}
			else
			{
				throw new ArgumentException("Invalid data type for data", "data");
			}
		}
		#endregion
	}

	public class TriggerStringData : TriggerData
	{
		#region Properties

		public string StringData { get; private set; }

		public override object Data
		{
			get { return this.StringData; }
		} 
		#endregion

		#region Ctor

		public TriggerStringData(string data)
		{
			this.DataType = DataItemType.String;
			this.StringData = data;
		} 
		#endregion
	}

	public class TriggerBinaryData : TriggerData
	{
		#region Properties

		public byte[] BinaryData { get; private set; }
		public override object Data
		{
			get { return this.BinaryData; }
		}
		#endregion

		#region Ctor

		public TriggerBinaryData(byte[] data)
		{
			this.DataType = DataItemType.Binary;
			this.BinaryData = data;
		}
		#endregion
	}
}
