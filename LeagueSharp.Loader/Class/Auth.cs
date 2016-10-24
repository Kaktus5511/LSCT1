using PlaySharp.Service.WebService;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace LeagueSharp.Loader.Class
{
	internal static class Auth
	{
		public static string Hash(string input)
		{
			return Utility.Md5Hash(Auth.IPB_Clean_Password(input));
		}

		private static string IPB_Clean_Password(string pass)
		{
			pass = pass.Replace("Ã\u008a", string.Empty);
			pass = pass.Replace("&", "&amp;");
			pass = pass.Replace("\\", "&#092;");
			pass = pass.Replace("!", "&#33;");
			pass = pass.Replace("$", "&#036;");
			pass = pass.Replace("\"", "&quot;");
			pass = pass.Replace("\"", "&quot;");
			pass = pass.Replace("<", "&lt;");
			pass = pass.Replace(">", "&gt;");
			pass = pass.Replace("'", "&#39;");
			return pass;
		}

		public static async Task<Tuple<bool, string>> Login(string user, string hash)
		{
			Auth.<Login>d__1 variable = new Auth.<Login>d__1();
			variable.user = user;
			variable.hash = hash;
			variable.<>t__builder = AsyncTaskMethodBuilder<Tuple<bool, string>>.Create();
			variable.<>1__state = -1;
			variable.<>t__builder.Start<Auth.<Login>d__1>(ref variable);
			return variable.<>t__builder.Task;
		}
	}
}