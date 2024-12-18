namespace PakReaderExe;
using System;
using System.Collections.Generic;
using System.IO;

class Pak
{
	public enum EntryType
	{
		Directory, File1
	}

	public string name;
	public int formSize;
	public int dataSize;
	public int entriesSize;
	public List<PakEntryFile> entries = new();

	public Pak(string src)
	{
		name = src;
		PakReader pr = new(File.OpenRead(src));

		ReadForm(pr);
		ReadHead(pr);
		ReadData(pr);
		ReadEntries(pr);
	}

	public void ReadForm(PakReader pr)
	{
		pr.SkipSignature();
		formSize = pr.ReadInt32BE();
		pr.SkipSignature();
	}

	public void ReadHead(PakReader pr)
	{
		pr.SkipSignature();
		Console.WriteLine("Head of the Pak File:" + Convert.ToBase64String(pr.ReadBytes(32)));
	}

	public void ReadData(PakReader pr)
	{
		pr.SkipSignature();
		dataSize = pr.ReadInt32BE();
		pr.Skip(dataSize);
	}

	public void ReadEntries(PakReader pr)
	{
		pr.SkipSignature();
		entriesSize = pr.ReadInt32BE();

		try
		{
			pr.Skip(2);
			pr.Skip(4);

			long posEntries = pr.Pos();
			while (pr.Pos() - posEntries < entriesSize)
			{
				long previousPos = pr.Pos();

				try
				{
					if (!pr.CanRead(2))
						break;

					EntryType entryType = (EntryType)pr.ReadByte();
					int entryNameLength = pr.ReadByte();

					if (!pr.CanRead(entryNameLength))
						break;

					string entryName = pr.ReadStringUtf8(entryNameLength);

					if (entryType == EntryType.Directory)
					{
						ReadEntriesFromDirectory(entryName, pr);
					}
					else
					{
						entries.Add(new(entryName, pr));
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error while reading an entry: {ex.Message}");
					pr.BaseStream.Position = previousPos;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error while processing the entries: {ex.Message}");
		}
	}


	public void ReadEntriesFromDirectory(string dirName, PakReader pr)
	{
		if (!pr.CanRead(4))
		{
			Console.WriteLine("Warning: Can't read directory entry as there is not enough data.");
			return;
		}

		int childCount = pr.ReadInt32();

		for (int i = 0; i < childCount; i++)
		{
			long previousPos = pr.Pos();

			try
			{
				if (!pr.CanRead(2))
					break;

				EntryType entryType = (EntryType)pr.ReadByte();
				int entryNameLength = pr.ReadByte();

				if (!pr.CanRead(entryNameLength))
					break;

				string entryName = dirName + "\\" + pr.ReadStringUtf8(entryNameLength);

				if (entryType == EntryType.File1)
				{
					entries.Add(new(entryName, pr));
				}
				else if (entryType == EntryType.Directory)
				{
					ReadEntriesFromDirectory(entryName, pr);
				}
				else
				{
					Console.WriteLine("Unknown entry type.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error while reading a directory entry: {ex.Message}");
				pr.BaseStream.Position = previousPos;
				break;
			}
		}
	}

	public void ExtractDataBlock(string dst)
	{
		using PakReader pr = new(File.OpenRead(name));

		foreach (PakEntryFile entry in entries)
		{
			try
			{
				string? dir = Path.GetDirectoryName(dst + "\\" + entry.name);
				if (dir is not null) Directory.CreateDirectory(dir);

				if (entry.offset + entry.size > pr.BaseStream.Length)
				{
					Console.WriteLine($"Warning: Entry '{entry.name}' exceeds file size. Skipping.");
					continue;
				}

				pr.BaseStream.Position = entry.offset;

				if (entry.compression == PakEntryFile.CompressionType.Zlib)
				{
					try
					{
						Compression.Zlib.Decompress(pr, entry.originalSize, dst + "\\" + entry.name);
					}
					catch
					{
						Console.WriteLine($"Error during Zlib decompression: {entry.name}");
						File.WriteAllBytes(dst + "\\" + entry.name + "_Error", pr.ReadBytes(entry.size));
					}
				}
				else
				{
					File.WriteAllBytes(dst + "\\" + entry.name, pr.ReadBytes(entry.size));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error while extracting a file ({entry.name}): {ex.Message}");
			}
		}
	}

}
