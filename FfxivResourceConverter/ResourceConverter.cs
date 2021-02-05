// © XIV-Tools.
// Licensed under the MIT license.

namespace FfxivResourceConverter
{
	using System;
	using System.IO;

	public static class ResourceConverter
	{
		public static void Convert(FileInfo file)
		{
			if (file.Extension == ".tex")
			{
				Console.WriteLine("Converting: " + file.Name);
				Texture tex = Texture.FromTex(file);
				tex.ToDDS(file);
			}
		}
	}
}
