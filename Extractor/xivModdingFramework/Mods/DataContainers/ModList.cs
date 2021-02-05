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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace xivModdingFramework.Mods.DataContainers
{
    public class ModList : ICloneable 
    {
        /// <summary>
        /// The ModList Version
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// The list of ModPacks currently installed
        /// </summary>
        public List<ModPack> ModPacks { get; set; }

        /// <summary>
        /// The list of Mods
        /// </summary>
        public List<Mod> Mods { get; set; }

        public object Clone()
        {
            // Since reflection methods have proven slightly unstable for this purpose, the safest
            // method is to simply serialize and deserialize us into a new object.

            // If perf is too bad, we can also introduce a full clone down the chain, but that's
            // slightly less safe in the event any of the classes ever get extended.
            return JsonConvert.DeserializeObject<ModList>(JsonConvert.SerializeObject(this));
        }
    }

    public class Mod
    {
        /// <summary>
        /// The source of the mod
        /// </summary>
        /// <remarks>
        /// This is normally the name of the application used to import the mod
        /// </remarks>
        public string source { get; set; }

        /// <summary>
        /// The modified items name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The modified items category
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// The internal path of the modified item
        /// </summary>
        public string fullPath { get; set; }

        /// <summary>
        /// The dat file where the modified item is located
        /// </summary>
        public string datFile { get; set; }

        /// <summary>
        /// The mod status
        /// </summary>
        /// <remarks>
        /// true if enabled, false if disabled
        /// </remarks>
        public bool enabled { get; set; }

        /// <summary>
        /// The minimum framework version necessary to operate on this mod safely.
        /// </summary>
        public string minimumFrameworkVersion = "1.0.0.0";

        /// <summary>
        /// The modPack associated with this mod
        /// </summary>
        public ModPack modPack { get; set; }

        /// <summary>
        /// The mod data including offsets
        /// </summary>
        public Data data { get; set; }

        public bool IsInternal()
        {
            return source == "_INTERNAL_";
        }

        public bool IsCustomFile()
        {
            return data.modOffset == data.originalOffset;
        }
    }

    public class Data
    {
        /// <summary>
        /// The datatype associated with this mod
        /// </summary>
        /// <remarks>
        /// 2: Binary Data, 3: Models, 4: Textures
        /// </remarks>
        public int dataType { get; set; }

        /// <summary>
        /// The oringial offset of the modified item
        /// </summary>
        /// <remarks>
        /// Used to revert to the items original texture
        /// </remarks>
        public long originalOffset { get; set; }

        /// <summary>
        /// The modified offset of the modified item
        /// </summary>
        public long modOffset { get; set; }

        /// <summary>
        /// The size of the modified items data
        /// </summary>
        /// <remarks>
        /// When importing a previously modified texture, this value is used to determine whether the modified data will be overwritten
        /// </remarks>
        public int modSize { get; set; }

    }

    public class ModPack
    {

        /// <summary>
        /// Generates a hash identifier from this mod's name and author information,
        /// in lowercase.  For identifying new versions of same mods, potentially.
        /// </summary>
        /// <returns></returns>
        public byte[] GetHash()
        {
            using (SHA256 sha = SHA256.Create())
            {
                var n = name.ToLower();
                var a = author.ToLower();
                var key = n + a;
                var keyBytes= Encoding.Unicode.GetBytes(key);
                var hash = sha.ComputeHash(keyBytes);
                return hash;
            }
        }

        /// <summary>
        /// The name of the modpack
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// The modpack author
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// The modpack version
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// The URL the author associated with this modpack.
        /// </summary>
        public string url { get; set; }
    }
}