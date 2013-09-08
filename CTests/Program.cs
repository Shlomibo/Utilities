using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Security;

namespace CTests
{
	class Program
	{
		static unsafe void Main(string[] args)
		{
			var arr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 };
			var sec = new SecuredArray<int>(arr);

			Console.WriteLine(string.Join(", ", (from item in sec
												 select item.ToString()).ToArray()));

			sec.Dispose();
		}
	}
}
