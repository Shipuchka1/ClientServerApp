using ClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

namespace TeacherApp.View
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
       public static User LogUser;
       public static List<User> users;
        public LoginPage()
        {
            InitializeComponent();
            string path = "/users?IsTeacher=true";
            RESTApi.ServerUrl = "http://localhost:8082";

           users = JsonConvert.DeserializeObject<List<User>>(RESTApi.Get(path));

            foreach (User item in users)
            {
                ComboBoxItem combo = new ComboBoxItem();
                combo.Content = item.FullName;
                combo.FontSize = 25;
                combo.Tag = item.Id;
                TeacherComboBox.Items.Add(combo);
            }
              
        }

       

        private void LoginTeacherButton_Click(object sender, RoutedEventArgs e)
        {
            if (TeacherComboBox.SelectedIndex != -1)
            {
                int t = (int)((ComboBoxItem)TeacherComboBox.SelectedItem).Tag;
                LogUser = users.FirstOrDefault(f => f.Id == t);
                if (LogUser.Password == LoginPasswordBox.Password)
                    MainWindow.mf.Source = new Uri("View/ProblemPage.xaml", UriKind.RelativeOrAbsolute);
                else MessageBox.Show("Wrong Password");
            }

        }
    }
}
