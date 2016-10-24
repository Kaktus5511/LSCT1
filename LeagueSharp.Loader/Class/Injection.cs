using System;

namespace LeagueSharp.Loader.Class
{
	public static class Injection
	{
		public struct SharedMemoryLayout
		{
			private readonly string SandboxPath;

			private readonly string BootstrapPath;

			private readonly string User;

			private readonly string Password;

			public SharedMemoryLayout(string sandboxPath, string bootstrapPath, string user, string password)
			{
				this.SandboxPath = sandboxPath;
				this.BootstrapPath = bootstrapPath;
				this.User = user;
				this.Password = password;
			}
		}
	}
}