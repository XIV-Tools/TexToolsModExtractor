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

	/*using xivModdingFramework.Cache;
	using xivModdingFramework.General.Enums;
	using xivModdingFramework.Helpers;
	using xivModdingFramework.Items.Enums;
	using xivModdingFramework.Models.Enums;
	using xivModdingFramework.Models.FileTypes;
	using xivModdingFramework.Models.Helpers;
	using xivModdingFramework.Textures.Enums;
	using static xivModdingFramework.Cache.XivCache;*/

	/// <summary>
	/// Class representing a fully qualified, Square-Enix style Vertex.
	/// In SE's system, these values are all keyed to the same index value, 
	/// so none of them can be separated from the others without creating
	/// an entirely new vertex.
	/// </summary>
	public class TTVertex : ICloneable {
		public Vector3 Position = new Vector3(0,0,0);

		public Vector3 Normal = new Vector3(0, 0, 0);
		public Vector3 Binormal = new Vector3(0, 0, 0);
		public Vector3 Tangent = new Vector3(0, 0, 0);

		// This is Technically BINORMAL handedness in FFXIV.
		// A values of TRUE indicates we need to flip the Tangent when generated. (-1)
		public bool Handedness = false;

		public Vector2 UV1 = new Vector2(0, 0);
		public Vector2 UV2 = new Vector2(0, 0);

		// RGBA
		public byte[] VertexColor = new byte[] { 255, 255, 255, 255 };

		// BoneIds and Weights.  FFXIV Vertices can only be affected by a maximum of 4 bones.
		public byte[] BoneIds = new byte[4];
		public byte[] Weights = new byte[4];

		public static bool operator ==(TTVertex a, TTVertex b)
		{
			// Memberwise equality.
			if (a.Position != b.Position) return false;
			if (a.Normal != b.Normal) return false;
			if (a.Binormal != b.Binormal) return false;
			if (a.Handedness != b.Handedness) return false;
			if (a.UV1 != b.UV1) return false;
			if (a.UV2 != b.UV2) return false;

			for(var ci = 0; ci < 4; ci++)
			{
				if (a.VertexColor[ci] != b.VertexColor[ci]) return false;
				if (a.BoneIds[ci] != b.BoneIds[ci]) return false;
				if (a.Weights[ci] != b.Weights[ci]) return false;

			}

			return true;
		}

		public static bool operator !=(TTVertex a, TTVertex b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(TTVertex)) return false;
			var b = (TTVertex)obj;
			return b == this;
		}

		public object Clone()
		{
			var clone = (TTVertex) this.MemberwiseClone();

			clone.VertexColor = new byte[4];
			clone.BoneIds = new byte[4];
			clone.Weights = new byte[4];

			Array.Copy(this.BoneIds, 0, clone.BoneIds, 0, 4);
			Array.Copy(this.Weights, 0, clone.Weights, 0, 4);
			Array.Copy(this.VertexColor, 0, clone.VertexColor, 0, 4);

			return clone;
		}
	}
}
