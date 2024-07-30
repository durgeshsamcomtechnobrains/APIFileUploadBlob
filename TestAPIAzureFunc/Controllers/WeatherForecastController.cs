using Azure.Storage.Queues; // Using directives for Azure Queue Storage and ASP.NET Core MVC
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace TestAPIAzureFunc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        // A static array of weather summaries
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        // HTTP POST endpoint that accepts a WeatherForecast object in the request body
        //Azure Queue Storage
        [HttpPost]
        public async Task Post([FromBody] WeatherForecast weatherForecast)
        {            
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=scstorage090724;AccountKey=e62bjsv7s3aCj3kkY0ijZUrjbe+sheyXiEXRbk/111Tq2pw9wfNzBCHN08pYqS3wswNWxKO/Nrkv+AStVauwvA==;EndpointSuffix=core.windows.net"; // Connection string for the Azure Queue Storage
            string queueName = "testqueue"; // Name of the queue

            var queueClient = new QueueClient(connectionString, queueName); // Creates a new QueueClient instance to interact with the specified queue
             
            string jsonDataWhetherForeact = JsonSerializer.Serialize(weatherForecast); // Serializes the WeatherForecast object to a JSON string

            await queueClient.SendMessageAsync(jsonDataWhetherForeact);   // Sends the JSON string as a message to the queue asynchronously

        }
    }
}
