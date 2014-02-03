using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Extansions.Object;
using Utilities.Extansions.Enumerable;

namespace Utilities.Concurrency
{
	public static class TaskOperators
	{
		public static IEnumerable<Task<T>> OrderByCompletion<T>(this IEnumerable<Task<T>> tasks)
		{
			tasks.ThrowWhen(
				when: sequence => sequence == null,
				what: () => new ArgumentNullException("tasks"));

			int index = -1;
			TaskCompletionSource<T>[] taskSources = tasks.Select(task => new TaskCompletionSource<T>())
														 .ToArray();

			foreach (Task<T> task in tasks)
			{
				task.ContinueWith(completedTask => SetCompleted(completedTask, taskSources, Interlocked.Increment(ref index)));
			}

			return taskSources.Select(source => source.Task);
		}

		private static void SetCompleted<T>(Task<T> completedTask, TaskCompletionSource<T>[] taskSources, int index)
		{
			TaskCompletionSource<T> source = taskSources[index];

			if (completedTask.IsCanceled)
			{
				source.TrySetCanceled();
			}
			else if (completedTask.IsFaulted)
			{
				source.TrySetException(completedTask.Exception);
			}
			else
			{
				source.TrySetResult(completedTask.Result);
			}
		}

		public static Task<T> RetryOnFault<T>(Func<Task<T>> operation, int maxTries)
		{
			ValidateRetryOnFault(operation, maxTries);

			Func<Task<T>> retries = async () =>
				{
					return await RetryOnFault(operation, maxTries, () => Task.FromResult((object)null));
				};

			return retries();
		}

		private static void ValidateRetryOnFault(Func<Task> operation, int maxTries)
		{
			operation.ThrowWhen(
				when: func => func == null,
				what: () => new ArgumentNullException("operation"));
			maxTries.ThrowWhen(
				when: tries => tries <= 0,
				what: () => new ArgumentOutOfRangeException("maxTries"));
		}

		public static Task<T> RetryOnFault<T>(Func<Task<T>> operation, int maxTries, Func<Task> continueWhen)
		{
			ValidateRetryOnFault(operation, maxTries, continueWhen);

			Func<Task<T>> retries = async () =>
				{
					Task<T> task = null;

					await RetryOnFault(() => (Task)(task = operation()), maxTries, continueWhen);
					return task.Result;
				};

			return retries();
		}

		private static void ValidateRetryOnFault(Func<Task> operation, int maxTries, Func<Task> continueWhen)
		{
			ValidateRetryOnFault(operation, maxTries);

			continueWhen.ThrowWhen(
				when: func => func == null,
				what: () => new ArgumentNullException("continueWhen"));
		}

		public static Task RetryOnFault(Func<Task> operation, int maxTries)
		{
			ValidateRetryOnFault(operation, maxTries);

			Func<Task> retries = async () =>
				{
					await RetryOnFault(operation, maxTries, () => Task.FromResult((object)null));
				};

			return retries();
		}

		public static Task RetryOnFault(Func<Task> operation, int maxTries, Func<Task> continueWhen)
		{
			ValidateRetryOnFault(operation, maxTries, continueWhen);

			Func<Task> retryTask = async () =>
				{
					var exceptions = new Exception[maxTries];
					int tries = 0;
					Task lastTask = null;

					while (true)
					{
						try
						{
							await (lastTask = operation()).ConfigureAwait(continueOnCapturedContext: false);
							return;
						}
						catch (Exception ex)
						{
							if (lastTask.IsCanceled)
							{
								throw;
							}
							else
							{
								exceptions[tries++] = ex;

								if (tries == maxTries)
								{
									throw new AggregateException(exceptions);
								}
							}
						}

						await continueWhen().ConfigureAwait(continueOnCapturedContext: false);
					}
				};

			return retryTask();
		}

		public static Task<T> FirstOf<T>(params Func<CancellationToken, Task<T>>[] operations)
		{
			operations.ThrowWhen(
				when: EnumerableExtansions.IsNullOrEmpty,
				what: () =>
					operations == null
						? new ArgumentNullException("operations")
						: new ArgumentException("operations cannot be empty."));

			Func<Task<T>> getFirst = async () =>
				{
					var cts = new CancellationTokenSource();
					Task<T>[] tasks = operations.Select(operation => operation(cts.Token))
												.ToArray();

					await Task.WhenAny(tasks);
					cts.Cancel();

					Task[] faulted = tasks.Where(task => task.IsFaulted)
										  .Cast<Task>()
										  .ToArray();

					if (faulted.Any())
					{
						throw new AggregateException(faulted.SelectMany(task => task.Exception.InnerExceptions));
					}

					return tasks.First(task => task.IsCompleted).Result;
				};

			return getFirst();
		}

		public static Task<IEnumerable<T>> WhenAllOrFirstException<T>(IEnumerable<Task<T>> tasks)
		{
			tasks.ThrowWhen(
				when: sequence => sequence == null,
				what: () => new ArgumentNullException("tasks"));

			Task<T>[] input = tasks.ToArray();
			var countdown = new CountdownEvent(input.Length);
			var tcs = new TaskCompletionSource<IEnumerable<T>>();

			foreach (Task<T> task in input)
			{
				task.ContinueWith(parent =>
					{
						if (parent.IsFaulted)
						{
							tcs.TrySetException(parent.Exception.InnerExceptions);
						}
						else if (countdown.Signal() && !tcs.Task.IsCompleted)
						{
							tcs.TrySetResult(input.Select(completed => completed.Result));
						}
					});
			}

			return tcs.Task;
		}
	}
}
