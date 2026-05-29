using System;
using System.Diagnostics;
using System.IO;
namespace Task5
{
    public class Program
    {
        public static void Main()
        {
            string filename = "bigdata.txt";
            if (!File.Exists(filename))
            {
                using (var sw = new StreamWriter(filename))
                {
                    for (int i = 0; i < 5_000_000; i++)
                    {
                        sw.WriteLine("Data line with some A symbols and other chars");
                    }
                }
            }
            Stopwatch swTimer = Stopwatch.StartNew();
            long countA = 0;
            using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[65536];
                int bytesRead;
                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        if (buffer[i] == 65)
                        {
                            countA++;
                        }
                    }
                }
            }
            swTimer.Stop();
            Console.WriteLine($"Символов 'A' найдено: {countA}");
            Console.WriteLine($"Время выполнения: {swTimer.ElapsedMilliseconds} мс");
        }
    }
}