using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiagramClient
{
    internal class Person
    {
        public int ID { get; set; }
    }

    internal class Teacher : Person
    {
        public Subject[] Subjects { get; set; }
    }

    internal class Professor : Teacher
    {
        public string[] Courses { get; set; }
    }

    internal class Student : Person
    {
        public int Name { get; set; }
        public Address Address { get; set; }
        public Subject[] Subjects { get; set; }
    }

    internal class RemoteStudent : Student
    {
        public string SourceSchool { get; set; }
    }

    internal class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public int ZIP { get; set; }
    }

    internal class Subject
    {
        public string Name { get; set; }
        public Student[] Students { get; set; }
        public Gradebook Gradebook { get; set; }
    }

    internal class Gradebook
    {
        public Dictionary<Student, StudentRecords> Grades { get; set; }
    }

    internal class StudentRecords
    {
        public List<Grade> Grades { get; set; }
    }

    internal class Grade
    {
        public DateTime DateTime { get; set; }
        public int Value { get; set; }
    }
}
