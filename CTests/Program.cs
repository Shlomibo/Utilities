using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using Utilities;
using CTests.Properties;

namespace CTests
{
	class Program
	{
		static void Main(string[] args)
		{
			args = new string[]
			{
				"not", 
				"keyed", 
				"values",
				"-dup key",
				"first dup",
				"second dup",
				"-first key",
				"-second key",
				"first value", 
				"second value",
				"-third key",
				"value",
				"-dup key",
				"third dup",
				"forth dup",
			};
			
			var managedArgs = new CLArguments(args);

			foreach (string key in managedArgs.Keys)
			{
				string printed = key == CLArguments.NO_KEY
					? "Unkeyed"
					: key;
				Console.WriteLine(printed + ": ");

				Console.WriteLine(string.Join("; ", managedArgs[key]));
				Console.WriteLine();
			}

			Console.ReadLine();


			
		}
	}
}
