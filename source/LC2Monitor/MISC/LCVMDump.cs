using System;
using System.Text;
using System.Xml;

namespace LC2Monitor.MISC
{
  public class LCVMDump
  {
    public uint RegisterPC { get; private set; }
    public short RegisterOperationSP { get; private set; }
    public short RegisterReturnSP { get; private set; }
    public uint RegisterFP { get; private set; }
    public byte ExceptionCode { get; private set; }
    public uint[] StackOperation { get; private set; }
    public uint[] StackReturn { get; private set; }
    public byte[] Memory { get; private set; }

    public LCVMDump(uint registerPC,
      short registerOperationSP,
      short registerReturnSP,
      uint registerFP,
      byte exceptionCode,
      uint[] stackOperation,
      uint[] stackReturn,
      byte[] memory)
    {
      RegisterPC = registerPC;
      RegisterOperationSP = registerOperationSP;
      RegisterReturnSP = registerReturnSP;
      RegisterFP = registerFP;
      ExceptionCode = exceptionCode;
      StackOperation = stackOperation;
      StackReturn = stackReturn;
      Memory = memory;
    }

    public LCVMDump(string filename)
    {
      if (string.IsNullOrWhiteSpace(filename))
        throw new ArgumentException("Filename cannot be null or empty.", nameof(filename));

      using (var reader = XmlReader.Create(filename))
      {
        reader.ReadToFollowing("LCVMDump");

        RegisterPC = uint.Parse(ReadElement(reader, "RegisterPC"));
        RegisterOperationSP = short.Parse(ReadElement(reader, "RegisterOperationSP"));
        RegisterReturnSP = short.Parse(ReadElement(reader, "RegisterReturnSP"));
        RegisterFP = uint.Parse(ReadElement(reader, "RegisterFP"));
        ExceptionCode = byte.Parse(ReadElement(reader, "ExceptionCode"));

        StackOperation = ReadUIntArray(reader, "StackOperation");
        StackReturn = ReadUIntArray(reader, "StackReturn");
        Memory = ReadByteArray(reader, "Memory");
      }
    }

    public void Save(string filename)
    {
      if (string.IsNullOrWhiteSpace(filename))
        throw new ArgumentException("Filename cannot be null or empty.", nameof(filename));

      using (var writer = XmlWriter.Create(filename, new XmlWriterSettings { Indent = true, Encoding = Encoding.UTF8 }))
      {
        writer.WriteStartDocument();
        writer.WriteStartElement("LCVMDump");

        WriteElement(writer, "RegisterPC", RegisterPC.ToString());
        WriteElement(writer, "RegisterOperationSP", RegisterOperationSP.ToString());
        WriteElement(writer, "RegisterReturnSP", RegisterReturnSP.ToString());
        WriteElement(writer, "RegisterFP", RegisterFP.ToString());
        WriteElement(writer, "ExceptionCode", ExceptionCode.ToString());

        WriteUIntArray(writer, "StackOperation", StackOperation);
        WriteUIntArray(writer, "StackReturn", StackReturn);
        WriteByteArray(writer, "Memory", Memory);

        writer.WriteEndElement();
        writer.WriteEndDocument();
      }
    }

    private static string ReadElement(XmlReader reader, string elementName)
    {
      if (reader.ReadToFollowing(elementName))
        return reader.ReadElementContentAsString();
      return null;
    }

    private static uint[] ReadUIntArray(XmlReader reader, string elementName)
    {
      if (reader.ReadToFollowing(elementName))
      {
        var content = reader.ReadElementContentAsString();
        if (!string.IsNullOrWhiteSpace(content))
        {
          var items = content.Split(',');
          var result = new uint[items.Length];
          for (int i = 0; i < items.Length; i++)
            result[i] = uint.Parse(items[i]);
          return result;
        }
      }
      return null;
    }

