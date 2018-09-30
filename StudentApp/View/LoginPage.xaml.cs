using ClassLibrary;
using Newtonsoft.Json;
using System;
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

namespace StudentApp.View
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public static User LogUser;
        public LoginPage()
        {
            InitializeComponent();
            
            RESTApi.ServerUrl = "http://localhost:8082";
            string path = "/groups";
            List<Group> groups;
            groups = JsonConvert.DeserializeObject<List<Group>>(RESTApi.Get(path));

            foreach (Group item in groups)
            {
                ComboBoxItem combo = new ComboBoxItem();
                combo.Content = item.Name;
                combo.Tag = item.Id;

                GroupComboBox.Items.Add(combo);
            }
        }

        private void GroupComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            StudentComboBox.Items.Clear();
            if(GroupComboBox.SelectedIndex!=-1)
            {
                string path = "/users?GroupId="+((ComboBoxItem)GroupComboBox.SelectedItem).Tag;
                List<User> users;
                users = JsonConvert.DeserializeObject<List<User>>(RESTApi.Get(path));

                foreach (User item in users)
                {
                    ComboBoxItem combo = new ComboBoxItem();
                    combo.Content = item.FullName;
                    combo.Tag = item.Id;

                    StudentComboBox.Items.Add(combo);
                }

            }

        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (StudentComboBox.SelectedIndex != -1)
            {
                string path = "/users?Id=" + ((ComboBoxItem)StudentComboBox.SelectedItem).Tag;
                List<User> users;
                users = JsonConvert.DeserializeObject<List<User>>(RESTApi.Get(path));
               
                if (users[0].Password == PasswordPB.Password)
                {
                    LogUser = users[0];

                    MainWindow.mf.Source = new Uri("View/MainPage.xaml", UriKind.RelativeOrAbsolute);
                }
                else
                    MessageBox.Show("Wrong password");
            }
        }
    }
}
