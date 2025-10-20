using System.Diagnostics;
using Medo;

namespace Luc.Embeddings.Test;

public class Simulador03Principal (
 int recordsToBeInserted = 10_000_000
)
{
  private int statQtThreads = 0;
  private int statQtRecords = 0;
  private int statLastQtRecords = 0;
  private Stopwatch statWatch = Stopwatch.StartNew();  
  

  private async Task ThreadUsuario()
  {
    try
    {

      int threadId = Interlocked.Increment(ref statQtThreads);
      //Log.Info($"[Simulacao Principal] Iniciando thread {threadId}");

      var rand = new Random();
      using var db = await DbMain.New();

      await Task.Delay(1000);
      
      while (statQtRecords < recordsToBeInserted)
      {
        for (int i = 0; i < 100; i++)
        {
          var embedding = $"[{string.Join(",", Embeddings.RandomEmbedding())}]";

          int similares = await db.SelectInt("select 1 from sim01.clientes order by embedding <-> @embedding::vector limit 1", new Dictionary<string, object>{
            {"@embedding", embedding}
          });

          //if( similares == 0 )
          //{
          //  var uuid = Uuid7.NewGuid();          
          //  await db.ExecuteQuery("insert into sim01.clientes (id, embedding) values (@id, @embedding)", new Dictionary<string, object>{
          //    {"@id", uuid},
          //    {"@embedding", embedding}
          //  });
          //}          
        }
        
        Interlocked.Add(ref statQtRecords, 100);
      }
    }
    catch (Exception ex)
    {
      Log.Info($"[Simulacao Principal] Erro! {ex}");
    }
  }

  private async Task MonitorTask()
  {
    while( statQtRecords < recordsToBeInserted )
    {
      var rps_mm = -1.0;
      var deltaTime = statWatch.ElapsedMilliseconds;
      var deltaRecords = statQtRecords - statLastQtRecords;
      if (deltaTime > 0)
      {
        statWatch.Restart();
        var rps = (deltaRecords * 1000.0) / deltaTime;

        if( rps_mm > 0 || rps > 0 )
          rps_mm = rps_mm < 0 ? rps : (rps_mm * 0.8) + (rps * 0.2);        
        statLastQtRecords = statQtRecords;

        Log.Info($"[Simulacao Principal] {statQtRecords}/{recordsToBeInserted} (tps: {rps_mm:N2}, threads: {statQtThreads})");
      }
      await Task.Delay(5000);
    }
  }


  public async Task Executa()
  {    
    Log.Info($"[Simulacao Principal] Iniciando...");
    List<Task> tasks = [];
    for (int i = 0; i < 50; i++)
    {
      tasks.Add(ThreadUsuario());
    }
    tasks.Add(MonitorTask());
    Log.Info($"[Simulacao Principal] Iniciado...");
    await Task.WhenAll(tasks);
    Log.Info($"[Simulacao Principal] Finalizado.");
  }
}