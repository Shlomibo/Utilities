using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Windows.Services;
using System.Windows.Services.XPCompatibility;
using Utilities;

namespace CTests
{
	class Program
	{
		static void Main(string[] args)
		{
			Test();

			Console.ReadLine();


			//using (var scm = new ServiceControlManager(ScmAccessRights.AllAccess))
			//{
			//	using (Service svc = scm.OpenService("TestSvc", ServiceAccessRights.AllAccess))
			//	{
			//		svc.Delete();
			//	}
			//}
		}

		private static void Test()
		{
			try
			{
				using (var scm = new ServiceControlManager())
				{
					using (var svc = scm.OpenService("TestSvc", ServiceAccessRights.AllAccess))
					{
						svc.Start("I", "testing", "command", "args");
					}
					//using (var service = scm.OpenService("TestSvc"))
					//{
					//	if (service.WaitForStates(5000, State.Running))
					//	{
					//		Console.WriteLine("svc -> Running");
					//	}
					//	else
					//	{
					//		Console.WriteLine("svc -> not running yet");
					//	}

					//	service.WaitForStates(State.Running);
					//	Console.WriteLine("svc -> Running");

					//	service.WaitForStates(State.Paused);
					//	Console.WriteLine("svc -> Paused");

					//	service.WaitForStates(State.Running);
					//	Console.WriteLine("svc -> Running"); 

					//	service.WaitForStates(State.Stopped);
					//	Console.WriteLine("svc -> Stopped");
					//}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error! \r\n" + ex);
			}

		}

		//private static string SafePrint(Func<object> printFunc)
		//{
		//	string str;

		//	try
		//	{
		//		str = printFunc().ToString();
		//	}
		//	catch (Exception ex)
		//	{
		//		str = "EXCEPTION! - " + ex.Message;
		//	}

		//	return str;
		//}

		//private static string PrintSvcStatus(object obj)
		//{
		//	var str = new StringBuilder();

		//	if (obj is ServiceStatus)
		//	{
		//		var serviceStatus = (ServiceStatus)obj;

		//		str.AppendLine("Status: ");
		//		str.AppendLine("\t\tAccepted controls: " + serviceStatus.AcceptedControls);
		//		str.AppendLine("\t\tCheck point: " + serviceStatus.CheckPoint);
		//		str.AppendLine("\t\tProcess id: " + serviceStatus.ProcessId);
		//		str.AppendLine("\t\tProcess flags: " + serviceStatus.ServiceProcFlags);
		//		str.AppendLine("\t\tSpecific exit code: " + (serviceStatus.ServiceSpecificExitCode ?? -1));
		//		str.AppendLine("\t\tState: " + serviceStatus.State);
		//		str.AppendLine("\t\tType: " + serviceStatus.Type);
		//		str.AppendLine("\t\tWait hint: " + serviceStatus.WaitHint);
		//		str.AppendLine("\t\tWin32 exit code: " + serviceStatus.Win32ExitCode);
		//	}

		//	return str.ToString();
		//}

		//private static string PrintTrigger(object arg)
		//{
		//	var str = new StringBuilder();

		//	if (arg is Trigger)
		//	{
		//		str.AppendLine("Trigger:");
		//		var trigger = (Trigger)arg;

		//		str.AppendLine("\t\tType: " + trigger.Type);
		//		str.AppendLine("\t\tAction: " + trigger.Action);
		//		str.AppendLine("\t\tSub type: " + trigger.Subtype);
		//		str.AppendLine("\t\tData items count: " + trigger.DataItems.Count);
		//	}

		//	return str.ToString();
		//}

		//private static void PrintCollection(IEnumerable collection)
		//{
		//	PrintCollection(collection, obj => obj.ToString());
		//}

		//private static void PrintCollection(IEnumerable collection, Func<object, string> toStringFunc)
		//{
		//	foreach (object item in collection)
		//	{
		//		Console.WriteLine("\t" + toStringFunc(item));
		//	}
		//}
	}
}
