using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;

namespace LeagueSharp.Sandbox.Shared
{
	[DataContract]
	public class Configuration
	{
		[DataMember]
		public bool AntiAfk
		{
			get;
			set;
		}

		[DataMember]
		public bool Console
		{
			get;
			set;
		}

		[DataMember]
		public string DataDirectory
		{
			get;
			set;
		}

		[DataMember]
		public bool ExtendedZoom
		{
			get;
			set;
		}

		[DataMember]
		public string LeagueSharpDllPath
		{
			get;
			set;
		}

		[DataMember]
		public string LibrariesDirectory
		{
			get;
			set;
		}

		[DataMember]
		public int MenuKey
		{
			get;
			set;
		}

		[DataMember]
		public int MenuToggleKey
		{
			get;
			set;
		}

		[DataMember]
		public PermissionSet Permissions
		{
			get;
			set;
		}

		[DataMember]
		public int ReloadAndRecompileKey
		{
			get;
			set;
		}

		[DataMember]
		public int ReloadKey
		{
			get;
			set;
		}

		[DataMember]
		public string SelectedLanguage
		{
			get;
			set;
		}

		[DataMember]
		public bool SendStatistics
		{
			get;
			set;
		}

		[DataMember]
		public bool ShowDrawing
		{
			get;
			set;
		}

		[DataMember]
		public bool ShowPing
		{
			get;
			set;
		}

		[DataMember]
		public bool TowerRange
		{
			get;
			set;
		}

		[DataMember]
		public int UnloadKey
		{
			get;
			set;
		}

		public Configuration()
		{
		}

		public override string ToString()
		{
			return string.Format("DataDirectory:{0}\nLeagueSharpDllPath:{1}\nLibrariesDirectory:{2}\nMenuKey:{3}\nMenuToggleKey:{4}\nAntiAfk:{5}\nConsole:{6}\nExtendedZoom:{7}\nTowerRange:{8}\nReloadKey:{9}\nReloadAndRecompileKey:{10}\nSelectedLanguage:{11}\nUnloadKey:{12}\n", new object[] { this.DataDirectory, this.LeagueSharpDllPath, this.LibrariesDirectory, this.MenuKey, this.MenuToggleKey, this.AntiAfk, this.Console, this.ExtendedZoom, this.TowerRange, this.ReloadKey, this.ReloadAndRecompileKey, this.SelectedLanguage, this.UnloadKey });
		}
	}
}