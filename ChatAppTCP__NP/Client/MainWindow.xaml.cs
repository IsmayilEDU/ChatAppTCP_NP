using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private List<MessageHistory> messageHistories;

        public List<MessageHistory> MessageHistories
        { get => messageHistories;
            set
            {
                messageHistories = value;
                OnNotifyPropertyChanged(nameof(MessageHistories));
            }
        }
        public string UserName { get; init; }
        public MainWindow(string userName)
        {
            InitializeComponent();
            UserName = userName;
            DataContext = this;
            ConnectToServer();
        }

        #region Property changed

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnNotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void btn_Send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ip = IPAddress.Parse("127.0.0.1");
                var port = 27001;

                var user = new TcpClient(ip.ToString(), port);

                var listenerStream = user.GetStream();

                var binaryWriter = new BinaryWriter(listenerStream);
                var SentString = UserName + '/' + textbox_NameOfUSer.Text + ':' + textbox_Message.Text;

                binaryWriter.Write(SentString);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ConnectToServer()
        {
            var ip = IPAddress.Parse("127.0.0.1");
            var port = 27001;

            var user = new TcpClient(ip.ToString(), port);
            var listenerStream = user.GetStream();
            var binaryWriter = new BinaryWriter(listenerStream);

            binaryWriter.Write(UserName);


            var binaryReader = new BinaryReader(listenerStream);

            Task.Run(() =>
            {
                while (true)
                {
                    //  Received message from server
                    string messageString = binaryReader.ReadString();

                    //  Seperate string to username and message
                    int index = messageString.IndexOf(':');

                    var messageHistory = new MessageHistory();
                    messageHistory.ReceivedClientName = messageString.Substring(0, index);
                    messageHistory.Message = messageString.Substring(0, index);
                    MessageBox.Show(messageHistory.ReceivedClientName + " - " + messageHistory.Message);
                    MessageHistories.Add(messageHistory);
                }
            });


        }
    }
}
