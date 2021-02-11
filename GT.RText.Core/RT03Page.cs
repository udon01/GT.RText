using System.Collections.Generic;
using System.IO;
using GT.Shared.Logging;
using GT.Shared.Polyphony;

namespace GT.RText.Core
{
    public class RT03Page : RTextPageBase
    {
        private readonly ILogWriter _logWriter;
        public const int EntrySize = 0x08;

        public RT03Page(ILogWriter logWriter = null)
        {
            _logWriter = logWriter;
        }

        public override void Read(EndianBinReader reader)
        {
            var pageNameOffset = reader.ReadUInt32();
            var pairUnitCount = reader.ReadUInt32();
            var pairUnitOffset = reader.ReadUInt32();

            reader.BaseStream.Position = pageNameOffset;
            Name = reader.ReadNullTerminatedString();

            reader.BaseStream.Position += reader.BaseStream.Position % 0x10; // Padding with 0x5E

            for (int i = 0; i < pairUnitCount; i++)
            {
                reader.BaseStream.Position = pairUnitOffset + (i * 0x08);
                uint labelOffset = reader.ReadUInt32();
                uint valueOffset = reader.ReadUInt32();

                reader.BaseStream.Position = labelOffset;
                string label = reader.ReadNullTerminatedString();

                reader.BaseStream.Position = valueOffset;
                string value = reader.ReadNullTerminatedString();

                var pair = new RTextPairUnit(0, label, value);
                PairUnits.Add(label, pair);
            }
        }

        public override void Write(EndianBinWriter writer, int baseOffset, int baseDataOffset)
        {
            writer.BaseStream.Position = baseDataOffset;
            int baseNameOffset = (int)writer.BaseStream.Position;
            writer.WriteNullTerminatedString(Name, 0x04);
            int pairUnitOffset = (int)writer.BaseStream.Position;

            // Proceed to write the string tree, skip the entry map for now
            writer.BaseStream.Position += EntrySize * PairUnits.Count;
            int lastStringPos = (int)writer.BaseStream.Position;

            // Write our strings
            int j = 0;
            foreach (var pair in PairUnits)
            {
                writer.BaseStream.Position = lastStringPos;

                int labelOffset = (int)writer.BaseStream.Position;
                writer.WriteNullTerminatedString(pair.Value.Label, 4);

                int valueOffset = (int)writer.BaseStream.Position;
                writer.WriteNullTerminatedString(pair.Value.Value, 4);

                lastStringPos = (int)writer.BaseStream.Position;

                // Write the offsets
                writer.BaseStream.Position = pairUnitOffset + (j * RT03Page.EntrySize);
                writer.Write(labelOffset);
                writer.Write(valueOffset);

                j++;
            }

            // Finish page toc entry
            writer.BaseStream.Position = baseOffset;
            writer.Write(baseNameOffset);
            writer.Write(PairUnits.Count);
            writer.Write(pairUnitOffset);
            writer.Write(0x5E5E5E5E); // Padding

            // Seek to the end of it
            writer.BaseStream.Position = writer.BaseStream.Length;
        }
    }
}
