using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using Serilog;

namespace LC2Monitor
{
  internal class PLCClientRequestManager
  {
    private readonly IPLCConnector server;
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private byte RequestId = 0;
    private AutoResetEvent ResponseEvent = new AutoResetEvent(false);
    private byte[] ResponseBuffer = null;
    private readonly object sendLock = new object();

    public event Action<byte, byte[]> OnEventReceived;
    public event Action ConnectionError;

    public PLCClientRequestManager(IPLCConnector server)
    {
      this.server = server;
      this.server.DataReceived += Server_DataReceived;
    }

    public byte[] SendRequest(byte[] data, int timeoutMs = 500)
    {
      lock (sendLock)
      {
        RequestId++;

        byte[] requestPacket = CreateRequestPacket(RequestId, data);

        ResponseEvent.Reset();

        // Создаем экземпляр Stopwatch
        Stopwatch stopwatch = Stopwatch.StartNew();

        Log.Verbose("(PLCClient) Req: {0}", BitConverter.ToString(requestPacket));
        server.Write(requestPacket);

        if (ResponseEvent.WaitOne(timeoutMs) == false)
        {
          string error = "The request timed out or failed.";
          Log.Error("(PLCClient) Communication error: '{0}'", error);
          ConnectionError?.Invoke();
          throw new TimeoutException(error);
        }
        // Останавливаем Stopwatch
        stopwatch.Stop();

        // Выводим время выполнения
        Log.Verbose("(PLCClient) Elapsed time: {0} ms", stopwatch.ElapsedMilliseconds.ToString());

        if (ResponseBuffer.Length < 2)
        {
          string error = "ResponseBuffer is too short";
          Log.Error("(PLCClient) Communication error: '{0}'", error);
          throw new Exception(error);
        }
        if (ResponseBuffer[1] != RequestId)
        {
          string error = "Invalid response id from remote device";
          Log.Error("(PLCClient) Communication error: '{0}'", error);
          throw new Exception(error);
        }

        var ret = SubArray(ResponseBuffer, 2, ResponseBuffer.Length - 2);
        return ret;
      }
    }

    private void Server_DataReceived(object sender, byte[] packet)
    {
      if (packet.Length < 2)
        return;

      byte packetType = packet[0];

      if (packetType == 0x80) // Event packet
      {
        Log.Verbose("(PLCClient) Event: {0}", BitConverter.ToString(packet));

        byte eventId = packet[1];
        byte[] eventData = packet.Length > 2 ? SubArray(packet, 2, packet.Length - 2) : new byte[0];
        OnEventReceived?.Invoke(eventId, eventData);
      }
      else if (packetType == 0x01) // Response packet
      {
        Log.Verbose("(PLCClient) Resp: {0}", BitConverter.ToString(packet));

        int requestId = packet[1];

        if (requestId == RequestId)
        {
          /*ResponseBuffer = new byte[packet.Length - 2];
          packet.CopyTo(ResponseBuffer, 2);*/
          //ResponseBuffer = SubArray(packet, 2, packet.Length - 2);
          ResponseBuffer = packet;
          ResponseEvent.Set();
        }
      }
    }

    private byte[] CreateRequestPacket(byte requestId, byte[] data)
    {
      byte[] packet = new byte[1 + data.Length];
      packet[0] = (byte)requestId;
      Array.Copy(data, 0, packet, 1, data.Length);
      return packet;
    }

    private byte[] SubArray(byte[] data, int index, int length)
    {
      byte[] result = new byte[length];
      Array.Copy(data, index, result, 0, length);
      return result;
    }

    public void Dispose()
    {
      cancellationTokenSource.Cancel();
      cancellationTokenSource.Dispose();
    }
  }
}
