using System;
using System.Reflection;

namespace LeagueSharp.Loader.Class
{
	public class Reflector
	{
		private Assembly m_asmb;

		private string m_ns;

		public Reflector(string ns) : this(ns, ns)
		{
		}

		public Reflector(string an, string ns)
		{
			this.m_ns = ns;
			this.m_asmb = null;
			AssemblyName[] referencedAssemblies = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
			for (int i = 0; i < (int)referencedAssemblies.Length; i++)
			{
				AssemblyName aN = referencedAssemblies[i];
				if (aN.FullName.StartsWith(an))
				{
					this.m_asmb = Assembly.Load(aN);
					return;
				}
			}
		}

		public object Call(object obj, string func, params object[] parameters)
		{
			return this.Call2(obj, func, parameters);
		}

		public object Call2(object obj, string func, object[] parameters)
		{
			return this.CallAs2(obj.GetType(), obj, func, parameters);
		}

		public object CallAs(Type type, object obj, string func, params object[] parameters)
		{
			return this.CallAs2(type, obj, func, parameters);
		}

		public object CallAs2(Type type, object obj, string func, object[] parameters)
		{
			return type.GetMethod(func, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Invoke(obj, parameters);
		}

		public object Get(object obj, string prop)
		{
			return this.GetAs(obj.GetType(), obj, prop);
		}

		public object GetAs(Type type, object obj, string prop)
		{
			return type.GetProperty(prop, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(obj, null);
		}

		public object GetEnum(string typeName, string name)
		{
			Type type = this.GetType(typeName);
			return type.GetField(name).GetValue(null);
		}

		public Type GetType(string typeName)
		{
			Type type = null;
			string[] names = typeName.Split(new char[] { '.' });
			if (names.Length != 0)
			{
				type = this.m_asmb.GetType(string.Concat(this.m_ns, ".", names[0]));
			}
			for (int i = 1; i < (int)names.Length; i++)
			{
				type = type.GetNestedType(names[i], BindingFlags.NonPublic);
			}
			return type;
		}

		public object New(string name, params object[] parameters)
		{
			object obj;
			ConstructorInfo[] constructors = this.GetType(name).GetConstructors();
			int num = 0;
		Label1:
			while (num < (int)constructors.Length)
			{
				ConstructorInfo ci = constructors[num];
				try
				{
					obj = ci.Invoke(parameters);
				}
				catch
				{
					goto Label0;
				}
				return obj;
			}
			return null;
		Label0:
			num++;
			goto Label1;
		}
	}
}