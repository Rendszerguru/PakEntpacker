namespace PakReaderExe;
using System;
using System.IO;
using System.Text;

class PakReader : BinaryReader
{
	public PakReader(FileStream fs) : base(fs) { }

	public void SkipSignature()
	{
		if (CanRead(4))
		{
			BaseStream.Position += 4;
		}
		else
		{
			Console.WriteLine("Warning: Not enough data to skip the signature.");
			BaseStream.Position = BaseStream.Length;
		}
	}
	public bool CanRead(int length)
	{
		return length >= 0 && BaseStream.Position + length <= BaseStream.Length;
	}
	public void Skip(int count)
	{
		if (CanRead(count))
		{
			BaseStream.Position += count;
		}
		else
		{
			Console.WriteLine($"Warning: Attempt to skip {count} bytes, but not enough data remains. Skipping to the end of the stream.");
			BaseStream.Position = BaseStream.Length;
		}
	}
	public long Pos() => BaseStream.Position;

	public int ReadInt32BE()
	{
		if (CanRead(4))
		{
			var data = ReadBytes(4);
			Array.Reverse(data);
			return BitConverter.ToInt32(data, 0);
		}
		else
		{
			Console.WriteLine("Warning: Not enough data to read 4 bytes for an Int32.");
			return 0;
		}
	}
	public string ReadStringUtf8(int length)
	{
		if (length <= 0)
		{
			Console.WriteLine("Warning: Attempted to read a string with non-positive length.");
			return string.Empty;
		}

		if (!CanRead(length))
		{
			Console.WriteLine($"Warning: Not enough data to read a string of length {length}.");
			return string.Empty;
		}

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
