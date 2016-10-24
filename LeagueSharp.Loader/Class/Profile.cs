using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace LeagueSharp.Loader.Class
{
	public class Profile : INotifyPropertyChanged
	{
		private ObservableCollection<LeagueSharpAssembly> _installedAssemblies;

		private string _name;

		public ObservableCollection<LeagueSharpAssembly> InstalledAssemblies
		{
			get
			{
				return this._installedAssemblies;
			}
			set
			{
				this._installedAssemblies = value;
				this.OnPropertyChanged("InstalledAssemblies");
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

		public Profile()
		{
		}

		public void OnPropertyChanged([CallerMemberName] string propertyName = null)
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