using LeagueSharp.Loader.Class;
using LeagueSharp.Loader.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.CodeDom.Compiler;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace LeagueSharp.Loader.Views
{
	public partial class ProfileControl : UserControl, INotifyPropertyChanged
	{
		public LeagueSharp.Loader.Data.Config Config
		{
			get
			{
				return LeagueSharp.Loader.Data.Config.Instance;
			}
		}

		public ProfileControl()
		{
			this.InitializeComponent();
			base.DataContext = this;
		}

		private void EditProfileMenuItem_OnClick(object sender, RoutedEventArgs e)
		{
			this.ShowProfileNameChangeDialog();
		}

		private void NewProfileMenuItem_OnClick(object sender, RoutedEventArgs e)
		{
			LeagueSharp.Loader.Data.Config.Instance.Profiles.Add(new Profile()
			{
				InstalledAssemblies = new ObservableCollection<LeagueSharpAssembly>(),
				Name = Utility.GetMultiLanguageText("NewProfile2")
			});
			LeagueSharp.Loader.Data.Config.Instance.SelectedProfile = LeagueSharp.Loader.Data.Config.Instance.Profiles.Last<Profile>();
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

		private void ProfilesButton_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			this.ShowProfileNameChangeDialog();
		}

		private void ProfilesButton_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.Config.OnPropertyChanged("SearchText");
		}

		private void RemoveProfileMenuItem_OnClick(object sender, RoutedEventArgs e)
		{
			if (LeagueSharp.Loader.Data.Config.Instance.Profiles.Count <= 1)
			{
				LeagueSharp.Loader.Data.Config.Instance.SelectedProfile.InstalledAssemblies = new ObservableCollection<LeagueSharpAssembly>();
				LeagueSharp.Loader.Data.Config.Instance.SelectedProfile.Name = Utility.GetMultiLanguageText("DefaultProfile");
				return;
			}
			LeagueSharp.Loader.Data.Config.Instance.Profiles.Remove(this.Config.SelectedProfile);
			LeagueSharp.Loader.Data.Config.Instance.SelectedProfile = LeagueSharp.Loader.Data.Config.Instance.Profiles.First<Profile>();
		}

		private async void ShowProfileNameChangeDialog()
		{
			MainWindow instance = MainWindow.Instance;
			string multiLanguageText = Utility.GetMultiLanguageText("Rename");
			string str = Utility.GetMultiLanguageText("RenameText");
			MetroDialogSettings metroDialogSetting = new MetroDialogSettings();
			metroDialogSetting.set_DefaultText(LeagueSharp.Loader.Data.Config.Instance.SelectedProfile.Name);
			string str1 = await DialogManager.ShowInputAsync((MetroWindow)instance, multiLanguageText, str, metroDialogSetting);
			string str2 = str1;
			if (!string.IsNullOrEmpty(str2))
			{
				LeagueSharp.Loader.Data.Config.Instance.SelectedProfile.Name = str2;
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}