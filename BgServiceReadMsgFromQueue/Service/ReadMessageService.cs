using Azure.Storage.Queues;
using System.Text.Json;

namespace BgServiceReadMsgFromQueue.Service
{
    public class ReadMessageService : BackgroundService
    {
        public ILogger<WeatherForecast> logger { get; }
        public ReadMessageService(ILogger<WeatherForecast> logger)
        {

            this.logger = logger;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                this.logger.LogInformation("This is a test message from read Message Service...");

                string connectionString = "DefaultEndpointsProtocol=https;AccountName=scstorage090724;AccountKey=e62bjsv7s3aCj3kkY0ijZUrjbe+sheyXiEXRbk/111Tq2pw9wfNzBCHN08pYqS3wswNWxKO/Nrkv+AStVauwvA==;EndpointSuffix=core.windows.net"; // Connection string for the Azure Queue Storage
                string queueName = "testqueue"; // Name of the queue                

                var QueueClient = new QueueClient(connectionString,queueName);

                //read the message from the queue
                var message = await QueueClient.ReceiveMessageAsync(); //using FIFO manners


                //get the body of the message
                if(message != null && message.Value != null) //this just of handle object reference error
                {
                    var messageBody = message.Value.Body; //Read the message body
                    
                    var weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(messageBody); //Processing the message body

                    //await QueueClient.DeleteMessageAsync(message.Value.MessageId, message.Value.PopReceipt);

                    //this.logger.LogInformation("Delete message from the queue with messageId ::" + message.Value.MessageId);                    
                }                    

                await Task.Delay(TimeSpan.FromSeconds(5));                
            }            
        }

    }
}
