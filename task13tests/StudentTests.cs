using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using task13;

namespace task13tests;

public class StudentTests : IDisposable
{
    private readonly StudentJsonService _service;
    private readonly string _testFilePath;

    public StudentTests()
    {
        _service = new StudentJsonService();
        _testFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");
    }
    public void Dispose()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [Fact]
    public void Serialize_ShouldIgnoreNullPropertiesAndFormatDate()
    {
        var student = new Student
        {
            FirstName = "Ivan",
            LastName = "Ivanov",
            BirthDate = new DateTime(2002, 1, 2),
            Grades = null 
        };

        string json = _service.Serialize(student);

        Assert.Contains("\"BirthDate\": \"2002-01-02\"", json);
        Assert.DoesNotContain("Grades", json);
    }

    [Fact]
    public void Deserialize_ValidJson_ShouldReturnCorrectObject()
    {
        string json = "{\"FirstName\":\"Ivan\",\"LastName\":\"Petrov\",\"BirthDate\":\"2004-12-01\",\"Grades\":[{\"Name\":\"Math\",\"Grade\":95}]}";

        var student = _service.Deserialize(json);

        Assert.NotNull(student);
        Assert.Equal("Ivan", student.FirstName);
        Assert.Equal(new DateTime(2004, 12, 1), student.BirthDate);
        Assert.NotNull(student.Grades);
        Assert.Single(student.Grades);
        Assert.Equal("Math", student.Grades[0].Name);
        Assert.Equal(95, student.Grades[0].Grade);
    }

    [Fact]
    public void Deserialize_InvalidData_ShouldThrowArgumentException()
    {
        string invalidJson = "{\"FirstName\":\"Ivan\",\"LastName\":\"\",\"BirthDate\":\"2004-12-01\"}";

        var exception = Assert.Throws<ArgumentException>(() => _service.Deserialize(invalidJson));
        Assert.Contains("Фамилия студента не может быть пустой", exception.Message);
    }

    [Fact]
    public void Deserialize_InvalidGrade_ShouldThrowArgumentOutOfRangeException()
    {
        string invalidGradeJson = "{\"FirstName\":\"Ivan\",\"LastName\":\"Petrov\",\"BirthDate\":\"2004-12-01\",\"Grades\":[{\"Name\":\"Math\",\"Grade\":150}]}";

        Assert.Throws<ArgumentOutOfRangeException>(() => _service.Deserialize(invalidGradeJson));
    }

    [Fact]
    public void SaveAndLoadFromFile_ShouldPreserveDataCorrectly()
    {
        var student = new Student
        {
            FirstName = "Alex",
            LastName = "Smirnov",
            BirthDate = new DateTime(2003, 1, 20),
            Grades = new List<Subject> 
            { 
                new Subject { Name = "Math", Grade = 90 } 
            }
        };
        _service.SaveToFile(_testFilePath, student);
        var loadedStudent = _service.LoadFromFile(_testFilePath);

        Assert.NotNull(loadedStudent);
        Assert.Equal(student.FirstName, loadedStudent.FirstName);
        Assert.Equal(student.LastName, loadedStudent.LastName);
        Assert.Equal(student.BirthDate, loadedStudent.BirthDate);
        Assert.NotNull(loadedStudent.Grades);
        Assert.Equal(student.Grades[0].Name, loadedStudent.Grades[0].Name);
        Assert.Equal(student.Grades[0].Grade, loadedStudent.Grades[0].Grade);
    }
}