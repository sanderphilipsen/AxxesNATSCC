using NATS.Client;
using Shared;
using System;
using System.Text;
using System.Windows;

namespace Subscriber
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IConnection? _connection = null;
        private const string QueueGroup = "FunctionalFullStack";
        public MainWindow() => InitializeComponent();

        private void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (BtnConnect.Content is "Disconnect")
            {
                _connection?.Close();
                LstSubscriptions.Items.Clear();
            }
            else
                Connect();
        }

        private void Connect()
        {
            var options = ConnectionFactory.GetDefaultOptions();

            options.AddConnectionStatusChangedEventHandler(ConnectionStatusEventHandler);
            options.User = "john";
            options.Password = "demo";
            _connection = ConnectionHelper.CreateConnection(options);
            UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect);
        }

        private void BtnSubscribe_Click(object sender, RoutedEventArgs e)
        {
            LblFeedback.Content = "";
            try
            {
                var subscription = _connection?.SubscribeAsync(TxtSubject.Text, QueueGroup, GetMessageHandler());
                LstSubscriptions.DisplayMemberPath = "Subject";
                LstSubscriptions.SelectedValuePath = "Subject";
                if (subscription != null && subscription.IsValid)
                    LstSubscriptions.Items.Add(subscription);

            }
            catch (Exception ex)
            {
                LblFeedback.Content = ex.Message;
            }
        }

        private void Unsubscribe_Click(object sender, RoutedEventArgs e)
        {
            var subscription = LstSubscriptions.SelectedItems[0] as IAsyncSubscription;
            subscription?.Unsubscribe();
            LstSubscriptions.Items.Remove(subscription);
        }

        private EventHandler<MsgHandlerEventArgs> GetMessageHandler()
            => (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Message.Data);

                Dispatcher.Invoke((Action)(() =>
                {
                    LstMessages.Items.Insert(0, message);
                }));
            };

        private void ConnectionStatusEventHandler(object? obj, EventArgs args)
            => Dispatcher.Invoke((Action)(() =>
            {
                UiHelper.UpdateConnectionStatus(_connection, ConnectionBorder, LblConnectionStatus, BtnConnect, LstSubscriptions);
            }));
    }
}
