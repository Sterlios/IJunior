IEnumerable<ISchool> students = new List<ISchool> { new Student() };

foreach(ISchool student in students)
    student.Study();

interface ISchool 
{
    void Study()
    {
        Console.WriteLine("ISchool.Study()");
    }
}

interface IUnivesity
{
    void Study()
    {
        Console.WriteLine("IUnivesity.Study()");
    }
}

class Student : ISchool, IUnivesity
{
    public void Do()
    {
        (this as ISchool).Study();
    }
}



