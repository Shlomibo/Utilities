using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
	[Serializable]
	public class OperationPerformedException : Exception
	{
		public OperationPerformedException() { }
		public OperationPerformedException(string message) : base(message) { }
		public OperationPerformedException(string message, Exception inner) : base(message, inner) { }
		protected OperationPerformedException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
