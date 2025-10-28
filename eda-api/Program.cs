using KafkaFlow;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// KafkaFlow
builder.Services.AddKafka(kafka => {
  kafka.AddCluster(cluster => {
    cluster.WithBrokers(["kafka-14f487f0-fredrkl-0955.k.aivencloud.com:14350"]);
    cluster.WithSecurityInformation(security => {
      security.SecurityProtocol = KafkaFlow.Configuration.SecurityProtocol.Ssl;
      security.SslCaPem = builder.Configuration["KAFKA_CA_CERTIFICATE"];
      security.SslCertificatePem = builder.Configuration["KAFKA_ACCESS_CERTIFICATE"];
      security.SslKeyPem = builder.Configuration["KAFKA_ACCESS_KEY"];
    });
  });
});

//        return services.AddKafka(kafka => kafka
//            .UseMicrosoftLog()
//            .AddCluster(cluster => cluster
//                .WithBrokers([kafkaConfiguration.ServiceUri])
//                .WithSecurityInformation(h =>
//                {
//                    h.SecurityProtocol = SecurityProtocol.Ssl;
//                    h.SslCaPem = kafkaConfiguration.CaCertificate;
//                    h.SslCertificatePem = kafkaConfiguration.AccessCertificate;
//                    h.SslKeyPem = kafkaConfiguration.AccessKey;
//                })
//                .WithSchemaRegistry(a =>
//                {
//                    a.Url = kafkaConfiguration.SchemaUrl;
//                    a.BasicAuthUserInfo = kafkaConfiguration.SchemaBasicAuth;
//                })
//                .AddProducer(Constants.ProducerName, kafkaConfiguration.Topic)));


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

app.MapGet("/weatherforecast", () =>
{
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

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
