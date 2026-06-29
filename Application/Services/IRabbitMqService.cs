namespace Application.Services
{
    public interface IRabbitMqService
    {
        Task PublishAsync<T>(T message, string exchange, string routingKey) where T : class;
    }
}