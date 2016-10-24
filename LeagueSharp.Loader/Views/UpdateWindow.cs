using LeagueSharp.Loader.Class;
using LeagueSharp.Loader.Data;
using MahApps.Metro.Controls;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Threading;

namespace LeagueSharp.Loader.Views
{
	public class UpdateWindow : MetroWindow, INotifyPropertyChanged, IComponentConnector
	{
		private string progressText;

		private string updateMessage;

		internal ProgressBar UpdateProgressBar;

		private bool _contentLoaded;

		private UpdateAction Action
		{
			get;
			set;
		}

		public string ProgressText
		{
			get
			{
				return this.progressText;
			}
			set
			{
				if (object.Equals(value, this.progressText))
				{
					return;
				}
				this.progressText = value;
				this.OnPropertyChanged("ProgressText");
			}
		}

		public string UpdateMessage
		{
			get
			{
				return this.updateMessage;
			}
			set
			{
				if (object.Equals(value, this.updateMessage))
				{
					return;
				}
				this.updateMessage = value;
				this.OnPropertyChanged("UpdateMessage");
			}
		}

		private string UpdateUrl
		{
			get;
			set;
		}

		public UpdateWindow(UpdateAction action, string url)
		{
			this.InitializeComponent();
			base.DataContext = this;
			this.Action = action;
			this.UpdateUrl = url;
		}

		public UpdateWindow()
		{
			this.InitializeComponent();
			base.DataContext = this;
			if (DesignerProperties.GetIsInDesignMode(this))
			{
				this.UpdateMessage = base.FindResource("Updating").ToString();
				this.ProgressText = base.FindResource("UpdateText").ToString();
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
			Application.LoadComponent(this, new Uri("/loader;component/views/updatewindow.xaml", UriKind.Relative));
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

		[DebuggerNonUserCode]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
		{
			if (connectionId != 1)
			{
				this._contentLoaded = true;
				return;
			}
			this.UpdateProgressBar = (ProgressBar)target;
		}

		public async Task<bool> Update()
		{
			bool flag;
			this.Focus();
			bool flag1 = false;
			this.UpdateProgressBar.Value = 0;
			this.UpdateProgressBar.Maximum = 100;
			UpdateAction action = this.Action;
			if (action == UpdateAction.Core)
			{
				flag = await this.UpdateCore();
				flag1 = flag;
			}
			else if (action == UpdateAction.Loader)
			{
				flag = await this.UpdateLoader();
				flag1 = flag;
			}
			Application.Current.Dispatcher.InvokeAsync<Task>(async () => {
				await Task.Delay(250);
				this.Close();
			});
			return flag1;
		}

		private async Task<bool> UpdateCore()
		{
			UpdateWindow.<UpdateCore>d__23 variable = new UpdateWindow.<UpdateCore>d__23();
			variable.<>4__this = this;
			variable.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			variable.<>1__state = -1;
			variable.<>t__builder.Start<UpdateWindow.<UpdateCore>d__23>(ref variable);
			return variable.<>t__builder.Task;
		}

		private async Task<bool> UpdateLoader()
		{
			UpdateWindow.<UpdateLoader>d__24 variable = new UpdateWindow.<UpdateLoader>d__24();
			variable.<>4__this = this;
			variable.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			variable.<>1__state = -1;
			variable.<>t__builder.Start<UpdateWindow.<UpdateLoader>d__24>(ref variable);
			return variable.<>t__builder.Task;
		}

		private void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs args)
		{
			Application.Current.Dispatcher.InvokeAsync(() => {
				this.UpdateProgressBar.Value = (double)args.ProgressPercentage;
				this.ProgressText = string.Format(base.FindResource("UpdateText").ToString(), args.BytesReceived / (long)1024, args.TotalBytesToReceive / (long)1024);
			});
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}