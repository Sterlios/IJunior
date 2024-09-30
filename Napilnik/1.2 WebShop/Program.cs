namespace _1._2_WebShop
{
    internal class Program
    {
        static void Main()
        {
            Good iPhone12 = new Good("IPhone 12");
            Good iPhone11 = new Good("IPhone 11");

            Warehouse warehouse = new Warehouse();

            Shop shop = new Shop(warehouse);

            warehouse.Delive(iPhone12, 10);
            warehouse.Delive(iPhone11, 1);

            //Вывод всех товаров на складе с их остатком

            Cart cart = shop.Cart();
            cart.Add(iPhone12, 4);
            cart.Add(iPhone11, 3); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

            //Вывод всех товаров в корзине

            Console.WriteLine(cart.Order().Paylink);

            cart.Add(iPhone12, 9); //Ошибка, после заказа со склада убираются заказанные товары
        }
    }

    public abstract class Storeage
    {
        private readonly Dictionary<Good, int> _goods;

        protected Storeage() =>
            _goods = new Dictionary<Good, int>();

        protected IReadOnlyDictionary<Good, int> Goods => _goods;

        protected void Add(Good good, int count)
        {
            if (_goods.ContainsKey(good) == false)
                _goods.Add(good, 0);

            _goods[good] += count;

            Show();
        }

        protected void Remove(Good good, int count)
        {
            if (_goods.ContainsKey(good) == false)
                throw new ArgumentException(nameof(good));

            if (count <= 0 || count > _goods[good])
                throw new ArgumentOutOfRangeException(nameof(count));

            _goods[good] -= count;

            if (_goods[good] > 0 == false)
                _goods.Remove(good);
        }

        private void Show()
        {
            int index = 0;

            foreach (var good in _goods)
            {
                index++;
                Console.WriteLine($"{index}: {good.Key.Name} x{good.Value}");
            }
        }
    }

    public class Cart : Storeage
    {
        private readonly IDeliveryService _deliveryService;

        public Cart(IDeliveryService deliveryService) : base() =>
            _deliveryService = deliveryService ?? throw new ArgumentNullException(nameof(deliveryService));

        public new void Add(Good good, int count)
        {
            ArgumentNullException.ThrowIfNull(good);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

            int summaryCount = count;

            if (Goods.ContainsKey(good))
                summaryCount += Goods[good];

            if (_deliveryService.HaveEnoughGoods(good, summaryCount) == false)
                throw new ArgumentOutOfRangeException(nameof(count));

            base.Add(good, count);
        }

        public Order Order()
        {
            foreach (var good in Goods)
                _deliveryService.Transfer(good.Key, good.Value);

            return new Order();
        }
    }

    public class Warehouse : Storeage, IDeliveryService
    {
        public void Delive(Good good, int count)
        {
            ArgumentNullException.ThrowIfNull(good);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

            Add(good, count);
        }

        public bool HaveEnoughGoods(Good good, int count)
        {
            ArgumentNullException.ThrowIfNull(good);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

            if (Goods.ContainsKey(good) == false)
                throw new ArgumentException(nameof(good));

            return Goods[good] >= count;
        }

        public void Transfer(Good good, int count)
        {
            ArgumentNullException.ThrowIfNull(good);

            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(count);

            Remove(good, count);
        }
    }

    public interface IDeliveryService
    {
        void Transfer(Good good, int count);
        bool HaveEnoughGoods(Good good, int count);
    }

    public class Shop
    {
        private readonly Warehouse _warehouse;

        public Shop(Warehouse warehouse) =>
            _warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));

        public Cart Cart() =>
            new Cart(_warehouse);
    }

    public class Order
    {
        public string Paylink => "просто какая-нибудь случайная строка";
    }

    public class Good
    {
        public Good(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public string Name { get; }

        public override bool Equals(object? obj) =>
            obj is not null && ((Good)obj).GetHashCode() == GetHashCode();

        public override int GetHashCode() =>
            string.GetHashCode(Name);
    }
}
