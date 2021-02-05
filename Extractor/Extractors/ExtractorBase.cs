// © XIV-Tools.
// Licensed under the MIT license.

namespace TexToolsModExtractor.Extractors
{
	using System.Collections.Generic;
	using System.IO;

	internal abstract class ExtractorBase
	{
		public abstract List<FileInfo> Extract(FileInfo file, DirectoryInfo outputDirectory);
	}
}
