using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    }
}
