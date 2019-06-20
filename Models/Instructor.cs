using System;

namespace StudentExercisesAPI.Models
{
    public class Instructor : NSSPerson
    {
        /*
        public Instructor(string firstName, string lastName, string specialty, int id)
        {
            FirstName = firstName;
            LastName = lastName;
            Specialty = specialty;
            Id = id;
        }
        */
        public int Id { get; set; }

        public int CohortId { get; set; }
        
        public string Specialty {get; set;}
      
        public void AssignExercise(Student student, Exercise exercise)
        {
            student.ExerciseList.Add(exercise);
            exercise.ListOfStudents.Add(student);
        }
    }
}