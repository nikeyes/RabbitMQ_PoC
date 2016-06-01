using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.Messaging
{
    class Receive
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare("topic_render", "topic");
                    channel.ExchangeDeclare("topic_import", "topic");
                    channel.ConfirmSelect();

                    var qRenderDaily =  channel.QueueDeclare("render.daily", true, false, false, null);
                    //var qRenderNightly = channel.QueueDeclare("render.nightly", true, false, false, null);
                    //var qImportDaily = channel.QueueDeclare("import.daily", true, false, false, null);

                    channel.QueueBind(qRenderDaily, "topic_render", "daily");
                    //channel.QueueBind(qRenderNightly, "topic_render", "nightly");
                    //channel.QueueBind(qImportDaily, "topic_import", "daily");
                   

                    Console.WriteLine(" [*] Waiting for messages. " +
                                      "To exit press CTRL+C");

                    var consumer = new QueueingBasicConsumer(channel);
                    channel.BasicConsume(qRenderDaily, false, consumer);
                    //channel.BasicConsume(qRenderNightly, true, consumer);
                    //channel.BasicConsume(qImportDaily, true, consumer);

                    while (true)
                    {
                        var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        channel.BasicAck(ea.DeliveryTag, false);

                        Console.WriteLine(" [x] Received '{0}':'{1}'",
                                          routingKey, message);
                    }
                }
            }
        }
    }
}
