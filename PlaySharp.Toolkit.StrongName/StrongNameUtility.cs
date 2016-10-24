using LeagueSharp.Loader.Class;
using LeagueSharp.Loader.Data;
using Mono.Security;
using Mono.Security.Cryptography;
using Mono.Security.X509;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace PlaySharp.Toolkit.StrongName
{
	[SecurityCritical]
	public static class StrongNameUtility
	{
		[SecurityCritical]
		public static bool Compare(byte[] value1, byte[] value2)
		{
			if (value1 == null || value2 == null)
			{
				return false;
			}
			bool result = (int)value1.Length == (int)value2.Length;
			if (result)
			{
				for (int i = 0; i < (int)value1.Length; i++)
				{
					if (value1[i] != value2[i])
					{
						return false;
					}
				}
			}
			return result;
		}

		[SecurityCritical]
		public static RSA GetKey(byte[] data, string password = null)
		{
			RSA rSA;
			try
			{
				rSA = (new Mono.Security.StrongName(data)).get_RSA();
			}
			catch
			{
				if (data.Length == 0 || data[0] != 48)
				{
					throw;
				}
				else
				{
					if (password == null)
					{
						throw new ArgumentNullException("password");
					}
					PKCS12 pfx = new PKCS12(data, password);
					if (pfx.get_Keys().Count != 1)
					{
						throw;
					}
					RSA rsa = pfx.get_Keys()[0] as RSA;
					if (rsa == null)
					{
						throw;
					}
					rSA = rsa;
				}
			}
			return rSA;
		}

		[SecurityCritical]
		public static RSA GetKeyFromFile(string filename, string password = null)
		{
			return StrongNameUtility.GetKey(File.ReadAllBytes(filename), password);
		}

		[SecurityCritical]
		public static bool LoadConfig(bool quiet)
		{
			MethodInfo config = typeof(Environment).GetMethod("GetMachineConfigPath", BindingFlags.Static | BindingFlags.NonPublic);
			if (config == null)
			{
				return false;
			}
			string path = (string)config.Invoke(null, null);
			bool exist = File.Exists(path);
			if (!quiet && !exist)
			{
				Console.WriteLine("Couldn't find machine.config");
			}
			StrongNameManager.LoadConfig(path);
			return exist;
		}

		[SecurityCritical]
		public static byte[] ReadFromFile(string fileName)
		{
			byte[] data = null;
			FileStream fs = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			try
			{
				data = new byte[checked((IntPtr)fs.Length)];
				fs.Read(data, 0, (int)data.Length);
			}
			finally
			{
				fs.Close();
			}
			return data;
		}

		[SecurityCritical]
		public static bool ReSign(string assemblyFile, string keyFile, string password = null)
		{
			return StrongNameUtility.ReSign(assemblyFile, StrongNameUtility.GetKeyFromFile(keyFile, password));
		}

		[SecurityCritical]
		public static bool ReSign(string assemblyFile, byte[] resourceKeyFile, string password = null)
		{
			return StrongNameUtility.ReSign(assemblyFile, StrongNameUtility.GetKey(resourceKeyFile, password));
		}

		[SecurityCritical]
		public static bool ReSign(string assemblyName, RSA key)
		{
			if (assemblyName == null)
			{
				throw new ArgumentNullException("assemblyName");
			}
			AssemblyName an = null;
			try
			{
				an = AssemblyName.GetAssemblyName(assemblyName);
			}
			catch
			{
			}
			if (an == null)
			{
				MessageBox.Show(string.Format("Unable to load assembly: {0}", assemblyName));
				return false;
			}
			Mono.Security.StrongName sign = new Mono.Security.StrongName(key);
			byte[] token = an.GetPublicKeyToken();
			bool same = StrongNameUtility.Compare(sign.get_PublicKey(), StrongNameManager.GetMappedPublicKey(token));
			if (!same)
			{
				same = StrongNameUtility.Compare(sign.get_PublicKey(), an.GetPublicKey());
				if (!same)
				{
					same = StrongNameUtility.Compare(sign.get_PublicKeyToken(), token);
				}
			}
			if (!same)
			{
				MessageBox.Show(string.Format("Couldn't sign the assembly {0} with this key pair.", assemblyName));
				return false;
			}
			bool signed = sign.Sign(assemblyName);
			if (!signed)
			{
				MessageBox.Show(string.Format("Couldn't sign the assembly: {0}", assemblyName));
			}
			else
			{
				Utility.Log(LogStatus.Info, "StrongNameUtility", string.Format("Assembly {0} successfully signed.", assemblyName), Logs.MainLog);
			}
			return signed;
		}

		[SecurityCritical]
		public static int SaveConfig()
		{
			return 1;
		}

		[SecurityCritical]
		private static string ToString(byte[] data)
		{
			StringBuilder sb = new StringBuilder();
			int i = 0;
			while (i < (int)data.Length)
			{
				if (i % 39 == 0 && (int)data.Length > 39)
				{
					sb.Append(Environment.NewLine);
				}
				sb.Append(data[i].ToString("x2"));
				if (i <= 2080)
				{
					i++;
				}
				else
				{
					sb.Append(" !!! TOO LONG !!!");
					break;
				}
			}
			return sb.ToString();
		}

		[SecurityCritical]
		public static int Verify(string assemblyName, bool forceVerification = true)
		{
			AssemblyName an = null;
			try
			{
				an = AssemblyName.GetAssemblyName(assemblyName);
			}
			catch
			{
			}
			if (an == null)
			{
				MessageBox.Show(string.Format("Unable to load assembly: {0}", assemblyName));
				return 2;
			}
			byte[] publicKey = StrongNameManager.GetMappedPublicKey(an.GetPublicKeyToken());
			if (publicKey == null || (int)publicKey.Length < 12)
			{
				publicKey = an.GetPublicKey();
				if (publicKey == null || (int)publicKey.Length < 12)
				{
					return 2;
				}
			}
			if (!forceVerification && !StrongNameManager.MustVerify(an))
			{
				MessageBox.Show(string.Format("Assembly {0} is strongnamed (verification skipped).", assemblyName));
				return 0;
			}
			if ((new Mono.Security.StrongName(CryptoConvert.FromCapiPublicKeyBlob(publicKey, 12))).Verify(assemblyName))
			{
				return 0;
			}
			MessageBox.Show(string.Format("Assembly {0} is delay-signed but not strongnamed.", assemblyName));
			return 1;
		}

		[SecurityCritical]
		public static void WriteCSVToFile(string fileName, byte[] data, string mask)
		{
			StreamWriter sw = File.CreateText(fileName);
			try
			{
				for (int i = 0; i < (int)data.Length; i++)
				{
					if (mask[0] == 'X')
					{
						sw.Write("0x");
					}
					sw.Write(data[i].ToString(mask));
					sw.Write(", ");
				}
			}
			finally
			{
				sw.Close();
			}
		}

		[SecurityCritical]
		public static void WriteToFile(string fileName, byte[] data)
		{
			FileStream fs = File.Open(fileName, FileMode.Create, FileAccess.Write);
			try
			{
				fs.Write(data, 0, (int)data.Length);
			}
			finally
			{
				fs.Close();
			}
		}
	}
}