using System;
using RabbitMQ.Client;
using System.Text;
using System.Threading;

namespace publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            Console.WriteLine("Starting publisher!");
            var factory = setupConnection();

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel()){
                // Queue
                //When declaring queues, remember to take into consideration
                //what kind of durability the queue should have and whether
                //or not it should be an exclusive queue.
                // channel.QueueDeclare(queue: "pingpong", 
                //                      durable: false,
                //                      exclusive: false,
                //                      autoDelete: false,
                //                      arguments: null);

                //Topic
                channel.ExchangeDeclare(exchange: "pingpong",
                                        type: "topic");
                
                while (true)
                {
                    string messageTopicA = Guid.NewGuid() + ", Hello TopicA!, " + DateTime.Now;
                    var bodyTopicA = Encoding.UTF8.GetBytes(messageTopicA);
                    string messageTopicB = Guid.NewGuid() + ", Hello TopicB!, " + DateTime.Now;
                    var bodyTopicB = Encoding.UTF8.GetBytes(messageTopicB);
                    try
                    {
                        //Queue
                        //For every queue, consider using a routing key to handled
                        //a request/respond pattern. This will use the same topic,
                        //but creating a specific routing on that topic.
                        // channel.BasicPublish(exchange: "",
                        //             routingKey: "sent",
                        //             basicProperties: null,
                        //             body: body);
                        //Consider doing publisher confirms, this will introduce overhead
                        //because it is needed to maintain a transactions, for further info
                        //see https://www.rabbitmq.com/confirms.html

                        
                        //Topic
                        channel.BasicPublish(exchange: "pingpong",
                                        routingKey: "topicA",
                                        basicProperties: null,
                                        body: bodyTopicA);

                        channel.BasicPublish(exchange: "pingpong",
                                        routingKey: "topicB",
                                        basicProperties: null,
                                        body: bodyTopicB);
                        
                        //Generating some random behavior
                        Console.WriteLine(" [x] Sent {0}", messageTopicA);
                        Console.WriteLine(" [y] Sent {0}", messageTopicB);
                        Thread.Sleep(rnd.Next(2000, 6000));
                    }
                    //Catch the generic exception, which is throw under network or rabbitmq
                    //changes. The exception contains a message, that sometimes can explain
                    //what kind of exception that has been thrown.
                    catch (RabbitMQ.Client.Exceptions.AlreadyClosedException)
                    {
                        Console.WriteLine("Connection is down, trying to reconnect.");
                    }
                   
                }
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
