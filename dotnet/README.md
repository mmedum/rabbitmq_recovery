# Setup

Use the official RabbitMQ docker container and start a queue by running

~~~~
docker run -d --hostname my-rabbit --name some-rabbit -p 8080:15672 -p 5672:5672 rabbitmq:3-management
~~~~

At the moment the publisher owns the queue, by starting it the queues will be
created and two topics defined with their own routing keys.
