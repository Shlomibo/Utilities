namespace Utilities
{
	/// <summary>
	/// Base class for the generic TimedResut
	/// </summary>
	public abstract class TimedResult
	{
		#region Properties

		/// <summary>
		/// Gets true if the timeout was elapsed; otherwise false.
		/// </summary>
		public bool DidTimedOut { get; }
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new instance.
		/// </summary>
		/// <param name="didTimedOut">true if the timeout was elapsed; otherwise false.</param>
		protected TimedResult(bool didTimedOut)
		{
			this.DidTimedOut = didTimedOut;
		}
		#endregion
	}

	/// <summary>
	/// Represent a result for a task that can timeout
	/// </summary>
	/// <typeparam name="T">The type of the result.</typeparam>
	public class TimedResult<T> : TimedResult
	{
		#region Properties

		/// <summary>
		/// Gets the result of the task.
		/// </summary>
		public T Result { get; }
		#endregion

		#region Ctor

		private TimedResult(T result, bool didTimedOut)
			: base(didTimedOut)
		{
			this.Result = result;
		}

		/// <summary>
		/// Creates new instance of the class for task that was finished before the timeout.
		/// </summary>
		/// <param name="result">The result of the task.</param>
		public TimedResult(T result) : this(result, false) { }

		/// <summary>
		/// Creates new instance of the class for task that was timed out before the before it was finished.
		/// </summary>
		public TimedResult() : this(default(T), true) { }
		#endregion
	}
}