    private static byte[] ReadByteArray(XmlReader reader, string elementName)
    {
      if (reader.ReadToFollowing(elementName))
      {
        var content = reader.ReadElementContentAsString();
        if (!string.IsNullOrWhiteSpace(content))
        {
          var items = content.Split(',');
          var result = new byte[items.Length];
          for (int i = 0; i < items.Length; i++)
            result[i] = byte.Parse(items[i]);
          return result;
        }
      }
      return null;
    }

    private static void WriteElement(XmlWriter writer, string name, string value)
    {
      writer.WriteStartElement(name);
      writer.WriteString(value);
      writer.WriteEndElement();
    }

    private static void WriteUIntArray(XmlWriter writer, string name, uint[] array)
    {
      writer.WriteStartElement(name);
      if (array != null)
        writer.WriteString(string.Join(",", array));
      writer.WriteEndElement();
    }

    private static void WriteByteArray(XmlWriter writer, string name, byte[] array)
    {
      writer.WriteStartElement(name);
      if (array != null)
        writer.WriteString(string.Join(",", array));
      writer.WriteEndElement();
    }

    public override string ToString()
    {
      var sb = new StringBuilder();

      sb.AppendLine($"PC: 0x{RegisterPC.ToString("X8")}");
      sb.AppendLine($"OperationSP: {RegisterOperationSP}");
      sb.AppendLine($"ReturnSP: {RegisterReturnSP}");
      sb.AppendLine($"FP: {RegisterFP}");
      sb.AppendLine($"ExceptionCode: {ExceptionCode} ({GetExceptionDescription(ExceptionCode)})");

      if (StackOperation != null)
      {
        sb.AppendLine("StackOperation:");
        if (StackOperation.Length == 0)
          sb.AppendLine("  [Empty]");
        else
          for (int i = StackOperation.Length - 1; i >= 0; i--)
            sb.AppendLine($"  {StackOperation[i]}");
      }

      if (StackReturn != null)
      {
        sb.AppendLine("StackReturn:");
        if (StackReturn.Length == 0)
          sb.AppendLine("  [Empty]");
        else
          for (int i = StackReturn.Length - 1; i >= 0; i--)
            sb.AppendLine($"  {StackReturn[i]}");
      }

      return sb.ToString();
    }

    private static string GetExceptionDescription(int exceptionCode)
    {
      switch (exceptionCode)
      {
        case 0:
          return "No Exception: Execution completed successfully.";
        case 1:
          return "Out of Program: Program counter moved outside the valid range.";
        case 2:
          return "Out of Memory: Memory allocation exceeded available limits.";
        case 3:
          return "Operand Stack Overflow: Too many items pushed onto the operand stack.";
        case 4:
          return "Operand Stack Empty: Attempted to pop an item from an empty operand stack.";
        case 5:
          return "Return Stack Overflow: Too many return addresses pushed onto the return stack.";
        case 6:
          return "Return Stack Empty: Attempted to return from a function, but the return stack is empty.";
        case 7:
          return "Write Protection: Attempted to write to a protected memory region.";
        case 8:
          return "Memory Out of Range: Memory access exceeded allocated bounds.";
        case 9:
          return "ROM Out of Range: Attempted to access ROM outside the valid address range.";
        case 10:
          return "Buffer Out of Range: Buffer access exceeded its valid size.";
        case 11:
          return "Buffer Pointer Error: Buffer pointer is invalid or misaligned.";
        case 16:
          return "Unknown Instruction: Encountered an invalid or undefined instruction.";
        case 17:
          return "Invalid Syscall ID: Syscall ID is not recognized.";
        case 18:
          return "Invalid Syscall Parameter: Syscall received invalid parameters.";
        case 19:
          return "Invalid Instruction Parameter: Instruction parameters are incorrect or out of bounds.";
        case 128:
          return "Internal Error: Unexpected internal error occurred.";
        case 255:
          return "Halt: Program execution halted intentionally.";
        default:
          return "Unknown Exception: Exception code is not recognized.";
      }
    }


  }
}
