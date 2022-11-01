using FinalProjectRegistration_22.Models;

namespace FinalProjectRegistration_22.Interfaces
{
    public interface IStudentRepository
    {
        void Add(Student s);
        void Update(Student s);
        void Delete(Student s);
        Student? GetById(int id);
        IEnumerable<Student> GetAll();
    }
}
