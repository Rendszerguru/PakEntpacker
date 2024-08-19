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

        using FileStream decompressedStream = File.Create(dst);
        DeflateStream deflateStream = new(pr.BaseStream, CompressionMode.Decompress);
        for (int bytesRead = 0; bytesRead < expectedSize;)
        {
            int toRead = 1000;
            if (bytesRead + toRead > expectedSize) toRead = expectedSize - bytesRead;
            byte[] buffer = new byte[toRead];
            deflateStream.Read(buffer, 0, toRead);
            decompressedStream.Write(buffer, 0, toRead);
            bytesRead += toRead;
        }
    }
}
