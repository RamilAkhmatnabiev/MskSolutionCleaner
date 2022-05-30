using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;

namespace MskSolutionCleaner
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            if (!SolutionCleaner.CanStart())
            {
                return;
            }
            
            SolutionCleaner.Clean();
            
            Console.WriteLine($"Очистка завершилась");
            Console.Read();
        }
    }
}