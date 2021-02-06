// © XIV-Tools.
// Licensed under the MIT license.

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
namespace FfxivResourceConverter.Resources.Models
{
	using HelixToolkit.SharpDX.Core;
	using HelixToolkit.SharpDX.Core.Animations;
	using HelixToolkit.SharpDX.Core.Core;
	using HelixToolkit.SharpDX.Core.Model.Scene2D;
	using Newtonsoft.Json;
	using SharpDX;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;


	/// <summary>
	/// Class representing the base information for a 3D Model, unrelated to the 
	/// item or anything else that it's associated with.  This should be writeable
	/// into the FFXIV file system with some calculation, but is primarly a class
	/// for I/O with importers/exporters, and should not contain information like
	/// padding bytes or unknown bytes unless this is data the end user can 
	/// manipulate to some effect.
	/// </summary>
	public class TTModel
	{
		public static string _SETTINGS_KEY_EXPORT_ALL_BONES = "setting_export_all_bones";

		/// <summary>
		/// The internal or external file path where this TTModel originated from.
		/// </summary>
		public string Source = "";

		/// <summary>
		/// The Mesh groups and parts of this mesh.
		/// </summary>
		public List<TTMeshGroup> MeshGroups = new List<TTMeshGroup>();

		public HashSet<string> ActiveShapes = new HashSet<string>();

		/// <summary>
		/// Enum containing all known races
		/// </summary>
		/// <remarks>
		/// Some of the NPC races don't seem to be present anywhere
		/// They were added to the list in case they are ever used in the future
		/// </remarks>
		public enum XivRace
		{
			[Description("0101")] Hyur_Midlander_Male,
			[Description("0104")] Hyur_Midlander_Male_NPC,
			[Description("0201")] Hyur_Midlander_Female,
			[Description("0204")] Hyur_Midlander_Female_NPC,
			[Description("0301")] Hyur_Highlander_Male,
			[Description("0304")] Hyur_Highlander_Male_NPC,
			[Description("0401")] Hyur_Highlander_Female,
			[Description("0404")] Hyur_Highlander_Female_NPC,
			[Description("0501")] Elezen_Male,
			[Description("0504")] Elezen_Male_NPC,
			[Description("0601")] Elezen_Female,
			[Description("0604")] Elezen_Female_NPC,
			[Description("0701")] Miqote_Male,
			[Description("0704")] Miqote_Male_NPC,
			[Description("0801")] Miqote_Female,
			[Description("0804")] Miqote_Female_NPC,
			[Description("0901")] Roegadyn_Male,
			[Description("0904")] Roegadyn_Male_NPC,
			[Description("1001")] Roegadyn_Female,
			[Description("1004")] Roegadyn_Female_NPC,
			[Description("1101")] Lalafell_Male,
			[Description("1104")] Lalafell_Male_NPC,
			[Description("1201")] Lalafell_Female,
			[Description("1204")] Lalafell_Female_NPC,
			[Description("1301")] AuRa_Male,
			[Description("1304")] AuRa_Male_NPC,
			[Description("1401")] AuRa_Female,
			[Description("1404")] AuRa_Female_NPC,
			[Description("1501")] Hrothgar,
			[Description("1504")] Hrothgar_NPC,
			[Description("1801")] Viera,
			[Description("1804")] Viera_NPC,
			[Description("9104")] NPC_Male,
			[Description("9204")] NPC_Female,
			[Description("0000")] All_Races,
			[Description("0000")] Monster,
			[Description("0000")] DemiHuman,
		}



		#region Calculated Properties

		/// <summary>
		/// Is this TTModel populated from an internal file, or external?
		/// </summary>
		public bool IsInternal
		{
			get
			{
				var regex = new Regex("\\.mdl$");
				var match = regex.Match(Source);
				return match.Success;
			}
		}

