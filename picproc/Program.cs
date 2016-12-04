using System;
using System.Collections.Generic;
using System.IO;

namespace picproc
{
    class Program
    {
        static List<string> GetImagePaths(string srcDir)
        {
            var fileExtensions = new List<string>()
            {
                "*.jpg",
                "*.jpeg",
                "*.png"
            };

            var imgPaths = new List<string>();

            fileExtensions.ForEach(fileExtension =>
            {
                imgPaths.AddRange(Directory.GetFiles(srcDir, fileExtension));
            });

            return imgPaths;
        }

        static void Main(string[] args)
        {
            var cla = new CommandLineArgs(args);

            if (!cla.ParamsAreValid())
            {
                cla.Usage();
               return;
            }
            
            var imgPaths = GetImagePaths(cla.Src);
            new PicProc(cla.Dst).Process(imgPaths);

            Console.WriteLine("Finished");
        }
    }
}
