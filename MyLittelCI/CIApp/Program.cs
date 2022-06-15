using System;
using System.Diagnostics;
using System.IO;

namespace CIApp
{
    internal class Program
    {

        private static string[] allPath;

        //private static string ChangeTextFile = @"C:\Users\user\Desktop\Change.txt";

        static void Main(string[] args)
        {
            allPath = args;
            Watcher();
        }

       

        private static void Watcher()
        {
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {

                watcher.Path = allPath[0];
                watcher.NotifyFilter = NotifyFilters.LastWrite
                    | NotifyFilters.LastAccess
                    | NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.Size;

                watcher.Filter = "*.*";// can use *.txt for only text files

                watcher.Changed += OnChanged;
                watcher.EnableRaisingEvents = true;

                Console.ReadKey(); //continue until user enters a key
            }
        }

        private static bool RunProcess(string fileName, string arguments)
        {
            Process process = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = fileName;
            startInfo.Arguments = arguments;

            using (Process p = Process.Start(startInfo))
            {
                p.WaitForExit();
                return (p.ExitCode == 0);
            }
        }

        private static void RunBulid()
        {
            string exeMSBulidPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe";

            if (RunProcess(exeMSBulidPath, allPath[1]))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Start running tests");
                Console.ResetColor();

                RunTest();
            }
        }

        private static void RunTest()
        {
            string testDllPath = allPath[2];
           
            string exeFilePath = allPath[3];

            RunProcess(exeFilePath, testDllPath);
        }

        static DateTime lastRead = DateTime.MinValue;
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            DateTime lastWrite = File.GetLastWriteTime(allPath[0]);
            if (lastWrite != lastRead)
            {
                RunBulid();
                lastRead = lastWrite;
            }
        }
    }
}

