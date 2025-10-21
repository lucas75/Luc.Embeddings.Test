# TESTE DE EMBEDDINGS

Esse projeto é um teste simples de indexação de embeddings no PostgreSQL

Um dos resultados da execução pode ser encontrado em:

* [20251019_102604_log.txt](20251019_102604_log.txt) em que foi usado um indexador ivfflat com embeddings de 256 dimensões.

* [20251019_102604_log.txt](20251019_102604_log.txt) em que foi usado um indexador hnsw com embeddings de 256 dimensões.

* [20251020_080416_log.txt](20251020_080416_log.txt) e [20251020_120614_log.txt](20251020_120614_log.txt) em que foi usado um indexador hnsw com embeddings de 128 dimensões.

A máquina utilizada nos testes foi um Ryzen 9 5900HX com a frequência limitada a 3GHz e SMT desabilitado (Eu costumo reduzir a frequencia de minhas CPU para evitar a ativação da ventoinha. Uma maquina como essa teria quase o dobro de performance sem esses limitadores).

CONCLUSÕES PRELIMINARES
=======================

O indexador ivfflat se mostrou insuficiente para o padrão de consultas aqui explorado. Meu entendimento é que o ivflat só serve para dados que tenham alguma clusterização natural. Quando os dados tem um espalhamento aleatório e um volume grande, a consulta se torna excessivamente lenta.

O indexador hnsw apresentou uma performance razoável até o limite testado. A máquina de testes é pequena, mas ainda assim foi possível inserir a uma velocidade de 400 registros por segundo enquanto a base cabia na RAM e 150 depois disso. Em menos de 24 horas foi possível inserir 16 milhões de registros.

Com embedings de 128, a performance se manteve acima de 900 registros por segundo enquanto a base cabia em RAM e 250 registros por segundo depois disso.

O teste ainda é pequeno para o padrão que estou trabalhando, mas já serviu para me dar uma direção incial.

PROXIMOS PASSOS
===============
* Simulador mais realista: criar uma função no banco para checar a colisão e trabalhar apenas com insert (sem copy).
* Simulação por mais tempo: eu quero chegar pelo menos em 100 milhões de registros.
* Avaliar possibilidades de sharding.

POSSIBILIDADES DE SHARDING
==========================
* https://milvus.io/docs/pt/quickstart.md que possui o indexador hnsw e autosharding
* https://qdrant.tech/ que possui o indexador hnsw e autosharding
