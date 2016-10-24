using System;

namespace LeagueSharp.Loader.Data
{
	public static class Logs
	{
		public static Log MainLog;

		static Logs()
		{
			Logs.MainLog = new Log();
		}
	}
}