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
    /// Interaction logic for MailMessagePage.xaml
    /// </summary>
    public partial class MailMessagePage : Page
    {
        public MailMessagePage()
        {
            InitializeComponent();
            SubjectLabel.Content = EmailPage.temp.Subject;
            BodyWebBrowser.NavigateToString(EmailPage.temp.Body);

            FromLabel.Content = EmailPage.temp.From;
            DateLabel.Content = EmailPage.temp.Date;
        }
    }
}
