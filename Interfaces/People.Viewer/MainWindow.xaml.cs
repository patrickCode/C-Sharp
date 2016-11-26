using PersonLibrary;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace People.Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PeopleRepository peopleRepo = new PeopleRepository();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void ConcreteFetchButton_Click(object sender, RoutedEventArgs e)
        {
            ClearListBox();

            List<Person> people;
            people = peopleRepo.GetPeople();

            foreach (var person in people)
                PersonListBox.Items.Add(person);
        }

        private void InterfaceFetchButton_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<Person> people;
            people = peopleRepo.GetPeople();
            
            foreach (var person in people)
                PersonListBox.Items.Add(person);

            //PersonListBox.ItemsSource = people;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearListBox();
        }

        private void ClearListBox()
        {
            PersonListBox.Items.Clear();
        }
    }
}
