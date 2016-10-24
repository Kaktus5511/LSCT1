using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LeagueSharp.Loader.Class
{
	internal class Win32Imports
	{
		public Win32Imports()
		{
		}

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr FindWindow(IntPtr ZeroOnly, string lpWindowName);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern bool FreeLibrary(IntPtr hModule);

		[DllImport("user32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

		[DllImport("user32.dll", CharSet=CharSet.Unicode, ExactSpelling=false)]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

		[DllImport("user32.dll", CharSet=CharSet.Unicode, ExactSpelling=false)]
		public static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr LoadLibrary(string dllToLoad);
	}
}