using System.Collections.Generic;

namespace GT.RText.Core
{
    public interface IRTextCategory
    {
        string Name { get; set; }
        List<(int Index, int Id, string Label, string Data)> Entries { get; set; }

        void EditRow(int index, int id, string label, string data);
        int AddRow(int id, string label, string data);
        void DeleteRow(int index);

        void Save(int baseOffset, EndianBinWriter writer);
    }
}
