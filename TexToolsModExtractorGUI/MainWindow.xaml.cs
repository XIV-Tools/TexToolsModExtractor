using System.Windows;
using TexToolsModExtractor;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using FfxivResourceConverter;

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
		private void OnSelectClick(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.ShowDialog();
			this.OutputBox.Text = dlg.SelectedPath;
		}

		private void OnExtractClick(object sender, RoutedEventArgs e)
		{
			FileInfo modPackFile = new FileInfo(this.PathBox.Text);
			DirectoryInfo outputdirectory = new DirectoryInfo(this.OutputBox.Text);
			List<FileInfo> files = Extractor.Extract(modPackFile, outputdirectory);
			foreach (FileInfo extractedFile in files)
			{
				ResourceConverter.Convert(extractedFile);
			}

		}
	}
}
