using System;

namespace Luc.Embeddings.Test;

public static class Simulador01CriaTabelas
{
  public async static Task Executa()
  {
    using var db = await DbMain.New();

    Log.Info("[Cria Tabelas] Iniciando...");

    // cria uma tabela com coluna cpf, vetor 
    await db.ExecuteQuery(
      $"""
      drop schema if exists sim01 cascade;
      
      create schema sim01;
      
      create table sim01.clientes (
        id uuid not null primary key,
        embedding vector({Embeddings.Dimension}) not null
      );
      """,
      []
    );

    var index = "create index idx_clients_embedding on sim01.clientes using hnsw (embedding vector_l2_ops) with(m=24, ef_construction=48);";
    //var index = "create index idx_clients_embedding on sim01.clientes using ivfflat (embedding vector_l2_ops) with(lists=30000);";

    await db.ExecuteQuery(index, []);

    Log.Info( $"[Cria Tabelas] Índice: {index}" );

    Log.Info("[Cria Tabelas] OK");
  }
}
