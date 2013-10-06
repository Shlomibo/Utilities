using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Services
{
	/// <summary>
	/// Represent object that can wait for notifications to accure
	/// </summary>
	public interface INotificationWaiter
	{
		/// <summary>
		/// Blocks the thread until wanted notification is raised, or until the timeout elapses.
		/// </summary>
		/// <param name="waitFor">
		/// Flags of the notification to wait for. 
		/// If one of the notifications raised - the block ends.
		/// </param>
		/// <param name="millisecondsTimeout">
		/// The timeout, in milliseconds, until the block ends, even if no notification raised.
		/// </param>
		/// <param name="triggered">Return the notification that was actually raised.</param>
		/// <returns>
		/// true if one of notification was raised before the timeout elapsed;
		/// otherwise false.
		/// </returns>
		bool WaitForNotification(Notification waitFor, int millisecondsTimeout, out Notification triggered);

		/// <summary>
		/// Blocks the thread until wanted notification is raised, or until the timeout elapses.
		/// </summary>
		/// <param name="waitFor">
		/// Flags of the notification to wait for. 
		/// If one of the notifications raised - the block ends.
		/// </param>
		/// <param name="timeout">
		/// The timeout, until the block ends, even if no notification raised.
		/// </param>
		/// <param name="triggered">Return the notification that was actually raised.</param>
		/// <returns>
		/// true if one of notification was raised before the timeout elapsed;
		/// otherwise false.
		/// </returns>
		bool WaitForNotification(Notification waitFor, TimeSpan timeout, out Notification triggered);

		/// <summary>
		/// Blocks the thread until wanted notification is raised.
		/// </summary>
		/// <param name="waitFor">
		/// Flags of the notification to wait for. 
		/// If one of the notifications raised - the block ends.
		/// </param>
		/// <returns>The notification that was actually raised.</returns>
		Notification WaitForNotification(Notification waitFor);
	}
}
