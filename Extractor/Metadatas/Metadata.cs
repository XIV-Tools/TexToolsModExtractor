// © XIV-Tools.
// Licensed under the MIT license.

namespace TexToolsModExtractor.Metadatas
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using FfxivResourceConverter;
	using FfxivResourceConverter.Resources;

	/// <summary>
	/// .meta files are an arbitrarily created fake file type used for storing and managing item metadata.
	///
	/// A .meta "file" is composed of five elements.
	///
	/// 1. An EQP entry (Part Hiding Information)
	/// 2. A Set of EQDP Entries (Racial Model Availability)
	/// 3. A Set of IMC Entries (IMC Part Hiding mask, etc.)
	/// 4. A set of EST Table Entries (Extra Skeleton References)
	/// 5. A GMP Entry (Gimmick/Visor Information)
	///
	/// .meta files must be capable of being serialized and deserialized to a pure binary representation,
	/// for storage within DAT files or Modpack files if needed.
	/// </summary>
	public class Metadata
	{
		/// <summary>
		/// The available IMC entries for this item. (May be length 0).
		/// </summary>
		public List<Imc> ImcEntries = new List<Imc>();

		/// <summary>
		/// The available EQDP entries for the item.  (May be length 0).
		/// </summary>
		public Dictionary<XivRace, EquipmentDeformationParameter> EqdpEntries = new Dictionary<XivRace, EquipmentDeformationParameter>();

		/// <summary>
		/// The available Extra Skeleton Table entries for the item.  (May be length 0).
		/// </summary>
		public Dictionary<XivRace, ExtraSkeletonEntry> EstEntries = new Dictionary<XivRace, ExtraSkeletonEntry>();

		/// <summary>
		/// The available Gimmick Paramater for the item. (May be null).
		/// </summary>
		public GimmickParameter GmpEntry;

		/// <summary>
		/// The available EQP entry for the item.  (May be null).
		/// </summary>
		public EquipmentParameter EqpEntry = null;

		// The list of all playable races.
		private static readonly List<XivRace> PlayableRaces = new List<XivRace>()
		{
			XivRace.Hyur_Midlander_Male,
			XivRace.Hyur_Midlander_Female,
			XivRace.Hyur_Highlander_Male,
			XivRace.Hyur_Highlander_Female,
			XivRace.Elezen_Male,
			XivRace.Elezen_Female,
			XivRace.Miqote_Male,
			XivRace.Miqote_Female,
			XivRace.Roegadyn_Male,
			XivRace.Roegadyn_Female,
			XivRace.Lalafell_Male,
			XivRace.Lalafell_Female,
			XivRace.AuRa_Male,
			XivRace.AuRa_Female,
			XivRace.Hrothgar,
			XivRace.Viera,
		};

		public enum XivRace
		{
			[Description("0101")]
			Hyur_Midlander_Male,
			[Description("0104")]
			Hyur_Midlander_Male_NPC,
			[Description("0201")]
			Hyur_Midlander_Female,
			[Description("0204")]
			Hyur_Midlander_Female_NPC,
			[Description("0301")]
			Hyur_Highlander_Male,
			[Description("0304")]
			Hyur_Highlander_Male_NPC,
			[Description("0401")]
			Hyur_Highlander_Female,
			[Description("0404")]
			Hyur_Highlander_Female_NPC,
			[Description("0501")]
			Elezen_Male,
			[Description("0504")]
			Elezen_Male_NPC,
			[Description("0601")]
			Elezen_Female,
			[Description("0604")]
			Elezen_Female_NPC,
			[Description("0701")]
			Miqote_Male,
			[Description("0704")]
			Miqote_Male_NPC,
			[Description("0801")]
			Miqote_Female,
			[Description("0804")]
			Miqote_Female_NPC,
			[Description("0901")]
			Roegadyn_Male,
			[Description("0904")]
			Roegadyn_Male_NPC,
			[Description("1001")]
			Roegadyn_Female,
			[Description("1004")]
			Roegadyn_Female_NPC,
			[Description("1101")]
			Lalafell_Male,
			[Description("1104")]
			Lalafell_Male_NPC,
			[Description("1201")]
			Lalafell_Female,
			[Description("1204")]
			Lalafell_Female_NPC,
			[Description("1301")]
			AuRa_Male,
			[Description("1304")]
			AuRa_Male_NPC,
			[Description("1401")]
			AuRa_Female,
			[Description("1404")]
			AuRa_Female_NPC,
			[Description("1501")]
			Hrothgar,
			[Description("1504")]
			Hrothgar_NPC,
			[Description("1801")]
			Viera,
			[Description("1804")]
			Viera_NPC,
			[Description("9104")]
			NPC_Male,
			[Description("9204")]
			NPC_Female,
			[Description("0000")]
			All_Races,
			[Description("0000")]
			Monster,
			[Description("0000")]
			DemiHuman,
		}

		/// <summary>
		/// Binary enum types for usage when serializing/deserializing the data in question.
		/// These values may be added to but should --NEVER BE CHANGED--, as existing Metadata
		/// entries will depend on the values of these enums.
		/// </summary>
		private enum MetaDataType : uint
		{
			Invalid = 0,
			Imc = 1,
			Eqdp = 2,
			Eqp = 3,
			Est = 4,
			Gmp = 5,
		}

		public static List<FileInfo> Expand(FileInfo file)
		{
			Metadata met = FromMeta(file);

			List<FileInfo> expandedfiles = new List<FileInfo>();

			ConverterSettings settngs = new ConverterSettings();

			for (int i = 0; i < met.ImcEntries.Count; i++)
			{
				string name = Path.GetFileNameWithoutExtension(file.FullName);
				FileInfo imcfile = new FileInfo(file.DirectoryName + "/" + name + "_imc_" + i);
				met.ImcEntries[i].ToImc(imcfile, settngs);
				expandedfiles.Add(imcfile);
			}

			/*foreach (KeyValuePair<XivRace, EquipmentDeformationParameter> entry in met.EqdpEntries)
			{
			}

			foreach (KeyValuePair<XivRace, ExtraSkeletonEntry> entry in met.EstEntries)
			{
			}

			GmpEntry;
			EqpEntry;
			*/

			return expandedfiles;
		}

		public static Metadata FromMeta(FileInfo file)
		{
			using BinaryReader reader = new BinaryReader(file.OpenRead());

			uint version = reader.ReadUInt32();

			// File path name.
			string path = string.Empty;
			char c;
			while ((c = reader.ReadChar()) != '\0')
			{
				path += c;
			}

			Console.WriteLine("WARNING: No slot for metadata!");
			string slot = "UNK";

			////var root = await XivCache.GetFirstRoot(path);
			Metadata ret = new Metadata();

			// General header data.
			uint headerCount = reader.ReadUInt32();
			uint perHeaderSize = reader.ReadUInt32();
			uint headerEntryStart = reader.ReadUInt32();

			// Per-Segment Header data.
			reader.BaseStream.Seek(headerEntryStart, SeekOrigin.Begin);

			List<(MetaDataType type, uint offset, uint size)> entries = new List<(MetaDataType type, uint size, uint offset)>();

			for (int i = 0; i < headerCount; i++)
			{
				// Save offset.
				long currentOffset = reader.BaseStream.Position;

				// Read data.
				MetaDataType type = (MetaDataType)reader.ReadUInt32();
				uint offset = reader.ReadUInt32();
				uint size = reader.ReadUInt32();

				entries.Add((type, offset, size));

				// Seek to next.
				reader.BaseStream.Seek(currentOffset + perHeaderSize, SeekOrigin.Begin);
			}

			(MetaDataType type, uint offset, uint size) imc = entries.FirstOrDefault(x => x.type == MetaDataType.Imc);
			if (imc.type != MetaDataType.Invalid)
			{
				reader.BaseStream.Seek(imc.offset, SeekOrigin.Begin);
				byte[] bytes = reader.ReadBytes((int)imc.size);

				// Deserialize IMC entry bytes here.
				ret.ImcEntries = DeserializeImcData(bytes, version);
			}

			(MetaDataType type, uint offset, uint size) eqp = entries.FirstOrDefault(x => x.type == MetaDataType.Eqp);
			if (eqp.type != MetaDataType.Invalid)
			{
				reader.BaseStream.Seek(eqp.offset, SeekOrigin.Begin);
				byte[] bytes = reader.ReadBytes((int)eqp.size);

				// Deserialize EQP entry bytes here.
				ret.EqpEntry = new EquipmentParameter(slot, bytes);
				////ret.EqpEntry = DeserializeEqpData(bytes, root, version);
			}

			(MetaDataType type, uint offset, uint size) eqdp = entries.FirstOrDefault(x => x.type == MetaDataType.Eqdp);
			if (eqdp.type != MetaDataType.Invalid)
			{
				reader.BaseStream.Seek(eqdp.offset, SeekOrigin.Begin);
				byte[] bytes = reader.ReadBytes((int)eqdp.size);
				ret.EqdpEntries = DeserializeEqdpData(bytes, version);
			}

			(MetaDataType type, uint offset, uint size) est = entries.FirstOrDefault(x => x.type == MetaDataType.Est);
			if (est.type != MetaDataType.Invalid)
			{
				reader.BaseStream.Seek(est.offset, SeekOrigin.Begin);
				byte[] bytes = reader.ReadBytes((int)est.size);
				ret.EstEntries = DeserializeEstData(bytes, version);
			}

			(MetaDataType type, uint offset, uint size) gmp = entries.FirstOrDefault(x => x.type == MetaDataType.Gmp);
			if (gmp.type != MetaDataType.Invalid)
			{
				reader.BaseStream.Seek(gmp.offset, SeekOrigin.Begin);
				byte[] bytes = reader.ReadBytes((int)gmp.size);
				ret.GmpEntry = DeserializeGmpData(bytes, version);
			}

			// Done deserializing all the parts.
			return ret;
		}

		/// <summary>
		/// Deserializes the binary IMC data into a list of IMC entries.
		/// </summary>
		private static List<Imc> DeserializeImcData(byte[] data, uint dataVersion)
		{
			const int ImcSubEntrySize = 6;
			int entries = data.Length / ImcSubEntrySize;

			List<Imc> ret = new List<Imc>();
			for (int i = 0; i < entries; i++)
			{
				byte[] entryData = data.Skip(i * ImcSubEntrySize).Take(ImcSubEntrySize).ToArray();

				using BinaryReader br = new BinaryReader(new MemoryStream(entryData));

				byte variant = br.ReadByte();
				byte unknown = br.ReadByte();
				ushort mask = br.ReadUInt16();
				byte vfx = br.ReadByte();
				byte anim = br.ReadByte();
				ret.Add(new Imc()
				{
					MaterialSet = variant,
					Decal = unknown,
					Mask = mask,
					Vfx = vfx,
					Animation = anim,
				});
			}

			return ret;
		}

		/// <summary>
		/// Deserializes the binary EQDP data into a dictionary of EQDP entries.
		/// </summary>
		private static Dictionary<XivRace, EquipmentDeformationParameter> DeserializeEqdpData(byte[] data, uint dataVersion)
		{
			const int eqdpEntrySize = 5;
			int entries = data.Length / eqdpEntrySize;

			Dictionary<XivRace, EquipmentDeformationParameter> ret = new Dictionary<XivRace, EquipmentDeformationParameter>();

			int read = 0;
			using (BinaryReader reader = new BinaryReader(new MemoryStream(data)))
			{
				while (read < entries)
				{
					int raceCode = reader.ReadInt32();
					XivRace race = GetXivRace(raceCode.ToString().PadLeft(4, '0'));

					byte eqpByte = reader.ReadByte();
					EquipmentDeformationParameter entry = EquipmentDeformationParameter.FromByte(eqpByte);

					ret.Add(race, entry);

					read++;
				}
			}

			// Catch for cases where for some reason the EQP doesn't have all races,
			// for example, SE adding more races in the future, and we're
			// reading old metadata entries.
			foreach (XivRace race in PlayableRaces)
			{
				if (!ret.ContainsKey(race))
				{
					ret.Add(race, new EquipmentDeformationParameter());
				}
			}

			return ret;
		}

		private static Dictionary<XivRace, ExtraSkeletonEntry> DeserializeEstData(byte[] data, uint dataVersion)
		{
			if (dataVersion == 1)
			{
				// Version 1 didn't include EST data, so just get the defaults.
				return null; //// await Est.GetExtraSkeletonEntries(root);
			}

			// 6 Bytes per entry.
			int count = data.Length / 6;
			Dictionary<XivRace, ExtraSkeletonEntry> ret = new Dictionary<XivRace, ExtraSkeletonEntry>(count);

			for (int i = 0; i < count; i++)
			{
				int offset = i * 6;
				ushort raceCode = BitConverter.ToUInt16(data, offset);
				ushort setId = BitConverter.ToUInt16(data, offset + 2);
				ushort skelId = BitConverter.ToUInt16(data, offset + 4);

				XivRace race = GetXivRace(raceCode);

				ret.Add(race, new ExtraSkeletonEntry(race, setId, skelId));
			}

			return ret;
		}

		private static GimmickParameter DeserializeGmpData(byte[] data, uint dataVersion)
		{
			if (dataVersion == 1)
			{
				// Version 1 didn't have GMP data, so include the default GMP data.
				////Eqp _eqp = new Eqp(XivCache.GameInfo.GameDirectory);
				return null; ////await _eqp.GetGimmickParameter(root, true);
			}

			// 5 Bytes to parse, ezpz lemon sqzy
			return new GimmickParameter(data);
		}

		private static void ReplaceBytesAt(byte[] original, byte[] toInject, int index)
		{
			for (int i = 0; i < toInject.Length; i++)
			{
				original[index + i] = toInject[i];
			}
		}

		private static XivRace GetXivRace(string value)
		{
			IEnumerable<XivRace> races = Enum.GetValues(typeof(XivRace)).Cast<XivRace>();
			return races.FirstOrDefault(race => GetRaceCode(race) == value);
		}

		private static XivRace GetXivRace(int value)
		{
			string code = value.ToString().PadLeft(4, '0');
			return GetXivRace(code);
		}

		private static string GetRaceCode(XivRace value)
		{
			FieldInfo field = value.GetType().GetField(value.ToString());
			DescriptionAttribute[] attribute = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
			return attribute.Length > 0 ? attribute[0].Description : value.ToString();
		}

		/// <summary>
		/// Class representing an Equipment Deformation parameter for a given race/slot.
		/// </summary>
		public class EquipmentDeformationParameter
		{
#pragma warning disable SA1307
			public bool bit0;
			public bool bit1;
#pragma warning restore SA1307

			/// <summary>
			/// Create a EquipmentDeformation Parameter from a full byte representation.
			/// </summary>
			public static EquipmentDeformationParameter FromByte(byte b)
			{
				BitArray r = new BitArray(new byte[] { b });
				EquipmentDeformationParameter def = new EquipmentDeformationParameter();
				def.bit0 = r[0];
				def.bit1 = r[1];

				return def;
			}

			/// <summary>
			/// Gets a single byte representation of this entry.
			/// </summary>
			public byte GetByte()
			{
				BitArray r = new BitArray(8);
				r[0] = this.bit0;
				r[1] = this.bit1;
				byte[] arr = new byte[1];
				r.CopyTo(arr, 0);

				return arr[0];
			}
		}

		/// <summary>
		/// Class Representing a Extra Skeletons Table Entry for a Equipment Set.
		/// </summary>
		public class ExtraSkeletonEntry
		{
			public ushort SetId;
			public XivRace Race;
			public ushort SkelId;

			public ExtraSkeletonEntry(XivRace race, ushort setId)
			{
				this.Race = race;
				this.SetId = setId;
				this.SkelId = 0;
			}

			public ExtraSkeletonEntry(XivRace race, ushort setId, ushort skelId)
			{
				this.SetId = setId;
				this.Race = race;
				this.SkelId = skelId;
			}

			public static void Write(byte[] data, ExtraSkeletonEntry entry, int count, int index)
			{
				int offset = (int)(4 + (index * 4));
				short raceId = short.Parse(GetRaceCode(entry.Race));
				ReplaceBytesAt(data, BitConverter.GetBytes(entry.SetId), offset);
				ReplaceBytesAt(data, BitConverter.GetBytes(raceId), offset + 2);

				int baseOffset = 4 + (count * 4);
				offset = (int)(baseOffset + (index * 2));
				ReplaceBytesAt(data, BitConverter.GetBytes(entry.SkelId), offset);
			}

			public static ExtraSkeletonEntry Read(byte[] data, uint count, uint index)
			{
				int offset = (int)(4 + (index * 4));

				ushort setId = BitConverter.ToUInt16(data, offset);
				ushort raceId = BitConverter.ToUInt16(data, offset + 2);
				XivRace race = GetXivRace(raceId.ToString().PadLeft(4, '0'));

				uint baseOffset = 4 + (count * 4);
				offset = (int)(baseOffset + (index * 2));

				ushort skelId = BitConverter.ToUInt16(data, offset);

				ExtraSkeletonEntry ret = new ExtraSkeletonEntry(race, setId, skelId);
				return ret;
			}
		}

		public class GimmickParameter
		{
			public bool Enabled;
			public bool Animated;

			public ushort RotationA;
			public ushort RotationB;
			public ushort RotationC;

			public byte UnknownHigh;
			public byte UnknownLow;

			public GimmickParameter()
			{
				this.Enabled = false;
				this.Animated = false;

				this.RotationA = 0;
				this.RotationB = 0;
				this.RotationC = 0;

				this.UnknownHigh = 0;
				this.UnknownLow = 0;
			}

			public GimmickParameter(byte[] bytes)
			{
				uint l = BitConverter.ToUInt32(bytes, 0);

				this.Enabled = (l & 1) > 0;
				this.Animated = (l & 2) > 0;

				uint d1 = l >> 2;
				this.RotationA = (ushort)(d1 & 0x3FF);

				uint d2 = l >> 12;
				this.RotationB = (ushort)(d2 & 0x3FF);

				uint d3 = l >> 22;
				this.RotationC = (ushort)(d3 & 0x3FF);

				this.UnknownHigh = (byte)((bytes[4] >> 4) & 0x0F);
				this.UnknownLow = (byte)(bytes[4] & 0x0F);
			}

			/// <summary>
			/// Retrieves the raw bytewise representation of the parameter.
			/// </summary>
			public byte[] GetBytes()
			{
				int ret = 0;
				if (this.Enabled)
				{
					ret = ret | 1;
				}

				if (this.Animated)
				{
					ret = ret | 2;
				}

				int rot1 = (this.RotationA & 0x3FF) << 2;
				int rot2 = (this.RotationB & 0x3FF) << 12;
				int rot3 = (this.RotationC & 0x3FF) << 22;

				ret = ret | rot1;
				ret = ret | rot2;
				ret = ret | rot3;

				byte last = (byte)((this.UnknownHigh << 4) | (this.UnknownLow & 0x0F));

				byte[] bytes = new byte[5];

				ReplaceBytesAt(bytes, BitConverter.GetBytes(ret), 0);
				bytes[4] = last;

				return bytes;
			}
		}

		/// <summary>
		/// Class representing an EquipmentParameter entry,
		/// mostly contains data relating to whether or not
		/// certain elements should be shown or hidden for
		/// this piece of gear.
		/// </summary>
		public class EquipmentParameter
		{
			/// <summary>
			/// Slot abbreviation for ths parameter set.
			/// </summary>
			public readonly string Slot;

			/// <summary>
			/// The raw bits which make up this parameter.
			/// </summary>
			private BitArray bits;

			/// <summary>
			/// Initializes a new instance of the <see cref="EquipmentParameter"/> class.
			/// Constructor. Slot is required.
			/// </summary>
			public EquipmentParameter(string slot, byte[] rawBytes)
			{
				this.Slot = slot;
				this.bits = new BitArray(rawBytes);
			}

			/// <summary>
			/// Bitwise flags for the main equipment parameter 64 bit array.
			/// Flag names are set based on what they do when they are set to [1].
			/// </summary>
			public enum EquipmentParameterFlag
			{
				// Default flag set is 0x 3F E0 00 70 60 3F 00

				// For FULL GEAR PIECES, they're always marked as TRUE = Show
				// For PARTIAL GEAR PIECES, they're marked as TRUE = HIDE

				// Byte 0 - Body
				EnableBodyFlags = 0,
				BodyHideWaist = 1,
				Bit2 = 2,
				BodyHideShortGloves = 3, // Bit 3 OR Bit 4 is often set on Legacy gear.
				Bit4 = 4,                // Bit 3 OR Bit 4 is often set on Legacy gear.
				BodyHideMidGloves = 5,
				BodyHideLongGloves = 6,
				BodyHideGorget = 7,

				// Byte 1 - Body
				BodyShowLeg = 8,                // When turned off,  Leg hiding data is resolved from this same set, rather than the set of the equipped piece.
				BodyShowHand = 9,               // When turned off, Hand hiding data is resolved from this same set, rather than the set of the equipped piece.
				BodyShowHead = 10,              // When turned off, Head hiding data is resolved from this same set, rather than the set of the equipped piece.
				BodyShowNecklace = 11,
				BodyShowBracelet = 12,          // "Wrist[slot]" is not used in this context b/c it can be confusing with other settings.
				BodyShowTail = 13,
				BodyTriggersomeShapeData = 14,
				Bit15 = 15,

				// Byte 2 - Leg
				EnableLegFlags = 16,
				LegHideKneePads = 17,           // atr_lpd
				LegHideShortBoot = 18,          // atr_leg
				LegHideHalfBoot = 19,           // atr_leg
				LegBootUnknown = 20,
				LegShowFoot = 21,
				Bit22 = 22,
				Bit23 = 23,

				// Byte 3 - Hand
				EnableHandFlags = 24,
				HandHideElbow = 25,            // Requires bit 26 on as well to work.
				HandHideForearm = 26,          // "Wrist[anatomy]" is not used in this context b/c it can be confusing with other settings.
				Bit27 = 27,
				HandShowBracelet = 28,         // "Wrist[slot]" is not used in this context b/c it can be confusing with other settings.
				HandShowRingL = 29,
				HandShowRingR = 30,
				Bit31 = 31,

				// Byte 4 - Foot
				EnableFootFlags = 32,
				FootHideKnee = 33,              // Requires bit 34 on as well to work.
				FootHideCalf = 34,
				FootHideAnkle = 35,             // Usually set to [1], the remaining bits of this byte are always [0].
				Bit36 = 36,
				Bit37 = 37,
				Bit38 = 38,
				Bit39 = 39,

				// Byte 5 - Head & Hair
				EnableHeadFlags = 40,
				HeadHideScalp = 41,          // When set alone, hides top(hat part) of hair.  When set with 42, hides everything.
				HeadHideHair = 42,             // When set with 41, hides everything neck up.  When set without, hides all hair.
				HeadShowHairOverride = 43,     // Overrides Bit 41 & 42 When set.
				HeadHideNeck = 44,
				HeadShowNecklace = 45,
				Bit46 = 46,
				HeadShowEarrings = 47,        // This cannot be toggled off without enabling bit 42.

				// Byte 6 - Ears/Horns/Etc.
				HeadShowEarringsHuman = 48,
				HeadShowEarringsAura = 49,
				HeadShowEarHuman = 50,
				HeadShowEarMiqo = 51,
				HeadShowEarAura = 52,
				HeadShowEarViera = 53,
				HeadUnknownHelmet1 = 54,      // Usually set on for helmets, in place of 48/49
				HeadUnknownHelmet2 = 55,      // Usually set on for helmets, in place of 48/49

				// Byte 7 - Shadowbringers Race Settings
				HeadShowHrothgarHat = 56,
				HeadShowVieraHat = 57,
				Bit58 = 58,
				Bit59 = 59,
				Bit60 = 60,
				Bit61 = 61,
				Bit62 = 62,
				Bit63 = 63,
			}

			/// <summary>
			/// Gets a dictionary of [Slot] => [Flag] => [Index within the slot's byte array] for each flag.
			/// </summary>
			public static Dictionary<string, Dictionary<EquipmentParameterFlag, int>> FlagOffsetDictionaries
			{
				get
				{
					Dictionary<string, Dictionary<EquipmentParameterFlag, int>> ret = new Dictionary<string, Dictionary<EquipmentParameterFlag, int>>()
					{
						{ "met", new Dictionary<EquipmentParameterFlag, int>() },
						{ "top", new Dictionary<EquipmentParameterFlag, int>() },
						{ "glv", new Dictionary<EquipmentParameterFlag, int>() },
						{ "dwn", new Dictionary<EquipmentParameterFlag, int>() },
						{ "sho", new Dictionary<EquipmentParameterFlag, int>() },
					};

					IEnumerable<EquipmentParameterFlag> flags = Enum.GetValues(typeof(EquipmentParameterFlag)).Cast<EquipmentParameterFlag>();

					foreach (EquipmentParameterFlag flag in flags)
					{
						int raw = (int)flag;
						int byteIndex = raw / 8;

						// Find the slot that this byte belongs to.
						KeyValuePair<string, int> slotKv = EquipmentParameterSet.EntryOffsets.Reverse().First(x => x.Value <= byteIndex);
						string slot = slotKv.Key;
						int slotByteOffset = slotKv.Value;

						// Compute the relevant bit position within the slot's grouping.
						int relevantIndex = raw - (slotByteOffset * 8);

						ret[slot].Add(flag, relevantIndex);
					}

					return ret;
				}
			}

			/// <summary>
			/// Gets the available flags for this EquipmentParameter.
			/// </summary>
			public List<EquipmentParameterFlag> AvailableFlags
			{
				get
				{
					return FlagOffsetDictionaries[this.Slot].Keys.ToList();
				}
			}

			/// <summary>
			/// Retrieves the list of all available flags, with their values.
			/// Changing the values will not affect the actual underlying data.
			/// </summary>
			public Dictionary<EquipmentParameterFlag, bool> GetFlags()
			{
				Dictionary<EquipmentParameterFlag, bool> ret = new Dictionary<EquipmentParameterFlag, bool>();
				List<EquipmentParameterFlag> flags = this.AvailableFlags;
				foreach (EquipmentParameterFlag flag in flags)
				{
					ret.Add(flag, this.GetFlag(flag));
				}

				return ret;
			}

			/// <summary>
			/// Set all (or a subset) of flags in this Parameter set at once.
			/// </summary>
			public void SetFlags(Dictionary<EquipmentParameterFlag, bool> flags)
			{
				foreach (KeyValuePair<EquipmentParameterFlag, bool> kv in flags)
				{
					this.SetFlag(kv.Key, kv.Value);
				}
			}

			public bool GetFlag(EquipmentParameterFlag flag)
			{
				if (!FlagOffsetDictionaries[this.Slot].ContainsKey(flag))
					return false;

				int index = FlagOffsetDictionaries[this.Slot][flag];
				return this.bits[index];
			}

			public void SetFlag(EquipmentParameterFlag flag, bool value)
			{
				if (!FlagOffsetDictionaries[this.Slot].ContainsKey(flag))
					return;

				int index = FlagOffsetDictionaries[this.Slot][flag];
				this.bits[index] = value;
			}

			/// <summary>
			/// Gets the raw bytes of this EquipmentParameter.
			/// </summary>
			public byte[] GetBytes()
			{
				byte[] bytes = new byte[this.bits.Count / 8];
				this.bits.CopyTo(bytes, 0);
				return bytes;
			}

			public void SetBytes(byte[] bytes)
			{
				this.bits = new BitArray(bytes);
			}

			public class EquipmentParameterSet
			{
				// Entry order within the set.
				public static readonly List<string> EntryOrder = new List<string>()
				{
					"top", "dwn", "glv", "sho", "met",
				};

				// Byte sizes within the set.
				public static readonly Dictionary<string, int> EntrySizes = new Dictionary<string, int>()
				{
					{ "top", 2 },
					{ "dwn", 1 },
					{ "glv", 1 },
					{ "sho", 1 },
					{ "met", 3 },
				};

				// Byte offsets within the set.
				public static readonly Dictionary<string, int> EntryOffsets = new Dictionary<string, int>()
				{
					{ "top", 0 },
					{ "dwn", 2 },
					{ "glv", 3 },
					{ "sho", 4 },
					{ "met", 5 },
				};

				/// <summary>
				/// The actual parameters contained in this set, by Slot Abbreviation.
				/// Strings should match up to Mdl.SlotAbbreviationDictionary Keys
				/// This element should always contain 5 entries: [met, top, glv, dwn, sho].
				/// </summary>
				public Dictionary<string, EquipmentParameter> Parameters;

				public EquipmentParameterSet(List<byte> rawBytes)
				{
					Dictionary<string, List<byte>> slotBytes = new Dictionary<string, List<byte>>();
					slotBytes.Add("top", new List<byte>());
					slotBytes.Add("dwn", new List<byte>());
					slotBytes.Add("glv", new List<byte>());
					slotBytes.Add("sho", new List<byte>());
					slotBytes.Add("met", new List<byte>());

					slotBytes["top"].Add(rawBytes[0]);
					slotBytes["top"].Add(rawBytes[1]);
					slotBytes["dwn"].Add(rawBytes[2]);
					slotBytes["glv"].Add(rawBytes[3]);
					slotBytes["sho"].Add(rawBytes[4]);
					slotBytes["met"].Add(rawBytes[5]);
					slotBytes["met"].Add(rawBytes[6]);
					slotBytes["met"].Add(rawBytes[7]);

					this.Parameters = new Dictionary<string, EquipmentParameter>()
					{
						{ "top", new EquipmentParameter("top", slotBytes["top"].ToArray()) },
						{ "dwn", new EquipmentParameter("dwn", slotBytes["dwn"].ToArray()) },
						{ "glv", new EquipmentParameter("glv", slotBytes["glv"].ToArray()) },
						{ "sho", new EquipmentParameter("sho", slotBytes["sho"].ToArray()) },
						{ "met", new EquipmentParameter("met", slotBytes["met"].ToArray()) },
					};
				}

				public static List<string> SlotsAsList()
				{
					return new List<string>() { "met", "top", "glv", "dwn", "sho" };
				}
			}
		}
	}
}
