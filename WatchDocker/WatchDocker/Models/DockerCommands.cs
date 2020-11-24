using WatchDocker.Extensions;

namespace WatchDocker.Models
{
	public class DockerCommands
	{
		private readonly string _registry;
		private readonly string _username;
		private readonly string _password;
		private readonly string _image;
		private readonly string _container;
		private readonly string _expose;

		public DockerCommands(string registry, string username, string password, string image, string container, string expose)
		{
			_registry = registry;
			_username = username;
			_password = password;
			_image = image;
			_container = container;
			_expose = expose;
		}

		public string Login => "docker login".AppendWithPrefixIfNotEmpty(_registry).AppendWithPrefixIfNotEmpty(_username, "-u").AppendWithPrefixIfNotEmpty(_password, "-p");
		public string PullImage => "docker pull".AppendWithPathIfNotEmpty(_image, _registry);
		public string StopContainer => $"docker stop {_container}";
		public string RemoveContainer => $"docker rm {_container}";
		public string RunContainer => "docker run -d".AppendWithPrefixIfNotEmpty(_expose, "-p").AppendWithPrefixIfNotEmpty(_container, "--name").AppendWithPathIfNotEmpty(_image, _registry);
		public string PruneImages => $"docker image prune -f";
	}
}
