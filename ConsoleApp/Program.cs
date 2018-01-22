using System;
using System.Threading;

namespace ConsoleApp
{
    partial class Program
    {
        static void Main()
        {
            var path = @"C:\Users\Cornelia\source\repos\Logger\LoggerWeb\TextFile2.log";
            var maxRows = 30;
            var offset = 0;

            var reader = new CKReader(path, maxRows);

            for (int i = 0; i < 10; i++)
            {
                var logData = reader.ReadLines(offset);
                offset = logData.Offset;
                foreach (var logDataRow in logData.Rows)
                {
                    if (!string.IsNullOrEmpty(logDataRow))
                    {
                        Console.WriteLine(logDataRow);
                    }                    
                }
                Thread.Sleep(2000);
            }
                       
            Console.WriteLine(Environment.NewLine + "Finished" + Environment.NewLine);
            
            Console.ReadKey();
        }
    }
}
