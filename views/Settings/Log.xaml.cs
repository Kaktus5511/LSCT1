using LeagueSharp.Loader.Data;
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace LeagueSharp.Loader.Views.Settings
{
	public partial class Log : UserControl
	{
		public Log()
		{
			this.InitializeComponent();
			this.LogsDataGrid.ItemsSource = Logs.MainLog.Items;
		}

		private void OpenFolderButton_Click(object sender, RoutedEventArgs e)
		{
			if (Directory.Exists(Directories.LogsDir))
			{
				Process.Start(Directories.LogsDir);
			}
		}
	}
}