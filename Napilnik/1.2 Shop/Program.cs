namespace _1._2_Shop
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

    internal class Shop
    {
        private readonly Warehouse _warehouse;

        public Shop(Warehouse warehouse) => 
            _warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));

        internal Cart Cart() => 
            new Cart(_warehouse);
    }

    public abstract class Storage
    {
        private readonly Dictionary<Good, int> _goods;

        public Storage() => 
            _goods = new Dictionary<Good, int>();

        public IReadOnlyDictionary<Good, int> Goods => _goods;

        protected void Add(Good good, int count)
        {
            if(good == null)
                throw new ArgumentNullException(nameof(good));

            if(count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if(_goods.ContainsKey(good))
                _goods[good] += count;
            else
                _goods[good] = count;
        }

        protected void Remove(Good good, int count)
        {
            if (good == null)
                throw new ArgumentNullException(nameof(good));

            if (count <= 0 || count > _goods[good])
                throw new ArgumentOutOfRangeException(nameof(count));

            _goods[good] -= count;

            if (ContainsGood(good) == false)
                _goods.Remove(good);
        }

        private bool ContainsGood(Good good) => 
            good == null ? throw new ArgumentNullException(nameof(good)) : _goods[good] > 0;
    }

    internal class Cart : Storage
    {
        private readonly IDeliveryService _deliveryService;

        public Cart(IDeliveryService deliveryService) => 
            _deliveryService = deliveryService ?? throw new ArgumentNullException(nameof(deliveryService));

        public new void Add(Good good, int count)
        {
            if (good == null)
                throw new ArgumentNullException(nameof(good));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            _deliveryService.Transfer(good, count);
            base.Add(good, count);
        }

        internal Order Order() => 
            new Order(Goods);
    }

    public class Warehouse : Storage, IDeliveryService
    {
        public void Delive(Good good, int count)
        {
            if (good == null)
                throw new ArgumentNullException(nameof(good));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Add(good, count);

            Console.WriteLine($"Поступил на склад товар: \"{good.Name}\" = {count}");

            ShowGoods();
        }

        public void Transfer(Good good, int count)
        {
            if (good == null)
                throw new ArgumentNullException(nameof(good));

            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            Remove(good, count);

            Console.WriteLine($"Ушел со склада товар: \"{good.Name}\" = {count}");

            ShowGoods();
        }

        private void ShowGoods()
        {
            Console.WriteLine("Содержимое склада: ");

            foreach (var goodInfo in Goods)
                Console.WriteLine($"\"{goodInfo.Key.Name}\" = {goodInfo.Value}");

            Console.WriteLine();
        }
    }

    public class Good
    {
        public Good(string name)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        public string Name{ get; }
    }
    
    public class Order
    {
        private static int s_ids = 0;

        private readonly IReadOnlyDictionary<Good, int> _goods;
        private readonly int _id;

        public Order(IReadOnlyDictionary<Good, int> goods)
        {
            if (_goods == null)
                throw new ArgumentNullException(nameof(goods));

            s_ids++;
            _id = s_ids;
            _goods = goods;
        }

        public string Paylink => GenerateOrderText();

        private string GenerateOrderText()
        {
            string text = $"Order: {_id}\n";

            foreach (var goodInfo in _goods)
                text += $"\"{goodInfo.Key.Name}\" = {goodInfo.Value}\n";

            return text;
        }
    }

    internal interface IDeliveryService
    {
        public void Transfer(Good good, int count);
    }
}