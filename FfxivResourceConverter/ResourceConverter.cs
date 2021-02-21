// © XIV-Tools.
// Licensed under the MIT license.

namespace FfxivResourceConverter
{
	using System;
	using System.IO;
	using FfxivResourceConverter.Resources;

	public static class ResourceConverter
	{
		public static bool Convert(FileInfo file, ConverterSettings settings)
		{
			if (string.IsNullOrEmpty(file.Extension))
				return false;

			if (file.Extension == ".tex")
			{
				Console.WriteLine("Converting: " + file.Name);
				Texture tex = Texture.FromTex(file);

				if (settings.TextureFormat.HasFlag(ConverterSettings.TextureFormats.Dds))
					tex.ToDDS(file, settings);

				if (settings.TextureFormat.HasFlag(ConverterSettings.TextureFormats.Png))
					tex.ToPNG(file, settings);

				return true;
			}
			else if (file.Extension == ".mtrl")
			{
				Console.WriteLine("Converting: " + file.Name);
				Material mat = Material.FromMtrl(file);
				mat.ToJson(file, settings);

				return true;
			}
			/*else if (file.Extension == ".mdl")
			{
				Console.WriteLine("Converting: " + file.Name);

				Model mdl = Model.FromMdl(file);
				mdl.ToFbx(file, settings);
				return true;
			}*/

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"Unknown file format: {file.Extension}");
			Console.ForegroundColor = ConsoleColor.White;
			return false;
		}
	}
}
