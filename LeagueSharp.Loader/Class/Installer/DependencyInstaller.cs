using LeagueSharp.Loader.Class;
using LeagueSharp.Loader.Data;
using PlaySharp.Service.WebService.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueSharp.Loader.Class.Installer
{
	public class DependencyInstaller
	{
		public static List<Dependency> Cache
		{
			get;
			set;
		}

		public IReadOnlyList<string> Projects
		{
			get;
			set;
		}

		static DependencyInstaller()
		{
			DependencyInstaller.Cache = new List<Dependency>();
			DependencyInstaller.UpdateReferenceCache();
		}

		public DependencyInstaller(List<string> projects)
		{
			this.Projects = projects;
		}

		private bool IsInstalled(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return Config.Instance.Profiles.First<Profile>().InstalledAssemblies.Any<LeagueSharpAssembly>((LeagueSharpAssembly a) => Path.GetFileNameWithoutExtension(a.PathToBinary) == name);
		}

		private bool IsKnown(string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return DependencyInstaller.Cache.Any<Dependency>((Dependency d) => d.Name == name);
		}

		private static Dependency ParseAssemblyName(AssemblyEntry assembly)
		{
			Dependency dependency1;
			if (assembly == null)
			{
				throw new ArgumentNullException("assembly");
			}
			try
			{
				string project = assembly.get_GithubUrl();
				project = project.Replace("https://github.com/", "https://raw.githubusercontent.com/");
				project = project.Replace("/blob/master/", "/master/");
				using (WebClientEx client = new WebClientEx())
				{
					Dependency dependency = Dependency.FromAssemblyEntry(assembly);
					Match assemblyNameMatch = Regex.Match(client.DownloadString(project), "<AssemblyName>(?<name>.*?)</AssemblyName>");
					dependency.Name = assemblyNameMatch.Groups["name"].Value;
					dependency1 = dependency;
				}
			}
			catch
			{
				Utility.Log(LogStatus.Info, "ParseAssemblyName", string.Format("Invalid Library: {0} - {1}", assembly.get_Id(), assembly.get_GithubUrl()), Logs.MainLog);
				return null;
			}
			return dependency1;
		}

		private List<string> ParseReferences(string project)
		{
			if (project == null)
			{
				throw new ArgumentNullException("project");
			}
			List<string> projectReferences = new List<string>();
			try
			{
				foreach (Match match in Regex.Matches(File.ReadAllText(project), "<Reference Include=\"(?<assembly>.*?)\"(?<space>.*?)>"))
				{
					string m = match.Groups["assembly"].Value;
					if (m.Contains(","))
					{
						m = m.Split(new char[] { ',' })[0];
					}
					projectReferences.Add(m);
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
			return projectReferences;
		}

		public async Task<bool> SatisfyAsync()
		{
			DependencyInstaller.<SatisfyAsync>d__10 variable = new DependencyInstaller.<SatisfyAsync>d__10();
			variable.<>4__this = this;
			variable.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			variable.<>1__state = -1;
			variable.<>t__builder.Start<DependencyInstaller.<SatisfyAsync>d__10>(ref variable);
			return variable.<>t__builder.Task;
		}

		private static void UpdateReferenceCache()
		{
			List<AssemblyEntry> assemblies = new List<AssemblyEntry>();
			try
			{
				assemblies = WebService.Assemblies.Where<AssemblyEntry>((AssemblyEntry a) => {
					if (a.get_Type() != 3 || a.get_Deleted())
					{
						return false;
					}
					return a.get_Approved();
				}).ToList<AssemblyEntry>();
			}
			catch (Exception exception)
			{
				Exception e = exception;
				Utility.Log(LogStatus.Error, "UpdateReferenceCache", e.Message, Logs.MainLog);
			}
			DependencyInstaller.Cache.Clear();
			foreach (AssemblyEntry lib in assemblies)
			{
				DependencyInstaller.Cache.Add(DependencyInstaller.ParseAssemblyName(lib));
			}
			DependencyInstaller.Cache.RemoveAll((Dependency a) => a == null);
		}
	}
}