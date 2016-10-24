using System;
using System.Net;
using System.Net.Cache;

namespace LeagueSharp.Loader.Class.Installer
{
	public class WebClientEx : WebClient
	{
		public WebClientEx()
		{
		}

		protected override WebRequest GetWebRequest(Uri uri)
		{
			HttpWebRequest request = base.GetWebRequest(uri) as HttpWebRequest;
			request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.Revalidate);
			request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
			request.Timeout = 10000;
			return request;
		}
	}
}