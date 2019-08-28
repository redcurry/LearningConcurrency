using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snippets.ConcurrentDataStructures
{
    public class ParallelFileFinderWithBag
    {
        public static List<FileInfo> FindAllFiles(string path, string match)
        {
            var fileTasks = new List<Task<List<FileInfo>>>();
            var directories = new ConcurrentBag<DirectoryInfo>();

            // Run as many Tasks as there are top-level directories
            foreach (var dir in new DirectoryInfo(path).GetDirectories())
                fileTasks.Add(Task.Run(() => Find(dir, directories, match)));

            // Gather all results (alternative to using Task.WaitAll)
            return (from fileTask in fileTasks
                from file in fileTask.Result
                select file).ToList();
        }

        private static List<FileInfo> Find(DirectoryInfo dir, ConcurrentBag<DirectoryInfo> directories, string match)
        {
            var files = new List<FileInfo>();

            // Once added, another Task may take it
            directories.Add(dir);

            while (directories.TryTake(out DirectoryInfo dirToExamine))
            {
                // Any of the directories added may be taken by another Task
                foreach (var subDir in dirToExamine.GetDirectories())
                    directories.Add(subDir);

                // Since files is local, it's thread-safe to add to it
                files.AddRange(dirToExamine.GetFiles(match));
            }

            return files;
        }
    }
}
