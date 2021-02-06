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
	/// Class representing a mesh group in TexTools
	/// At the FFXIV level, all the parts are crushed down together into one
	/// Singular 'Mesh'.
	/// </summary>
	public class TTMeshGroup
	{
		public List<TTMeshPart> Parts = new List<TTMeshPart>();

		/// <summary>
		/// Material used by this Mesh Group.
		/// </summary>
		public string Material;

		
		/// <summary>
		/// Purely semantic.
		/// </summary>
		public string Name;


		/// <summary>
		/// List of bones used by this mesh group's vertices.
		/// </summary>
		public List<string> Bones = new List<string>();

		public int GetVertexCount()
		{
			int count = 0;
			foreach(var p in Parts)
			{
				count += p.Vertices.Count;
			}
			return count;
		}

		public int GetIndexCount()
		{
			int count = 0;
			foreach (var p in Parts)
			{
				count += p.TriangleIndices.Count;
			}
			return count;
		}

		/// <summary>
		/// Set an index by its MESH RELEVANT index ID and vertex ID.
		/// </summary>
		/// <param name="indexId"></param>
		/// <param name="vertexIdToSet"></param>
		public void SetIndexAt(int indexId, int vertexIdToSet)
		{
			int verticesSoFar = 0;
			int indicesSoFar = 0;

			foreach(var p in Parts)
			{
				if(indexId >= indicesSoFar + p.TriangleIndices.Count)
				{
					// Need to keep looping.
					verticesSoFar += p.Vertices.Count;
					indicesSoFar += p.TriangleIndices.Count;
					continue;
				}
				// Okay, we've found the part containing our index.
				var relevantIndex = indexId - indicesSoFar;
				var relevantVertex = vertexIdToSet - verticesSoFar;
				if(relevantVertex < 0 || relevantVertex >= p.Vertices.Count)
				{
					throw new InvalidDataException("Cannot set triangle index to vertex which is not contained by the same mesh part.");
				}

				p.TriangleIndices[relevantIndex] = relevantVertex;
			}
		}

		/// <summary>
		/// Set a vertex by its MESH RELEVANT vertex id.
		/// </summary>
		/// <param name="vertex"></param>
		public void SetVertexAt(int vertexId, TTVertex vertex)
		{
			int verticesSoFar = 0;

			foreach (var p in Parts)
			{
				if (vertexId >= verticesSoFar + p.Vertices.Count)
				{
					// Need to keep looping.
					verticesSoFar += p.Vertices.Count;
					continue;
				}

				var relevantVertex = vertexId - verticesSoFar;
				p.Vertices[relevantVertex] = vertex;
			}
		}


		/// <summary>
		/// Retrieves all the part information for a given Mesh-Relevant vertex Id.
		/// </summary>
		/// <param name="vertexId"></param>
		/// <returns></returns>
		public (int PartId, int PartReleventOffset) GetPartRelevantVertexInformation(int vertexId)
		{
			int verticesSoFar = 0;

			var pIdx = 0;
			foreach (var p in Parts)
			{
				if (vertexId >= verticesSoFar + p.Vertices.Count)
				{
					// Need to keep looping.
					verticesSoFar += p.Vertices.Count;
					pIdx++;
					continue;
				}

				var relevantVertex = vertexId - verticesSoFar;
				return (pIdx, relevantVertex);
			}

			return (-1, -1);
		}

		/// <summary>
		/// Gets the part id of the part which owns a given triangle index.
		/// </summary>
		/// <param name="meshRelevantTriangleIndex"></param>
		/// <returns></returns>
		public int GetOwningPartIdByIndex(int meshRelevantTriangleIndex)
		{
			int indicesSoFar = 0;

			var idx = 0;
			foreach (var p in Parts)
			{
				if (meshRelevantTriangleIndex >= indicesSoFar + p.TriangleIndices.Count)
				{
					// Need to keep looping.
					indicesSoFar += p.TriangleIndices.Count;
					idx++;
					continue;
				}
				return idx;
			}
			return -1;
		}

		/// <summary>
		/// Gets the part id of the part which owns a given vertex.
		/// </summary>
		/// <param name="meshRelevantTriangleIndex"></param>
		/// <returns></returns>
		public int GetOwningPartIdByVertex(int meshRelevantVertexId)
		{
			int verticesSoFar = 0;

			var idx = 0;
			foreach (var p in Parts)
			{
				if (meshRelevantVertexId >= verticesSoFar + p.Vertices.Count)
				{
					// Need to keep looping.
					verticesSoFar += p.Vertices.Count;
					idx++;
					continue;
				}
				return idx;
			}
			return -1;
		}

		/// <summary>
		/// Accessor for the full unified MeshGroup level Vertex list.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public TTVertex GetVertexAt(int id)
		{
			if (Parts.Count == 0)
				return null;

			var startingOffset = 0;
			TTMeshPart part = Parts[0];
			foreach(var p in Parts)
			{
				if(startingOffset + p.Vertices.Count < id)
				{
					startingOffset += p.Vertices.Count;
				} else
				{
					part = p;
					break;
				}
			}

			var realId = id - startingOffset;
			return part.Vertices[realId];
		}

		/// <summary>
		/// Accessor for the full unified MeshGroup level Index list
		/// Also corrects the resultant Index to point to the MeshGroup level Vertex list.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public int GetIndexAt(int id)
		{
			if (Parts.Count == 0)
				return -1;

			var startingOffset = 0;
			var partId = 0;
			for(var i = 0; i < Parts.Count; i++)
			{
				var p = Parts[i];
				if (startingOffset + p.TriangleIndices.Count <= id)
				{
					startingOffset += p.TriangleIndices.Count;
				}
				else
				{
					partId = i;
					break;
				}
			}
			var part = Parts[partId];
			var realId = id - startingOffset;
			var realVertexId = part.TriangleIndices[realId];

			var offsets = PartVertexOffsets;
			var modifiedVertexId = realVertexId + offsets[partId];
			return modifiedVertexId;

		}

		/// <summary>
		/// Updates all shapes in this mesh group to any updated UV/Normal/etc. data from the base model.
		/// </summary>
		public void UpdateShapeData()
		{
			foreach (var p in Parts)
			{
				p.UpdateShapeData();
			}
		}

		/// <summary>
		/// When stacked together, this is the list of points which the Triangle Index pointer would start for each part.
		/// </summary>
		public List<int> PartIndexOffsets
		{
			get
			{
				var list = new List<int>();
				var offset = 0;
				foreach (var p in Parts)
				{
					list.Add(offset);
					offset += p.TriangleIndices.Count;
				}
				return list;
			}
		}

		/// <summary>
		/// When stacked together, this is the list of points which the Vertex pointer would start for each part.
		/// </summary>
		public List<int> PartVertexOffsets
		{
			get
			{
				var list = new List<int>();
				var offset = 0;
				foreach (var p in Parts)
				{
					list.Add(offset);
					offset += p.Vertices.Count;
				}
				return list;
			}
		}

		public uint VertexCount
		{
			get
			{
				uint count = 0;
				foreach (var p in Parts)
				{
					count += (uint)p.Vertices.Count;
				}
				return count;
			}
		}
		public uint IndexCount
		{
			get
			{
				uint count = 0;
				foreach (var p in Parts)
				{
					count += (uint)p.TriangleIndices.Count;
				}
				return count;
			}
		}
	}
}
