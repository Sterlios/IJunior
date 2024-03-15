namespace tutorial
{
    public class Program
    {
        static void Main()
        {
            string[] values = { "6", "7", "8", "9", "10", "V", "D", "K", "A" };
            string[] suits = { "♥", "♦", "♣", "♠" };

            Deck deck = new DeckCreator().Create(values, suits);
            Player player = new Player();
            Game game = new Game(deck);
            game.Play(player);
        }
    }

    internal class Game
    {
        private readonly Deck _deck;

        public Game(Deck deck) =>
            _deck = deck;

        public void Play(Player player)
        {
            string takeCardCommand = "1";
            string exitCommand = "0";

            bool isWork = true;

            while (isWork)
            {
                Console.Clear();
                _deck.ShowInfo();

                Console.WriteLine($"Берем карту или заканчиваем игру? ({takeCardCommand} - взять карту, {exitCommand} - выход)");
                string userInput = Console.ReadLine();

                if (userInput == takeCardCommand)
                {
                    if (_deck.TryGetCard(out Card card))
                        player.TakeCard(card);
                }
                else if (userInput == exitCommand)
                {
                    player.ShowCards();
                    isWork = false;
                }
                else
                {
                    Console.WriteLine("Неизвестная команда.");
                }
            }
        }
    }

    class Player
    {
        private readonly List<Card> _cards = new List<Card>();

        public void TakeCard(Card card) =>
            _cards.Add(card);

        public void ShowCards()
        {
            foreach (var card in _cards)
                Console.WriteLine($"|{card.Value:2}{card.Suit}|");
        }
    }

    class Card
    {
        public Card(string value, string suit)
        {
            Value = value;
            Suit = suit;
        }

        public string Value { get; }
        public string Suit { get; }
    }

    class Deck
    {
        private Stack<Card> _cards;

        public Deck(Stack<Card> cards)
        {
            _cards = cards;
            Shuffle();
        }

        public bool TryGetCard(out Card? card)
        {
            if (_cards.Count == 0)
            {
                card = null;
                return false;
            }

            card = _cards.Pop();
            return true;
        }

        public void ShowInfo() =>
            Console.WriteLine($"В колоде осталось {_cards.Count} карт.");

        private void Shuffle()
        {
            Card[] cards = _cards.ToArray();

            for (int i = 0; i < cards.Length; i++)
            {
                int randomIndex = Utils.GetRandomNumber(0, cards.Length);
                (cards[i], cards[randomIndex]) = (cards[randomIndex], cards[i]);
            }

            _cards = new Stack<Card>(cards);
        }
    }

    class DeckCreator
    {
        public Deck Create(string[] values, string[] suits)
        {
            Stack<Card> cards = new Stack<Card>();

            foreach (string value in values)
                foreach (string suit in suits)
                    cards.Push(new Card(value, suit));

            return new Deck(cards);
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