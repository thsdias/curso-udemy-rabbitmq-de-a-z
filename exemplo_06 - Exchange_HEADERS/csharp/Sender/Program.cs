using System;
using RabbitMQ.Client;
using System.Text;

/*
    EXCHANGE HEADERS:

    Permite que você roteie mensagens para filas com base em cabeçalhos específicos (headers) em vez de uma chave de roteamento. 
    Isso é útil quando você precisa de roteamento mais complexo e flexível que não pode ser facilmente alcançado com chaves de 
    roteamento.

    Produtor:

    Declare a Exchange: 
        channel.ExchangeDeclare(exchange: "header_logs", type: "headers") cria uma exchange do tipo headers chamada "headers_logs".

    Enviar Mensagens: 
        Para cada conjunto de cabeçalhos, uma mensagem é enviada para a exchange "header_logs".
*/

string _exchange = "header_logs";

var connection = new ConnectionFactory { HostName = "localhost" }.CreateConnection();

using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: _exchange, 
                            type: ExchangeType.Headers);

    var messages = new[] { "Primeiro log", "Segundo log", "Terceiro log", "Quarto log", "Quinto log" };
    var severities = new[] { "info"      , "error"      , "warning"     , "error"     , "info" };
    var components = new[] { "A"         , "B"          , "A"           , "A"         , "B" };

    for (int i = 0; i < 5; i++)
    {
        var props = channel.CreateBasicProperties();
        props.Headers = new Dictionary<string, object>
        {
            { "component", components[i] },
            { "severity", severities[i] }
        };

        channel.BasicPublish(exchange: _exchange, 
                             routingKey: "", 
                             basicProperties: props, 
                             body: Encoding.UTF8.GetBytes(messages[i]));

        Console.WriteLine($" [x] Sent [component]: {components[i]}, [severity]: {severities[i]} => {messages[i]}");
        System.Threading.Thread.Sleep(1000);
    }
}
