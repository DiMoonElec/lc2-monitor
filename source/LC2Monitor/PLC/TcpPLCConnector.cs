using System;
using System.Net.Sockets;
using System.Threading;
using DiMoon.Protocols;

namespace LC2Monitor
{
  internal class TcpPLCConnector : IPLCConnector
  {
    private TcpClient tcpClient;
    private NetworkStream networkStream;
    private readonly BinexLibReceiver binexLibReceiver = new BinexLibReceiver(512);
    private readonly BinexLibTransmitter binexLibTransmitter = new BinexLibTransmitter();
    private Thread receiveThread;
    private volatile bool running;

    public void Connect(string ipAddress, int port)
    {
      tcpClient = new TcpClient();
      tcpClient.Connect(ipAddress, port);
      networkStream = tcpClient.GetStream();

      binexLibReceiver.Reset();

      running = true;
      receiveThread = new Thread(ReceiveLoop)
      {
        IsBackground = true
      };
      receiveThread.Start();
    }

    public void Close()
    {
      running = false;

      try
      {
        networkStream?.Close();
        tcpClient?.Close();
      }
      catch { }

      networkStream = null;
      tcpClient = null;
    }

    public bool IsOpen()
    {
      return tcpClient?.Connected == true;
    }

    public void Write(byte[] data)
    {
      if (!IsOpen())
        return;

      var pack = binexLibTransmitter.BuildPackage(data);
      try
      {
        networkStream.Write(pack, 0, pack.Length);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Write Error]: {ex.Message}");
      }
    }

    public event EventHandler<byte[]> DataReceived;

    private void ReceiveLoop()
    {
      var buffer = new byte[1024];

      try
      {
        while (running && IsOpen())
        {
          if (!networkStream.DataAvailable)
          {
            Thread.Sleep(10);
            continue;
          }

          int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
          for (int i = 0; i < bytesRead; i++)
          {
            if (binexLibReceiver.Input(buffer[i]))
            {
              var receivedData = binexLibReceiver.GetReceiveData();
              DataReceived?.Invoke(this, receivedData);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"[Receive Error]: {ex.Message}");
      }
    }
  }
}
