using System;
using System.Collections.Generic;
using System.Reflection;

namespace LC2Monitor
{
  public enum PLCStatus
  {
    Stop = 0,
    Run,
    Cycle,
    Pause,
    Exception,
    Disconnected
  }

  public enum PLCRunStatus
  {
    LaunchOk = 0,
    ContinueOk,
    InvalidOperationError,
    ProgramIsDamagedError,
  }

  public enum SaveProgramResult
  {
    OK,                        // Успешное завершение
    EmptyProgram,              // Ошибка: программа пуста
    ProgramTooLarge,           // Ошибка: программа превышает размер доступной области флеш
    FlashEraseFailed,          // Ошибка: сбой при очистке флеш
    FlashWriteFailed,          // Ошибка: сбой при записи флеш
    OperationNotAllowed,       // Ошибка: операция невозможна в текущем состоянии ПЛК
    UnknownError               // Неизвестная ошибка
  }

  public enum PLCCycleStatus
  {
    Ok = 0,
    InvalidOperationError,
    ProgramIsDamagedError,
  }

  public enum LCVMStack
  {
    Operation = (byte)0,
    Return = (byte)1,
  }

  public class PLCInfo
  {
    public string SerialNumber { get; private set; }
    public int MajorVersion { get; private set; }
    public int MinorVersion { get; private set; }
    public int PatchVersion { get; private set; }

    public PLCInfo(string serialNumber, int majorVersion, int minorVersion, int patchVersion)
    {
      SerialNumber = serialNumber;
      MajorVersion = majorVersion;
      MinorVersion = minorVersion;
      PatchVersion = patchVersion;
    }
  }

  internal class PLCClientRequests
  {
    public event Action<PLCStatus> PLCStatusChanged;
    public event Action ConnectionError;

    PLCClientRequestManager requestManager;
    public PLCClientRequests(PLCClientRequestManager serverRequestManager)
    {
      requestManager = serverRequestManager;
      requestManager.ConnectionError += () => ConnectionError?.Invoke();
      requestManager.OnEventReceived += RequestManager_OnEventReceived;
    }

    private void RequestManager_OnEventReceived(byte reqID, byte[] data)
    {
      switch (reqID)
      {
        case 0x10:
          OnEventStatusChanged(data);
          break;
      }
    }

    private void OnEventStatusChanged(byte[] data)
    {
      if (data.Length < 1)
        return;

      switch (data[0])
      {
        case 0:
          PLCStatusChanged?.Invoke(PLCStatus.Stop);
          break;

        case 1:
          PLCStatusChanged?.Invoke(PLCStatus.Run);
          break;

        case 2:
          PLCStatusChanged?.Invoke(PLCStatus.Cycle);
          break;

        case 3:
          PLCStatusChanged?.Invoke(PLCStatus.Pause);
          break;

        case 4:
          PLCStatusChanged?.Invoke(PLCStatus.Exception);
          break;

      }
    }

    public void Ping()
    {
      var resp = requestManager.SendRequest(new byte[] { 0x01 });
    }

    public PLCInfo GetInformation()
    {
      var resp = requestManager.SendRequest(new byte[] { 0x03 }); // CMD_GET_INFO

      if (resp.Length < 3)
        throw new InvalidOperationException("Invalud response from controller.");

      int major = resp[0];
      int minor = resp[1];
      int patch = resp[2];

      string serial;

      if (resp.Length <= 3)
      {
        serial = "MISSING"; // Серийный номер отсутствует
      }
      else
      {
        var raw = resp.AsSpan(3);
        serial = SafeUtf8Decode(raw);
      }

      return new PLCInfo(serial, major, minor, patch);
    }

    public PLCStatus GetStatus()
    {
      var resp = requestManager.SendRequest(new byte[] { 0x02 });



      switch (resp[0])
      {
        case 0:
          return PLCStatus.Stop;

        case 1:
          return PLCStatus.Run;

        case 2:
          return PLCStatus.Cycle;

        case 3:
          return PLCStatus.Pause;

        case 4:
          return PLCStatus.Exception;

        default:
          throw new Exception("Неизвестный ответ");
      }
    }

    public PLCRunStatus PLCRun()
    {
      var resp = requestManager.SendRequest(new byte[] { 0x04 });
      if (resp == null || resp.Length < 1)
        throw new InvalidOperationException("Invalud response from controller.");

      switch ((sbyte)resp[0])
      {
        case 0:
          return PLCRunStatus.LaunchOk;
        case 1:
          return PLCRunStatus.ContinueOk;

        case -1:
          return PLCRunStatus.ProgramIsDamagedError;
        case -2:
          return PLCRunStatus.InvalidOperationError;

        default:
          throw new InvalidOperationException("Invalud response from controller.");
      }
    }

    public void PLCStop()
    {
      var resp = requestManager.SendRequest(new byte[] { 0x05 });
    }

