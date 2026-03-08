using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Domain.Constants.AppConstants;

namespace Infrastructure.Extensions.RabbitMQ
{
    public class RabbitMqConsumer : BackgroundService
    {
        private const string QueueName = "Hello";
        private const string DlqName = "Hello.DLQ";
        private const int MaxRetryCount = 3;
        private const string RetryCountHeader = "x-retry-count";

        private readonly RabbitMqConnection _rabbitMqConnection;
        private IChannel? _channel;

        public RabbitMqConsumer(RabbitMqConnection rabbitMqConnection)
        {
            _rabbitMqConnection = rabbitMqConnection;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _channel = await _rabbitMqConnection.Connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await _channel.QueueDeclareAsync(DlqName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += OnMessageReceivedAsync;

            await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task OnMessageReceivedAsync(object sender, BasicDeliverEventArgs ea)
        {
            if (_channel == null || !_channel.IsOpen) return;

            var body = ea.Body.ToArray();
            var message = System.Text.Encoding.UTF8.GetString(body);
            var deliveryTag = ea.DeliveryTag;

            try
            {
                Console.WriteLine($" [x] Received {message}");
                await _channel.BasicAckAsync(deliveryTag, false, CancellationToken.None);
            }
            catch (Exception ex)
            {
                var retryCount = GetRetryCount(ea);
                if (retryCount < MaxRetryCount)
                {
                    await RepublishWithRetryAsync(body, retryCount + 1, ea);
                    await _channel.BasicAckAsync(deliveryTag, false, CancellationToken.None);
                }
                else
                {
                    await PublishToDlqAsync(body, ea, ex.Message);
                    await _channel.BasicNackAsync(deliveryTag, false, false, CancellationToken.None);
                }
            }
        }

        private static int GetRetryCount(BasicDeliverEventArgs ea)
        {
            if (ea.BasicProperties.Headers == null || !ea.BasicProperties.Headers.TryGetValue(RetryCountHeader, out var value))
                return 0;
            if (value is int i) return i;
            if (value is byte[] bytes)
            {
                var s = System.Text.Encoding.UTF8.GetString(bytes);
                return int.TryParse(s, out var n) ? n : 0;
            }
            return 0;
        }

        private async Task RepublishWithRetryAsync(byte[] body, int retryCount, BasicDeliverEventArgs ea)
        {
            if (_channel == null) return;
            var props = new BasicProperties
            {
                Persistent = true,
                Headers = new Dictionary<string, object?>
                {
                    [RetryCountHeader] = retryCount
                }
            };
            await _channel.BasicPublishAsync("", QueueName, false, props, new ReadOnlyMemory<byte>(body), CancellationToken.None);
        }

        private async Task PublishToDlqAsync(byte[] body, BasicDeliverEventArgs ea, string errorReason)
        {
            if (_channel == null) return;
            var props = new BasicProperties
            {
                Persistent = true,
                Headers = new Dictionary<string, object?>
                {
                    [RetryCountHeader] = GetRetryCount(ea),
                    ["x-dlq-reason"] = errorReason,
                    ["x-dlq-at"] = DateTime.UtcNow.ToString("O")
                }
            };
            await _channel.BasicPublishAsync("", DlqName, false, props, new ReadOnlyMemory<byte>(body), CancellationToken.None);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel?.IsOpen == true)
                await _channel.CloseAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
