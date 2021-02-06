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
	using System.Collections.Generic;

	/// <summary>
	/// This class contains properties for the Level of Detail data.
	/// </summary>
	public class LevelOfDetail
	{
		/// <summary>
		/// The offset to the mesh data block.
		/// </summary>
		public ushort MeshOffset;

		/// <summary>
		/// The number of meshes to use.
		/// </summary>
		public short MeshCount;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public int Unknown0;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public int Unknown1;

		/// <summary>
		/// Mesh End.
		/// </summary>
		public short MeshEnd;

		/// <summary>
		/// Extra Mesh Count.
		/// </summary>
		public short ExtraMeshCount;

		/// <summary>
		/// Mesh Sum.
		/// </summary>
		public short MeshSum;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown2;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public int Unknown3;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public int Unknown4;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public int Unknown5;

		/// <summary>
		/// The offset at which the index data begins.
		/// </summary>
		public int IndexDataStart;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public int Unknown6;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public int Unknown7;

		/// <summary>
		/// The size of the Vertex Data Block.
		/// </summary>
		public int VertexDataSize;

		/// <summary>
		/// The size of the Index Data Block.
		/// </summary>
		public int IndexDataSize;

		/// <summary>
		/// The offset to the Vertex Data Block.
		/// </summary>
		public int VertexDataOffset;

		/// <summary>
		/// The offset to the Index Data Block.
		/// </summary>
		public int IndexDataOffset;

		/// <summary>
		/// The list of MeshData for the LoD.
		/// </summary>
		public List<MeshData> MeshDataList;
	}
}