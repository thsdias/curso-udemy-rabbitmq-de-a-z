using System;
using RabbitMQ.Client;
using System.Text;

/*
    EXCHANGE FANOUT:

    Em RabbitMQ, uma exchange do tipo fanout é usada para distribuir mensagens para todas as 
    filas que estão ligadas a ela, sem considerar a chave de roteamento (routing key). 
    Isso é útil em cenários de broadcast, onde você deseja que a mesma mensagem seja entregue a múltiplos consumidores.

    Produtor:
    
    Declare a Exchange: 
        channel.ExchangeDeclare(exchange: "logs", type: "fanout") cria uma exchange do tipo fanout chamada "logs".
    
    Enviar Mensagem: 
        channel.BasicPublish(exchange: "logs", routingKey: "", basicProperties: null, body: body) envia uma mensagem 
        para a exchange "logs". A chave de roteamento (routing key) é ignorada em uma exchange do tipo fanout.
*/

string _exchange = "logs";

var connection = new ConnectionFactory { HostName = "localhost" }.CreateConnection();

using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: _exchange, 
                            type: "fanout");

    var messages = new[] { "Primeiro log", "Segundo log", "Terceiro log", "Quarto log", "Quinto log" };

    foreach (var message in messages)
    {
        var body = Encoding.UTF8.GetBytes(message);
        
        channel.BasicPublish(exchange: _exchange, 
                             routingKey: "", 
                             basicProperties: null, 
                             body: body);
        
        Console.WriteLine($" [x] Enviada '{message}'");
        System.Threading.Thread.Sleep(1000);
    }
}
