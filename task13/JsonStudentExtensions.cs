using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace task13;

public class StudentJsonService
{
    private readonly JsonSerializerOptions _options;

    public StudentJsonService()
    {
        _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    public string Serialize(Student student)
    {
        if (student == null) throw new ArgumentNullException(nameof(student));
        return JsonSerializer.Serialize(student, _options);
    }

    public Student Deserialize(string json)
    {
        if (json == null) throw new ArgumentNullException(nameof(json));

        var student = JsonSerializer.Deserialize<Student>(json, _options);
        if (student == null) throw new JsonException("Ошибка десериализации JSON");

        student.Validate(); 

        return student;
    }

    public void SaveToFile(string filePath, Student student)
    {
        if (filePath == null) throw new ArgumentNullException(nameof(filePath));
        if (student == null) throw new ArgumentNullException(nameof(student));

        string json = Serialize(student);
        File.WriteAllText(filePath, json);
    }

    public Student LoadFromFile(string filePath)
    {
        if (filePath == null) throw new ArgumentNullException(nameof(filePath));
        if (!File.Exists(filePath)) throw new FileNotFoundException($"Файл не найден: {filePath}");

        string json = File.ReadAllText(filePath);
        return Deserialize(json);
    }
}