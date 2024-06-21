using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

/*
    DLX:

    Consumidor:

    Declarar DLX e Fila de Mensagens Mortas: 
        channel.ExchangeDeclare e channel.QueueDeclare criam a DLX e a fila de mensagens mortas (DLQ), respectivamente.
    
    Declarar a Fila Principal: 
        channel.QueueDeclare cria a fila principal com as mesmas configurações que no produtor.
    
    Consumir e Rejeitar Mensagens: 
        channel.BasicConsume consome mensagens da fila principal, e channel.BasicReject rejeita as mensagens, redirecionando-as para a DLX.
*/

#region Exemplo 1

// Ex.: dotnet run A.info

var factory = new ConnectionFactory { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare("dlx_logs", ExchangeType.Topic);

    var queueDeclareOk = channel.QueueDeclare("dlq_logs", false, false, false, null);
    
    channel.QueueBind("dlq_logs", "dlx_logs", "#");

    channel.ExchangeDeclare("topic_logs", ExchangeType.Topic);

    var arguments = new Dictionary<string, object>
    {
        { "x-dead-letter-exchange", "dlx_logs" },
        { "x-dead-letter-routing-key", "ABC" }
    };

    var queueName = channel.QueueDeclare("", true, false, true, arguments).QueueName;

    if (args.Length == 0)
    {
        Console.Error.WriteLine("Usage: {0} [binding_key]...", Environment.GetCommandLineArgs()[0]);
        return;
    }

    foreach (var bindingKey in args)
    {
        channel.QueueBind(queueName, "topic_logs", bindingKey);
    }

    Console.WriteLine(" [*] Waiting for logs. To exit press CTRL+C");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var routingKey = ea.RoutingKey;
        Console.WriteLine(" [x] {0}:{1}", routingKey, System.Text.Encoding.UTF8.GetString(body));
        channel.BasicNack(ea.DeliveryTag, false, false);
    };

    channel.BasicConsume(queueName, false, consumer);

    Console.ReadLine();
}
#endregion

#region Exemplo 2
/*
var _dlxExchange = "dlx_exchange";
var _mainQueue = "main_queue";
var _dlxQueue = "dlx_queue";
var _dlxRoutingKey = "dlx_routing_key";

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    // Declare DLX and DLQ
    channel.ExchangeDeclare(exchange: _dlxExchange, 
                            type: ExchangeType.Direct);

    channel.QueueDeclare(queue: _dlxQueue,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

    channel.QueueBind(queue: _dlxQueue,
                        exchange: _dlxExchange,
                        routingKey: _dlxRoutingKey);

    // Declare main queue
    channel.QueueDeclare(queue: _mainQueue,
                            durable: true,
                            exclusive: false,
                            autoDelete: false,
                            arguments: new Dictionary<string, object>
                            {
                                { "x-dead-letter-exchange", _dlxExchange },
                                { "x-dead-letter-routing-key", _dlxRoutingKey }
                            });

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, ea) =>
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine(" [x] Received '{0}'", message);

        // Reject message and send to DLX.
        channel.BasicReject(deliveryTag: ea.DeliveryTag, requeue: false);
    };

    channel.BasicConsume(queue: _mainQueue,
                            autoAck: false,
                            consumer: consumer);

    Console.WriteLine(" Press [enter] to exit.");
    Console.ReadLine();
}
*/
#endregion
