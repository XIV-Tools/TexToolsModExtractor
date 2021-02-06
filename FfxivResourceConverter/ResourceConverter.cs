// © XIV-Tools.
// Licensed under the MIT license.

namespace FfxivResourceConverter
{
	using System;
	using System.IO;

	public static class ResourceConverter
	{
		public static bool Convert(FileInfo file, ConverterSettings settings)
		{
			if (file.Extension == ".tex")
			{
				Console.WriteLine("Converting: " + file.Name);
				Texture tex = Texture.FromTex(file);

				if (settings.TextureFormat.HasFlag(ConverterSettings.TextureFormats.Dds))
					tex.ToDDS(file);

				if (settings.TextureFormat.HasFlag(ConverterSettings.TextureFormats.Png))
					tex.ToPNG(file);

				return true;
			}

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"Unknown file format: {file.Extension}");
			Console.ForegroundColor = ConsoleColor.White;
			return false;
		}
	}
}
