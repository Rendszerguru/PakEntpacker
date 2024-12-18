namespace PakReaderExe.Compression
{
	using System.IO.Compression;

	static class Zlib
	{
		public static byte[] Decompress(byte[] data)
		{
			// Check if the input data is valid
			if (data.Length < 2)
			{
				throw new InvalidDataException("The input data is too short for decompression.");
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
			catch (Exception ex)
			{
				Console.WriteLine($"Error during decompression: {ex.Message}");
				throw;
			}
		}

		public static void Decompress(PakReader pr, int expectedSize, string dst)
		{
			// Check if enough data is available for decompression
			if (!pr.CanRead(2))
			{
				Console.WriteLine("Error: The input data is insufficient for decompression.");
				return;
			}

			pr.Skip(2); // Skip the Zlib header

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
				}
				// Console.WriteLine($"The file was successfully decompressed: {dst}");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error during Zlib decompression: {ex.Message}");

				// Save corrupted data with an error file
				try
				{
					pr.BaseStream.Position = pr.Pos(); // Reset position to before the error
					File.WriteAllBytes(dst + "_Error", pr.ReadBytes(expectedSize));
					Console.WriteLine($"Corrupted data saved: {dst}_Error");
				}
				catch (Exception innerEx)
				{
					Console.WriteLine($"Error during corrupted data saving: {innerEx.Message}");
				}
			}
		}
	}
}
