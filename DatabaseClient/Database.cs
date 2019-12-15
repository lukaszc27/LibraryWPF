using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DatabaseClient
{
    /// <summary>
    /// Główna klasa stanowiąca interfejs pomiędzy bazą danych SQLite a aplikacją
    /// </summary>
    public class Database
    {
        private static SqlConnection connection;
        private static bool connectionIsOpen = false;

        public bool IsOpen
        {
            get
            {
                return connectionIsOpen;
            }
        }

        public Database()
        {
            Connect();
        }

        //~Database()
        //{
        //    connection.Close();
        //}

        /// <summary>
        /// Nawiązuje połączenie z serwerem SQL
        /// </summary>
        public static void Connect()
        {
            string dbName = "LibraryWPF";
            string instance = "LUKASZ-KOMPUTER\\SQLEXPRESS";

            string connectionString = $"Data Source={instance};" +
                "Trusted_Connection=Yes;" +
                $"database={dbName};" +
                "connection timeout=3";

            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                connectionIsOpen = true;
            }
            catch (SqlException e)
            {
                connectionIsOpen = false;
                throw e;
            }
        }

        /// <summary>
        /// Zwraca instancję aktualnego połączenia SQL
        /// </summary>
        /// <returns>SqlConnection</returns>
        public static SqlConnection GetInstance() => connection;
    }
}
