// © XIV-Tools.
// Licensed under the MIT license.

// xivModdingFramework
// Copyright © 2018 Rafael Gonzalez - All Rights Reserved
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
namespace FfxivResourceConverter.Resources
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using FfxivResourceConverter.Resources.Materials;
	using Newtonsoft.Json;
	using SharpDX;

	[Serializable]
	public class Material
	{
		/// <summary>
		/// Gets or sets the MTRL file signature.
		/// </summary>
		/// <remarks>
		/// 0x00000301 (16973824).
		/// </remarks>
		public int Signature { get; set; }

		/// <summary>
		/// Gets or sets the size of the MTRL file.
		/// </summary>
		public short FileSize { get; set; }

		/// <summary>
		/// Gets or sets the size of the Material Data section.
		/// </summary>
		/// <remarks>
		/// This is the size of the data chunk containing all of the path and filename strings.
		/// </remarks>
		public ushort MaterialDataSize { get; set; }

		/// <summary>
		/// Gets or sets the size of the Texture Path Data section.
		/// </summary>
		/// <remarks>
		/// This is the size of the data chucnk containing only the texture paths.
		/// </remarks>
		public ushort TexturePathsDataSize { get; set; }

		/// <summary>
		/// Gets or sets the number of textures paths in the mtrl.
		/// </summary>
		public byte TextureCount { get; set; }

		/// <summary>
		/// Gets or sets the number of map paths in the mtrl.
		/// </summary>
		public byte MapCount { get; set; }

		/// <summary>
		/// Gets or sets the amount of color sets in the mtrl.
		/// </summary>
		/// <remarks>
		/// It is not known if there are any instances where this is greater than 1.
		/// </remarks>
		public byte ColorSetCount { get; set; }

		/// <summary>
		/// Gets or sets the number of bytes to skip after path section.
		/// </summary>
		public byte UnknownDataSize { get; set; }

		/// <summary>
		/// Gets or sets a list containing the Texture Path offsets.
		/// </summary>
		public List<int> TexturePathOffsetList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the Texture Path Unknowns.
		/// </summary>
		public List<short> TexturePathUnknownList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the Map Path offsets.
		/// </summary>
		public List<int> MapPathOffsetList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the Map Path Unknowns.
		/// </summary>
		public List<short> MapPathUnknownList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the ColorSet Path offsets.
		/// </summary>
		public List<int> ColorSetPathOffsetList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the ColorSet Path Unknowns.
		/// </summary>
		public List<short> ColorSetPathUnknownList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the Texture Path strings.
		/// </summary>
		public List<string> TexturePathList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the Map Path strings.
		/// </summary>
		public List<string> MapPathList { get; set; }

		/// <summary>
		/// Gets or sets a list containing the ColorSet Path strings.
		/// </summary>
		public List<string> ColorSetPathList { get; set; }

		/// <summary>
		/// Gets or sets the name of the shader used by the item.
		/// </summary>
		public string Shader { get; set; }

		public byte[] Unknown2 { get; set; }
		public ushort Unknown3 { get; set; }

		/// <summary>
		/// Gets or sets the list of half floats containing the ColorSet data.
		/// </summary>
		public List<Half> ColorSetData { get; set; }

		/// <summary>
		/// Gets or sets the byte array containing the extra ColorSet data.
		/// </summary>
		public byte[] ColorSetDyeData { get; set; }

		/// <summary>
		/// Gets or sets the shader number used by the item.
		/// </summary>
		/// <remarks>
		/// This is a guess and has not been tested to be true
		/// Seems to be more likely that this is some base argument passed into the shader.
		/// </remarks>
		public ushort ShaderNumber { get; set; }

		/// <summary>
		/// Gets or sets the list of Type 1 data structures.
		/// </summary>
		public List<TextureUsageStruct> TextureUsageList { get; set; }

		/// <summary>
		/// Gets or sets the list of Type 2 data structures.
		/// </summary>
		public List<ShaderParameterStruct> ShaderParameterList { get; set; }

		/// <summary>
		/// Gets or sets the list of Parameter data structures.
		/// </summary>
		public List<TextureDescriptorStruct> TextureDescriptorList { get; set; }

		/// <summary>
		/// Gets the number of type 1 data sturctures.
		/// </summary>
		public ushort TextureUsageCount => (ushort)this.TextureUsageList.Count;

		/// <summary>
		/// Gets the number of type 2 data structures.
		/// </summary>
		public ushort ShaderParameterCount => (ushort)this.ShaderParameterList.Count;

		/// <summary>
		/// Gets the number of parameter stuctures.
		/// </summary>
		public ushort TextureDescriptorCount => (ushort)this.TextureDescriptorList.Count;

		/// <summary>
		/// Gets the size of the ColorSet Data section.
		/// </summary>
		/// <remarks>
		/// Can be 0 if there is no ColorSet Data.
		/// </remarks>
		public ushort ColorSetDataSize
		{
			get
			{
				int size = this.ColorSetData.Count * 2;
				size += this.ColorSetDyeData == null ? 0 : this.ColorSetDyeData.Length;
				return (ushort)size;
			}
		}

		/// <summary>
		/// Gets the size of the additional MTRL Data.
		/// </summary>
		public ushort ShaderParameterDataSize
		{
			get
			{
				int size = 0;
				this.ShaderParameterList.ForEach(x =>
				{
					size += x.Args.Count * 4;
				});

				return (ushort)size;
			}
		}

		public static Material FromMtrl(FileInfo file)
		{
			return MaterialMtrl.FromMtrl(file);
		}

		public void ToJson(FileInfo file, JsonSerializerSettings settings)
		{
			string json = JsonConvert.SerializeObject(this, settings);

			string fileName = file.DirectoryName + "/" + Path.GetFileNameWithoutExtension(file.FullName) + ".json";
			File.WriteAllText(fileName, json);
		}
	}
}
