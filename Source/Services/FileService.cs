using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Services
{
    public class FileService: IFileService
    {
        public void Save(string path, Stream stream)
        {
            var bytes = new Byte[stream.Length];
            stream.Read(bytes, 0, (int)stream.Length);
            File.WriteAllBytes(path, bytes);
        }

        public void Save(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        public void Save(string path, byte[] content)
        {
            File.WriteAllBytes(path, content);
        }

        public void Delete(string path)
        {
            File.Delete(path);  
        }

        public IList<string> ReadAllLines(string path)
        {
            return File.ReadAllLines(path).ToList();
        }

        public void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
