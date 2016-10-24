using LeagueSharp.Loader.Class;
using LeagueSharp.Loader.Class.Installer;
using LeagueSharp.Loader.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PlaySharp.Service.WebService.Model;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace LeagueSharp.Loader.Views
{
	public class InstallerWindow : MetroWindow, INotifyPropertyChanged, IComponentConnector
	{
		private bool _ableToList = true;

		private List<LeagueSharpAssembly> _foundAssemblies = new List<LeagueSharpAssembly>();

		internal TabControl installTabControl;

		internal RadioButton LocalRadioButton;

		internal TextBox PathTextBox;

		internal RadioButton SvnRadioButton;

		internal ComboBox SvnComboBox;

		internal RadioButton InstalledRadioButton;

		internal Button Step1;

		internal Button Step2;

		internal Button Step2P;

		internal DataGrid MainDataGrid;

		internal TextBox search;

		private bool _contentLoaded;

		public bool AbleToList
		{
			get
			{
				return this._ableToList;
			}
			set
			{
				this._ableToList = value;
				this.OnPropertyChanged("AbleToList");
			}
		}

		private ProgressDialogController controller
		{
			get;
			set;
		}

		public List<LeagueSharpAssembly> FoundAssemblies
		{
			get
			{
				return this._foundAssemblies;
			}
			set
			{
				this._foundAssemblies = value;
				this.OnPropertyChanged("FoundAssemblies");
			}
		}

		public InstallerWindow()
		{
			this.InitializeComponent();
			base.DataContext = this;
		}

		private async void AfterInstallMessage(string msg, bool close = false)
		{
			if (!close)
			{
				await DialogManager.ShowMessageAsync(this, Utility.GetMultiLanguageText("Installer"), msg, 0, null);
			}
			else
			{
				Config.Save(false);
				try
				{
					if (this.IsVisible)
					{
						this.Close();
					}
				}
				catch
				{
				}
			}
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Application.LoadComponent(this, new Uri("/loader;component/views/installerwindow.xaml", UriKind.Relative));
		}

		public static async Task InstallAssembly(AssemblyEntry assembly, bool silent)
		{
			InstallerWindow.<InstallAssembly>d__16 variable = new InstallerWindow.<InstallAssembly>d__16();
			variable.assembly = assembly;
			variable.silent = silent;
			variable.<>t__builder = AsyncTaskMethodBuilder.Create();
			variable.<>1__state = -1;
			variable.<>t__builder.Start<InstallerWindow.<InstallAssembly>d__16>(ref variable);
			return variable.<>t__builder.Task;
		}

		public static void InstallAssembly(Match m)
		{
			string str;
			string gitHubUser = m.Groups[2].ToString();
			string repositoryName = m.Groups[3].ToString();
			string assemblyName = m.Groups[4].ToString();
			InstallerWindow installerWindow = new InstallerWindow();
			((Window)installerWindow).Owner = MainWindow.Instance;
			InstallerWindow w = installerWindow;
			InstallerWindow installerWindow1 = w;
			string str1 = string.Format("https://github.com/{0}/{1}", gitHubUser, repositoryName);
			if (assemblyName != string.Empty)
			{
				str = m.Groups[4].ToString();
			}
			else
			{
				str = null;
			}
			installerWindow1.ShowProgress(str1, true, str);
			w.ShowDialog();
		}

		private void InstallerWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			this.SvnComboBox.ItemsSource = Config.Instance.KnownRepositories;
		}

		public async Task InstallSelected(bool silent)
		{
			InstallerWindow.<InstallSelected>d__18 variable = new InstallerWindow.<InstallSelected>d__18();
			variable.<>4__this = this;
			variable.silent = silent;
			variable.<>t__builder = AsyncTaskMethodBuilder.Create();
			variable.<>1__state = -1;
			variable.<>t__builder.Start<InstallerWindow.<InstallSelected>d__18>(ref variable);
			return variable.<>t__builder.Task;
		}

		public async Task ListAssemblies(string location, bool isSvn, bool silent, string autoInstallName = null)
		{
			Action action1 = null;
			this.AbleToList = false;
			if (isSvn)
			{
				await Task.Factory.StartNew(() => {
					string str = location;
					ObservableCollection<RepositoryEntry> blockedRepositories = Config.Instance.BlockedRepositories;
					Func<RepositoryEntry, bool> u003cu003e9_191 = InstallerWindow.<>c.<>9__19_1;
					if (u003cu003e9_191 == null)
					{
						u003cu003e9_191 = (RepositoryEntry r) => r.get_HasRedirect();
						InstallerWindow.<>c.<>9__19_1 = u003cu003e9_191;
					}
					RepositoryEntry redirect = blockedRepositories.Where<RepositoryEntry>(u003cu003e9_191).FirstOrDefault<RepositoryEntry>((RepositoryEntry r) => str.StartsWith(r.get_Url()));
					if (redirect != null)
					{
						location = redirect.get_Redirect();
					}
					string updatedDir = GitUpdater.Update(location);
					this.FoundAssemblies = LeagueSharpAssemblies.GetAssemblies(updatedDir, location);
					LeagueSharpAssembly[] array = this.FoundAssemblies.ToArray();
					for (int i = 0; i < (int)array.Length; i++)
					{
						LeagueSharpAssembly leagueSharpAssembly = array[i];
						Config.Instance.SelectedProfile.InstalledAssemblies.Where<LeagueSharpAssembly>((LeagueSharpAssembly y) => {
							if (y.Name != leagueSharpAssembly.Name)
							{
								return false;
							}
							return y.SvnUrl == leagueSharpAssembly.SvnUrl;
						}).ToList<LeagueSharpAssembly>().ForEach((LeagueSharpAssembly a) => this.FoundAssemblies.Remove(a));
					}
					if (autoInstallName != null)
					{
						foreach (LeagueSharpAssembly assembly in this.FoundAssemblies)
						{
							if (assembly.Name.ToLower() != autoInstallName.ToLower())
							{
								continue;
							}
							assembly.InstallChecked = true;
							Dispatcher dispatcher = Application.Current.Dispatcher;
							Action u003cu003e9_5 = action1;
							if (u003cu003e9_5 == null)
							{
								Action text = () => this.search.Text = autoInstallName;
								Action action = text;
								action1 = text;
								u003cu003e9_5 = action;
							}
							dispatcher.Invoke(u003cu003e9_5);
						}
					}
				});
			}
			else
			{
				this.FoundAssemblies = LeagueSharpAssemblies.GetAssemblies(location, "");
			}
			this.AbleToList = true;
			TabControl selectedIndex = this.installTabControl;
			selectedIndex.SelectedIndex = selectedIndex.SelectedIndex + 1;
			if (autoInstallName != null)
			{
				await this.InstallSelected(silent);
			}
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
			if (propertyChangedEventHandler == null)
			{
				return;
			}
			propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
		}

		private void PathTextBox_OnGotFocus(object sender, RoutedEventArgs e)
		{
			bool? nullable;
			this.SvnRadioButton.IsChecked = new bool?(false);
			RadioButton localRadioButton = this.LocalRadioButton;
			bool? isChecked = this.SvnRadioButton.IsChecked;
			if (isChecked.HasValue)
			{
				nullable = new bool?(!isChecked.GetValueOrDefault());
			}
			else
			{
				nullable = null;
			}
			localRadioButton.IsChecked = nullable;
		}

		private void PathTextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (textBox != null && string.IsNullOrWhiteSpace(textBox.SelectedText))
			{
				FolderSelectDialog folderDialog = new FolderSelectDialog()
				{
					Title = "Select project folder"
				};
				if (folderDialog.ShowDialog())
				{
					textBox.Text = folderDialog.FileName;
					this.LocalRadioButton.IsChecked = new bool?(true);
				}
			}
		}

		private void SelectAllButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (LeagueSharpAssembly assembly in this.FoundAssemblies)
			{
				assembly.InstallChecked = true;
			}
			this.OnPropertyChanged("FoundAssemblies");
		}

		public async void ShowProgress(string location, bool isSvn, string autoInstallName = null)
		{
			while (!this.IsInitialized || !this.IsVisible)
			{
				await Task.Delay(100);
			}
			try
			{
				ProgressDialogController progressDialogController = await DialogManager.ShowProgressAsync(this, Utility.GetMultiLanguageText("Updating"), Utility.GetMultiLanguageText("DownloadingData"), false, null);
				this.controller = progressDialogController;
				this.controller.SetIndeterminate();
				this.controller.SetCancelable(true);
			}
			catch
			{
			}
			await this.ListAssemblies(location, isSvn, false, autoInstallName);
			try
			{
				await this.controller.CloseAsync();
			}
			catch
			{
			}
		}

		private void Step1_Click(object sender, RoutedEventArgs e)
		{
			string str;
			bool? isChecked = this.InstalledRadioButton.IsChecked;
			if ((isChecked.GetValueOrDefault() ? isChecked.HasValue : false))
			{
				this.FoundAssemblies.Clear();
				foreach (Profile profile in Config.Instance.Profiles)
				{
					foreach (LeagueSharpAssembly assembly in profile.InstalledAssemblies)
					{
						this.FoundAssemblies.Add(assembly.Copy());
					}
				}
				this.FoundAssemblies = this.FoundAssemblies.Distinct<LeagueSharpAssembly>().ToList<LeagueSharpAssembly>();
				TabControl selectedIndex = this.installTabControl;
				selectedIndex.SelectedIndex = selectedIndex.SelectedIndex + 1;
				return;
			}
			isChecked = this.SvnRadioButton.IsChecked;
			str = ((isChecked.GetValueOrDefault() ? isChecked.HasValue : false) ? this.SvnComboBox.Text : this.PathTextBox.Text);
			isChecked = this.SvnRadioButton.IsChecked;
			this.ShowProgress(str, (isChecked.GetValueOrDefault() ? isChecked.HasValue : false), null);
		}

		private async void Step2_Click(object sender, RoutedEventArgs e)
		{
			await this.InstallSelected(false);
		}

		private void Step2P_Click(object sender, RoutedEventArgs e)
		{
			TabControl selectedIndex = this.installTabControl;
			selectedIndex.SelectedIndex = selectedIndex.SelectedIndex - 1;
		}

		private void SvnComboBox_OnGotFocus(object sender, RoutedEventArgs e)
		{
			bool? nullable;
			this.SvnRadioButton.IsChecked = new bool?(true);
			RadioButton localRadioButton = this.LocalRadioButton;
			bool? isChecked = this.SvnRadioButton.IsChecked;
			if (isChecked.HasValue)
			{
				nullable = new bool?(!isChecked.GetValueOrDefault());
			}
			else
			{
				nullable = null;
			}
			localRadioButton.IsChecked = nullable;
		}

		[DebuggerNonUserCode]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
				case 1:
				{
					((InstallerWindow)target).Loaded += new RoutedEventHandler(this.InstallerWindow_OnLoaded);
					return;
				}
				case 2:
				{
					this.installTabControl = (TabControl)target;
					return;
				}
				case 3:
				{
					this.LocalRadioButton = (RadioButton)target;
					return;
				}
				case 4:
				{
					this.PathTextBox = (TextBox)target;
					this.PathTextBox.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.PathTextBox_PreviewMouseLeftButtonDown);
					this.PathTextBox.GotFocus += new RoutedEventHandler(this.PathTextBox_OnGotFocus);
					return;
				}
				case 5:
				{
					this.SvnRadioButton = (RadioButton)target;
					return;
				}
				case 6:
				{
					this.SvnComboBox = (ComboBox)target;
					this.SvnComboBox.GotFocus += new RoutedEventHandler(this.SvnComboBox_OnGotFocus);
					return;
				}
				case 7:
				{
					this.InstalledRadioButton = (RadioButton)target;
					return;
				}
				case 8:
				{
					this.Step1 = (Button)target;
					this.Step1.Click += new RoutedEventHandler(this.Step1_Click);
					return;
				}
				case 9:
				{
					this.Step2 = (Button)target;
					this.Step2.Click += new RoutedEventHandler(this.Step2_Click);
					return;
				}
				case 10:
				{
					this.Step2P = (Button)target;
					this.Step2P.Click += new RoutedEventHandler(this.Step2P_Click);
					return;
				}
				case 11:
				{
					this.MainDataGrid = (DataGrid)target;
					return;
				}
				case 12:
				{
					((Button)target).Click += new RoutedEventHandler(this.SelectAllButton_Click);
					return;
				}
				case 13:
				{
					((Button)target).Click += new RoutedEventHandler(this.UnselectAllButton_Click);
					return;
				}
				case 14:
				{
					this.search = (TextBox)target;
					this.search.TextChanged += new TextChangedEventHandler(this.TextBoxBase_OnTextChanged);
					return;
				}
			}
			this._contentLoaded = true;
		}

		private void TextBoxBase_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			string text = this.search.Text;
			ICollectionView view = CollectionViewSource.GetDefaultView(this.FoundAssemblies);
			text = text.Replace("*", "(.*)");
			view.Filter = (object obj) => {
				bool success;
				try
				{
					success = Regex.Match((obj as LeagueSharpAssembly).Name, text, RegexOptions.IgnoreCase).Success;
				}
				catch (Exception exception)
				{
					success = true;
				}
				return success;
			};
		}

		private void UnselectAllButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (LeagueSharpAssembly assembly in this.FoundAssemblies)
			{
				assembly.InstallChecked = false;
			}
			this.OnPropertyChanged("FoundAssemblies");
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}