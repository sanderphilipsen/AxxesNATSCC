﻿using NATS.Client;
using NATS.Client.JetStream;
using Shared;
using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Consumer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IConnection? _connection = null;
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

            UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
        }

        private void BtnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            LblException.Visibility = Visibility.Hidden;

            ConsumerConfiguration cc = ConsumerConfiguration.Builder()
                .WithDurable(Guid.NewGuid().ToString())
                .WithReplayPolicy(ReplayPolicy.Original)
                .Build();

            PushSubscribeOptions options = PushSubscribeOptions.Builder()
                .WithConfiguration(cc)
                .Build();

            var jetStreamContext = _connection?.CreateJetStreamContext();
            try
            {
                var subscription = jetStreamContext?.PushSubscribeAsync(TxtSubject.Text, GetMessageHandler(), true, options);
                LstSubscriptions.Items.Add(TxtSubject.Text);
            }
            catch (Exception exception)
            {
                LblException.Content = exception.Message;
                LblException.Visibility = Visibility.Visible;
                LblException.Foreground = new SolidColorBrush(Colors.Red);
            }

        }

        private EventHandler<MsgHandlerEventArgs> GetMessageHandler()
        {
            return (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Message.Data);

                Dispatcher.Invoke((Action)(() =>
                {
                    LstMessages.Items.Insert(0, message);
                }));
            };
        }

        private void ConnectionStatusEventHandler(object? obj, EventArgs args)
            => Dispatcher.Invoke((Action)(() =>
            {
                UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
            }));

    }
}
