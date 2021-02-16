// © XIV-Tools.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using System.Windows;

namespace TexToolsModExtractorGUI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			AssemblyLoadContext.Default.Resolving += this.ResolveAssembly;
		}
		protected override void OnStartup(StartupEventArgs e)
		{
			XivToolsWpf.Themes.Apply(XivToolsWpf.Brightness.Dark, XivToolsWpf.Colors.DeepOrange);
			base.OnStartup(e);
		}

		private Assembly ResolveAssembly(AssemblyLoadContext context, AssemblyName name)
		{
			if (name.Name == null)
				return null;

			string path = AppContext.BaseDirectory + "/bin/" + name.Name + ".dll";
			if (File.Exists(path))
				return context.LoadFromAssemblyPath(path);

			return null;
		}
	}


}
