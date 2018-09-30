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
    /// Interaction logic for StatesPage.xaml
    /// </summary>
    public partial class StatesPage : Page
    {
        public StatesPage()
        {
            InitializeComponent();
            int ProblemId = ((Problem)ProblemPage.lv.SelectedItem).Id;

            List<User> users = new List<User>();
            string path = "/users";
            users = JsonConvert.DeserializeObject<List<User>>(RESTApi.Get(path));

            List<State> states = new List<State>();
            path = "/states?ProblemId="+ProblemId;
            states = JsonConvert.DeserializeObject<List<State>>(RESTApi.Get(path));

            List<Group> groups = new List<Group>();
            path = "/groups";
            groups = JsonConvert.DeserializeObject<List<Group>>(RESTApi.Get(path));

            List<UserState> userstates = new List<UserState>();

            foreach (State item in states)
            {
                
                UserState tmp = new UserState();
                tmp.StateStatus = item.Status;
                tmp.UserGroupId = users.FirstOrDefault(x => x.Id == item.UserId).GroupId;
                tmp.UserName = users.FirstOrDefault(x => x.Id == item.UserId).FullName;
                
                tmp.GroupName = groups.FirstOrDefault(x => x.Id == tmp.UserGroupId).Name;

                userstates.Add(tmp);
            }
            
            UserStatesLV.ItemsSource = userstates;


        }

       
    }

    public class UserState
    {
        public string UserName { get; set; }
        public int UserGroupId { get; set; }
        public string StateStatus { get; set; }
        public string GroupName { get; set; }
    }
}
