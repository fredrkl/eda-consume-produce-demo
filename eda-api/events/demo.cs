using KafkaFlow;
using KafkaFlow.Producers;

namespace eda_api.events
{
  public class WeatherForecastHandler(IProducerAccessor producerAccessor) : IMessageHandler<WeatherForecast>
  {
    Task IMessageHandler<WeatherForecast>.Handle(IMessageContext context, WeatherForecast message)
    {
      var producer = producerAccessor.GetProducer("destination");
      Console.WriteLine($"Received message: {message.Date} - {message.Summary} - {message.TemperatureC}C");
      producer.ProduceAsync(null, message);

      // this is where I should test for exceptions and transactions
      return Task.CompletedTask;
    }
  }
}
