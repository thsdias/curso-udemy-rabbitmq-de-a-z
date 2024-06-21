using System;
using RabbitMQ.Client;
using System.Text;

/*
    EXCHANGE DIRECT:

    Permite que você roteie mensagens para filas com base em uma chave de roteamento (routing key) exata. 
    Isso é útil quando você deseja que apenas determinadas filas recebam mensagens que correspondem a uma 
    chave de roteamento específica.

    Produtor:

    Declare a Exchange: 
        channel.ExchangeDeclare(exchange: "direct_logs", type: "direct") cria uma exchange do tipo direct chamada "direct_logs".

    Enviar Mensagens: 
        Para cada severidade ("info", "warning", "error"), uma mensagem é enviada para a exchange "direct_logs" com a chave de roteamento correspondente. 
        Por exemplo, uma mensagem com a chave de roteamento "info" será enviada para a exchange "direct_logs".
*/

string _exchange = "direct_logs";

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: _exchange, 
                            type: "direct");

    string[] messages = { "Primeiro log", "Segundo log", "Terceiro log", "Quarto log", "Quinto log" };
    string[] severities = { "info", "error", "warning", "error", "info" };

    for (int i = 0; i < 5; i++)
    {
        channel.BasicPublish(exchange: _exchange,
                             routingKey: severities[i],
                             basicProperties: null,
                             body: Encoding.UTF8.GetBytes(messages[i]));
        
        Console.WriteLine($" [x] Sent {severities[i]}:{messages[i]}");
        System.Threading.Thread.Sleep(1000);
    }
}
