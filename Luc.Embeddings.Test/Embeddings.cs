namespace Luc.Embeddings.Test;

public class Embeddings
{
  public static int Dimension { get; } = 256;

  /// <summary>
  /// Generate a random embedding vector
  /// </summary>
  public static double[] RandomEmbedding(double length = 10.0)
  {
    var rand = new Random();
    var embedding = new double[Dimension];
    for (int i = 0; i < Dimension; i++)
    {
      embedding[i] = rand.NextDouble() * length;
    }
    return embedding;
  }

  /// <summary>
  /// Add two vectors
  /// </summary>
  public static double[] Add(double[] a, double[] b)
  {
    if (a.Length != b.Length)
      throw new ArgumentException("Vectors must be of same length");

    var result = new double[a.Length];
    for (int i = 0; i < a.Length; i++)
    {
      result[i] = a[i] + b[i];
    }
    return result;
  }


  /// <summary>
  /// Generate an infinite sequence of random embeddings
  /// </summary>
  public static IEnumerable<double[]> RandomEmbeddings(double scale = 10.0, int count = int.MaxValue)
  {
    var rand = new Random();
    for (int n = 0; n < count; n++)
    {        
        yield return RandomEmbedding(scale);
    }
  }

}

