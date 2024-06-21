using System;
using RabbitMQ.Client;
using System.Text;

/*
    EXCHANGE TOPIC:

    Permite que você roteie mensagens para uma ou mais filas com base em padrões de chave de roteamento (routing key). 
    As chaves de roteamento podem usar caracteres especiais como * (um único caractere) e # (zero ou mais caracteres) 
    para criar padrões de roteamento flexíveis.

    Produtor:

    Declare a Exchange: 
        channel.ExchangeDeclare(exchange: "topic_logs", type: "topic") cria uma exchange do tipo topic chamada "topic_logs".

    Enviar Mensagens: 
        Para cada chave de roteamento na lista routingKey, uma mensagem é enviada para a exchange "topic_logs" com a chave de roteamento correspondente.
*/

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
