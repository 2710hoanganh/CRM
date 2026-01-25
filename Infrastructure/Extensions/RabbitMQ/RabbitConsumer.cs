using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Extensions.RabbitMQ
{
    public class RabbitMqConsumer : BackgroundService
    {
        private readonly RabbitMqConnection _rabbitMqConnection;
        private IChannel? _channel;

        public RabbitMqConsumer(RabbitMqConnection rabbitMqConnection)
        {
            _rabbitMqConnection = rabbitMqConnection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = await _rabbitMqConnection.Connection.CreateChannelAsync();
            await _channel.QueueDeclareAsync(queue: "Hello", durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += (_, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = System.Text.Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {message}");
                return Task.CompletedTask;
            };

            await _channel.BasicConsumeAsync(queue: "Hello", autoAck: true, consumer: consumer);

            // Giữ service chạy đến khi app shutdown (stoppingToken bị cancel)
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel?.IsOpen == true)
                await _channel.CloseAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
