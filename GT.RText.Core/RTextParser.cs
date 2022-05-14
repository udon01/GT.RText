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

        public RTextBase RText { get; set; }

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
            { "NO", "Norwegian" },
            { "NL", "Dutch" },
            { "PL", "Polish" },
            { "PT", "Portuguese" },
            { "RU", "Russian" },
            { "SE", "Swedish" },
            { "TR", "Turkish" },
            { "TW", "Chinese (Taiwan)" },
            { "US", "American" }
        };

        public RTextParser(ILogWriter logWriter = null)
        {
            _logWriter = logWriter;
        }

        public void Read(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var reader = new EndianBinReader(ms))
            {
                switch (reader.ReadUInt32())
                {
                    case Constants.RT03_MAGIC:
                        RText = new RT03(_logWriter);
                        break;
                    case Constants.RT04_MAGIC:
                        RText = new RT04(_logWriter);
                        break;
                    case Constants.RT05_MAGIC:
                         RText = new RT05(_logWriter);
                         break;
                    case Constants._50TR_MAGIC:
                        try
                        {
                            RText = new _50TR(_logWriter);
                            RText.Read(data);
                            return;
                        }
                        catch { }

                        // Failed, try GT7
                        RText = new _50TR(_logWriter, gt7: true);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Unknown header magic.");
                }

                RText.Read(data);
            }
        }
    }
}
