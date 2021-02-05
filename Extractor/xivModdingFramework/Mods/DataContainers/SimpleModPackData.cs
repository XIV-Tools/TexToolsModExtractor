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
    public class SimpleModPackData
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
        /// The modpack Url
        /// </summary>
        public string Url = "";

        /// <summary>
        /// The description for the mod pack
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// List of simple mod data
        /// </summary>
        public List<SimpleModData> SimpleModDataList { get; set; }

    }

    public class SimpleModData
    {
        /// <summary>
        /// The name of the item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The category of the item
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
        /// If the entry is SE Default data or not.
        /// </summary>
        public bool IsDefault = false;

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