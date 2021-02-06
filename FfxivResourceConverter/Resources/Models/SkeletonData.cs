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
	/// <summary>
	/// This class contains properties for the model skeleton.
	/// </summary>
	/// <remarks>
	/// The model skeleton is kept separate from the mdl file and is usually in
	/// the chara/human/[raceID]/skeleton/base/[bodyID] folder, it is a Havok file.
	/// </remarks>
	public class SkeletonData
	{
		/// <summary>
		/// The Name of the bone.
		/// </summary>
		public string BoneName;

		/// <summary>
		/// The bone number.
		/// </summary>
		public int BoneNumber;

		/// <summary>
		/// The bone parent.
		/// </summary>
		/// <remarks>
		/// The base bone that has no parent will have a value of -1.
		/// </remarks>
		public int BoneParent;

		/// <summary>
		/// The Pose Matrix for the bone.
		/// </summary>
		public float[] PoseMatrix;

		/// <summary>
		/// The Inverse Pose Matrix for the bone.
		/// </summary>
		public float[] InversePoseMatrix;
	}
}