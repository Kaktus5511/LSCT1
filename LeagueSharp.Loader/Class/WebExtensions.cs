using System;
using System.Runtime.CompilerServices;
using System.Web;

namespace LeagueSharp.Loader.Class
{
	public static class WebExtensions
	{
		public static string WebDecode(this string s)
		{
			return HttpUtility.HtmlDecode(HttpUtility.UrlDecode(s));
		}
	}
}