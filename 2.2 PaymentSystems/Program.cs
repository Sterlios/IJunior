using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        Order order = new Order(268, 12000);

        //Выведите платёжные ссылки для трёх разных систем платежа: 
        //pay.system1.ru/order?amount=12000RUB&hash={MD5 хеш ID заказа}
        //order.system2.ru/pay?hash={MD5 хеш ID заказа + сумма заказа}
        //system3.com/pay?amount=12000&curency=RUB&hash={SHA-1 хеш сумма заказа + ID заказа + секретный ключ от системы}
    }
}

public class Order
{
    public readonly int Id;
    public readonly int Amount;

    public Order(int id, int amount) => (Id, Amount) = (id, amount);
}

public interface IPaymentSystem
{
    public string GetPayingLink(Order order);
}

public abstract class PaymentSystem : IPaymentSystem
{
    private readonly HashAlgorithm _algorithm;

    public PaymentSystem(HashAlgorithm algorithm) =>
        _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));

    public abstract string GetPayingLink(Order order);
}

public class PaymentSystem1 : PaymentSystem
{
    public PaymentSystem1(HashAlgorithm algorithm) : base(algorithm)
    {
    }

    //pay.system1.ru/order?amount=12000RUB&hash={MD5 хеш ID заказа}
    public override string GetPayingLink(Order order)
    {

    }
}

public class PaymentSystem2 : PaymentSystem
{
    public PaymentSystem2(HashAlgorithm algorithm) : base(algorithm)
    {
    }

    public override string GetPayingLink(Order order) => throw new NotImplementedException();
}

public class PaymentSystem3 : PaymentSystem
{
    public PaymentSystem3(HashAlgorithm algorithm) : base(algorithm)
    {
    }

    public override string GetPayingLink(Order order) => throw new NotImplementedException();
}
