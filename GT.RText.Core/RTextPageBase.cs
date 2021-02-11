using System.Collections.Generic;

using GT.Shared;

namespace GT.RText.Core
{
    public abstract class RTextPageBase
    {
        public string Name { get; set; }

        // We need to have them ordered, game uses binary searching
        public SortedDictionary<string, RTextPairUnit> PairUnits { get; set; }
            = new SortedDictionary<string, RTextPairUnit>(AlphaNumStringComparer.Default);

        public void EditRow(int id, string label, string data)
        {
            if (!PairUnits.ContainsKey(label))
                return;

            PairUnits[label] = new RTextPairUnit(id, label, data);
        }

        public int AddRow(int id, string label, string data)
        {
            var index = PairUnits.Count;
            PairUnits.Add(label, new RTextPairUnit(id, label, data));
            return index;
        }

        public void DeleteRow(string label)
            => PairUnits.Remove(label);

        public abstract void Read(EndianBinReader reader);
        public abstract void Write(EndianBinWriter writer, int baseOffset, int baseDataOffset);
    }
}
