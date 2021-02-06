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
	/// Class representing a shape data part.
	/// A MeshGroup may have any amount of these, including
	/// multiple that have the same shape name.
	/// </summary>
	public class TTShapePart
	{
		/// <summary>
		/// The raw shp_ identifier.
		/// </summary>
		public string Name;

		/// <summary>
		/// The list of vertices this Shape introduces.
		/// </summary>
		public List<TTVertex> Vertices = new List<TTVertex>();

		/// <summary>
		/// Dictionary of [Part Level Vertex #] => [Shape Part Level Vertex #] to replace it with.
		/// </summary>
		public Dictionary<int, int> VertexReplacements = new Dictionary<int, int>(); 
	}
}
