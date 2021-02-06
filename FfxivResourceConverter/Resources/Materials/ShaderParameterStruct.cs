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
	using System.Collections.Generic;

	/// <summary>
	/// This class containst he information for the shader parameters at the end the MTRL File.
	/// </summary>
	[Serializable]
	public class ShaderParameterStruct
	{
		/// <summary>
		/// Enums for the various shader parameter Ids.  These likely are used to pipe extra data from elsewhere into the shader.
		/// Ex. Local Reflection Maps, Character Skin/Hair Color, Dye Color, etc.
		/// </summary>
		public enum MtrlShaderParameterId : uint
		{
			// Used in every material.  Overwriting bytes with 0s seems to have no effect.
			AlphaLimiter = 699138595,

			// Used in every material.  Overwriting bytes with 0s seems to have no effect.
			Occlusion = 1465565106,

			// This skin args seem to be the same for all races.
			SkinColor = 740963549,
			SkinWetnessLerp = 2569562539,
			SkinMatParamRow2 = 390837838,

			// Always all 0 data?
			SkinUnknown2 = 950420322,
			SkinTileMaterial = 1112929012,

			Face1 = 2274043692,

			// Used in some equipment rarely, particularly Legacy items.  Always seems to have [0]'s for data.
			Equipment1 = 3036724004,

			// This seems to be some kind of setting related to reflecitivty/specular intensity.
			Reflection1 = 906496720,

			// Character Hair Color?
			Hair1 = 364318261,

			// Character Highlight Color
			Hair2 = 3042205627,

			// This arg is the same for most races, but Highlander and Roe M use a different value
			SkinFresnel = 1659128399,

			// Roe M is the only one that has a change to this arg's data.?
			SkinTileMultiplier = 778088561,

			Furniture1 = 1066058257,
			Furniture2 = 337060565,
			Furniture3 = 2858904847,
			Furniture4 = 2033894819,
			Furniture5 = 2408251504,
			Furniture6 = 1139120744,
			Furniture7 = 3086627810,
			Furniture8 = 2456716813,
			Furniture9 = 3219771693,
			Furniture10 = 2781883474,
			Furniture11 = 2365826946,
			Furniture12 = 3147419510,
			Furniture13 = 133014596,
		}

		public MtrlShaderParameterId ParameterID { get; set; }
		public short Offset { get; set; }
		public short Size { get; set; }
		public List<float> Args { get; set; }
	}
}
