using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Utilities.Concurrency;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace UnitTests.Concurrency
{
	[TestClass]
	public class MessageQueueTests
	{
		#region Consts

		private const int MAX_SLEEP = 100;
		#endregion

		#region Fields

		private SortedList<int, int> list;
		private MessageQueue<int> queue;
		private object syncRoot = new object();
		private int TIMEOUT = 30000;
		private IEnumerable<int> messageSequence = Enumerable.Range(0, 100);
		private Random random = new Random();
		#endregion

		#region Methods

		[TestInitialize]
		public void Init()
		{
			this.list = new SortedList<int, int>();

			this.queue = new MessageQueue<int>(msg =>
				{
					try
					{
						if (!Monitor.TryEnter(this.syncRoot))
						{
							Assert.Fail("Concurrent access to list");
						}
						else
						{
							list.Add(msg, msg);
						}
					}
					finally
					{
						if (Monitor.IsEntered(this.syncRoot))
						{
							Monitor.Exit(this.syncRoot);
						}
					}
				});
		}

		public async Task EnqueNumber(int num)
		{
			await Task.Run(() =>
				{
					int sleepTime;

					lock (this.syncRoot)
					{
						sleepTime = this.random.Next(MAX_SLEEP);
					}

					Thread.Sleep(sleepTime);
					this.queue.EnqueueMessage(num);
				});
		}

		[TestMethod]
		public void TestQueueing()
		{
			Task[] enqueueing = (from num in this.messageSequence
								 select EnqueNumber(num)).ToArray();

			Assert.IsTrue(Task.WaitAll(enqueueing, TIMEOUT), "Failed to enqueue all messages within the timeout period");

			Exception[] faults = (from task in enqueueing
								  where task.IsFaulted
								  select task.Exception).ToArray();
			Assert.IsTrue(faults.Length == 0, "Errors in enqueueing:" + new AggregateException(faults));

			Assert.IsTrue(Enumerable.SequenceEqual(this.list.Keys, messageSequence), "List values mismatch");
		}

		[TestMethod]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void TestDisposing()
		{
			using (this.queue)
			{
				Assert.IsFalse(this.queue.IsDisposed);
			}

			Assert.IsTrue(this.queue.IsDisposed);
			this.queue.EnqueueMessage(0);
		}

		[TestCleanup]
		public void Cleanup()
		{
			this.queue.StopQueue();
		}
		#endregion
	}
}
