namespace task02;

public class Student
{
    public string Name { get; set; }
    public string Faculty { get; set; }
    public List<int> Grades { get; set; }
}
public class StudentService
{
    private readonly List<Student> _students;
    public StudentService(List<Student> students) => _students = students;

    public IEnumerable<Student> GetStudentsByFaculty(string faculty)
    =>  _students.Where(x => x.Faculty.Equals(faculty, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Student> GetStudentsWithMinAverageGrade(double minAverageGrade) 
    => _students.Where(x => x.Grades.Any() && x.Grades.Average() >= minAverageGrade);

    public IEnumerable<Student> GetStudentsOrderedByName()
        => _students.OrderBy(x => x.Name);

    public ILookup<string, Student> GroupStudentsByFaculty()
        => _students.ToLookup(x => x.Faculty);

    public string GetFacultyWithHighestAverageGrade()
        => _students.GroupBy(x => x.Faculty)
                    .OrderByDescending(g => g.SelectMany(x => x.Grades).DefaultIfEmpty(0).Average())
                    .Select(g => g.Key)
                    .FirstOrDefault() ?? string.Empty;
}

