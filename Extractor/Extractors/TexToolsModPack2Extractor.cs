// © XIV-Tools.
// Licensed under the MIT license.

namespace TexToolsModExtractor.Extractors
{
	using System;
	using System.IO;
	using System.IO.Compression;
	using System.Linq;
	using Lumina.Data;
	using Newtonsoft.Json;
	using xivModdingFramework.Mods.DataContainers;

	public class TexToolsModPack2Extractor : ExtractorBase
	{
		public override void Extract(FileInfo file)
		{
			string outputDirPath = Path.GetFileNameWithoutExtension(file.Name) + "_Extracted/";
			DirectoryInfo outputDirectory = file.Directory.CreateSubdirectory(outputDirPath);
			outputDirectory.Delete(true);
			outputDirectory.Create();

			string mplPath = outputDirectory + "ttmp.mpl";
			string mpdPath = outputDirectory + "ttmp.mpd";

			using (ZipArchive zipFile = ZipFile.OpenRead(file.FullName))
			{
				// Extract the mpl
				ZipArchiveEntry mpl = zipFile.Entries.First(x => x.FullName.EndsWith(".mpl"));
				Console.WriteLine("Extracting MPL to " + mplPath);
				mpl.ExtractToFile(mplPath);

				// Extract the mpd
				ZipArchiveEntry mpd = zipFile.Entries.First(x => x.FullName.EndsWith(".mpd"));
				Console.WriteLine("Extracting MPD to " + mpdPath);
				mpd.ExtractToFile(mpdPath);
			}

			ModPackJson modPack = JsonConvert.DeserializeObject<ModPackJson>(File.ReadAllText(mplPath));
			Console.WriteLine("Read MPL: " + modPack.Name);

			Console.WriteLine("Extracting Modded resources");
			FileStream fs = new FileStream(mpdPath, FileMode.Open);
			using (SqPackStream pack = new SqPackStream(fs))
			{
				if (modPack.SimpleModsList != null)
				{
					foreach (ModsJson mods in modPack.SimpleModsList)
					{
						this.Extract(mods, pack, outputDirectory);
					}
				}

				if (modPack.ModPackPages != null)
				{
					foreach (ModPackPageJson page in modPack.ModPackPages)
					{
						foreach (ModGroupJson group in page.ModGroups)
						{
							foreach (ModOptionJson option in group.OptionList)
							{
								foreach (ModsJson mods in option.ModsJsons)
								{
									string directoryName = page.PageIndex.ToString() + "_" + group.GroupName + "_" + option.Name;
									DirectoryInfo dir = outputDirectory.CreateSubdirectory(directoryName);
									this.Extract(mods, pack, dir);
								}
							}
						}
					}
				}
			}

			File.Delete(mpdPath);
			Console.WriteLine("Deleted MPD");
			File.Delete(mplPath);
			Console.WriteLine("Deleted MPL");
		}

		private void Extract(ModsJson mods, SqPackStream pack, DirectoryInfo outputDirectory)
		{
			Console.WriteLine(" > " + mods.FullPath);
			FileResource dat = pack.ReadFile<FileResource>(mods.ModOffset);

			FileInfo fileInfo = new FileInfo(outputDirectory.FullName + "/" + mods.FullPath);

			if (!fileInfo.Directory.Exists)
				fileInfo.Directory.Create();

			File.WriteAllBytes(fileInfo.FullName, dat.Data);
		}
	}
}
