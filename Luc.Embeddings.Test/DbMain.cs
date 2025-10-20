using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace Luc.Embeddings.Test;

public interface IDb : IDisposable
{
  public ValueTask<int>  ExecuteQuery(string sql, Dictionary<string, object> parameters);
  public DataTable Select(string sql, Dictionary<string, object> parameters);
  public ValueTask<int> SelectInt(string sql, Dictionary<string, object> parameters);  
  public NpgsqlConnection GetConnection();
}

public class DbPostgres : IDb
{
  private readonly NpgsqlConnection _connection;


  public DbPostgres( NpgsqlConnection connection)
  {
    _connection = connection;
    _connection.Open();
  }

  public void Dispose()
  {
    GC.SuppressFinalize(this);
    _connection.Dispose();
  }

  public async ValueTask<int> ExecuteQuery(string sql, Dictionary<string, object> parameters)
  {
    using var command = new NpgsqlCommand(sql, _connection);
    await command.PrepareAsync();
    foreach (var param in parameters)
    {
      command.Parameters.AddWithValue(param.Key, param.Value);
    } 
    return await command.ExecuteNonQueryAsync();
  }

  public DataTable Select(string sql, Dictionary<string, object> parameters)
  {
    using var command = new NpgsqlCommand(sql, _connection);
    foreach (var param in parameters)
    {
      command.Parameters.AddWithValue(param.Key, param.Value);
    }
    using var reader = command.ExecuteReader();
    var dataTable = new DataTable();
    dataTable.Load(reader);
    return dataTable;
  }

  public async ValueTask<int> SelectInt(string sql, Dictionary<string, object> parameters)
  {
    using var command = new NpgsqlCommand(sql, _connection);
    foreach (var param in parameters)
    {
      command.Parameters.AddWithValue(param.Key, param.Value);
    }
    var result = command.ExecuteScalar();
    return Convert.ToInt32(result);
  }

  public NpgsqlConnection GetConnection()
  {
    return _connection;
  }
}


public static class DbMain
{
  private static readonly string _connectionString =
    "Host=localhost;" +
    "Username=lucas;" +
    "Password=;" +
    "Database=test_vector;" +
    "Port=5432;" +
    "Pooling=true;" +
    "Minimum Pool Size=50;" +
    "Maximum Pool Size=250;" +
    "Timeout=300;" +
    "CommandTimeout=300";

  public static async ValueTask<DbPostgres> New()
  {
    var connection = new NpgsqlConnection(_connectionString);    
    return new DbPostgres(connection);
  }
}
