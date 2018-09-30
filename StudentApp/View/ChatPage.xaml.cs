using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
    /// Interaction logic for ChatPage.xaml
    /// </summary>
    public partial class ChatPage : Page
    {
        bool alive = false; 
     
        Udp client;
        const int LOCALPORT = 8001; 
        const int REMOTEPORT = 8001; 
        const int TTL = 20;
       
        const string HOST = "255.255.255.255"; 
        IPAddress groupAddress; 

        string userName; 
        public ChatPage()
        {
            InitializeComponent();
            loginButton.IsEnabled = true; 
            logoutButton.IsEnabled = false; 
            sendButton.IsEnabled = false; 
            chatTextBox.IsReadOnly = true; 

            groupAddress = IPAddress.Parse(HOST);

            
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            userName = userNameTextBox.Text;
            userNameTextBox.IsReadOnly = true;

            try
            {
                

                client = new Udp(HOST, LOCALPORT);

                
                Task receiveTask = new Task(ReceiveMessages);
                receiveTask.Start();

                
                string message = userName + " вошел в чат";
                byte[] data = Encoding.Unicode.GetBytes(message);
              
                client.Send(data);

                loginButton.IsEnabled = false;
                logoutButton.IsEnabled = true;
                sendButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            ExitChat();
        }

        private void sendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = String.Format("{0}: {1}", userName, messageTextBox.Text);
                byte[] data = Encoding.Unicode.GetBytes(message);
                
                client.Send(data);
                messageTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ReceiveMessages()
        {
            alive = true;
            try
            {
                while (alive)
                {
                    
                    byte[] data = client.Receive();
                    string message = Encoding.Unicode.GetString(data);
                    

                    
                    chatTextBox.Dispatcher.Invoke(() =>
                    {
                        string time = DateTime.Now.ToShortTimeString();
                        chatTextBox.Text = time + " " + message + "\r\n" + chatTextBox.Text;
                    });
                }
            }
            catch (ObjectDisposedException)
            {
                if (!alive)
                    return;
                throw;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExitChat()
        {
            string message = userName + " покидает чат";
            byte[] data = Encoding.Unicode.GetBytes(message);
            
            client.Send(data);


            alive = false;
            

            loginButton.IsEnabled = true;
            logoutButton.IsEnabled = false;
            sendButton.IsEnabled = false;
            userNameTextBox.IsReadOnly = false;
        }
    }

    public class Udp
    {
        private readonly UdpClient _sender;
        private readonly UdpClient _listener;

        public Udp(string address, int port)
        {
            _sender = new UdpClient(address, port);
            _listener = new UdpClient();

            _listener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _listener.Client.Bind(new IPEndPoint(IPAddress.Any, port));
        }

        public byte[] Receive()
        {
            var _ = null as IPEndPoint;
            
            return _listener.Receive(ref _);
        }

        public void Send(byte[] message)
        {
            var dataAsBytes = message;
            _sender.Send(dataAsBytes, dataAsBytes.Length);
        }
    }
}
