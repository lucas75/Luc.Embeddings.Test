using System;
using Luc.Embeddings.Test;
using Npgsql;





ThreadPool.SetMinThreads(16, 16);
ThreadPool.SetMaxThreads(256, 256);


await Simulador01CriaTabelas.Executa();

await new Simulador02PopulaTabelas(recordsToBeInserted: 50_000).Executa();
await new Simulador03Principal(recordsToBeInserted: 100_000).Executa();

await new Simulador02PopulaTabelas(recordsToBeInserted: 500_000).Executa();
await new Simulador03Principal(recordsToBeInserted: 100_000).Executa();

await new Simulador02PopulaTabelas(recordsToBeInserted: 5_000_000).Executa();
await new Simulador03Principal(recordsToBeInserted: 100_000).Executa();

await new Simulador02PopulaTabelas(recordsToBeInserted: 20_000_000).Executa();
await new Simulador03Principal(recordsToBeInserted: 100_000).Executa();

await new Simulador02PopulaTabelas(recordsToBeInserted: 50_000_000).Executa();
await new Simulador03Principal(recordsToBeInserted: 100_000).Executa();

await new Simulador02PopulaTabelas(recordsToBeInserted: 50_000_000).Executa();
await new Simulador03Principal(recordsToBeInserted: 100_000).Executa();