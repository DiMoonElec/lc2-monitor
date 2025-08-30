using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Windows.Forms;
using LC2Monitor.BL;
using Serilog;

namespace LC2Monitor
{
  internal static class Program
  {
    /// <summary>
    /// Главная точка входа для приложения.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Debug() // Уровень логирования
           .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day) // Логирование в файл
           .CreateLogger();

      try
      {
        Log.Information("Application starting...");

        var section = ConfigurationManager.GetSection("System.Windows.Forms.ApplicationConfigurationSection") as NameValueCollection;
        if (section != null)
        {
          section["DpiAwareness"] = "PerMonitorV2";
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var mainForm = new MainForm(); // Создание главной формы
        var businessLogic = new BusinessLogic(); // Создание бизнес-логики
        var presenter = new Presenter(mainForm,
          () => new DateTimeInputForm(BusinessLogic.RTCMinDateTime, BusinessLogic.RTCMaxDateTime),
          businessLogic); // Создание Presenter и связывание с формой и бизнес-логикой

        Application.Run(mainForm); // Запуск приложения
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Application terminated unexpectedly");
      }
      finally
      {
        Log.CloseAndFlush(); // Завершаем работу логера
      }
      //Application.Run(new MainForm());
    }
  }
}
