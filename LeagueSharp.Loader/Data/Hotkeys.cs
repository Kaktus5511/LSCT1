using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;

namespace LeagueSharp.Loader.Data
{
	[XmlType(AnonymousType=true)]
	public class Hotkeys : INotifyPropertyChanged
	{
		private ObservableCollection<HotkeyEntry> _selectedHotkeys;

		[XmlArrayItem("SelectedHotkeys", IsNullable=true)]
		public ObservableCollection<HotkeyEntry> SelectedHotkeys
		{
			get
			{
				return this._selectedHotkeys;
			}
			set
			{
				this._selectedHotkeys = value;
				this.OnPropertyChanged("Hotkeys");
			}
		}

		public Hotkeys()
		{
		}

		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
			if (propertyChangedEventHandler == null)
			{
				return;
			}
			propertyChangedEventHandler(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}