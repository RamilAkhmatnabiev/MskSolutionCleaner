using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MskSolutionCleaner
{
    public static class SolutionCleaner
    {
        private static readonly List<string> folderNames;

        private static readonly List<string> fileNames;

        static SolutionCleaner()
        {
            folderNames = ReadLines(System.Configuration.ConfigurationManager.AppSettings["foldersToDelete"]);
            fileNames = ReadLines(System.Configuration.ConfigurationManager.AppSettings["filePostfixesToDelete"]);
            
            Console.WriteLine("Прочитал все каталоги");
        }

        public static bool CanStart()
        {
            Console.WriteLine("Press Enter to start");
            var key = Console.ReadKey();

            return key.Key == ConsoleKey.Enter;
        }

        public static void Clean()
        {
            var rootPath = GetSolutionPath();

            try
            {
                ProcessFolder(rootPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при очистке: {e.Message}");
                throw;
            }
        }

        private static string GetSolutionPath()
        {
            var isCurrentFolderIsSolution =
                System.Configuration.ConfigurationManager.AppSettings["appInSolutionForClean"] == "true";

            return isCurrentFolderIsSolution
                ? Directory.GetCurrentDirectory()
                : System.Configuration.ConfigurationManager.AppSettings["solutionPath"];
        }

        private static void ProcessFolder(string path)
        {
            var folders = Directory.GetDirectories(path);

            foreach (var folder in folders)
            {
                if (folderNames.Contains(new DirectoryInfo(folder.ToLower().Trim()).Name.ToLower().Trim()))
                {
                    try
                    {
                        Console.Write($"Попытка удалить каталог {folder} ... ");

                        var dirInfo = new DirectoryInfo(folder);
                        
                        if (dirInfo.Exists)
                        {
                            SetAttributesNormal(dirInfo);
                            dirInfo.Delete(true);
                        }

                        Console.WriteLine($" Ok");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("!!! FAIL");
                        throw;
                    }
                }
                else
                {
                    ProcessFolder(folder);
                }
            }

            ProcessFile(path);
        }
        
        private static void ProcessFile(string path)
        {
            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                if (fileNames.Any(x => file.ToLower().Trim().EndsWith(x.ToLower().Trim())))
                {
                    try
                    {
                        Console.Write($"Попытка удалить файл {file} ...");
                        
                        File.Delete(file);
                        
                        Console.WriteLine(" Ok");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("!!! FAIL");
                        throw;
                    }
                }
            }
        }

        private static void SetAttributesNormal(DirectoryInfo dir)
        {
            foreach (var subDir in dir.GetDirectories())
            {
                SetAttributesNormal(subDir);
                subDir.Attributes = FileAttributes.Normal;
            }
            foreach (var file in dir.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
            }
        }
        
        private static List<string> ReadLines(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return new List<string>();
            }

            try
            {
                return File.ReadLines(fileName)
                    .Select(x => x.Trim('"').Trim())
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToHashSet()
                    .ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при чтении файлов и каталогов на удаление: {e.Message}");
                throw;
            }
        }
    }
}