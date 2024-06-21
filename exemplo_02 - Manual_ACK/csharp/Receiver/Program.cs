using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

/*
    Exemplo com Manual ACK: (autoAck = false)
    A mensagem so sera retirada da fila, apos confirmacao manual (possibilidade de tratativa a falhas) => channel.BasicAck

    No RabbitMQ, a confirmação manual (manual acknowledgment ou manual ACK) é uma técnica 
    utilizada para garantir que as mensagens foram processadas com sucesso pelo consumidor 
    antes de serem removidas da fila. Isso aumenta a confiabilidade ao prevenir a perda de 
    mensagens em caso de falhas do consumidor.
*/

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "hello", 
                         durable: false, 
                         exclusive: false, 
                         autoDelete: false, 
                         arguments: null);

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine(" [x] Received {0}", message);

        Thread.Sleep(1000);
        Console.WriteLine(" [x] Done");

        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    };

    channel.BasicConsume(queue: "hello", 
                         autoAck: false, 
                         consumer: consumer);

    Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");
    Console.ReadLine();
}
