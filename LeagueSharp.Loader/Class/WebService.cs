using PlaySharp.Service.WebService;
using PlaySharp.Service.WebService.Model;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LeagueSharp.Loader.Class
{
	internal static class WebService
	{
		private static IReadOnlyList<AssemblyEntry> assemblies;

		public static IReadOnlyList<AssemblyEntry> Assemblies
		{
			get
			{
				try
				{
					if (!WebService.Client.get_IsAuthenticated())
					{
						return new AssemblyEntry[0];
					}
					else if (WebService.assemblies.Count == 0)
					{
						WebService.assemblies = WebService.Client.Assemblies();
					}
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception);
				}
				return WebService.assemblies;
			}
		}

		public static WebServiceClient Client
		{
			get;
		}

		static WebService()
		{
			WebService.assemblies = new List<AssemblyEntry>();
			WebService.Client = new WebServiceClient();
		}
	}
}