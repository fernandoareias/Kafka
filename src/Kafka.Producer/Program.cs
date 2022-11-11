// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Serilog;

var _logger = new LoggerConfiguration()
                      .WriteTo.Console()
                      .CreateLogger();

try
{



  
    _logger.Information("Iniciando publisher...");

    var config = new ProducerConfig
    {
        BootstrapServers = "172.31.13.233:9092"
    };

    while (true)
    {
        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            var result = await producer.ProduceAsync(
                      "KAFKA_EXAMPLE",
                      new Message<Null, string>
                      { Value = "Hello Kafka :) " });

            _logger.Information(
                $"Mensagem: {"Hello Kafka :) "} | " +
                $"Status: {result.Status}");
        }

        Thread.Sleep(10);
    }

    _logger.Information("Concluído o envio de mensagens");
}
catch (Exception ex)
{
    _logger.Error($"Exceção: {ex.GetType().FullName} | " +
                 $"Mensagem: {ex.Message}");
}