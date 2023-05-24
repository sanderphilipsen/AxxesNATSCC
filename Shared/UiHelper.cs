using NATS.Client;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Shared
{
    public static class UiHelper
    {
        public static void UpdateConnectionStatus(IConnection connection, Border border, Label label, Button button, ListBox? list = null)
        {
            switch (connection?.State)
            {
                case ConnState.CONNECTED:
                    UpdateConnectionStatusControls(ConnectionStatus.Connected, border, label, button);
                    break;
                case ConnState.CONNECTING:
                    UpdateConnectionStatusControls(ConnectionStatus.Connecting, border, label, button);
                    break;
                case ConnState.CLOSED:
                    UpdateConnectionStatusControls(ConnectionStatus.Closed, border, label, button, list);
                    break;
                case ConnState.RECONNECTING:
                    UpdateConnectionStatusControls(ConnectionStatus.Reconnecting, border, label, button);
                    break;
                default:
                    UpdateConnectionStatusControls(ConnectionStatus.Disconnected, border, label, button, list);
                    break;
            }
        }

        private static void UpdateConnectionStatusControls(ConnectionStatus connectionStatus, Border border, Label label, Button button, ListBox? list = null)
        {
            label.Content = connectionStatus.Status;
            border.BorderBrush = new SolidColorBrush(connectionStatus.Color);
            button.Content = connectionStatus.ButtonConnectText;
            if (list is not null)
                list.Items.Clear();
        }
    }
}
