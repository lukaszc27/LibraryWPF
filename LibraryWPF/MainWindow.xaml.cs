using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DatabaseClient;


namespace LibraryWPF
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Rent rent;
        public MainWindow()
        {
            InitializeComponent();
            GetRents();

            try
            {
                Database myDatabase = new Database();
            }
            catch(SqlException e)
            {
                MessageBox.Show(e.Message, "Connect to Sql Server",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GetRents()
        {
            try
            {
                rent = new Rent();
                listView.ItemsSource = Rent.All();
            }
            catch (SqlException e)
            {
                MessageBox.Show(e.Message, e.TargetSite.ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message, e.TargetSite.ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Interakcja na kliknięcie opcji dodaj książkę w menu książki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBookMenuItem_Click(object sender, RoutedEventArgs e) {
            AddBook addBook = new AddBook();
            if (addBook.ShowDialog() == true)
            {
                try
                {
                    Book.Create(addBook.ISBN, addBook.Title, addBook.Author);
                }
                catch (SqlException exception)
                {
                    MessageBox.Show(exception.Message, "Sql query",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Interakcja na kliknięcie opcji listy książek w menu książki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBookMenuItem_Click(object sender, RoutedEventArgs e) {
            ListBook listBook = new ListBook();
            listBook.ShowDialog();
        }

        /// <summary>
        /// Interakcja na kliknięcie opcji listy studentów w menu Studenci
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StudentListMenuItem_Click(object sender, RoutedEventArgs e) {
            StudentList studentList = new StudentList();
            studentList.ShowDialog();
        }

        private void AddRentButton_Click(object sender, RoutedEventArgs e)
        {
            AddRent addRent = new AddRent();
            if (addRent.ShowDialog() == true)
            {
                Rent.Create(Book.Find(addRent.BookISBN.Text).Id,
                    Student.FindByAlbumNumber(Convert.ToUInt32(addRent.AlbumNumber.Text)).Id,
                    addRent.datePicker.DisplayDate);

                this.GetRents();    // odświerzamy listę z wyporzyczeniami
            }
        }

        /// <summary>
        /// Dodawanie studenta bezpośrednio z menu kontekstowego okna głównego
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddStudentMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddStudent addStudent = new AddStudent();
            if (addStudent.ShowDialog() == true)
            {
                Student.Create(addStudent.Name, addStudent.PersonSurname, addStudent.AlbumNumber);
            }
        }
    }
}
