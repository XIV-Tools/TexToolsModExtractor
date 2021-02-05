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


namespace xivModdingFramework.Mods.DataContainers
{
	internal class OriginalModPackJson
	{
		/// <summary>
		/// The name of the item
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The item category
		/// </summary>
		public string Category { get; set; }

		/// <summary>
		/// The full path of the item data for the game
		/// </summary>
		public string FullPath { get; set; }

		/// <summary>
		/// The offset to where the mod data is located
		/// </summary>
		public long ModOffset { get; set; }

		/// <summary>
		/// The size of the mod data
		/// </summary>
		public int ModSize { get; set; }

		/// <summary>
		/// The dat file associated with the item
		/// </summary>
		public string DatFile { get; set; }
	}
}