﻿using System;
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
using System.Windows.Shapes;
using DatabaseClient;

namespace LibraryWPF
{
    /// <summary>
    /// Interaction logic for AddRent.xaml
    /// </summary>
    public partial class AddRent : Window
    {
        public AddRent(Book book = null)
        {
            InitializeComponent();

            datePicker.DisplayDate = DateTime.Now;

            if (book == null)
            {
                // wybrana książka z listy w celu aktualizacji pokazuje przycisk umożliwiający zwrot
                ReturnButton.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Przycisk anulowania dialogu (dodawania wyporzyczenia książki)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e) => this.DialogResult = false;

        /// <summary>
        /// Wybranie studenta z listy w celu wyporzyczenia książki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StudentChooseButton_Click(object sender, RoutedEventArgs e)
        {
            StudentList studentList = new StudentList(true);
            if (studentList.ShowDialog() == true)
            {
                Name.Text = studentList.SelectedItem.Name;
                Lastname.Text = studentList.SelectedItem.Surname;
                AlbumNumber.Text = Convert.ToString(studentList.SelectedItem.AlbumNumber);
            }
        }

        /// <summary>
        /// Wybranie ksiązki z listy do wyporzyczenia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BookChooseButton_Click(object sender, RoutedEventArgs e)
        {
            ListBook bookList = new ListBook(true);
            if (bookList.ShowDialog() == true)
            {
                BookTitle.Text = bookList.SelectedBook.Title;
                BookAuthor.Text = bookList.SelectedBook.Author;
                BookISBN.Text = bookList.SelectedBook.ISBN;
            }
        }

        private void AcceptButtonClick(object sender, RoutedEventArgs e) => this.DialogResult = true;
        private void RejectButtonClick(object sender, RoutedEventArgs e) => this.DialogResult = false;

        private void ReturnBookButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hello World!");
        }
    }
}
