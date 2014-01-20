using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Concurrency
{
	/// <summary>
	/// Message handler for messages enqueued in MessageQueue&lt;T&gt;
	/// </summary>
	/// <typeparam name="T">The type of the message</typeparam>
	/// <param name="message">The messaged that was queued</param>
	public delegate void MessageHandler<T>(T message);

	/// <summary>
	/// Concurrent message queue, which recieve messages asynchronously, and dispatch them synchronously
	/// </summary>
	/// <typeparam name="T">The type of the message</typeparam>
	public class MessageQueue<T> : IDisposable
	{
		#region Consts

		private const int SEM_TIMEOUT = 50;
		#endregion

		#region Fields

		private readonly MessageHandler<T> handler;
		private readonly ConcurrentQueue<T> messagesQueue = new ConcurrentQueue<T>();
		private readonly Semaphore queueSemaphore = new Semaphore(0, int.MaxValue);
		private readonly Task handlingTask;
		private readonly CancellationTokenSource canellation;
		private bool isDisposing;
		private object syncRoot = new object();
		#endregion

		#region Properties

		/// <summary>
		/// Get value which indicates if the queue is disposed
		/// </summary>
		public bool IsDisposed { get; private set; }

		private bool IsDisposing
		{
			get { lock (this.syncRoot) { return this.isDisposing; } }
			set { lock (this.syncRoot) { this.isDisposing = value; } }
		}
		#endregion

		#region Ctor

		/// <summary>
		/// Creates new queue that would dispatch messages to the given handler
		/// </summary>
		/// <param name="handler">The handler for message dispatch</param>
		public MessageQueue(MessageHandler<T> handler) : this(handler, TaskScheduler.Default) { }

		/// <summary>
		/// Creates new queue that would dispatch messages to the given handler
		/// </summary>
		/// <param name="handler">The handler for message dispatch</param>
		/// <param name="scheduler">Task scheduler for message dispatch</param>
		public MessageQueue(MessageHandler<T> handler, TaskScheduler scheduler)
		{
			this.handler = handler;
			this.canellation = new CancellationTokenSource();
			this.handlingTask = QueueHandler(canellation.Token, scheduler);
		}

		/// <summary>
		/// Finallizes the object
		/// </summary>
		~MessageQueue()
		{
			Dispose(false);
		}
		#endregion

		#region Methods

		/// <summary>
		/// Enqueues message
		/// </summary>
		/// <param name="message">The message to enqueue</param>
		public void EnqueueMessage(T message)
		{
			ThrowIfDisposed();

			this.messagesQueue.Enqueue(message);
			this.queueSemaphore.Release();
		}

		private void ThrowIfDisposed()
		{
			if (this.IsDisposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		private Task QueueHandler(CancellationToken cancel, TaskScheduler scheduler)
		{
			return Task.Factory.StartNew(() =>
				{
					bool didReleased;

					while ((didReleased = queueSemaphore.WaitOne(SEM_TIMEOUT)) || !cancel.IsCancellationRequested)
					{
						if (didReleased)
						{
							T message;

							while (this.messagesQueue.TryDequeue(out message))
							{
								this.handler(message);
							}
						}
					}

					if (this.IsDisposing)
					{
						this.queueSemaphore.Dispose();
						this.canellation.Dispose();
					}
				}, cancel, TaskCreationOptions.LongRunning, scheduler);
		}

		void IDisposable.Dispose()
		{
			StopQueue();
		}

		/// <summary>
		/// Stops dispatching, and release resources
		/// </summary>
		public void StopQueue()
		{
			if (!this.IsDisposed)
			{
				Dispose(true);
				GC.SuppressFinalize(this); 
			}
		}

		/// <summary>
		/// Releases unamanaged resources.
		/// </summary>
		/// <param name="disposing">true if managed resources sould be released too; otherwise false</param>
		protected virtual void Dispose(bool disposing)
		{
			this.IsDisposing = disposing;
			this.canellation.Cancel();
			this.IsDisposed = true;
		}
		#endregion
	}
}
