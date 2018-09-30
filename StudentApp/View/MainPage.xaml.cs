using ClassLibrary;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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


namespace StudentApp.View
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
       public static StudentFile FileToSend;
       public static string ServerUrl = "http://localhost:8082/getfilestudent";
        public MainPage()
        {
            InitializeComponent();
            FileToSend = new StudentFile();
            RESTApi.ServerUrl = "http://localhost:8082";
            List<State> states;
            List<StudentFile> problemStates = new List<StudentFile>();
            string path = "/states?UserId=" + LoginPage.LogUser.Id;
            states = JsonConvert.DeserializeObject<List<State>>(RESTApi.Get(path));


            foreach (State state in states)
            {
                StudentFile ps = new StudentFile();
                path = "/problems?Id=" + state.ProblemId;
                ps.problem = JsonConvert.DeserializeObject<List<Problem>>(RESTApi.Get(path))[0];
                ps.state = state;
                ps.user = LoginPage.LogUser;

                problemStates.Add(ps);
            }

            ProblesListView.ItemsSource = problemStates;
        }

        private void GetFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProblesListView.SelectedIndex != -1)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    FileNameLabel.Content = openFileDialog.FileName;
                    FileToSend.FileName = openFileDialog.SafeFileName;
                    FileToSend.FileInBytes = File.ReadAllBytes(openFileDialog.FileName);
                    FileToSend.user = ((StudentFile)ProblesListView.SelectedItem).user;
                    FileToSend.problem = ((StudentFile)ProblesListView.SelectedItem).problem;
                    FileToSend.state = ((StudentFile)ProblesListView.SelectedItem).state;
                }
            }
            else
            {
                MessageBox.Show("Click to problem");
            }
                
        }

        private void SendFileButton_Click(object sender, RoutedEventArgs e)
        {
            if(ProblesListView.SelectedIndex!=-1)
            SendFileAsync();
            else
            {
                MessageBox.Show("Click to problem");
            }
        }
        public static async void SendFileAsync()
        {
            string t = JsonConvert.SerializeObject(MainPage.FileToSend);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    MainPage.ServerUrl,
                     new StringContent(t, Encoding.UTF8, "application/json"));
            }
        }

        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mf.Source = new Uri("View/ChatPage.xaml", UriKind.RelativeOrAbsolute);
        }

        private void EmailButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mf.Source = new Uri("View/EmailPage.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
