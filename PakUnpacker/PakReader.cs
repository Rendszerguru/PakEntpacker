namespace PakReaderExe;
using System;
using System.IO;
using System.Text;

class PakReader : BinaryReader
{
	public PakReader(FileStream fs) : base(fs) { }

	public void SkipSignature()
	{
		BaseStream.Position += 4;
	}
	public bool CanRead(int length)
	{
		return BaseStream.Position + length <= BaseStream.Length;
	}
	public void Skip(int count)
	{
		BaseStream.Position += count;
	}

	public long Pos() => BaseStream.Position;

	public int ReadInt32BE()
	{
		var data = ReadBytes(4);
		Array.Reverse(data);
		return BitConverter.ToInt32(data, 0);
	}

	public string ReadStringUtf8(int length)
	{
		var bytes = ReadBytes(length);
		try
		{
			return Encoding.UTF8.GetString(bytes);
		}
		catch (DecoderFallbackException)
		{
			Console.WriteLine("Warning: Found invalid UTF-8 string, using replacement characters.");
			return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error while reading the string: {ex.Message}");
			return "InvalidEntry";
		}
	}
}
