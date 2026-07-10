using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace task13;

public class Subject
{
    public string Name { get; set; } = string.Empty;
    public int Grade { get; set; }
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) 
            throw new ArgumentException("Название предмета не может быть пустым.");
        
        if (Grade < 0 || Grade > 100) 
            throw new ArgumentOutOfRangeException(nameof(Grade), "Оценка должна быть в диапазоне от 0 до 100.");
    }
}

public class Student
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonDateTimeConverter))]
    public DateTime BirthDate { get; set; }
    
    public List<Subject>? Grades { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(FirstName)) 
            throw new ArgumentException("Имя студента не может быть пустым.");
        
        if (string.IsNullOrWhiteSpace(LastName)) 
            throw new ArgumentException("Фамилия студента не может быть пустой.");
        
        if (BirthDate > DateTime.Now || BirthDate.Year < 1900) 
            throw new ArgumentException("Указана некорректная дата рождения.");

        if (Grades != null)
        {
            foreach (var subject in Grades)
            {
                if (subject == null) 
                    throw new ArgumentNullException(nameof(Grades), "Элемент списка оценок не может быть null.");
                subject.Validate();
            }
        }
    }
}