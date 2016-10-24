using LeagueSharp.Loader.Data;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LeagueSharp.Loader.Class
{
	internal class GitUpdater
	{
		public GitUpdater()
		{
		}

		public static void ClearUnusedRepoFolder(string repoDirectory, Log log)
		{
			try
			{
				string dir = repoDirectory.Remove(repoDirectory.LastIndexOf("\\"));
				if (!dir.EndsWith("trunk"))
				{
					if (Directory.Exists(dir))
					{
						Directory.Delete(dir, true);
					}
					dir = repoDirectory.Remove(dir.LastIndexOf("\\"));
					Directory.GetFiles(dir).ToList<string>().ForEach(new Action<string>(File.Delete));
				}
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				Utility.Log(LogStatus.Error, "Clear Unused", string.Format("{0} - {1}", ex.Message, repoDirectory), log);
			}
		}

		public static void ClearUnusedRepos(List<LeagueSharpAssembly> assemblyList)
		{
			try
			{
				List<string> usedRepos = new List<string>();
				foreach (LeagueSharpAssembly assembly in 
					from a in assemblyList
					where !string.IsNullOrEmpty(a.SvnUrl)
					select a)
				{
					int hashCode = assembly.SvnUrl.GetHashCode();
					usedRepos.Add(hashCode.ToString("X"));
				}
				foreach (string dir in new List<string>(Directory.EnumerateDirectories(Directories.RepositoryDir)))
				{
					if (usedRepos.Contains(Path.GetFileName(dir)))
					{
						continue;
					}
					Utility.ClearDirectory(dir);
					Directory.Delete(dir);
				}
			}
			catch (Exception exception)
			{
				Exception ex = exception;
				Utility.Log(LogStatus.Error, "Clear Unused", ex.Message, Logs.MainLog);
			}
		}

		private static bool Clone(string url, string directory)
		{
			bool flag;
			try
			{
				if (Directory.Exists(directory))
				{
					Utility.ClearDirectory(directory);
					Directory.Delete(directory, true);
				}
				Utility.Log(LogStatus.Info, "Clone", url, Logs.MainLog);
				Repository.Clone(url, directory);
				flag = true;
			}
			catch (Exception exception)
			{
				Exception e = exception;
				Utility.Log(LogStatus.Error, "Clone", e.Message, Logs.MainLog);
				flag = false;
			}
			return flag;
		}

		private static bool IsValid(string directory)
		{
			bool flag;
			try
			{
				if (!Repository.IsValid(directory))
				{
					flag = false;
				}
				else
				{
					using (Repository repo = new Repository(directory))
					{
						if (repo.get_Head() == null)
						{
							flag = false;
							return flag;
						}
						else if (repo.get_Info().get_IsHeadDetached())
						{
							flag = false;
							return flag;
						}
						else if (repo.get_Info().get_IsBare())
						{
							flag = false;
							return flag;
						}
					}
					return true;
				}
			}
			catch (Exception exception)
			{
				Exception e = exception;
				Utility.Log(LogStatus.Error, "IsValid", e.Message, Logs.MainLog);
				flag = false;
			}
			return flag;
		}

		private static bool Pull(string directory)
		{
			bool flag;
			try
			{
				using (Repository repo = new Repository(directory))
				{
					Utility.Log(LogStatus.Info, "Pull", directory, Logs.MainLog);
					RepositoryExtensions.Reset(repo, 3);
					repo.RemoveUntrackedFiles();
					Network network = repo.get_Network();
					LibGit2Sharp.Signature signature = new LibGit2Sharp.Signature(Config.Instance.Username, string.Format("{0}@joduska.me", Config.Instance.Username), DateTimeOffset.Now);
					PullOptions pullOption = new PullOptions();
					MergeOptions mergeOption = new MergeOptions();
					mergeOption.set_FastForwardStrategy(0);
					mergeOption.set_FileConflictStrategy(2);
					mergeOption.set_MergeFileFavor(2);
					mergeOption.set_CommitOnSuccess(true);
					pullOption.set_MergeOptions(mergeOption);
					network.Pull(signature, pullOption);
					Branch head = repo.get_Head();
					CheckoutOptions checkoutOption = new CheckoutOptions();
					checkoutOption.set_CheckoutModifiers(1);
					repo.Checkout(head, checkoutOption);
					if (repo.get_Info().get_IsHeadDetached())
					{
						Utility.Log(LogStatus.Error, "Pull", "Update+Detached", Logs.MainLog);
					}
				}
				flag = true;
			}
			catch (Exception exception)
			{
				Exception e = exception;
				Utility.Log(LogStatus.Error, "Pull", e.Message, Logs.MainLog);
				flag = false;
			}
			return flag;
		}

		internal static string Update(string url)
		{
			string repositoryDir = Directories.RepositoryDir;
			int hashCode = url.GetHashCode();
			string root = Path.Combine(repositoryDir, hashCode.ToString("X"), "trunk");
			if (!GitUpdater.IsValid(root) && !GitUpdater.Clone(url, root))
			{
				Utility.Log(LogStatus.Error, "Updater", string.Format("Failed to Clone - {0}", url), Logs.MainLog);
				return root;
			}
			if (!GitUpdater.Pull(root))
			{
				Utility.Log(LogStatus.Error, "Updater", string.Format("Failed to Pull Updates - {0}", url), Logs.MainLog);
				GitUpdater.Clone(url, root);
			}
			return root;
		}
	}
}