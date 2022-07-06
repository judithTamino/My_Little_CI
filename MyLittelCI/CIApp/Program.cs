using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;

namespace CIApp
{
    internal class Program
    {
        private static string[] allPath;
        
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("WELCOME TO MY LITTLE CI");
            Console.ResetColor();

            TagCommit tagCommit = new TagCommit();
            tagCommit.TagAndUploadToGithub();

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
                Console.ReadKey();
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

        private static bool RunBulid()
        {
            string exeMSBulidPath = @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe";
            string solutionPath = allPath[1];

            return RunProcess(exeMSBulidPath, solutionPath);
        }

        private static void RunTest()
        {
            string testDllPath = allPath[2];
            string exeFilePath = allPath[3];

            RunProcess(exeFilePath, testDllPath);
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            UploadToGit uploadToGit = new UploadToGit();

            bool isBulidSuccessfully = RunBulid();
            RunTest();
            int failedTest = GetFailedTest();

            if (failedTest == 0 && isBulidSuccessfully)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("UPLOADING FILES TO GITHUB...");
                Console.ResetColor();

                uploadToGit.UploadFiles(e.FullPath);
            }
        }

        public static int GetFailedTest()
        {
            string testResult = allPath[4];
            XmlDocument document = new XmlDocument();
            document.Load(testResult);

            XmlElement rootElement = document.DocumentElement;
            int failedTest = Convert.ToInt32(rootElement.GetAttribute("failed"));

            return failedTest;
        }
    }
}

