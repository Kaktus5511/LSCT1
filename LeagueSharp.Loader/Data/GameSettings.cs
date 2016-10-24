using LeagueSharp.Loader.Class;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Xml.Serialization;

namespace LeagueSharp.Loader.Data
{
	public class GameSettings : INotifyPropertyChanged
	{
		private string _name;

		private List<string> _posibleValues;

		private string _selectedValue;

		[JsonIgnore]
		[XmlIgnore]
		public string DisplayName
		{
			get
			{
				return Utility.GetMultiLanguageText(this._name);
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = value;
				this.OnPropertyChanged("Name");
			}
		}

		public List<string> PosibleValues
		{
			get
			{
				return this._posibleValues;
			}
			set
			{
				this._posibleValues = value;
				this.OnPropertyChanged("PosibleValues");
			}
		}

		public string SelectedValue
		{
			get
			{
				return this._selectedValue;
			}
			set
			{
				this._selectedValue = value;
				this.OnPropertyChanged("SelectedValue");
			}
		}

		public GameSettings()
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