using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snippets.ConcurrentDataStructures
{
    public class CvsRepository
    {
        private readonly string _directory;
        private readonly Dictionary<string, List<string[]>> _csvFiles;

        public CvsRepository(string directory)
        {
            _directory = directory;

            _csvFiles = new DirectoryInfo(directory)
                .GetFiles("*.csv")
                .ToDictionary(f => f.Name, f => LoadData(f.FullName).ToList());
        }

        public IEnumerable<string> Files => _csvFiles.Keys;

        public IEnumerable<T> Map<T>(string fileName, Func<string[], T> map) =>
            _csvFiles[fileName].Skip(1).Select(map);

        private IEnumerable<string[]> LoadData(string fileName)
        {
            using (var reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                    yield return reader.ReadLine().Split(',');
            }
        }
    }
}
