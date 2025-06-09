using System;
using System.Collections.Generic;
using System.ComponentModel;

public class AdvancedBindingList<T> : BindingList<T>
{
  //private bool _suppressListChangedEvents;

  public bool SuppressListChangedEvents { get; set; }

  public void RemoveRange(IEnumerable<T> items)
  {
    if (items == null)
      throw new ArgumentNullException(nameof(items));

    SuppressListChangedEvents = true; // Приостанавливаем уведомления
    try
    {
      foreach (var item in items)
      {
        Remove(item);
      }
    }
    finally
    {
      SuppressListChangedEvents = false; // Возобновляем уведомления
      ResetBindings(); // Уведомляем об изменении списка
    }
  }

  protected override void OnListChanged(ListChangedEventArgs e)
  {
    if (!SuppressListChangedEvents)
    {
      base.OnListChanged(e);
    }
  }
}
