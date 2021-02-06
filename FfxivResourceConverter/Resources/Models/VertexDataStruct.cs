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
namespace FfxivResourceConverter.Resources.Models
{
	/// <summary>
	/// This class contains the properties for the Vertex Data Structures.
	/// </summary>
	public class VertexDataStruct
	{
		/// <summary>
		/// The vertex data block the data belongs to.
		/// </summary>
		/// <remarks>
		/// There are usually 2 data blocks and usually contain the following data
		/// Block 0: Positions, Blend Weights, Blend indices
		/// Block 1: Normal, Tangent, Color, Texture Coordinates.
		/// </remarks>
		public byte DataBlock;

		/// <summary>
		/// The offset to the data within the Data Block.
		/// </summary>
		public byte DataOffset;

		/// <summary>
		/// The type of the data.
		/// </summary>
		public VertexDataType DataType;

		/// <summary>
		/// What the data will be used for.
		/// </summary>
		public VertexUsageType DataUsage;

		/// <summary>
		/// Enum containing the Data Type for data entries in the Vertex Data Blocks.
		/// </summary>
		public enum VertexDataType
		{
			Float1 = 0x0,
			Float2 = 0x1,
			Float3 = 0x2,
			Float4 = 0x3,
			Ubyte4 = 0x5,
			Short2 = 0x6,
			Short4 = 0x7,
			Ubyte4n = 0x8,
			Short2n = 0x9,
			Short4n = 0xA,
			Half2 = 0xD,
			Half4 = 0xE,
			Compress = 0xF,
		}

		/// <summary>
		/// Enum containing the what the data entries in the Vertex Data Block will be used for.
		/// </summary>
		public enum VertexUsageType
		{
			Position = 0x0,
			BoneWeight = 0x1,
			BoneIndex = 0x2,
			Normal = 0x3,
			TextureCoordinate = 0x4,
			Tangent = 0x5,
			Binormal = 0x6,
			Color = 0x7,
		}
	}
}
