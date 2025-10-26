# Event Driven Architecture - Consume, process, and produce event pattern

This repository demonstrates the _Transactional Consume–Produce_ pattern using
an event-driven architecture. It showcases how to consume events from a source,
process them, and produce new events to a target system in a transactional
manner. The repo uses the [KafkaFlow](https://github.com/Farfetch/kafkaflow)
library to facilitate Kafka integration.

## API Produce event endpoint

```bash
mkdir eda-api
cd eda-api
dotnet new webapi
```

## Kafka cluster setup

I use Aiven to create a managed Kafka cluster at:
<https://console.aiven.io/account/a530d754cb24/project/fredrkl-0955/services/kafka-14f487f0/overview>.

## Resources

- <https://learn.microsoft.com/en-us/azure/architecture/guide/architecture-styles/event-driven>
- <https://farfetch.github.io/kafkaflow/>
