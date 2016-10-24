using LeagueSharp.Loader.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace LeagueSharp.Loader.Class
{
	public class Utility
	{
		public Utility()
		{
		}

		public static void ClearDirectory(string directory)
		{
			try
			{
				DirectoryInfo dir = new DirectoryInfo(directory);
				FileInfo[] files = dir.GetFiles();
				for (int i = 0; i < (int)files.Length; i++)
				{
					FileInfo fi = files[i];
					try
					{
						fi.Attributes = FileAttributes.Normal;
						fi.Delete();
					}
					catch
					{
					}
				}
				DirectoryInfo[] directories = dir.GetDirectories();
				for (int j = 0; j < (int)directories.Length; j++)
				{
					DirectoryInfo di = directories[j];
					try
					{
						di.Attributes = FileAttributes.Normal;
						Utility.ClearDirectory(di.FullName);
						di.Delete();
					}
					catch
					{
					}
				}
			}
			catch
			{
			}
		}

		public static void CopyDirectory(string sourceDirName, string destDirName, bool copySubDirs = false, bool overrideFiles = false)
		{
			try
			{
				DirectoryInfo dir = new DirectoryInfo(sourceDirName)
				{
					Attributes = FileAttributes.Directory
				};
				DirectoryInfo[] dirs = dir.GetDirectories();
				if (!dir.Exists)
				{
					throw new DirectoryNotFoundException(string.Concat("Source directory does not exist or could not be found: ", sourceDirName));
				}
				if (!Directory.Exists(destDirName))
				{
					Directory.CreateDirectory(destDirName);
				}
				FileInfo[] files = dir.GetFiles();
				for (int i = 0; i < (int)files.Length; i++)
				{
					FileInfo file = files[i];
					string temppath = Path.Combine(destDirName, file.Name);
					file.Attributes = FileAttributes.Normal;
					file.CopyTo(temppath, overrideFiles);
				}
				if (copySubDirs)
				{
					DirectoryInfo[] directoryInfoArray = dirs;
					for (int j = 0; j < (int)directoryInfoArray.Length; j++)
					{
						DirectoryInfo subdir = directoryInfoArray[j];
						string temppath = Path.Combine(destDirName, subdir.Name);
						Utility.CopyDirectory(subdir.FullName, temppath, true, overrideFiles);
					}
				}
			}
			catch
			{
			}
		}

		public static LeagueSharpAssembly CreateEmptyAssembly(string assemblyName)
		{
			LeagueSharpAssembly leagueSharpAssembly;
			try
			{
				string appconfig = Utility.ReadResourceString("LeagueSharp.Loader.Resources.DefaultProject.App.config");
				string assemblyInfocs = Utility.ReadResourceString("LeagueSharp.Loader.Resources.DefaultProject.AssemblyInfo.cs");
				string defaultProjectcsproj = Utility.ReadResourceString("LeagueSharp.Loader.Resources.DefaultProject.DefaultProject.csproj");
				string programcs = Utility.ReadResourceString("LeagueSharp.Loader.Resources.DefaultProject.Program.cs");
				string localRepoDir = Directories.LocalRepoDir;
				int hashCode = Environment.TickCount.GetHashCode();
				string targetPath = Path.Combine(localRepoDir, string.Concat(assemblyName, hashCode.ToString("X")));
				Directory.CreateDirectory(targetPath);
				programcs = programcs.Replace("{ProjectName}", assemblyName);
				assemblyInfocs = assemblyInfocs.Replace("{ProjectName}", assemblyName);
				defaultProjectcsproj = defaultProjectcsproj.Replace("{ProjectName}", assemblyName);
				defaultProjectcsproj = defaultProjectcsproj.Replace("{SystemDirectory}", Directories.CoreDirectory);
				File.WriteAllText(Path.Combine(targetPath, "App.config"), appconfig);
				File.WriteAllText(Path.Combine(targetPath, "AssemblyInfo.cs"), assemblyInfocs);
				File.WriteAllText(Path.Combine(targetPath, string.Concat(assemblyName, ".csproj")), defaultProjectcsproj);
				File.WriteAllText(Path.Combine(targetPath, "Program.cs"), programcs);
				leagueSharpAssembly = new LeagueSharpAssembly(assemblyName, Path.Combine(targetPath, string.Concat(assemblyName, ".csproj")), string.Empty);
			}
			catch (Exception exception)
			{
				System.Windows.Forms.MessageBox.Show(exception.ToString());
				leagueSharpAssembly = null;
			}
			return leagueSharpAssembly;
		}

		public static void CreateFileFromResource(string path, string resource, bool overwrite = false)
		{
			if (!overwrite && File.Exists(path))
			{
				return;
			}
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
			{
				if (stream != null)
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
						{
							sw.Write(reader.ReadToEnd());
						}
					}
				}
			}
		}

		public static string GetLatestLeagueOfLegendsExePath(string lastKnownPath)
		{
			string str;
			Version version;
			string str1;
			try
			{
				if (!lastKnownPath.EndsWith("Game\\League of Legends.exe"))
				{
					string dir = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(lastKnownPath), "..\\..\\"));
					if (Directory.Exists(dir))
					{
						string[] versionPaths = Directory.GetDirectories(dir);
						string greatestVersionString = string.Empty;
						long greatestVersion = (long)0;
						string[] strArrays = versionPaths;
						for (int i = 0; i < (int)strArrays.Length; i++)
						{
							string versionPath = strArrays[i];
							if (Version.TryParse(Path.GetFileName(versionPath), out version))
							{
								double test = (double)version.Build * Math.Pow(600, 4) + (double)version.Major * Math.Pow(600, 3) + (double)version.Minor * Math.Pow(600, 2) + (double)version.Revision * Math.Pow(600, 1);
								if (test > (double)greatestVersion)
								{
									greatestVersion = (long)test;
									greatestVersionString = Path.GetFileName(versionPath);
								}
							}
						}
						if (greatestVersion != 0)
						{
							string[] exe = Directory.GetFiles(Path.Combine(dir, greatestVersionString), "League of Legends.exe", SearchOption.AllDirectories);
							if (exe.Length != 0)
							{
								str1 = exe[0];
							}
							else
							{
								str1 = null;
							}
							str = str1;
							return str;
						}
					}
					return null;
				}
				else
				{
					str = lastKnownPath;
				}
			}
			catch (Exception exception)
			{
				return null;
			}
			return str;
		}

		public static string GetMultiLanguageText(string key)
		{
			return System.Windows.Application.Current.FindResource(key).ToString();
		}

		public static string GetUniqueKey(int maxSize)
		{
			char[] chars = new char[62];
			chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
			byte[] data = new byte[1];
			using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
			{
				crypto.GetNonZeroBytes(data);
				data = new byte[maxSize];
				crypto.GetNonZeroBytes(data);
			}
			StringBuilder result = new StringBuilder(maxSize);
			byte[] numArray = data;
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				byte b = numArray[i];
				result.Append(chars[b % (int)chars.Length]);
			}
			return result.ToString();
		}

		public static void Log(string status, string source, string message, Log log)
		{
			if (System.Windows.Application.Current == null)
			{
				return;
			}
			System.Windows.Application.Current.Dispatcher.Invoke(() => log.Items.Add(new LogItem()
			{
				Status = status,
				Source = source,
				Message = message
			}));
		}

		public static string MakeValidFileName(string name)
		{
			string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
			string invalidRegStr = string.Format("([{0}]*\\.+$)|([{0}]+)", invalidChars);
			return Regex.Replace(name, invalidRegStr, "_");
		}

		public static void MapClassToXmlFile(Type type, object obj, string path)
		{
			XmlSerializer serializer = new XmlSerializer(type);
			using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
			{
				serializer.Serialize(sw, obj);
			}
		}

		public static object MapXmlFileToClass(Type type, string path)
		{
			object obj;
			XmlSerializer serializer = new XmlSerializer(type);
			using (StreamReader reader = new StreamReader(path, Encoding.UTF8))
			{
				obj = serializer.Deserialize(reader);
			}
			return obj;
		}

		public static string Md5Checksum(string filePath)
		{
			string lower;
			if (filePath == null)
			{
				throw new ArgumentNullException("filePath");
			}
			try
			{
				if (File.Exists(filePath))
				{
					using (MD5 md5 = MD5.Create())
					{
						using (FileStream stream = File.OpenRead(filePath))
						{
							lower = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
						}
					}
				}
				else
				{
					lower = "-1";
				}
			}
			catch (Exception exception)
			{
				lower = "-1";
			}
			return lower;
		}

		public static string Md5Hash(string s)
		{
			StringBuilder sb = new StringBuilder();
			HashAlgorithm algorithm = MD5.Create();
			byte[] h = algorithm.ComputeHash(Encoding.Default.GetBytes(s));
			byte[] numArray = h;
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				byte b = numArray[i];
				sb.Append(b.ToString("x2"));
			}
			return sb.ToString();
		}

		public static bool OverwriteFile(string file, string path, bool copy = false)
		{
			bool flag;
			try
			{
				string dir = Path.GetDirectoryName(path);
				if (dir != null && !Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}
				if (File.Exists(path))
				{
					File.Delete(path);
				}
				try
				{
					if (!copy)
					{
						File.Move(file, path);
					}
					else
					{
						File.Copy(file, path);
					}
				}
				catch (Exception exception)
				{
					System.Windows.Forms.MessageBox.Show(exception.ToString());
					throw;
				}
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		public static string ReadResourceString(string resource)
		{
			string end;
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
			{
				if (stream == null)
				{
					return string.Empty;
				}
				else
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						end = reader.ReadToEnd();
					}
				}
			}
			return end;
		}

		public static bool RenameFileIfExists(string file, string path)
		{
			bool flag;
			try
			{
				int counter = 1;
				string fileName = Path.GetFileNameWithoutExtension(file);
				string fileExtension = Path.GetExtension(file);
				string newPath = path;
				string pathDirectory = Path.GetDirectoryName(path);
				if (pathDirectory == null)
				{
					return false;
				}
				else
				{
					if (!Directory.Exists(pathDirectory))
					{
						Directory.CreateDirectory(pathDirectory);
					}
					while (File.Exists(newPath))
					{
						int num = counter;
						counter = num + 1;
						string tmpFileName = string.Format("{0} ({1})", fileName, num);
						newPath = Path.Combine(pathDirectory, string.Concat(tmpFileName, fileExtension));
					}
					File.Move(file, newPath);
					flag = true;
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		internal static byte[] ReplaceFilling(byte[] input, byte[] pattern, byte[] replacement)
		{
			int i;
			if (pattern.Length == 0)
			{
				return input;
			}
			List<byte> result = new List<byte>();
			for (i = 0; i <= (int)input.Length - (int)pattern.Length; i++)
			{
				bool foundMatch = true;
				int j = 0;
				while (j < (int)pattern.Length)
				{
					if (input[i + j] == pattern[j])
					{
						j++;
					}
					else
					{
						foundMatch = false;
						break;
					}
				}
				if (!foundMatch)
				{
					result.Add(input[i]);
				}
				else
				{
					result.AddRange(replacement);
					for (int k = 0; k < (int)pattern.Length - (int)replacement.Length; k++)
					{
						result.Add(0);
					}
					i = i + ((int)pattern.Length - 1);
				}
			}
			while (i < (int)input.Length)
			{
				result.Add(input[i]);
				i++;
			}
			return result.ToArray();
		}

		public static int VersionToInt(Version version)
		{
			return version.Major * 10000000 + version.Minor * 10000 + version.Build * 100 + version.Revision;
		}

		public static string WildcardToRegex(string pattern)
		{
			return string.Concat("^", Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", "."), "$");
		}
	}
}