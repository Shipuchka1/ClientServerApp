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

namespace TeacherApp.View
{
    /// <summary>
    /// Interaction logic for ProblemPage.xaml
    /// </summary>
    public partial class ProblemPage : Page
    {
        public static ComboBox cb;
        public static ListView lv;
        public static TextBox InTB;
        public static TextBox OutTB;
        public ProblemPage()
        {
            InitializeComponent();
            cb = GroupsCB;
            lv = ProblemsListView;
            InTB = InputArgsTB;
           
            RefreshPage();

        }

        private void AddProblemButton_Click(object sender, RoutedEventArgs e)
        {
            Problem problem = new Problem();
            problem.Subject = SubjectTB.Text;
            problem.TeacherId = LoginPage.LogUser.Id;
            problem.InputArgs = InputArgsTB.Text;
            
            problem.Description = DescriptionTB.Text;

            List<Problem> problems = new List<Problem>();
            List<User> users = new List<User>();
            problems.Add(problem);

            string path = "/problems";
            problems = JsonConvert.DeserializeObject<List<Problem>>(RESTApi.Post<Problem>(path, problems));
            path = "/users?GroupId="+((ComboBoxItem)GroupsCB.SelectedItem).Tag;
            users = JsonConvert.DeserializeObject<List<User>>(RESTApi.Get(path));
            
            List<State> states = new List<State>();
            foreach (User item in users)
            {
                State state = new State();
                state.ProblemId = problems[0].Id;
                state.UserId = item.Id;
                state.Status = "Undone";
                states.Add(state);
            }
            path = "/states";
            RESTApi.Post<State>(path, states);
            RefreshPage();
        }

        public static void RefreshPage()
        {
           
            List<Group> groups;
            List<Problem> problems;


            RESTApi.ServerUrl = "http://localhost:8082";
            string path = "/groups";



            groups = JsonConvert.DeserializeObject<List<Group>>(RESTApi.Get(path));

            cb.Items.Clear();
            foreach (Group item in groups)
            {
                ComboBoxItem combo = new ComboBoxItem();
                combo.Content = item.Name;
                combo.Tag = item.Id;
                cb.Items.Add(combo);
            }
          //  lv.Items.Clear();

            path = "/problems?TeacherId=" + LoginPage.LogUser.Id;
            problems = JsonConvert.DeserializeObject<List<Problem>>(RESTApi.Get(path));
            lv.ItemsSource = problems;

            List<Arguments> args = new List<Arguments>();
            Arguments arg = new Arguments();
            arg.className = "MyNamespace.MyClass";
            arg.methodName = "MyMethod";
            
            Argument argument = new Argument();
            argument.type = typeof(int).FullName;
            argument.value = (int)6;

            arg.args.Add(argument);
            argument = new Argument();
            argument.type = typeof(string).FullName;
            argument.value = (string)"hello";
            arg.args.Add(argument);
            argument = new Argument();
            argument.type = typeof(string).FullName;
            argument.value = (string)"hi";
            arg.result = argument;


            args.Add(arg);

            InTB.Text = JsonConvert.SerializeObject(args);
            

        }

        private void StatesContextMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mf.Source = new Uri("View/StatesPage.xaml", UriKind.RelativeOrAbsolute);
        }

        private void ChatButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mf.Source = new Uri("View/ChatPage.xaml", UriKind.RelativeOrAbsolute);
        }
    }
}
