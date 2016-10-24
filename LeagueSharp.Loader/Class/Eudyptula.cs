using LeagueSharp.Loader.Data;
using LeagueSharp.Loader.Views;
using PlaySharp.Service.WebService;
using PlaySharp.Service.WebService.Model;
using PlaySharp.Toolkit.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LeagueSharp.Loader.Class
{
	public static class Eudyptula
	{
		private static IntPtr bootstrapper;

		private static Eudyptula.GetFilePathDelegate getFilePath;

		private static Eudyptula.HasModuleDelegate hasModule;

		private static Eudyptula.InjectDLLDelegate injectDLL;

		private static MemoryMappedFile mmf;

		public static bool IsInjected
		{
			get
			{
				return ((IEnumerable<Process>)Process.GetProcessesByName("League of Legends")).Any<Process>((Process process) => {
					bool id;
					try
					{
						id = Eudyptula.hasModule(process.Id, PathRandomizer.LeagueSharpCoreDllName);
					}
					catch (Exception exception)
					{
						id = false;
					}
					return id;
				});
			}
		}

		public static string[] KilledInstance
		{
			get;
			set;
		}

		public static System.Threading.Thread KillThread
		{
			get;
			set;
		}

		public static System.Threading.Thread Thread
		{
			get;
			set;
		}

		public static void Feelsbadman(string filePath)
		{
			try
			{
				using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
				{
					Guid guid = Guid.NewGuid();
					int time = (new Random(guid.GetHashCode())).Next(1451602800, DateTime.UtcNow.ToUnixTimestamp());
					BinaryReader reader = new BinaryReader(fs);
					BinaryWriter writer = new BinaryWriter(fs);
					fs.Seek((long)60, SeekOrigin.Begin);
					fs.Seek((long)reader.ReadInt32(), SeekOrigin.Begin);
					fs.Seek((long)8, SeekOrigin.Current);
					writer.Write(time);
				}
			}
			catch
			{
			}
		}

		private static string[] GetCommandline(this Process process)
		{
			try
			{
				string query = string.Format("SELECT CommandLine FROM Win32_Process WHERE ProcessId = {0}", process.Id);
				using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
				{
					using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = searcher.Get().GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							string cmd = enumerator.Current["CommandLine"].ToString();
							int fileEnd = cmd.IndexOf("\"", 1);
							string file = cmd.Substring(1, fileEnd - 1);
							string args = cmd.Substring(fileEnd + 2);
							return new string[] { file, args };
						}
					}
				}
			}
			catch (Exception exception)
			{
				Exception e = exception;
				Utility.Log(LogStatus.Error, "GetCommandline", e.Message, Logs.MainLog);
			}
			return null;
		}

		private static string GetFilePath(Process process)
		{
			StringBuilder sb = new StringBuilder(255);
			Eudyptula.getFilePath(process.Id, sb, sb.Capacity);
			return sb.ToString();
		}

		private static string GetModuleFilePath(Process process)
		{
			string str;
			List<Func<string>>.Enumerator enumerator = (new List<Func<string>>()
			{
				new Func<string>(() => Eudyptula.GetFilePath(process)),
				new Func<string>(() => process.MainModule.FileName),
				new Func<string>(() => Config.Instance.LeagueOfLegendsExePath)
			}).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Func<string> operation = enumerator.Current;
					try
					{
						string value = operation();
						if (!string.IsNullOrEmpty(value) && File.Exists(value))
						{
							str = value;
							return str;
						}
					}
					catch (Exception exception)
					{
						Exception e = exception;
						Utility.Log(LogStatus.Error, "GetModuleFilePath", e.Message, Logs.MainLog);
					}
				}
				return null;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return str;
		}

		private static void KillThemAll(string reason)
		{
			Action action = null;
			try
			{
				try
				{
					while (true)
					{
						bool killed = false;
						Process[] processesByName = Process.GetProcessesByName("League of Legends");
						for (int i = 0; i < (int)processesByName.Length; i++)
						{
							Process process = processesByName[i];
							try
							{
								Eudyptula.KilledInstance = process.GetCommandline();
								process.Kill();
								killed = true;
								Console.Beep();
								Console.Beep();
								Console.Beep();
							}
							catch (Exception exception)
							{
								Exception e = exception;
								Utility.Log(LogStatus.Info, "KillThemAll", e.Message, Logs.MainLog);
							}
						}
						if (killed)
						{
							TaskFactory factory = Task.Factory;
							Action action1 = action;
							if (action1 == null)
							{
								Action action2 = () => MessageBox.Show(string.Format("Killed League of Legends.exe\n\nReason: {0}", reason), "LeagueSharp", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
								Action action3 = action2;
								action = action2;
								action1 = action3;
							}
							factory.StartNew(action1);
						}
						System.Threading.Thread.Sleep(250);
					}
				}
				catch (ThreadAbortException threadAbortException)
				{
				}
			}
			finally
			{
				if (Eudyptula.KilledInstance != null)
				{
					Eudyptula.StartProcess(Eudyptula.KilledInstance);
				}
			}
		}

		private static void ResolveInjectDLL()
		{
			Eudyptula.mmf = MemoryMappedFile.CreateOrOpen("Local\\LeagueSharpBootstrap", (long)520, MemoryMappedFileAccess.ReadWrite);
			Injection.SharedMemoryLayout sharedMem = new Injection.SharedMemoryLayout(PathRandomizer.LeagueSharpSandBoxDllPath, PathRandomizer.LeagueSharpBootstrapDllPath, Config.Instance.Username, Config.Instance.Password);
			using (MemoryMappedViewAccessor writer = Eudyptula.mmf.CreateViewAccessor())
			{
				int len = Marshal.SizeOf(typeof(Injection.SharedMemoryLayout));
				byte[] arr = new byte[len];
				IntPtr ptr = Marshal.AllocHGlobal(len);
				Marshal.StructureToPtr(sharedMem, ptr, true);
				Marshal.Copy(ptr, arr, 0, len);
				Marshal.FreeHGlobal(ptr);
				writer.WriteArray<byte>((long)0, arr, 0, (int)arr.Length);
			}
			Eudyptula.bootstrapper = Win32Imports.LoadLibrary(Directories.BootstrapFilePath);
			if (Eudyptula.bootstrapper == IntPtr.Zero)
			{
				return;
			}
			IntPtr procAddress = Win32Imports.GetProcAddress(Eudyptula.bootstrapper, "InjectModule");
			if (procAddress == IntPtr.Zero)
			{
				return;
			}
			Eudyptula.injectDLL = Marshal.GetDelegateForFunctionPointer(procAddress, typeof(Eudyptula.InjectDLLDelegate)) as Eudyptula.InjectDLLDelegate;
			procAddress = Win32Imports.GetProcAddress(Eudyptula.bootstrapper, "HasModule");
			if (procAddress == IntPtr.Zero)
			{
				return;
			}
			Eudyptula.hasModule = Marshal.GetDelegateForFunctionPointer(procAddress, typeof(Eudyptula.HasModuleDelegate)) as Eudyptula.HasModuleDelegate;
			procAddress = Win32Imports.GetProcAddress(Eudyptula.bootstrapper, "GetFilePath");
			if (procAddress == IntPtr.Zero)
			{
				return;
			}
			Eudyptula.getFilePath = Marshal.GetDelegateForFunctionPointer(procAddress, typeof(Eudyptula.GetFilePathDelegate)) as Eudyptula.GetFilePathDelegate;
		}

		private static async void Run()
		{
			Eudyptula.<Run>d__32 variable = new Eudyptula.<Run>d__32();
			variable.<>t__builder = AsyncVoidMethodBuilder.Create();
			variable.<>1__state = -1;
			variable.<>t__builder.Start<Eudyptula.<Run>d__32>(ref variable);
		}

		public static void Start()
		{
			if (Eudyptula.Thread != null)
			{
				return;
			}
			Eudyptula.Thread = new System.Threading.Thread(new ThreadStart(Eudyptula.Run));
			Eudyptula.Thread.SetApartmentState(ApartmentState.STA);
			Eudyptula.Thread.IsBackground = true;
			Eudyptula.Thread.Start();
		}

		public static void StartKill(string reason)
		{
			if (Eudyptula.KillThread != null)
			{
				return;
			}
			Eudyptula.KillThread = new System.Threading.Thread(() => Eudyptula.KillThemAll(reason));
			Eudyptula.KillThread.SetApartmentState(ApartmentState.STA);
			Eudyptula.KillThread.IsBackground = true;
			Eudyptula.KillThread.Start();
		}

		private static void StartProcess(string[] args)
		{
			ProcessStartInfo info = new ProcessStartInfo(args[0])
			{
				WorkingDirectory = Path.GetDirectoryName(args[0]),
				Arguments = args[1]
			};
			Process.Start(info);
		}

		public static void Stop()
		{
			System.Threading.Thread thread = Eudyptula.Thread;
			if (thread != null)
			{
				thread.Abort();
			}
			else
			{
			}
			Eudyptula.Thread = null;
		}

		public static void StopKill()
		{
			System.Threading.Thread killThread = Eudyptula.KillThread;
			if (killThread != null)
			{
				killThread.Abort();
			}
			else
			{
			}
			Eudyptula.KillThread = null;
		}

		public static void Unload()
		{
			if (Eudyptula.bootstrapper != IntPtr.Zero)
			{
				try
				{
					Win32Imports.FreeLibrary(Eudyptula.bootstrapper);
				}
				catch
				{
				}
			}
		}

		public static async Task<CoreUpdateResult> Update(Process process)
		{
			Eudyptula.<Update>d__27 variable = new Eudyptula.<Update>d__27();
			variable.process = process;
			variable.<>t__builder = AsyncTaskMethodBuilder<CoreUpdateResult>.Create();
			variable.<>1__state = -1;
			variable.<>t__builder.Start<Eudyptula.<Update>d__27>(ref variable);
			return variable.<>t__builder.Task;
		}

		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet=CharSet.Unicode)]
		private delegate bool GetFilePathDelegate(int processId, [Out] StringBuilder path, int size);

		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet=CharSet.Unicode)]
		private delegate bool HasModuleDelegate(int processId, string path);

		[UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet=CharSet.Unicode)]
		private delegate bool InjectDLLDelegate(int processId, string path);
	}
}