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
    public class InstructorController : ControllerBase
    {

        private readonly IConfiguration _config;

        public InstructorController(IConfiguration config)
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
        // GET: api/Instructor
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                            i.Id AS InstructorId,
                                            i.FirstName,
                                            i.LastName,
                                            i.CohortId,
                                            c.Id AS CId,
                                            c.CohortName
                                            FROM Instructor i
                                            JOIN Cohort c ON c.Id = i.CohortId";

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Instructor> instructors = new List<Instructor>();

                    while (reader.Read())
                    {
                        Instructor instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            }
                        };

                        instructors.Add(instructor);
                    }

                    reader.Close();

                    return Ok(instructors);
                }
            }
        }

        
        // GET: api/Instructor/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                            i.Id AS InstructorId,
                                            i.FirstName,
                                            i.LastName,
                                            i.CohortId,
                                            c.Id AS CId,
                                            c.CohortName
                                            FROM Instructor i
                                            JOIN Cohort c ON c.Id = i.CohortId
                                            WHERE i.Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@Id", Id));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Instructor instructor = null;

                    if (reader.Read())
                    {
                        instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            Cohort = new Cohort
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CId")),
                                CohortName = reader.GetString(reader.GetOrdinal("CohortName"))
                            }
                        };

                    }

                    reader.Close();

                    return Ok(instructor);
                }
            }

        }

        // POST: api/Instructor
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Instructor instructor)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Instructor (FirstName, LastName, CohortId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@FirstName, @LastName, @CohortId)";
                    cmd.Parameters.Add(new SqlParameter("@FirstName", instructor.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@LastName", instructor.LastName));
                    cmd.Parameters.Add(new SqlParameter("@CohortId", instructor.CohortId));

                    int newId = (int)await cmd.ExecuteScalarAsync();

                    instructor.Id = newId;

                    return CreatedAtAction("Get", new { Id = newId }, instructor);
                }
            }
        }

        // PUT: api/Instructor/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int Id, [FromBody] Instructor instructor)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Instructor
                                            SET FirstName = @FirstName,
                                                LastName = @LastName,
                                                CohortId = @CohortId
                                            WHERE Id = @Id";

                        cmd.Parameters.Add(new SqlParameter("@FirstName", instructor.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@LastName", instructor.LastName));
                        cmd.Parameters.Add(new SqlParameter("@CohortId", instructor.CohortId));
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
                if (!InstructorExists(Id))
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
                        cmd.CommandText = "DELETE FROM Instructor WHERE Id = @Id";

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
                if (!InstructorExists(Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool InstructorExists(int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                        Id, FirstName, LastName, CohortId
                                        FROM Instructor
                                        WHERE Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@Id", Id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
