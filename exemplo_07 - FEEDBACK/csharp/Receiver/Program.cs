using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/*
    FEEDBACK:

*/

string _queue = "rpc_queue";

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: _queue, 
                         durable: false, 
                         exclusive: false, 
                         autoDelete: false, 
                         arguments: null);

    Console.WriteLine(" [x] Awaiting RPC requests");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        string response;
        var body = ea.Body.ToArray();
        int n = int.Parse(Encoding.UTF8.GetString(body));

        Console.WriteLine(" [.] Calculating {0} * {0}", n);
        response = (n * n).ToString();

        var props = ea.BasicProperties;
        var replyProps = channel.CreateBasicProperties();
        replyProps.CorrelationId = props.CorrelationId;

        channel.BasicPublish(exchange: "", 
                             routingKey: props.ReplyTo, 
                             basicProperties: replyProps, 
                             body: Encoding.UTF8.GetBytes(response));
        
        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
    };

    channel.BasicConsume(queue: _queue, 
                         autoAck: false, 
                         consumer: consumer);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}
