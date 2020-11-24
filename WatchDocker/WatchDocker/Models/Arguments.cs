using WatchDocker.Constants;
using WatchDocker.Extensions;

namespace WatchDocker.Models
{
	public class Arguments
	{
		private readonly string[] _args;

		public Arguments(string[] args)
		{
			_args = args;
		}

		public string Shell => _args.GetArgumentValueOrDefault<string>(Args.Shell);
		public string Username => _args.GetArgumentValueOrDefault<string>(Args.Username);
		public string Password => _args.GetArgumentValueOrDefault<string>(Args.Password);
		public string Registry => _args.GetArgumentValueOrDefault<string>(Args.Registry);
		public string Image => _args.GetArgumentValueOrDefault<string>(Args.Image);
		public string Container => _args.GetArgumentValueOrDefault<string>(Args.Container);
		public int Interval => _args.GetArgumentValueOrDefault(Args.Interval, Defaults.DefaultInterval);
		public string Expose => _args.GetArgumentValueOrDefault(Args.Expose, Defaults.DefaultExposedPorts);
		public bool ShowHelp => _args.IsArgumentPresent(Args.Help);
		public bool IsValid => Image.IsNotEmpty() && Container.IsNotEmpty();
	}
}
