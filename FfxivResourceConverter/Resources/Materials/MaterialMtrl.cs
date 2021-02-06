// © XIV-Tools.
// Licensed under the MIT license.

namespace FfxivResourceConverter.Resources.Materials
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using SharpDX;

	internal static class MaterialMtrl
	{
		public static Material FromMtrl(FileInfo file)
		{
			Material mat = new Material();
			BinaryReader br = new BinaryReader(file.OpenRead());

			int signature = br.ReadInt32();
			int fileSize = br.ReadInt16();

			ushort colorSetDataSize = br.ReadUInt16();
			mat.MaterialDataSize = br.ReadUInt16();
			mat.TexturePathsDataSize = br.ReadUInt16();
			mat.TextureCount = br.ReadByte();
			mat.MapCount = br.ReadByte();
			mat.ColorSetCount = br.ReadByte();
			mat.UnknownDataSize = br.ReadByte();

			List<int> pathSizeList = new List<int>();

			// get the texture path offsets
			mat.TexturePathOffsetList = new List<int>(mat.TextureCount);
			mat.TexturePathUnknownList = new List<short>(mat.TextureCount);
			for (int i = 0; i < mat.TextureCount; i++)
			{
				mat.TexturePathOffsetList.Add(br.ReadInt16());
				mat.TexturePathUnknownList.Add(br.ReadInt16());

				// add the size of the paths
				if (i > 0)
				{
					pathSizeList.Add(
						mat.TexturePathOffsetList[i] - mat.TexturePathOffsetList[i - 1]);
				}
			}

			// get the map path offsets
			mat.MapPathOffsetList = new List<int>(mat.MapCount);
			mat.MapPathUnknownList = new List<short>(mat.MapCount);
			for (int i = 0; i < mat.MapCount; i++)
			{
				mat.MapPathOffsetList.Add(br.ReadInt16());
				mat.MapPathUnknownList.Add(br.ReadInt16());

				// add the size of the paths
				if (i > 0)
				{
					pathSizeList.Add(mat.MapPathOffsetList[i] - mat.MapPathOffsetList[i - 1]);
				}
				else
				{
					if (mat.TextureCount > 0)
					{
						pathSizeList.Add(mat.MapPathOffsetList[i] -
										 mat.TexturePathOffsetList[mat.TextureCount - 1]);
					}
				}
			}

			// get the color set offsets
			mat.ColorSetPathOffsetList = new List<int>(mat.ColorSetCount);
			mat.ColorSetPathUnknownList = new List<short>(mat.ColorSetCount);
			for (int i = 0; i < mat.ColorSetCount; i++)
			{
				mat.ColorSetPathOffsetList.Add(br.ReadInt16());
				mat.ColorSetPathUnknownList.Add(br.ReadInt16());

				// add the size of the paths
				if (i > 0)
				{
					pathSizeList.Add(mat.ColorSetPathOffsetList[i] -
									 mat.ColorSetPathOffsetList[i - 1]);
				}
				else
				{
					pathSizeList.Add(mat.ColorSetPathOffsetList[i] -
									 mat.MapPathOffsetList[mat.MapCount - 1]);
				}
			}

			pathSizeList.Add(mat.TexturePathsDataSize -
							 mat.ColorSetPathOffsetList[mat.ColorSetCount - 1]);

			int count = 0;

			// get the texture path strings
			mat.TexturePathList = new List<string>(mat.TextureCount);
			for (int i = 0; i < mat.TextureCount; i++)
			{
				string texturePath = Encoding.UTF8.GetString(br.ReadBytes(pathSizeList[count]));
				texturePath = texturePath.Replace("\0", string.Empty);

				if (string.IsNullOrEmpty(texturePath))
					continue;

				////string dx11FileName = Path.GetFileName(texturePath).Insert(0, "--");
				////if (await index.FileExists(Path.GetDirectoryName(texturePath).Replace("\\", "/") + "/" + dx11FileName, df))
				////{
				////texturePath = texturePath.Insert(texturePath.LastIndexOf("/") + 1, "--");
				////}

				mat.TexturePathList.Add(texturePath);
				count++;
			}

			// get the map path strings
			mat.MapPathList = new List<string>(mat.MapCount);
			for (int i = 0; i < mat.MapCount; i++)
			{
				mat.MapPathList.Add(Encoding.UTF8.GetString(br.ReadBytes(pathSizeList[count]))
					.Replace("\0", string.Empty));
				count++;
			}

			// get the color set path strings
			mat.ColorSetPathList = new List<string>(mat.ColorSetCount);
			for (int i = 0; i < mat.ColorSetCount; i++)
			{
				mat.ColorSetPathList.Add(Encoding.UTF8.GetString(br.ReadBytes(pathSizeList[count]))
					.Replace("\0", string.Empty));
				count++;
			}

			int shaderPathSize = mat.MaterialDataSize - mat.TexturePathsDataSize;

			mat.Shader = Encoding.UTF8.GetString(br.ReadBytes(shaderPathSize)).Replace("\0", string.Empty);
			mat.Unknown2 = br.ReadBytes(mat.UnknownDataSize);

			mat.ColorSetData = new List<Half>();
			mat.ColorSetDyeData = null;
			if (colorSetDataSize > 0)
			{
				// Color Data is always 512 (6 x 14 = 64 x 8bpp = 512)
				int colorDataSize = 512;

				for (int i = 0; i < colorDataSize / 2; i++)
				{
					mat.ColorSetData.Add(new Half(br.ReadUInt16()));
				}

				// If the color set is 544 in length, it has an extra 32 bytes at the end
				if (colorSetDataSize == 544)
				{
					mat.ColorSetDyeData = br.ReadBytes(32);
				}
			}

			ushort originalShaderParameterDataSize = br.ReadUInt16();
			ushort originalTextureUsageCount = br.ReadUInt16();
			ushort originalShaderParameterCount = br.ReadUInt16();
			ushort originalTextureDescriptorCount = br.ReadUInt16();

			mat.ShaderNumber = br.ReadUInt16();
			mat.Unknown3 = br.ReadUInt16();

			mat.TextureUsageList = new List<TextureUsageStruct>((int)originalTextureUsageCount);
			for (int i = 0; i < originalTextureUsageCount; i++)
			{
				mat.TextureUsageList.Add(new TextureUsageStruct
				{
					TextureType = br.ReadUInt32(),
					Unknown = br.ReadUInt32(),
				});
			}

			mat.ShaderParameterList = new List<ShaderParameterStruct>(originalShaderParameterCount);
			for (int i = 0; i < originalShaderParameterCount; i++)
			{
				mat.ShaderParameterList.Add(new ShaderParameterStruct
				{
					ParameterID = (ShaderParameterStruct.MtrlShaderParameterId)br.ReadUInt32(),
					Offset = br.ReadInt16(),
					Size = br.ReadInt16(),
				});
			}

			mat.TextureDescriptorList = new List<TextureDescriptorStruct>(originalTextureDescriptorCount);
			for (int i = 0; i < originalTextureDescriptorCount; i++)
			{
				mat.TextureDescriptorList.Add(new TextureDescriptorStruct
				{
					TextureType = br.ReadUInt32(),
					FileFormat = br.ReadInt16(),
					Unknown = br.ReadInt16(),
					TextureIndex = br.ReadUInt32(),
				});
			}

			int bytesRead = 0;
			foreach (ShaderParameterStruct shaderParam in mat.ShaderParameterList)
			{
				short offset = shaderParam.Offset;
				short size = shaderParam.Size;
				shaderParam.Args = new List<float>();
				if (bytesRead + size <= originalShaderParameterDataSize)
				{
					for (short idx = offset; idx < offset + size; idx += 4)
					{
						float arg = br.ReadSingle();
						shaderParam.Args.Add(arg);
						bytesRead += 4;
					}
				}
				else
				{
					// Just use a blank array if we have missing/invalid shader data.
					shaderParam.Args = new List<float>(new float[size / 4]);
				}
			}

			// Chew through any remaining padding.
			while (bytesRead < originalShaderParameterDataSize)
			{
				br.ReadByte();
				bytesRead++;
			}

			return mat;
		}
	}
}
