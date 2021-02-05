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

using System;
using System.Collections.Generic;

namespace xivModdingFramework.Mods.DataContainers
{
	internal class ModPackData
	{
		/// <summary>
		/// The name of the mod pack
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The mod pack author
		/// </summary>
		public string Author { get; set; }

		/// <summary>
		/// The mod pack version
		/// </summary>
		public Version Version { get; set; }

		/// <summary>
		/// The description for the mod pack
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Author's supplied URL for the modpack.
		/// </summary>
		public string Url { get; set; }

		/// <summary>
		/// A list of pages containing a list of mod groups for that particular page
		/// </summary>
		public List<ModPackPage> ModPackPages { get; set; }

		public class ModPackPage
		{
			/// <summary>
			/// The page index
			/// </summary>
			public int PageIndex { get; set; }

			/// <summary>
			/// The list of mod groups contained in the mod pack
			/// </summary>
			public List<ModGroup> ModGroups { get; set; }

		}
	}
}