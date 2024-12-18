namespace PakReaderExe.Compression
{
	using System.IO.Compression;

	static class Zlib
	{
		public static byte[] Decompress(byte[] data)
		{
			if (data == null || data.Length < 2)
			{
				throw new InvalidDataException("The input data is too short for decompression.");
			}

			if (data[0] != 0x78)
			{
				throw new InvalidDataException("Invalid Zlib header.");
			}

			byte[] buffer = new byte[data.Length - 2];
			Buffer.BlockCopy(data, 2, buffer, 0, buffer.Length);

			try
			{
				using MemoryStream decompressedStream = new();
				using MemoryStream compressStream = new(buffer);
				using DeflateStream deflateStream = new(compressStream, CompressionMode.Decompress);
				deflateStream.CopyTo(decompressedStream);
				return decompressedStream.ToArray();
			}
			catch (InvalidDataException ex)
			{
				Console.WriteLine($"Invalid data during decompression: {ex.Message}");
				throw;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error during decompression: {ex.Message}");
				throw;
			}
		}
		public static void Decompress(PakReader pr, int expectedSize, string dst)
		{
			if (!pr.CanRead(2))
			{
				Console.WriteLine("Error: Insufficient data to skip Zlib header.");
				return;
			}

			long originalPosition = pr.Pos();
			pr.Skip(2);

			try
			{
				using (FileStream decompressedStream = File.Create(dst))
				using (DeflateStream deflateStream = new(pr.BaseStream, CompressionMode.Decompress, leaveOpen: true))
				{
					byte[] buffer = new byte[1000];
					int bytesReadTotal = 0;
					int bytesRead;

					while (bytesReadTotal < expectedSize &&
						   (bytesRead = deflateStream.Read(buffer, 0, Math.Min(buffer.Length, expectedSize - bytesReadTotal))) > 0)
					{
						decompressedStream.Write(buffer, 0, bytesRead);
						bytesReadTotal += bytesRead;
					}

					if (bytesReadTotal < expectedSize)
					{
						Console.WriteLine($"Warning: Decompressed size ({bytesReadTotal} bytes) is smaller than expected ({expectedSize} bytes).");
					}
				}

				Console.WriteLine($"The file was successfully decompressed: {dst}");
			}
			catch (InvalidDataException ex)
			{
				Console.WriteLine($"Invalid data during decompression: {ex.Message}");
				pr.BaseStream.Position = originalPosition;
				SaveCorruptedData(pr, expectedSize, dst);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error during Zlib decompression: {ex.Message}");
				pr.BaseStream.Position = originalPosition;
				SaveCorruptedData(pr, expectedSize, dst);
			}
		}

		private static void SaveCorruptedData(PakReader pr, int expectedSize, string dst)
		{
			try
			{
				byte[] corruptedData = pr.ReadBytes(expectedSize);
				File.WriteAllBytes(dst + "_Error", corruptedData);
				Console.WriteLine($"Corrupted data saved: {dst}_Error");
			}
			catch (Exception innerEx)
			{
				Console.WriteLine($"Error while saving corrupted data: {innerEx.Message}");
			}
		}
	}
}
