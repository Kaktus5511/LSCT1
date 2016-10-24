using LeagueSharp.Loader.Data;
using LeagueSharp.Loader.Views;
using MahApps.Metro;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace LeagueSharp.Loader.Views.Settings
{
	public partial class General : UserControl
	{
		private readonly string[] _accentColors = new string[] { "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald", "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta", "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve", "Taupe", "Sienna" };

		public General()
		{
			GameSettings[] array = Config.Instance.Settings.GameSettings.ToArray<GameSettings>();
			for (int i = 0; i < (int)array.Length; i++)
			{
				GameSettings setting = array[i];
				string name = setting.Name;
				if (name == "Anti-AFK" || name == "Display Enemy Tower Range" || name == "Extended Zoom" || name == "Show Ping")
				{
					Config.Instance.Settings.GameSettings.Remove(setting);
				}
			}
			this.InitializeComponent();
		}

		private void AppData_OnClick(object sender, RoutedEventArgs e)
		{
			Process.Start(Directories.AppDataDirectory);
		}

		private void Color_Loaded(object sender, RoutedEventArgs e)
		{
			ComboBox colorBox = (ComboBox)sender;
			string[] strArrays = this._accentColors;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string asccent = strArrays[i];
				colorBox.Items.Add(asccent);
			}
			if (Config.Instance.SelectedColor != null)
			{
				colorBox.SelectedItem = Config.Instance.SelectedColor;
			}
			if (colorBox.SelectedIndex == -1)
			{
				colorBox.SelectedIndex = colorBox.Items.IndexOf("Blue");
			}
		}

		private void Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string color = ((ComboBox)sender).SelectedValue.ToString();
			if (this._accentColors.Contains<string>(color))
			{
				ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(color), ThemeManager.GetAppTheme("BaseLight"));
				Config.Instance.SelectedColor = color;
			}
		}

		private void ComboBox_Loaded(object sender, RoutedEventArgs e)
		{
			ComboBox senderBox = (ComboBox)sender;
			senderBox.Items.Clear();
			senderBox.Items.Add("Arabic");
			senderBox.Items.Add("Bulgarian");
			senderBox.Items.Add("Chinese");
			senderBox.Items.Add("Czech");
			senderBox.Items.Add("Dutch");
			senderBox.Items.Add("English");
			senderBox.Items.Add("French");
			senderBox.Items.Add("German");
			senderBox.Items.Add("Greek");
			senderBox.Items.Add("Hungarian");
			senderBox.Items.Add("Italian");
			senderBox.Items.Add("Korean");
			senderBox.Items.Add("Latvian");
			senderBox.Items.Add("Lithuanian");
			senderBox.Items.Add("Polish");
			senderBox.Items.Add("Portuguese");
			senderBox.Items.Add("Romanian");
			senderBox.Items.Add("Russian");
			senderBox.Items.Add("Spanish");
			senderBox.Items.Add("Swedish");
			senderBox.Items.Add("Thai");
			senderBox.Items.Add("Traditional-Chinese");
			senderBox.Items.Add("Turkish");
			senderBox.Items.Add("Vietnamese");
			if (Config.Instance.SelectedLanguage != null)
			{
				senderBox.SelectedItem = senderBox.Items.Cast<string>().FirstOrDefault<string>((string item) => item == Config.Instance.SelectedLanguage);
			}
			if (senderBox.SelectedIndex == -1)
			{
				senderBox.SelectedIndex = senderBox.Items.IndexOf("English");
			}
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count <= 0 || e.RemovedItems.Count <= 0)
			{
				return;
			}
			string selected = (string)e.AddedItems[0];
			if (Config.Instance.SelectedLanguage == selected || Config.Instance.SelectedLanguage == null && selected == "English")
			{
				return;
			}
			Config.Instance.SelectedLanguage = selected;
			Config.SaveAndRestart(true);
		}

		private void GameSettingsDataGrid_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			object item = ((DataGrid)sender).SelectedItem;
			if (item != null)
			{
				((GameSettings)item).SelectedValue = (((GameSettings)item).SelectedValue == ((GameSettings)item).PosibleValues[0] ? ((GameSettings)item).PosibleValues[1] : ((GameSettings)item).PosibleValues[0]);
			}
		}

		private void Logout_OnClick(object sender, RoutedEventArgs e)
		{
			Config.Instance.Username = string.Empty;
			Config.Instance.Password = string.Empty;
			((MainWindow)base.DataContext).MainWindow_OnClosing(null, null);
			Process.Start(Application.ResourceAssembly.Location);
			Environment.Exit(0);
		}

		private void SaveConfig_Click(object sender, RoutedEventArgs e)
		{
			Config.Save(true);
		}
	}
}