using NATS.Client;
using NATS.Client.JetStream;
using Shared;
using System;
using System.Collections;
using System.Collections.Generic;
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
        private List<string> _subjects = new();
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
            options.AddConnectionStatusChangedEventHandler(ConnectionStatusEventHandler);

            _connection = ConnectionHelper.CreateConnection(options);
            _jetStreamManagement = _connection.CreateJetStreamManagementContext();

            UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
        }

        private void BtnPublish_Click(object sender, RoutedEventArgs e)
        {
            LblMessageFeedback.Visibility = Visibility.Hidden;

            if (_connection?.State is not ConnState.CONNECTED)
                return;

            if (_subjects.All(subject => subject != TxtSubject.Text))
            {
                _subjects.Add(TxtSubject.Text);
                AddOrUpdateStream();
            }

            var header = new MsgHeader();
            var message = new Msg(TxtSubject.Text, header, Encoding.UTF8.GetBytes(TxtMessage.Text));

            try
            {
                // Publish the message to the stream

                LblMessageFeedback.Content = "Message published";
                LblMessageFeedback.Visibility = Visibility.Visible;
                LblMessageFeedback.Foreground = new SolidColorBrush(Colors.Green);
            }
            catch (Exception exception)
            {
                LblMessageFeedback.Content = $"Message not published. Exception: {exception.Message}";
                LblMessageFeedback.Visibility = Visibility.Visible;
                LblMessageFeedback.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void BtnPurgeStream_Click(object sender, RoutedEventArgs e)
        {
            // Purge the stream
        }

        private void BtnDeleteStream_Click(object sender, RoutedEventArgs e)
        {
            // Delete the stream
            _subjects.Clear(); ;
        }

        private void AddOrUpdateStream()
        {
            //Create a stream with storage type file and the entered subject or update stream with new subject
        }

        private void ConnectionStatusEventHandler(object? obj, EventArgs args)
            => Dispatcher.Invoke((Action)(() =>
            {
                UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
            }));
    }
}
