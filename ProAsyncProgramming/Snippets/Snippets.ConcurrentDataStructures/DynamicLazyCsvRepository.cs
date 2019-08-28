using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snippets.ConcurrentDataStructures
{
    public class DynamicLazyCvsRepository
    {
        private readonly string _directory;
        private readonly Dictionary<string, List<string[]>> _csvFiles;

        public DynamicLazyCvsRepository(string directory)
        {
            _directory = directory;
            _csvFiles = new Dictionary<string, List<string[]>>();
        }

        public IEnumerable<string> Files =>
            new DirectoryInfo(_directory).GetFiles().Select(f => f.FullName);

        public IEnumerable<T> Map<T>(string fileName, Func<string[], T> map)
        {
            if (!_csvFiles.TryGetValue(fileName, out var csvFile))
            {
                csvFile = LoadData(fileName).ToList();
                _csvFiles.Add(fileName, csvFile);
            }

            return csvFile.Skip(1).Select(map);
        }

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
