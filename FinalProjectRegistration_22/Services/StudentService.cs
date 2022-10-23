using FinalProjectRegistration_22.Interfaces;
using FinalProjectRegistration_22.Models;

namespace FinalProjectRegistration_22.Services
{
    public class StudentService
    {
        private IStudentRepository studentRepository;

        public StudentService(IStudentRepository repository)
        {
            studentRepository = repository ?? throw new ArgumentException("Missing StudentRepository");
        }

        public void AddStudent(Student s)
        {
            if (s == null) 
                throw new ArgumentException("Student is missing");
            
            ThrowIfInvalidStudent(s);
            studentRepository.Add(s);
        }

        private void ThrowIfInvalidStudent(Student s)
        {
            if (s.Id < 1) throw new ArgumentException("Invalid id");
            if (string.IsNullOrEmpty(s.Name)) throw new ArgumentException("Invalid name");
            if (string.IsNullOrEmpty(s.Address)) throw new ArgumentException("Invalid address");
            if (s.Zipcode < 1 || s.Zipcode > 9999) throw new ArgumentException("Invalid zipcode");
            if (string.IsNullOrEmpty(s.City)) throw new ArgumentException("Invalid city");
            if (s.Email != null && s.Email.Length == 0) throw new ArgumentException("Invalid email");
        }
    }
}
