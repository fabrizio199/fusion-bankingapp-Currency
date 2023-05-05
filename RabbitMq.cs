using System;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;

class Send
{
    public void RabbitMqWork()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();
        Produce(channel);
        Consume(channel);
    }
    public void Consume(IModel channel)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);
        };
        channel.BasicConsume(queue: "Rabbit",
                                autoAck: true,
                                consumer: consumer);


        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
	
	public void Produce(IModel channel)
	{
		channel.QueueDeclare(queue: "Rabbit",
							 durable: false,
							 exclusive: false,
							 autoDelete: false,
							 arguments: null);

		string message = "Hello World!";
		var body = Encoding.UTF8.GetBytes(message);

		channel.BasicPublish(exchange: "",
							 routingKey: "Rabbit",
							 basicProperties: null,
							 body: body);
		Console.WriteLine(" [x] Sent {0}", message);	
	}
}
