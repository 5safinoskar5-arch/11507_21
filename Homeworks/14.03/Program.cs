using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
namespace Task2
{
    public delegate void LogHandler(string message);
    public class OrderProcessor
    {
        public LogHandler Logger;
        public void Process()
        {
            Logger?.Invoke("Заказ принят");
            Logger?.Invoke("Платеж прошел");
        }
    }
    public class Employee
    {
        public string Name { get; set; }
        public decimal Salary { get; set; }
        public int Experience { get; set; }
    }
    public class User { }
    public static class Extensions
    {
        public static void ForEachWithIndex<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            int index = 0;
            foreach (var item in source)
            {
                action(item, index);
                index++;
            }
        }
    }
    public class Sensor
    {
        public event Action<string, DateTime> OnAlert;
        public void Trigger(string message)
        {
            OnAlert?.Invoke(message, DateTime.Now);
        }
    }
    public class Program
    {
        public static void Main()
        {
            OrderProcessor processor = new OrderProcessor();
            processor.Logger += msg => { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(msg); Console.ResetColor(); };
            processor.Logger += msg => Console.WriteLine(msg);
            processor.Process();
            List<Employee> employees = new List<Employee>
            {
                new Employee { Name = "Иван", Salary = 60000, Experience = 6 },
                new Employee { Name = "Петр", Salary = 40000, Experience = 2 }
            };
            FilterEmployees(employees, e => e.Salary > 50000);
            FilterEmployees(employees, e => e.Experience > 5);
            Action<User> notifyChain = SendEmail;
            notifyChain += SaveToDb;
            notifyChain += UpdateStats;
            SafeInvoke(notifyChain, new User());
            string[] names = { "Иван", "Петр", "Анна" };
            names.ForEachWithIndex((name, index) => Console.WriteLine($"{index + 1}. {name}"));
            Sensor sensor = new Sensor();
            ObservableCollection<string> logs = new ObservableCollection<string>();
            logs.CollectionChanged += (s, e) => Console.WriteLine("Запись добавлена в БД");
            sensor.OnAlert += (msg, time) => Console.WriteLine($"ВКЛЮЧЕНА СИРЕНА: {msg}");
            sensor.OnAlert += (msg, time) => logs.Add($"[{time}] {msg}");
            sensor.Trigger("Взлом двери");
            sensor.Trigger("Критично: Пожар");
            AnalyzeLog(logs, log => log.Contains("Критично"));
        }
        public static void FilterEmployees(List<Employee> list, Predicate<Employee> filter)
        {
            foreach (var emp in list)
            {
                if (filter(emp)) Console.WriteLine(emp.Name);
            }
        }
        public static void SendEmail(User u) => Console.WriteLine("Email sent");
        public static void SaveToDb(User u) { throw new Exception("DB Error"); }
        public static void UpdateStats(User u) => Console.WriteLine("Stats updated");
        public static void SafeInvoke(Action<User> chain, User u)
        {
            foreach (Action<User> handler in chain.GetInvocationList())
            {
                try
                {
                    handler(u);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка в обработчике: {ex.Message}");
                }
            }
        }
        public static void AnalyzeLog<T>(IEnumerable<T> logs, Predicate<T> filter)
        {
            foreach (var log in logs)
            {
                if (filter(log)) Console.WriteLine($"Анализ: {log}");
            }
        }
    }
}