// © XIV-Tools.
// Licensed under the MIT license.

namespace TexToolsModExtractorCli
{
	using System;
	using System.IO;
	using TexToolsModExtractor;

	public class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length <= 0)
			{
				Console.WriteLine("You must provide the path to a file to extract");
				return;
			}

			string path = args[0];
			Console.WriteLine("Running extractor on file: " + path);

			try
			{
				FileInfo file = new FileInfo(path);
				Extractor.Extract(file);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ReadKey();
				throw;
			}

			Console.WriteLine("Extraction complete");
			Console.ReadKey();
		}
	}
}
