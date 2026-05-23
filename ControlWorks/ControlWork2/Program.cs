/*Вариант 1
(7 баллов) Напишите класс DataTransformer.Метод Transform(object obj) должен через рефлексию найти все публичные свойства string с атрибутом [Trimmed] и выполнить для них value.Trim().

(7 баллов) Напишите класс ParallelProcessor. Он принимает List<object> и обрабатывает его: запускает 4 потока (Task.Run или Parallel.ForEach), каждый из которых берет объект и выводит в консоль его тип.

(1 балл) Объедините их: используйте ParallelProcessor для обработки списка, но внутри каждого потока вместо простого вывода типа сначала вызывайте DataTransformer.Transform для объекта. Обеспечьте потокобезопасность трансформации.
*/
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
[AttributeUsage(AttributeTargets.Property)]
public class TrimmedAttribute : Attribute { }
public class DataTransformer
{
    public void Transform(object obj)
    {
        if (obj == null) return;
        lock (obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(string) && prop.CanRead && prop.CanWrite)
                {
                    if (prop.IsDefined(typeof(TrimmedAttribute), false))
                    {
                        string value = (string)prop.GetValue(obj);
                        if (value != null)
                        {
                            prop.SetValue(obj, value.Trim());
                        }
                    }
                }
            }
        }
    }
}
public class ParallelProcessor
{
    public void Process(List<object> items)
    {
        if (items == null || items.Count == 0) return;
        var transformer = new DataTransformer();
        Parallel.ForEach(items, new ParallelOptions { MaxDegreeOfParallelism = 4 }, item =>
        {
            if (item != null)
            {
                transformer.Transform(item);
                Console.WriteLine(item.GetType().Name);
            }
        });
    }
}