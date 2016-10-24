using System;
using System.Runtime.CompilerServices;

namespace LeagueSharp.Loader.Class
{
	public static class DateExtensions
	{
		public static int ToUnixTimestamp(this DateTime date)
		{
			TimeSpan timeSpan = date.Subtract(new DateTime(1970, 1, 1));
			return (int)timeSpan.TotalSeconds;
		}
	}
}