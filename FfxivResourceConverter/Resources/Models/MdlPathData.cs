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

	/// <summary>
	/// This class contains the properties for the MDL files path data.
	/// </summary>
	public class MdlPathData
	{
		/// <summary>
		/// The number of paths contained in the Path Data Block.
		/// </summary>
		public int PathCount;

		/// <summary>
		/// The size of the Path Data Block.
		/// </summary>
		public int PathBlockSize;

		/// <summary>
		/// The list of attribute strings.
		/// </summary>
		/// <remarks>
		/// These will usually begin with 'atr_'.
		/// </remarks>
		public List<string> AttributeList;

		/// <summary>
		/// The list of bone strings.
		/// </summary>
		/// <remarks>
		/// These will usually begin with 'j_'.
		/// </remarks>
		public List<string> BoneList;

		/// <summary>
		/// The list of material strings.
		/// </summary>
		/// <remarks>
		/// These are references to the MTRL files used by the model.
		/// </remarks>
		public List<string> MaterialList;

		/// <summary>
		/// The list of shape strings.
		/// </summary>
		/// <remarks>
		/// These will usually begin with shp_.
		/// </remarks>
		public List<string> ShapeList;

		/// <summary>
		/// The list of extra path strings.
		/// </summary>
		/// <remarks>
		/// These are extra paths contained in the mdl path data.
		/// </remarks>
		public List<string> ExtraPathList;
	}
}