using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GT.Shared;
using GT.Shared.Logging;
using GT.Shared.Polyphony;

namespace GT.RText.Core
{
    public class RTextParser
    {
        private readonly ILogWriter _logWriter;
        private readonly byte[] _data;

        public IRText RText { get; set; }

        public string LocaleCode { get; set; }

        public static readonly Dictionary<string, string> Locales = new Dictionary<string, string>()
        {
            { "BP", "Portuguese (Brazillian)" },
            { "CN", "Chinese (China)" },
            { "CZ", "Czech" },
            { "DK", "Danish" }, // Stubbed in GT6
            { "DE", "German" },
            { "EL", "Greek" },
            { "ES", "Spanish" },
            { "FI", "Finnish" }, // Stubbed in GT6
            { "FR", "French" },
            { "GB", "British" },
            { "HU", "Magyar (Hungary)" },
            { "IT", "Italian" },
            { "JP", "Japanese" },
            { "KR", "Korean" },
            { "MS", "Spanish (Mexican)" },
            { "NO", "NO" }, // TODO
            { "NL", "Dutch" },
            { "PL", "Polish" },
            { "PT", "Portuguese" },
            { "RU", "Russian" },
            { "SE", "SE" }, // TODO
            { "TR", "Turkish" },
            { "TW", "Chinese (Taiwan)" },
            { "US", "American" }
        };

        public RTextParser(string filePath, ILogWriter logWriter = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException();

            _data = File.ReadAllBytes(filePath);
            _logWriter = logWriter;

            Read();
        }

        public RTextParser(byte[] data, ILogWriter logWriter = null)
        {
            _data = data;
            _logWriter = logWriter;

            Read();
        }

        private void Read()
        {
            using (var ms = new MemoryStream(_data))
            using (var reader = new EndianBinReader(ms))
            {
                switch (reader.ReadUInt32())
                {
                    case Constants.RT03_MAGIC:
                        RText = new RT03(_data, _logWriter);
                        break;
                    case Constants.RT04_MAGIC:
                        RText = new RT04(_data, _logWriter);
                        break;
                    case Constants.RT05_MAGIC:
                        RText = new RT05(_data, _logWriter);
                        break;
                    case Constants._50TR_MAGIC:
                        RText = new _50TR(_data, _logWriter);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Unknown header magic.");
                }
            }
        }
    }
}
