using AE.Net.Mail;
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
    /// Interaction logic for EmailPage.xaml
    /// </summary>
    public partial class EmailPage : Page
    {
        public static ImapClient ic;

        public static MailMessage temp;
        public EmailPage()
        {
            InitializeComponent();
            
           
           // ic.Dispose();
        }


       

        private void EmailListView_SelectionChanged(object sender, MouseButtonEventArgs e)
        {
            
            temp = (MailMessage)EmailListView.SelectedItem;
            temp = ic.GetMessage(temp.Uid);

            MainWindow.mf.Source = new Uri("View/MailMessagePage.xaml", UriKind.RelativeOrAbsolute);
        }

        private void PasswordEmailButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ic = new ImapClient("imap.gmail.com", "natalyaplestsova@gmail.com", PasswordrEmailPB.Password,
                               AuthMethods.Login, 993, true);
                

                ic.SelectMailbox("INBOX");
                

                MailMessage[] mm = ic.GetMessages(0,ic.GetMessageCount(), true, false);
                List<MailMessage> mms;
                mms = mm.Reverse().ToList();
                mms.RemoveAll(r => r.Encoding != Encoding.UTF8);
                EmailListView.ItemsSource = mms;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
