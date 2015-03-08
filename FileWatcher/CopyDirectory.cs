using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher
{
    static class CopyDirectory
    {
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
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
    }
}
