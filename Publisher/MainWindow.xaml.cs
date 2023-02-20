using NATS.Client;
using Shared;
using System;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Publisher
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

        private void BtnPublish_Click(object sender, RoutedEventArgs e)
        {
            if (_connection?.State is not ConnState.CONNECTED)
                return;

            LblError.Visibility = Visibility.Hidden;

            var header = new MsgHeader();
            var message = new Msg(TxtSubject.Text, header, Encoding.UTF8.GetBytes(TxtMessage.Text));

            _connection.Publish(message);

            LblError.Content = "Message published";
            LblError.Visibility = Visibility.Visible;
            LblError.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void Connect()
        {
            var options = ConnectionFactory.GetDefaultOptions();
            options.AllowReconnect = true;
            options.AddConnectionStatusChangedEventHandler(ConnectionStatusEvent);
            options.Url = $"nats://localhost:4222";

            _connection = ConnectionHelper.CreateConnection(options);

            UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
        }

        private void ConnectionStatusEvent(object? obj, EventArgs args)
            => Dispatcher.Invoke((Action)(() =>
            {
                UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
            }));
    }
}
