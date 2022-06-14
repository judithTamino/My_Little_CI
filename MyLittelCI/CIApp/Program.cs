using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;
using System.Diagnostics;

namespace CIApp
{
    internal class Program
    {

        private static string projectPath = @"C:\Users\user\Desktop\My_Little_CI\DemoProject\";
        private static string solutionPath = @"C:\Users\user\Desktop\My_Little_CI\DemoProject\DemoProject.sln";
        private static string ChangeTextFile = @"C:\Users\user\Desktop\Change.txt";

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
                //watcher.Changed += MSBuild;
                watcher.EnableRaisingEvents = true;
                Console.ReadKey(); //continue until user enters a key
            }
        }

        private static void MSBuild()
        {
            string pathToBuildSolution = solutionPath;

            var sb = new StringBuilder();
            Process p = new Process();

            // redirect the output
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;

            // hookup the eventhandlers to capture the data that is received
            p.OutputDataReceived += (sender, args) => sb.AppendLine(args.Data);
            p.ErrorDataReceived += (sender, args) => sb.AppendLine(args.Data);

            // direct start
            p.StartInfo.UseShellExecute = false;

            //p.StartInfo.FileName = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe";
            p.StartInfo.FileName = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe";
            p.StartInfo.Arguments = pathToBuildSolution;
               
            //p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.BeginErrorReadLine();
            p.BeginOutputReadLine();

            p.WaitForExit();
            Console.WriteLine(sb.ToString());

            if (p.ExitCode == 0)
                RunTest();
               // NunitTestingConsole();
        }

        private static void RunTest()
        {
            string testDllPath = @"C:\Users\user\Desktop\My_Little_CI\DemoProject\TestProject\bin\Debug\net6.0\TestProject.dll";
            string exeFilePath = @"C:\Users\user\Desktop\My_Little_CI\MyLittelCI\packages\NUnit.ConsoleRunner.3.15.0\tools\nunit3-console.exe";

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.FileName = exeFilePath;
            startInfo.Arguments = testDllPath;

            try
            {
                using (Process p = Process.Start(startInfo))
                    p.WaitForExit();
            }
            catch(Exception e)
            {
                string msg = e.Message;
            }
        }

        private static void WriteLine(string line)
        {
            File.AppendAllText(ChangeTextFile, Environment.NewLine + line);

            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(ChangeTextFile, Environment.NewLine + line);
            Console.ResetColor();

            MSBuild();
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            WriteLine("Path:" + e.FullPath + " " + "Type:" + e.ChangeType + " " + ",Date:" + DateTime.Now);
        }
    }
}

