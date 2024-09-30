using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


public interface IObjectFactory
{
    object Create(string data);
}


public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int TeacherId { get; set; }
    public List<int> StudentIds { get; set; } = new List<int>();

    
    public override string ToString()
    {
        return $"id курса = {Id} Название = {Name} id преподавателя курса = {TeacherId} , id ученика = {string.Join(",", StudentIds)}";
    }
}


public class CourseFactory : IObjectFactory
{
    public object Create(string data)
    {
        try
        {
            var regex = new Regex(@"id курса\s*=\s*(\d+)\s*Название\s*=\s*([^\s]+)\s*id преподавателя курса\s*=\s*(\d+)\s*,\s*id ученика\s*=\s*([\d,]+)");
            var match = regex.Match(data);

            if (!match.Success)
                throw new FormatException("Строка не соответствует ожидаемому формату.");

            int id = int.Parse(match.Groups[1].Value);            
            string name = match.Groups[2].Value;                  
            int teacherId = int.Parse(match.Groups[3].Value);    
            var studentIds = new List<int>(Array.ConvertAll(match.Groups[4].Value.Split(','), int.Parse)); 

            return new Course
            {
                Id = id,
                Name = name,
                TeacherId = teacherId,
                StudentIds = studentIds
            };
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Ошибка формата данных при создании Course: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Неизвестная ошибка при создании Course: {ex.Message}");
        }

        return null;
    }
}


public class DataManager
{
    public List<Course> Courses { get; set; } = new List<Course>();

  
    public void Save(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var course in Courses)
            {
                writer.WriteLine(course.ToString());
            }
        }
    }

    public void Load(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
               
                var courseFactory = new CourseFactory();
                var course = courseFactory.Create(line);
                if (course != null)
                {
                    Courses.Add((Course)course);
                }
            }
        }
    }
}


class Program
{
    static void Main(string[] args)
    {
        DataManager dataManager = new DataManager();


        var course1 = new Course
        {
            Id = 1,
            Name = "Math",
            TeacherId = 1,
            StudentIds = new List<int> { 234, 12, 11 }
        };

        var course2 = new Course
        {
            Id = 2,
            Name = "Physics",
            TeacherId = 2,
            StudentIds = new List<int> { 234, 45, 78 }
        };

        var course3 = new Course
        {
            Id = 3,
            Name = "Chemistry",
            TeacherId = 3,
            StudentIds = new List<int> { 12, 11, 99 }
        };

        
        dataManager.Courses.Add(course1);
        dataManager.Courses.Add(course2);
        dataManager.Courses.Add(course3);


        
        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "data.txt");
        dataManager.Save(filePath);
        Console.WriteLine($"Данные сохранены в файл: {filePath}");

       
        DataManager newManager = new DataManager();
        newManager.Load(filePath);

        
        Console.WriteLine("\nЗагруженные данные:");
        foreach (var courseLoaded in newManager.Courses)
        {
            Console.WriteLine(courseLoaded.ToString()); 
        }
    }
}
