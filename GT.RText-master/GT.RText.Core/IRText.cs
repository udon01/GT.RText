using System.Collections.Generic;

namespace GT.RText.Core
{
    public interface IRText
    {
        void Save(string filePath);

        List<IRTextCategory> GetCategories();
    }
}
