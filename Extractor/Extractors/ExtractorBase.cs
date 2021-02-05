// © XIV-Tools.
// Licensed under the MIT license.

namespace TexToolsModExtractor.Extractors
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;

	public abstract class ExtractorBase
	{
		public abstract void Extract(FileInfo file);
	}
}
