using System;
using RabbitMQ.Client;
using System.Text;

namespace RabbitMQ.Messaging
{
    class Send
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    for (int i = 0; i < 100000; i++)
                    {
                        string message = "Hello World!: " + i ;
                        var body = Encoding.UTF8.GetBytes(message);

                        var properties = channel.CreateBasicProperties();
                        properties.SetPersistent(true);
                        channel.ConfirmSelect();
                        channel.BasicPublish("topic_render", "daily", properties, body);
                        //.BasicPublish("topic_render", "nightly", properties, body);
                        //channel.BasicPublish("topic_import", "daily", properties, body);


                        Console.WriteLine(" [x] Sent {0}", message);
                    }  
                }
            }
        }
    }
}
