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
            options.Url = $"nats://localhost:4221";

            _connection = ConnectionHelper.CreateConnection(options);
            _jetStreamManagement = _connection.CreateJetStreamManagementContext();

            UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
        }

        private void BtnPublish_Click(object sender, RoutedEventArgs e)
        {
            if (_connection?.State is not ConnState.CONNECTED)
                return;

            LblMessageFeedback.Visibility = Visibility.Hidden;

            var stream = _jetStreamManagement?.GetStreams().FirstOrDefault(x => x.Config.Name == StreamName)?.Config;
            if (stream is null)
            {
                stream = StreamConfiguration.Builder()
                    .WithName(StreamName)
                    .WithStorageType(StorageType.File)
                    .Build();

                _jetStreamManagement?.AddStream(stream);
            }

            if (!stream.Subjects.Any(x => x == TxtSubject.Text))
                stream.Subjects.Add(TxtSubject.Text);

            var header = new MsgHeader();
            var message = new Msg(TxtSubject.Text, header, Encoding.UTF8.GetBytes(TxtMessage.Text));

            try
            {
                PublishAck? pa = _connection?.CreateJetStreamContext().Publish(message);
                LblMessageFeedback.Content = "Message published";
                LblMessageFeedback.Visibility = Visibility.Visible;
                LblMessageFeedback.Foreground = new SolidColorBrush(Colors.Green);
            }
            catch (NATSNoRespondersException)
            {
                LblMessageFeedback.Content = "Message not published, no clients are listening on this subject";
                LblMessageFeedback.Visibility = Visibility.Visible;
                LblMessageFeedback.Foreground = new SolidColorBrush(Colors.Red);

            }
            catch (Exception exception)
            {
                LblMessageFeedback.Content = $"Message not published. Exception: {exception.Message}";
                LblMessageFeedback.Visibility = Visibility.Visible;
                LblMessageFeedback.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void ConnectionStatusEventHandler(object? obj, EventArgs args)
            => Dispatcher.Invoke((Action)(() =>
            {
                UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
            }));
    }
}
