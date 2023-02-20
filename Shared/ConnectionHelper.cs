using NATS.Client;
using System;

namespace Shared
{
    public static class ConnectionHelper
    {
        public static IConnection CreateConnection(Options? options = null)
        {
            ConnectionFactory connectionFactory = new();

            //Configuration configuration = ReadConfiguration();

            options ??= ConnectionFactory.GetDefaultOptions();

            //if (options.Url.Contains("0.0.0.0"))
            //    options.Url = options.Url.Replace("0.0.0.0", "localhost");

            //options.Url = $"nats://{configuration.Host}:{configuration.Port}";
            return connectionFactory.CreateConnection(options);
        }

        public static Options AddConnectionStatusChangedEventHandler(this Options options, EventHandler<ConnEventArgs> handler)
        {
            options.DisconnectedEventHandler += handler;
            options.ClosedEventHandler += handler;
            options.ReconnectedEventHandler += handler;
            return options;
        }
    }
}