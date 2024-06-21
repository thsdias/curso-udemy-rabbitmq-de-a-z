using RabbitMQ.Client;
using System.Text;

// Nome da fila.
string _queue = "hello";

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: _queue,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

string[] messages = ["Primeira mensagem", "Segunda mensagem", "Terceira mensagem", "Quarta mensagem", "Quinta mensagem"];

Console.WriteLine();

foreach (var message in messages)
{
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: "",
                         routingKey: _queue,
                         body: body);

    Console.WriteLine(" [x] Sent {0}", message);
    Thread.Sleep(3000);
}

Console.WriteLine();
