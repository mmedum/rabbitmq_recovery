using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Consumer!");
            var factory = setupConnection();

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //Queue
                //Declare to be certain that the queues exist,
                //consider whether the publisher or consumer should
                //create the queues
                // channel.QueueDeclare(queue: "pingpongA",
                //                      durable: true,
                //                      exclusive: false,
                //                      autoDelete: false,
                //                      arguments: null);

                // channel.QueueDeclare(queue: "pingpongB",
                //                      durable: false,
                //                      exclusive: false,
                //                      autoDelete: false,
                //                      arguments: null);

                // var consumer = new EventingBasicConsumer(channel);
                // consumer.Received += (model, ea) =>
                // {
                //     var body = ea.Body;
                //     var message = Encoding.UTF8.GetString(body);
                //     Console.WriteLine(" [x] Received {0}", message);
                // };
                
                //Remember to consider whether auto acknowledgement should
                //true, a preference would be to handle the request fully
                //and then do acknowledge.
                // channel.BasicConsume(queue: "pingpong",
                //                      autoAck: true,
                //                      consumer: consumer);

                //Topic
                channel.ExchangeDeclare(exchange: "pingpong", type: "topic");
        
                //Bind specific queues with the topics and define the routingkeys
                // channel.QueueBind(queue: "pingpongA",
                //                   exchange: "pingpong",
                //                   routingKey: "topicA");

                // channel.QueueBind(queue: "pingpongB",
                //                   exchange: "pingpong",
                //                   routingKey: "topicB");

                //Generic consumer
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var routingKey = ea.RoutingKey;
                    Console.WriteLine(" [x] Received '{0}':'{1}'",
                                    routingKey,
                                    message);
                    channel.BasicAck(ea.DeliveryTag, false);
                };

                //Starting the consumers for two different queues
                channel.BasicConsume(queue: "pingpongA",
                                 autoAck: false,
                                 consumer: consumer);

                channel.BasicConsume(queue: "pingpongB",
                                 autoAck: false,
                                 consumer: consumer);
                Console.ReadLine();
            }
        }

        static ConnectionFactory setupConnection()
        {
            var factory = new ConnectionFactory();
            factory.HostName = "localhost";
     
            //Setup Recovery options
            factory.AutomaticRecoveryEnabled = true;
            factory.TopologyRecoveryEnabled = true;

            //From the standard documentation
            factory.RequestedHeartbeat = 60;

            //Randomize the connection interval, so multiple connections
            // do not try to reconnect at the same time.
            Random rnd = new Random();
            factory.NetworkRecoveryInterval = TimeSpan.FromSeconds(rnd.Next(15, 45));
    

            return factory;
        }
    }
}
