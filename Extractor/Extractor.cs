// © XIV-Tools.
// Licensed under the MIT license.

namespace TexToolsModExtractor
{
	using System;
	using System.IO;
	using TexToolsModExtractor.Extractors;

	public static class Extractor
	{
		public static void Extract(FileInfo file)
		{
			if (!file.Exists)
				throw new FileNotFoundException("Mod pack not found", file.FullName);

			ExtractorBase extractor = GetExtractor(file.Extension);
			extractor.Extract(file);
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