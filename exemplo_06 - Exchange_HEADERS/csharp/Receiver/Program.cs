using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/*
    EXCHANGE HEADERS:

    Consumidor:

    Declarar a Exchange: 
        channel.ExchangeDeclare(exchange: "header_logs", type: "headers") assegura que a exchange "header_logs" existe.
    
    Criar Fila: 
        var queueName = channel.QueueDeclare().QueueName cria uma fila temporária com um nome gerado pelo servidor.
    
    Ligar Fila à Exchange: 
        A fila é ligada à exchange "header_logs" com cabeçalhos específicos. Neste exemplo, usamos { "component", "severity" } para especificar que todos os cabeçalhos devem corresponder.
    
    Consumir Mensagens: 
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer) começa a consumir mensagens da fila temporária.

    Execucao:
        dotnet run A error
*/

string _exchange = "header_logs";

if (args.Length < 2)
{
    Console.Error.WriteLine("Usage: {0} [component] [severity]...", Environment.GetCommandLineArgs()[0]);
    return;
}

var connectionFactory = new ConnectionFactory { HostName = "localhost" };

using (var connection = connectionFactory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: _exchange, 
                            type: ExchangeType.Headers);

    var queueName = channel.QueueDeclare().QueueName;

    var headers = new Dictionary<string, object>
    {
        { "component", args[0] },
        { "severity", args[1] }
    };

    channel.QueueBind(queue: queueName, 
                      exchange: _exchange, 
                      routingKey: "", 
                      arguments: headers);

    Console.WriteLine(" [*] Waiting for logs. To exit press CTRL+C");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var properties = ea.BasicProperties;
        var body = ea.Body.ToArray();

        Console.WriteLine(" [x] {0}:{1}", properties.Headers, System.Text.Encoding.UTF8.GetString(body));
    };

    channel.BasicConsume(queueName, true, consumer);

    Console.ReadLine();
}
