using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Windows.Interop
{
	internal static class Windows
	{
		[DllImport("user32.dll")]
		public static extern uint RegisterWindowMessage(string message);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SendMessage(IntPtr wHnd, uint message, UIntPtr wParam, IntPtr lParam);
	}
}
