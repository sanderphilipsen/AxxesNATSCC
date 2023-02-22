using NATS.Client;
using System;

namespace Shared
{
    public static class ConnectionHelper
    {
        public static IConnection CreateConnection(Options? options = null)
        {
            ConnectionFactory connectionFactory = new();
            options ??= ConnectionFactory.GetDefaultOptions();

            options.Url = "nats://localhost:4221";
            return connectionFactory.CreateConnection(options);
        }

        public static Options AddConnectionStatusChangedEventHandler(this Options options, EventHandler<ConnEventArgs> handler)
        {
            options.DisconnectedEventHandler += handler;
            options.ClosedEventHandler += handler;
            options.ReconnectedEventHandler += handler;
            options.AllowReconnect = true;
            return options;
        }
    }
}