using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/*
    EXCHANGE TOPIC:

    Consumidor:

    Declarar a Exchange: 
        channel.ExchangeDeclare(exchange: "topic_logs", type: "topic") assegura que a exchange "topic_logs" existe.

    Criar Fila: 
        var queueName = channel.QueueDeclare().QueueName cria uma fila temporária com um nome gerado pelo servidor.

    Ligar Fila à Exchange: 
        Para cada chave de binding passada como argumento, a fila é ligada à exchange "topic_logs" com a chave de binding correspondente.
    
    Consumir Mensagens: 
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer) começa a consumir mensagens da fila temporária.

    Execucao:
        Consumer "A.warning*"   // Chave de roteamento específica.
        Consumer "*.info"       // Qualquer chave de roteamento com "info" como a segunda palavra.
        Consumer "A.#"          // Qualquer chave de roteamento que comece com "A".
*/

string _exchange = "topic_logs";

var factory = new ConnectionFactory { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: _exchange, 
                            type: ExchangeType.Topic);

    var queueName = channel.QueueDeclare().QueueName;

    if (args.Length < 1)
    {
        Console.Error.WriteLine("Usage: {0} [binding_key]...", Environment.GetCommandLineArgs()[0]);
        return;
    }

    foreach (var bindingKey in args)
    {
        channel.QueueBind(queue: queueName, 
                          exchange: _exchange, 
                          routingKey: bindingKey);
    }

    Console.WriteLine(" [*] Waiting for logs. To exit press CTRL+C");

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
        var routingKey = ea.RoutingKey;
        var body = ea.Body.ToArray();
        Console.WriteLine(" [x] {0}:{1}", routingKey, Encoding.UTF8.GetString(body));
    };

    channel.BasicConsume(queue: queueName, 
                         autoAck: true, 
                         consumer: consumer);

    Console.ReadLine();
}