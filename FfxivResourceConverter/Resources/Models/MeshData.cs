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

	public class MeshData
	{
		/// <summary>
		/// The information for the mesh data.
		/// </summary>
		public MeshDataInfo MeshInfo;

		/// <summary>
		/// The list of parts for the mesh.
		/// </summary>
		public List<MeshPart> MeshPartList;

		/// <summary>
		/// The list of vertex data structures for the mesh.
		/// </summary>
		public List<VertexDataStruct> VertexDataStructList;

		/// <summary>
		/// The vertex data for the mesh.
		/// </summary>
		public VertexData VertexData;

		/// <summary>
		/// Determines whether this mesh contains a body material.
		/// </summary>
		public bool IsBody;

		/// <summary>
		/// A list of the shape paths associated with this mesh.
		/// </summary>
		public List<string> ShapePathList;
	}
}