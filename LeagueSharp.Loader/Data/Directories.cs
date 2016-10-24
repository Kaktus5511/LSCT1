using System;
using System.Diagnostics;
using System.IO;

namespace LeagueSharp.Loader.Data
{
	public static class Directories
	{
		public readonly static string AppDataDirectory;

		public readonly static string AssembliesDir;

		public readonly static string CurrentDirectory;

		public readonly static string CoreDirectory;

		public readonly static string BootstrapFilePath;

		public readonly static string ConfigFilePath;

		public readonly static string CoreBridgeFilePath;

		public readonly static string CoreFilePath;

		public readonly static string LoaderFilePath;

		public readonly static string LocalRepoDir;

		public readonly static string LogsDir;

		public readonly static string RepositoryDir;

		public readonly static string SandboxFilePath;

		public static string AssemblyConfigFile
		{
			get
			{
				object randomName;
				string currentDirectory = Directories.CurrentDirectory;
				Config instance = Config.Instance;
				if (instance != null)
				{
					randomName = instance.RandomName;
				}
				else
				{
					randomName = null;
				}
				return Path.Combine(currentDirectory, string.Format("{0}.exe.config", randomName));
			}
		}

		public static string AssemblyFile
		{
			get
			{
				object randomName;
				string currentDirectory = Directories.CurrentDirectory;
				Config instance = Config.Instance;
				if (instance != null)
				{
					randomName = instance.RandomName;
				}
				else
				{
					randomName = null;
				}
				return Path.Combine(currentDirectory, string.Format("{0}.exe", randomName));
			}
		}

		public static string AssemblyPdbFile
		{
			get
			{
				object randomName;
				string currentDirectory = Directories.CurrentDirectory;
				Config instance = Config.Instance;
				if (instance != null)
				{
					randomName = instance.RandomName;
				}
				else
				{
					randomName = null;
				}
				return Path.Combine(currentDirectory, string.Format("{0}.pdb", randomName));
			}
		}

		static Directories()
		{
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			int hashCode = Environment.UserName.GetHashCode();
			Directories.AppDataDirectory = string.Concat(Path.Combine(folderPath, string.Concat("LS", hashCode.ToString("X"))), "\\");
			Directories.AssembliesDir = string.Concat(Path.Combine(Directories.AppDataDirectory, "1"), "\\");
			Directories.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
			Directories.CoreDirectory = string.Concat(Path.Combine(Directories.CurrentDirectory, "System"), "\\");
			Directories.BootstrapFilePath = Path.Combine(Directories.CoreDirectory, "LeagueSharp.Bootstrap.dll");
			Directories.ConfigFilePath = Path.Combine(Directories.CurrentDirectory, "config.xml");
			Directories.CoreBridgeFilePath = Path.Combine(Directories.CoreDirectory, "Leaguesharp.dll");
			Directories.CoreFilePath = Path.Combine(Directories.CoreDirectory, "Leaguesharp.Core.dll");
			Directories.LoaderFilePath = Path.Combine(Directories.CurrentDirectory, Process.GetCurrentProcess().ProcessName);
			Directories.LocalRepoDir = string.Concat(Path.Combine(Directories.CurrentDirectory, "LocalAssemblies"), "\\");
			Directories.LogsDir = string.Concat(Path.Combine(Directories.CurrentDirectory, "Logs"), "\\");
			Directories.RepositoryDir = string.Concat(Path.Combine(Directories.AppDataDirectory, "Repositories"), "\\");
			Directories.SandboxFilePath = Path.Combine(Directories.CoreDirectory, "LeagueSharp.Sandbox.dll");
			Directory.CreateDirectory(Directories.AssembliesDir);
			Directory.CreateDirectory(Directories.RepositoryDir);
			Directory.CreateDirectory(Directories.LogsDir);
		}
	}
}