namespace WatchDocker.Extensions
{
	internal static class StringExtensions
	{
		public static bool IsEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}

		public static bool IsNotEmpty(this string value)
		{
			return !string.IsNullOrEmpty(value);
		}

		public static string AppendWithPrefixIfNotEmpty(this string value, string parameter, string prefix = null)
		{
			return parameter.IsNotEmpty() ? $"{value} {prefix} {parameter}" : value;
		}

		public static string AppendWithPathIfNotEmpty(this string value, string parameter, string path = null)
		{
			var fullPath = path.IsNotEmpty() ? $"{path}/{parameter}" : parameter;
			return parameter.IsNotEmpty() ? $"{value} {fullPath}" : value;
		}
	}
}
