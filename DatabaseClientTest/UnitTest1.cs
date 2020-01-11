using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseClient;
using System.Data.SqlClient;
namespace DatabaseClientTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void DatabaseConnectionTest()
        {
            Database database;
            try
            {
                database = new Database();

                // testy na studencie
                Student.Create("Test", "Student", 999);

                Assert.IsTrue(Student.All().Count > 0);
                Assert.IsNotNull(Student.FindByAlbumNumber(999));

                // testy na ksi¹¿kach
                Book.Create("1234-567-890-1", "Test Book", "UnitTest");

                Assert.IsTrue(Book.All().Count > 0);
                Assert.IsNotNull(Book.Find("1234-567-890-1"));
            }
            catch (SqlException e)
            {
            }
        }
    }
}
