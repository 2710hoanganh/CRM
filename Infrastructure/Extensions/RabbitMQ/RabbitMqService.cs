using Application.Services;
using Infrastructure.Extensions.RabbitMQ;
using RabbitMQ.Client;
using System.Text.Json;
namespace Infrastructure.Extensions.RabbitMQ
{
    public class RabbitMqService : IRabbitMqService
    {
        private readonly RabbitMqConnection _rabbitMqConnection;
        public RabbitMqService(RabbitMqConnection rabbitMqConnection)
        {
            _rabbitMqConnection = rabbitMqConnection;
        }

        public async Task PublishAsync<T>(T message, string exchange, string routingKey) where T : class
        {
            try
            {
                var channel = await _rabbitMqConnection.Connection.CreateChannelAsync();
                await channel.QueueDeclareAsync(queue: routingKey,
                                                durable: true,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);
                var body = JsonSerializer.SerializeToUtf8Bytes(message);

                await channel.BasicPublishAsync(exchange: exchange ?? "",
                                                routingKey: routingKey,
                                                body: body);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}