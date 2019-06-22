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
    public class CohortController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CohortController(IConfiguration config)
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
        // GET: api/Cohort
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                        c.Id AS CohortId,
                                        c.CohortName,
                                        s.Id AS StudentId,
                                        s.FirstName AS SFirst,
                                        s.LastName AS SLast,
                                        i.Id AS InstructorId,
                                        i.FirstName AS IFirst,
                                        i.LastName AS ILast
                                        FROM Cohort c
                                        LEFT JOIN Student s ON s.CohortId = c.Id
                                        LEFT JOIN Instructor i ON i.CohortId = c.Id";

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Instructor> instructors = new List<Instructor>();

                    List<Student> students = new List<Student>();

                    List<Cohort> cohorts = new List<Cohort>();

                    while (reader.Read())
                    {
                        Student student = new Student
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                            FirstName = reader.GetString(reader.GetOrdinal("SFirst")),
                            LastName = reader.GetString(reader.GetOrdinal("SLast")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };

                       
                            students.Add(student);
                        

                        

                        Instructor instructor = new Instructor
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                            FirstName = reader.GetString(reader.GetOrdinal("IFirst")),
                            LastName = reader.GetString(reader.GetOrdinal("ILast")),
                            CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        };

                        instructors.Add(instructor);

                        
                        Cohort cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            ListOfStudents = students.Where(s => s.CohortId == reader.GetInt32(reader.GetOrdinal("CohortId"))).ToList(),
                            ListOfInstructors = instructors.Where(i => i.CohortId == reader.GetInt32(reader.GetOrdinal("CohortId"))).ToList()
                        };

                        if(cohorts.Any(c => c.Id == cohort.Id))
                        {
                            //cohort.ListOfInstructors.Add(instructor);

                            //cohort.ListOfStudents.Add(student);

                            Cohort cohortToUpdate = cohorts.Find(c => c.Id == cohort.Id);
                            if(!cohortToUpdate.ListOfInstructors.Any(i => i.Id == instructor.Id))
                            {
                                cohortToUpdate.ListOfInstructors.Add(instructor);
                            }
                            
                            if (!cohortToUpdate.ListOfStudents.Any(s => s.Id == student.Id))
                            {
                                cohortToUpdate.ListOfStudents.Add(student);
                            }
                            
                        }
                        else
                        {
                            cohorts.Add(cohort);
                        }
                        
                    }

                    reader.Close();

                    return Ok(cohorts);
                }
            }
            
            
        }

        // GET: api/Cohort/5
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT 
                                        c.Id AS CohortId,
                                        c.CohortName,
                                        s.Id AS StudentId,
                                        s.FirstName AS SFirst,
                                        s.LastName AS SLast,
                                        i.Id AS InstructorId,
                                        i.FirstName AS IFirst,
                                        i.LastName AS ILast
                                        FROM Cohort c
                                        LEFT JOIN Student s ON s.CohortId = c.Id
                                        LEFT JOIN Instructor i ON i.CohortId = c.Id
                                        WHERE c.Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@Id", Id));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Instructor> instructors = new List<Instructor>();

                    List<Student> students = new List<Student>();

                    Cohort cohort = null;

                    if (reader.Read())
                    {
                        //Student student = new Student
                        //{
                        //    Id = reader.GetInt32(reader.GetOrdinal("StudentId")),
                        //    FirstName = reader.GetString(reader.GetOrdinal("SFirst")),
                        //    LastName = reader.GetString(reader.GetOrdinal("SLast")),
                        //    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        //};


                        //students.Add(student);




                        //Instructor instructor = new Instructor
                        //{
                        //    Id = reader.GetInt32(reader.GetOrdinal("InstructorId")),
                        //    FirstName = reader.GetString(reader.GetOrdinal("IFirst")),
                        //    LastName = reader.GetString(reader.GetOrdinal("ILast")),
                        //    CohortId = reader.GetInt32(reader.GetOrdinal("CohortId"))
                        //};

                        //instructors.Add(instructor);


                        cohort = new Cohort
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CohortId")),
                            CohortName = reader.GetString(reader.GetOrdinal("CohortName")),
                            ListOfStudents = students.Where(s => s.CohortId == reader.GetInt32(reader.GetOrdinal("CohortId"))).ToList(),
                            ListOfInstructors = instructors.Where(i => i.CohortId == reader.GetInt32(reader.GetOrdinal("CohortId"))).ToList()
                        };

                        
                            //if (!cohort.ListOfInstructors.Any(i => i.Id == instructor.Id))
                            //{
                            //    cohort.ListOfInstructors.Add(instructor);
                            //}

                            //if (!cohort.ListOfStudents.Any(s => s.Id == student.Id))
                            //{
                            //    cohort.ListOfStudents.Add(student);
                            //}                   

                    }

                    reader.Close();

                    return Ok(cohort);
                }
            }


        }


        // POST: api/Cohort
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Cohort cohort)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Cohort (CohortName)
                                        OUTPUT INSERTED.Id
                                        VALUES (@CohortName)";
                    cmd.Parameters.Add(new SqlParameter("@CohortName", cohort.CohortName));

                    int newId = (int)await cmd.ExecuteScalarAsync();

                    cohort.Id = newId;

                    return CreatedAtAction("Get", new { Id = newId }, cohort);
                }
            }
        }

        // PUT: api/Cohort/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int Id, [FromBody] Cohort cohort)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Cohort
                                            SET CohortName = @CohortName
                                            WHERE Id = @Id";

                        cmd.Parameters.Add(new SqlParameter("@CohortName", cohort.CohortName));
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
                if (!CohortExists(Id))
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
                        cmd.CommandText = "DELETE FROM Cohort WHERE Id = @Id";

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
                if (!CohortExists(Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CohortExists(int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                        Id, CohortName
                                        FROM Cohort
                                        WHERE Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@Id", Id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
