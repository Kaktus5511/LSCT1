using LeagueSharp.Loader.Data;
using Microsoft.Build.Evaluation;
using PlaySharp.Service.WebService.Model;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;

namespace LeagueSharp.Loader.Class
{
	[Serializable]
	[XmlType(AnonymousType=true)]
	public class LeagueSharpAssembly : INotifyPropertyChanged
	{
		private string author;

		private string description;

		private string displayName = string.Empty;

		private bool injectChecked;

		private bool installChecked;

		private string name;

		private string pathToBinary;

		private string pathToProjectFile = string.Empty;

		private string svnUrl;

		private AssemblyType? type;

		public string Author
		{
			get
			{
				if (string.IsNullOrEmpty(this.SvnUrl) && string.IsNullOrEmpty(this.author))
				{
					return "Local";
				}
				try
				{
					if (string.IsNullOrEmpty(this.author))
					{
						AssemblyEntry assembly = WebService.Assemblies.FirstOrDefault<AssemblyEntry>((AssemblyEntry a) => Path.GetFileName(a.get_GithubUrl()) == Path.GetFileName(this.PathToProjectFile));
						if (assembly == null)
						{
							Match repositoryMatch = Regex.Match(this.SvnUrl, "^(http[s]?)://(?<host>.*?)/(?<author>.*?)/(?<repo>.*?)(/{1}|$)");
							if (repositoryMatch.Success)
							{
								this.author = repositoryMatch.Groups["author"].Value;
							}
						}
						else
						{
							this.author = assembly.get_AuthorName();
						}
					}
				}
				catch
				{
				}
				return this.author;
			}
			set
			{
				this.author = value;
				this.OnPropertyChanged("Author");
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
				this.OnPropertyChanged("Description");
			}
		}

		public string DisplayName
		{
			get
			{
				if (this.displayName != string.Empty)
				{
					return this.displayName;
				}
				return this.Name;
			}
			set
			{
				this.displayName = value;
				this.OnPropertyChanged("DisplayName");
			}
		}

		public bool InjectChecked
		{
			get
			{
				if (this.Type == 3)
				{
					return true;
				}
				return this.injectChecked;
			}
			set
			{
				this.injectChecked = value;
				this.OnPropertyChanged("InjectChecked");
			}
		}

		public bool InstallChecked
		{
			get
			{
				return this.installChecked;
			}
			set
			{
				this.installChecked = value;
				this.OnPropertyChanged("InstallChecked");
			}
		}

		public string Location
		{
			get
			{
				if (this.SvnUrl == string.Empty)
				{
					return "Local";
				}
				return this.SvnUrl;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				this.OnPropertyChanged("Name");
			}
		}

		public string PathToBinary
		{
			get
			{
				if (this.pathToBinary == null)
				{
					this.pathToBinary = Path.Combine((this.Type == 3 ? Directories.CoreDirectory : Directories.AssembliesDir), string.Concat((this.Type == 3 ? string.Empty : this.PathToProjectFile.GetHashCode().ToString("X")), Path.GetFileName(LeagueSharp.Loader.Class.Compiler.GetOutputFilePath(this.GetProject()))));
				}
				return this.pathToBinary;
			}
		}

