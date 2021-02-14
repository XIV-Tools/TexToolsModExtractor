// © XIV-Tools.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TexToolsModExtractorGUI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			XivToolsWpf.Themes.Apply(XivToolsWpf.Brightness.Dark, XivToolsWpf.Colors.DeepOrange);
			base.OnStartup(e);
		}
	}
}
