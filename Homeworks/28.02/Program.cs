using System;
using System.Numerics;
namespace Task1
{
    public class Order<T> where T : INumber<T>
    {
        public int Id { get; set; }
        public T BasePrice { get; set; }
        public T FinalPrice { get; set; }
    }
    public interface IIdStep<T> where T : INumber<T>
    {
        IPriceStep<T> SetId(int id);
    }
    public interface IPriceStep<T> where T : INumber<T>
    {
        IFinalStep<T> SetBasePrice(T price);
    }
    public interface IFinalStep<T> where T : INumber<T>
    {
        Order<T> Build();
    }
    public class OrderBuilder<T> : IIdStep<T>, IPriceStep<T>, IFinalStep<T> where T : INumber<T>
    {
        private Order<T> _order = new Order<T>();
        public IPriceStep<T> SetId(int id)
        {
            _order.Id = id;
            return this;
        }
        public IFinalStep<T> SetBasePrice(T price)
        {
            _order.BasePrice = price;
            _order.FinalPrice = price;
            return this;
        }
        public Order<T> Build()
        {
            return _order;
        }
    }
    public abstract class OrderHandler<T> where T : INumber<T>
    {
        protected OrderHandler<T> Next;
        public OrderHandler<T> SetNext(OrderHandler<T> next)
        {
            Next = next;
            return next;
        }
        public virtual void Process(Order<T> order)
        {
            Next?.Process(order);
        }
    }
    public class DiscountHandler<T> : OrderHandler<T> where T : INumber<T>
    {
        private T _discount;
        public DiscountHandler(T discount)
        {
            _discount = discount;
        }
        public override void Process(Order<T> order)
        {
            order.FinalPrice -= _discount;
            base.Process(order);
        }
    }
    public class TaxHandler<T> : OrderHandler<T> where T : INumber<T>
    {
        public override void Process(Order<T> order)
        {
            T multiplier = T.CreateChecked(1.2);
            order.FinalPrice *= multiplier;
            base.Process(order);
        }
    }
    public class ValidationHandler<T> : OrderHandler<T> where T : INumber<T>
    {
        public override void Process(Order<T> order)
        {
            if (order.FinalPrice < T.Zero)
            {
                throw new InvalidOperationException("Цена не может быть отрицательной.");
            }
            base.Process(order);
        }
    }
    public class Program
    {
        public static void Main()
        {
            var orderDecimal = new OrderBuilder<decimal>().SetId(1).SetBasePrice(1000m).Build();
            var handler1 = new DiscountHandler<decimal>(100m);
            var handler2 = new TaxHandler<decimal>();
            var handler3 = new ValidationHandler<decimal>();
            handler1.SetNext(handler2).SetNext(handler3);
            handler1.Process(orderDecimal);
            Console.WriteLine($"Order 1 (decimal): {orderDecimal.FinalPrice}");
            var orderDouble = new OrderBuilder<double>().SetId(2).SetBasePrice(50.5).Build();
            var handler4 = new DiscountHandler<double>(10.0);
            var handler5 = new TaxHandler<double>();
            var handler6 = new ValidationHandler<double>();
            handler4.SetNext(handler5).SetNext(handler6);
            handler4.Process(orderDouble);
            Console.WriteLine($"Order 2 (double): {orderDouble.FinalPrice}");
        }
    }
}