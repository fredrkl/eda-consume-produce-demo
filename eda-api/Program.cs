using eda_api.events;
using KafkaFlow;
using KafkaFlow.Admin.Dashboard;
using KafkaFlow.Producers;
using KafkaFlow.Serializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Console.WriteLine("ACCESS_KEY:{0}", builder.Configuration["ACCESS_KEY"]);
//Console.WriteLine("ACCESS_CERTIFICATE:{0}", builder.Configuration["ACCESS_CERTIFICATE"]);
//Console.WriteLine("CA_CERTIFICATE:{0}", builder.Configuration["CA_CERTIFICATE"]);

builder.Services.AddKafka(kafka => {
  kafka.AddCluster(cluster => cluster
    .WithBrokers(["kafka-14f487f0-fredrkl-0955.k.aivencloud.com:14350"])
    .EnableAdminMessages("kafka-flow.admin")
    .EnableTelemetry("kafka-flow.admin")
    .WithSecurityInformation(security => {
      security.SecurityProtocol = KafkaFlow.Configuration.SecurityProtocol.Ssl;
      security.SslKeyPem = builder.Configuration["ACCESS_KEY"];
      security.SslCertificatePem = builder.Configuration["ACCESS_CERTIFICATE"];
      security.SslCaPem = builder.Configuration["CA_CERTIFICATE"];
      security.EnableSslCertificateVerification = true;
    })
    .AddProducer("producer-1", producer => {
      producer.AddMiddlewares(middlewares => {
        middlewares.AddSerializer<JsonCoreSerializer>();
      });
      producer.DefaultTopic("source-topic");
    })
    .AddProducer("destination", producer => producer
      .AddMiddlewares(middlewares => {
        middlewares.AddSerializer<JsonCoreSerializer>();
      })
      .DefaultTopic("destination-topic")
      .WithAcks(Acks.All)
      //.WithProducerConfig(new Confluent.Kafka.ProducerConfig(new Dictionary<string, string>() {
      //  { "enable.idempotence", "true" },
      //  { "acks", "all" },
      //  { "transactional.id", "destination-tx-1" },  // required for transactions
      //  { "max.in.flight.requests.per.connection", "1" }, // ensures ordering
      //  { "retries", "3" },
      //  { "retry.backoff.ms", "100" }
      //}))
    )
    .AddConsumer(consumer => consumer
      .Topic("source-topic")
      .WithGroupId("source-group")
      .WithBufferSize(100)
      .WithWorkersCount(10)
      .AddMiddlewares(middlewares => middlewares
        .AddDeserializer<JsonCoreDeserializer>()
        .AddTypedHandlers(handlers => handlers
          .AddHandler<WeatherForecastHandler>())
        )
    )
  );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (IProducerAccessor producerAccessor) =>
{
  var producer = producerAccessor.GetProducer("producer-1");
  WeatherForecast forecast_to_kafka = new(DateOnly.FromDateTime(DateTime.Now), Random.Shared.Next(-20, 55), summaries[Random.Shared.Next(0,9)]);
  _ = await producer.ProduceAsync(null, forecast_to_kafka);

    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Kafka Flow
app.UseKafkaFlowDashboard();
var kafkaBus = app.Services.CreateKafkaBus();
await kafkaBus.StartAsync();

app.Lifetime.ApplicationStopping.Register(() =>
{
    kafkaBus.StopAsync().GetAwaiter().GetResult();
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
