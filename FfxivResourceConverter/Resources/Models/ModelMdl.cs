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
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text;
	using HelixToolkit.SharpDX.Core;
	using SharpDX;

	public static class ModelMdl
	{
		public static Model FromMdl(FileInfo file)
		{
			Model xivMdl = new Model();
			int totalNonNullMaterials = 0;

			using (BinaryReader br = new BinaryReader(file.OpenRead()))
			{
				// Wrong!
				int meshCount = 3;

				// We skip the Vertex Data Structures for now
				// This is done so that we can get the correct number of meshes per LoD first
				br.BaseStream.Seek(64 + (136 * meshCount) + 4, SeekOrigin.Begin);

				MdlPathData mdlPathData = new MdlPathData();
				mdlPathData.PathCount = br.ReadInt32();
				mdlPathData.PathBlockSize = br.ReadInt32();
				mdlPathData.AttributeList = new List<string>();
				mdlPathData.BoneList = new List<string>();
				mdlPathData.MaterialList = new List<string>();
				mdlPathData.ShapeList = new List<string>();
				mdlPathData.ExtraPathList = new List<string>();

				// Get the entire path string block to parse later
				// This will be done when we obtain the path counts for each type
				byte[] pathBlock = br.ReadBytes(mdlPathData.PathBlockSize);

				MdlModelData mdlModelData = new MdlModelData();
				mdlModelData.Unknown0 = br.ReadInt32();
				mdlModelData.MeshCount = br.ReadInt16();
				mdlModelData.AttributeCount = br.ReadInt16();
				mdlModelData.MeshPartCount = br.ReadInt16();
				mdlModelData.MaterialCount = br.ReadInt16();
				mdlModelData.BoneCount = br.ReadInt16();
				mdlModelData.BoneListCount = br.ReadInt16();
				mdlModelData.ShapeCount = br.ReadInt16();
				mdlModelData.ShapePartCount = br.ReadInt16();
				mdlModelData.ShapeDataCount = br.ReadUInt16();
				mdlModelData.Unknown1 = br.ReadInt16();
				mdlModelData.Unknown2 = br.ReadInt16();
				mdlModelData.Unknown3 = br.ReadInt16();
				mdlModelData.Unknown4 = br.ReadInt16();
				mdlModelData.Unknown5 = br.ReadInt16();
				mdlModelData.Unknown6 = br.ReadInt16();
				mdlModelData.Unknown7 = br.ReadInt16();
				mdlModelData.Unknown8 = br.ReadInt16(); // Used for transform count with furniture
				mdlModelData.Unknown9 = br.ReadInt16();
				mdlModelData.Unknown10a = br.ReadByte();
				mdlModelData.Unknown10b = br.ReadByte();
				mdlModelData.Unknown11 = br.ReadInt16();
				mdlModelData.Unknown12 = br.ReadInt16();
				mdlModelData.Unknown13 = br.ReadInt16();
				mdlModelData.Unknown14 = br.ReadInt16();
				mdlModelData.Unknown15 = br.ReadInt16();
				mdlModelData.Unknown16 = br.ReadInt16();
				mdlModelData.Unknown17 = br.ReadInt16();

				// Finished reading all MdlModelData
				// Adding to xivMdl
				xivMdl.ModelData = mdlModelData;

				// Now that we have the path counts wee can parse the path strings
				using (BinaryReader br1 = new BinaryReader(new MemoryStream(pathBlock)))
				{
					// Attribute Paths
					for (int i = 0; i < mdlModelData.AttributeCount; i++)
					{
						// Because we don't know the length of the string, we read the data until we reach a 0 value
						// That 0 value is the space between strings
						byte a;
						List<byte> atrName = new List<byte>();
						while ((a = br1.ReadByte()) != 0)
						{
							atrName.Add(a);
						}

						// Read the string from the byte array and remove null terminators
						string atr = Encoding.ASCII.GetString(atrName.ToArray()).Replace("\0", string.Empty);

						// Add the attribute to the list
						mdlPathData.AttributeList.Add(atr);
					}

					// Bone Paths
					for (int i = 0; i < mdlModelData.BoneCount; i++)
					{
						byte a;
						List<byte> boneName = new List<byte>();
						while ((a = br1.ReadByte()) != 0)
						{
							boneName.Add(a);
						}

						string bone = Encoding.ASCII.GetString(boneName.ToArray()).Replace("\0", string.Empty);

						mdlPathData.BoneList.Add(bone);
					}

					// Material Paths
					for (int i = 0; i < mdlModelData.MaterialCount; i++)
					{
						byte a;
						List<byte> materialName = new List<byte>();
						while ((a = br1.ReadByte()) != 0)
						{
							materialName.Add(a);
						}

						string mat = Encoding.ASCII.GetString(materialName.ToArray()).Replace("\0", string.Empty);
						if (mat.StartsWith("shp_"))
						{
							// Catch case for situation where there's null values at the end of the materials list.
							mdlPathData.ShapeList.Add(mat);
						}
						else
						{
							totalNonNullMaterials++;
							mdlPathData.MaterialList.Add(mat);
						}
					}

					// Shape Paths
					for (int i = 0; i < mdlModelData.ShapeCount; i++)
					{
						byte a;
						List<byte> shapeName = new List<byte>();
						while ((a = br1.ReadByte()) != 0)
						{
							shapeName.Add(a);
						}

						string shp = Encoding.ASCII.GetString(shapeName.ToArray()).Replace("\0", string.Empty);

						mdlPathData.ShapeList.Add(shp);
					}

					long remainingPathData = mdlPathData.PathBlockSize - br1.BaseStream.Position;
					if (remainingPathData > 2)
					{
						while (remainingPathData != 0)
						{
							byte a;
							List<byte> extraName = new List<byte>();
							while ((a = br1.ReadByte()) != 0)
							{
								extraName.Add(a);
								remainingPathData--;
							}

							remainingPathData--;

							if (extraName.Count > 0)
							{
								string extra = Encoding.ASCII.GetString(extraName.ToArray()).Replace("\0", string.Empty);

								mdlPathData.ExtraPathList.Add(extra);
							}
						}
					}
				}

				// Finished reading all Path Data
				// Adding to xivMdl
				xivMdl.PathData = mdlPathData;

				// Currently Unknown Data
				UnknownData0 unkData0 = new UnknownData0
				{
					Unknown = br.ReadBytes(mdlModelData.Unknown2 * 32),
				};

				// Finished reading all UnknownData0
				// Adding to xivMdl
				xivMdl.UnkData0 = unkData0;

				int totalLoDMeshes = 0;

				// We add each LoD to the list
				// Note: There is always 3 LoD
				xivMdl.LoDList = new List<LevelOfDetail>();
				for (int i = 0; i < 3; i++)
				{
					LevelOfDetail lod = new LevelOfDetail
					{
						MeshOffset = br.ReadUInt16(),
						MeshCount = br.ReadInt16(),
						Unknown0 = br.ReadInt32(),
						Unknown1 = br.ReadInt32(),
						MeshEnd = br.ReadInt16(),
						ExtraMeshCount = br.ReadInt16(),
						MeshSum = br.ReadInt16(),
						Unknown2 = br.ReadInt16(),
						Unknown3 = br.ReadInt32(),
						Unknown4 = br.ReadInt32(),
						Unknown5 = br.ReadInt32(),
						IndexDataStart = br.ReadInt32(),
						Unknown6 = br.ReadInt32(),
						Unknown7 = br.ReadInt32(),
						VertexDataSize = br.ReadInt32(),
						IndexDataSize = br.ReadInt32(),
						VertexDataOffset = br.ReadInt32(),
						IndexDataOffset = br.ReadInt32(),
						MeshDataList = new List<MeshData>(),
					};

					// Finished reading LoD
					totalLoDMeshes += lod.MeshCount;

					// if LoD0 shows no mesh, add one (This is rare, but happens on company chest for example)
					if (i == 0 && lod.MeshCount == 0)
					{
						lod.MeshCount = 1;
					}

					// Adding to xivMdl
					xivMdl.LoDList.Add(lod);
				}

				// HACK: This is a workaround for certain furniture items, mainly with picture frames and easel
				bool isEmpty = false;
				try
				{
					isEmpty = br.PeekChar() == 0;
				}
				catch
				{
				}

				if (isEmpty && totalLoDMeshes < mdlModelData.MeshCount)
				{
					xivMdl.ExtraLoDList = new List<LevelOfDetail>();

					for (int i = 0; i < mdlModelData.Unknown10a; i++)
					{
						LevelOfDetail lod = new LevelOfDetail
						{
							MeshOffset = br.ReadUInt16(),
							MeshCount = br.ReadInt16(),
							Unknown0 = br.ReadInt32(),
							Unknown1 = br.ReadInt32(),
							MeshEnd = br.ReadInt16(),
							ExtraMeshCount = br.ReadInt16(),
							MeshSum = br.ReadInt16(),
							Unknown2 = br.ReadInt16(),
							Unknown3 = br.ReadInt32(),
							Unknown4 = br.ReadInt32(),
							Unknown5 = br.ReadInt32(),
							IndexDataStart = br.ReadInt32(),
							Unknown6 = br.ReadInt32(),
							Unknown7 = br.ReadInt32(),
							VertexDataSize = br.ReadInt32(),
							IndexDataSize = br.ReadInt32(),
							VertexDataOffset = br.ReadInt32(),
							IndexDataOffset = br.ReadInt32(),
							MeshDataList = new List<MeshData>(),
						};

						xivMdl.ExtraLoDList.Add(lod);
					}
				}

				// Now that we have the LoD data, we can go back and read the Vertex Data Structures
				// First we save our current position
				long savePosition = br.BaseStream.Position;

				int loDStructPos = 68;

				// for each mesh in each lod
				for (int i = 0; i < xivMdl.LoDList.Count; i++)
				{
					int totalMeshCount = xivMdl.LoDList[i].MeshCount + xivMdl.LoDList[i].ExtraMeshCount;
					for (int j = 0; j < totalMeshCount; j++)
					{
						xivMdl.LoDList[i].MeshDataList.Add(new MeshData());
						xivMdl.LoDList[i].MeshDataList[j].VertexDataStructList = new List<VertexDataStruct>();

						// LoD Index * Vertex Data Structure size + Header
						br.BaseStream.Seek((j * 136) + loDStructPos, SeekOrigin.Begin);

						// If the first byte is 255, we reached the end of the Vertex Data Structs
						byte dataBlockNum = br.ReadByte();
						while (dataBlockNum != 255)
						{
							VertexDataStruct vertexDataStruct = new VertexDataStruct
							{
								DataBlock = dataBlockNum,
								DataOffset = br.ReadByte(),
								DataType = (VertexDataStruct.VertexDataType)br.ReadByte(),
								DataUsage = (VertexDataStruct.VertexUsageType)br.ReadByte(),
							};

							xivMdl.LoDList[i].MeshDataList[j].VertexDataStructList.Add(vertexDataStruct);

							// padding between Vertex Data Structs
							br.ReadBytes(4);

							dataBlockNum = br.ReadByte();
						}
					}

					loDStructPos += 136 * xivMdl.LoDList[i].MeshCount;
				}

				// Now that we finished reading the Vertex Data Structures, we can go back to our saved position
				br.BaseStream.Seek(savePosition, SeekOrigin.Begin);

				// Mesh Data Information
				int meshNum = 0;
				foreach (LevelOfDetail lod in xivMdl.LoDList)
				{
					int totalMeshCount = lod.MeshCount + lod.ExtraMeshCount;

					for (int i = 0; i < totalMeshCount; i++)
					{
						MeshDataInfo meshDataInfo = new MeshDataInfo
						{
							VertexCount = br.ReadInt32(),
							IndexCount = br.ReadInt32(),
							MaterialIndex = br.ReadInt16(),
							MeshPartIndex = br.ReadInt16(),
							MeshPartCount = br.ReadInt16(),
							BoneSetIndex = br.ReadInt16(),
							IndexDataOffset = br.ReadInt32(),
							VertexDataOffset0 = br.ReadInt32(),
							VertexDataOffset1 = br.ReadInt32(),
							VertexDataOffset2 = br.ReadInt32(),
							VertexDataEntrySize0 = br.ReadByte(),
							VertexDataEntrySize1 = br.ReadByte(),
							VertexDataEntrySize2 = br.ReadByte(),
							VertexDataBlockCount = br.ReadByte(),
						};

						lod.MeshDataList[i].MeshInfo = meshDataInfo;

						// In the event we have a null material reference, set it to material 0 to be safe.
						if (meshDataInfo.MaterialIndex >= totalNonNullMaterials)
						{
							meshDataInfo.MaterialIndex = 0;
						}

						string materialString = xivMdl.PathData.MaterialList[meshDataInfo.MaterialIndex];

						// Try block to cover odd cases like Au Ra Male Face #92 where for some reason the
						// Last LoD points to using a shp for a material for some reason.
						try
						{
							string typeChar = materialString[4].ToString() + materialString[9].ToString();

							if (typeChar.Equals("cb"))
							{
								lod.MeshDataList[i].IsBody = true;
							}
						}
						catch (Exception)
						{
						}

						meshNum++;
					}
				}

				// Data block for attributes offset paths
				AttributeDataBlock attributeDataBlock = new AttributeDataBlock
				{
					AttributePathOffsetList = new List<int>(xivMdl.ModelData.AttributeCount),
				};

				for (int i = 0; i < xivMdl.ModelData.AttributeCount; i++)
				{
					attributeDataBlock.AttributePathOffsetList.Add(br.ReadInt32());
				}

				xivMdl.AttrDataBlock = attributeDataBlock;

				// Unknown data block
				// This is commented out to allow housing items to display, the data does not exist for housing items
				// more investigation needed as to what this data is
				UnknownData1 unkData1 = new UnknownData1
				{
					////Unknown = br.ReadBytes(xivMdl.ModelData.Unknown3 * 20)
				};
				xivMdl.UnkData1 = unkData1;

				// Mesh Parts
				foreach (LevelOfDetail lod in xivMdl.LoDList)
				{
					foreach (MeshData meshData in lod.MeshDataList)
					{
						meshData.MeshPartList = new List<MeshPart>();

						for (int i = 0; i < meshData.MeshInfo.MeshPartCount; i++)
						{
							MeshPart meshPart = new MeshPart
							{
								IndexOffset = br.ReadInt32(),
								IndexCount = br.ReadInt32(),
								AttributeBitmask = br.ReadUInt32(),
								BoneStartOffset = br.ReadInt16(),
								BoneCount = br.ReadInt16(),
							};

							meshData.MeshPartList.Add(meshPart);
						}
					}
				}

				// Unknown data block
				UnknownData2 unkData2 = new UnknownData2
				{
					Unknown = br.ReadBytes(xivMdl.ModelData.Unknown9 * 12),
				};
				xivMdl.UnkData2 = unkData2;

				// Data block for materials
				// Currently unknown usage
				MaterialDataBlock matDataBlock = new MaterialDataBlock
				{
					MaterialPathOffsetList = new List<int>(xivMdl.ModelData.MaterialCount),
				};

				for (int i = 0; i < xivMdl.ModelData.MaterialCount; i++)
				{
					matDataBlock.MaterialPathOffsetList.Add(br.ReadInt32());
				}

				xivMdl.MatDataBlock = matDataBlock;

				// Data block for bones
				// Currently unknown usage
				BoneDataBlock boneDataBlock = new BoneDataBlock
				{
					BonePathOffsetList = new List<int>(xivMdl.ModelData.BoneCount),
				};

				for (int i = 0; i < xivMdl.ModelData.BoneCount; i++)
				{
					boneDataBlock.BonePathOffsetList.Add(br.ReadInt32());
				}

				xivMdl.BoneDataBlock = boneDataBlock;

				// Bone Lists
				xivMdl.MeshBoneSets = new List<BoneSet>();
				for (int i = 0; i < xivMdl.ModelData.BoneListCount; i++)
				{
					BoneSet boneIndexMesh = new BoneSet
					{
						BoneIndices = new List<short>(64),
					};

					for (int j = 0; j < 64; j++)
					{
						boneIndexMesh.BoneIndices.Add(br.ReadInt16());
					}

					boneIndexMesh.BoneIndexCount = br.ReadInt32();

					xivMdl.MeshBoneSets.Add(boneIndexMesh);
				}

				ShapeData shapeDataLists = new ShapeData
				{
					ShapeInfoList = new List<ShapeData.ShapeInfo>(),
					ShapeParts = new List<ShapeData.ShapePart>(),
					ShapeDataList = new List<ShapeData.ShapeDataEntry>(),
				};

				int totalPartCount = 0;

				// Shape Info

				// Each shape has a header entry, then a per-lod entry.
				for (int i = 0; i < xivMdl.ModelData.ShapeCount; i++)
				{
					// Header - Offset to the shape name.
					ShapeData.ShapeInfo shapeInfo = new ShapeData.ShapeInfo
					{
						ShapeNameOffset = br.ReadInt32(),
						Name = xivMdl.PathData.ShapeList[i],
						ShapeLods = new List<ShapeData.ShapeLodInfo>(),
					};

					// Per LoD entry (offset to this shape's parts in the shape set)
					List<ushort> dataInfoIndexList = new List<ushort>();
					for (int j = 0; j < xivMdl.LoDList.Count; j++)
					{
						dataInfoIndexList.Add(br.ReadUInt16());
					}

					// Per LoD entry (number of parts in the shape set)
					List<short> infoPartCountList = new List<short>();
					for (int j = 0; j < xivMdl.LoDList.Count; j++)
					{
						infoPartCountList.Add(br.ReadInt16());
					}

					for (int j = 0; j < xivMdl.LoDList.Count; j++)
					{
						ShapeData.ShapeLodInfo shapeIndexPart = new ShapeData.ShapeLodInfo
						{
							PartOffset = dataInfoIndexList[j],
							PartCount = infoPartCountList[j],
						};
						shapeInfo.ShapeLods.Add(shapeIndexPart);
						totalPartCount += shapeIndexPart.PartCount;
					}

					shapeDataLists.ShapeInfoList.Add(shapeInfo);
				}

				// Shape Index Info
				for (int i = 0; i < xivMdl.ModelData.ShapePartCount; i++)
				{
					ShapeData.ShapePart shapeIndexInfo = new ShapeData.ShapePart
					{
						MeshIndexOffset = br.ReadInt32(),  // The offset to the index block this Shape Data should be replacing in. -- This is how Shape Data is tied to each mesh.
						IndexCount = br.ReadInt32(),  // # of triangle indices to replace.
						ShapeDataOffset = br.ReadInt32(),   // The offset where this part should start reading in the Shape Data list.
					};

					shapeDataLists.ShapeParts.Add(shapeIndexInfo);
				}

				// Shape data
				for (int i = 0; i < xivMdl.ModelData.ShapeDataCount; i++)
				{
					ShapeData.ShapeDataEntry shapeData = new ShapeData.ShapeDataEntry
					{
						BaseIndex = br.ReadUInt16(),  // Base Triangle Index we're replacing
						ShapeVertex = br.ReadUInt16(),  // The Vertex that Triangle Index should now point to instead.
					};
					shapeDataLists.ShapeDataList.Add(shapeData);
				}

				xivMdl.MeshShapeData = shapeDataLists;

				// Build the list of offsets so we can match it for shape data.
				List<List<int>> indexOffsets = new List<List<int>>();
				for (int l = 0; l < xivMdl.LoDList.Count; l++)
				{
					indexOffsets.Add(new List<int>());
					for (int m = 0; m < xivMdl.LoDList[l].MeshDataList.Count; m++)
					{
						indexOffsets[l].Add(xivMdl.LoDList[l].MeshDataList[m].MeshInfo.IndexDataOffset);
					}
				}

				xivMdl.MeshShapeData.AssignMeshAndLodNumbers(indexOffsets);

				// Sets the boolean flag if the model has shape data
				xivMdl.HasShapeData = xivMdl.ModelData.ShapeCount > 0;

				// Bone index for Parts
				BoneSet partBoneSet = new BoneSet
				{
					BoneIndexCount = br.ReadInt32(),
					BoneIndices = new List<short>(),
				};

				for (int i = 0; i < partBoneSet.BoneIndexCount / 2; i++)
				{
					partBoneSet.BoneIndices.Add(br.ReadInt16());
				}

				xivMdl.PartBoneSets = partBoneSet;

				// Padding
				xivMdl.PaddingSize = br.ReadByte();
				xivMdl.PaddedBytes = br.ReadBytes(xivMdl.PaddingSize);

				// Bounding box
				BoundingBox boundingBox = new BoundingBox
				{
					PointList = new List<Vector4>(),
				};

				for (int i = 0; i < 8; i++)
				{
					boundingBox.PointList.Add(new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
				}

				xivMdl.BoundBox = boundingBox;

				// Bone Transform Data
				xivMdl.BoneTransformDataList = new List<BoneTransformData>();

				short transformCount = xivMdl.ModelData.BoneCount;

				if (transformCount == 0)
				{
					transformCount = xivMdl.ModelData.Unknown8;
				}

				for (int i = 0; i < transformCount; i++)
				{
					BoneTransformData boneTransformData = new BoneTransformData
					{
						Transform0 = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
						Transform1 = new Vector4(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle()),
					};

					xivMdl.BoneTransformDataList.Add(boneTransformData);
				}

				int lodNum = 0;
				int totalMeshNum = 0;
				foreach (LevelOfDetail lod in xivMdl.LoDList)
				{
					if (lod.MeshCount == 0) continue;

					List<MeshData> meshDataList = lod.MeshDataList;

					if (lod.MeshOffset != totalMeshNum)
					{
						meshDataList = xivMdl.LoDList[lodNum + 1].MeshDataList;
					}

					foreach (MeshData meshData in meshDataList)
					{
						VertexData vertexData = new VertexData
						{
							Positions = new Vector3Collection(),
							BoneWeights = new List<float[]>(),
							BoneIndices = new List<byte[]>(),
							Normals = new Vector3Collection(),
							BiNormals = new Vector3Collection(),
							BiNormalHandedness = new List<byte>(),
							Tangents = new Vector3Collection(),
							Colors = new List<SharpDX.Color>(),
							Colors4 = new Color4Collection(),
							TextureCoordinates0 = new Vector2Collection(),
							TextureCoordinates1 = new Vector2Collection(),
							Indices = new IntCollection(),
						};

						// region Positions

						// Get the Vertex Data Structure for positions
						VertexDataStruct posDataStruct = (from vertexDataStruct in meshData.VertexDataStructList
											 where vertexDataStruct.DataUsage == VertexDataStruct.VertexUsageType.Position
											 select vertexDataStruct).FirstOrDefault();

						int vertexDataOffset;
						int vertexDataSize;

						if (posDataStruct != null)
						{
							// Determine which data block the position data is in
							// This always seems to be in the first data block
							switch (posDataStruct.DataBlock)
							{
								case 0:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset0;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize0;
									break;
								case 1:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset1;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize1;

									break;
								default:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset2;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize2;
									break;
							}

							for (int i = 0; i < meshData.MeshInfo.VertexCount; i++)
							{
								// Get the offset for the position data for each vertex
								int positionOffset = lod.VertexDataOffset + vertexDataOffset + posDataStruct.DataOffset + (vertexDataSize * i);

								// Go to the Data Block
								br.BaseStream.Seek(positionOffset, SeekOrigin.Begin);

								Vector3 positionVector;

								// Position data is either stored in half-floats or singles
								if (posDataStruct.DataType == VertexDataStruct.VertexDataType.Half4)
								{
									SharpDX.Half x = new SharpDX.Half(br.ReadUInt16());
									SharpDX.Half y = new SharpDX.Half(br.ReadUInt16());
									SharpDX.Half z = new SharpDX.Half(br.ReadUInt16());
									SharpDX.Half w = new SharpDX.Half(br.ReadUInt16());

									positionVector = new Vector3(x, y, z);
								}
								else
								{
									float x = br.ReadSingle();
									float y = br.ReadSingle();
									float z = br.ReadSingle();

									positionVector = new Vector3(x, y, z);
								}

								vertexData.Positions.Add(positionVector);
							}
						}

						// region BoneWeights

						// Get the Vertex Data Structure for bone weights
						VertexDataStruct bwDataStruct = (from vertexDataStruct in meshData.VertexDataStructList
											where vertexDataStruct.DataUsage == VertexDataStruct.VertexUsageType.BoneWeight
											select vertexDataStruct).FirstOrDefault();

						if (bwDataStruct != null)
						{
							// Determine which data block the bone weight data is in
							// This always seems to be in the first data block
							switch (bwDataStruct.DataBlock)
							{
								case 0:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset0;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize0;
									break;
								case 1:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset1;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize1;

									break;
								default:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset2;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize2;
									break;
							}

							// There is always one set of bone weights per vertex
							for (int i = 0; i < meshData.MeshInfo.VertexCount; i++)
							{
								int bwOffset = lod.VertexDataOffset + vertexDataOffset + bwDataStruct.DataOffset + (vertexDataSize * i);

								br.BaseStream.Seek(bwOffset, SeekOrigin.Begin);

								float bw0 = br.ReadByte() / 255f;
								float bw1 = br.ReadByte() / 255f;
								float bw2 = br.ReadByte() / 255f;
								float bw3 = br.ReadByte() / 255f;

								vertexData.BoneWeights.Add(new[] { bw0, bw1, bw2, bw3 });
							}
						}

						// region BoneIndices

						// Get the Vertex Data Structure for bone indices
						VertexDataStruct biDataStruct = (from vertexDataStruct in meshData.VertexDataStructList
											where vertexDataStruct.DataUsage == VertexDataStruct.VertexUsageType.BoneIndex
											select vertexDataStruct).FirstOrDefault();

						if (biDataStruct != null)
						{
							// Determine which data block the bone index data is in
							// This always seems to be in the first data block
							switch (biDataStruct.DataBlock)
							{
								case 0:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset0;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize0;
									break;
								case 1:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset1;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize1;

									break;
								default:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset2;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize2;
									break;
							}

							// There is always one set of bone indices per vertex
							for (int i = 0; i < meshData.MeshInfo.VertexCount; i++)
							{
								int biOffset = lod.VertexDataOffset + vertexDataOffset + biDataStruct.DataOffset + (vertexDataSize * i);

								br.BaseStream.Seek(biOffset, SeekOrigin.Begin);

								byte bi0 = br.ReadByte();
								byte bi1 = br.ReadByte();
								byte bi2 = br.ReadByte();
								byte bi3 = br.ReadByte();

								vertexData.BoneIndices.Add(new[] { bi0, bi1, bi2, bi3 });
							}
						}

						// region Normals

						// Get the Vertex Data Structure for Normals
						VertexDataStruct normDataStruct = (from vertexDataStruct in meshData.VertexDataStructList
											  where vertexDataStruct.DataUsage == VertexDataStruct.VertexUsageType.Normal
											  select vertexDataStruct).FirstOrDefault();

						if (normDataStruct != null)
						{
							// Determine which data block the normal data is in
							// This always seems to be in the second data block
							switch (normDataStruct.DataBlock)
							{
								case 0:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset0;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize0;
									break;
								case 1:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset1;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize1;

									break;
								default:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset2;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize2;
									break;
							}

							// There is always one set of normals per vertex
							for (int i = 0; i < meshData.MeshInfo.VertexCount; i++)
							{
								int normOffset = lod.VertexDataOffset + vertexDataOffset + normDataStruct.DataOffset + (vertexDataSize * i);

								br.BaseStream.Seek(normOffset, SeekOrigin.Begin);

								Vector3 normalVector;

								// Normal data is either stored in half-floats or singles
								if (normDataStruct.DataType == VertexDataStruct.VertexDataType.Half4)
								{
									SharpDX.Half x = new SharpDX.Half(br.ReadUInt16());
									SharpDX.Half y = new SharpDX.Half(br.ReadUInt16());
									SharpDX.Half z = new SharpDX.Half(br.ReadUInt16());
									SharpDX.Half w = new SharpDX.Half(br.ReadUInt16());

									normalVector = new Vector3(x, y, z);
								}
								else
								{
									float x = br.ReadSingle();
									float y = br.ReadSingle();
									float z = br.ReadSingle();

									normalVector = new Vector3(x, y, z);
								}

								vertexData.Normals.Add(normalVector);
							}
						}

						// region BiNormals

						// Get the Vertex Data Structure for BiNormals
						VertexDataStruct biNormDataStruct = (from vertexDataStruct in meshData.VertexDataStructList
												where vertexDataStruct.DataUsage == VertexDataStruct.VertexUsageType.Binormal
												select vertexDataStruct).FirstOrDefault();

						if (biNormDataStruct != null)
						{
							// Determine which data block the binormal data is in
							// This always seems to be in the second data block
							switch (biNormDataStruct.DataBlock)
							{
								case 0:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset0;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize0;
									break;
								case 1:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset1;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize1;

									break;
								default:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset2;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize2;
									break;
							}

							// There is always one set of biNormals per vertex
							for (int i = 0; i < meshData.MeshInfo.VertexCount; i++)
							{
								int biNormOffset = lod.VertexDataOffset + vertexDataOffset + biNormDataStruct.DataOffset + (vertexDataSize * i);

								br.BaseStream.Seek(biNormOffset, SeekOrigin.Begin);

								float x = (br.ReadByte() * 2 / 255f) - 1f;
								float y = (br.ReadByte() * 2 / 255f) - 1f;
								float z = (br.ReadByte() * 2 / 255f) - 1f;
								byte w = br.ReadByte();

								vertexData.BiNormals.Add(new Vector3(x, y, z));
								vertexData.BiNormalHandedness.Add(w);
							}
						}

						// region Tangents

						// Get the Vertex Data Structure for Tangents
						VertexDataStruct tangentDataStruct = (from vertexDataStruct in meshData.VertexDataStructList
												 where vertexDataStruct.DataUsage == VertexDataStruct.VertexUsageType.Tangent
												 select vertexDataStruct).FirstOrDefault();

						if (tangentDataStruct != null)
						{
							// Determine which data block the tangent data is in
							// This always seems to be in the second data block
							switch (tangentDataStruct.DataBlock)
							{
								case 0:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset0;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize0;
									break;
								case 1:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset1;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize1;

									break;
								default:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset2;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize2;
									break;
							}

							// There is one set of tangents per vertex
							for (int i = 0; i < meshData.MeshInfo.VertexCount; i++)
							{
								int tangentOffset = lod.VertexDataOffset + vertexDataOffset + tangentDataStruct.DataOffset + (vertexDataSize * i);

								br.BaseStream.Seek(tangentOffset, SeekOrigin.Begin);

								float x = (br.ReadByte() * 2 / 255f) - 1f;
								float y = (br.ReadByte() * 2 / 255f) - 1f;
								float z = (br.ReadByte() * 2 / 255f) - 1f;
								byte w = br.ReadByte();

								vertexData.Tangents.Add(new Vector3(x, y, z));
								////vertexData.TangentHandedness.Add(w);
							}
						}

						// region VertexColor

						// Get the Vertex Data Structure for colors
						VertexDataStruct colorDataStruct = (from vertexDataStruct in meshData.VertexDataStructList
											   where vertexDataStruct.DataUsage == VertexDataStruct.VertexUsageType.Color
											   select vertexDataStruct).FirstOrDefault();

						if (colorDataStruct != null)
						{
							// Determine which data block the color data is in
							// This always seems to be in the second data block
							switch (colorDataStruct.DataBlock)
							{
								case 0:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset0;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize0;
									break;
								case 1:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset1;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize1;

									break;
								default:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset2;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize2;
									break;
							}

							// There is always one set of colors per vertex
							for (int i = 0; i < meshData.MeshInfo.VertexCount; i++)
							{
								int colorOffset = lod.VertexDataOffset + vertexDataOffset + colorDataStruct.DataOffset + (vertexDataSize * i);

								br.BaseStream.Seek(colorOffset, SeekOrigin.Begin);

								byte r = br.ReadByte();
								byte g = br.ReadByte();
								byte b = br.ReadByte();
								byte a = br.ReadByte();

								vertexData.Colors.Add(new SharpDX.Color(r, g, b, a));
								vertexData.Colors4.Add(new Color4(r / 255f, g / 255f, b / 255f, a / 255f));
							}
						}

						// region TextureCoordinates

						// Get the Vertex Data Structure for texture coordinates
						VertexDataStruct tcDataStruct = (from vertexDataStruct in meshData.VertexDataStructList
											where vertexDataStruct.DataUsage == VertexDataStruct.VertexUsageType.TextureCoordinate
											select vertexDataStruct).FirstOrDefault();

						if (tcDataStruct != null)
						{
							// Determine which data block the texture coordinate data is in
							// This always seems to be in the second data block
							switch (tcDataStruct.DataBlock)
							{
								case 0:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset0;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize0;
									break;
								case 1:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset1;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize1;
									break;
								default:
									vertexDataOffset = meshData.MeshInfo.VertexDataOffset2;
									vertexDataSize = meshData.MeshInfo.VertexDataEntrySize2;
									break;
							}

							// There is always one set of texture coordinates per vertex
							for (int i = 0; i < meshData.MeshInfo.VertexCount; i++)
							{
								int tcOffset = lod.VertexDataOffset + vertexDataOffset + tcDataStruct.DataOffset + (vertexDataSize * i);

								br.BaseStream.Seek(tcOffset, SeekOrigin.Begin);

								Vector2 tcVector1;
								Vector2 tcVector2;

								// Normal data is either stored in half-floats or singles
								if (tcDataStruct.DataType == VertexDataStruct.VertexDataType.Half4)
								{
									SharpDX.Half x = new SharpDX.Half(br.ReadUInt16());
									SharpDX.Half y = new SharpDX.Half(br.ReadUInt16());
									SharpDX.Half x1 = new SharpDX.Half(br.ReadUInt16());
									SharpDX.Half y1 = new SharpDX.Half(br.ReadUInt16());

									tcVector1 = new Vector2(x, y);
									tcVector2 = new Vector2(x1, y1);

									vertexData.TextureCoordinates0.Add(tcVector1);
									vertexData.TextureCoordinates1.Add(tcVector2);
								}
								else if (tcDataStruct.DataType == VertexDataStruct.VertexDataType.Half2)
								{
									SharpDX.Half x = new SharpDX.Half(br.ReadUInt16());
									SharpDX.Half y = new SharpDX.Half(br.ReadUInt16());

									tcVector1 = new Vector2(x, y);

									vertexData.TextureCoordinates0.Add(tcVector1);
								}
								else if (tcDataStruct.DataType == VertexDataStruct.VertexDataType.Float2)
								{
									float x = br.ReadSingle();
									float y = br.ReadSingle();

									tcVector1 = new Vector2(x, y);
									vertexData.TextureCoordinates0.Add(tcVector1);
								}
								else if (tcDataStruct.DataType == VertexDataStruct.VertexDataType.Float4)
								{
									float x = br.ReadSingle();
									float y = br.ReadSingle();
									float x1 = br.ReadSingle();
									float y1 = br.ReadSingle();

									tcVector1 = new Vector2(x, y);
									tcVector2 = new Vector2(x1, y1);

									vertexData.TextureCoordinates0.Add(tcVector1);
									vertexData.TextureCoordinates1.Add(tcVector2);
								}
							}
						}

						// region Indices
						int indexOffset = lod.IndexDataOffset + (meshData.MeshInfo.IndexDataOffset * 2);

						br.BaseStream.Seek(indexOffset, SeekOrigin.Begin);

						for (int i = 0; i < meshData.MeshInfo.IndexCount; i++)
						{
							vertexData.Indices.Add(br.ReadUInt16());
						}

						meshData.VertexData = vertexData;
						totalMeshNum++;
					}

					// region MeshShape

					// If the model contains Shape Data, parse the data for each mesh
					if (xivMdl.HasShapeData)
					{
						// Dictionary containing <index data offset, mesh number>
						Dictionary<int, int> indexMeshNum = new Dictionary<int, int>();

						List<ShapeData.ShapeDataEntry> shapeData = xivMdl.MeshShapeData.ShapeDataList;

						// Get the index data offsets in each mesh
						for (int i = 0; i < lod.MeshCount; i++)
						{
							int indexDataOffset = lod.MeshDataList[i].MeshInfo.IndexDataOffset;

							if (!indexMeshNum.ContainsKey(indexDataOffset))
							{
								indexMeshNum.Add(indexDataOffset, i);
							}
						}

						for (int i = 0; i < lod.MeshCount; i++)
						{
							Dictionary<int, Vector3> referencePositionsDictionary = new Dictionary<int, Vector3>();
							SortedDictionary<int, Vector3> meshShapePositionsDictionary = new SortedDictionary<int, Vector3>();
							Dictionary<int, Dictionary<ushort, ushort>> shapeIndexOffsetDictionary = new Dictionary<int, Dictionary<ushort, ushort>>();

							// Shape info list
							List<ShapeData.ShapeInfo> shapeInfoList = xivMdl.MeshShapeData.ShapeInfoList;

							// Number of shape info in each mesh
							short perMeshCount = xivMdl.ModelData.ShapeCount;

							for (int j = 0; j < perMeshCount; j++)
							{
								ShapeData.ShapeInfo shapeInfo = shapeInfoList[j];

								ShapeData.ShapeLodInfo indexPart = shapeInfo.ShapeLods[lodNum];

								// The part count
								short infoPartCount = indexPart.PartCount;

								for (int k = 0; k < infoPartCount; k++)
								{
									// Gets the data info for the part
									ShapeData.ShapePart shapeDataInfo = xivMdl.MeshShapeData.ShapeParts[indexPart.PartOffset + k];

									// The offset in the shape data
									int indexDataOffset = shapeDataInfo.MeshIndexOffset;

									int indexMeshLocation = 0;

									// Determine which mesh the info belongs to
									if (indexMeshNum.ContainsKey(indexDataOffset))
									{
										indexMeshLocation = indexMeshNum[indexDataOffset];

										// Move to the next part if it is not the current mesh
										if (indexMeshLocation != i)
										{
											continue;
										}
									}

									// Get the mesh data
									MeshData mesh = lod.MeshDataList[indexMeshLocation];

									// Get the shape data for the current mesh
									List<ShapeData.ShapeDataEntry> shapeDataForMesh = shapeData.GetRange(shapeDataInfo.ShapeDataOffset, shapeDataInfo.IndexCount);

									// Fill shape data dictionaries
									////ushort dataIndex = ushort.MaxValue;
									foreach (ShapeData.ShapeDataEntry data in shapeDataForMesh)
									{
										int referenceIndex = 0;

										if (data.BaseIndex < mesh.VertexData.Indices.Count)
										{
											// Gets the index to which the data is referencing
											referenceIndex = mesh.VertexData.Indices[data.BaseIndex];

											////throw new Exception($"Reference Index is larger than the index count. Reference Index: {data.ReferenceIndexOffset}  Index Count: {mesh.VertexData.Indices.Count}");
										}

										if (!referencePositionsDictionary.ContainsKey(data.BaseIndex))
										{
											if (mesh.VertexData.Positions.Count > referenceIndex)
											{
												referencePositionsDictionary.Add(data.BaseIndex, mesh.VertexData.Positions[referenceIndex]);
											}
										}

										if (!meshShapePositionsDictionary.ContainsKey(data.ShapeVertex))
										{
											if (data.ShapeVertex >= mesh.VertexData.Positions.Count)
											{
												meshShapePositionsDictionary.Add(data.ShapeVertex, new Vector3(0));
											}
											else
											{
												meshShapePositionsDictionary.Add(data.ShapeVertex, mesh.VertexData.Positions[data.ShapeVertex]);
											}
										}
									}

									if (mesh.ShapePathList != null)
									{
										mesh.ShapePathList.Add(shapeInfo.Name);
									}
									else
									{
										mesh.ShapePathList = new List<string> { shapeInfo.Name };
									}
								}
							}
						}
					}

					lodNum++;
				}
			}

			return xivMdl;
		}
	}
}
