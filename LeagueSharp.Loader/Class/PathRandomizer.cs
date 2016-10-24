using LeagueSharp.Loader.Data;
using PlaySharp.Toolkit.StrongName;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace LeagueSharp.Loader.Class
{
	internal class PathRandomizer
	{
		public static Random RandomNumberGenerator;

		private static string _leagueSharpBootstrapDllName;

		private static string _leagueSharpCoreDllName;

		private static string _leagueSharpDllName;

		private static string _leagueSharpSandBoxDllName;

		private static PathRandomizer.ModifyIATDelegate ModifyIAT;

		public static string BaseDirectory
		{
			get
			{
				return Directories.AssembliesDir;
			}
		}

		public static string LeagueSharpBootstrapDllName
		{
			get
			{
				if (PathRandomizer._leagueSharpBootstrapDllName == null)
				{
					PathRandomizer._leagueSharpBootstrapDllName = PathRandomizer.GetRandomName("LeagueSharp.Bootstrap.dll");
				}
				return PathRandomizer._leagueSharpBootstrapDllName;
			}
		}

		public static string LeagueSharpBootstrapDllPath
		{
			get
			{
				return Path.Combine(PathRandomizer.BaseDirectory, PathRandomizer.LeagueSharpBootstrapDllName);
			}
		}

		public static string LeagueSharpCoreDllName
		{
			get
			{
				if (PathRandomizer._leagueSharpCoreDllName == null)
				{
					PathRandomizer._leagueSharpCoreDllName = PathRandomizer.GetRandomName("LeagueSharp.Core.dll");
				}
				return PathRandomizer._leagueSharpCoreDllName;
			}
		}

		public static string LeagueSharpCoreDllPath
		{
			get
			{
				return Path.Combine(PathRandomizer.BaseDirectory, PathRandomizer.LeagueSharpCoreDllName);
			}
		}

		public static string LeagueSharpDllName
		{
			get
			{
				if (PathRandomizer._leagueSharpDllName == null)
				{
					PathRandomizer._leagueSharpDllName = PathRandomizer.GetRandomName("LeagueSharp.dll");
				}
				return PathRandomizer._leagueSharpDllName;
			}
		}

		public static string LeagueSharpDllPath
		{
			get
			{
				return Path.Combine(PathRandomizer.BaseDirectory, PathRandomizer.LeagueSharpDllName);
			}
		}

		public static string LeagueSharpSandBoxDllName
		{
			get
			{
				if (PathRandomizer._leagueSharpSandBoxDllName == null)
				{
					PathRandomizer._leagueSharpSandBoxDllName = PathRandomizer.GetRandomName("LeagueSharp.SandBox.dll");
				}
				return PathRandomizer._leagueSharpSandBoxDllName;
			}
		}

		public static string LeagueSharpSandBoxDllPath
		{
			get
			{
				return Path.Combine(PathRandomizer.BaseDirectory, PathRandomizer.LeagueSharpSandBoxDllName);
			}
		}

		static PathRandomizer()
		{
			PathRandomizer.RandomNumberGenerator = new Random();
			PathRandomizer._leagueSharpBootstrapDllName = null;
			PathRandomizer._leagueSharpCoreDllName = null;
			PathRandomizer._leagueSharpDllName = null;
			PathRandomizer._leagueSharpSandBoxDllName = null;
			PathRandomizer.ModifyIAT = null;
		}

		public PathRandomizer()
		{
		}

		public static bool CopyFiles()
		{
			bool flag;
			bool result = true;
			if (PathRandomizer.ModifyIAT == null)
			{
				PathRandomizer.ResolveImports();
			}
			if (PathRandomizer.ModifyIAT == null)
			{
				return false;
			}
			if (!File.Exists(Path.Combine(Directories.CoreDirectory, "LeagueSharp.Core.dll")))
			{
				return false;
			}
			if (!File.Exists(Path.Combine(Directories.CoreDirectory, "LeagueSharp.dll")))
			{
				return false;
			}
			try
			{
				result = (!result ? false : Utility.OverwriteFile(Path.Combine(Directories.CoreDirectory, "LeagueSharp.Core.dll"), PathRandomizer.LeagueSharpCoreDllPath, true));
				result = (!result ? false : Utility.OverwriteFile(Path.Combine(Directories.CoreDirectory, "LeagueSharp.Bootstrap.dll"), PathRandomizer.LeagueSharpBootstrapDllPath, true));
				result = (!result ? false : Utility.OverwriteFile(Path.Combine(Directories.CoreDirectory, "LeagueSharp.SandBox.dll"), PathRandomizer.LeagueSharpSandBoxDllPath, true));
				byte[] byteArray = File.ReadAllBytes(Path.Combine(Directories.CoreDirectory, "LeagueSharp.dll"));
				byteArray = Utility.ReplaceFilling(byteArray, Encoding.ASCII.GetBytes("LeagueSharp.Core.dll"), Encoding.ASCII.GetBytes(PathRandomizer.LeagueSharpCoreDllName));
				File.WriteAllBytes(PathRandomizer.LeagueSharpDllPath, byteArray);
				result = (!result ? false : StrongNameUtility.ReSign(PathRandomizer.LeagueSharpDllPath, Path.Combine(Directories.CoreDirectory, "key.snk"), null));
				flag = result;
			}
			catch (Exception exception)
			{
				flag = false;
			}
			return flag;
		}

		public static string GetRandomName(string oldName)
		{
			string ar1 = Utility.Md5Hash(oldName);
			string ar2 = Utility.Md5Hash(Config.Instance.Username);
			string result = string.Empty;
			for (int i = 0; i < Math.Min(15, Math.Max(3, Config.Instance.Username.Length)); i++)
			{
				int j = ar1.ToCharArray()[i] * ar2.ToCharArray()[i] * 2;
				j = j % ("0123456789abcdefhijkmnopqrstuvwxyz".Length - 1);
				char chr = "0123456789abcdefhijkmnopqrstuvwxyz"[j];
				result = string.Concat(result, chr.ToString());
			}
			return string.Concat(result, ".dll");
		}

		public static void ResolveImports()
		{
			IntPtr hModule = Win32Imports.LoadLibrary(Directories.BootstrapFilePath);
			if (hModule == IntPtr.Zero)
			{
				return;
			}
			IntPtr procAddress = Win32Imports.GetProcAddress(hModule, "ModifyIAT");
			if (procAddress == IntPtr.Zero)
			{
				return;
			}
			PathRandomizer.ModifyIAT = Marshal.GetDelegateForFunctionPointer(procAddress, typeof(PathRandomizer.ModifyIATDelegate)) as PathRandomizer.ModifyIATDelegate;
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate bool ModifyIATDelegate(string modulePath, string newModulePath, string moduleName, string newModuleName);
	}
}