// © XIV-Tools.
// Licensed under the MIT license.

#pragma warning disable

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
	using HelixToolkit.SharpDX.Core;
	using HelixToolkit.SharpDX.Core.Animations;
	using HelixToolkit.SharpDX.Core.Core;
	using HelixToolkit.SharpDX.Core.Model.Scene2D;
	using Newtonsoft.Json;
	using SharpDX;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;

	/// <summary>
	/// Class representing the base infromation for a Mesh Part, unrelated
	/// to the Item or anything else above the level of the base 3D model.
	/// </summary>
	public class TTMeshPart
	{
		// Purely semantic/not guaranteed to be unique.
		public string Name = null;

		// List of fully qualified TT/SE style vertices.
		public List<TTVertex> Vertices = new List<TTVertex>();

		// List of Vertex IDs that make up the triangles of the mesh.
		public List<int> TriangleIndices = new List<int>();

		// List of Attributes attached to this part.
		public HashSet<string> Attributes = new HashSet<string>();

		public Dictionary<string, TTShapePart> ShapeParts = new Dictionary<string, TTShapePart>();

		/// <summary>
		/// Updates all shapes in this part to any updated UV/Normal/etc. data from the base model.
		/// </summary>
		public void UpdateShapeData()
		{
			foreach(var shpKv in ShapeParts)
			{
				var shp = shpKv.Value;

				foreach(var rKv in shp.VertexReplacements)
				{
					var baseVert = Vertices[rKv.Key];
					var shapeVert = shp.Vertices[rKv.Value];

					shp.Vertices[rKv.Value] = (TTVertex)baseVert.Clone();
					shp.Vertices[rKv.Value].Position = shapeVert.Position;
				}
			}
		}

	}
}
