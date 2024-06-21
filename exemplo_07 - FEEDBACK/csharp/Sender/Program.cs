using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/*
    FEEDBACK:


*/

string _corr_id = string.Empty;
string _routingKey = "rpc_queue";

void OnResponse(object model, BasicDeliverEventArgs ea)
{
    if (_corr_id == ea.BasicProperties.CorrelationId)
    {
        Console.WriteLine(" [.] Got {0}", Encoding.UTF8.GetString(ea.Body.ToArray()));
    }
}

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    var replyQueueName = channel.QueueDeclare(queue: "", exclusive: true).QueueName;

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += OnResponse;

    channel.BasicConsume(queue: replyQueueName, 
                         autoAck: true, 
                         consumer: consumer);

    Console.WriteLine(" [x] Requesting 30 * 30");

    _corr_id = Guid.NewGuid().ToString();

    var props = channel.CreateBasicProperties();
    props.CorrelationId = _corr_id;
    props.ReplyTo = replyQueueName;

    channel.BasicPublish(exchange: "", 
                         routingKey: _routingKey, 
                         basicProperties: props, 
                         body: Encoding.UTF8.GetBytes("30"));

    Console.ReadLine();
}
