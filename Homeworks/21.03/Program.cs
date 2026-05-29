using System;
using System.Collections.Generic;
using System.Text;
namespace Task3
{
    public static class CustomLinq
    {
        public static IEnumerable<T> ShiftLeft<T>(this IEnumerable<T> source, int k)
        {
            var list = new List<T>(source);
            int count = list.Count;
            if (count == 0) yield break;
            k = k % count;
            for (int i = k; i < count; i++) yield return list[i];
            for (int i = 0; i < k; i++) yield return list[i];
        }
        public static IEnumerable<(int x, int y)> GetNeighborhood(this IEnumerable<(int x, int y)> points)
        {
            var result = new HashSet<(int x, int y)>();
            int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
            int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };
            foreach (var p in points)
            {
                for (int i = 0; i < 8; i++)
                {
                    result.Add((p.x + dx[i], p.y + dy[i]));
                }
            }
            foreach (var p in result) yield return p;
        }
        public static IEnumerable<string> FilterUniqueChars(this IEnumerable<string> source)
        {
            foreach (var str in source)
            {
                var counts = new Dictionary<char, int>();
                bool isValid = true;
                foreach (char c in str)
                {
                    if (!counts.ContainsKey(c)) counts[c] = 0;
                    counts[c]++;
                    if (counts[c] > 2)
                    {
                        isValid = false;
                        break;
                    }
                }
                if (isValid) yield return str;
            }
        }
        public static IEnumerable<string> FindWithOneDifference(this IEnumerable<string> source, string target)
        {
            foreach (var str in source)
            {
                if (str.Length != target.Length) continue;
                int diffs = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] != target[i]) diffs++;
                    if (diffs > 1) break;
                }
                if (diffs == 1) yield return str;
            }
        }
    }
    public class Program
    {
        public static void Main()
        {
            int[] arr = { 1, 2, 3, 4, 5 };
            foreach (var item in arr.ShiftLeft(2)) Console.Write(item + " ");
            Console.WriteLine();
            var points = new List<(int, int)> { (0, 0) };
            int count = 0;
            foreach (var p in points.GetNeighborhood()) count++;
            Console.WriteLine($"Точек в окрестности: {count}");
            string[] strings = { "abc", "aabbcc", "aaabbb" };
            foreach (var s in strings.FilterUniqueChars()) Console.WriteLine(s);
            string[] dict = { "cat", "bat", "cot", "dog" };
            foreach (var s in dict.FindWithOneDifference("cat")) Console.WriteLine(s);
        }
    }
}