using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Windows;

namespace LeagueSharp.Sandbox.Shared
{
	public static class ServiceFactory
	{
		public static TInterfaceType CreateProxy<TInterfaceType>()
		where TInterfaceType : class
		{
			TInterfaceType tInterfaceType;
			try
			{
				tInterfaceType = (new ChannelFactory<TInterfaceType>(new NetNamedPipeBinding(), new EndpointAddress(string.Concat("net.pipe://localhost/", typeof(TInterfaceType).Name)))).CreateChannel();
			}
			catch (Exception exception)
			{
				Exception e = exception;
				throw new Exception(string.Concat("Failed to connect to pipe for communication. The targetted pipe may not be loaded yet. Desired interface: ", typeof(TInterfaceType).Name), e);
			}
			return tInterfaceType;
		}

		public static ServiceHost CreateService<TInterfaceType, TImplementationType>(bool open = true)
		where TImplementationType : class
		{
			ServiceHost serviceHost;
			try
			{
				if (!typeof(TInterfaceType).IsAssignableFrom(typeof(TImplementationType)))
				{
					throw new NotImplementedException(string.Concat(typeof(TImplementationType).FullName, " does not implement ", typeof(TInterfaceType).FullName));
				}
				Uri uri = new Uri(string.Concat("net.pipe://localhost/", typeof(TInterfaceType).Name));
				ServiceHost host = new ServiceHost(typeof(TImplementationType), new Uri[0]);
				host.AddServiceEndpoint(typeof(TInterfaceType), new NetNamedPipeBinding(), uri);
				host.Opened += new EventHandler((object sender, EventArgs args) => Console.WriteLine(string.Concat("Opened: ", uri)));
				host.Faulted += new EventHandler((object sender, EventArgs args) => Console.WriteLine(string.Concat("Faulted: ", uri)));
				host.UnknownMessageReceived += new EventHandler<UnknownMessageReceivedEventArgs>((object sender, UnknownMessageReceivedEventArgs args) => Console.WriteLine(string.Concat("UnknownMessage: ", uri)));
				if (open)
				{
					host.Open();
				}
				serviceHost = host;
			}
			catch (Exception exception)
			{
				Exception e = exception;
				MessageBox.Show(string.Format("Make sure only one LeagueSharp Instance is running on your Computer.\n\n{0}", e.Message), "Failed to initialize Remoting");
				Environment.Exit(0);
				return null;
			}
			return serviceHost;
		}
	}
}