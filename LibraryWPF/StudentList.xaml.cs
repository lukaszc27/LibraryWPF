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
using Microsoft.Win32;
using System.IO;

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

        /// <summary>
        /// Eksportowanie książek z bazy do pliku pliku tekstowego
        /// Sposób zapisu danych do pliku: TYTUŁ;AUTOR;ISBN
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Eksport studentów do pliku CSV";
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "Plik CSV|*.csv";

            if (saveFileDialog.ShowDialog() == true)
            {
                uint counter = 0;   // licznik wyeksportowanych studentów
                ObservableCollection<Student> students = Student.All();

                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.CreateNew))
                {
                    using (BinaryWriter bw = new BinaryWriter(fileStream))
                    {
                        foreach (Student student in students)
                        {
                            string line = $"{student.Name};{student.Surname};{student.AlbumNumber}\r\n";
                            bw.Write(line);
                            counter++;
                        }
                    }
                }

                MessageBox.Show($"Wyeksportowano {counter}/{students.Count} studentów.\r\nEksport zakończony.",
                "Eksport do pliku CSV", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofn = new OpenFileDialog();
            ofn.Filter = "Plik CSV|*.csv";
            ofn.DefaultExt = "*.csv";
            ofn.Title = "Import danych z pliku CSV";

            if (ofn.ShowDialog() == true)
            {
                using (FileStream fs = new FileStream(ofn.FileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        List<Student> students = new List<Student>();
                        string line;
                        uint counter = 0;
                        uint readCounter = 0;

                        do
                        {
                            line = string.Empty;
                            try
                            {
                                line = br.ReadString();
                                readCounter++;

                                line.Trim();
                                string[] param = line.Split(';');
                                for (uint i = 0; i < param.Length; i++)
                                    param[i].Trim();

                                if (Student.Find(Convert.ToUInt32(param[2])) == null)
                                {
                                    students.Add(new Student(param[0], param[1], Convert.ToUInt64(param[2])));
                                    counter++;
                                }
                            }
                            catch (EndOfStreamException)
                            {
                                // kiedy zakończy się odczytywanie pliku występuje wyjątek EndOfStreamException
                                // który przechwytujemy i zapisujemy wczytane dane do bazy SQL
                                foreach (Student student in students)
                                    Student.Create(student);

                                break;  // wychodzimy poza pętlę
                            }

                        } while (line != string.Empty);

                        MessageBox.Show($"Zaimportowano {counter}/{readCounter}\r\nImport studentów zakończony.",
                            "Import studentów z pliku CSV", MessageBoxButton.OK, MessageBoxImage.Information);

                        GetStudents(); // pobranie studentów z bazy (w celu aktualizacji listy)

                        if (readCounter != counter)
                        {
                            MessageBox.Show("Nie wszyscy studenci zostali zaimportowanei!",
                                "Ostrzeżenie", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
        }
    }
}
