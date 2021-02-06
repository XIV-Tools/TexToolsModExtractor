// © XIV-Tools.
// Licensed under the MIT license.

namespace FfxivResourceConverter.Resources.Models
{
	using System;

	public class TTModelMdl
	{
		/// <summary>
		/// Creates and populates a TTModel object from a raw XivMdl.
		/// </summary>
		public static TTModel FromRaw(Model rawMdl, Action<bool, string> loggingFunction = null)
		{
			if (rawMdl == null)
			{
				return null;
			}

			if (loggingFunction == null)
			{
				loggingFunction = ModelModifiers.NoOp;
			}

			TTModel ttModel = new TTModel();
			ModelModifiers.MergeGeometryData(ttModel, rawMdl, loggingFunction);
			ModelModifiers.MergeAttributeData(ttModel, rawMdl, loggingFunction);
			ModelModifiers.MergeMaterialData(ttModel, rawMdl, loggingFunction);
			try
			{
				ModelModifiers.MergeShapeData(ttModel, rawMdl, loggingFunction);
			}
			catch (Exception ex)
			{
				loggingFunction(true, "Unable to load shape data: " + ex.Message);
				ModelModifiers.ClearShapeData(ttModel, loggingFunction);
			}

			return ttModel;
		}
	}
}
