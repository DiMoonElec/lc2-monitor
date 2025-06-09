using System;
using System.Collections.Generic;
using System.Linq;
using DebugViews.DataClasses;

namespace LC2Monitor
{
  public class MemoryRequest
  {
    public int StartAddress { get; set; }
    public int Size { get; set; }

    public MemoryRequest(int startAddress, int size)
    {
      StartAddress = startAddress;
      Size = size;
    }
  }

  public static class DataElementHelper
  {
    public static List<MemoryRequest> BuildMemoryRequests(IEnumerable<DataElementBase> elements, int mergeThreshold = 4, int maxRequestSize = 128)
    {
      if (elements == null || !elements.Any())
        return new List<MemoryRequest>();

      // Сортировка элементов по начальному адресу
      var sortedElements = elements
          .OrderBy(e => e.Address)
          .Select(e => new MemoryRequest(e.Address, e.Size))
          .ToList();

      var result = new List<MemoryRequest>();

      // Объединение областей
      var current = sortedElements[0];
      for (int i = 1; i < sortedElements.Count; i++)
      {
        var next = sortedElements[i];

        // Если следующий элемент пересекается или находится близко к текущему
        if (next.StartAddress <= current.StartAddress + current.Size + mergeThreshold)
        {
          // Расширяем текущую область
          int newEnd = Math.Max(current.StartAddress + current.Size, next.StartAddress + next.Size);
          current.Size = newEnd - current.StartAddress;
        }
        else
        {
          // Разделяем текущую область, если она превышает maxRequestSize
          SplitAndAddMemoryRequest(result, current, maxRequestSize);
          current = next;
        }
      }

      // Добавляем последнюю область
      SplitAndAddMemoryRequest(result, current, maxRequestSize);

      return result;
    }

    private static void SplitAndAddMemoryRequest(List<MemoryRequest> result, MemoryRequest request, int maxRequestSize)
    {
      while (request.Size > maxRequestSize)
      {
        result.Add(new MemoryRequest(request.StartAddress, maxRequestSize));
        request.StartAddress += maxRequestSize;
        request.Size -= maxRequestSize;
      }

      if (request.Size > 0)
      {
        result.Add(request);
      }
    }
  }
}
