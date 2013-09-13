using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Services
{
	/// <summary>
	/// Throws an exception for a Win32 service error code.
	/// </summary>
	[Serializable]
	public class ServiceException : Win32Exception
	{
		/// <summary>
		/// Initializes a new instance of the ServiceException class with the last Win32 error that occurred.
		/// </summary>
		public ServiceException() : base() { }

		/// <summary>
		/// Initializes a new instance of the ServiceException class with the specified error.
		/// </summary>
		/// <param name="error">The Win32 error code associated with this exception.</param>
		public ServiceException(int error) : base(error) { }

		/// <summary>
		/// Initializes a new instance of the ServiceException class with 
		/// the specified error and the specified detailed description.
		/// </summary>
		/// <param name="error">The Win32 error code associated with this exception.</param>
		/// <param name="message">A detailed description of the error.</param>
		public ServiceException(int error, string message) : base(error, message) { }

		/// <summary>
		/// Initializes a new instance of the ServiceException class with the specified detailed description.
		/// </summary>
		/// <param name="message">A detailed description of the error.</param>
		public ServiceException(string message) : base(message) { }

		/// <summary>
		/// Initializes a new instance of the ServiceException class with
		/// the specified detailed description and the specified exception.
		/// </summary>
		/// <param name="message">A detailed description of the error.</param>
		/// <param name="inner">A reference to the inner exception that is the cause of this exception.</param>
		public ServiceException(string message, Exception inner) : base(message, inner) { }

		/// <summary>
		/// Initializes a new instance of the ServiceException class with
		/// the specified context and the serialization information.
		/// </summary>
		/// <param name="info">The SerializationInfo associated with this exception.</param>
		/// <param name="context">A StreamingContext that represents the context of this exception.</param>
		protected ServiceException(
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}
