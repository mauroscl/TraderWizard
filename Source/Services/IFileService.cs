using System.Collections.Generic;
using System.IO;

namespace Services
{
    public interface IFileService
    {
        void Save(string path, Stream stream);
        void Delete(string path);
        IList<string> ReadAllLines(string path );
        void Save(string path, string content);
        void CreateFolder(string path);
        bool FileExists(string path);
    }
}