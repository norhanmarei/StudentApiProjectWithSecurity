using BCrypt.Net;
using StudentApi.Models;
namespace StudentApi.DataSimulation
{
    public class StudentDataSimulation
    {
        public static readonly List<Student> StudentsList = new List<Student>
        {
            new Student{
                Id = 1,
                Name = "Norhan Marei",
                Age = 24,
                Grade = 97,
                Email = "norhan.marie@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123"),
                Role = "Admin"
                },
            new Student{
                Id = 2,
                Name = "Ali Basha",
                Age = 21,
                Grade = 40,
                Email = "ali.basha@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password2"),
                Role = "Student"
                },
            new Student{
                Id = 3,
                Name = "Ahmad Ali",
                Age = 32,
                Grade = 88,
                Email = "ahmad.ali@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password3"),
                Role = "Student"
                },
            new Student{
                Id = 4,
                Name = "Sara Ahmad",
                Age = 33,
                Grade = 35,
                Email = "sara.ahmad@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password4"),
                Role = "Student"
                },
            new Student{
                Id = 5,
                Name = "Reem Barak",
                Age = 22,
                Grade = 85,
                Email = "reem.barak@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password5"),
                Role = "Student"
                },
            new Student{
                Id = 6,
                Name = "Emily Smith",
                Age = 20,
                Grade = 48,
                Email = "emily.smith@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password1"),
                Role = "Student"
                }
        };
    }
}