using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Services
{
	internal static class ExceptionCreator
	{
		public static ServiceException Create(IDictionary<int, string> messages, int error)
		{
			ServiceException ex;

			if (messages.ContainsKey(error))
			{
				ex = new ServiceException(error, messages[error]);
			}
			else
			{
				ex = new ServiceException(error);
			}

			return ex;
		}
	}
}
