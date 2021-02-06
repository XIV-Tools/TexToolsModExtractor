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
namespace FfxivResourceConverter.Resources.Materials
{
	using System;

	/// <summary>
	/// This class contains the information for the texture usage structs in MTRL data.
	/// </summary>
	[Serializable]
	public class TextureUsageStruct
	{
		// Mappings for this TextureType value to XivTexType value are available in Mtrl.cs
		public uint TextureType { get; set; }

		public uint Unknown { get; set; }
	}
}
