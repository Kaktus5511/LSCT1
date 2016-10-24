using LeagueSharp.Sandbox.Shared;
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace LeagueSharp.Loader.Class
{
	internal class Remoting
	{
		private static ServiceHost _loaderServiceHost;

		public Remoting()
		{
		}

		public static void Init()
		{
			Remoting._loaderServiceHost = ServiceFactory.CreateService<ILoaderService, LoaderService>(true);
			Remoting._loaderServiceHost.Faulted += new EventHandler(Remoting.OnLoaderServiceFaulted);
		}

		private static void OnLoaderServiceFaulted(object sender, EventArgs eventArgs)
		{
			Console.WriteLine("ILoaderService faulted, trying restart");
			Remoting._loaderServiceHost.Faulted -= new EventHandler(Remoting.OnLoaderServiceFaulted);
			Remoting._loaderServiceHost.Abort();
			try
			{
				Remoting._loaderServiceHost = ServiceFactory.CreateService<ILoaderService, LoaderService>(true);
				Remoting._loaderServiceHost.Faulted += new EventHandler(Remoting.OnLoaderServiceFaulted);
			}
			catch (Exception exception)
			{
				Console.WriteLine(exception);
			}
		}
	}
}