using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;

namespace LeagueSharp.Loader.Data
{
	[XmlType(AnonymousType=true)]
	public class ConfigSettings : INotifyPropertyChanged
	{
		private ObservableCollection<LeagueSharp.Loader.Data.GameSettings> _gameSettings;

		[XmlArrayItem("GameSettings", IsNullable=true)]
		public ObservableCollection<LeagueSharp.Loader.Data.GameSettings> GameSettings
		{
			get
			{
				return this._gameSettings;
			}
			set
			{
				this._gameSettings = value;
				this.OnPropertyChanged("GameSettings");
			}
		}

		public ConfigSettings()
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