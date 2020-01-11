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
    /// Klasa/Model książki przechowywanej w bazie danych
    /// </summary>
    public class Book : IEquatable<Book>
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public uint Id { get; set; }

        public Book(string sISBN, string sTitle, string sAuthor, uint id = 0)
        {
            ISBN = sISBN;
            Title = sTitle;
            Author = sAuthor;
            Id = id;
        }

        public Book()
        {
        }

        /// <summary>
        /// Pobiera wszystkie książki z bazy danych
        /// </summary>
        /// <param name="desc">Sposob sortowania listy (rosnąco lub malejąco)</param>
        /// <returns>Lista książek</returns>
        public static ObservableCollection<Book> All(bool desc = false)
        {
            if (Database.GetInstance() == null)
                Database.Connect();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT id, title, author, isbn FROM Books ");

            if (desc)
                sb.Append("ORDER BY id DESC");
            else sb.Append("ORDER BY id ASC");

            using (SqlCommand command = new SqlCommand(sb.ToString(), Database.GetInstance()))
            {
                try
                {
                    ObservableCollection<Book> books = new ObservableCollection<Book>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books.Add(new Book(reader.GetValue(3).ToString(), reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString(), Convert.ToUInt32(reader.GetValue(0))));
                        }
                        reader.Close();

                        return books;
                    }
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

        /// <summary>
        /// Tworzenie nowego obiektu w bazie
        /// </summary>
        /// <param name="book">Obiekt książki który ma być dodany do bazy</param>
        public static void Create(Book book)
        {
            if (Database.GetInstance() == null)
                Database.Connect();

            using (SqlCommand cmd = new SqlCommand("INSERT INTO Books (title, author, isbn) VALUES(@title, @author, @isbn)", Database.GetInstance()))
            {
                cmd.Parameters.Add("@title", SqlDbType.NVarChar);
                cmd.Parameters.Add("@author", SqlDbType.NVarChar);
                cmd.Parameters.Add("@isbn", SqlDbType.NVarChar);

                cmd.Parameters["@title"].Value = book.Title;
                cmd.Parameters["@author"].Value = book.Author;
                cmd.Parameters["@isbn"].Value = book.ISBN;

                try
                {
                    cmd.ExecuteNonQuery();
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
        public static void Create(string sISBN, string sTitle, string sAuthor) => Create(new Book(sISBN, sTitle, sAuthor));

        /// <summary>
        /// Wyszukiwanie książki po ID
        /// </summary>
        /// <param name="id">indyfikator książki w bazie danych</param>
        /// <returns>Student</returns>
        public static Book Find(uint id)
        {
            if (Database.GetInstance() == null)
                Database.Connect();

            string sql = "SELECT TOP 1 * FROM Books WHERE id = @id";
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
                            Book book = new Book(reader.GetValue(3).ToString(), reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString(), Convert.ToUInt32(reader.GetValue(0)));

                            reader.Close();
                            return book;
                        }
                        else return null;
                    }
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
        public static Book Find(string ISBN)
        {
            ISBN.Trim();    // remove white characters from ISBN

            if (Database.GetInstance() == null)
                Database.Connect();

            string sql = "SELECT TOP 1 * FROM Books WHERE isbn=@isbn";
            using (SqlCommand command = new SqlCommand(sql, Database.GetInstance()))
            {
                command.Parameters.Add("@isbn", SqlDbType.NVarChar);
                command.Parameters["@isbn"].Value = ISBN;

                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Book book = new Book(reader.GetValue(3).ToString(), reader.GetValue(1).ToString(),
                                reader.GetValue(2).ToString(), Convert.ToUInt32(reader.GetValue(0)));

                            reader.Close();
                            return book;
                        }
                    }
                }
                catch(SqlException e)
                {
                    throw e;
                }
                catch (InvalidOperationException e)
                {
                    throw e;
                }
            }
            return null;
        }

        /// <summary>
        /// Oznacza zwrot książki w bazie danych
        /// </summary>
        /// <returns></returns>
        public bool Rent()
        {
            if (Database.GetInstance() == null)
                Database.Connect();

            string sql = "UPDATE [Rents] SET [EndDate]=@endDate WHERE [bookId] = @bookId";
            using (SqlCommand command = new SqlCommand(sql, Database.GetInstance()))
            {
                command.Parameters.AddWithValue("@endDate", DateTime.Now);
                command.Parameters.AddWithValue("@bookId", this.Id);

                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (SqlException ex)
                {
                    return false;
                }
            }
        }

        public bool Equals(Book other)
        {
            if (other == null)
                return false;

            if ((this.ISBN == other.ISBN) && (this.Title == other.Title) && (this.Author == other.Author))
                return true;

            return false;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Book bookObj = obj as Book;
            if (bookObj == null)
                return false;

            return Equals(bookObj);
        }
        public override int GetHashCode() => this.ISBN.GetHashCode();
        public static bool operator== (Book book1, Book book2)
        {
            if (((object)book1) == null || ((object)book2) == null)
                return object.Equals(book1, book2);

            return book1.Equals(book2);
        }
        public static bool operator!= (Book book1, Book book2)
        {
            if (((object)book1) == null || ((object)book2) == null)
                return !object.Equals(book1, book2);

            return !book1.Equals(book2);
        }
    }
}
