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
        /*
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        */

        // POST: api/Cohort
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Cohort/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
