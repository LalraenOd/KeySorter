using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace KeySorter
{
    internal class Program
    {
        private static DateTime LastRequestDateTime { get; set; }

        private static void Main(string[] args)
        {
            Console.WriteLine("Enter file Directory like:\n" + Directory.GetCurrentDirectory(),
                Console.ForegroundColor = ConsoleColor.Green);
            Console.ForegroundColor = ConsoleColor.Gray;
            string fileDirectory = Console.ReadLine();
            Console.WriteLine("");
            string[] filePaths = Directory.GetFiles(fileDirectory);
            foreach (var filePath in filePaths)
            {
                FileInfo fileInfo = new FileInfo(filePath);
                var fileName = fileInfo.Name;
                var fileCode = fileName.Split('_').First();
                var fopName = "";
                Console.WriteLine("Found code: " + fileCode, Console.ForegroundColor = ConsoleColor.Green);
                Console.WriteLine("Loading info...", Console.ForegroundColor = ConsoleColor.Green);
                if (DateTime.Now < LastRequestDateTime.AddSeconds(10))
                    fopName = fopFinder(fileCode);
                else
                {
                    Thread.Sleep(5000);
                    fopName = fopFinder(fileCode);
                }
                Console.WriteLine("Find: " + fopName);
                var fileResult = fileDirectory + "\\" + fileCode + "_" + fopName;
                Console.WriteLine("Find:\n" + fileName, Console.ForegroundColor = ConsoleColor.Gray);
                if (!Directory.Exists(fileResult))
                {
                    Directory.CreateDirectory(fileResult);
                    Console.WriteLine("Directory created:\n" + fileDirectory + "\\" + fileCode);
                }
                if (!File.Exists(fileResult + "\\" + fileName))
                {
                    Console.WriteLine("Moving file to:\n" + fileDirectory, "\\" + fileCode);
                    File.Copy(filePath, fileResult + "\\" + fileName);
                    Console.WriteLine("Moved {0}\n to {1}\n", filePath, fileDirectory + "\\" + fileName);
                }
                Console.WriteLine();
            }
            Console.WriteLine("Finished", Console.ForegroundColor = ConsoleColor.Green);
            Console.ReadLine();
        }

        private static string fopFinder(string code)
        {
            string result = "";
            string link = "https://youcontrol.com.ua/search/?country=1&q=" + code;
            using (WebClient client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                string page = client.DownloadString(link);
                Regex regex = new Regex(" <meta name=\"description\" content=\"➤➤ (?<result>.+) Повне досьє на");
                Match match = regex.Match(page);
                result = match.Groups["result"].Value.ToString();
                LastRequestDateTime = DateTime.Now;
            }
            return result;
        }
    }
}