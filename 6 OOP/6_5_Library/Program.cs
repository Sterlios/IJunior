using System.Net;
using static System.Reflection.Metadata.BlobBuilder;

namespace _6_5_Library
{
    internal class Program
    {
        static void Main()
        {
            Labriary labriary = new Labriary();
            labriary.Work();
        }
    }

    class Book
    {
        public Book(int id, string title, string author, int year)
        {
            Id = id;
            Title = title;
            Author = author;
            Year = year;
        }

        public int Id { get; }
        public string Title { get; }
        public string Author { get; }
        public int Year { get; }

        public override string ToString() =>
            $"{Id}: {Title} ({Year})" +
            $"\n\t{Author}";
    }

    class Labriary
    {
        private readonly Dictionary<int, Book> _books = new Dictionary<int, Book>();
        private readonly BookCreator _bookCreator = new BookCreator();

        private bool IsEmpty => _books.Count == 0;

        public void Work()
        {
            string addBookCommand = "1";
            string removeBookCommand = "2";
            string showBookByParameterCommand = "3";
            string exitCommand = "0";

            bool isWorking = true;

            while (isWorking)
            {
                Console.Clear();

                Console.WriteLine("Список книг:");
                ShowBooks();

                Console.WriteLine();

                Console.WriteLine("Меню:");
                Console.WriteLine($"{addBookCommand} - Добавить книгу.");
                Console.WriteLine($"{removeBookCommand} - Удалить книгу.");
                Console.WriteLine($"{showBookByParameterCommand} - Показать книги.");
                Console.WriteLine($"{exitCommand} - Выход.");
                Console.WriteLine();
                Console.Write("Выберите команду: ");

                string command = Console.ReadLine();

                if (command == addBookCommand)
                    AddBook();
                else if (command == removeBookCommand)
                    RemoveBook();
                else if (command == showBookByParameterCommand)
                    ShowBookByParameter();
                else if (command == exitCommand)
                    isWorking = false;

                Console.ReadKey();
            }
        }

        private void ShowBookByParameter()
        {
            string showBooksByTitleCommand = "1";
            string showBooksByAuthorCommand = "2";
            string showBooksByYearCommand = "3";
            string returnCommand = "0";

            if (IsEmpty)
            {
                Console.WriteLine("Не из чего выбирать");
                return;
            }

            bool isWorking = true;

            while (isWorking)
            {
                Console.Clear();

                Console.WriteLine("Список книг:");
                ShowBooks();

                Console.WriteLine();

                Console.WriteLine("Показать книги по параметру:");
                Console.WriteLine($"{showBooksByTitleCommand} - по названию.");
                Console.WriteLine($"{showBooksByAuthorCommand} - по автору.");
                Console.WriteLine($"{showBooksByYearCommand} - по году.");
                Console.WriteLine($"{returnCommand} - Назад.");
                Console.WriteLine();
                Console.Write("Выберите параметр: ");

                string command = Console.ReadLine();

                if (command == showBooksByTitleCommand)
                    ShowBooksByTitle();
                else if (command == showBooksByAuthorCommand)
                    ShowBooksByAuthor();
                else if (command == showBooksByYearCommand)
                    ShowBooksByYear();
                else if (command == returnCommand)
                    isWorking = false;

                Console.ReadKey();
            }
        }

        private void ShowBooksByYear()
        {
            Console.Write("Введите автора: ");
            int year = Utils.ReadNumber();

            foreach (Book book in _books.Values)
                if (book.Year == year)
                    Console.WriteLine(book);
        }

        private void ShowBooksByAuthor()
        {
            Console.Write("Введите автора: ");
            string author = Console.ReadLine();

            foreach (Book book in _books.Values)
                if (book.Author.ToLower() == author.ToLower())
                    Console.WriteLine(book);
        }

        private void ShowBooksByTitle()
        {
            Console.Write("Введите название: ");
            string title = Console.ReadLine();

            foreach(Book book in _books.Values)
                if(book.Title.ToLower() == title.ToLower())
                    Console.WriteLine(book);
        }

        private void RemoveBook()
        {
            if (IsEmpty)
            {
                Console.WriteLine("Не из чего выбирать");
                return;
            }

            Console.Write("Введите ID книги для удаления: ");
            int bookId = Utils.ReadNumber();

            if (_books.ContainsKey(bookId) == false)
            {
                Console.WriteLine("Такого ID не существует");
                return;
            }

            _books.Remove(bookId);
        }

        private void AddBook()
        {
            Book book = _bookCreator.Create();

            _books.Add(book.Id, book);
        }

        private void ShowBooks()
        {
            if (IsEmpty)
            {
                Console.WriteLine("(Пусто)");
                return;
            }

            foreach (Book book in _books.Values)
                Console.WriteLine(book);
        }
    }

    class BookCreator
    {
        private int _ids = 0;

        public Book Create()
        {
            _ids++;

            Console.Write("Введите название: ");
            string title = Console.ReadLine();

            Console.Write("Введите автора: ");
            string author = Console.ReadLine();

            Console.Write("Введите год: ");
            int year = Utils.ReadNumber();

            return new Book(_ids, title, author, year);
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