using Hardcodet.Wpf.TaskbarNotification;
using LeagueSharp.Loader.Class;
using LeagueSharp.Loader.Data;
using LeagueSharp.Loader.Views;
using MahApps.Metro;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LeagueSharp.Loader
{
	public partial class App : Application
	{
		private bool createdNew;

		private Mutex mutex;

		public static string[] Args
		{
			get;
			set;
		}

		public App()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(App.DomainOnUnhandledException);
			Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App.OnDispatcherUnhandledException);
		}

		private void AppDataRandomization()
		{
			try
			{
				if (!Directory.Exists(Directories.AppDataDirectory))
				{
					Directory.CreateDirectory(Directories.AppDataDirectory);
					string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
					int hashCode = Environment.UserName.GetHashCode();
					string oldPath = Path.Combine(folderPath, string.Concat("LeagueSharp", hashCode.ToString("X")));
					string oldPath2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LeagueSharp");
					if (Directory.Exists(oldPath))
					{
						Utility.CopyDirectory(oldPath, Directories.AppDataDirectory, true, true);
						Utility.ClearDirectory(oldPath);
						Directory.Delete(oldPath, true);
					}
					if (Directory.Exists(oldPath2))
					{
						Utility.CopyDirectory(oldPath2, Directories.AppDataDirectory, true, true);
						Utility.ClearDirectory(oldPath2);
						Directory.Delete(oldPath2, true);
					}
				}
			}
			catch
			{
			}
		}

		private void ConfigInit()
		{
			Config.Load(Assembly.GetExecutingAssembly().Location.EndsWith("loader.exe", StringComparison.OrdinalIgnoreCase));
			if (Config.Instance.Settings.GameSettings.All<GameSettings>((GameSettings x) => x.Name != "Show Drawings"))
			{
				Config.Instance.Settings.GameSettings.Add(new GameSettings()
				{
					Name = "Show Drawings",
					PosibleValues = new List<string>()
					{
						"True",
						"False"
					},
					SelectedValue = "True"
				});
			}
			if (Config.Instance.Settings.GameSettings.All<GameSettings>((GameSettings x) => x.Name != "Show Ping"))
			{
				Config.Instance.Settings.GameSettings.Add(new GameSettings()
				{
					Name = "Show Ping",
					PosibleValues = new List<string>()
					{
						"True",
						"False"
					},
					SelectedValue = "True"
				});
			}
			if (Config.Instance.Settings.GameSettings.All<GameSettings>((GameSettings x) => x.Name != "Send Anonymous Assembly Statistics"))
			{
				Config.Instance.Settings.GameSettings.Add(new GameSettings()
				{
					Name = "Send Anonymous Assembly Statistics",
					PosibleValues = new List<string>()
					{
						"True",
						"False"
					},
					SelectedValue = "True"
				});
			}
			if (Config.Instance.Settings.GameSettings.All<GameSettings>((GameSettings x) => x.Name != "Always Inject Default Profile"))
			{
				Config.Instance.Settings.GameSettings.Add(new GameSettings()
				{
					Name = "Always Inject Default Profile",
					PosibleValues = new List<string>()
					{
						"True",
						"False"
					},
					SelectedValue = "False"
				});
			}
		}

		private static void DomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			try
			{
				File.AppendAllText("ERROR.log", args.ExceptionObject.ToString());
			}
			catch
			{
			}
		}

		private void ExecutableRandomization()
		{
			if (Assembly.GetExecutingAssembly().Location.EndsWith("loader.exe", StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					if (Config.Instance.RandomName != null)
					{
						try
						{
							if (File.Exists(Directories.AssemblyFile))
							{
								File.SetAttributes(Directories.AssemblyFile, FileAttributes.Normal);
								File.Delete(Directories.AssemblyFile);
							}
							if (File.Exists(Directories.AssemblyPdbFile))
							{
								File.SetAttributes(Directories.AssemblyPdbFile, FileAttributes.Normal);
								File.Delete(Directories.AssemblyPdbFile);
							}
							if (File.Exists(Directories.AssemblyConfigFile))
							{
								File.SetAttributes(Directories.AssemblyConfigFile, FileAttributes.Normal);
								File.Delete(Directories.AssemblyConfigFile);
							}
						}
						catch
						{
						}
						if (!this.createdNew)
						{
							if (App.Args.Length != 0)
							{
								Process loader = Process.GetProcessesByName(Config.Instance.RandomName).FirstOrDefault<Process>();
								if (loader != null && loader.MainWindowHandle != IntPtr.Zero)
								{
									Clipboard.SetText(App.Args[0]);
									App.ShowWindow(loader.MainWindowHandle, 5);
									App.SetForegroundWindow(loader.MainWindowHandle);
								}
							}
							this.mutex = null;
							Environment.Exit(0);
						}
					}
					try
					{
						Config.Instance.RandomName = Utility.GetUniqueKey(6);
						Config.Save(false);
						File.Copy(Path.Combine(Directories.CurrentDirectory, "loader.exe"), Directories.AssemblyFile);
						File.Copy(Path.Combine(Directories.CurrentDirectory, "loader.pdb"), Directories.AssemblyPdbFile);
						File.Copy(Path.Combine(Directories.CurrentDirectory, "loader.exe.config"), Directories.AssemblyConfigFile);
						Eudyptula.Feelsbadman(Directories.AssemblyFile);
						Process.Start(Directories.AssemblyFile);
					}
					catch (Exception exception)
					{
						Console.WriteLine(exception);
					}
					Environment.Exit(0);
				}
				catch (Exception exception1)
				{
				}
			}
			AppDomain.CurrentDomain.ProcessExit += new EventHandler((object sender, EventArgs args) => {
				try
				{
					Eudyptula.Stop();
					Utility.ClearDirectory(Directories.AssembliesDir);
					Utility.ClearDirectory(Directories.LogsDir);
					LeagueSharp.Loader.Views.MainWindow instance = LeagueSharp.Loader.Views.MainWindow.Instance;
					if (instance != null)
					{
						TaskbarIcon trayIcon = instance.TrayIcon;
						if (trayIcon != null)
						{
							trayIcon.Dispose();
						}
						else
						{
						}
					}
					else
					{
					}
					if (this.mutex != null && this.createdNew)
					{
						this.mutex.ReleaseMutex();
					}
				}
				catch
				{
				}
				if (!Assembly.GetExecutingAssembly().Location.EndsWith("loader.exe"))
				{
					Process.Start(new ProcessStartInfo()
					{
						Arguments = string.Concat(new string[] { "/C choice /C Y /N /D Y /T 1 & Del \"", Directories.AssemblyFile, "\" \"", Directories.AssemblyConfigFile, "\" \"", Directories.AssemblyPdbFile, "\"" }),
						WindowStyle = ProcessWindowStyle.Hidden,
						CreateNoWindow = true,
						FileName = "cmd.exe"
					});
				}
			});
		}

		private void Localize()
		{
			ResourceDictionary dict = new ResourceDictionary();
			if (Config.Instance.SelectedLanguage == null)
			{
				string lid = (Thread.CurrentThread.CurrentCulture.ToString().Contains("-") ? Thread.CurrentThread.CurrentCulture.ToString().Split(new char[] { '-' })[0].ToUpperInvariant() : Thread.CurrentThread.CurrentCulture.ToString().ToUpperInvariant());
				switch (lid)
				{
					case "DE":
					{
						dict.Source = new Uri("..\\Resources\\Language\\German.xaml", UriKind.Relative);
						break;
					}
					case "AR":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Arabic.xaml", UriKind.Relative);
						break;
					}
					case "ES":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Spanish.xaml", UriKind.Relative);
						break;
					}
					case "FR":
					{
						dict.Source = new Uri("..\\Resources\\Language\\French.xaml", UriKind.Relative);
						break;
					}
					case "IT":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Italian.xaml", UriKind.Relative);
						break;
					}
					case "KO":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Korean.xaml", UriKind.Relative);
						break;
					}
					case "NL":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Dutch.xaml", UriKind.Relative);
						break;
					}
					case "PL":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Polish.xaml", UriKind.Relative);
						break;
					}
					case "PT":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Portuguese.xaml", UriKind.Relative);
						break;
					}
					case "RO":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Romanian.xaml", UriKind.Relative);
						break;
					}
					case "RU":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Russian.xaml", UriKind.Relative);
						break;
					}
					case "SE":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Swedish.xaml", UriKind.Relative);
						break;
					}
					case "TR":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Turkish.xaml", UriKind.Relative);
						break;
					}
					case "VI":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Vietnamese.xaml", UriKind.Relative);
						break;
					}
					case "ZH":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Chinese.xaml", UriKind.Relative);
						break;
					}
					case "LT":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Lithuanian.xaml", UriKind.Relative);
						break;
					}
					case "CZ":
					{
						dict.Source = new Uri("..\\Resources\\Language\\Czech.xaml", UriKind.Relative);
						break;
					}
					default:
					{
						dict.Source = new Uri("..\\Resources\\Language\\English.xaml", UriKind.Relative);
						break;
					}
				}
			}
			else
			{
				dict.Source = new Uri(string.Concat("..\\Resources\\Language\\", Config.Instance.SelectedLanguage, ".xaml"), UriKind.Relative);
			}
			base.Resources.MergedDictionaries.Add(dict);
			CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
			CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		}

		private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
		{
			try
			{
				File.AppendAllText("ERROR.log", args.Exception.ToString());
			}
			catch
			{
			}
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			if (File.Exists(Updater.SetupFile))
			{
				Thread.Sleep(1000);
			}
			this.mutex = new Mutex(true, Utility.Md5Hash(Environment.UserName), out this.createdNew);
			App.Args = e.Args;
			try
			{
				if (string.Compare(Process.GetCurrentProcess().ProcessName, "LeagueSharp.Loader.exe", StringComparison.InvariantCultureIgnoreCase) != 0 && File.Exists(Path.Combine(Directories.CurrentDirectory, "LeagueSharp.Loader.exe")))
				{
					File.Delete(Path.Combine(Directories.CurrentDirectory, "LeagueSharp.Loader.exe"));
					File.Delete(Path.Combine(Directories.CurrentDirectory, "LeagueSharp.Loader.exe.config"));
				}
			}
			catch
			{
			}
			this.ConfigInit();
			this.AppDataRandomization();
			this.ExecutableRandomization();
			this.Localize();
			if (Config.Instance.SelectedColor != null)
			{
				ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(Config.Instance.SelectedColor), ThemeManager.GetAppTheme("BaseLight"));
			}
			await Auth.Login(Config.Instance.Username, Config.Instance.Password);
			this.<>n__0(e);
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
	}
}