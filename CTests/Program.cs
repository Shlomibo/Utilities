using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Security;
using Utilities.Windows.Services;

namespace CTests
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var scm = new ServiceControlManager())
			{
				using (var service = scm.OpenService("TestSvc"))
				{
					service.WaitForNotification(Notification.Running);
					Console.WriteLine("svc -> running");

					service.WaitForNotification(Notification.Paused);
					Console.WriteLine("svc -> paused");

					service.WaitForNotification(Notification.Running);
					Console.WriteLine("svc -> running");

					service.WaitForNotification(Notification.Stopped);
					Console.WriteLine("svc -> stopped");
				}
			}

			Console.ReadLine();


			//using (var scm = new ServiceControlManager(ScmAccessRights.AllAccess))
			//{
			//	using (Service svc = scm.OpenService("TestSvc", ServiceAccessRights.AllAccess))
			//	{
			//		svc.Delete();
			//	}
			//}
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
