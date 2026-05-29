using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
namespace Task6
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : Attribute { }
    public class Order
    {
        public string Id { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
    }
    public interface IDiscountRule
    {
        void Apply(Order order);
    }
    public class VipDiscount : IDiscountRule
    {
        public void Apply(Order order)
        {
            if (order.TotalAmount > 1000) order.TotalAmount *= 0.9m;
        }
    }
    public class HolidayDiscount : IDiscountRule
    {
        public void Apply(Order order)
        {
            order.TotalAmount -= 50;
        }
    }
    public class SystemCore
    {
        public event Action<string, string> OnOrderStateChanged;
        private SemaphoreSlim _semaphore = new SemaphoreSlim(5);
        private Random _random = new Random();
        public void Validate(Order order)
        {
            var properties = order.GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.GetCustomAttribute<RequiredAttribute>() != null)
                {
                    var value = prop.GetValue(order);
                    if (value == null || (value is decimal d && d == 0))
                    {
                        throw new ArgumentException($"Поле {prop.Name} обязательно.");
                    }
                }
            }
            OnOrderStateChanged?.Invoke(order.Id, "Валидация пройдена");
        }
        public void ApplyDiscounts(Order order)
        {
            var discountTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IDiscountRule).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            foreach (var type in discountTypes)
            {
                var rule = (IDiscountRule)Activator.CreateInstance(type);
                rule.Apply(order);
            }
            OnOrderStateChanged?.Invoke(order.Id, "Скидки применены");
        }
        public async Task SendToDeliveryAsync(Order order)
        {
            await Task.Delay(500);
            if (_random.Next(0, 100) < 1) throw new Exception("Внешняя ошибка API");
            if (order.Status != "Оплачен")
            {
                throw new InvalidOperationException("Заказ не оплачен.");
            }
            OnOrderStateChanged?.Invoke(order.Id, "Отправлен в доставку");
        }
        public async Task ProcessBatchAsync(List<Order> orders)
        {
            var tasks = new List<Task>();
            foreach (var order in orders)
            {
                tasks.Add(ProcessSingleOrderAsync(order));
            }
            await Task.WhenAll(tasks);
        }
        private async Task ProcessSingleOrderAsync(Order order)
        {
            await _semaphore.WaitAsync();
            try
            {
                Validate(order);
                ApplyDiscounts(order);
                await SendToDeliveryAsync(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка заказа {order.Id}: {ex.Message}");
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
    public class Program
    {
        public static async Task Main()
        {
            SystemCore core = new SystemCore();
            core.OnOrderStateChanged += (id, state) => Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Заказ {id}: {state}");
            List<Order> orders = new List<Order>();
            for (int i = 1; i <= 10; i++)
            {
                orders.Add(new Order { Id = i.ToString(), UserEmail = "test@mail.ru", TotalAmount = 1500, Status = "Оплачен" });
            }
            orders.Add(new Order { Id = "11", TotalAmount = 500, Status = "Не оплачен" });
            await core.ProcessBatchAsync(orders);
        }
    }
}