		/// <summary>
		/// Readonly list of bones that are used in this model.
		/// </summary>
		public List<string> Bones
		{
			get
			{
				var ret = new SortedSet<string>();
				foreach (var m in MeshGroups)
				{
					foreach(var b in m.Bones)
					{
						ret.Add(b);
					}
				}
				return ret.ToList();
			}
		}

		/// <summary>
		/// Readonly list of Materials used in this model.
		/// </summary>
		public List<string> Materials
		{
			get
			{
				var ret = new SortedSet<string>();
				foreach(var m in MeshGroups)
				{
					if (m.Material != null)
					{
						ret.Add(m.Material);
					}
				}
				return ret.ToList();
			}
		}

		/// <summary>
		/// Readonly list of attributes used by this model.
		/// </summary>
		public List<string> Attributes
		{
			get
			{
				var ret = new SortedSet<string>();
				foreach( var m in MeshGroups)
				{
					foreach(var p in m.Parts)
					{
						foreach(var a in p.Attributes)
						{
							ret.Add(a);
						}
					}
				}
				return ret.ToList();
			}
		}


		/// <summary>
		/// Whether or not to write Shape data to the resulting MDL.
		/// </summary>
		public bool HasShapeData
		{
			get
			{
				return MeshGroups.Any(x => x.Parts.Any( x => x.ShapeParts.Count(x => x.Key.StartsWith("shp_")) > 0 ));
			}
		}
		
		/// <summary>
		/// List of all shape names used in the model.
		/// </summary>
		public List<string> ShapeNames
		{
			get
			{
				var shapes = new SortedSet<string>();
				foreach(var m in MeshGroups)
				{
					foreach(var p in m.Parts)
					{
						foreach (var shp in p.ShapeParts)
						{
							if (!shp.Key.StartsWith("shp_")) continue;
							shapes.Add(shp.Key);
						}
					}
				}
				return shapes.ToList();
			}
		}
		
		/// <summary>
		/// Total # of Shape Parts
		/// </summary>
		public short ShapePartCount
		{
			get
			{
				short sum = 0;
				foreach(var m in MeshGroups)
				{
					HashSet<string> shapeNames = new HashSet<string>();
					foreach (var p in m.Parts)
					{
						foreach(var shp in p.ShapeParts)
						{
							if (!shp.Key.StartsWith("shp_")) continue;
							shapeNames.Add(shp.Key);
						}
					}
					sum += (short)shapeNames.Count;
				}
				return sum;
			}
		}

		/// <summary>
		/// Total Shape Data (Index) Entries
		/// </summary>
		public ushort ShapeDataCount
		{
			get
			{
				uint sum = 0;
				// This one is a little more complex.
				foreach (var m in MeshGroups)
				{
					foreach(var p in m.Parts)
					{
						foreach(var index in p.TriangleIndices)
						{
							// For every index.
							foreach(var shp in p.ShapeParts)
							{
								if (!shp.Key.StartsWith("shp_")) continue;
								// There is an entry for every shape it shows up in.
								if (shp.Value.VertexReplacements.ContainsKey(index))
								{
									sum++;
								}
							}
						}
					}
				}
				
				if(sum > ushort.MaxValue)
				{
					throw new Exception($"Model exceeds the maximum possible shape data indices.\n\nCurrent: {sum.ToString()}\nMaximum: {ushort.MaxValue.ToString()}");
				}

				return (ushort) sum;
			}
		}

		/// <summary>
		/// Per-Shape sum of parts; matches up by index to ShapeNames.
		/// </summary>
		/// <returns></returns>
		public List<short> ShapePartCounts
		{
			get
			{
				var counts = new List<short>(new short[ShapeNames.Count]);
				var shapeNames = ShapeNames;

				var shapes = new SortedSet<string>();
				var shpIdx = 0;
				foreach (var shpNm in shapeNames)
				{
					if (!shpNm.StartsWith("shp_")) continue;
					foreach (var m in MeshGroups)
					{
						if (m.Parts.Any(x => x.ShapeParts.ContainsKey(shpNm)))
						{
							counts[shpIdx]++;
						}
					}
					shpIdx++;
				}
				return counts;
			}
		}

