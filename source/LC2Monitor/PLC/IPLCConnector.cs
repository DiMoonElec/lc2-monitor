using System;

namespace LC2Monitor
{
  internal interface IPLCConnector
  {
    void Write(byte[] data);
    event EventHandler<byte[]> DataReceived;
  }
}
