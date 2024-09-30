namespace Lesson
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger consoleLogger = new ConsoleLogWritter();
            ILogger fileLogger = new FileLogWritter();
            ILogger secureConsoleLogger = new SecureLogWritter(consoleLogger, DayOfWeek.Friday);
            ILogger secureFileLogger = new SecureLogWritter(fileLogger, DayOfWeek.Friday);
            ILogger compositeLogger = new CompositeLogger(consoleLogger, secureFileLogger);

            Pathfinder pathfinder1 = new Pathfinder(consoleLogger);
            Pathfinder pathfinder2 = new Pathfinder(fileLogger);
            Pathfinder pathfinder3 = new Pathfinder(secureFileLogger);
            Pathfinder pathfinder4 = new Pathfinder(secureConsoleLogger);
            Pathfinder pathfinder5 = new Pathfinder(compositeLogger);

            pathfinder1.Find($"Hi! {nameof(pathfinder1)}");
            pathfinder2.Find($"Hi! {nameof(pathfinder2)}");
            pathfinder3.Find($"Hi! {nameof(pathfinder3)}");
            pathfinder4.Find($"Hi! {nameof(pathfinder4)}");
            pathfinder5.Find($"Hi! {nameof(pathfinder5)}");
        }
    }

    internal class Pathfinder
    {
        private readonly ILogger _logger;

        public Pathfinder(ILogger logger) =>
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        internal void Find(string message) =>
            _logger.WriteError(message);
    }

    public class CompositeLogger : ILogger
    {
        private readonly ILogger[] _loggers;

        public CompositeLogger(params ILogger[] loggers)
        {
            if (loggers.Length <= 0)
                throw new ArgumentNullException(nameof(loggers));

            foreach (var logger in loggers)
                if (logger == null)
                    throw new ArgumentNullException(nameof(logger));

            _loggers = loggers;
        }

        public void WriteError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));

            foreach (var logger in _loggers)
                logger.WriteError(message);
        }
    }

    public interface ILogger
    {
        void WriteError(string message);
    }

    public class ConsoleLogWritter : ILogger
    {
        public void WriteError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));

            Console.WriteLine(message);
        }
    }

    public class FileLogWritter : ILogger
    {
        private const string Path = "log.txt";

        public void WriteError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));

            File.WriteAllText(Path, message);
        }
    }

    public class SecureLogWritter : ILogger
    {
        private readonly ILogger _logger;
        private readonly DayOfWeek _dayOfWeek;

        public SecureLogWritter(ILogger logger, DayOfWeek dayOfWeek)
        {
            if (Enum.IsDefined(dayOfWeek))
                throw new ArgumentException(nameof(dayOfWeek));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dayOfWeek = dayOfWeek;
        }

        public void WriteError(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException($"'{nameof(message)}' cannot be null or whitespace.", nameof(message));

            if (DateTime.Now.DayOfWeek == _dayOfWeek)
            {
                _logger.WriteError(message);
            }
        }
    }
}