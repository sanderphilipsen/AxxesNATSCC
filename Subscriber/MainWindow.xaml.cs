using NATS.Client;
using Shared;
using System;
using System.Windows;

namespace Subscriber
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IConnection? _connection = null;

        public MainWindow() => InitializeComponent();

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (BtnConnect.Content is "Disconnect")
                _connection?.Close();
            else
                Connect();
        }

        private void Connect()
        {
            //Configure connection options

            //Create the connection

            UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
        }

        private void BtnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            // Subscribe to the entered subject
            IAsyncSubscription? subscription = null;

            LstSubscriptions.DisplayMemberPath = "Subject";
            LstSubscriptions.SelectedValuePath = "Subject";
            LstSubscriptions.Items.Add(subscription);
        }

        private void Unsubscribe_Click(object sender, RoutedEventArgs e)
        {
            //  Unsubscribe   
        }

        //private EventHandler<MsgHandlerEventArgs> GetMessageHandler()
        //{
        //  Return a eventHandler and add incoming messages to LstMessages listbox 
        //}

        private void ConnectionStatusEventHandler(object? obj, EventArgs args)
            => Dispatcher.Invoke((Action)(() =>
            {
                UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
            }));
    }
}
