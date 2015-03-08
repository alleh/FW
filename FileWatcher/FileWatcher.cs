using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher
{
    class FileWatcher
    {
        private string _folderToWatch = "C:/Users/alc/Desktop/folder1";
        private string _copyDestination = "C:/Users/alc/Desktop/folder2";

        public void Start()
        {
            var watcher = new FileSystemWatcher(_folderToWatch);
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.Error += new ErrorEventHandler(OnError);
            watcher.EnableRaisingEvents = true;

            Console.WriteLine("Press \'Enter\' to quit.");
            Console.ReadLine();
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var sourceFolder = new DirectoryInfo(_folderToWatch);
            var targetFolder = new DirectoryInfo(_copyDestination);

            CopyAll(sourceFolder, targetFolder);
            Console.WriteLine("File {0} {1}", e.FullPath, e.ChangeType.ToString());
            
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            var sourceFolder = new DirectoryInfo(_folderToWatch);
            var targetFolder = new DirectoryInfo(_copyDestination);
            CopyAll(sourceFolder, targetFolder);
            Console.WriteLine("File {0} {2} to {1}", e.OldFullPath, e.FullPath, e.ChangeType.ToString());
        }

        private void OnError(object source, ErrorEventArgs e)
        {
            Console.WriteLine("The FileSystemWatcher has detected an error");
            if (e.GetException().GetType() == typeof(InternalBufferOverflowException))
            {
                Console.WriteLine(("The file system watcher experienced an internal buffer overflow: " + e.GetException().Message));
            }
        }

        public void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {

            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            foreach (var file in target.GetFiles())
            {
                File.Delete(file.FullName);
            }

            foreach (var file in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, file.Name);
                file.CopyTo(Path.Combine(target.ToString(), file.Name), true);
            }

            foreach (var SourceSubDir in source.GetDirectories())
            {
                var targetSubDir = target.CreateSubdirectory(SourceSubDir.Name);
                CopyAll(SourceSubDir, targetSubDir);
            }
        }

        static void Main(string[] args)
        {
            var watch = new FileWatcher();

            watch.Start();
        }
    }
}
