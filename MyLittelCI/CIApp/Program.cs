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
        private const string solutionPath = @"C:\Users\user\Desktop\My_Little_CI\DemoProject\DemoProject.sln";
        private const string ChangeTextFile = @"C:\Users\user\Desktop\Change.txt";

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
                watcher.Created += OnChanged;
                watcher.Changed += MSBuild;
                watcher.EnableRaisingEvents = true;
                Console.ReadKey(); //continue until user enters a key
            }
        }

        private static void MSBuild(object source, FileSystemEventArgs e)
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
        }

        private static void WriteLine(string line)
        {
            File.AppendAllText(ChangeTextFile, Environment.NewLine + line);
            Console.WriteLine(ChangeTextFile, Environment.NewLine + line);
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            WriteLine("Path:" + e.FullPath + "Type:" + e.ChangeType + ",Date:" + DateTime.Now);
        }
    }
}

