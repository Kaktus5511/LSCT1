using Mono.Security.Cryptography;
using Mono.Xml;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;

namespace PlaySharp.Toolkit.StrongName
{
	internal class StrongNameManager
	{
		private static Hashtable mappings;

		private static Hashtable tokens;

		static StrongNameManager()
		{
		}

		public StrongNameManager()
		{
		}

		public static byte[] GetMappedPublicKey(byte[] token)
		{
			if (StrongNameManager.mappings == null || token == null)
			{
				return null;
			}
			string t = CryptoConvert.ToHex(token);
			string pk = (string)StrongNameManager.mappings[t];
			if (pk == null)
			{
				return null;
			}
			return CryptoConvert.FromHex(pk);
		}

		public static void LoadConfig(string filename)
		{
			if (File.Exists(filename))
			{
				SecurityParser sp = new SecurityParser();
				using (StreamReader sr = new StreamReader(filename))
				{
					sp.LoadXml(sr.ReadToEnd());
				}
				SecurityElement root = sp.ToXml();
				if (root != null && root.Tag == "configuration")
				{
					SecurityElement strongnames = root.SearchForChildByTag("strongNames");
					if (strongnames != null && strongnames.Children.Count > 0)
					{
						SecurityElement mapping = strongnames.SearchForChildByTag("pubTokenMapping");
						if (mapping != null && mapping.Children.Count > 0)
						{
							StrongNameManager.LoadMapping(mapping);
						}
						SecurityElement settings = strongnames.SearchForChildByTag("verificationSettings");
						if (settings != null && settings.Children.Count > 0)
						{
							StrongNameManager.LoadVerificationSettings(settings);
						}
					}
				}
			}
		}

		private static void LoadMapping(SecurityElement mapping)
		{
			if (StrongNameManager.mappings == null)
			{
				StrongNameManager.mappings = new Hashtable();
			}
			lock (StrongNameManager.mappings.SyncRoot)
			{
				foreach (SecurityElement item in mapping.Children)
				{
					if (item.Tag != "map")
					{
						continue;
					}
					string token = item.Attribute("Token");
					if (token == null || token.Length != 16)
					{
						continue;
					}
					token = token.ToUpper(CultureInfo.InvariantCulture);
					string publicKey = item.Attribute("PublicKey");
					if (publicKey == null)
					{
						continue;
					}
					if (StrongNameManager.mappings[token] != null)
					{
						StrongNameManager.mappings[token] = publicKey;
					}
					else
					{
						StrongNameManager.mappings.Add(token, publicKey);
					}
				}
			}
		}

		private static void LoadVerificationSettings(SecurityElement settings)
		{
			if (StrongNameManager.tokens == null)
			{
				StrongNameManager.tokens = new Hashtable();
			}
			lock (StrongNameManager.tokens.SyncRoot)
			{
				foreach (SecurityElement item in settings.Children)
				{
					if (item.Tag != "skip")
					{
						continue;
					}
					string token = item.Attribute("Token");
					if (token == null)
					{
						continue;
					}
					token = token.ToUpper(CultureInfo.InvariantCulture);
					string assembly = item.Attribute("Assembly") ?? "*";
					string users = item.Attribute("Users") ?? "*";
					StrongNameManager.Element el = (StrongNameManager.Element)StrongNameManager.tokens[token];
					if (el == null)
					{
						el = new StrongNameManager.Element(assembly, users);
						StrongNameManager.tokens.Add(token, el);
					}
					else if ((string)el.assemblies[assembly] == null)
					{
						el.assemblies.Add(assembly, users);
					}
					else if (users != "*")
					{
						string existing = (string)el.assemblies[assembly];
						string newusers = string.Concat(existing, ",", users);
						el.assemblies[assembly] = newusers;
					}
					else
					{
						el.assemblies[assembly] = "*";
					}
				}
			}
		}

		public static bool MustVerify(AssemblyName an)
		{
			if (an == null || StrongNameManager.tokens == null)
			{
				return true;
			}
			string token = CryptoConvert.ToHex(an.GetPublicKeyToken());
			StrongNameManager.Element el = (StrongNameManager.Element)StrongNameManager.tokens[token];
			if (el != null)
			{
				string users = el.GetUsers(an.Name) ?? el.GetUsers("*");
				if (users != null)
				{
					if (users == "*")
					{
						return false;
					}
					return users.IndexOf(Environment.UserName) < 0;
				}
			}
			return true;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Public Key Token\tAssemblies\t\tUsers");
			sb.Append(Environment.NewLine);
			if (StrongNameManager.tokens == null)
			{
				sb.Append("none");
				return sb.ToString();
			}
			foreach (DictionaryEntry token in StrongNameManager.tokens)
			{
				sb.Append((string)token.Key);
				StrongNameManager.Element t = (StrongNameManager.Element)token.Value;
				bool first = true;
				foreach (DictionaryEntry assembly in t.assemblies)
				{
					if (!first)
					{
						sb.Append("\t\t\t");
					}
					else
					{
						sb.Append("\t");
						first = false;
					}
					sb.Append((string)assembly.Key);
					sb.Append("\t");
					string users = (string)assembly.Value;
					if (users == "*")
					{
						users = "All users";
					}
					sb.Append(users);
					sb.Append(Environment.NewLine);
				}
			}
			return sb.ToString();
		}

		private class Element
		{
			internal Hashtable assemblies;

			public Element()
			{
				this.assemblies = new Hashtable();
			}

			public Element(string assembly, string users) : this()
			{
				this.assemblies.Add(assembly, users);
			}

			public string GetUsers(string assembly)
			{
				return (string)this.assemblies[assembly];
			}
		}
	}
}