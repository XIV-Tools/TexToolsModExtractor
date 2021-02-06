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
	using HelixToolkit.SharpDX.Core;
	using SharpDX;

	/// <summary>
	/// This class contains the properties for the Vertex Data.
	/// </summary>
	public class VertexData
	{
		/// <summary>
		/// The vertex position data in Vector3 format (X, Y, Z).
		/// </summary>
		public Vector3Collection Positions;

		/// <summary>
		/// The bone weight array per vertex.
		/// </summary>
		/// <remarks>
		/// Each vertex can hold a maximum of 4 bone weights.
		/// </remarks>
		public List<float[]> BoneWeights;

		/// <summary>
		/// The bone index array per vertex.
		/// </summary>
		/// <remarks>
		/// Each vertex can hold a maximum of 4 bone indices.
		/// </remarks>
		public List<byte[]> BoneIndices;

		/// <summary>
		/// The vertex normal data in Vector4 format (X, Y, Z, W).
		/// </summary>
		/// <remarks>
		/// The W coordinate is present but has never been noticed to be anything other than 0.
		/// </remarks>
		public Vector3Collection Normals;

		/// <summary>
		/// The vertex BiNormal data in Vector3 format (X, Y, Z).
		/// </summary>
		public Vector3Collection BiNormals;

		/// <summary>
		/// The vertex BiNormal Handedness data in bytes.
		/// </summary>
		public List<byte> BiNormalHandedness;

		/// <summary>
		/// The vertex Tangent data in Vector3 format (X, Y, Z).
		/// </summary>
		public Vector3Collection Tangents;

		/// <summary>
		/// The vertex color data in Byte4 format (R, G, B, A).
		/// </summary>
		public List<Color> Colors;

		/// <summary>
		/// The vertex color data in Color4 format.
		/// </summary>
		public Color4Collection Colors4;

		/// <summary>
		/// The primary texture coordinates for the mesh in Vector2 format (X, Y).
		/// </summary>
		public Vector2Collection TextureCoordinates0;

		/// <summary>
		/// The secondary texture coordinates for the mesh in Vector2 format (X, Y).
		/// </summary>
		public Vector2Collection TextureCoordinates1;

		/// <summary>
		/// The index data for the mesh.
		/// </summary>
		public IntCollection Indices;
	}
}