// © XIV-Tools.
// Licensed under the MIT license.

namespace FfxivResourceConverter.Resources
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using FfxivResourceConverter.Resources.Models;

	public class Model
	{
		/// <summary>
		/// The path data contained in the mdl file.
		/// </summary>
		public MdlPathData PathData;

		/// <summary>
		/// The model data contained in the mdl file.
		/// </summary>
		public MdlModelData ModelData;

		/// <summary>
		/// Currently unknown data.
		/// </summary>
		public UnknownData0 UnkData0;

		/// <summary>
		/// The list containing the info for each Level of Detail of the model.
		/// </summary>
		public List<LevelOfDetail> LoDList;

		/// <summary>
		/// The data block containing attribute information.
		/// </summary>
		public AttributeDataBlock AttrDataBlock;

		/// <summary>
		/// Currently unknown data.
		/// </summary>
		public UnknownData1 UnkData1;

		/// <summary>
		/// Currently unknown data.
		/// </summary>
		public UnknownData2 UnkData2;

		/// <summary>
		/// The data block containing material information.
		/// </summary>
		public MaterialDataBlock MatDataBlock;

		/// <summary>
		/// The data block containing bone information.
		/// </summary>
		public BoneDataBlock BoneDataBlock;

		/// <summary>
		/// The list continaing each of the Bone Index Lists for each LoD.
		/// </summary>
		public List<BoneSet> MeshBoneSets;

		/// <summary>
		/// The data containing the information for mesh shapes.
		/// </summary>
		public ShapeData MeshShapeData;

		/// <summary>
		/// The data containing the information for the bone indices used by mesh parts.
		/// </summary>
		public BoneSet PartBoneSets;

		/// <summary>
		/// The size of the padded bytes immediately following.
		/// </summary>
		public byte PaddingSize;

		/// <summary>
		/// The padded bytes.
		/// </summary>
		public byte[] PaddedBytes;

		/// <summary>
		/// The bounding box information for the model.
		/// </summary>
		public BoundingBox BoundBox;

		/// <summary>
		/// The list containing the transform data for each bone.
		/// </summary>
		public List<BoneTransformData> BoneTransformDataList;

		/// <summary>
		/// Flag set when the model has shape data.
		/// </summary>
		public bool HasShapeData;

		/// <summary>
		/// The list containing the info for each etra Level of Detail of the model.
		/// </summary>
		/// <remarks>
		/// This happens when the sum of all LoD mesh counts is less than the model data mesh count.
		/// The number of extra LoDs seems to be the value of Unknown10.
		/// </remarks>
		public List<LevelOfDetail> ExtraLoDList;

		/// <summary>
		/// The list of extra MeshData for the Model.
		/// </summary>
		/// <remarks>
		/// This happens when the sum of all LoD mesh counts is less than the model data mesh count.
		/// </remarks>
		public List<MeshData> ExtraMeshData;

		private TTModel ttModel;

		/// <summary>
		/// Gets a value indicating whether special indicator that says the mesh does not use parts at all.
		/// This is the case for some furniture/background models.
		/// </summary>
		public bool Partless
		{
			get
			{
				bool anyParts = false;
				bool anyIndices = false;
				foreach (MeshData m in this.LoDList[0].MeshDataList)
				{
					anyParts = anyParts || m.MeshPartList.Count > 0;
					if (m.MeshInfo.IndexCount > 0)
					{
						anyIndices = true;
					}
				}

				bool noParts = !anyParts;
				return noParts && (!this.HasShapeData) && anyIndices;
			}
		}

		public static Model FromMdl(FileInfo file)
		{
			Model model = ModelMdl.FromMdl(file);
			model.ttModel = TTModelMdl.FromRaw(model);
			return model;
		}

		public void ToFbx(FileInfo file, ConverterSettings settings)
		{
		}
	}
}
