using System;
using System.Diagnostics;
using System.IO;

namespace CIApp
{
    internal class Program
    {
        private static string[] allPath;
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

                watcher.NotifyFilter = NotifyFilters.Attributes
                                     | NotifyFilters.CreationTime
                                     | NotifyFilters.DirectoryName
                                     | NotifyFilters.FileName
                                     | NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.Security
                                     | NotifyFilters.Size;

                watcher.Changed += OnChanged;

                watcher.Filter = "*.cs";
                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;

                Console.WriteLine("Press enter to exit.");
                Console.ReadKey(); //continue until user enters a key
            }
        }

        private static bool RunProcess(string fileName, string arguments)
        {
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

