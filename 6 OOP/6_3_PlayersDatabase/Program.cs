using System;
using System.Collections.Generic;
using System.Threading.Channels;

namespace Library
{
    internal class Program
    {
        static void Main() => 
            new Database().Work();
    }

    class Player
    {
        private readonly string _name;

        public Player(int id, string name)
        {
            Id = id;
            _name = name;
            IsBanned = false;
        }

        public int Id { get; }
        public bool IsBanned { get; private set; }

        public void Ban()
            => IsBanned = true;

        public void Unban()
            => IsBanned = false;

        public override string ToString() =>
            $"{Id}: {_name} - {(IsBanned ? "Забанен" : "Не забанен")}";
    }

    class PlayerCreator
    {
        private int _lastPlayerID = 0;

        public Player Create()
        {
            Console.WriteLine("Введите ник: ");
            string playerName = Console.ReadLine();

            return new Player(++_lastPlayerID, playerName);
        }
    }

    class Database
    {
        private readonly Dictionary<int, Player> _players = new Dictionary<int, Player>();
        private readonly PlayerCreator _playerCreator = new();
        private int _lastRecordId = 0;

        public void Work()
        {
            const string AddPlayerCommand = "Add";
            const string BanPlayerCommand = "Ban";
            const string UnbanPlayerCommand = "Unban";
            const string RemovePlayerCommand = "Remove";
            const string ExitCommand = "Exit";

            bool isWorking = true;

            while (isWorking)
            {
                foreach (var playerInfo in _players)
                    Console.WriteLine($"{playerInfo.Key}) {playerInfo.Value}");

                Console.WriteLine("\nМеню: ");
                Console.WriteLine($"{AddPlayerCommand} - Добавить игрока.");
                Console.WriteLine($"{BanPlayerCommand} - Забанить игрока.");
                Console.WriteLine($"{UnbanPlayerCommand} - Разбанить игрока.");
                Console.WriteLine($"{RemovePlayerCommand} - Удалить игрока.");
                Console.WriteLine($"{ExitCommand} - Выход.");
                Console.WriteLine("\nВыбор команды: ");

                switch (Console.ReadLine())
                {
                    case AddPlayerCommand:
                        AddPlayer();
                        break;

                    case BanPlayerCommand:
                        BanPlayer();
                        break;

                    case UnbanPlayerCommand:
                        UnbanPlayer();
                        break;

                    case RemovePlayerCommand:
                        DeletePlayer();
                        break;

                    case ExitCommand:
                        isWorking = false;
                        break;

                    default:
                        Console.WriteLine("Неопознанная команда!");
                        break;
                }

                Console.ReadLine();
                Console.Clear();
            }
        }

        private void AddPlayer() =>
            _players.Add(++_lastRecordId, _playerCreator.Create());

        private void BanPlayer()
        {
            if (TryGetPlayer(out Player player) == false)
                ShowNotFoundMessage();

            player?.Ban();
        }

        private void UnbanPlayer()
        {
            if (TryGetPlayer(out Player player) == false)
                ShowNotFoundMessage();

            player?.Unban();
        }

        private void DeletePlayer()
        {
            Console.WriteLine("введите id игрока: ");

            if (int.TryParse(Console.ReadLine(), out int playerId) == false)
                ShowNotFoundMessage();

            if (_players.Remove(playerId) == false)
                ShowNotFoundMessage();
        }

        private bool TryGetPlayer(out Player? player)
        {
            player = null;
            Console.WriteLine("введите id игрока: ");

            return int.TryParse(Console.ReadLine(), out int playerId) != false && _players.TryGetValue(playerId, out player);
        }

        private void ShowNotFoundMessage()=>
                Console.WriteLine("Игрок не найден");
    }
}