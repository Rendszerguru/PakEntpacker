namespace PakReaderExe;
using System;

class PakReader : BinaryReader
{
	public PakReader( FileStream fs ): base(fs)
	{
        
	}
    
    public void SkipSignature()
    {
        BaseStream.Position += 4;
    }
    
    public void Skip( int count )
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
}
