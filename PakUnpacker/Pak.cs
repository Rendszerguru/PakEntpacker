namespace PakReaderExe;
using System;

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
    
    public void ReadForm( PakReader pr )
    {
		pr.SkipSignature();
		formSize = pr.ReadInt32BE();
		pr.SkipSignature();
    }
    
    public void ReadHead( PakReader pr)
    {
		pr.SkipSignature();
		Console.WriteLine("Head of the Pak File:" + Convert.ToBase64String(pr.ReadBytes(32)));
	}
    
    public void ReadData( PakReader pr)
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
            
            for ( long posEntries = pr.Pos(); pr.Pos() - posEntries < entriesSize; )
            {
				EntryType entryType = (EntryType)pr.ReadByte();
				int entryNameLength = pr.ReadByte();
				string entryName = new(pr.ReadChars(entryNameLength));

				if (entryType == EntryType.Directory) ReadEntriesFromDirectory(entryName, pr);
				else entries.Add(new(entryName, pr));
            }
		}
        catch { }
    }
    
    public void ReadEntriesFromDirectory(string dirName, PakReader pr)
    {
        int childCount = pr.ReadInt32();
        
        for ( int i = 0; i < childCount; i++ )
        {
            EntryType entryType = (EntryType)pr.ReadByte();
            int entryNameLength = pr.ReadByte();
            string entryName = dirName + "\\" + new string(pr.ReadChars(entryNameLength));

            if ( entryType == EntryType.File1 )
            {
                entries.Add(new(entryName, pr));
            }
            else if ( entryType == EntryType.Directory )
            {
                ReadEntriesFromDirectory(entryName, pr);
            }
            else
            {
                throw new Exception("Unbekannter Entry Typ");
            }
        }
    }

    public void ExtractDataBlock( string dst)
    {
        using PakReader pr = new(File.OpenRead(name));
        foreach (PakEntryFile entry in entries)
        {
            string? dir = Path.GetDirectoryName(dst + "\\" + entry.name);
            if (dir is not null) Directory.CreateDirectory(dir);

            if (entry.compression == PakEntryFile.CompressionType.Zlib)
            {
                try
                {
                    pr.BaseStream.Position = entry.offset;
                    Compression.Zlib.Decompress(pr, entry.originalSize, dst + "\\" + entry.name);
                }
                catch
                {
                    pr.BaseStream.Position = entry.offset;
                    File.WriteAllBytes(dst + "\\" + entry.name + "_DekomprimierungFehlgeschlagen", pr.ReadBytes(entry.size));

                }
            }
            else
            {
                pr.BaseStream.Position = entry.offset;
                File.WriteAllBytes(dst + "\\" + entry.name, pr.ReadBytes(entry.size));
            }
        }
    }
}