    public PLCCycleStatus PLCCycle()
    {
      var resp = requestManager.SendRequest(new byte[] { 0x06 });
      if (resp == null || resp.Length < 1)
        throw new InvalidOperationException("Invalud response from controller.");

      switch ((sbyte)resp[0])
      {
        case 0:
          return PLCCycleStatus.Ok;
        case -1:
          return PLCCycleStatus.ProgramIsDamagedError;
        case -2:
          return PLCCycleStatus.InvalidOperationError;

        default:
          throw new InvalidOperationException("Invalud response from controller.");
      }
    }

    public byte[] PLCGetMemory(uint StartAddress, uint Size)
    {
      List<byte> req = new List<byte>();
      req.Add(0x15); //CMD_GET_MEMORY

      //Участок памяти
      req.AddRange(BitConverter.GetBytes(StartAddress));
      req.AddRange(BitConverter.GetBytes(Size));

      var resp = requestManager.SendRequest(req.ToArray());
      return resp; //Возвращаем дамп
    }

    public void PLCSetMemory(uint StartAddress, byte[] Dump)
    {
      List<byte> req = new List<byte>();
      req.Add(0x16); //CMD_SET_MEMORY

      //Участок памяти
      req.AddRange(BitConverter.GetBytes(StartAddress));
      req.AddRange(BitConverter.GetBytes(Dump.Length));

      foreach (byte b in Dump)
        req.Add(b);

      var resp = requestManager.SendRequest(req.ToArray());
    }

    public void PLCUploadApplication(byte[] app)
    {
      const byte CMD_UPLOAD_BEGIN = 0x17; // Код команды для начала загрузки
      const byte CMD_UPLOAD_CHUNK = 0x18; // Код команды для загрузки чанка
      const byte CMD_UPLOAD_END = 0x19;   // Код команды для завершения загрузки
      const int CHUNK_SIZE = 128;

      // Проверяем входные данные
      if (app == null || app.Length == 0)
        throw new ArgumentException("Application data cannot be null or empty.");

      // Отправка команды начала загрузки
      requestManager.SendRequest(new[] { CMD_UPLOAD_BEGIN });

      int totalChunks = (app.Length + CHUNK_SIZE - 1) / CHUNK_SIZE; // Округление вверх

      for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
      {
        uint startAddress = (uint)(chunkIndex * CHUNK_SIZE);
        int chunkLength = Math.Min(CHUNK_SIZE, app.Length - chunkIndex * CHUNK_SIZE);
        byte[] chunkData = new byte[chunkLength];

        Array.Copy(app, chunkIndex * CHUNK_SIZE, chunkData, 0, chunkLength);

        // Формируем запрос
        List<byte> request = new List<byte>();
        request.Add(CMD_UPLOAD_CHUNK); // Код команды
        request.AddRange(BitConverter.GetBytes(startAddress)); // Стартовый адрес
        request.AddRange(BitConverter.GetBytes(chunkLength));  // Длина чанка
        request.AddRange(chunkData);                          // Данные чанка

        // Отправляем запрос и получаем ответ
        byte[] response = requestManager.SendRequest(request.ToArray(), -1);

        // Проверяем ответ
        if (response == null || response.Length == 0)
          throw new InvalidOperationException("Empty response from controller.");

        byte status = response[0]; // Первый байт ответа

        switch (status)
        {
          case 0x00: // Успешное выполнение
            continue;

          case 0x01: // Несуществующая область
            throw new InvalidOperationException($"Invalid memory address: 0x{startAddress:X8}.");

          case 0x02: // Неверное состояние контроллера
            throw new InvalidOperationException("Controller is in an invalid state to perform this operation.");

          default:
            throw new InvalidOperationException($"Unknown response status: 0x{status:X2}.");
        }
      }

      // Отправка команды завершения загрузки
      requestManager.SendRequest(new byte[] { CMD_UPLOAD_END });
    }

    public byte[] PLCGetApplicationMemory(uint StartAddress, uint Size)
    {
      List<byte> req = new List<byte>();
      req.Add(0x1A); //CMD_GET_APP_MEMORY

      //Участок памяти
      req.AddRange(BitConverter.GetBytes(StartAddress));
      req.AddRange(BitConverter.GetBytes(Size));

      var resp = requestManager.SendRequest(req.ToArray());
      return resp; //Возвращаем дамп
    }

    public void GetRegisterValue(out uint PC, out uint FP,
      out short OperationSP, out short ReturnSP, out byte Except)
    {
      List<byte> req = new List<byte>();
      req.Add(0x81); //CMD_GET_REGISTER

      var resp = requestManager.SendRequest(req.ToArray());

      if (resp == null || resp.Length < 13)
        throw new InvalidOperationException($"Invalid response");

      PC = BitConverter.ToUInt32(resp, 0);
      FP = BitConverter.ToUInt32(resp, 4);
      OperationSP = BitConverter.ToInt16(resp, 8);
      ReturnSP = BitConverter.ToInt16(resp, 10);
      Except = resp[12];
    }

