using System;
using System.IO;

namespace Lesson
{
    class Program
    {
        static void Main()
        {
            ConsoleLogWritter consoleLogWritter = new ConsoleLogWritter();
            FileLogWritter fileLogWritter = new FileLogWritter();

            Pathfinder pathfinder1 = new Pathfinder(consoleLogWritter);
            Pathfinder pathfinder2 = new Pathfinder(fileLogWritter);
            Pathfinder pathfinder3 = new Pathfinder(new SecureLogWritter(DayOfWeek.Friday, fileLogWritter));
            Pathfinder pathfinder4 = new Pathfinder(new SecureLogWritter(DayOfWeek.Friday, consoleLogWritter));
            Pathfinder pathfinder5 = new Pathfinder(consoleLogWritter, new SecureLogWritter(DayOfWeek.Friday, fileLogWritter));

            pathfinder1.Find($"Hi! {nameof(pathfinder1)}\n");
            pathfinder2.Find($"Hi! {nameof(pathfinder2)}\n");
            pathfinder3.Find($"Hi! {nameof(pathfinder3)}\n");
            pathfinder4.Find($"Hi! {nameof(pathfinder4)}\n");
            pathfinder5.Find($"Hi! {nameof(pathfinder5)}\n");
        }
    }

    class Pathfinder
    {
        private readonly ILogger[] _loggers;

        public Pathfinder(params ILogger[] loggers)
        {
            if (loggers.Length <= 0)
                throw new ArgumentNullException();

            _loggers = loggers;
        }

        public void Find(string message)
        {
            if (message is null)
                throw new ArgumentNullException(nameof(message));

            foreach (var logger in _loggers)
                logger.WriteError(message);
        }
    }

    interface ILogger
    {
        void WriteError(string message);
    }

    class ConsoleLogWritter : ILogger
    {
        public void WriteError(string message) => 
            Console.WriteLine(message);
    }

    class FileLogWritter : ILogger
    {
        public void WriteError(string message) => 
            File.AppendAllText("log.txt", message);
    }

    class SecureLogWritter : ILogger
    {
        private readonly ILogger[] _loggers;
        private readonly DayOfWeek _loggingDay;

        public SecureLogWritter(DayOfWeek loggingDay, params ILogger[] loggers)
        {
            if (loggers.Length <= 0)
                throw new ArgumentNullException();

            _loggers = loggers;
            _loggingDay = loggingDay;
        }

        public void WriteError(string message)
        {
            foreach (var logger in _loggers)
                if (DateTime.Now.DayOfWeek == _loggingDay)
                    logger.WriteError(message);
        }
    }
}