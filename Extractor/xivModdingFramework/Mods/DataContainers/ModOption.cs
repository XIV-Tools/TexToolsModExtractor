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

using System.Collections.Generic;

namespace xivModdingFramework.Mods.DataContainers
{
    public class ModOption
    {
        /// <summary>
        /// The name of the option
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The option description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The preview image for the option
        /// </summary>
        ////public Image Image { get; set; }

        /// <summary>
        /// This is the path for the Image
        /// </summary>
        public string ImageFileName { get; set; }

        /// <summary>
        /// A dictionary containing the (key)mod path and (value)mod data
        /// </summary>
        public Dictionary<string, ModData> Mods { get; } = new Dictionary<string, ModData>();

        /// <summary>
        /// The name of the group this mod option belongs to
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The selection type for this mod option
        /// </summary>
        public string SelectionType { get; set; }

        /// <summary>
        /// The status of the radio or checkbox
        /// </summary>
        public bool IsChecked { get; set; }
    }

    public class ModData
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
        /// The full path of the item
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Whether or not this item is a default SE file.
        /// These files are still included in full, for the sake
        /// of backwards compatability, but in the future may have
        /// handling to simply disable existing mods, rather than copy over.
        /// </summary>
        public bool IsDefault = false;

        /// <summary>
        /// The raw mod data
        /// </summary>
        public byte[] ModDataBytes { get; set; }

    }
}