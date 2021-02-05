// © XIV-Tools.
// Licensed under the MIT license.

namespace TexToolsModExtractor
{
	using System;
	using System.IO;

	public static class Extractor
	{
		public static void Extract(string path)
		{
			if (!File.Exists(path))
				throw new FileNotFoundException("Mod pack not found", path);
		}
	}
}