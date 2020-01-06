using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using Microsoft.Win32;
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
    /// Logika interakcji dla klasy ListBook.xaml
    /// </summary>
    public partial class ListBook : Window
    {
        private static Book books;
        private bool selecedMode;

        public Book SelectedBook { get; set; }

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="selectedMode">Czy okno obsługuje możliwość wybrania z listy</param>
        public ListBook(bool selectedMode = false)
        {
            InitializeComponent();
            this.selecedMode = selectedMode;

            GetBooks();
        }

        private void GetBooks()
        {
            try
            {
                books = new Book();
                ListBookView.ItemsSource = Book.All();
            }
            catch (InvalidOperationException e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Dodawnie nowej książki do bazy z danych pobranych przez dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            AddBook addBook = new AddBook();
            if (addBook.ShowDialog() == true) {
                Book.Create(addBook.ISBN, addBook.Title, addBook.Author);
            }
        }

        /// <summary>
        /// Zwracanie zaznaczonego elementu po podwójnym kliknięciu na liście
        /// obsługiwane tylko w momencie otworzenia dialogu w trybie do wyboru
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBookView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBookView.SelectedItems.Count == 1 && selecedMode)
            {
                SelectedBook = (Book)ListBookView.SelectedItems[0];
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Zaznaczono za dużo elementów! Lub nie zaznaczono ich wogóle!",
                    "Book View", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            saveFileDialog.Title = "Eksport książek do pliku CSV";
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.Filter = "Plik CSV|*.csv";

            if (saveFileDialog.ShowDialog() == true)
            {
                uint counter = 0;   // licznik wyeksportowanych książek
                ObservableCollection<Book> books = Book.All();

                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.CreateNew))
                {
                    using (BinaryWriter bw = new BinaryWriter(fileStream))
                    {
                        foreach (Book book in books)
                        {
                            string line = $"{book.Title};{book.Author};{book.ISBN}\r\n";
                            bw.Write(line);
                            counter++;
                        }
                    }
                }

                MessageBox.Show($"Wyeksportowano {counter}/{books.Count} książek.\r\nEksport zakończony.",
                "Eksport do pliku CSV", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Importowanie książek z pliku CSV
        /// Format odczytywanych danych: TYTUŁ;AUTOR;ISBN
        /// Funkcja sprawdza czy książka o wczytywanym ISBN istnieje w bazie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        List<Book> books = new List<Book>();
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

                                if (Book.Find(param[2]) == null)
                                {
                                    books.Add(new Book(param[2], param[0], param[1]));
                                    counter++;
                                }
                            }
                            catch (EndOfStreamException)
                            {
                                // kiedy zakończy się odczytywanie pliku występuje wyjątek EndOfStreamException
                                // który przechwytujemy i zapisujemy wczytane dane do bazy SQL
                                foreach (Book book in books)
                                    Book.Create(book);

                                break;  // wychodzimy poza pętlę
                            }

                        } while (line != string.Empty);

                        MessageBox.Show($"Zaimportowano {counter}/{readCounter}\r\nImport książek zakończony.",
                            "Import książek z pliku CSV", MessageBoxButton.OK, MessageBoxImage.Information);

                        GetBooks(); // pobranie książek z bazy (w celu aktualizacji listy)

                        if (readCounter != counter)
                        {
                            MessageBox.Show("Nie wszystkie książki zostaly zaimportowane!",
                                "Ostrzeżenie", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                }
            }
        }
    }
}
