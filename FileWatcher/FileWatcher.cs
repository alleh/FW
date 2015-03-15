using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher
{
    class FileWatcher
    {
        private readonly string _folderToWatch;
        //private string _copyDestination = "C:/Users/alc/Desktop/folder2";
        private readonly List<string> _copyDestinations; //= new List<string>();
        //private List<DirectoryInfo> targetFolder = new List<DirectoryInfo>();

        public FileWatcher(string source, List<string> destination)
        {
            _folderToWatch = source;
            _copyDestinations = destination;
        }

        public void Start()
        {
            //_copyDestinations.Add(@"C:\Repos\folder2");
            //_copyDestinations.Add(@"C:\Repos\folder3");
            //_copyDestinations.Add(@"C:\Repos\folder4");

            var watcher = new FileSystemWatcher(_folderToWatch);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Deleted += new FileSystemEventHandler(OnChanged);
            watcher.Renamed += new RenamedEventHandler(OnRenamed);
            watcher.Error += new ErrorEventHandler(OnError);
            watcher.EnableRaisingEvents = true;
            watcher.IncludeSubdirectories = true;

            Console.WriteLine("Press \'Enter\' to quit.");
            Console.ReadLine();
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {

            var now = DateTime.Now.Subtract(TimeSpan.FromMinutes(1));

            var sourceFolder = new DirectoryInfo(_folderToWatch);

            foreach (var folder in _copyDestinations)
            {
                var targetFolder = new DirectoryInfo(folder);
                CopyAll(sourceFolder, targetFolder, now);
            }

            Console.WriteLine("File {0} {1}", e.FullPath, e.ChangeType.ToString());
        }

        private void OnRenamed(object source, RenamedEventArgs e)
        {
            var now = DateTime.Now.Subtract(TimeSpan.FromMinutes(1));

            var sourceFolder = new DirectoryInfo(_folderToWatch);
            foreach (var folder in _copyDestinations)
            {
                var targetFolder = new DirectoryInfo(folder);
                CopyAll(sourceFolder, targetFolder, now);
            }
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

        public void CopyAll(DirectoryInfo source, DirectoryInfo target, DateTime since)
        {

            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            //foreach (var file in target.GetFiles("*", SearchOption.AllDirectories))
            //{
            //    File.Delete(file.FullName);
            //}

            foreach (var file in source.GetFiles("*", SearchOption.AllDirectories).Where(f => f.LastWriteTime > since))
            {

                var path = file.FullName.Replace(source.FullName, target.FullName);

                Console.WriteLine(@"Copying {0}\{1}", target.FullName, file.Name);
                try
                {
                    file.CopyTo(path, true);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("ERROR: " + exc.Message);
                }
            }



            //foreach (var SourceSubDir in source.GetDirectories())
            //{
            //    var targetSubDir = target.CreateSubdirectory(SourceSubDir.Name);
            //    CopyAll(SourceSubDir, targetSubDir);
            //}
        }
    }
}
