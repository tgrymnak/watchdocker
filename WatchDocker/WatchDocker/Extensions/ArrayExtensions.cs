using System;
using System.Linq;
using WatchDocker.Constants;

namespace WatchDocker.Extensions
{
	internal static class ArrayExtensions
	{
		public static bool IsArgumentPresent(this string[] args, string arg)
		{
			var options = arg.Split(new[] { Separators.Space, Separators.Comma }, StringSplitOptions.RemoveEmptyEntries).Select(e => e.ToLowerInvariant()).ToArray();
			var argument = args.FirstOrDefault(e => options.Any(a => e.Equals(a, StringComparison.InvariantCultureIgnoreCase)));
			return argument.IsNotEmpty();
		}

		public static T GetArgumentValueOrDefault<T>(this string[] args, string arg, T defaultValue = default)
		{
			var options = arg.Split(new[] { Separators.Space, Separators.Comma }, StringSplitOptions.RemoveEmptyEntries).Select(e => e.ToLowerInvariant()).ToArray();
			var argument = args.FirstOrDefault(e => options.Any(a => e.StartsWith(a, StringComparison.InvariantCultureIgnoreCase)));
			if (argument.IsEmpty())
			{
				return defaultValue;
			}

			var values = argument.Split(new[] { Separators.EqualSign, Separators.Space }, StringSplitOptions.RemoveEmptyEntries);
			if (!values.Any() || values.Length != 2 || values.First().ToLowerInvariant().IsNotIn(options))
			{
				return defaultValue;
			}

			return (T)Convert.ChangeType(values.Last(), typeof(T));
		}

		public static bool IsNotIn(this string value, params string[] array)
		{
			return !array.Contains(value);
		}
	}
}
