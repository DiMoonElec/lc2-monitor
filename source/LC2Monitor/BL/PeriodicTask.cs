using System;
using System.Threading;
using System.Threading.Tasks;

namespace LC2Monitor.BL
{
  public class PeriodicTask : IDisposable
  {
    private readonly Action _action;
    private readonly int _periodMs;
    private CancellationTokenSource _cts;
    private Task _task;

    public event Action<PeriodicTask, Exception> OnError;

    public PeriodicTask(Action action, int periodMs)
    {
      _action = action ?? throw new ArgumentNullException(nameof(action));
      if (periodMs <= 0) throw new ArgumentOutOfRangeException(nameof(periodMs));
      _periodMs = periodMs;
    }

    public void Start()
    {
      if (_cts != null)
        return; // уже запущен

      _cts = new CancellationTokenSource();
      var token = _cts.Token;

      _task = Task.Run(async () =>
      {
        try
        {
          while (!token.IsCancellationRequested)
          {
            try
            {
              _action();
            }
            catch (Exception ex)
            {
              // Сообщаем пользователю
              OnError?.Invoke(this, ex);
            }

            await Task.Delay(_periodMs, token).ConfigureAwait(false);
          }
        }
        catch (OperationCanceledException)
        {
          // Нормальное завершение
        }
      }, token);
    }

    public void Stop()
    {
      if (_cts == null)
        return;

      _cts.Cancel();
      try
      {
        _task?.Wait();
      }
      catch (AggregateException ex) when (ex.InnerException is OperationCanceledException)
      {
        // игнорируем отмену
      }

      _cts.Dispose();
      _cts = null;
      _task = null;
    }

    public void Dispose()
    {
      Stop();
    }
  }
}
