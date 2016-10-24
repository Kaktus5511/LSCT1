using LeagueSharp.Loader.Class;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Input;

namespace LeagueSharp.Loader.Data
{
	public class HotkeyEntry : INotifyPropertyChanged
	{
		private Key _hotkey;

		private string _name;

		public Key DefaultKey
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string DisplayDescription
		{
			get
			{
				return Utility.GetMultiLanguageText(this.Description);
			}
		}

		public Key Hotkey
		{
			get
			{
				return this._hotkey;
			}
			set
			{
				this._hotkey = value;
				this.OnPropertyChanged("Hotkey");
				this.OnPropertyChanged("HotkeyString");
			}
		}

		public byte HotkeyInt
		{
			get
			{
				if (this.Hotkey == Key.LeftShift || this.Hotkey == Key.RightShift)
				{
					return (byte)16;
				}
				if (this.Hotkey == Key.LeftAlt || this.Hotkey == Key.RightAlt)
				{
					return (byte)18;
				}
				if (this.Hotkey == Key.LeftCtrl || this.Hotkey == Key.RightCtrl)
				{
					return (byte)17;
				}
				return (byte)KeyInterop.VirtualKeyFromKey(this.Hotkey);
			}
			set
			{
			}
		}

		public string HotkeyString
		{
			get
			{
				return this._hotkey.ToString();
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

		public HotkeyEntry()
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