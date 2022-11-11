﻿// See https://aka.ms/new-console-template for more information
using Confluent.Kafka;
using Serilog;

var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
logger.Information("Iniciando consumer");


string bootstrapServers = "172.31.13.233:9092";
string nomeTopic = "KAFKA_EXAMPLE";

logger.Information($"BootstrapServers = {bootstrapServers}");
logger.Information($"Topic = {nomeTopic}");

var config = new ConsumerConfig
{
    BootstrapServers = bootstrapServers,
    GroupId = $"{nomeTopic}-group-0",
    AutoOffsetReset = AutoOffsetReset.Earliest
};

CancellationTokenSource cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
    {
        consumer.Subscribe(nomeTopic);

        try
        {
            while (true)
            {
                var cr = consumer.Consume(cts.Token);
                logger.Information(
                    $"Mensagem lida: {cr.Message.Value}");
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
            logger.Warning("Cancelada a execução do Consumer...");
        }
    }
}
catch (Exception ex)
{
    logger.Error($"Exceção: {ex.GetType().FullName} | " +
                 $"Mensagem: {ex.Message}");
}