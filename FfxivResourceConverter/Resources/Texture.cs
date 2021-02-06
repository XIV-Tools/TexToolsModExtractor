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
	using SixLabors.ImageSharp;
	using SixLabors.ImageSharp.Formats.Png;
	using SixLabors.ImageSharp.PixelFormats;

	public class Texture
	{
		private XivTexFormat textureFormat;
		private int width;
		private int height;
		private int layers;
		private uint mipMapCount;
		private byte[] rawData;
		private byte[] pixelData;

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
			tex.width = br.ReadInt16();
			tex.height = br.ReadInt16();
			tex.layers = br.ReadInt16();
			tex.mipMapCount = (uint)br.ReadInt16();
			br.ReadBytes(64);
			tex.rawData = br.ReadBytes((int)br.BaseStream.Length - 80);
			tex.pixelData = tex.GetImageData();

			return tex;
		}

		public void ToDDS(FileInfo file)
		{
			List<byte> dds = new List<byte>();
			dds.AddRange(DDS.CreateDDSHeader(this.textureFormat, (uint)this.width, (uint)this.height, this.mipMapCount, this.layers));
			byte[] data = this.rawData;
			if (this.textureFormat == XivTexFormat.A8R8G8B8 && this.layers > 1)
			{
				data = ShiftLayers(data);
			}

			dds.AddRange(data);

			string fileName = file.DirectoryName + "/" + Path.GetFileNameWithoutExtension(file.FullName) + ".dds";
			File.WriteAllBytes(fileName, dds.ToArray());
		}

		public void ToPNG(FileInfo file)
		{
			string fileName = file.DirectoryName + "/" + Path.GetFileNameWithoutExtension(file.FullName) + ".png";

			PngEncoder encoder = new PngEncoder();
			encoder.BitDepth = PngBitDepth.Bit16;

			Image img = Image.LoadPixelData<Rgba32>(this.pixelData, this.width, this.height);
			img.Save(fileName, encoder);
			img.Dispose();
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

		/// <summary>
		/// Creates bitmap from decompressed A4R4G4B4 texture data.
		/// </summary>
		/// <param name="textureData">The decompressed texture data.</param>
		/// <param name="width">The textures width.</param>
		/// <param name="height">The textures height.</param>
		/// <returns>The raw byte data in 32bit</returns>
		private static byte[] Read4444Image(byte[] textureData, int width, int height)
		{
			List<byte> convertedBytes = new List<byte>();

			using MemoryStream ms = new MemoryStream(textureData);
			using BinaryReader br = new BinaryReader(ms);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int pixel = br.ReadUInt16() & 0xFFFF;
					int red = (pixel & 0xF) * 16;
					int green = ((pixel & 0xF0) >> 4) * 16;
					int blue = ((pixel & 0xF00) >> 8) * 16;
					int alpha = ((pixel & 0xF000) >> 12) * 16;

					convertedBytes.Add((byte)blue);
					convertedBytes.Add((byte)green);
					convertedBytes.Add((byte)red);
					convertedBytes.Add((byte)alpha);
				}
			}

			return convertedBytes.ToArray();
		}

		/// <summary>
		/// Creates bitmap from decompressed A1R5G5B5 texture data.
		/// </summary>
		/// <param name="textureData">The decompressed texture data.</param>
		/// <param name="width">The textures width.</param>
		/// <param name="height">The textures height.</param>
		/// <returns>The raw byte data in 32bit</returns>
		private static byte[] Read5551Image(byte[] textureData, int width, int height)
		{
			List<byte> convertedBytes = new List<byte>();

			using MemoryStream ms = new MemoryStream(textureData);
			using BinaryReader br = new BinaryReader(ms);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int pixel = br.ReadUInt16() & 0xFFFF;

					int red = ((pixel & 0x7E00) >> 10) * 8;
					int green = ((pixel & 0x3E0) >> 5) * 8;
					int blue = (pixel & 0x1F) * 8;
					int alpha = ((pixel & 0x8000) >> 15) * 255;

					convertedBytes.Add((byte)red);
					convertedBytes.Add((byte)green);
					convertedBytes.Add((byte)blue);
					convertedBytes.Add((byte)alpha);
				}
			}

			return convertedBytes.ToArray();
		}

		/// <summary>
		/// Creates bitmap from decompressed A8/L8 texture data.
		/// </summary>
		/// <param name="textureData">The decompressed texture data.</param>
		/// <param name="width">The textures width.</param>
		/// <param name="height">The textures height.</param>
		/// <returns>The created bitmap.</returns>
		private static byte[] Read8bitImage(byte[] textureData, int width, int height)
		{
			List<byte> convertedBytes = new List<byte>();

			using MemoryStream ms = new MemoryStream(textureData);
			using BinaryReader br = new BinaryReader(ms);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					int pixel = br.ReadByte() & 0xFF;

					convertedBytes.Add((byte)pixel);
					convertedBytes.Add((byte)pixel);
					convertedBytes.Add((byte)pixel);
					convertedBytes.Add(255);
				}
			}

			return convertedBytes.ToArray();
		}

		/// <summary>
		/// Creates bitmap from decompressed Linear texture data.
		/// </summary>
		/// <param name="textureData">The decompressed texture data.</param>
		/// <param name="width">The textures width.</param>
		/// <param name="height">The textures height.</param>
		/// <returns>The raw byte data in 32bit</returns>
		private static byte[] SwapRBColors(byte[] textureData, int width, int height)
		{
			List<byte> convertedBytes = new List<byte>();

			using MemoryStream ms = new MemoryStream(textureData);
			using BinaryReader br = new BinaryReader(ms);

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					byte red = br.ReadByte();
					byte green = br.ReadByte();
					byte blue = br.ReadByte();
					byte alpha = br.ReadByte();

					convertedBytes.Add(blue);
					convertedBytes.Add(green);
					convertedBytes.Add(red);
					convertedBytes.Add(alpha);
				}
			}

			return convertedBytes.ToArray();
		}

		/// <summary>
		/// Gets the raw pixel data for the texture.
		/// </summary>
		/// <returns>A byte array with the image data.</returns>
		private byte[] GetImageData(int layer = -1)
		{
			byte[] imageData = null;

			int layers = this.layers;
			if (layers == 0)
			{
				layers = 1;
			}

			switch (this.textureFormat)
			{
				case XivTexFormat.DXT1:
					imageData = DxtUtil.DecompressDxt1(this.rawData, this.width, this.height * layers);
					break;
				case XivTexFormat.DXT3:
					imageData = DxtUtil.DecompressDxt3(this.rawData, this.width, this.height * layers);
					break;
				case XivTexFormat.DXT5:
					imageData = DxtUtil.DecompressDxt5(this.rawData, this.width, this.height * layers);
					break;
				case XivTexFormat.A4R4G4B4:
					imageData = Read4444Image(this.rawData, this.width, this.height * layers);
					break;
				case XivTexFormat.A1R5G5B5:
					imageData = Read5551Image(this.rawData, this.width, this.height * layers);
					break;
				case XivTexFormat.A8R8G8B8:
					imageData = SwapRBColors(this.rawData, this.width, this.height * layers);
					break;
				case XivTexFormat.L8:
				case XivTexFormat.A8:
					imageData = Read8bitImage(this.rawData, this.width, this.height * layers);
					break;
				case XivTexFormat.X8R8G8B8:
				case XivTexFormat.R32F:
				case XivTexFormat.G16R16F:
				case XivTexFormat.G32R32F:
				case XivTexFormat.A16B16G16R16F:
				case XivTexFormat.A32B32G32R32F:
				case XivTexFormat.D16:
				default:
					imageData = this.rawData;
					break;
			}

			if (layer >= 0)
			{
				int bytesPerLayer = imageData.Length / this.layers;
				int offset = bytesPerLayer * layer;

				byte[] nData = new byte[bytesPerLayer];
				Array.Copy(imageData, offset, nData, 0, bytesPerLayer);

				imageData = nData;
			}

			return imageData;
		}
	}
}
