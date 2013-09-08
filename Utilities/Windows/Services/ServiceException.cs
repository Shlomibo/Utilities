using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Services
{
	[Serializable]
	public class ServiceException : Win32Exception
	{
		public ServiceException() { }
		public ServiceException(int error) : base(error) { }
		public ServiceException(int error, string message) : base(error, message) { }
		public ServiceException(string message) : base(message) { }
		public ServiceException(string message, Exception inner) : base(message, inner) { }
		protected ServiceException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
