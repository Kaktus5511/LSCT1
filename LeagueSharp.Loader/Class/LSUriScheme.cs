using LeagueSharp.Loader.Views;
using MahApps.Metro.Controls;
using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueSharp.Loader.Class
{
	public static class LSUriScheme
	{
		public const string Name = "ls";

		public static string FullName
		{
			get
			{
				return "ls://";
			}
		}

		public static async Task HandleUrl(string url, MetroWindow window)
		{
			url = url.Remove(0, LSUriScheme.FullName.Length).WebDecode();
			foreach (Match match in Regex.Matches(url, "(project|projectGroup)/([^/]*)/([^/]*)/([^/]*)/?"))
			{
				if (match.Groups[1].ToString() != "project")
				{
					continue;
				}
				InstallerWindow.InstallAssembly(match);
			}
		}
	}
}