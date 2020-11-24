using System;
using System.Threading;

namespace WatchDocker.Utils
{
	/// <summary>
	/// Responsible for scheduling and executing specific tasks periodically
	/// </summary>
	public class TaskScheduler : IDisposable
	{
		private readonly Timer Timer;

		/// <summary>
		/// Creates instance of <see cref="TaskScheduler(int, Action)"/>
		/// </summary>
		/// <param name="interval">Time interval between invocations in seconds</param>
		/// <param name="action">Action to be executed</param>
		public TaskScheduler(int interval, Action<object> action)
		{
			Timer = new Timer(new TimerCallback(action), null, TimeSpan.Zero, TimeSpan.FromSeconds(interval));
		}

		/// <summary>
		/// Disposes all resources held by scheduler
		/// </summary>
		public void Dispose()
		{
			Timer?.Dispose();
		}
	}
}
