using System;
using System.Diagnostics;
using System.IO;

namespace CIApp
{
    internal class Program
    {
        // fileWatcher
        private static string projectPath = @"C:\Users\yosef\source\repos\My_Little_CI\DemoProject\";
        //msBuild
        private static string solutionPath = @"C:\Users\yosef\source\repos\My_Little_CI\DemoProject\DemoProject.sln";
        //private static string ChangeTextFile = @"C:\Users\user\Desktop\Change.txt";

        static void Main(string[] args)
        {
            Watcher();
        }

        private static void Watcher()
        {
            using (FileSystemWatcher watcher = new FileSystemWatcher())
            {

                watcher.Path = projectPath;
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

            if (RunProcess(exeMSBulidPath, solutionPath))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Start running tests");
                Console.ResetColor();

                RunTest();
            }
        }

        private static void RunTest()
        {
            string testDllPath = @"C:\Users\yosef\source\repos\My_Little_CI\DemoProject\TestProject\bin\Debug\net6.0\TestProject.dll";
           
            string exeFilePath = @"C:\Users\yosef\source\repos\My_Little_CI\MyLittelCI\packages\NUnit.ConsoleRunner.3.15.0\tools\nunit3-console.exe";

            RunProcess(exeFilePath, testDllPath);
        }

        static DateTime lastRead = DateTime.MinValue;
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            DateTime lastWrite = File.GetLastWriteTime(projectPath);
            if (lastWrite != lastRead)
            {
                RunBulid();
                lastRead = lastWrite;
            }
        }
    }
}

