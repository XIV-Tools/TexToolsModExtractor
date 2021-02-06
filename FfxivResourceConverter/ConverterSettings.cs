// © XIV-Tools.
// Licensed under the MIT license.

namespace FfxivResourceConverter
{
	using System;
	using System.IO;

	public class ConverterSettings
	{
		public TextureFormats TextureFormat = TextureFormats.Png;

		[Flags]
		public enum TextureFormats
		{
			Png = 0,
			Dds = 1,
		}
	}
}
