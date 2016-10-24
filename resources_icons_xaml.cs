using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Markup;

public class resources_icons_xaml : ResourceDictionary, IComponentConnector
{
	private bool _contentLoaded;

	public resources_icons_xaml()
	{
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	public void InitializeComponent()
	{
		if (this._contentLoaded)
		{
			return;
		}
		this._contentLoaded = true;
		Application.LoadComponent(this, new Uri("/loader;component/resources/icons.xaml", UriKind.Relative));
	}

	[DebuggerNonUserCode]
	[EditorBrowsable(EditorBrowsableState.Never)]
	[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
	void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
	{
		this._contentLoaded = true;
	}
}