		public string PathToProjectFile
		{
			get
			{
				if (File.Exists(this.pathToProjectFile))
				{
					return this.pathToProjectFile;
				}
				try
				{
					string repositoryDir = Directories.RepositoryDir;
					int hashCode = this.SvnUrl.GetHashCode();
					string folderToSearch = Path.Combine(repositoryDir, hashCode.ToString("X"), "trunk");
					if (Directory.Exists(folderToSearch))
					{
						string projectFile = Directory.GetFiles(folderToSearch, "*.csproj", SearchOption.AllDirectories).FirstOrDefault<string>((string file) => Path.GetFileNameWithoutExtension(file) == this.Name);
						if (!string.IsNullOrEmpty(projectFile))
						{
							this.OnPropertyChanged("PathToProjectFile");
							this.pathToProjectFile = projectFile;
							return projectFile;
						}
					}
				}
				catch
				{
				}
				return this.pathToProjectFile;
			}
			set
			{
				if (value.Contains("%AppData%"))
				{
					this.pathToProjectFile = value.Replace("%AppData%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
				}
				else
				{
					this.pathToProjectFile = value;
				}
				this.OnPropertyChanged("PathToProjectFile");
			}
		}

		public AssemblyStatus Status
		{
			get;
			set;
		}

		public string SvnUrl
		{
			get
			{
				RepositoryEntry redirect = (
					from r in Config.Instance.BlockedRepositories
					where r.get_HasRedirect()
					select r).FirstOrDefault<RepositoryEntry>((RepositoryEntry r) => r.get_Url() == this.svnUrl);
				if (redirect == null)
				{
					return this.svnUrl;
				}
				this.OnPropertyChanged("SvnUrl");
				return redirect.get_Redirect();
			}
			set
			{
				this.svnUrl = value;
				this.OnPropertyChanged("SvnUrl");
			}
		}

		[XmlIgnore]
		public AssemblyType Type
		{
			get
			{
				if (!this.type.HasValue)
				{
					AssemblyEntry assembly = WebService.Assemblies.FirstOrDefault<AssemblyEntry>((AssemblyEntry a) => {
						if (a.get_Name() == this.Name)
						{
							return true;
						}
						return a.get_Name() == this.DisplayName;
					});
					if (assembly != null)
					{
						this.type = new AssemblyType?(assembly.get_Type());
						return assembly.get_Type();
					}
					Project project = this.GetProject();
					if (project != null)
					{
						this.type = new AssemblyType?((project.GetPropertyValue("OutputType").ToLower().Contains("exe") ? 1 : 3));
					}
				}
				AssemblyType? nullable = this.type;
				if (!nullable.HasValue)
				{
					return 0;
				}
				return nullable.GetValueOrDefault();
			}
		}

		public string Version
		{
			get
			{
				if (this.Status != AssemblyStatus.Ready)
				{
					return this.Status.ToString();
				}
				if (string.IsNullOrEmpty(this.PathToBinary) || !File.Exists(this.PathToBinary))
				{
					return "?";
				}
				return AssemblyName.GetAssemblyName(this.PathToBinary).Version.ToString();
			}
		}

		public LeagueSharpAssembly()
		{
			this.Status = AssemblyStatus.Ready;
		}

		public LeagueSharpAssembly(string name, string path, string svnUrl)
		{
			this.Name = name;
			this.PathToProjectFile = path;
			this.SvnUrl = svnUrl;
			this.Description = string.Empty;
			this.Status = AssemblyStatus.Ready;
		}

		public bool Compile()
		{
			this.Status = AssemblyStatus.Compiling;
			this.OnPropertyChanged("Version");
			Project project = this.GetProject();
			if (!LeagueSharp.Loader.Class.Compiler.Compile(project, Path.Combine(Directories.LogsDir, string.Concat(this.Name, ".txt")), Logs.MainLog))
			{
				this.Status = AssemblyStatus.CompilingError;
				this.OnPropertyChanged("Version");
				return false;
			}
			bool result = true;
			string assemblySource = LeagueSharp.Loader.Class.Compiler.GetOutputFilePath(project);
			string assemblyDestination = this.PathToBinary;
			string pdbSource = Path.ChangeExtension(assemblySource, ".pdb");
			string pdbDestination = Path.ChangeExtension(assemblyDestination, ".pdb");
			if (File.Exists(assemblySource))
			{
				result = Utility.OverwriteFile(assemblySource, assemblyDestination, false);
			}
			if (File.Exists(pdbSource))
			{
				Utility.OverwriteFile(pdbSource, pdbDestination, false);
			}
			Utility.ClearDirectory(Path.Combine(project.DirectoryPath, "bin"));
			Utility.ClearDirectory(Path.Combine(project.DirectoryPath, "obj"));
			if (!result)
			{
				this.Status = AssemblyStatus.CompilingError;
			}
			else
			{
				this.Status = AssemblyStatus.Ready;
			}
			this.OnPropertyChanged("Version");
			this.OnPropertyChanged("Type");
			return result;
		}

		public LeagueSharpAssembly Copy()
		{
			return new LeagueSharpAssembly(this.Name, this.PathToProjectFile, this.SvnUrl);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is LeagueSharpAssembly))
			{
				return false;
			}
			return ((LeagueSharpAssembly)obj).PathToProjectFile == this.PathToProjectFile;
		}

		public static LeagueSharpAssembly FromAssemblyEntry(AssemblyEntry entry)
		{
			LeagueSharpAssembly leagueSharpAssembly;
			try
			{
				Match repositoryMatch = Regex.Match(entry.get_GithubUrl(), "^(http[s]?)://(?<host>.*?)/(?<author>.*?)/(?<repo>.*?)(/{1}|$)");
				string repositoryUrl = string.Format("https://{0}/{1}/{2}", repositoryMatch.Groups["host"], repositoryMatch.Groups["author"], repositoryMatch.Groups["repo"]);
				string repositoryDir = Directories.RepositoryDir;
				int hashCode = repositoryUrl.GetHashCode();
				string repositoryDirectory = Path.Combine(repositoryDir, hashCode.ToString("X"), "trunk");
				string path = Path.Combine(repositoryDirectory, entry.get_GithubUrl().Replace(repositoryUrl, string.Empty).Replace("/blob/master/", string.Empty).Replace("/", "\\"));
				leagueSharpAssembly = new LeagueSharpAssembly(entry.get_Name(), path, repositoryUrl);
			}
			catch
			{
				leagueSharpAssembly = null;
			}
			return leagueSharpAssembly;
		}

		public override int GetHashCode()
		{
			return this.PathToProjectFile.GetHashCode();
		}

		public Project GetProject()
		{
			Project project;
			if (File.Exists(this.PathToProjectFile))
			{
				try
				{
					ProjectFile pf = new ProjectFile(this.PathToProjectFile, Logs.MainLog)
					{
						Configuration = (Config.Instance.EnableDebug ? "Debug" : "Release"),
						PlatformTarget = "x86",
						ReferencesPath = Directories.CoreDirectory
					};
					pf.Change();
					project = pf.Project;
				}
				catch (Exception exception)
				{
					Exception e = exception;
					Utility.Log(LogStatus.Error, "Builder", string.Concat("Error: ", e), Logs.MainLog);
					return null;
				}
				return project;
			}
			return null;
		}

		public void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
			if (propertyChangedEventHandler == null)
			{
				return;
			}
			propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
		}

		public void Update()
		{
			if (this.Status == AssemblyStatus.Updating || this.SvnUrl == string.Empty)
			{
				return;
			}
			this.Status = AssemblyStatus.Updating;
			this.OnPropertyChanged("Version");
			try
			{
				GitUpdater.Update(this.SvnUrl);
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.ToString());
			}
			this.Status = AssemblyStatus.Ready;
			this.OnPropertyChanged("Version");
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}