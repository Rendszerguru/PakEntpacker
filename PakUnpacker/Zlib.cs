namespace PakReaderExe.Compression;
using System.IO.Compression;

static class Zlib
{
    public static byte[] Decompress(byte[] data)
    {
        byte[] buffer = new byte[data.Length - 2];
        Buffer.BlockCopy(data, 2, buffer, 0, buffer.Length);

        using MemoryStream decompressedStream = new();
        using MemoryStream compressStream = new(buffer);
        using DeflateStream deflateStream = new(compressStream, CompressionMode.Decompress);
        deflateStream.CopyTo(decompressedStream);
        return decompressedStream.ToArray();
    }

    public static void Decompress(PakReader pr, int expectedSize, string dst)
    {
        pr.Skip(2);

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
    }
}
