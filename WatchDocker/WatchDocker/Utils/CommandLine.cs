using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WatchDocker.Extensions;

namespace WatchDocker.Utils
{
	/// <summary>
	/// Provides an interface to work with specific shell
	/// </summary>
	public class CommandLine : IDisposable
	{
		private static readonly ConcurrentQueue<Task> Commands = new ConcurrentQueue<Task>();
		private static readonly CancellationTokenSource CancellationToken = new CancellationTokenSource();
		public bool IsWindows => Environment.OSVersion.Platform.ToString().Contains(Constants.Platform.Win, StringComparison.InvariantCultureIgnoreCase);
		public bool IsBusy => !Commands.IsEmpty && Commands.ToArray().Any(e => e.Status == TaskStatus.Running);
		public string Shell { get; private set; }
		public Action<CommandLine, string> Output { get; private set; }

		/// <summary>
		/// Creates instance of <see cref="CommandLine(string, Action)"/>
		/// </summary>
		/// <param name="shell">The shell to be used to execute commands (automatically detected shell will be used if not specified)</param>
		/// <param name="output">Action to be used to handle the output from the commands</param>
		public CommandLine(string shell = null, Action<CommandLine, string> output = null)
		{
			Shell = shell.IsNotEmpty() ? shell : IsWindows ? Constants.Shell.Cmd : Constants.Shell.Bash;
			Output = output;
		}

		/// <summary>
		/// Appends commands to the commands queue to be processed later
		/// </summary>
		/// <param name="commands">Command line commands</param>
		/// <returns></returns>
		public CommandLine Command(params string[] commands)
		{
			foreach (var command in commands)
			{
				Commands.Enqueue(new Task(() =>
				{
					try
					{
						var c = IsWindows ? "/c" : "-c";

						var startInfo = new ProcessStartInfo
						{
							FileName = Shell,
							Arguments = $"{c} \"{command}\"",
							RedirectStandardOutput = true
						};

						using var process = new Process { StartInfo = startInfo };
						process.OutputDataReceived += (sender, data) => Output?.Invoke(this, data.Data);
						process.Start();
						process.BeginOutputReadLine();
						process.WaitForExit();
					}
					catch (Exception ex)
					{
						Output?.Invoke(this, ex.Message);
					}
				}, CancellationToken.Token));
			}

			return this;
		}

		/// <summary>
		/// Processes all commands in FIFO order
		/// </summary>
		public void Process()
		{
			Task.Run(() =>
			{
				lock (Commands)
				{
					while (!Commands.IsEmpty)
					{
						if (Commands.TryPeek(out Task task))
						{
							if (task.Status == TaskStatus.Created)
							{
								task.Start();
							}
							else if (task.IsCompleted || task.IsCanceled)
							{
								Commands.TryDequeue(out _);
							}
						}
					}
				}
			}, CancellationToken.Token);
		}

		/// <summary>
		/// Disposes all resources held by the command line
		/// </summary>
		public void Dispose()
		{
			while (!Commands.IsEmpty)
			{
				Commands.TryDequeue(out Task _);
				CancellationToken.Cancel(true);
			}
		}
	}
}
