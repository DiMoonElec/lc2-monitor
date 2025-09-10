using System;
using DebugViews.DataClasses;

namespace LC2Monitor.BL.Message
{
  internal abstract class BLMessage { }

  /// <summary>
  /// Выполнить подключение к контроллеру
  /// </summary>
  internal class ConnectUIMessage : BLMessage
  {
    public string Port { get; }

    public ConnectUIMessage(string port) { Port = port; }
  }

  /// <summary>
  /// Закрыть соединение с контроллером
  /// </summary>
  internal class CloseUIMessage : BLMessage { }

  /// <summary>
  /// Загрузить файл проекта
  /// </summary>
  internal class LoadProjectUIMessage : BLMessage
  {
    public string Path { get; }
    public LoadProjectUIMessage(string path) { Path = path; }
  }

  /// <summary>
  /// Загрузить исполняемый файл в контроллер
  /// </summary>
  internal class UploadExeFileUIMessage : BLMessage { }

  /// <summary>
  /// Запустить программу
  /// </summary>
  internal class RuntimeRunUIMessage : BLMessage { }

  /// <summary>
  /// Остановить программу
  /// </summary>
  internal class RuntimeStopUIMessage : BLMessage { }

  /// <summary>
  /// Выполниит один цикл программы
  /// </summary>
  internal class RuntimeCycleUIMessage : BLMessage { }

  /// <summary>
  /// Сохранить программу в энергонезависимую память контроллера
  /// </summary>
  internal class FlashExeUIMessage : BLMessage { }

  /// <summary>
  /// Установить значение RTC
  /// </summary>
  internal class RTCSetUIMessage : BLMessage
  {
    public DateTime DT {  get; }

    public RTCSetUIMessage(DateTime dt) { DT = dt; }
  }

  /// <summary>
  /// Синхронизировать RTC с часами ПК
  /// </summary>
  internal class RTCSyncWithPCUIMessage : BLMessage { }

  /// <summary>
  /// Вывести врутреннее состояние LCVM
  /// </summary>
  internal class RuntimePrintDumpUIMessage : BLMessage { }

  /// <summary>
  /// Поступило сообщение от контроллера о изменении состояния
  /// </summary>
  internal class StateChangedDeviceMessage : BLMessage 
  {
    public PLCStatus State { get; }

    public StateChangedDeviceMessage(PLCStatus state) { State = state; }
  }

  /// <summary>
  /// Потеряно соединение с контроллером
  /// </summary>
  internal class ConnectionLostDeviceMessage : BLMessage { }

  /// <summary>
  /// Пользователь изменил значение переменной в LiveWath
  /// </summary>
  internal class VariableViewerValueChangedUIMessage : BLMessage
  {
    public DataElementBase Element { get; }
    public VariableViewerValueChangedUIMessage(DataElementBase element)
    {
      Element = element;
    }
  }
}
