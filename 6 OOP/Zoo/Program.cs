using System;

class Program
{
    private static void Main()
    {
        List<Animal> animals = new List<Animal>()
        {
            new Animal("Тигр", "Рычит"),
            new Animal("Лев", "Ревет"),
            new Animal("Лошадь", "Ржет"),
            new Animal("Мышь", "Пищит"),
            new Animal("Зебра", "Игогокае")
        };

        AnimalCreator animalCreator = new AnimalCreator();
        AviaryCreator aviaryCreator = new AviaryCreator(animalCreator);
        ZooCreator zooCreator = new ZooCreator(aviaryCreator);
        Zoo zoo = zooCreator.Create(animals);

        zoo.Work();
    }
}

class Zoo
{
    private readonly List<Aviary> _aviaries = new List<Aviary>();

    public Zoo(List<Aviary> aviaries) =>
        _aviaries = aviaries;

    internal void Work()
    {
        int exitCommand = 0;
        bool isWork = true;

        while (isWork)
        {
            Console.Clear();
            ShowAviaries();

            Console.Write($"Выберите номер вольера, к которому подойдете ({exitCommand} - для выхода): ");
            int userNumber = Utils.ReadNumber();

            if (userNumber == exitCommand)
            {
                Console.WriteLine("Программа завершена");
                isWork = false;
            }
            else
            {
                int aviaryIndex = userNumber - 1;

                if (TryGetAviary(aviaryIndex, out Aviary? aviary))
                    aviary?.ShowInfo();
            }

            Console.ReadKey();
        }
    }

    private bool TryGetAviary(int index, out Aviary? aviary)
    {
        if (index < 0 || index >= _aviaries.Count)
        {
            aviary = null;
            Console.WriteLine("Вольера с таким номером нет.");

            return false;
        }

        aviary = _aviaries[index];

        return true;
    }

    private void ShowAviaries()
    {
        Console.WriteLine("Краткая информация по вольерам:");

        for (int i = 0; i < _aviaries.Count; i++)
        {
            Console.Write($"{i + 1}. ");
            _aviaries[i].ShowIntro();
        }
    }
}

class Aviary
{
    private readonly List<Animal> _animals = new List<Animal>();

    public Aviary(List<Animal> animals) =>
        _animals = animals;

    public void ShowInfo()
    {
        Console.WriteLine("Вы полошли к вольеру, и видите, что в нем обитают:");

        for (int i = 0; i < _animals.Count; i++)
        {
            Console.Write($"{i + 1}. ");
            _animals[i].ShowInfo();
        }
    }

    public void ShowIntro() =>
        _animals[0].ShowShortInfo();
}

class Animal
{
    public Animal(string type, string sound)
    {
        Type = type;
        Sound = sound;
    }

    public Animal(Animal animal, string name, string gender)
    {
        Type = animal.Type;
        Sound = animal.Sound;
        Name = name;
        Gender = gender;
    }

    public string Type { get; }
    public string? Name { get; }
    public string? Gender { get; }
    public string Sound { get; }

    public void ShowInfo() =>
        Console.WriteLine($"{Type} {Name} \n\tпол: {Gender} \n\tзвук: {Sound}");

    public void ShowShortInfo() =>
        Console.WriteLine(Type);
}

class ZooCreator
{
    private readonly AviaryCreator _aviaryCreator;

    public ZooCreator(AviaryCreator aviaryCreator) => 
        _aviaryCreator = aviaryCreator;

    public Zoo Create(List<Animal> animals)
    {
        List<Aviary> aviaries = new List<Aviary>();

        for (int i = 0; i < animals.Count; i++)
            aviaries.Add(_aviaryCreator.Create(animals[i]));

        return new Zoo(aviaries);
    }
}

class AviaryCreator
{
    private readonly AnimalCreator _animalCreator;
    private readonly int _minCapacity = 3;
    private readonly int _maxCapacity = 10;

    public AviaryCreator(AnimalCreator animalCreator) =>
        _animalCreator = animalCreator;

    public Aviary Create(Animal animal)
    {
        List<Animal> animals = new List<Animal>();

        int capacity = Utils.GetRandomNumber(_minCapacity, _maxCapacity);

        for (int i = 0; i < capacity; i++)
            animals.Add(_animalCreator.Create(animal));

        return new Aviary(animals);
    }
}

class AnimalCreator
{
    private readonly List<string> _names = new List<string>() { "Петя", "Витя", "Артем", "Лида", "Зая", "Иван", "Шурик" };
    private readonly List<string> _genders = new List<string>() { "самец", "самка", "Неопределившийся" };

    public Animal Create(Animal animal)
    {
        string animalName = _names[Utils.GetRandomNumber(0, _names.Count)];
        string animalGender = _genders[Utils.GetRandomNumber(0, _genders.Count)];

        return new Animal(animal, animalName, animalGender);
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