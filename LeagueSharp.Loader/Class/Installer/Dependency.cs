using LeagueSharp.Loader.Class;
using LeagueSharp.Loader.Views;
using PlaySharp.Service.WebService.Model;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace LeagueSharp.Loader.Class.Installer
{
	public class Dependency
	{
		public PlaySharp.Service.WebService.Model.AssemblyEntry AssemblyEntry
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Project
		{
			get;
			set;
		}

		public string Repository
		{
			get;
			set;
		}

		public Dependency()
		{
		}

		public static Dependency FromAssemblyEntry(PlaySharp.Service.WebService.Model.AssemblyEntry assembly)
		{
			Dependency dependency;
			try
			{
				Match repositoryMatch = Regex.Match(assembly.get_GithubUrl(), "^(http[s]?)://(?<host>.*?)/(?<author>.*?)/(?<repo>.*?)(/{1}|$)");
				string projectName = assembly.get_GithubUrl().Substring(assembly.get_GithubUrl().LastIndexOf("/") + 1);
				string repositoryUrl = string.Format("https://{0}/{1}/{2}", repositoryMatch.Groups["host"], repositoryMatch.Groups["author"], repositoryMatch.Groups["repo"]);
				dependency = new Dependency()
				{
					AssemblyEntry = assembly,
					Repository = repositoryUrl,
					Project = projectName.WebDecode(),
					Name = assembly.get_Name().WebDecode(),
					Description = assembly.get_Description().WebDecode()
				};
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
				return null;
			}
			return dependency;
		}

		public async Task<bool> InstallAsync()
		{
			bool flag;
			try
			{
				await InstallerWindow.InstallAssembly(this.AssemblyEntry, true);
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public override string ToString()
		{
			return string.Format("{0} - {1} - {2}", this.Name, this.Project, this.Repository);
		}
	}
}