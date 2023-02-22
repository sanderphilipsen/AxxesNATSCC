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

            LblMessageFeedback.Visibility = Visibility.Hidden;

            var header = new MsgHeader();
            var message = new Msg(TxtSubject.Text, header, Encoding.UTF8.GetBytes(TxtMessage.Text));

            _connection.Publish(message);

            LblMessageFeedback.Content = "Message published";
            LblMessageFeedback.Visibility = Visibility.Visible;
            LblMessageFeedback.Foreground = new SolidColorBrush(Colors.Green);
        }

        private void Connect()
        {
            var options = ConnectionFactory.GetDefaultOptions();
            options.AddConnectionStatusChangedEventHandler(ConnectionStatusEvent);

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
