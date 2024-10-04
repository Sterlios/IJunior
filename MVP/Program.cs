using System.Data;
using System.Data.SQLite;
using System.Data.SqlTypes;
using System.Reflection;
using System.Security.Cryptography;

class Program
{
    public static void Main()
    {
        PassportService passportService = new PassportService();
        HashGenerator hashGenerator = new HashGenerator();
        Database database = new Database();
        PresenterFactory presentorFactory = new PresenterFactory(passportService, hashGenerator, database);
        PassportView passportView = new PassportView(presentorFactory);

        passportView.ReadPassport();
    }
}

public class PassportView : IView
{
    private readonly PassportPresenter _presenter;

    public PassportView(PresenterFactory presenterfactory)
    {
        if (presenterfactory is null)
            throw new ArgumentNullException(nameof(presenterfactory));

        _presenter = presenterfactory.Create(this);
    }

    public void ShowMessage(string message) =>
        ShowColorMessage(message, ConsoleColor.Green);

    public void ShowError(string message) =>
        ShowColorMessage(message, ConsoleColor.Red);

    public void ReadPassport()
    {
        Console.Write("Введите серию и номер паспорта");

        _presenter.ServePassportData(Console.ReadLine());
    }

    private void ShowColorMessage(string message, ConsoleColor color)
    {
        ConsoleColor defaultColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = defaultColor;
    }
}

public class PresenterFactory
{
    private readonly PassportService _passportService;
    private readonly HashGenerator _hashGenerator;
    private readonly Database _database;

    public PresenterFactory(PassportService passportService, HashGenerator hashGenerator, Database database)
    {
        _passportService = passportService ?? throw new ArgumentNullException(nameof(passportService));
        _hashGenerator = hashGenerator ?? throw new ArgumentNullException(nameof(hashGenerator));
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public PassportPresenter Create(PassportView passportView) =>
        new PassportPresenter(passportView, _passportService, _hashGenerator, _database);
}

public class PassportPresenter
{
    private readonly IView _view;
    private readonly PassportService _passportService;
    private readonly HashGenerator _hashGenerator;
    private readonly Database _database;

    public PassportPresenter(IView view, PassportService passportService, HashGenerator hashGenerator, Database database)
    {
        _view = view ?? throw new ArgumentNullException(nameof(view));
        _passportService = passportService ?? throw new ArgumentNullException(nameof(passportService));
        _hashGenerator = hashGenerator ?? throw new ArgumentNullException(nameof(hashGenerator));
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public void ServePassportData(string? rawPassportData)
    {
        const string HasAccess = "ПРЕДОСТАВЛЕН";
        const string HasNotAccess = "НЕ ПРЕДОСТАВЛЕН";

        try
        {
            Passport passport = _passportService.GetPassport(rawPassportData);
            string passportHash = _hashGenerator.CalculateHash(passport.Data);

            using (_database.Connect())
            {
                DataTable dataTable = _database.GetVotingData(passportHash);

                if (dataTable.Rows.Count <= 0)
                    throw new SqlNullValueException($"Паспорт «{passport.Data}» в списке участников дистанционного голосования НЕ НАЙДЕН");

                string accessStatus = Convert.ToBoolean(dataTable.Rows[0].ItemArray[1]) ? HasAccess : HasNotAccess;

                _view.ShowMessage($"По паспорту «{passport.Data}» доступ к бюллетеню на дистанционном электронном голосовании {accessStatus}");
            }
        }
        catch (ArgumentNullException exception)
        {
            _view.ShowError($"Получено пустое значение: {exception.Message}");
        }
        catch (ArgumentException exception)
        {
            _view.ShowError($"Получено неверное значение: {exception.Message}");
        }
        catch (SqlNullValueException exception)
        {
            _view.ShowMessage($"Нет данных в БД: {exception.Message}");
        }
        catch (SQLiteException exception)
        {
            if (exception.ErrorCode != 1)
                return;

            _view.ShowError($"Файл {_database.Name} не найден. Положите файл в папку вместе с exe.");
        }
    }
}

public class HashGenerator : IDisposable
{
    private readonly HashAlgorithm _hashAlgorithm;

    public HashGenerator() =>
        _hashAlgorithm = SHA256.Create();

    public string CalculateHash(string data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(data);

        return Convert.ToBase64String(_hashAlgorithm.ComputeHash(Convert.FromBase64String(data)));
    }

    public void Dispose() =>
        _hashAlgorithm.Dispose();
}

public class Database : IDisposable
{
    private SQLiteConnection? _connection;

    public string Name => "db.sqlite";

    public Database Connect()
    {
        string connectionString = string.Format("Data Source=" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + Name);
        _connection = new SQLiteConnection(connectionString);
        _connection.Open();

        return this;
    }

    public DataTable GetVotingData(string passportHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace("Получено пустое значение хеша паспорта", passportHash);

        string query = $"select * from passports where num='{passportHash}' limit 1;";

        SQLiteDataAdapter sqLiteDataAdapter = new SQLiteDataAdapter(new SQLiteCommand(query, _connection));
        DataTable dataTable = new DataTable();
        sqLiteDataAdapter.Fill(dataTable);

        return dataTable;
    }

    public void Dispose() =>
        _connection?.Close();
}

public class PassportService
{
    private const int PassportDataLength = 10;

    internal Passport GetPassport(string? rawPassportData)
    {
        if (string.IsNullOrWhiteSpace(rawPassportData))
            throw new ArgumentNullException("Получено пустое значение серии и номера паспорта", nameof(rawPassportData));

        string passportData = rawPassportData.Trim().Replace(" ", string.Empty);

        return passportData.Length != PassportDataLength ?
            throw new ArgumentException("Неверный формат серии или номера паспорта", nameof(passportData)) :
            new Passport(passportData);
    }
}

public interface IView
{
    void ShowError(string message);
    void ShowMessage(string message);
}

public class Passport
{
    public Passport(string passportData)
    {
        if (string.IsNullOrWhiteSpace(passportData))
            throw new ArgumentException($"Получено пустое значение серии и номера паспорта", nameof(passportData));

        Data = passportData;
    }

    public string Data { get; private set; }
}