// © XIV-Tools.
// Licensed under the MIT license.

namespace TexToolsModExtractorCli
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using FfxivResourceConverter;
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

			FileInfo inputModPack = new FileInfo(path);

			string outputDirPath = Path.GetFileNameWithoutExtension(inputModPack.Name) + "_Extracted/";
			DirectoryInfo outputDirectory = inputModPack.Directory.CreateSubdirectory(outputDirPath);
			outputDirectory.Delete(true);
			outputDirectory.Create();

			List<FileInfo> files = Extractor.Extract(inputModPack, outputDirectory);

			foreach (FileInfo extractedFile in files)
			{
				ResourceConverter.Convert(extractedFile);
			}

			Console.WriteLine("Extraction complete");
			Console.ReadKey();
		}
	}
}
