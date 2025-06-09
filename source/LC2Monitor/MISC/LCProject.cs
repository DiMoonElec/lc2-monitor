using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LC2.LCCompiler
{
  class LCProject
  {
    /// <summary>
    /// Имя платформы (ПЛК), под которую разрабатывается ПО.
    /// Данное имя являеся именем xml-файла конфигурации ПЛК
    /// </summary>
    public string Platform = null;

    /// <summary>
    /// Имя каталога, в который будут сохранены выходные файлы
    /// </summary>
    public string OutputDirectoryName = "out";

    /// <summary>
    /// Пути к файлам исходников, относительно файла проекта
    /// </summary>
    public List<string> SourceFiles = new List<string>();


    public string ProjectFilePath { get; }
    public string ProjectFileDirectory { get; }
    public string ProjectFileName { get; }
    public string OutputDirectoryPath { get; }
    public string OutputBinaryFilePath { get; }
    public string OutputMemoryAllocationReportFilePath { get; }
    public string OutputAsmFilePath { get; }
    public string OutputDebugInformationFilePath { get; }

    public LCProject(string file)
    {
      ProjectFilePath = GetAbsolutePath(file);
      xmlLoad(ProjectFilePath);

      ProjectFileDirectory = Path.GetDirectoryName(ProjectFilePath);
      ProjectFileName = Path.GetFileNameWithoutExtension(ProjectFilePath);
      OutputDirectoryPath = Path.Combine(ProjectFileDirectory, OutputDirectoryName);
      OutputBinaryFilePath = Path.Combine(OutputDirectoryPath, ProjectFileName + ".lcx");
      OutputMemoryAllocationReportFilePath = Path.Combine(OutputDirectoryPath, ProjectFileName + ".xml");
      OutputAsmFilePath = Path.Combine(OutputDirectoryPath, ProjectFileName + ".lcasm");
      OutputDebugInformationFilePath = Path.Combine(OutputDirectoryPath, ProjectFileName + ".dbginfo");
    }

    private void xmlLoad(string file)
    {
      XmlDocument xDoc = new XmlDocument();
      xDoc.Load(file);

      XmlElement xRoot = xDoc.DocumentElement;

      if (xRoot == null)
        throw new Exception();

      foreach (XmlElement xnode in xRoot)
      {
        var name = xnode.Name;
        switch (name)
        {
          case "platform":
            Platform = xnode.InnerText;
            break;

          case "source":
            loadSourceSection(xnode);
            break;

          case "outputdir":
            OutputDirectoryName = xnode.InnerText;
            break;
        }
      }
    }

    private void loadSourceSection(XmlElement xnode)
    {
      foreach (XmlElement file in xnode)
      {
        var name = file.Name;
        if (name == "file")
        {
          var filePath = file.InnerText;
          SourceFiles.Add(filePath);
        }
      }
    }


    static string GetAbsolutePath(string path)
    {
      if (string.IsNullOrWhiteSpace(path))
      {
        throw new ArgumentException("Path cannot be null or empty.", nameof(path));
      }

      // Проверяем, является ли путь абсолютным
      if (Path.IsPathRooted(path))
      {
        return Path.GetFullPath(path); // Убеждаемся, что путь полон
      }

      // Преобразуем относительный путь в абсолютный
      string currentDirectory = Directory.GetCurrentDirectory();
      return Path.GetFullPath(Path.Combine(currentDirectory, path));
    }
  }
}
