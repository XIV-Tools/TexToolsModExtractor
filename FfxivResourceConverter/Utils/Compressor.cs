// © XIV-Tools.
// Licensed under the MIT license.

namespace FfxivResourceConverter
{
	using System.IO;
	using System.IO.Compression;

	public static class Compressor
	{
		/// <summary>
		/// Compresses raw byte data.
		/// </summary>
		/// <param name="uncompressedBytes">The data to be compressed.</param>
		/// <returns>The compressed byte data.</returns>
		public static byte[] Compress(byte[] uncompressedBytes)
		{
			using MemoryStream uMemoryStream = new MemoryStream(uncompressedBytes);
			using MemoryStream cMemoryStream = new MemoryStream();
			using DeflateStream deflateStream = new DeflateStream(cMemoryStream, CompressionMode.Compress);

			uMemoryStream.CopyTo(deflateStream);
			deflateStream.Close();

			return cMemoryStream.ToArray();
		}

		/// <summary>
		/// Decompresses raw byte data.
		/// </summary>
		/// <param name="compressedBytes">The byte data to decompress.</param>
		/// <param name="uncompressedSize">The final size of the compressed data after decompression.</param>
		/// <returns>The decompressed byte data.</returns>
		public static byte[] Decompress(byte[] compressedBytes, int uncompressedSize)
		{
			byte[] decompressedBytes = new byte[uncompressedSize];

			using MemoryStream ms = new MemoryStream(compressedBytes);
			using DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress, true);

			int offset = 0; // offset for writing into buffer
			int bytesRead; // number of bytes read from Read operation
			while ((bytesRead = ds.Read(decompressedBytes, offset, uncompressedSize - offset)) > 0)
			{
				offset += bytesRead;  // offset in buffer for results of next reading
				if (bytesRead == uncompressedSize)
				{
					break;
				}
			}

			return decompressedBytes;
		}
	}
}
