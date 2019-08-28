using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Snippets.ConcurrentDataStructures
{
    public class CvsRepositoryLazyWithDoubleCheck
    {
        public class VirtualCsv
        {
            public List<string[]> Value;
        }

        private readonly string _directory;
        private readonly Dictionary<string, VirtualCsv> _csvFiles;

        public CvsRepositoryLazyWithDoubleCheck(string directory)
        {
            _directory = directory;

            _csvFiles = new DirectoryInfo(directory)
                .GetFiles("*.csv")
                .ToDictionary(f => f.Name, f => new VirtualCsv());
        }

        public IEnumerable<string> Files => _csvFiles.Keys;

        public IEnumerable<T> Map<T>(string fileName, Func<string[], T> map) =>
            LazyLoadData(fileName).Skip(1).Select(map);

        private IEnumerable<string[]> LazyLoadData(string fileName)
        {
            var csvFile = _csvFiles[fileName].Value;

            if (csvFile == null)
            {
                lock (_csvFiles[fileName])
                {
                    csvFile = _csvFiles[fileName].Value;
                    if (csvFile == null)
                    {
                        csvFile = LoadData(Path.Combine(_directory, fileName)).ToList();
                        _csvFiles[fileName].Value = csvFile;
                    }
                }

            }

            return csvFile;
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
