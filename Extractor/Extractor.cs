// © XIV-Tools.
// Licensed under the MIT license.

namespace TexToolsModExtractor
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using TexToolsModExtractor.Extractors;

	public static class Extractor
	{
		public static List<FileInfo> Extract(FileInfo file, DirectoryInfo outputDirectory)
		{
			if (!file.Exists)
				throw new FileNotFoundException("Mod pack not found", file.FullName);

			ExtractorBase extractor = GetExtractor(file.Extension);
			return extractor.Extract(file, outputDirectory);
		}

		private static ExtractorBase GetExtractor(string extension)
		{
			if (extension == ".ttmp")
			{
				return new TexToolsModPackExtractor();
			}
			else if (extension == ".ttmp2")
			{
				return new TexToolsModPack2Extractor();
			}

			throw new Exception($"Unrecognized TexTools mod pack extension: {extension}");
		}
	}
}