using System.Diagnostics;
using Medo;

namespace Luc.Embeddings.Test;


public class Simulador02PopulaTabelas(
  int recordsToBeInserted = 10_000_000
)
{
  private int statQtThreads = 0;
  private int statQtRecords = 0;
  private int statLastQtRecords = 0;
  private Stopwatch statWatch = Stopwatch.StartNew();


  private async Task ThreadInsercao()
  {
    try
    {

      int threadId = Interlocked.Increment(ref statQtThreads);
      //Log.Info($"[Populando tabelas] Iniciando thread {threadId}");

      var rand = new Random();
      using var db = await DbMain.New();

      await Task.Delay(1000);

      while (statQtRecords < recordsToBeInserted)
      {
        using (var writer = await db.GetConnection().BeginTextImportAsync("copy sim01.clientes (id, embedding) from stdin (format text)"))
        {
          for (int i = 0; i < 1000; i++)
          {
            var uuid = Uuid7.NewGuid();
            var embedding = Embeddings.RandomEmbedding();
            await writer.WriteLineAsync($"{uuid}\t[" + string.Join(",", embedding) + "]");
          }
        }

        Interlocked.Add(ref statQtRecords, 1000);
      }
    }
    catch (Exception ex)
    {
      Log.Error($"[Populando tabelas] Erro! {ex}");
    }
  }

  private async Task MonitorTask()
  {
    var rps_mm = -1.0;
    while (statQtRecords < recordsToBeInserted)
    {

      var deltaTime = statWatch.ElapsedMilliseconds;
      var deltaRecords = statQtRecords - statLastQtRecords;
      if (deltaTime > 0)
      {
        statWatch.Restart();
        var rps = (deltaRecords * 1000.0) / deltaTime;
        statLastQtRecords = statQtRecords;

        if( rps_mm > 0 || rps > 0 )
          rps_mm = rps_mm < 0 ? rps : (rps_mm * 0.8) + (rps * 0.2);

        Log.Info($"[Populando tabelas] {statQtRecords}/{recordsToBeInserted} (tps: {rps_mm:N2}, threads: {statQtThreads})");
      }
      await Task.Delay(5000);
    }
  }


  public async Task Executa()
  {
    Log.Info($"[Populando tabelas] Iniciando...");
    using (var db = await DbMain.New())
    {
      statLastQtRecords = statQtRecords = await db.SelectInt("select count(1) from sim01.clientes", []);
    }

    List<Task> tasks = [];
    for (int i = 0; i < 10; i++)
    {
      tasks.Add(ThreadInsercao());
    }
    tasks.Add(MonitorTask());

    Log.Info($"[Populando tabelas] Iniciado...");
    await Task.WhenAll(tasks);

    Log.Info($"[Populando tabelas] Concluído.");
  }
}