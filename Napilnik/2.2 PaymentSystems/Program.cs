using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Order order = new Order(268, 12000, Currency.Rub);

        HashGenerator hashGenerator1 = new HashGenerator(MD5.Create());
        HashGenerator hashGenerator2 = new HashGenerator(SHA1.Create());

        PaymentSystem1 paymentSystem1 = new PaymentSystem1(hashGenerator1);
        PaymentSystem2 paymentSystem2 = new PaymentSystem2(hashGenerator1);
        PaymentSystem3 paymentSystem3 = new PaymentSystem3(hashGenerator2);

        Console.WriteLine(paymentSystem1.GetPayingLink(order));
        Console.WriteLine(paymentSystem2.GetPayingLink(order));
        Console.WriteLine(paymentSystem3.GetPayingLink(order));
    }
}

public class Order
{
    public readonly int Id;
    public readonly int Amount;
    public readonly Currency Currency;

    public Order(int id, int amount, Currency currency)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegative(amount);

        if (Enum.IsDefined(currency) == false)
            throw new ArgumentException(nameof(currency));

        Id = id;
        Amount = amount;
        Currency = currency;
    }
}

public class HashGenerator
{
    private readonly HashAlgorithm _algorithm;

    public HashGenerator(HashAlgorithm algorithm) =>
        _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));

    public string GetHash(params string[] data)
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (string line in data)
            stringBuilder.Append(line);

        byte[] hash = _algorithm.ComputeHash(Encoding.ASCII.GetBytes(stringBuilder.ToString()));

        return Encoding.ASCII.GetString(hash);
    }
}

public interface IPaymentSystem
{
    public string GetPayingLink(Order order);
}

public class PaymentSystem1 : IPaymentSystem
{
    private readonly HashGenerator _hashGenerator;

    public PaymentSystem1(HashGenerator hashGenerator) =>
        _hashGenerator = hashGenerator ?? throw new ArgumentNullException(nameof(hashGenerator));

    public string GetPayingLink(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        string hash = _hashGenerator.GetHash(order.Id.ToString());

        return $"pay.system1.ru/order?amount={order.Amount}{order.Currency.ToString().ToUpper()}&hash={hash}";
    }
}

public class PaymentSystem2 : IPaymentSystem
{
    private readonly HashGenerator _hashGenerator;

    public PaymentSystem2(HashGenerator hashGenerator) =>
        _hashGenerator = hashGenerator ?? throw new ArgumentNullException(nameof(hashGenerator));

    public string GetPayingLink(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        string hash = _hashGenerator.GetHash(order.Id.ToString());

        return $"order.system2.ru/pay?hash={hash}";
    }
}

public class PaymentSystem3 : IPaymentSystem
{
    private readonly HashGenerator _hashGenerator;

    public PaymentSystem3(HashGenerator hashGenerator) =>
        _hashGenerator = hashGenerator ?? throw new ArgumentNullException(nameof(hashGenerator));

    public string GetPayingLink(Order order)
    {
        ArgumentNullException.ThrowIfNull(order);

        string hash = _hashGenerator.GetHash(order.Id.ToString());

        return $"system3.com/pay?amount={order.Amount}&curency={order.Currency.ToString().ToUpper()}&hash={hash}";
    }
}

public enum Currency
{
    Rub,
    Eur,
    Usd
}