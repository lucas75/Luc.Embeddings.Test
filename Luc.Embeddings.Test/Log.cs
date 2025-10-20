namespace Luc.Embeddings.Test;

class Log
{
  private static readonly string LogFilePath = $"{DateTime.Now:yyyyMMdd_HHmmss}_log.txt";
  private static readonly object LockObject = new();

  public Log()
  {
    // Ensure the log file is created or cleared at the start
    lock (LockObject)
    {
      File.WriteAllText(LogFilePath, string.Empty);
    }
  }

  public static void Info(string message)
  {
    string logMessage = $"[INFO ] {DateTimeOffset.Now:yyyy-MM-dd'T'HH:mm:sszzz} {message}";    
    WriteLog(logMessage);
  }

  public static void Error(string message)
  {
    string logMessage = $"[ERROR] {DateTimeOffset.Now:yyyy-MM-dd'T'HH:mm:sszzz} {message}";
    WriteLog(logMessage);
  }

  private static void WriteLog(string message)
  {
    lock (LockObject)
    {
      Console.WriteLine(message);
      File.AppendAllText(LogFilePath, message + Environment.NewLine);
    }
  }
}