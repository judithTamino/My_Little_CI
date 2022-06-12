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

        private static string projectPath = @"C:\Users\yosef\source\repos\My_Little_CI\DemoProject\";






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
                //watcher.Changed += MSBuild;





                watcher.EnableRaisingEvents = true;

                Console.ReadKey(); //continue until user enters a key

            }
        }
        private static void MSBuild(object source, FileSystemEventArgs e)
        {
            string pathToBuildPro = projectPath;
            Process.Start(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe", pathToBuildPro);

        }
        private static void WriteLine(string line)
        {
            File.AppendAllText(@"C:\Users\yosef\OneDrive\Desktop\Demo\Change.txt", Environment.NewLine + line);
            Console.WriteLine(@"C:\Users\yosef\OneDrive\Desktop\Demo\Change.txt", Environment.NewLine + line);

        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            WriteLine("Path:" + e.FullPath + "Type:" + e.ChangeType + ",Date:" + DateTime.Now);

            string pathToBuildSolution = @"C:\Users\yosef\source\repos\My_Little_CI\DemoProject\DemoProject.sln";
            Process.Start(@"C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe", pathToBuildSolution);
        }

        private static void OnRenamed(object source, FileSystemEventArgs e)
        {
            WriteLine("The name renamed to" + e.Name + "," + DateTime.Now);
        }
    }
}

