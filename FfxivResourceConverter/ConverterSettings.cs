// © XIV-Tools.
// Licensed under the MIT license.

namespace FfxivResourceConverter
{
	using System;
	using System.Collections.Generic;
	using FfxivResourceConverter.Json;
	using Newtonsoft.Json;

	public class ConverterSettings
	{
		public TextureFormats TextureFormat = TextureFormats.Png;
		public JsonSerializerSettings JsonSettings = new JsonSerializerSettings()
		{
			Formatting = Formatting.Indented,
			Converters = new List<JsonConverter>()
			{
				new HalfConverter(),
			},
		};

		[Flags]
		public enum TextureFormats
		{
			Png = 0,
			Dds = 1,
		}
	}
}
