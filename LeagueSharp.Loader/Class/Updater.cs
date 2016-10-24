using LeagueSharp.Loader.Data;
using LeagueSharp.Loader.Views;
using PlaySharp.Service.WebService;
using PlaySharp.Service.WebService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LeagueSharp.Loader.Class
{
	internal class Updater
	{
		public static bool CheckedForUpdates;

		public static string SetupFile;

		public static string UpdateZip;

		public static bool Updating;

		static Updater()
		{
			Updater.CheckedForUpdates = false;
			Updater.SetupFile = Path.Combine(Directories.CurrentDirectory, "LeagueSharp-update.exe");
			Updater.UpdateZip = Path.Combine(Directories.CoreDirectory, "update.zip");
			Updater.Updating = false;
		}

		public Updater()
		{
		}

		public static async Task<bool> IsSupported(string path)
		{
			Updater.<IsSupported>d__6 variable = new Updater.<IsSupported>d__6();
			variable.path = path;
			variable.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			variable.<>1__state = -1;
			variable.<>t__builder.Start<Updater.<IsSupported>d__6>(ref variable);
			return variable.<>t__builder.Task;
		}

		public static async Task<Updater.UpdateResponse> UpdateCore(string path, bool showMessages)
		{
			Updater.<UpdateCore>d__7 variable = new Updater.<UpdateCore>d__7();
			variable.path = path;
			variable.showMessages = showMessages;
			variable.<>t__builder = AsyncTaskMethodBuilder<Updater.UpdateResponse>.Create();
			variable.<>1__state = -1;
			variable.<>t__builder.Start<Updater.<UpdateCore>d__7>(ref variable);
			return variable.<>t__builder.Task;
		}

		public static async Task UpdateLoader(string url)
		{
			try
			{
				Eudyptula.StartKill("Loader update in progress");
				UpdateWindow updateWindow = new UpdateWindow(UpdateAction.Loader, url);
				updateWindow.Show();
				await updateWindow.Update();
			}
			finally
			{
				Eudyptula.StopKill();
			}
		}

		public static async Task UpdateRepositories()
		{
			try
			{
				IReadOnlyList<RepositoryEntry> repositoryEntries = await WebService.Client.RepositoriesAsync(new CancellationToken());
				Config instance = Config.Instance;
				IReadOnlyList<RepositoryEntry> repositoryEntries1 = repositoryEntries;
				instance.KnownRepositories = new ObservableCollection<RepositoryEntry>(
					from r in repositoryEntries1
					where r.get_Display()
					select r);
				Config observableCollection = Config.Instance;
				IReadOnlyList<RepositoryEntry> repositoryEntries2 = repositoryEntries;
				observableCollection.BlockedRepositories = new ObservableCollection<RepositoryEntry>(
					from r in repositoryEntries2
					where r.get_HasRedirect()
					select r);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Utility.Log(LogStatus.Error, "UpdateRepositories", exception.Message, Logs.MainLog);
			}
		}

		public static async Task UpdateWebService()
		{
			try
			{
				ObservableCollection<AssemblyEntry> observableCollection = new ObservableCollection<AssemblyEntry>();
				await Task.Factory.StartNew(() => {
					try
					{
						IReadOnlyList<AssemblyEntry> assemblies = WebService.Assemblies;
						Func<AssemblyEntry, bool> u003cu003e9_101 = Updater.<>c.<>9__10_1;
						if (u003cu003e9_101 == null)
						{
							u003cu003e9_101 = (AssemblyEntry a) => {
								if (!a.get_Approved())
								{
									return false;
								}
								return !a.get_Deleted();
							};
							Updater.<>c.<>9__10_1 = u003cu003e9_101;
						}
						List<AssemblyEntry> entries = assemblies.Where<AssemblyEntry>(u003cu003e9_101).ToList<AssemblyEntry>();
						entries.ShuffleRandom<AssemblyEntry>();
						observableCollection = new ObservableCollection<AssemblyEntry>(entries);
					}
					catch (Exception exception)
					{
						Utility.Log(LogStatus.Error, "UpdateWebService", exception.Message, Logs.MainLog);
					}
				});
				Config.Instance.DatabaseAssemblies = observableCollection;
			}
			catch (Exception exception1)
			{
				Console.WriteLine(exception1);
			}
		}

		public enum CoreUpdateState
		{
			Operational,
			Maintenance,
			Unknown
		}

		public delegate void RepositoriesUpdateDelegate(List<string> list);

		public class UpdateResponse
		{
			public string Message
			{
				get;
				set;
			}

			public Updater.CoreUpdateState State
			{
				get;
				set;
			}

			public UpdateResponse(Updater.CoreUpdateState state, string message = "")
			{
				this.State = state;
				this.Message = message;
			}
		}
	}
}