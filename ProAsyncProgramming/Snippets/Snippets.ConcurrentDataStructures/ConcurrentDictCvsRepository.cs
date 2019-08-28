using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snippets.ConcurrentDataStructures
{
    public class ConcurrentDictCvsRepository
    {
        private readonly string _directory;
        private readonly ConcurrentDictionary<string, Lazy<List<string[]>>> _csvFiles;

        public ConcurrentDictCvsRepository(string directory)
        {
            _directory = directory;
            _csvFiles = new ConcurrentDictionary<string, Lazy<List<string[]>>>();
        }

        public IEnumerable<string> Files =>
            new DirectoryInfo(_directory).GetFiles().Select(f => f.FullName);

        public IEnumerable<T> Map<T>(string fileName, Func<string[], T> map)
        {
            var csvFile = new Lazy<List<string[]>>(() => LoadData(fileName).ToList());
            csvFile = _csvFiles.GetOrAdd(fileName, csvFile);
            return csvFile.Value.Skip(1).Select(map);
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