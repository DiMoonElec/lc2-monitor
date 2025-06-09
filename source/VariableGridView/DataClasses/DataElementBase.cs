using System;
using System.Linq;

namespace DebugViews.DataClasses
{
  public abstract class DataElementBase
  {
    internal string _displayName = null;

    public int IndentLevel { get; set; } // Новый уровень вложенности

    public abstract bool Watch { get; set; }
    public abstract byte[] Bytes { get; }

    public virtual bool ValueChanged { get; set; }

    /// <summary>
    /// Имя переменной
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Тип переменной
    /// </summary>
    public string Type { get; }

    /// <summary>
    /// Текстовое представление значения
    /// </summary>
    public abstract string Value { get; set; }

    /// <summary>
    /// Размер объекта
    /// </summary>
    public int Size { get; internal set; }

    /// <summary>
    /// Адрес переменной
    /// </summary>
    public int Address { get; internal set; }

    public virtual string DisplayName { get; set; }

    protected DataElementBase(string name, string type, int address, int size)
    {
      Name = name;
      Type = type;
      Address = address;
      Size = size;
      DisplayName = name;
    }

    public abstract void SetValue(string value);

    public virtual void UpdateValue(VariablesDump dump)
    {
      if (Watch == false)
      {
        ValueChanged = false;
        return;
      }

      // Получить релевантные данные из дампа
      var dataChunk = GetRelevantData(dump);
      if (dataChunk != null)
      {
        // Обновить значение с использованием данных из дампа
        UpdateValueFromData(dataChunk);
      }
      else
        ValueChanged = false;
    }

    /// <summary>
    /// Получить часть дампа, относящуюся к адресу переменной.
    /// </summary>
    protected byte[] GetRelevantData(VariablesDump dump)
    {
      if(dump ==  null)
        return null;

      foreach (var chunk in dump.Chunks)
      {
        if (chunk.Start <= Address && Address + Size <= chunk.Start + chunk.Data.Length)
        {
          // Извлечь только нужные байты для переменной
          int offset = Address - chunk.Start;
          return chunk.Data.Skip(offset).Take(Size).ToArray();
        }
      }
      return null; // Нужные данные не найдены
    }


    /// <summary>
    /// Обновить значение переменной из массива данных.
    /// Реализуется в дочерних классах.
    /// </summary>
    protected abstract void UpdateValueFromData(byte[] data);
  }

}
