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
	using System.Text;

	public static class DDS
	{
		/// <summary>
		/// Creates the DDS header for given texture data.
		/// <see cref="https://msdn.microsoft.com/en-us/library/windows/desktop/bb943982(v=vs.85).aspx"/>.
		/// </summary>
		/// <returns>Byte array containing DDS header.</returns>
		public static byte[] CreateDDSHeader(Texture.XivTexFormat format, uint dwWidth, uint dwHeight, uint dwMipMapCount, int layers)
		{
			uint dwPitchOrLinearSize, pfFlags, dwFourCC;
			List<byte> header = new List<byte>();

			// DDS header magic number
			const uint dwMagic = 0x20534444;
			header.AddRange(BitConverter.GetBytes(dwMagic));

			// Size of structure. This member must be set to 124.
			const uint dwSize = 124;
			header.AddRange(BitConverter.GetBytes(dwSize));

			// Flags to indicate which members contain valid data.
			uint dwFlags = 528391;
			if (layers > 1)
			{
				dwFlags = 0x00000004;
			}

			header.AddRange(BitConverter.GetBytes(dwFlags));

			// Surface height (in pixels).
			header.AddRange(BitConverter.GetBytes(dwWidth));

			// Surface width (in pixels).
			header.AddRange(BitConverter.GetBytes(dwHeight));

			// The pitch or number of bytes per scan line in an uncompressed texture; the total number of bytes in the top level texture for a compressed texture.
			if (format == Texture.XivTexFormat.A16B16G16R16F)
			{
				dwPitchOrLinearSize = 512;
			}
			else if (format == Texture.XivTexFormat.A8R8G8B8)
			{
				dwPitchOrLinearSize = (dwHeight * dwWidth) * 4;
			}
			else if (format == Texture.XivTexFormat.DXT1)
			{
				dwPitchOrLinearSize = (dwHeight * dwWidth) / 2;
			}
			else if (format == Texture.XivTexFormat.A4R4G4B4 || format == Texture.XivTexFormat.A1R5G5B5)
			{
				dwPitchOrLinearSize = (dwHeight * dwWidth) * 2;
			}
			else
			{
				dwPitchOrLinearSize = dwHeight * dwWidth;
			}

			header.AddRange(BitConverter.GetBytes(dwPitchOrLinearSize));

			// Depth of a volume texture (in pixels), otherwise unused.
			const uint dwDepth = 0;
			header.AddRange(BitConverter.GetBytes(dwDepth));

			// Number of mipmap levels, otherwise unused.
			header.AddRange(BitConverter.GetBytes(dwMipMapCount));

			// Unused.
			byte[] dwReserved1 = new byte[44];
			Array.Clear(dwReserved1, 0, 44);
			header.AddRange(dwReserved1);

			// DDS_PIXELFORMAT start

			// Structure size; set to 32 (bytes).
			const uint pfSize = 32;
			header.AddRange(BitConverter.GetBytes(pfSize));

			switch (format)
			{
				// Values which indicate what type of data is in the surface.
				case Texture.XivTexFormat.A8R8G8B8:
				case Texture.XivTexFormat.A4R4G4B4:
				case Texture.XivTexFormat.A1R5G5B5:
					pfFlags = 65;
					break;
				case Texture.XivTexFormat.A8:
					pfFlags = 2;
					break;
				default:
					pfFlags = 4;
					break;
			}

			header.AddRange(BitConverter.GetBytes(pfFlags));

			switch (format)
			{
				// Four-character codes for specifying compressed or custom formats.
				case Texture.XivTexFormat.DXT1:
					dwFourCC = 0x31545844;
					break;
				case Texture.XivTexFormat.DXT5:
					dwFourCC = 0x35545844;
					break;
				case Texture.XivTexFormat.DXT3:
					dwFourCC = 0x33545844;
					break;
				case Texture.XivTexFormat.A16B16G16R16F:
					dwFourCC = 0x71;
					break;
				case Texture.XivTexFormat.A8R8G8B8:
				case Texture.XivTexFormat.A8:
				case Texture.XivTexFormat.A4R4G4B4:
				case Texture.XivTexFormat.A1R5G5B5:
					dwFourCC = 0;
					break;
				default:
					return null;
			}

			if (layers > 1)
			{
				byte[] bytes = System.Text.Encoding.UTF8.GetBytes("DX10");
				dwFourCC = BitConverter.ToUInt32(bytes, 0);
			}

			header.AddRange(BitConverter.GetBytes(dwFourCC));

			switch (format)
			{
				case Texture.XivTexFormat.A8R8G8B8:
				{
					// Number of bits in an RGB (possibly including alpha) format.
					const uint dwRGBBitCount = 32;
					header.AddRange(BitConverter.GetBytes(dwRGBBitCount));

					// Red (or lumiannce or Y) mask for reading color data.
					const uint dwRBitMask = 16711680;
					header.AddRange(BitConverter.GetBytes(dwRBitMask));

					// Green (or U) mask for reading color data.
					const uint dwGBitMask = 65280;
					header.AddRange(BitConverter.GetBytes(dwGBitMask));

					// Blue (or V) mask for reading color data.
					const uint dwBBitMask = 255;
					header.AddRange(BitConverter.GetBytes(dwBBitMask));

					// Alpha mask for reading alpha data.
					const uint dwABitMask = 4278190080;
					header.AddRange(BitConverter.GetBytes(dwABitMask));

					// DDS_PIXELFORMAT End

					// Specifies the complexity of the surfaces stored.
					const uint dwCaps = 4096;
					header.AddRange(BitConverter.GetBytes(dwCaps));

					// dwCaps2, dwCaps3, dwCaps4, dwReserved2.
					// Unused.
					byte[] blank1 = new byte[16];
					header.AddRange(blank1);

					break;
				}

				case Texture.XivTexFormat.A8:
				{
					// Number of bits in an RGB (possibly including alpha) format.
					const uint dwRGBBitCount = 8;
					header.AddRange(BitConverter.GetBytes(dwRGBBitCount));

					// Red (or lumiannce or Y) mask for reading color data.
					const uint dwRBitMask = 0;
					header.AddRange(BitConverter.GetBytes(dwRBitMask));

					// Green (or U) mask for reading color data.
					const uint dwGBitMask = 0;
					header.AddRange(BitConverter.GetBytes(dwGBitMask));

					// Blue (or V) mask for reading color data.
					const uint dwBBitMask = 0;
					header.AddRange(BitConverter.GetBytes(dwBBitMask));

					// Alpha mask for reading alpha data.
					const uint dwABitMask = 255;
					header.AddRange(BitConverter.GetBytes(dwABitMask));

					// DDS_PIXELFORMAT End

					// Specifies the complexity of the surfaces stored.
					const uint dwCaps = 4096;
					header.AddRange(BitConverter.GetBytes(dwCaps));

					// dwCaps2, dwCaps3, dwCaps4, dwReserved2.
					// Unused.
					byte[] blank1 = new byte[16];
					header.AddRange(blank1);
					break;
				}

				case Texture.XivTexFormat.A1R5G5B5:
				{
					// Number of bits in an RGB (possibly including alpha) format.
					const uint dwRGBBitCount = 16;
					header.AddRange(BitConverter.GetBytes(dwRGBBitCount));

					// Red (or lumiannce or Y) mask for reading color data.
					const uint dwRBitMask = 31744;
					header.AddRange(BitConverter.GetBytes(dwRBitMask));

					// Green (or U) mask for reading color data.
					const uint dwGBitMask = 992;
					header.AddRange(BitConverter.GetBytes(dwGBitMask));

					// Blue (or V) mask for reading color data.
					const uint dwBBitMask = 31;
					header.AddRange(BitConverter.GetBytes(dwBBitMask));

					// Alpha mask for reading alpha data.
					const uint dwABitMask = 32768;
					header.AddRange(BitConverter.GetBytes(dwABitMask));

					// DDS_PIXELFORMAT End

					// Specifies the complexity of the surfaces stored.
					const uint dwCaps = 4096;
					header.AddRange(BitConverter.GetBytes(dwCaps));

					// dwCaps2, dwCaps3, dwCaps4, dwReserved2.
					// Unused.
					byte[] blank1 = new byte[16];
					header.AddRange(blank1);
					break;
				}

				case Texture.XivTexFormat.A4R4G4B4:
				{
					// Number of bits in an RGB (possibly including alpha) format.
					const uint dwRGBBitCount = 16;
					header.AddRange(BitConverter.GetBytes(dwRGBBitCount));

					// Red (or lumiannce or Y) mask for reading color data.
					const uint dwRBitMask = 3840;
					header.AddRange(BitConverter.GetBytes(dwRBitMask));

					// Green (or U) mask for reading color data.
					const uint dwGBitMask = 240;
					header.AddRange(BitConverter.GetBytes(dwGBitMask));

					// Blue (or V) mask for reading color data.
					const uint dwBBitMask = 15;
					header.AddRange(BitConverter.GetBytes(dwBBitMask));

					// Alpha mask for reading alpha data.
					const uint dwABitMask = 61440;
					header.AddRange(BitConverter.GetBytes(dwABitMask));

					// DDS_PIXELFORMAT End

					// Specifies the complexity of the surfaces stored.
					const uint dwCaps = 4096;
					header.AddRange(BitConverter.GetBytes(dwCaps));

					// dwCaps2, dwCaps3, dwCaps4, dwReserved2.
					// Unused.
					byte[] blank1 = new byte[16];
					header.AddRange(blank1);
					break;
				}

				default:
				{
					// dwRGBBitCount, dwRBitMask, dwGBitMask, dwBBitMask, dwABitMask, dwCaps, dwCaps2, dwCaps3, dwCaps4, dwReserved2.
					// Unused.
					byte[] blank1 = new byte[40];
					header.AddRange(blank1);
					break;
				}
			}

			// Need to write DX10 header here.
			if (layers > 1)
			{
				// DXGI_FORMAT dxgiFormat
				uint dxgiFormat = 0;
				if (format == Texture.XivTexFormat.DXT1)
				{
					dxgiFormat = 71; ////(uint)DXGI_FORMAT.DXGI_FORMAT_BC1_UNORM;
				}
				else if (format == Texture.XivTexFormat.DXT5)
				{
					dxgiFormat = 77; ////(uint)DXGI_FORMAT.DXGI_FORMAT_BC3_UNORM;
				}
				else
				{
					dxgiFormat = 28; ////(uint)DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
				}

				header.AddRange(BitConverter.GetBytes(dxgiFormat));

				// D3D10_RESOURCE_DIMENSION resourceDimension
				header.AddRange(BitConverter.GetBytes((int)3));

				// UINT miscFlag
				header.AddRange(BitConverter.GetBytes((int)0));

				// UINT arraySize
				header.AddRange(BitConverter.GetBytes(layers));

				// UINT miscFlags2
				header.AddRange(BitConverter.GetBytes((int)0));
			}

			return header.ToArray();
		}
	}
}
