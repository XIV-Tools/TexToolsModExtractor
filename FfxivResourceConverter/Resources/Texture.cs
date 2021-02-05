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
namespace FfxivResourceConverter
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	public class Texture
	{
		private XivTexFormat textureFormat;
		private uint width;
		private uint height;
		private int layers;
		private uint mipMapCount;
		private byte[] texData;

		public enum XivTexFormat
		{
			Invalid = 0,

			L8 = 4400,
			A8 = 4401,
			A4R4G4B4 = 5184,
			A1R5G5B5 = 5185,
			A8R8G8B8 = 5200,
			X8R8G8B8 = 5201,
			R32F = 8528,
			G16R16F = 8784,
			G32R32F = 8800,
			A16B16G16R16F = 9312,
			A32B32G32R32F = 9328,
			DXT1 = 13344,
			DXT3 = 13360,
			DXT5 = 13361,
			D16 = 16704,
		}

		public static Texture FromTex(FileInfo file)
		{
			Texture tex = new Texture();
			BinaryReader br = new BinaryReader(file.OpenRead());

			int signature = br.ReadInt32();
			tex.textureFormat = (XivTexFormat)br.ReadInt32();
			tex.width = (uint)br.ReadInt16();
			tex.height = (uint)br.ReadInt16();
			tex.layers = br.ReadInt16();
			tex.mipMapCount = (uint)br.ReadInt16();
			br.ReadBytes(64);
			tex.texData = br.ReadBytes((int)br.BaseStream.Length - 80);

			return tex;
		}

		public void ToDDS(FileInfo file)
		{
			List<byte> dds = new List<byte>();
			dds.AddRange(DDS.CreateDDSHeader(this.textureFormat, this.width, this.height, this.mipMapCount, this.layers));
			byte[] data = this.texData;
			if (this.textureFormat == XivTexFormat.A8R8G8B8 && this.layers > 1)
			{
				data = ShiftLayers(data);
			}

			dds.AddRange(data);

			string fileName = file.DirectoryName + "/" + Path.GetFileNameWithoutExtension(file.FullName) + ".dds";
			File.WriteAllBytes(fileName, dds.ToArray());
		}

		// This is a simple shift of the layers around in order to convert ARGB to RGBA
		private static byte[] ShiftLayers(byte[] data)
		{
			for (int i = 0; i < data.Length; i += 4)
			{
				byte alpha = data[i];
				byte red = data[i + 1];
				byte green = data[i + 2];
				byte blue = data[i + 3];

				data[i] = red;
				data[i + 1] = green;
				data[i + 2] = blue;
				data[i + 3] = alpha;
			}

			return data;
		}
	}
}
