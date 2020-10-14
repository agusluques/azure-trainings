using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Receiver
{
    class Program
    {
        const string ServiceBusConnectionString = "Endpoint=sb://svazuretraining.servicebus.windows.net/;SharedAccessKeyName=svtopic1;SharedAccessKey=n8w8dYEOdav48c6K/g6cBOeWcqTMafbNXntA4PWRwx0=;";
        const string TopicName = "svtopic1";
        const string SubscriptionName = "svtopic1subscription";
        static ISubscriptionClient subscriptionClient;

        static void Main(string[] args)
        {
            subscriptionClient = new SubscriptionClient(ServiceBusConnectionString, TopicName, SubscriptionName);

            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        { 
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            MessageHandlerOptions messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether MessagePump should automatically complete the messages after returning from User Callback.
                // False below indicates the Complete will be handled by the User Callback as in `ProcessMessagesAsync` below.
                AutoComplete = false
            };


            subscriptionClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            Console.ReadKey();

            await subscriptionClient.CloseAsync();
        }

        static async Task ProcessMessagesAsync(Message msg, CancellationToken token)
        {
            Console.WriteLine($"Received message: {Encoding.UTF8.GetString(msg.Body)}");

            // Complete the message so that it is not received again.
            // This can be done only if the subscriptionClient is created in ReceiveMode.PeekLock mode (which is the default).
            await subscriptionClient.CompleteAsync(msg.SystemProperties.LockToken);
        }

        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
