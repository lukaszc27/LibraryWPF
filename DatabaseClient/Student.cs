using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DatabaseClient
{
    /// <summary>
    /// Klasa/Model opisująca studenta w bazie danych
    /// </summary>
    public class Student : IEquatable<Student>
    {
        public ulong AlbumNumber { get; set; }
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public Student(string sName, string sSurname, ulong nAlbumNumber = 0, uint nId = 0)
        {
            Name = sName;
            Surname = sSurname;
            AlbumNumber = nAlbumNumber;
            Id = nId;
        }

        public Student()
        {
        }

        public static ObservableCollection<Student> All()
        {
            if (Database.GetInstance() == null)
                Database.Connect();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM Students ");

            using (SqlCommand command = new SqlCommand(sb.ToString(), Database.GetInstance()))
            {
                ObservableCollection<Student> students = new ObservableCollection<Student>();
                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            students.Add(new Student(reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString(),
                                Convert.ToUInt32(reader.GetValue(3)),
                                Convert.ToUInt32(reader.GetValue(0))));
                        }
                        reader.Close();

                        return students;
                    }
                }
                catch (SqlException e)
                {
                    throw e;
                }
            }
        }

        /// <summary>
        /// Tworzenie nowego obiektu w bazie
        /// </summary>
        /// <param name="sName">Imię tworzonego studenta</param>
        /// <param name="sSurname">Nazwisko studenta</param>
        /// <param name="nAlbumNumber">Number albumu</param>
        public static void Create(string sName, string sSurname, ulong nAlbumNumber)
        {
            if (Database.GetInstance() == null)
                Database.Connect();

            using (SqlCommand command = new SqlCommand("INSERT INTO Students (name, surname, albumNumber) VALUES(@name, @surname, @albumNumber)", Database.GetInstance()))
            {
                command.Parameters.Add("@name", SqlDbType.NVarChar);
                command.Parameters.Add("@surname", SqlDbType.NVarChar);
                command.Parameters.Add("@albumNumber", SqlDbType.BigInt);

                command.Parameters["@name"].Value = sName;
                command.Parameters["@surname"].Value = sSurname;
                command.Parameters["@albumNumber"].Value = nAlbumNumber;

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    throw e;
                }

            }
        }
        public static void Create(Student student) => Create(student.Name, student.Surname, student.AlbumNumber);

        /// <summary>
        /// Wyszukiwanie studenta po id 
        /// </summary>
        /// <param name="id">indyfikator studenta w bazie danych</param>
        /// <returns>Student</returns>
        public static Student Find(uint id)
        {
            if (Database.GetInstance() == null)
                Database.Connect();

            string sql = "SELECT TOP 1 * FROM Students WHERE id = @id";
            using (SqlCommand command = new SqlCommand(sql, Database.GetInstance()))
            {
                command.Parameters.Add("@id", SqlDbType.BigInt);
                command.Parameters["@id"].Value = id;

                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Student student = new Student(reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                                Convert.ToUInt32(reader.GetValue(3)), Convert.ToUInt32(reader.GetValue(0)));

                            reader.Close();
                            return student;
                        }
                        else return null;
                    }
                }
                catch (SqlException e)
                {
                    throw e;
                }
            }
        }
        /// <summary>
        /// Wyszukanie określonego studenta po numerze albumu 
        /// </summary>
        /// <param name="albumNumber">Numer studenta nadany przez uczelnię (unikalny)</param>
        /// <returns></returns>
        public static Student FindByAlbumNumber(uint albumNumber)
        {
            if (Database.GetInstance() == null)
                Database.Connect();

            string sql = "SELECT TOP 1 * FROM Students WHERE albumNumber = @albumNumber";
            using (SqlCommand command = new SqlCommand(sql, Database.GetInstance()))
            {
                command.Parameters.Add("@albumNumber", SqlDbType.BigInt);
                command.Parameters["@albumNumber"].Value = albumNumber;

                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Student student = new Student(reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString(),
                                Convert.ToUInt64(reader.GetValue(3)),
                                Convert.ToUInt32(reader.GetValue(0)));

                            reader.Close();
                            return student;
                        }
                    }
                }
                catch(SqlException e)
                {
                    throw e;
                }
            }
            return null;
        }

        public bool Equals(Student other)
        {
            if (other == null)
                return false;

            if ((this.Name == other.Name) && (this.Surname == other.Surname) && (this.AlbumNumber == other.AlbumNumber))
                return true;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Student studentObj = obj as Student;
            if (studentObj == null)
                return false;

            return Equals(studentObj);
        }

        public override int GetHashCode() => this.AlbumNumber.GetHashCode();

        public static bool operator==(Student student1, Student student2)
        {
            if (((object)student1) == null || ((object)student2) == null)
                return object.Equals(student1, student2);

            return student1.Equals(student2);
        }

        public static bool operator!=(Student student1, Student student2)
        {
            if (((object)student1) == null || ((object)student2) == null)
                return !object.Equals(student1, student2);

            return !student1.Equals(student2);
        }
    }
}