		/// <summary>
		/// Gets all the raw shape data of the mesh for use with importing the data back into FFXIV's file system.
		/// Calling/building this data is somewhat expensive, and should only be done
		/// if actually needed in this specified format.
		/// </summary>
		internal List<(string ShapeName, int MeshId, Dictionary<int, int> IndexReplacements, List<TTVertex> Vertices)> GetRawShapeParts()
		{
			var ret = new List<(string ShapeName, int MeshId, Dictionary<int, int> IndexReplacements, List<TTVertex> Vertices)>();

			var shapeNames = ShapeNames;
			shapeNames.Sort();

			// This is a key of [Mesh] [Part] [Vertex Id] => List of referencing indices
			var partRelevantVertexIdToReferringIndices = new Dictionary<int, Dictionary<int, Dictionary<int, List<int>>>>();
			var meshVertexOffsets = new Dictionary<int, int>();

			var idx = 0;
			foreach(var m in MeshGroups)
			{
				meshVertexOffsets.Add(idx, (int) m.VertexCount);
				idx++;
			}

			foreach (var shpName in shapeNames)
			{
				var mIdx = 0;
				foreach (var m in MeshGroups)
				{
					if (!m.Parts.Any(x => x.ShapeParts.Any(y => y.Key == shpName)))
					{
						mIdx++;
						continue;
					}

					// Generate key if needed.
					if(!partRelevantVertexIdToReferringIndices.ContainsKey(mIdx))
					{
						partRelevantVertexIdToReferringIndices.Add(mIdx, new Dictionary<int, Dictionary<int, List<int>>>());
					}

					Dictionary<int, int> replacements = new Dictionary<int, int>();
					List<TTVertex> vertices = new List<TTVertex>();


					var baseIndexOffset = m.IndexCount;

					var partVertexOffset = 0;
					var partIndexOffset = 0;
					var pIdx = 0;
					foreach(var p in m.Parts)
					{
						// Build index reference table if needed.
						if (!partRelevantVertexIdToReferringIndices[mIdx].ContainsKey(pIdx))
						{
							partRelevantVertexIdToReferringIndices[mIdx].Add(pIdx, new Dictionary<int, List<int>>());

							for(int i = 0; i < p.TriangleIndices.Count; i++)
							{
								var vertexId = p.TriangleIndices[i];
								if (!partRelevantVertexIdToReferringIndices[mIdx][pIdx].ContainsKey(vertexId))
								{
									partRelevantVertexIdToReferringIndices[mIdx][pIdx].Add(vertexId, new List<int>());
								}
								partRelevantVertexIdToReferringIndices[mIdx][pIdx][vertexId].Add(i);
							}
						}

						if (!p.ShapeParts.ContainsKey(shpName))
						{
							partVertexOffset += p.Vertices.Count;
							partIndexOffset += p.TriangleIndices.Count;
							pIdx++;
							continue;
						}

						var shp = p.ShapeParts[shpName];

						// Here we have to convert every vertex into a list of original
						// indices that reference it.
						foreach (var kv in shp.VertexReplacements)
						{
							var partRelevantOriginalVertexId = kv.Key;
							var shapeRelevantReplacementVertexId = kv.Value;
							var originalVertex = p.Vertices[partRelevantOriginalVertexId];
							var newVertex = shp.Vertices[shapeRelevantReplacementVertexId];
							
							// Clone the reference to an array.
							var originalReferencingIndices = partRelevantVertexIdToReferringIndices[mIdx][pIdx][partRelevantOriginalVertexId].ToArray();

							for(int i =0; i < originalReferencingIndices.Length; i++)
							{
								replacements.Add(originalReferencingIndices[i] + partIndexOffset, shapeRelevantReplacementVertexId + meshVertexOffsets[mIdx] + vertices.Count);
							}
						}

						vertices.AddRange(shp.Vertices);

						partIndexOffset += p.TriangleIndices.Count;
						partVertexOffset += p.Vertices.Count;
						pIdx++;
					}



					meshVertexOffsets[mIdx] += vertices.Count;
					ret.Add((shpName, mIdx, replacements, vertices));
					mIdx++;
				}
			}

			return ret;
		}

