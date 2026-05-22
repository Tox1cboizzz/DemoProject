using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace BillingService.Messaging
{
    public class RabbitMQProducer
    {
        private readonly String _hostname = "localhost";

        public async Task SendMessage<T>(string queueName, T message)
        {
            var factory = new ConnectionFactory() { HostName = _hostname };
            await using (var connection = await factory.CreateConnectionAsync())
            await using (var channel = await connection.CreateChannelAsync())
            {
                await channel.QueueDeclareAsync(
                                     queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                string jsonMessage = JsonSerializer.Serialize(message);
                var body = Encoding.UTF8.GetBytes(jsonMessage);
                await channel.BasicPublishAsync(
                                         exchange: string.Empty,
                                         routingKey: queueName,
                                         body: body);
            }
        }
    }
}