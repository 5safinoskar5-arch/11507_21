using System;
using System.IO;
using System.Text.Json;
namespace Task4
{
    public class Config
    {
        public decimal PriceCappuccino { get; set; }
        public int Water { get; set; }
        public int Milk { get; set; }
        public int Beans { get; set; }
    }
    public class DailyReport
    {
        public decimal TotalRevenue { get; set; }
        public int ItemsSold { get; set; }
    }
    public class Program
    {
        public static void Main()
        {
            string configPath = "config.json";
            if (!File.Exists(configPath))
            {
                var defaultConfig = new Config { PriceCappuccino = 150, Water = 1000, Milk = 500, Beans = 300 };
                File.WriteAllText(configPath, JsonSerializer.Serialize(defaultConfig));
            }
            string configJson = File.ReadAllText(configPath);
            Config config = JsonSerializer.Deserialize<Config>(configJson);
            string historyPath = "sales_history.txt";
            File.AppendAllText(historyPath, $"[{DateTime.Now}] Продано: Капучино, Цена: {config.PriceCappuccino}\n");
            GenerateReport(historyPath);
        }
        public static void GenerateReport(string historyPath)
        {
            decimal total = 0;
            int count = 0;
            if (File.Exists(historyPath))
            {
                string[] lines = File.ReadAllLines(historyPath);
                foreach (var line in lines)
                {
                    if (line.Contains("Цена: "))
                    {
                        string priceStr = line.Substring(line.LastIndexOf("Цена: ") + 6);
                        if (decimal.TryParse(priceStr, out decimal price))
                        {
                            total += price;
                            count++;
                        }
                    }
                }
            }
            var report = new DailyReport { TotalRevenue = total, ItemsSold = count };
            string reportName = $"report_{DateTime.Now:yyyy_MM_dd}.json";
            File.WriteAllText(reportName, JsonSerializer.Serialize(report));
        }
    }
}