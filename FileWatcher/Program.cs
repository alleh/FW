using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcher
{
    class Program
    {
        static void Main(string[] args)
        {

            var source = ConfigurationManager.AppSettings["Source"];
            var destinations = new List<string>();
            var index = 1;
            while (true)
            {
                var dest = ConfigurationManager.AppSettings["Destination" + index];
                if (string.IsNullOrEmpty(dest))
                {
                    break;
                }

                destinations.Add(dest);
                index++;
            }

            var watch = new FileWatcher(source, destinations);

            watch.Start();
        }
    }
}
