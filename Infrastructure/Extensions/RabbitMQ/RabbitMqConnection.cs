using RabbitMQ.Client;
using Domain.Models.Common;

namespace Infrastructure.Extensions.RabbitMQ
{
    public class RabbitMqConnection
    {
        public IConnection Connection { get; }

        private RabbitMqConnection(IConnection connection)
        {
            Connection = connection;
        }

        public static async Task<RabbitMqConnection> CreateAsync(RabbitMqConfig options)
        {
            var factory = new ConnectionFactory
            {
                HostName = options.Host,
                Port = options.Port,
                UserName = options.Username,
                Password = options.Password
            };

            var connection = await factory.CreateConnectionAsync();
            return new RabbitMqConnection(connection);
        }
    }

}
