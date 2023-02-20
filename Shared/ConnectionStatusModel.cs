using System.Windows.Media;


namespace Shared
{
    public class ConnectionStatus
    {
        public Color Color { get; }
        public string Status { get; }
        public string ButtonConnectText { get; }

        private ConnectionStatus(string status, Color color, string buttonText)
        {
            Color = color;
            Status = status;
            ButtonConnectText = buttonText;
        }

        public static readonly ConnectionStatus Connected = new("Connected", Colors.Green, "Disconnect");
        public static readonly ConnectionStatus Connecting = new("Connecting...", Colors.Orange, "Disconnect");
        public static readonly ConnectionStatus Closed = new("Closed", Colors.Red, "Connect");
        public static readonly ConnectionStatus Reconnecting = new("Reconnecting", Colors.Orange, "Disconnect");
        public static readonly ConnectionStatus Disconnected = new("Disconnected", Colors.Red, "Connect");
    }
}
