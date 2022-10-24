using FinalProjectRegistration_22.Interfaces;
using FinalProjectRegistration_22.Models;
using FinalProjectRegistration_22.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTests
{
    public class StudentServiceTest
    {
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
            StudentService? service = null;

            // Act + Assert
            var ex = Assert.Throws<ArgumentException>(() => service = new StudentService(null));
            Assert.Equal("Missing StudentRepository", ex.Message);
        }

        [Theory]
        [InlineData(1, "Name", "Address", 1234, "City", "Email")]
        [InlineData(2, "Name", "Address", 1234, "City", null)]
        public void AddStudent_ValidStudent_Test(int id, string name, string address, int zipcode, string city, string email)
        {
            // Arrange
            var fakeRepo = new List<Student>();

            Mock<IStudentRepository> repoMock = new Mock<IStudentRepository>();
            repoMock.Setup(r => r.Add(It.IsAny<Student>())).Callback<Student>(s => fakeRepo.Add(s));

            Student s = new Student(id, name, address, zipcode, city, email);

            var service = new StudentService(repoMock.Object);

            // Act
            service.AddStudent(s);

            // Assert
            Assert.True(fakeRepo.Count == 1);
            Assert.Equal(s, fakeRepo[0]);
            repoMock.Verify(r => r.Add(s), Times.Once);
        }

        [Fact]
        public void AddStudent_StudentIsNull_ExpectArgumentException_Test()
        {
            // Arrange
            Mock<IStudentRepository> repoMock = new Mock<IStudentRepository>();
            var service = new StudentService(repoMock.Object);

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
            Mock<IStudentRepository> repoMock = new Mock<IStudentRepository>();

            var service = new StudentService(repoMock.Object);

            var s = new Student(id, name, address, zipcode, city, email);

            var ex = Assert.Throws<ArgumentException>(() => service.AddStudent(s));
            Assert.Equal(expectedMessage, ex.Message);
            repoMock.Verify(r => r.Add(s), Times.Never);
        }

    }
}