		private static List<TTVertex> defaultBaseVerts = new List<TTVertex>();
		private static List<TTVertex> defaultShapeVerts = new List<TTVertex>();

		/// <summary>
		/// Whether or not this Model actually has animation/weight data.
		/// </summary>
		public bool HasWeights
		{
			get
			{
				foreach (var m in MeshGroups)
				{
					if (m.Bones.Count > 0)
					{
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// Sum count of Vertices in this model.
		/// </summary>
		public uint VertexCount
		{
			get
			{
				uint count = 0;
				foreach (var m in MeshGroups)
				{
					count += (uint)m.VertexCount;
				}
				return count;
			}
		}

		/// <summary>
		/// Sum count of Indices in this model.
		/// </summary>
		public uint IndexCount
		{
			get
			{
				uint count = 0;
				foreach (var m in MeshGroups)
				{
					count += (uint)m.IndexCount;
				}
				return count;
			}
		}


		/// <summary>
		/// Creates a bone set from the model and group information.
		/// </summary>
		/// <param name="PartNumber"></param>
		public List<byte> GetBoneSet(int groupNumber)
		{
			var fullList = Bones;
			var partial = MeshGroups[groupNumber].Bones;

			var result = new List<byte>(new byte[128]);

			if(partial.Count > 64)
			{
				throw new InvalidDataException("Individual Mesh groups cannot reference more than 64 bones.");
			}

			// This is essential a translation table of [mesh group bone index] => [full model bone index]
			for (int i = 0; i < partial.Count; i++)
			{
				var b = BitConverter.GetBytes(((short) fullList.IndexOf(partial[i])));
				ReplaceBytesAt(result, b, i * 2);
			}

			result.AddRange(BitConverter.GetBytes(partial.Count));

			return result;
		}

		/// <summary>
		/// Replaces the bytes in a given byte array with the bytes from another array, starting at the given index of the original array.
		/// </summary>
		public static void ReplaceBytesAt(List<byte> original, byte[] toInject, int index)
		{
			for (var i = 0; i < toInject.Length; i++)
			{
				original[index + i] = toInject[i];
			};
		}

		/// <summary>
		/// Gets the material index for a given group, based on model and group information.
		/// </summary>
		/// <param name="groupNumber"></param>
		/// <returns></returns>
		public short GetMaterialIndex(int groupNumber) {
			
			// Sanity check
			if (MeshGroups.Count <= groupNumber) return 0;

			var m = MeshGroups[groupNumber];

			
			short index = (short)Materials.IndexOf(m.Material);

			return index > 0 ? index : (short)0; 
		}

		/// <summary>
		/// Retrieves the bitmask value for a part's attributes, based on part and model settings.
		/// </summary>
		/// <param name="groupNumber"></param>
		/// <returns></returns>
		public uint GetAttributeBitmask(int groupNumber, int partNumber)
		{
			var allAttributes = Attributes;
			if(allAttributes.Count > 32)
			{
				throw new InvalidDataException("Models cannot have more than 32 total attributes.");
			}
			uint mask = 0;

			var partAttributes = MeshGroups[groupNumber].Parts[partNumber].Attributes;

			uint bit = 1;
			for(int i = 0; i < allAttributes.Count; i++)
			{
				var a = allAttributes[i];
				bit = (uint)1 << i;

				if(partAttributes.Contains(a))
				{
					mask = (uint)(mask | bit);
				}
				
			}

			return mask;
		}

		#endregion

		#region Major Public Functions

		/*public static Dictionary<string, SkeletonData> ResolveFullBoneHeirarchy(XivRace race, List<string> models, Action<bool, string> loggingFunction = null)
		{
			if (loggingFunction == null)
			{
				loggingFunction = ModelModifiers.NoOp;
			}


			// First thing we need to do here is scrap through the groups to
			// pull back out the extra skeletons of the constituent models.
			var _metRegex = new Regex("e([0-9]{4})_met");
			var _topRegex = new Regex("e([0-9]{4})_top");
			var _faceRegex = new Regex("f([0-9]{4})");
			var _hairRegex = new Regex("h([0-9]{4})");

			int topNum = -1;
			int metNum = -1;
			int faceNum = -1;
			int hairNum = -1;

			foreach (var m in models)
			{
				var metMatch = _metRegex.Match(m);
				var topMatch = _topRegex.Match(m);
				var faceMatch = _faceRegex.Match(m);
				var hairMatch = _hairRegex.Match(m);

				if (metMatch.Success)
				{
					metNum = Int32.Parse(metMatch.Groups[1].Value);
				}
				else if (topMatch.Success)
				{
					topNum = Int32.Parse(topMatch.Groups[1].Value);

				}
				else if (faceMatch.Success)
				{
					faceNum = Int32.Parse(faceMatch.Groups[1].Value);
				}
				else if (hairMatch.Success)
				{
					hairNum = Int32.Parse(hairMatch.Groups[1].Value);
				}
			}

			// This is a list of the roots we'll need to pull extra skeleton data for.
			List<XivDependencyRootInfo> rootsToResolve = new List<XivDependencyRootInfo>();

			if (metNum >= 0)
			{
				var root = new XivDependencyRootInfo();
				root.PrimaryType = XivItemType.equipment;
				root.PrimaryId = metNum;
				root.Slot = "met";

				rootsToResolve.Add(root);
			}
			if (topNum >= 0)
			{
				var root = new XivDependencyRootInfo();
				root.PrimaryType = XivItemType.equipment;
				root.PrimaryId = topNum;
				root.Slot = "top";
				rootsToResolve.Add(root);
			}
			if (faceNum >= 0)
			{
				var root = new XivDependencyRootInfo();
				root.PrimaryType = XivItemType.human;
				root.PrimaryId = XivRaces.GetRaceCodeInt(race);
				root.SecondaryType = XivItemType.face;
				root.SecondaryId = faceNum;
				root.Slot = "fac";
				rootsToResolve.Add(root);
			}
			if (hairNum >= 0)
			{
				var root = new XivDependencyRootInfo();
				root.PrimaryType = XivItemType.human;
				root.PrimaryId = XivRaces.GetRaceCodeInt(race);
				root.SecondaryType = XivItemType.hair;
				root.SecondaryId = hairNum;
				root.Slot = "hir";
				rootsToResolve.Add(root);
			}

			// No extra skeletons using slots were used, just add the base root so we get the race's standard skeleton at least.
			if (rootsToResolve.Count == 0)
			{
				var root = new XivDependencyRootInfo();
				root.PrimaryType = XivItemType.equipment;
				root.PrimaryId = 0;
				root.Slot = "top";
				rootsToResolve.Add(root);
			}

			var boneDict = TTModel.ResolveBoneHeirarchyRaw(rootsToResolve, race, null, loggingFunction);
			return boneDict;
		}*/

		/// <summary>
		/// Updates all shapes in this model to any updated UV/Normal/etc. data from the base model.
		/// </summary>
		public void UpdateShapeData()
		{
			foreach(var m in MeshGroups)
			{
				m.UpdateShapeData();
			}
		}

		#endregion

		#region  Internal Helper Functions

		/*private static float[] NewIdentityMatrix()
		{
			var arr = new float [16];
			arr[0] = 1f;
			arr[1] = 0f;
			arr[2] = 0f;
			arr[3] = 0f;

			arr[4] = 0f;
			arr[5] = 1f;
			arr[6] = 0f;
			arr[7] = 0f;

			arr[8] = 0f;
			arr[9] = 0f;
			arr[10] = 1f;
			arr[11] = 0f;

			arr[12] = 0f;
			arr[13] = 0f;
			arr[14] = 0f;
			arr[15] = 1f;
			return arr;
		}

		public Dictionary<string, SkeletonData> ResolveBoneHeirarchy(List<XivDependencyRootInfo> roots = null, XivRace race = XivRace.All_Races, List<string> bones = null, Action<bool, string> loggingFunction = null)
		{
			if (roots == null || roots.Count == 0)
			{
				if (!IsInternal)
				{
					throw new Exception("Cannot dynamically resolve bone heirarchy for external model.");
				}


				// We can use the raw function here since we know this is a valid internal model file.
				XivDependencyRootInfo root = XivDependencyGraph.ExtractRootInfo(Source);

				if (race == XivRace.All_Races)
				{
					race = IOUtil.GetRaceFromPath(Source);
				}


				// Just our one dynamically found root.
				roots = new List<XivDependencyRootInfo>() { root };
			}

			return TTModel.ResolveBoneHeirarchyRaw(roots, race, bones, loggingFunction);
		}

		/// <summary>
		/// Creates a row-by-row Matrix from a column order float set.
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private static float[] RowsFromColumns(float[] data)
		{
			var formatted = new float[16];

			formatted[0] = data[0];
			formatted[1] = data[4];
			formatted[2] = data[8];
			formatted[3] = data[12];


			formatted[4] = data[1];
			formatted[5] = data[5];
			formatted[6] = data[9];
			formatted[7] = data[13];

			formatted[8] = data[2];
			formatted[9] = data[6];
			formatted[10] = data[10];
			formatted[11] = data[14];

			formatted[12] = data[3];
			formatted[13] = data[7];
			formatted[14] = data[11];
			formatted[15] = data[15];

			return formatted;
		}

		/// <summary>
		/// Resolves the full bone heirarchy necessary to animate this TTModel.
		/// Used when saving the file to DB.  (Or potentially animating it)
		/// </summary>
		/// <returns></returns>
		public static Dictionary<string, SkeletonData> ResolveBoneHeirarchyRaw(List<XivDependencyRootInfo> roots, XivRace race, List<string> bones = null, Action<bool, string> loggingFunction = null)
		{
			if (loggingFunction == null)
			{
				loggingFunction = ModelModifiers.NoOp;
			}

			var fullSkel = new Dictionary<string, SkeletonData>();
			var skelDict = new Dictionary<string, SkeletonData>();
			
			bool parsedBase = false;
			var baseSkeletonPath = "";
			var extraSkeletonPath = "";

			foreach (var root in roots)
			{
				// Do we need to get the base skel still?
				string[] skeletonData;
				if (!parsedBase)
				{
					try
					{
						baseSkeletonPath = Sklb.GetBaseSkeletonFile(root, race);
						skeletonData = File.ReadAllLines(baseSkeletonPath);

						// Parse both skeleton files, starting with the base file.
						foreach (var b in skeletonData)
						{
							if (b == "") continue;
							var j = JsonConvert.DeserializeObject<SkeletonData>(b);
							j.PoseMatrix = RowsFromColumns(j.PoseMatrix);
							fullSkel.Add(j.BoneName, j);
						}

					} catch(Exception ex)
					{
						// If we failed to resolve the bones for some reason, log the error message and use a blank skel.
						loggingFunction(true, "Error Parsing Skeleton ("+ baseSkeletonPath.ToString() +"):" + ex.Message);
					}
					parsedBase = true;
				}


				extraSkeletonPath = Sklb.GetExtraSkeletonFile(root, race);
				// Did this root have an extra skeleton in use?
				if (!String.IsNullOrEmpty(extraSkeletonPath))
				{
					try
					{
						// If it did, add its bones to the resulting skeleton.
						Dictionary<int, int> exTranslationTable = new Dictionary<int, int>();
						skeletonData = File.ReadAllLines(extraSkeletonPath);
						foreach (var b in skeletonData)
						{
							if (b == "") continue;
							var j = JsonConvert.DeserializeObject<SkeletonData>(b);
							j.PoseMatrix = RowsFromColumns(j.PoseMatrix);

							if (fullSkel.ContainsKey(j.BoneName))
							{
								// This is a parent level reference to a base bone.
								exTranslationTable.Add(j.BoneNumber, fullSkel[j.BoneName].BoneNumber);
							} 
							else if (exTranslationTable.ContainsKey(j.BoneParent))
							{
								// Run it through the translation to match up with the base skeleton.
								j.BoneParent = exTranslationTable[j.BoneParent];

								// And generate its own new bone number
								var originalNumber = j.BoneNumber;
								j.BoneNumber = fullSkel.Select(x => x.Value.BoneNumber).Max() + 1;

								fullSkel.Add(j.BoneName, j);
								exTranslationTable.Add(originalNumber, j.BoneNumber);
							} else
							{
								// This is a root bone in the EX skeleton that has no parent element in the base skeleton.
								// Just stick it onto the root bone.
								j.BoneParent = fullSkel["n_root"].BoneNumber;

								// And generate its own new bone number
								var originalNumber = j.BoneNumber;
								j.BoneNumber = fullSkel.Select(x => x.Value.BoneNumber).Max() + 1;

								fullSkel.Add(j.BoneName, j);
								exTranslationTable.Add(originalNumber, j.BoneNumber);

							}
						}
					} catch(Exception ex)
					{
						// If we failed to resolve the bones for some reason, log the error message and use a blank skel.
						loggingFunction(true, "Error Parsing Extra Skeleton (" + extraSkeletonPath.ToString() + "):" + ex.Message);
					}
				}
			}
			

			// If no bones were specified, include all of them.
			if(bones == null)
			{
				bones = new List<string>();
				foreach(var e in fullSkel)
				{
					bones.Add(e.Value.BoneName);
				}

				bones = bones.Distinct().ToList();
			}


			var badBoneId = 900;
			foreach (var s in bones)
			{
				var fixedBone = Regex.Replace(s, "[0-9]+$", string.Empty);

				if (fullSkel.ContainsKey(fixedBone))
				{
					var skel = fullSkel[fixedBone];

					if (skel.BoneParent == -1 && !skelDict.ContainsKey(skel.BoneName))
					{
						skelDict.Add(skel.BoneName, skel);
					}

					while (skel.BoneParent != -1)
					{
						if (!skelDict.ContainsKey(skel.BoneName))
						{
							skelDict.Add(skel.BoneName, skel);
						}
						skel = fullSkel.First(x => x.Value.BoneNumber == skel.BoneParent).Value;

						if (skel.BoneParent == -1 && !skelDict.ContainsKey(skel.BoneName))
						{
							skelDict.Add(skel.BoneName, skel);
						}
					}
				}
				else
				{
					// Create a fake bone for this, rather than strictly crashing out.
					var skel = new SkeletonData();
					skel.BoneName = s;
					skel.BoneNumber = badBoneId;
					badBoneId++;
					skel.BoneParent = 0;
					skel.InversePoseMatrix = NewIdentityMatrix();
					skel.PoseMatrix = NewIdentityMatrix();

					skelDict.Add(s, skel);
					loggingFunction(true, $"The base game skeleton did not contain bone {s}. It has been parented to the root bone.");
				}
			}

			return skelDict;
		}*/


		/// <summary>
		/// Performs a basic sanity check on an incoming TTModel
		/// Returns true if there were no errors or errors that were resolvable.
		/// Returns false if the model was deemed insane.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="loggingFunction"></param>
		/// <returns></returns>
		public static bool SanityCheck(TTModel model, Action<bool, string> loggingFunction = null)
		{
			if (loggingFunction == null)
			{
				loggingFunction = ModelModifiers.NoOp;
			}
			loggingFunction(false, "Validating model sanity...");

			bool hasWeights = model.HasWeights;

			if (model.MeshGroups.Count == 0)
			{
				loggingFunction(true, "Model has no data. - Model must have at least one valid Mesh Group.");
				return false;
			}

			var mIdx = 0;
			foreach(var m in model.MeshGroups)
			{
				if(m.Parts.Count == 0)
				{
					var part = new TTMeshPart();
					part.Name = "Part 0";
					m.Parts.Add(part);
				}

				// Meshes in animated models must have at least one bone in their bone set in order to not generate a crash.
				if(hasWeights && m.Bones.Count == 0)
				{
					m.Bones.Add("n_root");
				}
				mIdx++;
			}

			return true;
		}

		/// <summary>
		/// Checks the model for common valid-but-unusual states that users often end up in by accident, providing 
		/// a warning message for each one, if the conditions are met.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="loggingFunction"></param>
		public static void CheckCommonUserErrors(TTModel model, Action<bool, string> loggingFunction = null)
		{
			if (loggingFunction == null)
			{
				loggingFunction = ModelModifiers.NoOp;
			}
			loggingFunction(false, "Checking for unusual data...");

			if (model.Materials.Count > 4)
			{
				loggingFunction(true, "Model has more than four active materials.  The following materials will be ignored in game: ");
				var idx = 0;
				foreach (var m in model.Materials)
				{
					if (idx >= 4)
					{
						loggingFunction(true, "Material: " + m);
					}
					idx++;
				}
			}

			int mIdx = 0;
			foreach (var m in model.MeshGroups)
			{
				int pIdx = 0;
				foreach (var p in m.Parts)
				{

					if (p.Vertices.Count == 0) continue;

					bool anyAlpha = false;
					bool anyColor = false;
					bool anyWeirdUV1s = false;
					bool anyWeirdUV2s = false;

					foreach (var v in p.Vertices)
					{
						anyAlpha = anyAlpha || (v.VertexColor[3] > 0);
						anyColor = anyColor || (v.VertexColor[0] > 0 || v.VertexColor[1] > 0 || v.VertexColor[2] > 0);
						anyWeirdUV1s = anyWeirdUV1s || (v.UV1.X > 2 || v.UV1.X < -2 || v.UV1.Y > 2 || v.UV1.Y < -2);
						anyWeirdUV2s = anyWeirdUV2s || (v.UV2.X > 2 || v.UV2.X < -2 || v.UV2.Y > 2 || v.UV2.Y < -2);
					}

					if (!anyAlpha)
					{
						loggingFunction(true, "Mesh: " + mIdx + " Part: " + pIdx + " has a fully black Vertex Alpha channel.  This will render the part invisible in-game.  Was this intended?");
					}

					if (!anyColor)
					{
						loggingFunction(true, "Mesh: " + mIdx + " Part: " + pIdx + " has a fully black Vertex Color channel.  This can have unexpected results on in-game rendering.  Was this intended?");
					}

					if (anyWeirdUV1s)
					{
						loggingFunction(true, "Mesh: " + mIdx + " Part: " + pIdx + " has unusual UV1 data.  This can have unexpected results on texture placement.  Was this inteneded?");
					}

					if (anyWeirdUV2s)
					{
						loggingFunction(true, "Mesh: " + mIdx + " Part: " + pIdx + " has unusual UV2 data.  This can have unexpected results on decal placement or opacity.  Was this inteneded?");
					}

					pIdx++;
				}
				mIdx++;
			}

		}

		#endregion
	}
}
