using System;
using System.Collections.Generic;

namespace StudentExercisesAPI.Models
{
    public class Student : NSSPerson 
    {
        /*
        public Student(string firstName, string lastName, int id) {
            FirstName = firstName;
            LastName = lastName;
            Id = id;
        }
        */
        public int Id { get; set; }
        
        public List<Exercise> ExerciseList {get; set;} = new List<Exercise>();

        public int CohortId { get; set; }


        public void GetNameAndExercises(Student student)
        {
            Console.WriteLine($"Exercise list for {base.GetFullName()}:");
            Console.WriteLine(" ");
            foreach(Exercise exercise in student.ExerciseList)
            {
                Console.WriteLine($"Exercise Name: {exercise.ExerciseName}");
                Console.WriteLine($"Language: {exercise.ExerciseLanguage}");
                Console.WriteLine(" ");
            }
        }
    }
}