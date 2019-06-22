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
    public class ExerciseController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ExerciseController(IConfiguration config)
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

        //Code for getting a list of exercises
        // GET: api/Exercise
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, ExerciseName, ExerciseLanguage FROM Exercise";

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Exercise> exercises = new List<Exercise>();

                    while (reader.Read())
                    {
                        Exercise exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            ExerciseLanguage = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))
                        };

                        exercises.Add(exercise);
                    }

                    reader.Close();

                    return Ok(exercises);
                }
            }
        }

        //Code for getting a single exercise
        // GET: api/Exercise/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                        Id, ExerciseName, ExerciseLanguage
                                        FROM Exercise
                                        WHERE Id = @id";

                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Exercise exercise = null;

                    if(reader.Read())
                    {
                        exercise = new Exercise
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ExerciseName = reader.GetString(reader.GetOrdinal("ExerciseName")),
                            ExerciseLanguage = reader.GetString(reader.GetOrdinal("ExerciseLanguage"))
                        };
                    }

                    reader.Close();

                    return Ok(exercise);
                }
            }
        }

        //Code for creating an exercise
        // POST: api/Exercise
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Exercise exercise)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Exercise (ExerciseName, ExerciseLanguage)
                                        OUTPUT INSERTED.Id
                                        VALUES (@ExerciseName, @ExerciseLanguage)";
                    cmd.Parameters.Add(new SqlParameter("@ExerciseName", exercise.ExerciseName));
                    cmd.Parameters.Add(new SqlParameter("@ExerciseLanguage", exercise.ExerciseLanguage));

                    int newId =  (int) await cmd.ExecuteScalarAsync();

                    exercise.Id = newId;

                    return CreatedAtAction("Get", new { Id = newId }, exercise);
                }
            }
        }

        // PUT: api/Exercise/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int Id, [FromBody] Exercise exercise)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();

                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Exercise
                                            SET ExerciseName = @ExerciseName,
                                                ExerciseLanguage = @ExerciseLanguage
                                            WHERE Id = @Id";

                        cmd.Parameters.Add(new SqlParameter("@ExerciseName", exercise.ExerciseName));
                        cmd.Parameters.Add(new SqlParameter("@ExerciseLanguage", exercise.ExerciseLanguage));
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
                if (!ExerciseExists(Id))
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
                        cmd.CommandText = "DELETE FROM Exercise WHERE Id = @Id";

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
                if (!ExerciseExists(Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool ExerciseExists(int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT
                                        Id, ExerciseName, ExerciseLanguage
                                        FROM Exercise
                                        WHERE Id = @Id";

                    cmd.Parameters.Add(new SqlParameter("@Id", Id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
