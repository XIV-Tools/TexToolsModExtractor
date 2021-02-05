using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TexToolsModExtractor;
using System.IO;

namespace TexToolsModExtractorGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void OnBrowseClick(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "TexTools Mod Pack (*.ttmp, *.ttmp2)|*.ttmp;*.ttmp2";
			dlg.ShowDialog();
			this.PathBox.Text = dlg.FileName;
		}
		private void OnExtractClick(object sender, RoutedEventArgs e)
		{
			Extractor.Extract(new FileInfo(this.PathBox.Text));
		}
	}
}
