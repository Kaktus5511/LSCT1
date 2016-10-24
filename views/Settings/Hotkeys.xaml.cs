using LeagueSharp.Loader.Data;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;

namespace LeagueSharp.Loader.Views.Settings
{
	public partial class Hotkeys : UserControl
	{
		public Hotkeys()
		{
			this.InitializeComponent();
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			foreach (HotkeyEntry item in this.HotkeysDataGrid.Items.Cast<HotkeyEntry>())
			{
				item.Hotkey = item.DefaultKey;
			}
		}

		private void Hotkeys_OnKeyUp(object sender, KeyEventArgs e)
		{
			object item = this.HotkeysDataGrid.SelectedItem;
			if (item != null)
			{
				((HotkeyEntry)item).Hotkey = e.Key;
			}
		}
	}
}