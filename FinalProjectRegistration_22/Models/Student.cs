using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProjectRegistration_22.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }

        public int Zipcode { get; set; }
        public string? City { get; set; }

        public string? Email { get; set; }

        public Student() {}

        public Student(int id, string name, string address, int zip, string city, string? email)
        {
            Id = id;
            Name = name;
            Address = address; 
            Zipcode = zip;
            City = city;
            Email = email;
        }

        public Student(int id, string name, string address, int zip, string city)
            : this(id, name, address, zip, city, null) {}

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            Student other = (Student)obj;
            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
