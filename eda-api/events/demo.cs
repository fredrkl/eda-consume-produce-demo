using KafkaFlow;

namespace eda_api.events
{
    public class WeatherForecastHandler : IMessageHandler<WeatherForecast>
    {
        Task IMessageHandler<WeatherForecast>.Handle(IMessageContext context, WeatherForecast message)
        {
          Console.WriteLine($"Received message: {message.Date} - {message.Summary} - {message.TemperatureC}C");
          return Task.CompletedTask;
        }
    }
}
