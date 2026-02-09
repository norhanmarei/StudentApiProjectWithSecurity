using StudentApi.Models;
namespace StudentApi.DataSimulation
{
    public class StudentDataSimulation
    {
        public static readonly List<Student> StudentsList = new List<Student>
        {
            new Student{Id = 1, Name = "Norhan Marei", Age = 24, Grade = 97},
            new Student{Id = 2, Name = "Ali Basha", Age = 21, Grade = 40},
            new Student{Id = 3, Name = "Ahmad Ali", Age = 32, Grade = 88},
            new Student{Id = 4, Name = "Sara Ahmad", Age = 33, Grade = 35},
            new Student{Id = 5, Name = "Reem Barak", Age = 22, Grade = 85},
            new Student{Id = 6, Name = "Emily Smith", Age = 20, Grade = 48}
        };
    }
}