using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/*
    Consumidor:

    Declare a Exchange: 
        channel.ExchangeDeclare(exchange: "logs", type: "fanout") assegura que a exchange "logs" existe.
    
    Criar Fila Temporária: 
        var queueName = channel.QueueDeclare().QueueName cria uma fila temporária com um nome gerado pelo servidor.
    
    Ligar Fila à Exchange: 
        channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "") liga a fila temporária à exchange "logs".
    
    Consumir Mensagens: 
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer) começa a consumir mensagens da fila temporária.
*/

string _exchange = "logs";

var factory = new ConnectionFactory { HostName = "localhost" };

using (var connection = factory.CreateConnection())

using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(_exchange, 
                            ExchangeType.Fanout);

    var queueName = channel.QueueDeclare().QueueName;

    channel.QueueBind(queueName, 
                      _exchange, 
                      string.Empty);

    Console.WriteLine(" [*] Waiting for logs. To exit press CTRL+C");

    var consumer = new EventingBasicConsumer(channel);
    
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        Console.WriteLine(" [x] {0}", Encoding.UTF8.GetString(body));
    };

    channel.BasicConsume(queueName, true, consumer);

    Console.ReadLine();
}
