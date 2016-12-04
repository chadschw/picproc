using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace picproc
{
    public class CommandLineArgs
    {
        public string Src;
        public string Dst;

        public CommandLineArgs(string[] inputArgs)
        {
            args = inputArgs;
        }

        public void Usage()
        {
            Console.WriteLine("picproc.exe <path to source folder> <path to destination folder>");
            Console.ReadKey();
        }

        public bool ParamsAreValid()
        {
            return CorrectNumberOfArgs() &&
                   SrcDirExists() &&
                   DstDirExists() &&
                   (DstDirIsEmpty() || ConfirmContinue());
        }

        private bool CorrectNumberOfArgs()
        {
            return args.Length == 2;
        }

        private bool SrcDirExists()
        {
            Src = args[0];
            if (!Directory.Exists(Src))
            {
                Console.WriteLine("Error: Source directory '" + Src + "' does not exist");
                return false;
            }

            return true;
        }

        private bool DstDirExists()
        {
            Dst = args[1];

            if (!Directory.Exists(Dst))
            {
                try
                {
                    Directory.CreateDirectory(Dst);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: Failed to create destination directory '" + Dst + "'.");
                    Console.WriteLine(e);
                    return false;
                }
            }

            return true;
        }

        private bool DstDirIsEmpty()
        {
            var dirsInDstFolder = Directory.GetDirectories(Dst);
            var filesInDstFolder = Directory.GetFiles(Dst);

            if (dirsInDstFolder.Count() != 0 || filesInDstFolder.Count() != 0)
            {
                Console.WriteLine("Waringing: Destination folder '" + Dst + "' is not empty.");
                foreach (var dir in dirsInDstFolder)
                {
                    Console.WriteLine(dir);
                }
                foreach (var file in filesInDstFolder)
                {
                    Console.WriteLine(file);
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        private bool ConfirmContinue()
        {
            Console.Write("Continue? (Y/N): ");
            var input = Console.ReadLine();
            if (input != "y")
            {
                Console.WriteLine("Aborting.");
                return false;
            }
            else
            {
                Console.WriteLine("Continuing.");
                return true;
            }
        }

        private string[] args;
    }
}
