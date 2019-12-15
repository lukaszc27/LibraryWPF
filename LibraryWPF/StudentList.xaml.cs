using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DatabaseClient;

namespace LibraryWPF
{
    /// <summary>
    /// Logika interakcji dla klasy StudentList.xaml
    /// </summary>
    public partial class StudentList : Window
    {
        private static Student student;
        private bool selectedMode;
        public Student SelectedItem { get; set; }

        public StudentList(bool selectedMode = false)
        {
            InitializeComponent();
            SelectedItem = null;
            this.selectedMode = selectedMode;

            GetStudents();
        }

        /// <summary>
        /// Pobiera studentów z bazy oraz umieszcza ich w widoku
        /// </summary>
        private void GetStudents()
        {
            try
            {
                student = new Student();
                listView.ItemsSource = Student.All();
            }
            catch (SqlException e)
            {
                MessageBox.Show(e.Message, "Sql syntax error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddStudentButton_Click(object sender, RoutedEventArgs e)
        {
            AddStudent addStudent = new AddStudent();
            if (addStudent.ShowDialog() == true)
            {
                Student.Create(addStudent.PersonName, addStudent.PersonSurname, addStudent.AlbumNumber);
                GetStudents();
            }
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listView.SelectedItems.Count == 1 && this.selectedMode)
            {
                SelectedItem = (Student)listView.SelectedItems[0];
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Nie zaznaczono elementów lub zaznaczono za dużo elementów!" +
                    "(więcej niż jeden element)", "ListView",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
