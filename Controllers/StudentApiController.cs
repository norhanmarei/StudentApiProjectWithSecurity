using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using StudentApi.DataSimulation;

using StudentApi.Models;

using System.Collections.Generic;
using System.Security.Claims;

namespace StudentApi.Controllers

{
    [Authorize]
    [ApiController]

    [Route("api/students")]

    public class StudentAPIController : ControllerBase

    {
        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<Student>> GetAllStudents()

        {

            if (StudentDataSimulation.StudentsList.Count == 0) return NotFound("No Students Found.");

            return Ok(StudentDataSimulation.StudentsList);

        }

        [AllowAnonymous]
        [HttpGet("Passed", Name = "GetPassedStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<Student>> GetPassedStudents()

        {

            if (StudentDataSimulation.StudentsList.Count == 0) return NotFound("No Students Found.");

            var PassedStudents = StudentDataSimulation.StudentsList.Where(student => student.Grade >= 50);

            return Ok(PassedStudents);

        }

        [AllowAnonymous]
        [HttpGet("Average", Name = "GetAverageGrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<double> GetAverageGrade()

        {

            //StudentDataSimulation.StudentsList.Clear();

            if (StudentDataSimulation.StudentsList.Count == 0) return NotFound("No Students Found.");

            var averageGrade = StudentDataSimulation.StudentsList.Average(student => student.Grade);

            return Ok(averageGrade);

        }


        [HttpGet("{Id}", Name = "GetStudentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Student> GetStudentById(int Id)

        {

            if (Id < 1) return BadRequest($"Bad Request: Not Accepted Id [{Id}]");
            var student = StudentDataSimulation.StudentsList.Find(s => s.Id == Id);
            if (student == null) return NotFound($"Not Found: Student With Id [{Id}] Not Found.");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            bool isAdmin = userRole == "Admin";
            if (!isAdmin && userId != Id) return Forbid();
            return Ok(student);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost(Name = "AddNewStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Student> AddNewStudent(Student student)

        {

            if (student == null || string.IsNullOrEmpty(student.Name) || student.Age < 0 || student.Grade < 0)

                return BadRequest("Invalid Student Info.");

            student.Id = StudentDataSimulation.StudentsList.Count != 0 ? StudentDataSimulation.StudentsList.Max(s => s.Id) + 1 : student.Id = 0;

            StudentDataSimulation.StudentsList.Add(student);

            return CreatedAtRoute("GetStudentById", new { student.Id }, student);

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult DeleteStudent(int Id)

        {

            if (Id <= 0) return BadRequest($"Bad Request: Id [{Id}] Not Valid.");

            int removedCount = StudentDataSimulation.StudentsList.RemoveAll(s => s.Id == Id);

            if (removedCount == 0) return NotFound($"Student With [{Id}] Not Found.");

            return Ok($"Student With Id [{Id}] Removed!");

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{Id}", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<Student> UpdateStudent(int Id, Student updatedStudent)


        {


            if (Id < 1 || string.IsNullOrEmpty(updatedStudent.Name) || updatedStudent.Age < 0 || updatedStudent.Grade < 0)


                return BadRequest("Invalid Student Data.");


            var student = StudentDataSimulation.StudentsList.FirstOrDefault(s => s.Id == Id);


            if (student == null) return NotFound($"Student With Id [{Id}] Is Not Found.");


            student.Name = updatedStudent.Name;


            student.Age = updatedStudent.Age;


            student.Grade = updatedStudent.Grade;


            return Ok(student);


        }
    }

}