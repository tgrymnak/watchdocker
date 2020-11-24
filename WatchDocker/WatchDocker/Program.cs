using System;
using System.Collections.Generic;
using System.Threading;
using WatchDocker.Constants;
using WatchDocker.Models;
using WatchDocker.Utils;

namespace WatchDocker
{
	class Program
	{
		static DockerCommands DockerCommands;
		static Arguments Arguments;

		static void Main(string[] args)
		{
			Arguments = new Arguments(args);
			if (Arguments.ShowHelp || !Arguments.IsValid)
			{
				ShowHelp(!Arguments.ShowHelp && !Arguments.IsValid);
				return;
			}

			DockerCommands = new DockerCommands(Arguments.Registry, Arguments.Username, Arguments.Password, Arguments.Image, Arguments.Container, Arguments.Expose);
			using var cli = new CommandLine(Arguments.Shell, ProcessOutput);
			using var scheduler = new TaskScheduler(Arguments.Interval, (object obj) =>
			{
				Console.WriteLine($"Trying to schedule new task. Next execution at {DateTime.Now.AddSeconds(Arguments.Interval)}");
				if (!cli.IsBusy) cli.Command(DockerCommands.Login, DockerCommands.PullImage).Process();
			});

			while (true)
			{
				Console.WriteLine("Idle...");
				Thread.Sleep(Defaults.IdleTime * 1000);
			}
		}

		static void ProcessOutput(CommandLine cli, string data)
		{
			if (data == null) return;

			Console.WriteLine($"CLI: {data}");
			if (data.Contains("Status:"))
			{
				if (data.Contains("Downloaded newer image for"))
				{
					cli.Command(DockerCommands.StopContainer, DockerCommands.RemoveContainer, DockerCommands.RunContainer, DockerCommands.PruneImages).Process();
				}
				else if (data.Contains("Image is up to date for"))
				{
					// Container is up to date
				}
			}
		}

		static void ShowHelp(bool argumentsInvalid = false)
		{
			if (argumentsInvalid)
			{
				Console.WriteLine("Some of input arguments were not present or invalid. See --help section.\n");
			}

			Console.WriteLine("Usage:\n\ndocker run --name watchdocker -d watchdocker -s=[shell] -u=[username] -p=[password or token] -r=[registry] -i=[image name] -c=[container name] -e=[port:port] -t=[interval in seconds]\n");
			var options = new List<(string Command, bool Required, string Description)>
			{
				("-s, --shell", false, "Shell to be used to execute commands (automatically detected shell will be used if option isn't specified)"),
				("-u, --username", false, "Username to connect to docker registry"),
				("-p, --password", false, "Password to connect to docker registry"),
				("-r, --registry", false, "Registry where images are kept"),
				("-i, --image", true, "Image name to pull from docker registry"),
				("-c, --container", true, "Container name to watch"),
				("-e, --expose", false, "Ports to be exposed (443:443 will be used if option isn't specified)"),
				("-t, --time", false, "Time interval between invocations (checks for the newer images) in seconds")
			};

			Console.WriteLine("Options:\n");
			foreach (var option in options)
			{
				Console.WriteLine("{0,-15} {1,5} {2}", option.Command, option.Required ? "(*)" : "( )", option.Description);
			}
		}
	}
}
