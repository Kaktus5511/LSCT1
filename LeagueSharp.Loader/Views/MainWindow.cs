using Hardcodet.Wpf.TaskbarNotification;
using LeagueSharp.Loader.Class;
using LeagueSharp.Loader.Class.Installer;
using LeagueSharp.Loader.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Build.Evaluation;
using PlaySharp.Service.WebService;
using PlaySharp.Service.WebService.Endpoints;
using PlaySharp.Service.WebService.Model;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LeagueSharp.Loader.Views
{
	public class MainWindow : MetroWindow, INotifyPropertyChanged, IComponentConnector
	{
		public const int TAB_ASSEMBLIES = 2;

		public const int TAB_DATABASE = 4;

		public const int TAB_NEWS = 1;

		public const int TAB_SETTINGS = 3;

		public const int TAB_TOS = 0;

		public readonly BackgroundWorker AssembliesWorker = new BackgroundWorker();

		public bool AssembliesWorkerCancelled;

		private bool checkingForUpdates;

		private bool columnWidthChanging;

		private int rowIndex = -1;

		private string statusString = "?";

		private string updateMessage;

		private bool working;

		internal Rectangle icon_connected;

		internal Rectangle icon_disconnected;

		internal Button NewsButton;

		internal Button AssemblyButton;

		internal Button AssemblyDBButton;

		internal Button StatusButton;

		internal Button SettingsButton;

		internal TabControl MainTabControl;

		internal TabItem TosTabItem;

		internal WebBrowser TosBrowser;

		internal TabItem NewsTabItem;

		internal WebBrowser Browser;

		internal TabItem AssembliesTabItem;

		internal DataGrid InstalledAssembliesDataGrid;

		internal MenuItem UpdateAndCompileMenuItem;

		internal MenuItem RemoveMenuItem;

		internal MenuItem GithubItem;

		internal MenuItem GithubAssembliesItem;

		internal MenuItem ShareItem;

		internal MenuItem DevMenu;

		internal MenuItem NewItem;

		internal MenuItem EditItem;

		internal MenuItem CloneItem;

		internal MenuItem LogItem;

		internal DataGridCheckBoxColumn ColumnCheck;

		internal DataGridTextColumn ColumnName;

		internal DataGridTextColumn ColumnType;

		internal DataGridTextColumn ColumnVersion;

		internal DataGridTextColumn ColumnLocation;

		internal DataGridTextColumn ColumnDescription;

		internal Button InstallButton;

		internal Button UpgradeButton;

		internal TabItem SettingsTabItem;

		internal ContentControl SettingsFrame;

		internal TreeViewItem GeneralSettingsItem;

		internal TabItem AssemblyDB;

		internal DataGrid AssembliesDBDataGrid;

		internal MenuItem InstallFromDbItem;

		internal DataGridTextColumn DBColumnName;

		internal DataGridTextColumn DBColumnAuthor;

		internal DataGridTextColumn DBColumnDescription;

		internal TextBlock Header;

		internal TaskbarIcon TrayIcon;

		internal MenuItem MenuItemLabelHide;

		private bool _contentLoaded;

		public string BaseUrl
		{
			get
			{
				return "https://services.joduska.me/api/v2.0";
			}
		}

		public bool CheckingForUpdates
		{
			get
			{
				return this.checkingForUpdates;
			}
			set
			{
				this.checkingForUpdates = value;
				this.OnPropertyChanged("CheckingForUpdates");
			}
		}

		public LeagueSharp.Loader.Data.Config Config
		{
			get
			{
				return LeagueSharp.Loader.Data.Config.Instance;
			}
		}

		public Thread InjectThread
		{
			get;
			set;
		}

		public static MainWindow Instance
		{
			get;
			private set;
		}

		private DateTime LastAccountUpdate
		{
			get;
			set;
		}

		public string StatusString
		{
			get
			{
				return this.statusString;
			}
			set
			{
				this.statusString = value;
				this.OnPropertyChanged("StatusString");
			}
		}

		public bool Working
		{
			get
			{
				return this.working;
			}
			set
			{
				this.working = value;
				this.OnPropertyChanged("Working");
			}
		}

		public MainWindow()
		{
			MainWindow.Instance = this;
			this.InitializeComponent();
			base.DataContext = this;
		}

		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		private void AssemblyButton_OnClick(object sender, RoutedEventArgs e)
		{
			this.Browser.Visibility = Visibility.Hidden;
			this.TosBrowser.Visibility = Visibility.Hidden;
			this.MainTabControl.SelectedIndex = 2;
			this.UpdateFilters();
		}

		private async void AssemblyDBButton_OnClick(object sender, RoutedEventArgs e)
		{
			MainWindow.<AssemblyDBButton_OnClick>d__48 variable = new MainWindow.<AssemblyDBButton_OnClick>d__48();
			variable.<>4__this = this;
			variable.<>t__builder = AsyncVoidMethodBuilder.Create();
			variable.<>1__state = -1;
			variable.<>t__builder.Start<MainWindow.<AssemblyDBButton_OnClick>d__48>(ref variable);
		}

		private void BaseDataGrid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.columnWidthChanging)
			{
				this.columnWidthChanging = false;
				LeagueSharp.Loader.Data.Config instance = LeagueSharp.Loader.Data.Config.Instance;
				DataGridLength width = this.ColumnCheck.Width;
				instance.ColumnCheckWidth = width.DesiredValue;
				LeagueSharp.Loader.Data.Config desiredValue = LeagueSharp.Loader.Data.Config.Instance;
				width = this.ColumnName.Width;
				desiredValue.ColumnNameWidth = width.DesiredValue;
				LeagueSharp.Loader.Data.Config config = LeagueSharp.Loader.Data.Config.Instance;
				width = this.ColumnType.Width;
				config.ColumnTypeWidth = width.DesiredValue;
				LeagueSharp.Loader.Data.Config instance1 = LeagueSharp.Loader.Data.Config.Instance;
				width = this.ColumnVersion.Width;
				instance1.ColumnVersionWidth = width.DesiredValue;
				LeagueSharp.Loader.Data.Config desiredValue1 = LeagueSharp.Loader.Data.Config.Instance;
				width = this.ColumnLocation.Width;
				desiredValue1.ColumnLocationWidth = width.DesiredValue;
			}
		}

		private async Task Bootstrap()
		{
			Visibility visibility;
			try
			{
				SplashScreen splashScreen = new SplashScreen("resources/splash.png");
				this.Visibility = Visibility.Hidden;
				splashScreen.Show(false, true);
				this.Browser.Visibility = Visibility.Hidden;
				this.TosBrowser.Visibility = Visibility.Hidden;
				this.GeneralSettingsItem.IsSelected = true;
				PropertyDescriptor propertyDescriptor = DependencyPropertyDescriptor.FromProperty(DataGridColumn.ActualWidthProperty, typeof(DataGridColumn));
				foreach (DataGridColumn dataGridColumn in this.InstalledAssembliesDataGrid.Columns)
				{
					propertyDescriptor.AddValueChanged(dataGridColumn, new EventHandler(this.ColumnWidthPropertyChanged));
				}
				this.ColumnCheck.Width = LeagueSharp.Loader.Data.Config.Instance.ColumnCheckWidth;
				this.ColumnName.Width = LeagueSharp.Loader.Data.Config.Instance.ColumnNameWidth;
				this.ColumnType.Width = LeagueSharp.Loader.Data.Config.Instance.ColumnTypeWidth;
				this.ColumnVersion.Width = LeagueSharp.Loader.Data.Config.Instance.ColumnVersionWidth;
				this.ColumnLocation.Width = LeagueSharp.Loader.Data.Config.Instance.ColumnLocationWidth;
				this.NewsTabItem.Visibility = Visibility.Hidden;
				this.AssembliesTabItem.Visibility = Visibility.Hidden;
				this.SettingsTabItem.Visibility = Visibility.Hidden;
				this.AssemblyDB.Visibility = Visibility.Hidden;
				MenuItem devMenu = this.DevMenu;
				visibility = (LeagueSharp.Loader.Data.Config.Instance.ShowDevOptions ? Visibility.Visible : Visibility.Collapsed);
				devMenu.Visibility = visibility;
				this.Config.PropertyChanged += new PropertyChangedEventHandler((object o, PropertyChangedEventArgs args) => {
					if (args.PropertyName == "ShowDevOptions")
					{
						this.DevMenu.Visibility = (LeagueSharp.Loader.Data.Config.Instance.ShowDevOptions ? Visibility.Visible : Visibility.Collapsed);
					}
				});
				await this.CheckForUpdates(true, true, false);
				if (LeagueSharp.Loader.Data.Config.Instance.TosAccepted)
				{
					this.AssemblyButton_OnClick(null, null);
				}
				else
				{
					Eudyptula.StartKill("Terms of Service not Accepted");
					splashScreen.Close(TimeSpan.FromMilliseconds(300));
					this.Visibility = Visibility.Visible;
					this.get_RightWindowCommands().Visibility = Visibility.Collapsed;
					this.TosButton_OnClick(null, null);
				}
				TaskFactory factory = Task.Factory;
				await factory.StartNew(() => {
					while (!LeagueSharp.Loader.Data.Config.Instance.TosAccepted)
					{
						Thread.Sleep(100);
					}
				});
				this.Config.PropertyChanged += new PropertyChangedEventHandler(this.ConfigOnSearchTextChanged);
				this.UpdateFilters();
				if (WebService.Client.get_IsAuthenticated())
				{
					this.OnLogin(LeagueSharp.Loader.Data.Config.Instance.Username);
				}
				else
				{
					Eudyptula.StartKill("Login required");
					splashScreen.Close(TimeSpan.FromMilliseconds(300));
					this.Browser.Visibility = Visibility.Hidden;
					this.TosBrowser.Visibility = Visibility.Hidden;
					this.Visibility = Visibility.Visible;
					await this.ShowLoginDialog();
					this.NewsButton_OnClick(null, null);
				}
				if (LeagueSharp.Loader.Data.Config.Instance.FirstRun)
				{
					LeagueSharp.Loader.Data.Config.SaveAndRestart(false);
				}
				this.get_RightWindowCommands().Visibility = Visibility.Visible;
				splashScreen.Close(TimeSpan.FromMilliseconds(300));
				this.Visibility = Visibility.Visible;
				await Updater.UpdateRepositories();
				await Updater.UpdateWebService();
				await this.UpdateAccount();
				Utility.Log(LogStatus.Info, "Bootstrap", "Update Complete", Logs.MainLog);
				List<LeagueSharpAssembly> leagueSharpAssemblies = new List<LeagueSharpAssembly>();
				foreach (Profile profile in LeagueSharp.Loader.Data.Config.Instance.Profiles)
				{
					leagueSharpAssemblies.AddRange(profile.InstalledAssemblies);
				}
				leagueSharpAssemblies = leagueSharpAssemblies.Distinct<LeagueSharpAssembly>().ToList<LeagueSharpAssembly>();
				GitUpdater.ClearUnusedRepos(leagueSharpAssemblies);
				this.PrepareAssemblies(leagueSharpAssemblies, true, true);
				Utility.Log(LogStatus.Info, "Bootstrap", "Compile Complete", Logs.MainLog);
				this.InitSystem();
				Utility.Log(LogStatus.Info, "Bootstrap", "System Initialisation Complete", Logs.MainLog);
				this.MainTabControl.SelectedIndex = 2;
				splashScreen = null;
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.ToString(), "Bootstrap Error");
			}
		}

		private async Task CheckForUpdates(bool loader, bool core, bool showDialogOnFinish)
		{
			try
			{
				if (!this.CheckingForUpdates)
				{
					this.StatusString = Utility.GetMultiLanguageText("Checking");
					this.updateMessage = string.Empty;
					this.CheckingForUpdates = true;
					if (loader)
					{
						LoaderVersionData loaderVersionDatum = await WebService.Client.LoaderVersionAsync(new CancellationToken());
						try
						{
							if (File.Exists(Updater.SetupFile))
							{
								Thread.Sleep(1000);
								File.Delete(Updater.SetupFile);
							}
						}
						catch
						{
							MessageBox.Show(Utility.GetMultiLanguageText("FailedToDelete"));
							Environment.Exit(0);
						}
						if (loaderVersionDatum.get_Version() > Assembly.GetExecutingAssembly().GetName().Version && !Eudyptula.IsInjected)
						{
							Console.WriteLine("Update Loader");
							Updater.Updating = true;
							await Updater.UpdateLoader(loaderVersionDatum.get_Url());
						}
					}
					if (core)
					{
						if (LeagueSharp.Loader.Data.Config.Instance.LeagueOfLegendsExePath != null)
						{
							string latestLeagueOfLegendsExePath = Utility.GetLatestLeagueOfLegendsExePath(LeagueSharp.Loader.Data.Config.Instance.LeagueOfLegendsExePath);
							if (latestLeagueOfLegendsExePath != null)
							{
								Console.WriteLine("Update Core");
								Updater.UpdateResponse updateResponse = await Updater.UpdateCore(latestLeagueOfLegendsExePath, !showDialogOnFinish);
								this.updateMessage = updateResponse.Message;
								Updater.CoreUpdateState state = updateResponse.State;
								if (state == Updater.CoreUpdateState.Operational)
								{
									this.StatusString = Utility.GetMultiLanguageText("Updated");
								}
								else if (state == Updater.CoreUpdateState.Maintenance)
								{
									this.StatusString = Utility.GetMultiLanguageText("OUTDATED");
								}
								else
								{
									this.StatusString = Utility.GetMultiLanguageText("Unknown");
								}
								return;
							}
						}
						this.StatusString = Utility.GetMultiLanguageText("Unknown");
						this.updateMessage = Utility.GetMultiLanguageText("LeagueNotFound");
					}
				}
				else
				{
					return;
				}
			}
			finally
			{
				this.CheckingForUpdates = false;
				Updater.CheckedForUpdates = true;
				if (showDialogOnFinish)
				{
					this.ShowTextMessage(Utility.GetMultiLanguageText("UpdateStatus"), this.updateMessage);
				}
			}
		}

		private void CloneItem_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.InstalledAssembliesDataGrid.SelectedItems.Count <= 0)
			{
				return;
			}
			LeagueSharpAssembly selectedAssembly = (LeagueSharpAssembly)this.InstalledAssembliesDataGrid.SelectedItems[0];
			try
			{
				string source = System.IO.Path.GetDirectoryName(selectedAssembly.PathToProjectFile);
				string str = System.IO.Path.Combine(Directories.LocalRepoDir, selectedAssembly.Name);
				int hashCode = Environment.TickCount.GetHashCode();
				string destination = string.Concat(str, "_clone_", hashCode.ToString("X"));
				Utility.CopyDirectory(source, destination, false, false);
				LeagueSharpAssembly leagueSharpAssembly = new LeagueSharpAssembly(string.Concat(selectedAssembly.Name, "_clone"), System.IO.Path.Combine(destination, System.IO.Path.GetFileName(selectedAssembly.PathToProjectFile)), string.Empty);
				leagueSharpAssembly.Compile();
				this.Config.SelectedProfile.InstalledAssemblies.Add(leagueSharpAssembly);
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.ToString());
			}
		}

		private void ColumnWidthPropertyChanged(object sender, EventArgs e)
		{
			this.columnWidthChanging = true;
			if (sender != null)
			{
				Mouse.AddPreviewMouseUpHandler(this, new MouseButtonEventHandler(this.BaseDataGrid_MouseLeftButtonUp));
			}
		}

		private async void CompileAll_OnClick(object sender, RoutedEventArgs e)
		{
			if (!this.Working)
			{
				await this.PrepareAssemblies(LeagueSharp.Loader.Data.Config.Instance.SelectedProfile.InstalledAssemblies, false, true);
			}
		}

		private void ConfigOnSearchTextChanged(object sender, PropertyChangedEventArgs args)
		{
			if (!args.PropertyName.EndsWith("Check") && args.PropertyName != "SearchText")
			{
				return;
			}
			this.UpdateFilters();
		}

		private async void DeleteWithConfirmation(IEnumerable<LeagueSharpAssembly> asemblies)
		{
			MessageDialogResult messageDialogResult = await DialogManager.ShowMessageAsync(this, Utility.GetMultiLanguageText("Uninstall"), Utility.GetMultiLanguageText("UninstallConfirm"), 1, null);
			if (messageDialogResult != null)
			{
				foreach (LeagueSharpAssembly asembly in asemblies)
				{
					LeagueSharp.Loader.Data.Config.Instance.SelectedProfile.InstalledAssemblies.Remove(asembly);
					if (asembly.Type != 3)
					{
						continue;
					}
					try
					{
						if (File.Exists(asembly.PathToBinary))
						{
							File.Delete(asembly.PathToBinary);
						}
					}
					catch
					{
					}
				}
			}
		}

		private void EditItem_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.InstalledAssembliesDataGrid.SelectedItems.Count <= 0)
			{
				return;
			}
			LeagueSharpAssembly selectedAssembly = (LeagueSharpAssembly)this.InstalledAssembliesDataGrid.SelectedItems[0];
			if (File.Exists(selectedAssembly.PathToProjectFile))
			{
				Process.Start(selectedAssembly.PathToProjectFile);
			}
		}

		private bool FilterAssemblies(object item)
		{
			bool injectChecked;
			try
			{
				string searchText = this.Config.SearchText.Replace("*", "(.*)");
				LeagueSharpAssembly assembly = item as LeagueSharpAssembly;
				if (assembly == null)
				{
					injectChecked = false;
				}
				else if (searchText != "checked")
				{
					switch (assembly.Type)
					{
						case 1:
						{
							if (this.Config.ChampionCheck)
							{
								break;
							}
							injectChecked = false;
							return injectChecked;
						}
						case 2:
						{
							if (this.Config.UtilityCheck)
							{
								break;
							}
							injectChecked = false;
							return injectChecked;
						}
						case 3:
						{
							if (this.Config.LibraryCheck)
							{
								break;
							}
							injectChecked = false;
							return injectChecked;
						}
					}
					Match nameMatch = Regex.Match(assembly.Name, searchText, RegexOptions.IgnoreCase);
					Match displayNameMatch = Regex.Match(assembly.DisplayName, searchText, RegexOptions.IgnoreCase);
					Match svnNameMatch = Regex.Match(assembly.SvnUrl, searchText, RegexOptions.IgnoreCase);
					Match descNameMatch = Regex.Match(assembly.Description, searchText, RegexOptions.IgnoreCase);
					injectChecked = (displayNameMatch.Success || nameMatch.Success || svnNameMatch.Success ? true : descNameMatch.Success);
				}
				else
				{
					injectChecked = assembly.InjectChecked;
				}
			}
			catch (Exception exception)
			{
				injectChecked = true;
			}
			return injectChecked;
		}

		private bool FilterDatabaseAssemblies(object item)
		{
			bool flag;
			try
			{
				string searchText = this.Config.SearchText.Replace("*", "(.*)");
				AssemblyEntry assembly = item as AssemblyEntry;
				if (assembly != null)
				{
					switch (assembly.get_Type())
					{
						case 1:
						{
							if (this.Config.ChampionCheck)
							{
								break;
							}
							flag = false;
							return flag;
						}
						case 2:
						{
							if (this.Config.UtilityCheck)
							{
								break;
							}
							flag = false;
							return flag;
						}
						case 3:
						{
							if (this.Config.LibraryCheck)
							{
								break;
							}
							flag = false;
							return flag;
						}
					}
					Match nameMatch = Regex.Match(assembly.get_Name(), searchText, RegexOptions.IgnoreCase);
					bool champeMatch = (assembly.get_Type() != 1 ? false : Regex.Match(string.Join(", ", assembly.get_Champions()), searchText, RegexOptions.IgnoreCase).Success);
					Match authorMatch = Regex.Match(assembly.get_AuthorName(), searchText, RegexOptions.IgnoreCase);
					Match svnNameMatch = Regex.Match(assembly.get_GithubUrl(), searchText, RegexOptions.IgnoreCase);
					Match descNameMatch = Regex.Match(assembly.get_Description(), searchText, RegexOptions.IgnoreCase);
					flag = (authorMatch.Success | champeMatch || nameMatch.Success || svnNameMatch.Success ? true : descNameMatch.Success);
				}
				else
				{
					flag = false;
				}
			}
			catch (Exception exception)
			{
				flag = true;
			}
			return flag;
		}

		private TChildItem FindVisualChild<TChildItem>(DependencyObject obj)
		where TChildItem : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child is TChildItem)
				{
					return (TChildItem)child;
				}
				TChildItem childOfChild = this.FindVisualChild<TChildItem>(child);
				if (childOfChild != null)
				{
					return childOfChild;
				}
			}
			return default(TChildItem);
		}

		private DataGridCell GetCell(DataGridRow row, int columnIndex = 0)
		{
			IEnumerable<DataGridCellsPresenter> dataGridCellsPresenters;
			if (row != null)
			{
				dataGridCellsPresenters = TreeHelper.FindChildren<DataGridCellsPresenter>(row, true);
			}
			else
			{
				dataGridCellsPresenters = null;
			}
			IEnumerable<DataGridCellsPresenter> presenterE = dataGridCellsPresenters;
			if (presenterE == null)
			{
				return null;
			}
			DataGridCellsPresenter presenter = presenterE.ToList<DataGridCellsPresenter>()[0];
			DataGridCell cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
			if (cell != null)
			{
				return cell;
			}
			this.InstalledAssembliesDataGrid.ScrollIntoView(row, this.InstalledAssembliesDataGrid.Columns[columnIndex]);
			cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex);
			return cell;
		}

		private int GetCurrentRowIndex(MainWindow.GetPosition pos)
		{
			int curIndex = -1;
			for (int i = 0; i < this.InstalledAssembliesDataGrid.Items.Count; i++)
			{
				DataGridRow row = this.GetRowItem(i);
				if (row != null)
				{
					DataGridCell cell = this.GetCell(row, 0);
					if (cell != null && this.GetMouseTargetRow(row, pos) && !this.GetMouseTargetRow(cell, pos))
					{
						curIndex = i;
						break;
					}
				}
			}
			return curIndex;
		}

		private bool GetMouseTargetRow(Visual theTarget, MainWindow.GetPosition position)
		{
			Rect rect = VisualTreeHelper.GetDescendantBounds(theTarget);
			return rect.Contains(position((IInputElement)theTarget));
		}

		private DataGridRow GetRowItem(int index)
		{
			if (this.InstalledAssembliesDataGrid.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
			{
				return null;
			}
			return this.InstalledAssembliesDataGrid.ItemContainerGenerator.ContainerFromIndex(index) as DataGridRow;
		}

		private void GithubAssembliesItem_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.InstalledAssembliesDataGrid.SelectedItems.Count <= 0)
			{
				return;
			}
			LeagueSharpAssembly selectedAssembly = (LeagueSharpAssembly)this.InstalledAssembliesDataGrid.SelectedItems[0];
			if (selectedAssembly.SvnUrl != string.Empty)
			{
				InstallerWindow installerWindow = new InstallerWindow();
				((Window)installerWindow).Owner = this;
				InstallerWindow window = installerWindow;
				window.ShowProgress(selectedAssembly.SvnUrl, true, null);
				window.ShowDialog();
			}
		}

		private void GithubItem_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.InstalledAssembliesDataGrid.SelectedItems.Count <= 0)
			{
				return;
			}
			LeagueSharpAssembly selectedAssembly = (LeagueSharpAssembly)this.InstalledAssembliesDataGrid.SelectedItems[0];
			if (selectedAssembly.SvnUrl != string.Empty)
			{
				Process.Start(selectedAssembly.SvnUrl);
				return;
			}
			if (Directory.Exists(System.IO.Path.GetDirectoryName(selectedAssembly.PathToProjectFile)))
			{
				Process.Start(System.IO.Path.GetDirectoryName(selectedAssembly.PathToProjectFile));
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
			Application.LoadComponent(this, new Uri("/loader;component/views/mainwindow.xaml", UriKind.Relative));
		}

		private void InitSystem()
		{
			PathRandomizer.CopyFiles();
			Remoting.Init();
			Eudyptula.StopKill();
			Eudyptula.Start();
		}

		private void InstallButton_Click(object sender, RoutedEventArgs e)
		{
			InstallerWindow installerWindow = new InstallerWindow();
			((Window)installerWindow).Owner = this;
			installerWindow.ShowDialog();
		}

		private void InstalledAssembliesDataGrid_Drop(object sender, DragEventArgs e)
		{
			if (this.rowIndex < 0)
			{
				return;
			}
			int index = this.GetCurrentRowIndex(new MainWindow.GetPosition(e.GetPosition));
			if (index < 0)
			{
				return;
			}
			if (index == this.rowIndex)
			{
				return;
			}
			LeagueSharpAssembly changedAssembly = this.Config.SelectedProfile.InstalledAssemblies[this.rowIndex];
			this.Config.SelectedProfile.InstalledAssemblies.RemoveAt(this.rowIndex);
			this.Config.SelectedProfile.InstalledAssemblies.Insert(index, changedAssembly);
		}

		private void InstalledAssembliesDataGrid_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
		{
			DataGrid dataGrid = (DataGrid)sender;
			if (dataGrid == null)
			{
				e.Handled = true;
			}
			else if (dataGrid.SelectedItems.Count == 0)
			{
				e.Handled = true;
				return;
			}
		}

		private void InstalledAssembliesDataGrid_OnPreviewDragOver(object sender, DragEventArgs e)
		{
			FrameworkElement container = sender as FrameworkElement;
			if (container == null)
			{
				return;
			}
			ScrollViewer scrollViewer = this.FindVisualChild<ScrollViewer>(container);
			if (scrollViewer == null)
			{
				return;
			}
			double tolerance = 15;
			double verticalPos = e.GetPosition(container).Y;
			double offset = 1;
			if (verticalPos < tolerance)
			{
				scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - offset);
				return;
			}
			if (verticalPos > container.ActualHeight - tolerance)
			{
				scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + offset);
			}
		}

		private void InstalledAssembliesDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.rowIndex = this.GetCurrentRowIndex(new MainWindow.GetPosition(e.GetPosition));
			if (this.rowIndex < 0)
			{
				return;
			}
			if (this.IsColumnSelected(e))
			{
				return;
			}
			this.InstalledAssembliesDataGrid.SelectedIndex = this.rowIndex;
			LeagueSharpAssembly selectedAssembly = this.InstalledAssembliesDataGrid.Items[this.rowIndex] as LeagueSharpAssembly;
			if (selectedAssembly == null)
			{
				return;
			}
			DragDrop.DoDragDrop(this.InstalledAssembliesDataGrid, selectedAssembly, DragDropEffects.Move);
		}

		private async void InstallFromDbItem_OnClick(object sender, RoutedEventArgs e)
		{
			MainWindow.<InstallFromDbItem_OnClick>d__73 variable = new MainWindow.<InstallFromDbItem_OnClick>d__73();
			variable.<>4__this = this;
			variable.<>t__builder = AsyncVoidMethodBuilder.Create();
			variable.<>1__state = -1;
			variable.<>t__builder.Start<MainWindow.<InstallFromDbItem_OnClick>d__73>(ref variable);
		}

		private bool IsColumnSelected(MouseEventArgs e)
		{
			DependencyObject dep = (DependencyObject)e.OriginalSource;
			while (dep != null && !(dep is DataGridCell) && !(dep is DataGridColumnHeader))
			{
				dep = VisualTreeHelper.GetParent(dep);
			}
			if (dep is DataGridColumnHeader)
			{
				return true;
			}
			return false;
		}

		private void LogItem_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.InstalledAssembliesDataGrid.SelectedItems.Count <= 0)
			{
				return;
			}
			LeagueSharpAssembly selectedAssembly = (LeagueSharpAssembly)this.InstalledAssembliesDataGrid.SelectedItems[0];
			string logFile = System.IO.Path.Combine(Directories.LogsDir, string.Concat("Error - ", System.IO.Path.GetFileName(string.Concat(selectedAssembly.Name, ".txt"))));
			if (File.Exists(logFile))
			{
				Process.Start(logFile);
				return;
			}
			this.ShowTextMessage("Error", Utility.GetMultiLanguageText("LogNotFound"));
		}

		private async void MainWindow_OnActivated(object sender, EventArgs e)
		{
			try
			{
				string str = Clipboard.GetText();
				if (str.StartsWith(LSUriScheme.FullName))
				{
					Clipboard.SetText(string.Empty);
					await LSUriScheme.HandleUrl(str, this);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Utility.Log(LogStatus.Error, "Clipboard", exception.Message, Logs.MainLog);
			}
		}

		public void MainWindow_OnClosing(object sender, CancelEventArgs e)
		{
			base.Hide();
			if (this.AssembliesWorker.IsBusy && e != null)
			{
				this.AssembliesWorker.CancelAsync();
				e.Cancel = true;
				base.Hide();
				return;
			}
			LeagueSharp.Loader.Data.Config.Save(true);
			try
			{
				Thread injectThread = this.InjectThread;
				if (injectThread != null)
				{
					injectThread.Abort();
				}
				else
				{
				}
				Eudyptula.Stop();
			}
			catch
			{
			}
		}

		private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
		{
			Console.WriteLine(Thread.CurrentThread.Name);
			await this.Bootstrap();
			this.SetForeground();
		}

		private void NewItem_OnClick(object sender, RoutedEventArgs e)
		{
			this.ShowNewAssemblyDialog();
		}

		private void NewsButton_OnClick(object sender, RoutedEventArgs e)
		{
			this.TosBrowser.Visibility = Visibility.Hidden;
			this.Browser.Navigate(string.Format("{0}/loader/news/1/html", this.BaseUrl));
			this.MainTabControl.SelectedIndex = 1;
			this.Browser.Visibility = Visibility.Visible;
		}

		private void OnLogin(string username)
		{
			Utility.Log(LogStatus.Ok, "Login", string.Format("Succesfully signed in as {0}", username), Logs.MainLog);
			this.Browser.Visibility = Visibility.Visible;
			this.TosBrowser.Visibility = Visibility.Visible;
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
			if (propertyChangedEventHandler == null)
			{
				return;
			}
			propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
		}

		private void OnUpgradeClick(object sender, RoutedEventArgs e)
		{
			try
			{
				try
				{
					this.UpgradeButton.IsEnabled = false;
					if (MessageBox.Show("Upgrade to PlaySharp and import Config?\n", "PlaySharp Upgrade", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly) != MessageBoxResult.No)
					{
						LeagueSharp.Loader.Data.Config.Save(false);
						if (!Directory.Exists(Directories.AppDataDirectory))
						{
							Directory.CreateDirectory(Directories.AppDataDirectory);
						}
						string setupPath = System.IO.Path.Combine(Directories.CurrentDirectory, "update.exe");
						string configPath = System.IO.Path.Combine(Directories.AppDataDirectory, "config.xml");
						File.Copy(Directories.ConfigFilePath, configPath, true);
						using (WebClient client = new WebClient())
						{
							client.DownloadFile("https://api.joduska.me/api/v2.0/loader/loader/download/alpha", setupPath);
						}
						StringBuilder arguments = new StringBuilder();
						arguments.Append("/CLOSEAPPLICATIONS ");
						arguments.Append("/TYPE=LeagueSharp ");
						Process process = new Process();
						process.StartInfo.FileName = setupPath;
						process.StartInfo.Arguments = arguments.ToString();
						process.Start();
						Environment.Exit(0);
					}
				}
				catch (Exception exception)
				{
					MessageBox.Show(exception.Message, "PlaySharp Upgrade");
				}
			}
			finally
			{
				this.UpgradeButton.IsEnabled = true;
			}
		}

		public async Task PrepareAssemblies(IEnumerable<LeagueSharpAssembly> assemblies, bool update, bool compile)
		{
			this.Working = true;
			object list = assemblies as IList<LeagueSharpAssembly>;
			if (list == null)
			{
				list = assemblies.ToList<LeagueSharpAssembly>();
			}
			IList<LeagueSharpAssembly> leagueSharpAssemblies2 = (IList<LeagueSharpAssembly>)list;
			Directory.CreateDirectory(Directories.AssembliesDir);
			await Task.Factory.StartNew(() => {
				if (update)
				{
					IList<LeagueSharpAssembly> leagueSharpAssemblies = leagueSharpAssemblies2;
					Func<LeagueSharpAssembly, string> u003cu003e9_441 = MainWindow.<>c.<>9__44_1;
					if (u003cu003e9_441 == null)
					{
						u003cu003e9_441 = (LeagueSharpAssembly a) => a.SvnUrl;
						MainWindow.<>c.<>9__44_1 = u003cu003e9_441;
					}
					IEnumerable<IGrouping<string, LeagueSharpAssembly>> groupings = leagueSharpAssemblies.GroupBy<LeagueSharpAssembly, string>(u003cu003e9_441);
					Func<IGrouping<string, LeagueSharpAssembly>, LeagueSharpAssembly> u003cu003e9_442 = MainWindow.<>c.<>9__44_2;
					if (u003cu003e9_442 == null)
					{
						u003cu003e9_442 = (IGrouping<string, LeagueSharpAssembly> g) => g.First<LeagueSharpAssembly>();
						MainWindow.<>c.<>9__44_2 = u003cu003e9_442;
					}
					Parallel.ForEach<LeagueSharpAssembly>(groupings.Select<IGrouping<string, LeagueSharpAssembly>, LeagueSharpAssembly>(u003cu003e9_442).ToList<LeagueSharpAssembly>(), new ParallelOptions()
					{
						MaxDegreeOfParallelism = this.Config.Workers
					}, (LeagueSharpAssembly assembly, ParallelLoopState state) => {
						assembly.Update();
						if (this.AssembliesWorker.CancellationPending)
						{
							this.AssembliesWorkerCancelled = true;
							state.Break();
						}
					});
				}
			});
			try
			{
				IList<LeagueSharpAssembly> leagueSharpAssemblies3 = leagueSharpAssemblies2;
				DependencyInstaller dependencyInstaller = new DependencyInstaller((
					from a in leagueSharpAssemblies3
					select a.PathToProjectFile).ToList<string>());
				await dependencyInstaller.SatisfyAsync();
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
			await Task.Factory.StartNew(() => {
				if (compile)
				{
					IList<LeagueSharpAssembly> leagueSharpAssemblies = leagueSharpAssemblies2;
					Func<LeagueSharpAssembly, bool> u003cu003e9_446 = MainWindow.<>c.<>9__44_6;
					if (u003cu003e9_446 == null)
					{
						u003cu003e9_446 = (LeagueSharpAssembly a) => a.Type == 3;
						MainWindow.<>c.<>9__44_6 = u003cu003e9_446;
					}
					IOrderedEnumerable<LeagueSharpAssembly> leagueSharpAssemblies1 = leagueSharpAssemblies.OrderByDescending<LeagueSharpAssembly, bool>(u003cu003e9_446);
					Func<LeagueSharpAssembly, bool> u003cu003e9_447 = MainWindow.<>c.<>9__44_7;
					if (u003cu003e9_447 == null)
					{
						u003cu003e9_447 = (LeagueSharpAssembly a) => a.Name.StartsWith("LeagueSharp.");
						MainWindow.<>c.<>9__44_7 = u003cu003e9_447;
					}
					foreach (LeagueSharpAssembly leagueSharpAssembly in leagueSharpAssemblies1.ThenByDescending<LeagueSharpAssembly, bool>(u003cu003e9_447).ToList<LeagueSharpAssembly>())
					{
						leagueSharpAssembly.Compile();
						if (!this.AssembliesWorker.CancellationPending)
						{
							continue;
						}
						this.AssembliesWorkerCancelled = true;
						return;
					}
				}
			});
			await Task.Factory.StartNew(() => {
				ProjectCollection.GlobalProjectCollection.UnloadAllProjects();
				if (this.AssembliesWorkerCancelled)
				{
					base.Close();
				}
			});
			if (!this.Config.EnableDebug)
			{
				foreach (string str in Directory.EnumerateFiles(Directories.CoreDirectory, "*.pdb"))
				{
					try
					{
						File.Delete(str);
					}
					catch
					{
					}
				}
				foreach (string str1 in Directory.EnumerateFiles(Directories.AssembliesDir, "*.pdb"))
				{
					try
					{
						File.Delete(str1);
					}
					catch
					{
					}
				}
			}
			this.Working = false;
		}

		private void RemoveMenuItem_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.InstalledAssembliesDataGrid.SelectedItems.Count <= 0)
			{
				return;
			}
			List<LeagueSharpAssembly> remove = this.InstalledAssembliesDataGrid.SelectedItems.Cast<LeagueSharpAssembly>().ToList<LeagueSharpAssembly>();
			this.DeleteWithConfirmation(remove);
		}

		private void SetForeground()
		{
			base.Show();
			if (base.WindowState == WindowState.Minimized)
			{
				base.WindowState = WindowState.Normal;
			}
			base.Activate();
			base.Topmost = true;
			base.Topmost = false;
			base.Focus();
		}

		private void SettingsButton_OnClick(object sender, RoutedEventArgs e)
		{
			this.Browser.Visibility = Visibility.Hidden;
			this.TosBrowser.Visibility = Visibility.Hidden;
			this.MainTabControl.SelectedIndex = 3;
		}

		private void ShareItem_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.InstalledAssembliesDataGrid.SelectedItems.Count <= 0)
			{
				return;
			}
			string stringToAppend = string.Empty;
			int count = 0;
			foreach (LeagueSharpAssembly selectedAssembly in this.InstalledAssembliesDataGrid.SelectedItems.Cast<LeagueSharpAssembly>())
			{
				if (!selectedAssembly.SvnUrl.StartsWith("https://github.com", StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}
				string user = selectedAssembly.SvnUrl.Remove(0, 19);
				stringToAppend = string.Concat(stringToAppend, string.Format("{0}/{1}/", user, selectedAssembly.Name));
				count++;
			}
			if (count > 0)
			{
				Clipboard.SetText(string.Concat(LSUriScheme.FullName, (count == 1 ? "project" : "projectGroup"), "/", stringToAppend));
				this.ShowTextMessage(Utility.GetMultiLanguageText("MenuShare"), Utility.GetMultiLanguageText("ShareText"));
			}
		}

		private async Task ShowLoginDialog()
		{
			LoginDialogData loginDialogDatum;
			this.get_MetroDialogOptions().set_ColorScheme(0);
			while (true)
			{
				MainWindow mainWindow = this;
				LoginDialogSettings loginDialogSetting = new LoginDialogSettings();
				loginDialogSetting.set_ColorScheme(this.get_MetroDialogOptions().get_ColorScheme());
				loginDialogSetting.set_NegativeButtonVisibility(Visibility.Visible);
				LoginDialogData loginDialogDatum1 = await DialogManager.ShowLoginAsync((MetroWindow)mainWindow, "LeagueSharp", "Sign in", loginDialogSetting);
				loginDialogDatum = loginDialogDatum1;
				if (loginDialogDatum == null)
				{
					this.MainWindow_OnClosing(null, null);
					Environment.Exit(0);
				}
				string str = Auth.Hash(loginDialogDatum.get_Password());
				Tuple<bool, string> tuple = await Auth.Login(loginDialogDatum.get_Username(), str);
				Tuple<bool, string> tuple1 = tuple;
				if (tuple1.Item1)
				{
					break;
				}
				await DialogManager.ShowMessageAsync(this, "Login", string.Format(Utility.GetMultiLanguageText("FailedToLogin"), tuple1.Item2), 0, null);
				Utility.Log(LogStatus.Error, Utility.GetMultiLanguageText("Login"), string.Format(Utility.GetMultiLanguageText("LoginError"), loginDialogDatum.get_Username(), tuple1.Item2), Logs.MainLog);
				loginDialogDatum = null;
				tuple1 = null;
			}
			LeagueSharp.Loader.Data.Config.Instance.Username = loginDialogDatum.get_Username();
			LeagueSharp.Loader.Data.Config.Instance.Password = Auth.Hash(loginDialogDatum.get_Password());
			this.OnLogin(loginDialogDatum.get_Username());
		}

		private async void ShowNewAssemblyDialog()
		{
			string str = await DialogManager.ShowInputAsync(this, "New Project", "Enter the new project name", null);
			string str1 = str;
			if (str1 != null)
			{
				str1 = Regex.Replace(str1, "[^A-Za-z0-9]+", string.Empty);
			}
			if (!string.IsNullOrEmpty(str1))
			{
				LeagueSharpAssembly leagueSharpAssembly = Utility.CreateEmptyAssembly(str1);
				if (leagueSharpAssembly != null)
				{
					leagueSharpAssembly.Compile();
					this.Config.SelectedProfile.InstalledAssemblies.Add(leagueSharpAssembly);
				}
			}
		}

		public async void ShowTextMessage(string title, string message)
		{
			this.Browser.Visibility = Visibility.Hidden;
			this.TosBrowser.Visibility = Visibility.Hidden;
			await DialogManager.ShowMessageAsync(this, title, message, 0, null);
		}

		private async void StatusButton_OnClick(object sender, RoutedEventArgs e)
		{
			this.Browser.Visibility = Visibility.Hidden;
			this.TosBrowser.Visibility = Visibility.Hidden;
			await this.UpdateAccount();
			await this.CheckForUpdates(true, true, true);
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
					((MainWindow)target).Loaded += new RoutedEventHandler(this.MainWindow_OnLoaded);
					((MainWindow)target).Closing += new CancelEventHandler(this.MainWindow_OnClosing);
					((MainWindow)target).Activated += new EventHandler(this.MainWindow_OnActivated);
					return;
				}
				case 2:
				{
					this.icon_connected = (Rectangle)target;
					return;
				}
				case 3:
				{
					this.icon_disconnected = (Rectangle)target;
					return;
				}
				case 4:
				{
					this.NewsButton = (Button)target;
					this.NewsButton.Click += new RoutedEventHandler(this.NewsButton_OnClick);
					return;
				}
				case 5:
				{
					this.AssemblyButton = (Button)target;
					this.AssemblyButton.Click += new RoutedEventHandler(this.AssemblyButton_OnClick);
					return;
				}
				case 6:
				{
					this.AssemblyDBButton = (Button)target;
					this.AssemblyDBButton.Click += new RoutedEventHandler(this.AssemblyDBButton_OnClick);
					return;
				}
				case 7:
				{
					this.StatusButton = (Button)target;
					this.StatusButton.Click += new RoutedEventHandler(this.StatusButton_OnClick);
					return;
				}
				case 8:
				{
					this.SettingsButton = (Button)target;
					this.SettingsButton.Click += new RoutedEventHandler(this.SettingsButton_OnClick);
					return;
				}
				case 9:
				{
					this.MainTabControl = (TabControl)target;
					return;
				}
				case 10:
				{
					this.TosTabItem = (TabItem)target;
					return;
				}
				case 11:
				{
					this.TosBrowser = (WebBrowser)target;
					return;
				}
				case 12:
				{
					((Button)target).Click += new RoutedEventHandler(this.TosAccept_Click);
					return;
				}
				case 13:
				{
					((Button)target).Click += new RoutedEventHandler(this.TosDecline_Click);
					return;
				}
				case 14:
				{
					this.NewsTabItem = (TabItem)target;
					return;
				}
				case 15:
				{
					this.Browser = (WebBrowser)target;
					return;
				}
				case 16:
				{
					this.AssembliesTabItem = (TabItem)target;
					return;
				}
				case 17:
				{
					this.InstalledAssembliesDataGrid = (DataGrid)target;
					this.InstalledAssembliesDataGrid.ContextMenuOpening += new ContextMenuEventHandler(this.InstalledAssembliesDataGrid_OnContextMenuOpening);
					this.InstalledAssembliesDataGrid.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.InstalledAssembliesDataGrid_PreviewMouseLeftButtonDown);
					this.InstalledAssembliesDataGrid.Drop += new DragEventHandler(this.InstalledAssembliesDataGrid_Drop);
					this.InstalledAssembliesDataGrid.PreviewDragOver += new DragEventHandler(this.InstalledAssembliesDataGrid_OnPreviewDragOver);
					return;
				}
				case 18:
				{
					this.UpdateAndCompileMenuItem = (MenuItem)target;
					this.UpdateAndCompileMenuItem.Click += new RoutedEventHandler(this.UpdateAndCompileMenuItem_OnClick);
					return;
				}
				case 19:
				{
					this.RemoveMenuItem = (MenuItem)target;
					this.RemoveMenuItem.Click += new RoutedEventHandler(this.RemoveMenuItem_OnClick);
					return;
				}
				case 20:
				{
					this.GithubItem = (MenuItem)target;
					this.GithubItem.Click += new RoutedEventHandler(this.GithubItem_OnClick);
					return;
				}
				case 21:
				{
					this.GithubAssembliesItem = (MenuItem)target;
					this.GithubAssembliesItem.Click += new RoutedEventHandler(this.GithubAssembliesItem_OnClick);
					return;
				}
				case 22:
				{
					this.ShareItem = (MenuItem)target;
					this.ShareItem.Click += new RoutedEventHandler(this.ShareItem_OnClick);
					return;
				}
				case 23:
				{
					this.DevMenu = (MenuItem)target;
					return;
				}
				case 24:
				{
					this.NewItem = (MenuItem)target;
					this.NewItem.Click += new RoutedEventHandler(this.NewItem_OnClick);
					return;
				}
				case 25:
				{
					this.EditItem = (MenuItem)target;
					this.EditItem.Click += new RoutedEventHandler(this.EditItem_OnClick);
					return;
				}
				case 26:
				{
					this.CloneItem = (MenuItem)target;
					this.CloneItem.Click += new RoutedEventHandler(this.CloneItem_OnClick);
					return;
				}
				case 27:
				{
					this.LogItem = (MenuItem)target;
					this.LogItem.Click += new RoutedEventHandler(this.LogItem_OnClick);
					return;
				}
				case 28:
				{
					this.ColumnCheck = (DataGridCheckBoxColumn)target;
					return;
				}
				case 29:
				{
					this.ColumnName = (DataGridTextColumn)target;
					return;
				}
				case 30:
				{
					this.ColumnType = (DataGridTextColumn)target;
					return;
				}
				case 31:
				{
					this.ColumnVersion = (DataGridTextColumn)target;
					return;
				}
				case 32:
				{
					this.ColumnLocation = (DataGridTextColumn)target;
					return;
				}
				case 33:
				{
					this.ColumnDescription = (DataGridTextColumn)target;
					return;
				}
				case 34:
				{
					this.InstallButton = (Button)target;
					this.InstallButton.Click += new RoutedEventHandler(this.InstallButton_Click);
					return;
				}
				case 35:
				{
					((Button)target).Click += new RoutedEventHandler(this.UpdateAll_OnClick);
					return;
				}
				case 36:
				{
					((Button)target).Click += new RoutedEventHandler(this.CompileAll_OnClick);
					return;
				}
				case 37:
				{
					this.UpgradeButton = (Button)target;
					this.UpgradeButton.Click += new RoutedEventHandler(this.OnUpgradeClick);
					return;
				}
				case 38:
				{
					this.SettingsTabItem = (TabItem)target;
					return;
				}
				case 39:
				{
					this.SettingsFrame = (ContentControl)target;
					return;
				}
				case 40:
				{
					((TreeView)target).SelectedItemChanged += new RoutedPropertyChangedEventHandler<object>(this.TreeView_OnSelectedItemChanged);
					return;
				}
				case 41:
				{
					this.GeneralSettingsItem = (TreeViewItem)target;
					return;
				}
				case 42:
				{
					this.AssemblyDB = (TabItem)target;
					return;
				}
				case 43:
				{
					this.AssembliesDBDataGrid = (DataGrid)target;
					this.AssembliesDBDataGrid.ContextMenuOpening += new ContextMenuEventHandler(this.InstalledAssembliesDataGrid_OnContextMenuOpening);
					this.AssembliesDBDataGrid.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.InstalledAssembliesDataGrid_PreviewMouseLeftButtonDown);
					this.AssembliesDBDataGrid.Drop += new DragEventHandler(this.InstalledAssembliesDataGrid_Drop);
					this.AssembliesDBDataGrid.PreviewDragOver += new DragEventHandler(this.InstalledAssembliesDataGrid_OnPreviewDragOver);
					return;
				}
				case 44:
				{
					this.InstallFromDbItem = (MenuItem)target;
					this.InstallFromDbItem.Click += new RoutedEventHandler(this.InstallFromDbItem_OnClick);
					return;
				}
				case 45:
				{
					this.DBColumnName = (DataGridTextColumn)target;
					return;
				}
				case 46:
				{
					this.DBColumnAuthor = (DataGridTextColumn)target;
					return;
				}
				case 47:
				{
					this.DBColumnDescription = (DataGridTextColumn)target;
					return;
				}
				case 48:
				{
					this.Header = (TextBlock)target;
					return;
				}
				case 49:
				{
					this.TrayIcon = (TaskbarIcon)target;
					this.TrayIcon.add_TrayMouseDoubleClick(new RoutedEventHandler(this.TrayIcon_OnTrayMouseDoubleClick));
					this.TrayIcon.add_TrayLeftMouseUp(new RoutedEventHandler(this.TrayIcon_OnTrayLeftMouseUp));
					return;
				}
				case 50:
				{
					this.MenuItemLabelHide = (MenuItem)target;
					this.MenuItemLabelHide.Click += new RoutedEventHandler(this.TrayMenuHide_OnClick);
					return;
				}
				case 51:
				{
					((MenuItem)target).Click += new RoutedEventHandler(this.TrayMenuClose_OnClick);
					return;
				}
			}
			this._contentLoaded = true;
		}

		private void TosAccept_Click(object sender, RoutedEventArgs e)
		{
			LeagueSharp.Loader.Data.Config.Instance.TosAccepted = true;
			this.MainTabControl.SelectedIndex = 1;
		}

		private void TosButton_OnClick(object sender, RoutedEventArgs e)
		{
			this.Browser.Visibility = Visibility.Hidden;
			this.TosBrowser.Navigate(string.Format("{0}/loader/tos/1", this.BaseUrl));
			this.MainTabControl.SelectedIndex = 0;
			this.TosBrowser.Visibility = Visibility.Visible;
		}

		private void TosDecline_Click(object sender, RoutedEventArgs e)
		{
			this.MainWindow_OnClosing(null, null);
			Environment.Exit(0);
		}

		private void TrayIcon_OnTrayLeftMouseUp(object sender, RoutedEventArgs e)
		{
			if (base.Visibility == Visibility.Visible)
			{
				base.Hide();
				this.MenuItemLabelHide.Header = "Show";
			}
		}

		private void TrayIcon_OnTrayMouseDoubleClick(object sender, RoutedEventArgs e)
		{
			if (base.Visibility == Visibility.Hidden)
			{
				this.SetForeground();
				this.MenuItemLabelHide.Header = "Hide";
			}
		}

		private void TrayMenuClose_OnClick(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		private void TrayMenuHide_OnClick(object sender, RoutedEventArgs e)
		{
			if (base.Visibility == Visibility.Visible)
			{
				base.Hide();
				this.MenuItemLabelHide.Header = "Show";
				return;
			}
			this.SetForeground();
			this.MenuItemLabelHide.Header = "Hide";
		}

		private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			string name = ((TreeViewItem)((TreeView)sender).SelectedItem).Uid;
			Type viewType = Type.GetType(string.Format("LeagueSharp.Loader.Views.Settings.{0}", name));
			if (viewType != null)
			{
				this.SettingsFrame.Content = Activator.CreateInstance(viewType);
				return;
			}
			Utility.Log(LogStatus.Error, "TreeView_OnSelectedItemChanged", string.Format("Could not find Settings View (LeagueSharp.Loader.Views.Settings.{0})", name), Logs.MainLog);
		}

		private async Task UpdateAccount()
		{
			try
			{
				if ((DateTime.Now - this.LastAccountUpdate) >= TimeSpan.FromMinutes(10))
				{
					this.LastAccountUpdate = DateTime.Now;
					Account account = await WebService.Client.AccountAsync(new CancellationToken());
					string str = "Normal";
					if (account.get_IsSubscriber())
					{
						str = "Sub";
					}
					if (account.get_IsBotter())
					{
						str = "Bot";
					}
					if (account.get_IsDev())
					{
						str = "Dev";
					}
					TextBlock header = this.Header;
					object[] displayName = new object[] { account.get_DisplayName(), str, account.get_GamesPlayed(), account.get_MaxGames(), Assembly.GetExecutingAssembly().GetName().Version };
					header.Text = string.Format("L# - {0}/{1} - {2}/{3} - {4}", displayName);
				}
				else
				{
					return;
				}
			}
			catch
			{
			}
		}

		private async void UpdateAll_OnClick(object sender, RoutedEventArgs e)
		{
			if (!this.Working)
			{
				await this.PrepareAssemblies(LeagueSharp.Loader.Data.Config.Instance.SelectedProfile.InstalledAssemblies, true, true);
			}
		}

		private async void UpdateAndCompileMenuItem_OnClick(object sender, RoutedEventArgs e)
		{
			if (this.InstalledAssembliesDataGrid.SelectedItems.Count != 0)
			{
				if (!this.Working)
				{
					await this.PrepareAssemblies(this.InstalledAssembliesDataGrid.SelectedItems.Cast<LeagueSharpAssembly>(), true, true);
				}
			}
		}

		private void UpdateFilters()
		{
			if (!base.Dispatcher.CheckAccess())
			{
				return;
			}
			ICollectionView view = null;
			int selectedIndex = this.MainTabControl.SelectedIndex;
			if (selectedIndex == 2)
			{
				view = CollectionViewSource.GetDefaultView(LeagueSharp.Loader.Data.Config.Instance.SelectedProfile.InstalledAssemblies);
				view.Filter = new Predicate<object>(this.FilterAssemblies);
				return;
			}
			if (selectedIndex != 4)
			{
				return;
			}
			view = CollectionViewSource.GetDefaultView(LeagueSharp.Loader.Data.Config.Instance.DatabaseAssemblies);
			view.Filter = new Predicate<object>(this.FilterDatabaseAssemblies);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public delegate Point GetPosition(IInputElement element);
	}
}