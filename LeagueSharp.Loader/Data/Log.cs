using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LeagueSharp.Loader.Data
{
	public class Log : INotifyPropertyChanged
	{
		private ObservableCollection<LogItem> _items = new ObservableCollection<LogItem>();

		public ObservableCollection<LogItem> Items
		{
			get
			{
				return this._items;
			}
			set
			{
				this._items = value;
				this.OnPropertyChanged("Items");
			}
		}

		public Log()
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