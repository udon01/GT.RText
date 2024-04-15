using GT.Shared.Polyphony;
using System;
using System.IO;
using System.Text;

public class EndianBinReader : BinaryReader
{
    public EndianType Endianess { get; set; }

    public EndianBinReader(Stream stream, EndianType endian = EndianType.BIG_ENDIAN) : base(stream)
    {
        Endianess = endian;
    }

    public double ReadDouble(EndianType endianess)
    {
        var memorize = Endianess;
        Endianess = endianess;
        var value = ReadDouble();
        Endianess = memorize;
        return value;
    }

    public override double ReadDouble()
    {
        byte[] array = base.ReadBytes(8);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(array);
        return BitConverter.ToDouble(array, 0);
    }

    public short ReadInt16(EndianType endianess)
    {
        var memorize = Endianess;
        Endianess = endianess;
        var value = ReadInt16();
        Endianess = memorize;
        return value;
    }

    public override short ReadInt16()
    {
        byte[] array = base.ReadBytes(2);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(array);
        return BitConverter.ToInt16(array, 0);
    }

    public int ReadInt32(EndianType endianess)
    {
        var memorize = Endianess;
        Endianess = endianess;
        var value = ReadInt32();
        Endianess = memorize;
        return value;
    }

    public override int ReadInt32()
    {
        byte[] array = base.ReadBytes(4);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(array);
        return BitConverter.ToInt32(array, 0);
    }

    public long ReadInt64(EndianType endianess)
    {
        var memorize = Endianess;
        Endianess = endianess;
        var value = ReadInt64();
        Endianess = memorize;
        return value;
    }

    public override long ReadInt64()
    {
        byte[] array = base.ReadBytes(8);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(array);
        return BitConverter.ToInt64(array, 0);
    }

    public float ReadSingle(EndianType endianess)
    {
        var memorize = Endianess;
        Endianess = endianess;
        var value = ReadSingle();
        Endianess = memorize;
        return value;
    }

    public override float ReadSingle()
    {
        byte[] array = base.ReadBytes(4);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(array);
        return BitConverter.ToSingle(array, 0);
    }

    public ushort ReadUInt16(EndianType endianess)
    {
        var memorize = Endianess;
        Endianess = endianess;
        var value = ReadUInt16();
        Endianess = memorize;
        return value;
    }

    public override ushort ReadUInt16()
    {
        byte[] array = base.ReadBytes(2);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(array);
        return BitConverter.ToUInt16(array, 0);
    }

    public uint ReadUInt32(EndianType endianess)
    {
        var memorize = Endianess;
        Endianess = endianess;
        var value = ReadUInt32();
        Endianess = memorize;
        return value;
    }

    public override uint ReadUInt32()
    {
        byte[] array = base.ReadBytes(4);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(array);
        return BitConverter.ToUInt32(array, 0);
    }

    public ulong ReadUInt64(EndianType endianess)
    {
        var memorize = Endianess;
        Endianess = endianess;
        var value = ReadUInt64();
        Endianess = memorize;
        return value;
    }

    public override ulong ReadUInt64()
    {
        byte[] array = base.ReadBytes(8);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(array);
        return BitConverter.ToUInt64(array, 0);
    }

    public string ReadString(ushort length)
    {
        byte[] buffer = base.ReadBytes(length);
        return Encoding.UTF8.GetString(buffer);
    }

    public string ReadNullTerminatedString()
    {
        var sb = new StringBuilder();
        char @char;
        while ((@char = base.ReadChar()) != '\0')
        {
            sb.Append(@char);
        }

        return sb.ToString();
    }
}

