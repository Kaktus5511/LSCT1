using LeagueSharp.Loader.Data;
using LeagueSharp.Sandbox.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace LeagueSharp.Loader.Class
{
	public class LoaderService : ILoaderService
	{
		public LoaderService()
		{
		}

		public List<LSharpAssembly> GetAssemblyList(int pid)
		{
			List<LSharpAssembly> assemblies = new List<LSharpAssembly>();
			if (Config.Instance.Settings.GameSettings.First<GameSettings>((GameSettings s) => s.Name == "Always Inject Default Profile").SelectedValue == "True" && Config.Instance.SelectedProfile != Config.Instance.Profiles[0] && Config.Instance.Profiles.Count > 0)
			{
				assemblies.AddRange(Config.Instance.Profiles[0].InstalledAssemblies.Where<LeagueSharpAssembly>((LeagueSharpAssembly a) => {
					if (!a.InjectChecked)
					{
						return false;
					}
					return a.Type != 3;
				}).Select<LeagueSharpAssembly, LSharpAssembly>((LeagueSharpAssembly assembly) => new LSharpAssembly()
				{
					Name = assembly.Name,
					PathToBinary = assembly.PathToBinary
				}).ToList<LSharpAssembly>());
			}
			assemblies.AddRange(Config.Instance.SelectedProfile.InstalledAssemblies.Where<LeagueSharpAssembly>((LeagueSharpAssembly a) => {
				if (!a.InjectChecked)
				{
					return false;
				}
				return a.Type != 3;
			}).Select<LeagueSharpAssembly, LSharpAssembly>((LeagueSharpAssembly assembly) => new LSharpAssembly()
			{
				Name = assembly.Name,
				PathToBinary = assembly.PathToBinary
			}).ToList<LSharpAssembly>());
			return assemblies;
		}

		public Configuration GetConfiguration(int pid)
		{
			int reload = 116;
			int recompile = 119;
			bool console = false;
			bool drawings = true;
			int menuToggle = 120;
			int menuPress = 16;
			string selectedLanguage = string.Empty;
			bool statistics = true;
			try
			{
				reload = KeyInterop.VirtualKeyFromKey(Config.Instance.Hotkeys.SelectedHotkeys.First<HotkeyEntry>((HotkeyEntry h) => h.Name == "Reload").Hotkey);
				recompile = KeyInterop.VirtualKeyFromKey(Config.Instance.Hotkeys.SelectedHotkeys.First<HotkeyEntry>((HotkeyEntry h) => h.Name == "CompileAndReload").Hotkey);
				console = (Config.Instance.Settings.GameSettings.First<GameSettings>((GameSettings s) => s.Name == "Debug Console").SelectedValue == "True" || Config.Instance.ShowDevOptions ? true : Config.Instance.EnableDebug);
				drawings = Config.Instance.Settings.GameSettings.First<GameSettings>((GameSettings s) => s.Name == "Show Drawings").SelectedValue == "True";
				statistics = Config.Instance.Settings.GameSettings.First<GameSettings>((GameSettings s) => s.Name == "Send Anonymous Assembly Statistics").SelectedValue == "True";
				menuToggle = KeyInterop.VirtualKeyFromKey(Config.Instance.Hotkeys.SelectedHotkeys.First<HotkeyEntry>((HotkeyEntry h) => h.Name == "ShowMenuToggle").Hotkey);
				menuPress = KeyInterop.VirtualKeyFromKey(Config.Instance.Hotkeys.SelectedHotkeys.First<HotkeyEntry>((HotkeyEntry h) => h.Name == "ShowMenuPress").Hotkey);
				selectedLanguage = Config.Instance.SelectedLanguage;
			}
			catch
			{
			}
			return new Configuration()
			{
				DataDirectory = Directories.AppDataDirectory,
				LeagueSharpDllPath = PathRandomizer.LeagueSharpDllPath,
				LibrariesDirectory = Directories.CoreDirectory,
				ReloadKey = reload,
				ReloadAndRecompileKey = recompile,
				MenuToggleKey = menuToggle,
				MenuKey = menuPress,
				UnloadKey = 117,
				Console = console,
				SelectedLanguage = selectedLanguage,
				ShowDrawing = drawings,
				SendStatistics = statistics,
				Permissions = null
			};
		}

		public void Recompile(int pid)
		{
			List<LeagueSharpAssembly> targetAssemblies = Config.Instance.SelectedProfile.InstalledAssemblies.Where<LeagueSharpAssembly>((LeagueSharpAssembly a) => {
				if (a.InjectChecked)
				{
					return true;
				}
				return a.Type == 3;
			}).ToList<LeagueSharpAssembly>();
			foreach (LeagueSharpAssembly assembly in targetAssemblies)
			{
				if (assembly.Type != 3)
				{
					continue;
				}
				assembly.Compile();
			}
			foreach (LeagueSharpAssembly assembly in targetAssemblies)
			{
				if (assembly.Type == 3)
				{
					continue;
				}
				assembly.Compile();
			}
		}
	}
}