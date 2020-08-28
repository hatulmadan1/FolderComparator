using System;
using System.IO;

namespace FolderComparator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the full path of the first folder");
            string firstPath = UIAssist.GetCorrectPath();

            Console.WriteLine("Enter the full path of the second folder");
            string secondPath = UIAssist.GetCorrectPath();

            Comparator comparator = new Comparator();

            Console.WriteLine("Precalc started. It may take a while");

            comparator.FillDict(1, firstPath);
            comparator.FillDict(2, secondPath);

            Console.WriteLine("Precalc completed. Comparsion started.");

            var result = comparator.Compare();

            UIAssist.OutputResult(result);
        }
    }

    class UIAssist
    {
        public static string GetCorrectPath()
        {
            string path = "";
            while (true)
            {
                path = Console.ReadLine();
                if (!Directory.Exists(path))
                {
                    Console.WriteLine("No such directory. Enter the full path of the first folder");
                }
                else break;    
            }
            return path;
        }

        public static void OutputResult(ComparsionResult result)
        {
            if (result.SameFiles.Count != 0)
            {
                Console.WriteLine("These files are same\n");
            }

            foreach (var same in result.SameFiles)
            {
                if (same.Value.Count == 1)
                {
                    continue;
                }
                foreach (var file in same.Value)
                {
                    Console.WriteLine(file);
                }
                Console.WriteLine();
            }

            if (result.FirstFolderNoMatch.Count != 0)
            {
                Console.WriteLine("These files from first folder have no match in second folder\n");
            }

            foreach (var alone in result.FirstFolderNoMatch)
            {
                Console.WriteLine(alone);
            }

            Console.WriteLine();

            if (result.SecondFolderNoMatch.Count != 0)
            {
                Console.WriteLine("These files from second folder have no match in first folder\n");
            }

            foreach (var alone in result.SecondFolderNoMatch)
            {
                Console.WriteLine(alone);
            }

            Console.WriteLine();
        }
    }
}
