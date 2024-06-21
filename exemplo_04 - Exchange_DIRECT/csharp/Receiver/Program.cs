using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/*
    EXCHANGE DIRECT:

    Consumidor:

    Declarar a Exchange: 
        channel.ExchangeDeclare(exchange: "direct_logs", type: "direct") assegura que a exchange "direct_logs" existe.
    Criar Fila: 
        var queueName = channel.QueueDeclare().QueueName cria uma fila temporária com um nome gerado pelo servidor.
    
    Ligar Fila à Exchange: 
        Para cada severidade passada como argumento (info, warning, error), a fila é ligada à exchange "direct_logs" com a chave de roteamento correspondente.
    
    Consumir Mensagens: 
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer) começa a consumir mensagens da fila temporária.   

    Exemplo execucao:
        dotnet run error
        dotnet run info warning
*/

string _exchange = "direct_logs";

var factory = new ConnectionFactory { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: _exchange, 
                            type: ExchangeType.Direct);

    var queueName = channel.QueueDeclare().QueueName;

    if (args.Length < 1)
    {
        Console.Error.WriteLine("Usage: {0} [info] [warning] [error]", Environment.GetCommandLineArgs()[0]);
        return;
    }

    foreach (var severity in args)
    {
        channel.QueueBind(queue: queueName,
                          exchange: _exchange,
                          routingKey: severity);
    }

    Console.WriteLine(" [*] Waiting for logs. To exit press CTRL+C");

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = System.Text.Encoding.UTF8.GetString(body);
        Console.WriteLine(" [x] {0}:{1}", ea.RoutingKey, message);
    };

    channel.BasicConsume(queue: queueName,
                         autoAck: true,
                         consumer: consumer);

    Console.ReadLine();
}
