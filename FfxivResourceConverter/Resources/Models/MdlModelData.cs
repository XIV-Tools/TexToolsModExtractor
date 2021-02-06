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
	/// <summary>
	/// This class cotains the properties for the MDL model data.
	/// </summary>
	/// <remarks>
	/// This section of the MDL file still has a lot of unknowns.
	/// </remarks>
	public class MdlModelData
	{
		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public int Unknown0;

		/// <summary>
		/// The total number of meshes that the model contains.
		/// </summary>
		/// <remarks>
		/// This includes all LoD meshes.
		/// </remarks>
		public short MeshCount;

		/// <summary>
		/// The number of attributes used by the model.
		/// </summary>
		public short AttributeCount;

		/// <summary>
		/// The total number of mesh parts the model contains.
		/// </summary>
		public short MeshPartCount;

		/// <summary>
		/// The number of materials used by the model.
		/// </summary>
		public short MaterialCount;

		/// <summary>
		/// The number of bones used by the model.
		/// </summary>
		public short BoneCount;

		/// <summary>
		/// The total number of Bone Lists the model uses.
		/// </summary>
		/// <remarks>
		/// There is usually one per LoD.
		/// </remarks>
		public short BoneListCount;

		/// <summary>
		/// The number of Mesh Shapes.
		/// </summary>
		public short ShapeCount;

		/// <summary>
		/// The number of data blocks in the mesh shapes.
		/// </summary>
		public short ShapePartCount;

		/// <summary>
		/// The total number of indices that the mesh shapes uses.
		/// </summary>
		public ushort ShapeDataCount;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown1;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown2;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown3;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown4;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown5;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown6;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown7;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown8;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown9;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public byte Unknown10a;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public byte Unknown10b;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown11;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown12;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown13;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown14;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown15;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown16;

		/// <summary>
		/// Unknown Usage.
		/// </summary>
		public short Unknown17;
	}
}