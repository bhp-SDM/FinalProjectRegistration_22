using FinalProjectRegistration_22.Interfaces;
using FinalProjectRegistration_22.Models;
using FinalProjectRegistration_22.Services;
using Moq;

namespace XUnitTests
{
    public class StudentServiceTest
    {
        private List<Student> fakeRepo = new List<Student>();
        private Mock<IStudentRepository> studentRepoMock = new Mock<IStudentRepository>();

        public StudentServiceTest()
        {
            studentRepoMock.Setup(x => x.GetAll()).Returns(fakeRepo);
            studentRepoMock.Setup(x => x.GetById(It.IsAny<int>())).Returns<int>(id => fakeRepo.FirstOrDefault(x => x.Id == id));
            studentRepoMock.Setup(x => x.Add(It.IsAny<Student>())).Callback<Student>(s => fakeRepo.Add(s));
            studentRepoMock.Setup(x => x.Update(It.IsAny<Student>())).Callback<Student>(s =>
           {
               var index = fakeRepo.IndexOf(s);
               if (index != -1)
                   fakeRepo[index] = s;
           });
            studentRepoMock.Setup(x => x.Delete(It.IsAny<Student>())).Callback<Student>(s => fakeRepo.Remove(s));
        }

        #region Create StudentService
        [Fact]
        public void CreateStudentService_ValidStudentService_Test()
        {
            // Arrange
            Mock<IStudentRepository> repoMock = new Mock<IStudentRepository>();

            StudentService? service = null;

            // Act
            service = new StudentService(repoMock.Object);

            // Assert
            Assert.NotNull(service);
            Assert.True(service is StudentService);
        }

        [Fact]
        public void CreateStudentService_NullRepository_ExpectArgumentExceptionTest()
        {
            // Arrange
            StudentService? service;

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => service = new StudentService(null));
            Assert.Equal("Missing StudentRepository", ex.Message);
        }
        #endregion

        #region AddStudent
        [Theory]
        [InlineData(1, "Name", "Address", 1234, "City", "Email")]
        [InlineData(2, "Name", "Address", 1234, "City", null)]
        public void AddStudent_ValidStudent_Test(int id, string name, string address, int zipcode, string city, string email)
        {
            // Arrange
            Student s = new Student(id, name, address, zipcode, city, email);

            var service = new StudentService(studentRepoMock.Object);

            // Act
            service.AddStudent(s);

            // Assert
            Assert.True(fakeRepo.Count == 1);
            Assert.Equal(s, fakeRepo[0]);
            studentRepoMock.Verify(r => r.Add(s), Times.Once);
        }

