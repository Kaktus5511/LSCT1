using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LeagueSharp.Loader.Data
{
	public class LogItem : INotifyPropertyChanged
	{
		private string _message;

		private string _source;

		private string _status;

		public string Message
		{
			get
			{
				return this._message;
			}
			set
			{
				this._message = value;
				this.OnPropertyChanged("Message");
			}
		}

		public string Source
		{
			get
			{
				return this._source;
			}
			set
			{
				this._source = value;
				this.OnPropertyChanged("Source");
			}
		}

		public string Status
		{
			get
			{
				return this._status;
			}
			set
			{
				this._status = value;
				this.OnPropertyChanged("Status");
			}
		}

		public LogItem()
		{
		}

		private void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}