    public uint[] GetStackValue(LCVMStack stack, int countElements)
    {
      const int maxChunkSize = 128; // Максимальный размер данных за один цикл обмена в байтах
      List<uint> result = new List<uint>();

      int elementSize = sizeof(uint);
      int maxElementsPerChunk = maxChunkSize / elementSize; // Максимальное число элементов в чанке
      int offset = 0;

      while (countElements > 0)
      {
        int elementsToRequest = Math.Min(countElements, maxElementsPerChunk);

        List<byte> req = new List<byte>();
        req.Add(0x82); // CMD_GET_STACK_VALUE
        req.Add((byte)stack);
        req.AddRange(BitConverter.GetBytes(offset)); // Смещение
        req.AddRange(BitConverter.GetBytes(elementsToRequest)); // Количество элементов

        var resp = requestManager.SendRequest(req.ToArray());
        if (resp == null || resp.Length < 1)
          throw new InvalidOperationException("Invalid response");

        if (resp[0] != 0x00)
          throw new InvalidOperationException("Invalid response");

        for (int i = 1; i < resp.Length; i += elementSize)
          result.Add(BitConverter.ToUInt32(resp, i));

        offset += elementsToRequest;
        countElements -= elementsToRequest;
      }

      return result.ToArray();
    }

    public void GetRuntimeMetrics(out int CyclePeriod, out int CycleDuration, out int CycleDurationMax)
    {
      List<byte> req = new List<byte>();
      req.Add(0x1B); //CMD_GET_RUNTIME_METRICS

      var resp = requestManager.SendRequest(req.ToArray());

      if (resp == null || resp.Length < 12)
        throw new InvalidOperationException($"Invalid response");

      CyclePeriod = BitConverter.ToInt32(resp, 0);
      CycleDuration = BitConverter.ToInt32(resp, 4);
      CycleDurationMax = BitConverter.ToInt32(resp, 8);
    }

    public void SetRTCTimestamp(uint value)
    {
      List<byte> req = new List<byte>();
      req.Add(0x1C); //CMD_SET_RTC_TIMESTAMP
      req.AddRange(BitConverter.GetBytes(value));

      var resp = requestManager.SendRequest(req.ToArray());

      if (resp == null || resp.Length < 1)
        throw new InvalidOperationException("Invalud response from controller.");
    }

    public SaveProgramResult SaveProgramToFlash()
    {
      List<byte> req = new List<byte>();
      req.Add(0x1F); // CMD_SAVE_PROGRAM_TO_FLASH

      var resp = requestManager.SendRequest(req.ToArray());

      if (resp == null || resp.Length < 1)
        throw new InvalidOperationException("Invalid response from controller.");

      switch ((sbyte)resp[0])
      {
        case 0:
          return SaveProgramResult.OK;

        case -1:
          return SaveProgramResult.EmptyProgram;

        case -2:
          return SaveProgramResult.ProgramTooLarge;

        case -4:
          return SaveProgramResult.FlashEraseFailed;

        case -5:
          return SaveProgramResult.FlashWriteFailed;

        case -6:
          return SaveProgramResult.OperationNotAllowed;

        default:
          return SaveProgramResult.UnknownError;
      }
    }

    private string SafeUtf8Decode(ReadOnlySpan<byte> data)
    {
      var sb = new System.Text.StringBuilder();

      var decoder = System.Text.Encoding.UTF8.GetDecoder();
      char[] charBuffer = new char[2];
      byte[] singleByte = new byte[1];

      int i = 0;
      while (i < data.Length)
      {
        // Пытаемся декодировать от текущей позиции
        int byteCount = 1;
        int charCount;

        try
        {
          int remaining = data.Length - i;
          if ((data[i] & 0x80) == 0x00)
          {
            // ASCII, 1 байт
            singleByte[0] = data[i];
            charCount = decoder.GetChars(singleByte, 0, 1, charBuffer, 0);
            sb.Append(charBuffer[0]);
            i += 1;
          }
          else if (remaining >= 2 && (data[i] & 0xE0) == 0xC0)
          {
            byteCount = 2;
          }
          else if (remaining >= 3 && (data[i] & 0xF0) == 0xE0)
          {
            byteCount = 3;
          }
          else if (remaining >= 4 && (data[i] & 0xF8) == 0xF0)
          {
            byteCount = 4;
          }

          if (byteCount > 1)
          {
            if (i + byteCount > data.Length)
            {
              // Неполный символ — выводим как hex
              sb.Append($"/{data[i]:X2}");
              i++;
              continue;
            }

            var segment = data.Slice(i, byteCount).ToArray();
            charCount = decoder.GetChars(segment, 0, byteCount, charBuffer, 0);

            if (charCount > 0)
            {
              sb.Append(charBuffer[0]);
            }

            i += byteCount;
          }
        }
        catch
        {
          // Некорректный байт — выводим его как /XX
          sb.Append($"/{data[i]:X2}");
          i++;
        }
      }

      return sb.ToString();
    }

  }
}
