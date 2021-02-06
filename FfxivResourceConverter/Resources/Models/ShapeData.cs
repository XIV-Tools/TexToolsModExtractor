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
	using System.Linq;

	public class ShapeData
	{
		/// <summary>
		/// The list of shapes in the mesh(?).  1:1 With shape strings.
		/// </summary>
		public List<ShapeInfo> ShapeInfoList;

		/// <summary>
		/// The shape parts.  Each full shape is composed of
		/// Shape -> [Lods] -> [Parts for those Lods].
		/// </summary>
		public List<ShapePart> ShapeParts;

		/// <summary>
		/// The Raw shape index list.  This is formatted as
		/// [Base Index To Replace] -> [New Vertex to point to].
		/// </summary>
		public List<ShapeDataEntry> ShapeDataList;

		/// <summary>
		/// Regenerates all the Mesh/Lod Numbers for the ShapeParts
		/// based on the index offsets and other information.
		///
		/// Kind of computationally expensive, so this is only done once
		/// and then cached in the shape parts themselves.
		/// </summary>
		public void AssignMeshAndLodNumbers(List<List<int>> indexOffsets)
		{
			// For every shape...
			foreach (ShapeInfo shape in this.ShapeInfoList)
			{
				string shapeName = shape.Name;

				// And every LoD in that shape...
				for (int lodNum = 0; lodNum < indexOffsets.Count; lodNum++)
				{
					ShapeLodInfo lod = shape.ShapeLods[lodNum];
					List<int> lodOffsets = indexOffsets[lodNum];

					short count = lod.PartCount;
					ushort offset = lod.PartOffset;
					List<ShapePart> parts = new List<ShapePart>(count);

					// And every part in that LoD...
					for (int i = offset; i < offset + count; i++)
					{
						ShapePart part = this.ShapeParts[i];

						// Assign its LoD level
						part.LodLevel = lodNum;

						// Assign its parent Shape name.
						part.ShapeName = shapeName;

						// And see which of our mesh offsets matches it.
						for (int meshNum = 0; meshNum < indexOffsets[lodNum].Count; meshNum++)
						{
							if (indexOffsets[lodNum][meshNum] == part.MeshIndexOffset)
							{
								part.MeshNumber = meshNum;
								break;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Retrieves the shape data for a given part.
		/// </summary>
		public List<ShapeDataEntry> GetShapeData(ShapePart part)
		{
			ShapeDataEntry[] data = new ShapeDataEntry[part.IndexCount];
			this.ShapeDataList.CopyTo(part.ShapeDataOffset, data, 0, part.IndexCount);
			return data.ToList();
		}

		public class ShapeInfo
		{
			/// <summary>
			/// Offset in the string block to the actual textual name.
			/// </summary>
			public int ShapeNameOffset;

			/// <summary>
			/// The textual name of the shape.
			/// </summary>
			public string Name;

			/// <summary>
			/// The list of part divisions within each LoD for this Shape.
			/// </summary>
			public List<ShapeLodInfo> ShapeLods;
		}

		public class ShapePart
		{
			/// <summary>
			/// This should match a Index Data Offset in MeshDataInfo.IndexDataOffset.
			/// </summary>
			public int MeshIndexOffset;

			/// <summary>
			/// The number of indices to read from the Shape Data.
			/// </summary>
			public int IndexCount;

			/// <summary>
			/// The offset to the index in the Shape Data.
			/// </summary>
			public int ShapeDataOffset;

			/// <summary>
			/// Lod Level this part is associated with.
			/// Derived from ShapeData.ShapeInfoList.
			/// </summary>
			public int LodLevel;

			/// <summary>
			/// Mesh Number this part is associated with.
			/// Derived from MeshIndexOffset.
			/// </summary>
			public int MeshNumber;

			/// <summary>
			/// The parent Shape Name that owns this part.
			/// </summary>
			public string ShapeName;
		}

		public class ShapeDataEntry
		{
			/// <summary>
			/// The Triangle Index that will be changed.
			/// </summary>
			public ushort BaseIndex;

			/// <summary>
			/// Vertex # the Triangle Index should point to when the shape data is active.
			/// </summary>
			public ushort ShapeVertex;
		}

		public class ShapeLodInfo
		{
			/// <summary>
			/// Offset where.
			/// </summary>
			public ushort PartOffset;

			/// <summary>
			/// The number of parts in the Shape Data Information.
			/// </summary>
			public short PartCount;

			public List<ShapePart> Parts;
		}
	}
}