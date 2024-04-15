using System;
using System.IO;
using System.Linq;
using System.Text;
using GT.Shared.Polyphony;

public class EndianBinWriter : BinaryWriter
{
    public EndianType Endianess { get; set; }

    public EndianBinWriter(Stream stream, EndianType endian = EndianType.BIG_ENDIAN) : base(stream)
    {
        Endianess = endian;
    }

    public override void Write(double data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(bytes);
        base.Write(bytes);
    }

    public override void Write(short data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(bytes);
        base.Write(bytes);
    }

    public override void Write(int data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(bytes);
        base.Write(bytes);
    }

    public override void Write(long data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(bytes);
        base.Write(bytes);
    }

    public override void Write(float data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(bytes);
        base.Write(bytes);
    }

    public override void Write(ushort data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(bytes);
        base.Write(bytes);
    }

    public override void Write(uint data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(bytes);
        base.Write(bytes);
    }

    public override void Write(ulong data)
    {
        byte[] bytes = BitConverter.GetBytes(data);
        if (Endianess == EndianType.BIG_ENDIAN)
            Array.Reverse(bytes);
        base.Write(bytes);
    }

    public void WriteAligned(byte[] data, int align = 0, bool nullTerminate = false)
    {
        base.Write(data);

        if (nullTerminate)
            base.Write((byte)0x00);

        if (align <= 0 || (data.Length + (nullTerminate ? 1 : 0)) % align == 0)
            return;

        int writtenBytes = 0;
        while (((data.Length + (nullTerminate ? 1 : 0)) + writtenBytes) % align != 0)
        {
            base.Write((byte)0x00);
            writtenBytes++;
        }
    }

    public void WriteNullTerminatedString(string data, int align = 0)
    {
        data += '\0';
        var bytes = Encoding.UTF8.GetBytes(data);
        if (align > 0 && bytes.Length % align != 0)
        {
            var byteList = bytes.ToList();
            while (byteList.Count % align != 0)
            {
                byteList.Add(0x00);
            }

            bytes = byteList.ToArray();
        }

        base.Write(bytes);
    }
}

