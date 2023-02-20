using NATS.Client;
using NATS.Client.JetStream;
using Shared;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace StreamPublisher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IConnection? _connection = null;
        private IJetStreamManagement _jetStreamManagement;
        private const string StreamName = "Axxes";

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
            var options = ConnectionFactory.GetDefaultOptions();
            options.AllowReconnect = true;
            options.AddConnectionStatusChangedEventHandler(ConnectionStatusEventHandler);
            options.Url = $"nats://localhost:4222";

            _connection = ConnectionHelper.CreateConnection(options);
            _jetStreamManagement = _connection.CreateJetStreamManagementContext();

            UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
        }

        private void BtnPublish_Click(object sender, RoutedEventArgs e)
        {
            if (_connection?.State is not ConnState.CONNECTED)
                return;

            LblError.Visibility = Visibility.Hidden;

            if (!_jetStreamManagement.GetStreams().Any(x => x.Config.Name.Equals(StreamName)))
            {
                var streamConfiguration = StreamConfiguration.Builder()
                    .WithName(StreamName)
                    .WithStorageType(StorageType.File)
                    .WithSubjects(TxtSubject.Text)
                    .Build();

                _jetStreamManagement.AddStream(streamConfiguration);
            }

            var header = new MsgHeader();
            var message = new Msg(TxtSubject.Text, header, Encoding.UTF8.GetBytes(TxtMessage.Text));

            try
            {
                PublishAck? pa = _connection?.CreateJetStreamContext().Publish(message);
                LblError.Content = "Message published";
                LblError.Visibility = Visibility.Visible;
                LblError.Foreground = new SolidColorBrush(Colors.Green);
            }
            catch (NATSNoRespondersException)
            {
                LblError.Content = "Message not published, no clients are listening on this subject";
                LblError.Visibility = Visibility.Visible;
                LblError.Foreground = new SolidColorBrush(Colors.Red);

            }
            catch (Exception exception)
            {
                LblError.Content = $"Message not published. Exception: {exception.Message}";
                LblError.Visibility = Visibility.Visible;
                LblError.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void ConnectionStatusEventHandler(object? obj, EventArgs args)
            => Dispatcher.Invoke((Action)(() =>
            {
                UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
            }));
    }
}
