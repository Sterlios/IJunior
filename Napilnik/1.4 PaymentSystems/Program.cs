using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        Order order1 = new Order(1, 12000);
        Order order2 = new Order(1, 13000);
        Order order3 = new Order(2, 12000);
        Order order4 = new Order(2, 13000);

        IHashGenerator md5HashGenerator = new MD5HashGenerator();
        IHashGenerator sha1HashGenerator = new SHA1HashGenerator();

        IPaymentSystem paymentSystem1 = new PaymentSystem1(md5HashGenerator);
        IPaymentSystem paymentSystem2 = new PaymentSystem2(md5HashGenerator);
        IPaymentSystem paymentSystem3 = new PaymentSystem3(sha1HashGenerator);

        Console.WriteLine(paymentSystem1.GetPayingLink(order1));
        Console.WriteLine(paymentSystem2.GetPayingLink(order1));
        Console.WriteLine(paymentSystem3.GetPayingLink(order1));
        Console.WriteLine();
        Console.WriteLine(paymentSystem1.GetPayingLink(order2));
        Console.WriteLine(paymentSystem2.GetPayingLink(order2));
        Console.WriteLine(paymentSystem3.GetPayingLink(order2));
        Console.WriteLine();
        Console.WriteLine(paymentSystem1.GetPayingLink(order3));
        Console.WriteLine(paymentSystem2.GetPayingLink(order3));
        Console.WriteLine(paymentSystem3.GetPayingLink(order3));
        Console.WriteLine();
        Console.WriteLine(paymentSystem1.GetPayingLink(order4));
        Console.WriteLine(paymentSystem2.GetPayingLink(order4));
        Console.WriteLine(paymentSystem3.GetPayingLink(order4));

    }
}

public class Order
{
    public readonly int Id;
    public readonly int Amount;

    public Order(int id, int amount)
    {
        if (id <= 0)
            throw new ArgumentOutOfRangeException(nameof(id));

        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        (Id, Amount) = (id, amount);
    }
}

public class PaymentSystem1 : BasePaymentSystem, IPaymentSystem
{
    public PaymentSystem1(IHashGenerator hashGenerator) : base(hashGenerator) { }

    public string GetPayingLink(Order order)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order));

        string hash = GetHash(order.Id.ToString());

        return $"pay.system1.ru/order?amount={order.Amount}RUB&hash={hash}";
    }
}

public class PaymentSystem2 : BasePaymentSystem, IPaymentSystem
{
    public PaymentSystem2(IHashGenerator hashGenerator) : base(hashGenerator) { }

    public string GetPayingLink(Order order)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order));

        string hash = GetHash(order.Id.ToString(), order.Amount.ToString());

        return $"order.system2.ru/pay?hash={hash}";
    }
}

public class PaymentSystem3 : BasePaymentSystem, IPaymentSystem
{
    private readonly string _secretKey;

    public PaymentSystem3(IHashGenerator hashGenerator) : base(hashGenerator) => 
        _secretKey = "secretKey";

    public string GetPayingLink(Order order)
    {
        if (order is null)
            throw new ArgumentNullException(nameof(order));

        string hash = GetHash(order.Amount.ToString(), order.Id.ToString(), _secretKey);

        return $"system3.com/pay?amount={order.Amount}&curency=RUB&hash={hash}";
    }
}

public abstract class BasePaymentSystem
{
    private readonly IHashGenerator _hashGenerator;

    public BasePaymentSystem(IHashGenerator hashGenerator) => 
        _hashGenerator = hashGenerator ?? throw new ArgumentNullException(nameof(hashGenerator));

    protected string GetHash(params string[] keys)
    {
        if (keys.Length <= 0)
            throw new ArgumentNullException();

        string keyText = string.Empty;

        foreach (string key in keys)
            keyText += key;

        return _hashGenerator.GetHash(keyText);
    }
}

public interface IPaymentSystem
{
    public string GetPayingLink(Order order);
}

public class MD5HashGenerator : IHashGenerator
{
    public string GetHash(string key)
    {
        return string.IsNullOrEmpty(key)
            ? throw new ArgumentNullException()
            : Convert.ToBase64String(MD5.HashData(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(key))));
    }
}

public class SHA1HashGenerator : IHashGenerator
{
    public string GetHash(string key)
    {
        return string.IsNullOrEmpty(key)
            ? throw new ArgumentNullException()
            : Convert.ToBase64String(SHA1.HashData(SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(key))));
    }
}

public interface IHashGenerator
{
    public string GetHash(string key);
}