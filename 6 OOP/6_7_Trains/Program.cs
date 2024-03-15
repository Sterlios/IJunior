namespace _6_7_Trains
{
    internal class Program
    {
        static void Main()
        {
            Station station = new Station();

            station.Work();
        }
    }

    class Station
    {
        public void Work()
        {
            string exitCommand = "0";
            bool isWorking = true;

            Console.WriteLine("ЖД Станция открыта");

            while (isWorking)
            {
                Console.WriteLine($"Формируем поезд? ({exitCommand} - нет)");


                if (Console.ReadLine() != exitCommand)
                {
                    Console.WriteLine();

                    Direction direction = CreateDirection();
                    int tickets = SellTickets();
                    Train train = CreateTrain(direction, tickets);
                    train.Run();
                }
                else
                {
                    isWorking = false;
                }
            }

            Console.WriteLine("ЖД Станция закрыта");
        }

        private Train CreateTrain(Direction direction, int tickets)
        {
            int minCarrigeCapacity = 10;
            int maxCarrigeCapacity = 20;
            List<Carriage> carriages = new List<Carriage>();

            while(tickets > 0)
            {
                Carriage carriage = new Carriage(Utils.GetRandomNumber(minCarrigeCapacity, maxCarrigeCapacity));
                int filledSeatsCount = Math.Min(carriage.EmptySeatsCount, tickets);
                carriage.Fill(filledSeatsCount);
                tickets -= filledSeatsCount;
                carriages.Add(carriage);
            }

            return new Train(carriages, direction);
        }

        private int SellTickets()
        {
            int minTicketsCount = 40;
            int maxTicketsCount = 120;

            return Utils.GetRandomNumber(minTicketsCount, maxTicketsCount);
        }

        private Direction CreateDirection()
        {
            string fromCity = "Тамара";

            List<string> cities = new List<string>()
            {
                "Москва",
                "СПб",
                "Екатеринбург",
                "Новгород",
                "Сочи",
                "Самара",
                "Саратов",
            };

            string toCity = cities[Utils.GetRandomNumber(0,cities.Count)];

            return new Direction(fromCity, toCity);
        }
    }

    class Direction
    {
        private readonly string _fromCity;
        private readonly string _toCity;

        public Direction(string fromCity, string toCity)
        {
            _fromCity = fromCity;
            _toCity = toCity;
        }

        public override string ToString() =>
            $"{_fromCity}-{_toCity}";
    }

    class Carriage
    {
        private readonly int _capacity;

        public Carriage(int сapacity) =>
            _capacity = сapacity;

        public int FilledSeatsCount { get; private set; }
        public int EmptySeatsCount => _capacity - FilledSeatsCount;

        public void Fill(int passangersCount) => 
            FilledSeatsCount += passangersCount;

        public override string ToString() =>
            $"[{FilledSeatsCount}/{_capacity}]";
    }

    class Train
    {
        private readonly List<Carriage> _carriages;
        private readonly Direction _direction;

        public Train(List<Carriage> carriages, Direction direction)
        {
            _carriages = carriages;
            _direction = direction;
        }

        public void Run()
        {
            Console.WriteLine($"По направлению: {_direction}");

            Console.WriteLine($"Количество пассажиров: {CalculatePassengers()}");

            ShowComposition();

            Console.WriteLine("Поезд отправлен");
        }

        private object CalculatePassengers()
        {
            int count = 0;

            foreach (Carriage carriage in _carriages)
                count += carriage.FilledSeatsCount;

            return count;
        }

        private void ShowComposition()
        {
            Console.Write("Состав: ");

            foreach (Carriage carriage in _carriages)
                Console.Write($"{carriage}-");

            Console.WriteLine("[Голова]");
        }
    }

    static class Utils
    {
        private static readonly Random s_random = new Random();

        public static int GetRandomNumber(int min, int max) =>
            s_random.Next(min, max);

        public static int ReadNumber()
        {
            int number;

            while (int.TryParse(Console.ReadLine(), out number) == false)
                Console.WriteLine("Число не распознано");

            return number;
        }
    }
}