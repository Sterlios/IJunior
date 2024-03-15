namespace _6_6_Shop
{
    internal class Program
    {
        static void Main()
        {
            Seller seller = new Seller();
            Shop shop = new Shop(seller);

            Buyer buyer = new Buyer("Покупатель");
            shop.Serve(buyer);
        }
    }

    class Product
    {
        public Product(string name, int price)
        {
            Name = name;
            Price = price;
        }

        public string Name { get; }
        public int Price { get; }

        public Product Clone() =>
            new Product(Name, Price);
    }

    abstract class Person
    {
        protected int Money;

        private readonly string _name;

        public Person(int money, string name)
        {
            Money = money;
            _name = name;
        }

        protected List<Product> Products { get; } = new List<Product>();

        public void ShowInfo()
        {
            Console.WriteLine(_name);
            Console.WriteLine($"Сумма: {Money}");

            ShowProducts();
        }

        protected abstract void ShowProducts();
    }

    class Buyer : Person
    {
        public Buyer(string name) : base(5000, name) 
        { 
        }

        public void Buy(Product product)
        {
            if (product == null)
                return;

            Money -= product.Price;
            Products.Add(product);
        }

        public bool CanBuy(int cost) =>
            Money >= cost;

        protected override void ShowProducts()
        {
            if (Products.Count == 0)
                Console.WriteLine("(Пусто)");

            for (int i = 0; i < Products.Count; i++)
                Console.WriteLine($"{i+1}: {Products[i].Name}");
        }
    }

    class Seller : Person
    {
        public Seller() : base(0, "Продавец") => 
            FillProducts();

        public bool TryGetProduct(string productName, out Product product)
        {
            foreach(Product currentProduct in Products)
            {
                if(currentProduct.Name == productName)
                {
                    product = currentProduct.Clone();
                    return true;
                }
            }

            Console.WriteLine("Такого товара нет.");
            product = null;
            return false;
        }

        protected override void ShowProducts()
        {
            for (int i = 0; i < Products.Count; i++)
                Console.WriteLine($"{i+1}: {Products[i].Name} | Цена {Products[i].Price}");
        }

        private void FillProducts()
        {
            Products.Add(new Product("Хлеб",50));
            Products.Add(new Product("Молоко",150));
            Products.Add(new Product("Йогурт",200));
            Products.Add(new Product("Пельмени",400));
            Products.Add(new Product("Лимонад",75));
            Products.Add(new Product("Автомобиль",80));
        }

        internal void Sell(Product product) =>
            Money += product.Price;
    }

    class Shop
    {
        private readonly Seller _seller;

        public Shop(Seller seller) => 
            _seller = seller;

        public void Serve(Buyer buyer)
        {
            string exitCommand = "0";

            bool isServing = true;

            while (isServing)
            {
                Console.Clear();
                _seller.ShowInfo();
                Console.WriteLine();
                buyer.ShowInfo();
                Console.WriteLine();

                Console.WriteLine($"введите название товара, который хотите купить ({exitCommand} - для выхода):");
                string productName = Console.ReadLine();

                if (productName == exitCommand)
                {
                    isServing = false;
                }
                else
                {
                    if (_seller.TryGetProduct(productName, out Product product))
                    {
                        if (buyer.CanBuy(product.Price))
                        {
                            _seller.Sell(product);
                            buyer.Buy(product);
                        }
                    }
                }

                Console.ReadKey();
            }
        }
    }
}