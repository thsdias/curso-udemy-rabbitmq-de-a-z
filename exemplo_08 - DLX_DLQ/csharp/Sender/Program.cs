using System;
using RabbitMQ.Client;
using System.Text;

/*
    DLX:

    Uma DLX (Dead-Letter Exchange) é uma exchange especial para onde as mensagens são redirecionadas quando não podem ser 
    processadas por uma fila. 
    Isso pode ocorrer por várias razões, como mensagens que foram rejeitadas ou que excederam o tempo de vida (TTL). 
    A DLX é usada para tratamento de erros e monitoramento de mensagens que não podem ser entregues.

    Produtor:

    Declarar a Exchange e Fila Principal: 
        channel.ExchangeDeclare e channel.QueueDeclare criam a exchange e a fila principal, respectivamente. 
        A fila principal tem uma configuração que aponta para a DLX (x-dead-letter-exchange).

    Enviar Mensagem: 
        channel.BasicPublish envia uma mensagem para a fila principal.
*/

#region Exemplo_1

string _exchange = "topic_logs";

var connection = new ConnectionFactory { HostName = "localhost" }.CreateConnection();

using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: _exchange, 
                            type: ExchangeType.Topic);

    string[] messages = { "Primeiro log", "Segundo log", "Terceiro log", "Quarto log", "Quinto log" };
    string[] severities = { "info"      , "error"      , "warning"     , "error"     , "info" };
    string[] components = { "A"         , "B"          , "A"           , "A"         , "B" };

    for (int i = 0; i < 5; i++)
    {
        string routingKey = $"{components[i]}.{severities[i]}";
        byte[] body = Encoding.UTF8.GetBytes(messages[i]);

        channel.BasicPublish(exchange: _exchange, 
                             routingKey: routingKey, 
                             basicProperties: null, 
                             body: body);
        
        Console.WriteLine($" [x] Sent {routingKey}:{messages[i]}");
        System.Threading.Thread.Sleep(1000);
    }
}

#endregion

#region Exemplo_2
/*
var _queue = "main_queue";
var _exchange = "main_exchange";
var _routingKey = "main_routing_key";
var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: _exchange, 
                            type: ExchangeType.Direct);

    channel.QueueDeclare(queue: _queue,
                         durable: true,
                         exclusive: false,
                         autoDelete: false,
                         arguments: new Dictionary<string, object>
                         {
                             { "x-dead-letter-exchange", "dlx_exchange" }
                         });

    channel.QueueBind(queue: _queue,
                      exchange: _exchange,
                      routingKey: _routingKey);

    string message = "Hello World!";
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish(exchange: _exchange,
                         routingKey: _routingKey,
                         basicProperties: null,
                         body: body);

    Console.WriteLine(" [x] Sent '{0}'", message);
}
*/
#endregion
