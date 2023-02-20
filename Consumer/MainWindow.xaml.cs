using NATS.Client;
using NATS.Client.JetStream;
using Shared;
using System;
using System.Text;
using System.Windows;

namespace Consumer
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
            var options = ConnectionFactory.GetDefaultOptions();

            options.AllowReconnect = true;
            options.AddConnectionStatusChangedEventHandler(ConnectionStatusEventHandler);
            options.Url = "nats://localhost:4222";

            _connection = ConnectionHelper.CreateConnection(options);

            UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
        }

        private void BtnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            ConsumerConfiguration cc = ConsumerConfiguration.Builder()
                .WithDurable("optional-durable-name")
                .Build();
            PushSubscribeOptions pso = PushSubscribeOptions.Builder()
                .WithConfiguration(cc)
                .Build();

            var jetStreamContext = _connection?.CreateJetStreamContext();
            var result = jetStreamContext?.PushSubscribeAsync(TxtSubject.Text, GetMessageHandler(), false, pso);

            LstSubscriptions.Items.Add(TxtSubject.Text);
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

        private void Unsubscribe_Click(object sender, RoutedEventArgs e)
        {
            var subscription = LstSubscriptions.SelectedItems[0] as IAsyncSubscription;
            subscription?.Unsubscribe();
            LstSubscriptions.Items.Remove(subscription);
        }

        private void ConnectionStatusEventHandler(object? obj, EventArgs args)
            => Dispatcher.Invoke((Action)(() =>
            {
                UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
            }));

    }
}
