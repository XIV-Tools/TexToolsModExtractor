// © XIV-Tools.
// Licensed under the MIT license.

namespace FfxivResourceConverter.Resources
{
	using System.IO;
	using Newtonsoft.Json;

	public class Imc
	{
		/// <summary>
		/// Gets or sets the Material Set / Variant #.
		/// </summary>
		public byte MaterialSet { get; set; }

		/// <summary>
		/// Gets or sets unknown byte next to the Variant.
		/// </summary>
		public byte Decal { get; set; }

		/// <summary>
		/// Gets or sets the IMC mask data.
		/// </summary>
		/// <remarks>
		/// This is used with a models mesh part mask data
		/// It is used to determine what parts of the mesh are hidden.
		/// </remarks>
		public ushort Mask { get; set; }

		/// <summary>
		/// Gets or sets the IMC VFX data.
		/// </summary>
		/// <remarks>
		/// Only a few items have VFX data associated with them
		/// Some examples would be any of the Lux weapons.
		/// </remarks>
		public byte Vfx { get; set; }

		/// <summary>
		/// Gets or sets material animation byte.
		/// </summary>
		public byte Animation { get; set; }

		public void ToImc(FileInfo file, ConverterSettings settings)
		{
			// Don't knoww what format imc files are actually in, so for now we'll dump them to json.
			this.ToJson(file, settings);
		}

		public void ToJson(FileInfo file, ConverterSettings settings)
		{
			string json = JsonConvert.SerializeObject(this, settings.JsonSettings);
			string fileName = file.DirectoryName + "/" + Path.GetFileNameWithoutExtension(file.FullName) + ".json";
			File.WriteAllText(fileName, json);
		}
	}
}
