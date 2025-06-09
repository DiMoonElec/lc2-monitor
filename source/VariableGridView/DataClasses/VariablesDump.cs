using System.Collections.Generic;

namespace DebugViews.DataClasses
{
  public class VariablesDump
  {
    /// <summary>
    /// Список чанков данных.
    /// </summary>
    public List<DataChunk> Chunks { get; } = new List<DataChunk>();

    /// <summary>
    /// Добавить чанк данных.
    /// </summary>
    public void AddChunk(byte[] data, int start)
    {
      Chunks.Add(new DataChunk(data, start));
    }

    /// <summary>
    /// Вложенный класс для хранения чанков данных.
    /// </summary>
    public class DataChunk
    {
      public byte[] Data { get; }
      public int Start { get; }

      public DataChunk(byte[] data, int start)
      {
        Data = data;
        Start = start;
      }
    }
  }
}
