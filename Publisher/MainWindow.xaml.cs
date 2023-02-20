using NATS.Client;
using Shared;
using System;
using System.Windows;
using System.Windows.Media;

namespace Publisher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IConnection? _connection = null;

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

        private void BtnPublish_Click(object sender, RoutedEventArgs e)
        {
            if (_connection?.State is not ConnState.CONNECTED)
                return;

            LblPublishFeedback.Visibility = Visibility.Hidden;

            // Configure the message and publish 

            LblPublishFeedback.Content = "Message published";
            LblPublishFeedback.Visibility = Visibility.Visible;
            LblPublishFeedback.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void ConnectionStatusEvent(object? obj, EventArgs args)
            => Dispatcher.Invoke((Action)(() =>
            {
                UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
            }));
    }
}
