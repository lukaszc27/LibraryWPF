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
    public class Rent
    {
        private uint BookId { get; set; }
        private uint StudentId { get; set; }

        public DateTime RentDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public uint Id { get; set; }
        public Book Book
        {
            get
            {
                return Book.Find(BookId);
            }
        }
        public Student Student
        {
            get
            {
                return Student.Find(StudentId);
            }
        }


        public Rent(uint bookId, uint studentId, DateTime rentDate, DateTime returnDate, uint id = 0)
        {
            this.BookId = bookId;
            this.StudentId = studentId;
            this.RentDate = rentDate;
            this.ReturnDate = returnDate;
            this.Id = id;
        }

        public Rent()
        {
        }

        /// <summary>
        /// Tworzy nowy obiekt w bazie danych
        /// </summary>
        /// <param name="book">Książka do wyporzyczenia</param>
        /// <param name="student">Student który wyporzycza</param>
        /// <param name="rentDate">Data wyporzyczenia</param>
        /// <returns></returns>
        public static void Create(uint bookId, uint studentId, DateTime rentDate)
        {
            if (Database.GetInstance() == null)
                Database.Connect();

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO Rents (startDate, endDate, bookId, studentId) ");
            sb.Append("VALUES(@startDate, @endDate, @bookId, @studentId)");

            using (SqlCommand command = new SqlCommand(sb.ToString(), Database.GetInstance()))
            {
                command.Parameters.Add("@startDate", SqlDbType.Date);
                command.Parameters.Add("@endDate", SqlDbType.Date);
                command.Parameters.Add("@bookId", SqlDbType.BigInt);
                command.Parameters.Add("@studentId", SqlDbType.BigInt);

                command.Parameters["@startDate"].Value = rentDate.Date;
                command.Parameters["@endDate"].Value = rentDate.AddDays(60).Date;   // do zmiany (tu będzie bug związany ze zwrotami)
                command.Parameters["@bookId"].Value = bookId;
                command.Parameters["@studentId"].Value = studentId;

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                    throw e;
                }
                catch (InvalidOperationException e)
                {
                    throw e;
                }
            }
        }

        public static ObservableCollection<Rent> All(bool desc = false)
        {
            if (Database.GetInstance() == null)
                Database.Connect();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT startDate, endDate, bookId, studentId, id FROM Rents ");
            if (desc)
                sb.Append("ORDER BY id DESC");
            else sb.Append("ORDER BY id ASC");

            using (SqlCommand command = new SqlCommand(sb.ToString(), Database.GetInstance()))
            {
                try
                {
                    ObservableCollection<Rent> rents = new ObservableCollection<Rent>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rents.Add(new Rent(Convert.ToUInt32(reader.GetValue(2)),
                                Convert.ToUInt32(reader.GetValue(3)),
                                Convert.ToDateTime(reader.GetValue(0)),
                                Convert.ToDateTime(reader.GetValue(1)),
                                Convert.ToUInt32(reader.GetValue(4))));
                        }
                        reader.Close();
                    }
                    return rents;
                }
                catch (SqlException e)
                {
                    throw e;
                }
            }
        }
    }
}
