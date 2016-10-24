using LeagueSharp.Loader;
using LeagueSharp.Loader.Class;
using Newtonsoft.Json;
using PlaySharp.Service.WebService;
using PlaySharp.Service.WebService.Endpoints;
using PlaySharp.Service.WebService.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace LeagueSharp.Loader.Data
{
	[XmlRoot(Namespace="", IsNullable=false)]
	[XmlType(AnonymousType=true)]
	public class Config : INotifyPropertyChanged
	{
		private string authKey;

		private ObservableCollection<RepositoryEntry> blockedRepositories = new ObservableCollection<RepositoryEntry>();

		private bool championCheck = true;

		private double columnCheckWidth = 20;

		private double columnLocationWidth = 180;

		private double columnNameWidth = 150;

		private double columnTypeWidth = 75;

		private double columnVersionWidth = 90;

		private ObservableCollection<AssemblyEntry> databaseAssemblies = new ObservableCollection<AssemblyEntry>();

		private bool enableDebug;

		private bool firstRun = true;

		private LeagueSharp.Loader.Data.Hotkeys hotkeys;

		private bool install = true;

		private ObservableCollection<RepositoryEntry> knownRepositories = new ObservableCollection<RepositoryEntry>();

		private string leagueOfLegendsExePath;

		private bool libraryCheck = true;

		private string password;

		private ObservableCollection<Profile> profiles = new ObservableCollection<Profile>();

		private string searchText = string.Empty;

		private string selectedColor;

		private string selectedLanguage;

		private int selectedProfileId;

		private ConfigSettings settings;

		private bool showDevOptions;

		private bool tosAccepted;

		private bool updateCoreOnInject = true;

		private bool updateOnLoad;

		private bool useCloudConfig = true;

		private string username;

		private bool utilityCheck = true;

		private double windowHeight = 450;

		private double windowLeft = 150;

		private double windowTop = 150;

		private double windowWidth = 800;

		private int workers = 5;

		public string AuthKey
		{
			get
			{
				return this.authKey;
			}
			set
			{
				this.authKey = value;
				this.OnPropertyChanged("AuthKey");
			}
		}

		public ObservableCollection<RepositoryEntry> BlockedRepositories
		{
			get
			{
				return this.blockedRepositories;
			}
			set
			{
				this.blockedRepositories = value;
				this.OnPropertyChanged("BlockedRepositories");
			}
		}

		public bool ChampionCheck
		{
			get
			{
				return this.championCheck;
			}
			set
			{
				this.championCheck = value;
				this.OnPropertyChanged("ChampionCheck");
			}
		}

		public double ColumnCheckWidth
		{
			get
			{
				return this.columnCheckWidth;
			}
			set
			{
				this.columnCheckWidth = value;
				this.OnPropertyChanged("ColumnCheckWidth");
			}
		}

		public double ColumnLocationWidth
		{
			get
			{
				return this.columnLocationWidth;
			}
			set
			{
				this.columnLocationWidth = value;
				this.OnPropertyChanged("ColumnLocationWidth");
			}
		}

		public double ColumnNameWidth
		{
			get
			{
				return this.columnNameWidth;
			}
			set
			{
				this.columnNameWidth = value;
				this.OnPropertyChanged("ColumnNameWidth");
			}
		}

		public double ColumnTypeWidth
		{
			get
			{
				return this.columnTypeWidth;
			}
			set
			{
				this.columnTypeWidth = value;
				this.OnPropertyChanged("ColumnTypeWidth");
			}
		}

		public double ColumnVersionWidth
		{
			get
			{
				return this.columnVersionWidth;
			}
			set
			{
				this.columnVersionWidth = value;
				this.OnPropertyChanged("ColumnVersionWidth");
			}
		}

		[JsonIgnore]
		[XmlIgnore]
		public ObservableCollection<AssemblyEntry> DatabaseAssemblies
		{
			get
			{
				return this.databaseAssemblies;
			}
			set
			{
				this.databaseAssemblies = value;
				this.OnPropertyChanged("DatabaseAssemblies");
			}
		}

		public bool EnableDebug
		{
			get
			{
				return this.enableDebug;
			}
			set
			{
				this.enableDebug = value;
				this.OnPropertyChanged("EnableDebug");
			}
		}

		public bool FirstRun
		{
			get
			{
				return this.firstRun;
			}
			set
			{
				this.firstRun = value;
				this.OnPropertyChanged("FirstRun");
			}
		}

		public LeagueSharp.Loader.Data.Hotkeys Hotkeys
		{
			get
			{
				return this.hotkeys;
			}
			set
			{
				this.hotkeys = value;
				this.OnPropertyChanged("Hotkeys");
			}
		}

		[JsonIgnore]
		[XmlIgnore]
		public static Config Instance
		{
			get;
			set;
		}

		[JsonIgnore]
		[XmlIgnore]
		public ObservableCollection<RepositoryEntry> KnownRepositories
		{
			get
			{
				return this.knownRepositories;
			}
			set
			{
				this.knownRepositories = value;
				this.OnPropertyChanged("KnownRepositories");
			}
		}

		public string LeagueOfLegendsExePath
		{
			get
			{
				return this.leagueOfLegendsExePath;
			}
			set
			{
				if (value.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
				{
					this.leagueOfLegendsExePath = value;
					this.OnPropertyChanged("LeagueOfLegendsExePath");
					return;
				}
				Utility.Log(LogStatus.Error, "LeagueOfLegendsExePath", string.Format("Invalid file: {0}", value), Logs.MainLog);
			}
		}

		public bool LibraryCheck
		{
			get
			{
				return this.libraryCheck;
			}
			set
			{
				this.libraryCheck = value;
				this.OnPropertyChanged("LibraryCheck");
			}
		}

		public bool NewLoader
		{
			get;
			set;
		}

		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = value;
				this.OnPropertyChanged("Password");
			}
		}

		[XmlArrayItem("Profiles", IsNullable=true)]
		public ObservableCollection<Profile> Profiles
		{
			get
			{
				return this.profiles;
			}
			set
			{
				this.profiles = value;
				this.OnPropertyChanged("Profiles");
			}
		}

		public string RandomName
		{
			get;
			set;
		}

		[JsonIgnore]
		[XmlIgnore]
		public string SearchText
		{
			get
			{
				return this.searchText;
			}
			set
			{
				this.searchText = value;
				this.OnPropertyChanged("SearchText");
			}
		}

		public string SelectedColor
		{
			get
			{
				return this.selectedColor;
			}
			set
			{
				this.selectedColor = value;
				this.OnPropertyChanged("SelectedColor");
			}
		}

		public string SelectedLanguage
		{
			get
			{
				return this.selectedLanguage;
			}
			set
			{
				this.selectedLanguage = value;
				this.OnPropertyChanged("SelectedLanguage");
			}
		}

		[JsonIgnore]
		[XmlIgnore]
		public Profile SelectedProfile
		{
			get
			{
				if (this.SelectedProfileId >= this.Profiles.Count)
				{
					return this.Profiles.FirstOrDefault<Profile>();
				}
				return this.Profiles[this.SelectedProfileId];
			}
			set
			{
				int index = this.Profiles.IndexOf(value);
				this.SelectedProfileId = (index < 0 ? 0 : index);
				this.OnPropertyChanged("SelectedProfile");
				this.OnPropertyChanged("SelectedProfileId");
			}
		}

		public int SelectedProfileId
		{
			get
			{
				return this.selectedProfileId;
			}
			set
			{
				this.selectedProfileId = value;
				this.OnPropertyChanged("SelectedProfileId");
				this.OnPropertyChanged("SelectedProfile");
			}
		}

		public ConfigSettings Settings
		{
			get
			{
				return this.settings;
			}
			set
			{
				this.settings = value;
				this.OnPropertyChanged("Settings");
			}
		}

		public bool ShowDevOptions
		{
			get
			{
				return this.showDevOptions;
			}
			set
			{
				this.showDevOptions = value;
				this.OnPropertyChanged("ShowDevOptions");
			}
		}

		public bool TosAccepted
		{
			get
			{
				return this.tosAccepted;
			}
			set
			{
				this.tosAccepted = value;
				this.OnPropertyChanged("TosAccepted");
			}
		}

		public bool UpdateOnLoad
		{
			get
			{
				return this.updateOnLoad;
			}
			set
			{
				this.updateOnLoad = value;
				this.OnPropertyChanged("UpdateOnLoad");
			}
		}

		public bool UseCloudConfig
		{
			get
			{
				return this.useCloudConfig;
			}
			set
			{
				this.useCloudConfig = value;
				this.OnPropertyChanged("UseCloudConfig");
			}
		}

		public string Username
		{
			get
			{
				return this.username;
			}
			set
			{
				this.username = value;
				this.OnPropertyChanged("Username");
			}
		}

		public bool UtilityCheck
		{
			get
			{
				return this.utilityCheck;
			}
			set
			{
				this.utilityCheck = value;
				this.OnPropertyChanged("UtilityCheck");
			}
		}

		public double WindowHeight
		{
			get
			{
				return this.windowHeight;
			}
			set
			{
				this.windowHeight = value;
				this.OnPropertyChanged("WindowHeight");
			}
		}

		public double WindowLeft
		{
			get
			{
				return this.windowLeft;
			}
			set
			{
				this.windowLeft = value;
				this.OnPropertyChanged("WindowLeft");
			}
		}

		public double WindowTop
		{
			get
			{
				return this.windowTop;
			}
			set
			{
				this.windowTop = value;
				this.OnPropertyChanged("WindowTop");
			}
		}

		public double WindowWidth
		{
			get
			{
				return this.windowWidth;
			}
			set
			{
				this.windowWidth = value;
				this.OnPropertyChanged("WindowWidth");
			}
		}

		public int Workers
		{
			get
			{
				return this.workers;
			}
			set
			{
				this.workers = value;
				this.OnPropertyChanged("Workers");
			}
		}

		public Config()
		{
		}

		private bool IsOnScreen()
		{
			Screen[] allScreens = Screen.AllScreens;
			for (int i = 0; i < (int)allScreens.Length; i++)
			{
				Screen screen = allScreens[i];
				System.Drawing.Point formTopLeft = new System.Drawing.Point((int)this.WindowLeft, (int)this.WindowTop);
				if (screen.WorkingArea.Contains(formTopLeft))
				{
					return true;
				}
			}
			return false;
		}

		public static void Load(bool isLoader = false)
		{
			if (App.Args.Length == 0 && !isLoader && Config.LoadFromCloud())
			{
				return;
			}
			if (Config.LoadFromFile())
			{
				return;
			}
			if (Config.LoadFromBackup())
			{
				return;
			}
			if (Config.LoadFromResource())
			{
				return;
			}
			System.Windows.MessageBox.Show("Something went horribly wrong while loading your Configuration /ff");
			Environment.Exit(0);
		}

		private static bool LoadFromBackup()
		{
			bool flag;
			try
			{
				if (File.Exists(string.Format("{0}.bak", Directories.ConfigFilePath)))
				{
					Config.Instance = (Config)Utility.MapXmlFileToClass(typeof(Config), string.Format("{0}.bak", Directories.ConfigFilePath));
					Config.Save(false);
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			catch
			{
				File.Delete(string.Format("{0}.bak", Directories.ConfigFilePath));
				return false;
			}
			return flag;
		}

		private static bool LoadFromCloud()
		{
			bool flag;
			string username;
			string password;
			try
			{
				try
				{
					if (File.Exists(Directories.ConfigFilePath))
					{
						Config.Instance = (Config)Utility.MapXmlFileToClass(typeof(Config), Directories.ConfigFilePath);
					}
				}
				catch
				{
				}
				if (Config.Instance.UseCloudConfig)
				{
					Config instance = Config.Instance;
					if (instance != null)
					{
						username = instance.Username;
					}
					else
					{
						username = null;
					}
					if (!string.IsNullOrEmpty(username))
					{
						Config instance1 = Config.Instance;
						if (instance1 != null)
						{
							password = instance1.Password;
						}
						else
						{
							password = null;
						}
						if (!string.IsNullOrEmpty(password))
						{
							if (WebService.Client.Login(Config.Instance.Username, Config.Instance.Password, false))
							{
								string configContent = WebService.Client.Cloud("Config");
								if (!string.IsNullOrEmpty(configContent))
								{
									Config config = JsonConvert.DeserializeObject<Config>(configContent);
									if (config != null)
									{
										config.RandomName = Config.Instance.RandomName;
										config.Username = Config.Instance.Username;
										config.Password = Config.Instance.Password;
										config.AuthKey = WebService.Client.get_LoginData().get_Token();
										Config.Instance = config;
										Config.Save(false);
										flag = true;
										return flag;
									}
									else
									{
										flag = false;
										return flag;
									}
								}
								else
								{
									flag = false;
									return flag;
								}
							}
							else
							{
								Config.Instance.Username = string.Empty;
								Config.Instance.Password = string.Empty;
								flag = false;
								return flag;
							}
						}
					}
					flag = false;
				}
				else
				{
					flag = false;
				}
			}
			catch
			{
				return false;
			}
			return flag;
		}

		private static bool LoadFromFile()
		{
			bool flag;
			try
			{
				if (File.Exists(Directories.ConfigFilePath))
				{
					Config.Instance = (Config)Utility.MapXmlFileToClass(typeof(Config), Directories.ConfigFilePath);
					string backupFile = string.Format("{0}.bak", Directories.ConfigFilePath);
					if (File.Exists(backupFile))
					{
						File.Delete(backupFile);
					}
					File.Copy(Directories.ConfigFilePath, backupFile);
					File.SetAttributes(backupFile, FileAttributes.Hidden);
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			catch
			{
				return false;
			}
			return flag;
		}

		private static bool LoadFromResource()
		{
			bool flag;
			try
			{
				Utility.CreateFileFromResource(Directories.ConfigFilePath, "LeagueSharp.Loader.Resources.config.xml", false);
				Config.Instance = (Config)Utility.MapXmlFileToClass(typeof(Config), Directories.ConfigFilePath);
				Config.Save(false);
				flag = true;
			}
			catch
			{
				return false;
			}
			return flag;
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

		public static void Save(bool cloud = false)
		{
			try
			{
				if (!Config.Instance.IsOnScreen())
				{
					Config.Instance.WindowTop = 100;
					Config.Instance.WindowLeft = 100;
				}
				Utility.MapClassToXmlFile(typeof(Config), Config.Instance, Directories.ConfigFilePath);
				if (cloud && Config.Instance.UseCloudConfig && !string.IsNullOrEmpty(Config.Instance.Username) && !string.IsNullOrEmpty(Config.Instance.Password) && WebService.Client.get_IsAuthenticated())
				{
					WebService.Client.CloudStore(Config.Instance, "Config");
				}
			}
			catch (Exception exception)
			{
				System.Windows.MessageBox.Show(exception.ToString());
			}
		}

		public static void SaveAndRestart(bool cloud = false)
		{
			Config.Instance.FirstRun = false;
			Config.Save(cloud);
			Process.Start(new ProcessStartInfo()
			{
				Arguments = string.Concat("/C choice /C Y /N /D Y /T 1 & ", Path.Combine(Directories.CurrentDirectory, "loader.exe")),
				WindowStyle = ProcessWindowStyle.Hidden,
				CreateNoWindow = true,
				FileName = "cmd.exe"
			});
			Environment.Exit(0);
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}