using System;
using System.Collections.Generic;
using System.Xml;
using DebugViews.DataClasses;

namespace LC2Monitor.MISC
{
  internal static class MemoryAllocationInformationDeserializer
  {
    public static List<DataElementBase> Deserialize(string path)
    {
      var elements = new List<DataElementBase>();

      using (var reader = XmlReader.Create(path))
      {
        while (reader.Read())
        {
          if (reader.NodeType == XmlNodeType.Element)
          {
            var element = ParseElement(reader);
            if (element != null)
              elements.Add(element);
          }
        }
      }

      return elements;
    }

    private static DataElementBase ParseElement(XmlReader reader)
    {
      switch (reader.Name)
      {
        case "var": return ParsePrimitive(reader);
        case "array": return ParseArray(reader);
        case "struct": return ParseStruct(reader);
        case "module": return ParseModule(reader);
        default: return null;
      }
    }

    private static DataElementBase ParsePrimitive(XmlReader reader)
    {
      var type = reader.GetAttribute("type");
      var name = reader.GetAttribute("name");
      var address = int.Parse(reader.GetAttribute("address"));

      return CreateElement(type, name, address);
    }

    private static DataElementBase ParseArray(XmlReader reader)
    {
      var type = reader.GetAttribute("type");
      var name = reader.GetAttribute("name");
      var depth = int.Parse(reader.GetAttribute("depth"));
      var address = int.Parse(reader.GetAttribute("address"));

      // Основной DataElementUnion для всего массива
      var array = new DataElementUnion(name,
          string.Format("{0}[{1}]", type, depth.ToString()), depth);

      var elementSize = GetElementSize(type);

      // Если массив содержит больше 100 элементов, разбиваем его на части
      if (depth > 100)
      {
        int startElement = 0;
        int endElement = 99;  // Первая группа будет содержать элементы с индексами от 0 до 99

        while (startElement < depth)
        {
          // Ограничиваем endElement на последнюю позицию в массиве
          endElement = Math.Min(startElement + 99, depth - 1);

          // Создаем вложенный DataElementUnion для текущего участка массива
          var part = new DataElementUnion(
            string.Format("{0}[{1}-{2}]", name, startElement, endElement),
            string.Format("{0}[100]", type), depth);

          // Добавляем элементы в этот DataElementUnion
          for (int i = startElement; i <= endElement; i++)
          {
            var elementName = name + "[" + i + "]";
            var elementAddress = address + elementSize * i;
            part.Elements.Add(CreateElement(type, elementName, elementAddress));
          }

          // Добавляем этот участок массива в основной array
          array.Elements.Add(part);

          // Переходим к следующему участку
          startElement = endElement + 1;
        }
      }
      else
      {
        // Если размер массива не превышает 100, просто добавляем элементы
        for (int i = 0; i < depth; i++)
        {
          var elementName = name + "[" + i + "]";
          var elementAddress = address + elementSize * i;
          array.Elements.Add(CreateElement(type, elementName, elementAddress));
        }
      }

      return array;
    }


    /*
    private static DataElementBase ParseArray(XmlReader reader)
    {
      var type = reader.GetAttribute("type");
      var name = reader.GetAttribute("name");
      var depth = int.Parse(reader.GetAttribute("depth"));
      var address = int.Parse(reader.GetAttribute("address"));

      var array = new DataElementUnion(name,
        string.Format("{0}[{1}]", type, depth.ToString()), depth);

      var elementSize = GetElementSize(type);

      for (int i = 0; i < depth; i++)
      {
        var elementName = name + "[" + i + "]";
        var elementAddress = address + elementSize * i;
        array.Elements.Add(CreateElement(type, elementName, elementAddress));
      }

      return array;
    }
    */

    private static DataElementBase ParseStruct(XmlReader reader)
    {
      var name = reader.GetAttribute("name");
      var address = int.Parse(reader.GetAttribute("address"));

      var structElement = new DataElementUnion(name, "struct", address);

      if (reader.IsEmptyElement)
      {
        return structElement;
      }

      var depth = reader.Depth;
      while (reader.Read() && reader.Depth > depth)
      {
        var element = ParseElement(reader);
        if (element != null)
          structElement.Elements.Add(element);
      }

      return structElement;
    }

    private static DataElementBase ParseModule(XmlReader reader)
    {
      var name = reader.GetAttribute("name");

      var moduleElement = new DataElementUnion(name, "module", 0);

      if (reader.IsEmptyElement)
      {
        return moduleElement;
      }

      var depth = reader.Depth;
      while (reader.Read() && reader.Depth > depth)
      {
        var element = ParseElement(reader);
        if (element != null)
          moduleElement.Elements.Add(element);
      }

      return moduleElement;
    }


    private static DataElementBase CreateElement(string type, string name, int address)
    {
      switch (type)
      {
        case "sbyte": return new DataElementSByte(name, address);
        case "short": return new DataElementShort(name, address);
        case "int": return new DataElementInt(name, address);
        case "long": return new DataElementLong(name, address);
        case "byte": return new DataElementByte(name, address);
        case "ushort": return new DataElementUShort(name, address);
        case "uint": return new DataElementUInt(name, address);
        case "ulong": return new DataElementULong(name, address);
        case "float": return new DataElementFloat(name, address);
        case "double": return new DataElementDouble(name, address);
        case "bool": return new DataElementBool(name, address);
        default: throw new NotSupportedException("Unsupported type: " + type);
      }
    }

    private static int GetElementSize(string type)
    {
      switch (type)
      {
        case "sbyte": return DataElementSByte.Sizeof;
        case "short": return DataElementShort.Sizeof;
        case "int": return DataElementInt.Sizeof;
        case "long": return DataElementLong.Sizeof;
        case "byte": return DataElementByte.Sizeof;
        case "ushort": return DataElementUShort.Sizeof;
        case "uint": return DataElementUInt.Sizeof;
        case "ulong": return DataElementULong.Sizeof;
        case "float": return DataElementFloat.Sizeof;
        case "double": return DataElementDouble.Sizeof;
        case "bool": return DataElementBool.Sizeof;
        default: throw new NotSupportedException("Unsupported type: " + type);
      }
    }
  }
}