        [Fact]
        public void AddStudent_StudentIsNull_ExpectArgumentException_Test()
        {
            // Arrange
            var service = new StudentService(studentRepoMock.Object);

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => service.AddStudent(null));
            Assert.Equal("Student is missing", ex.Message);
        }

        [Theory]
        [InlineData(0, "Name", "Address", 1234, "City", "Email", "Invalid id")]
        [InlineData(1, null, "Address", 1234, "City", "Email", "Invalid name")]
        [InlineData(1, "", "Address", 1234, "City", "Email", "Invalid name")]
        [InlineData(1, "Name", null, 1234, "City", "Email", "Invalid address")]
        [InlineData(1, "Name", "", 1234, "City", "Email", "Invalid address")]
        [InlineData(1, "Name", "Address", 0, "City", "Email", "Invalid zipcode")]
        [InlineData(1, "Name", "Address", 10000, "City", "Email", "Invalid zipcode")]
        [InlineData(1, "Name", "Address", 1234, null, "Email", "Invalid city")]
        [InlineData(1, "Name", "Address", 1234, "", "Email", "Invalid city")]
        [InlineData(1, "Name", "Address", 1234, "City", "", "Invalid email")]
        public void AddStudent_InvalidStudent_ExpectArgumentException(int id, string name, string address, int zipcode, string city, string email, string expectedMessage)
        {
            var service = new StudentService(studentRepoMock.Object);

            var s = new Student(id, name, address, zipcode, city, email);

            var ex = Assert.Throws<ArgumentException>(() => service.AddStudent(s));
            Assert.Equal(expectedMessage, ex.Message);
            studentRepoMock.Verify(r => r.Add(s), Times.Never);
        }

        #endregion // AddStudent

        #region UpdateStudent

        [Theory]
        [InlineData(1, "NewName", "Address", 1234, "City", "Email")]
        [InlineData(1, "Name", "NewAddress", 1234, "City", "Email")]
        [InlineData(1, "Name", "Address", 2345, "City", "Email")]
        [InlineData(1, "Name", "Address", 1234, "NewCity", "Email")]
        [InlineData(1, "Name", "Address", 1234, "City", "NewEmail")]
        [InlineData(1, "Name", "NewAddress", 1234, "City", null)]
        public void UpdateStudent_ValidStudent_Test(int id, string name, string address, int zipcode, string city, string email)
        {
            // Arrange
            fakeRepo.Add(new Student(id, "Name", "Address", 1234, "City", "Email"));

            var updatedStudent = new Student(id, name, address, zipcode, city, email);

            var service = new StudentService(studentRepoMock.Object);

            // Act
            service.UpdateStudent(updatedStudent);

            // Assert
            Assert.True(fakeRepo.Count == 1);
            Assert.Equal(updatedStudent, fakeRepo[0]);
            studentRepoMock.Verify(r => r.Update(updatedStudent), Times.Once);
        }

        [Fact]
        public void UpdateStudent_StudentIsNull_ExpectArgumentException_Test()
        {
            // Arrange
            Student? updatedStudent = null;

            var service = new StudentService(studentRepoMock.Object);

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateStudent(updatedStudent));
            Assert.Equal("Student is missing", ex.Message);
            studentRepoMock.Verify(r => r.Update(updatedStudent), Times.Never);
        }

        [Fact]
        public void UpdateStudent_StudentDoesNotExist_ExpectArgumentException_Test()
        {
            // Arrange
            var s1 = new Student(1, "name1", "address1", 1234, "city1", "email1");
            var s2 = new Student(2, "name2", "address2", 2345, "city2", "email2");

            fakeRepo.Add(s1);

            var service = new StudentService(studentRepoMock.Object);

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => service.UpdateStudent(s2));
            Assert.Equal("Student id does not exist", ex.Message);
            studentRepoMock.Verify(r => r.Update(s2), Times.Never);
        }

        [Theory]
        [InlineData(0, "Name", "Address", 1234, "City", "Email", "Invalid id")]
        [InlineData(1, null, "Address", 1234, "City", "Email", "Invalid name")]
        [InlineData(1, "", "Address", 1234, "City", "Email", "Invalid name")]
        [InlineData(1, "Name", null, 1234, "City", "Email", "Invalid address")]
        [InlineData(1, "Name", "", 1234, "City", "Email", "Invalid address")]
        [InlineData(1, "Name", "Address", 0, "City", "Email", "Invalid zipcode")]
        [InlineData(1, "Name", "Address", 10000, "City", "Email", "Invalid zipcode")]
        [InlineData(1, "Name", "Address", 1234, null, "Email", "Invalid city")]
        [InlineData(1, "Name", "Address", 1234, "", "Email", "Invalid city")]
        [InlineData(1, "Name", "Address", 1234, "City", "", "Invalid email")]
        public void UpdateStudent_InvalidStudent_ExpectArgumentException(int id, string name, string address, int zipcode, string city, string email, string expectedMessage)
        {
            var service = new StudentService(studentRepoMock.Object);

            var s = new Student(id, name, address, zipcode, city, email);

            var ex = Assert.Throws<ArgumentException>(() => service.UpdateStudent(s));
            Assert.Equal(expectedMessage, ex.Message);
            studentRepoMock.Verify(r => r.Update(s), Times.Never);
        }

        #endregion // UpdateStudent
        #region RemoveStudent

        [Fact]
        public void RemoveStudent_ValidStudent_Test()
        {
            // Arrange
            var s1 = new Student(1, "name1", "address1", 1234, "city1", "email1");
            var s2 = new Student(2, "name2", "address2", 2345, "city2", "email2");

            var fakeRepo = new List<Student>();
            fakeRepo.Add(s1);
            fakeRepo.Add(s2);   

            Mock<IStudentRepository> repoMock = new Mock<IStudentRepository>();
            repoMock.Setup(r => r.Delete(It.IsAny<Student>())).Callback<Student>(s => fakeRepo.Remove(s));
            repoMock.Setup(r => r.GetById(It.IsAny<int>())).Returns<int>(id => fakeRepo.FirstOrDefault(s => s.Id == id));
            
            var service = new StudentService(repoMock.Object);

            // Act
            service.RemoveStudent(s1);

            // Assert
            Assert.True(fakeRepo.Count == 1);
            Assert.Contains(s2, fakeRepo);
            Assert.DoesNotContain(s1, fakeRepo);
            repoMock.Verify(r => r.Delete(s1), Times.Once);
        }

        [Fact]
        public void RemoveStudent_StudentIsNull_ExpectArgumentException()
        {
            // Arrange
            Mock<IStudentRepository> repoMock = new Mock<IStudentRepository>();
            var service = new StudentService(repoMock.Object);

            // Act and assert
            var ex = Assert.Throws<ArgumentException>(() => service.RemoveStudent(null));
            Assert.Equal("Student is missing", ex.Message);
            repoMock.Verify(r => r.Delete(null), Times.Never);
        }

        [Fact]
        public void UpdateStudent_StudentDoesNotExist_ExpectArgumentException()
        {
            // Arrange
            var s1 = new Student(1, "name1", "address1", 1234, "city1", "email1");
            var s2 = new Student(2, "name2", "address2", 2345, "city2", "email2");

            var fakeRepo = new List<Student>();
            fakeRepo.Add(s1);

            Mock<IStudentRepository> repoMock = new Mock<IStudentRepository>();
            repoMock.Setup(r => r.Delete(It.IsAny<Student>())).Callback<Student>(s =>
            {
                fakeRepo.Remove(s);
            });

            var service = new StudentService(repoMock.Object);

            // Act + assert
            var ex = Assert.Throws<ArgumentException>(() => service.RemoveStudent(s2));
            Assert.Equal("Student does not exist", ex.Message);
            Assert.Contains(s1, fakeRepo);
            repoMock.Verify(r => r.Delete(s2), Times.Never);
        }

        #endregion // RemoveStudent
    }
}
