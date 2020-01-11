using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseClient;
using System.Data.SqlClient;

namespace UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DatabaseConnectionTest()
        {
            Database database = new Database();

            {
                

                // testy na studencie
                Student.Create("Test", "Student", 999);

                Assert.IsTrue(Student.All().Count > 0);
                Assert.IsNotNull(Student.FindByAlbumNumber(999));

                // testy na książkach
                Book.Create("1234-567-890-1", "Test Book", "UnitTest");

                Assert.IsTrue(Book.All().Count > 0);
                Assert.IsNotNull(Book.Find("1234-567-890-1"));
            }
            
        }
        [TestMethod]
        public void StudentsTwoObjectsSame()
        {
            Student student1 = new Student("Jan", "Nowak");
            Student student2 = new Student("Jan", "Nowak");

            Assert.AreEqual(student1, student2);
        }

        [TestMethod]
        public void StudentsTwoObjectsDifferent()
        {
            Student student1 = new Student("Jan", "Nowak");
            Student student2 = new Student("Janusz", "Nosacz");

            Assert.AreNotEqual(student1, student2);
        }

        [TestMethod]
        public void StudentsOperatorAssign()
        {
            Student student1 = new Student("Janusz", "Nosacz");
            Student student2 = student1;

            Assert.AreEqual(student1, student2);
        }
    }
}
