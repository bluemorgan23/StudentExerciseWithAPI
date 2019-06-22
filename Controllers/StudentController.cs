using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StudentExercisesAPI.Models;

namespace StudentExercisesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        private readonly IConfiguration _config;

        public StudentController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        // GET: api/Student
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT s.Id AS StudentId, s.FirstName, s.LastName, s.CohortId, c.CohortName, c.Id AS CohortId FROM Student s JOIN Cohort c ON c.Id = s.CohortId";

                    List<Student> students = new List<Student>();

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            }
                        };

                        students.Add(student);
                    }

                    reader.Close();

                    return Ok(students);
                }
            }
        }

        // GET: api/Student/5
        
        [HttpGet("{Id}")]
        public async Task<IActionResult> Get(int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT s.Id AS StudentId, s.FirstName, s.LastName, s.CohortId, c.CohortName, c.Id AS CohortId FROM Student s JOIN Cohort c ON c.Id = s.CohortId WHERE s.Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@Id", Id));

                    Student student = null;

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    if(reader.Read())
                    {
                        student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            }
                        };
                    }

                    reader.Close();

                    return Ok(student);
                }
            }
        }

        // POST: api/Student
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Student student)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Student (FirstName, LastName, CohortId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@FirstName, @LastName, @CohortId)";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", student.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", student.LastName));
                    cmd.Parameters.Add(new SqlParameter("@CohortId", student.CohortId));

                    int newId = (int)await cmd.ExecuteScalarAsync();

                    student.Id = newId;

                    return CreatedAtAction("Get", new { Id = newId }, student);
                }
            }
        }


        // PUT: api/Student/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int Id, [FromBody] Student student)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Student
                                            SET FirstName = @FirstName,
                                                LastName = @LastName,
                                                CohortId = @CohortId
                                            WHERE Id = @Id";

                        cmd.Parameters.Add(new SqlParameter("@FirstName", student.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", student.LastName));
                        cmd.Parameters.Add(new SqlParameter("@CohortId", student.CohortId));
                        cmd.Parameters.Add(new SqlParameter("@Id", Id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!StudentExists(Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int Id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "DELETE FROM Student WHERE Id = @Id";

                        cmd.Parameters.Add(new SqlParameter("@Id", Id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }

            }
            catch (Exception)
            {
                if (!StudentExists(Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool StudentExists(int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                        Id, FirstName, LastName, CohortId
                                        FROM Student
                                        WHERE Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@Id", Id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
