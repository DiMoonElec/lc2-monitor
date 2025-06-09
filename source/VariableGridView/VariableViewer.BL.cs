using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using DebugViews.DataClasses;

namespace DebugViews
{
  public partial class VariableViewer : UserControl
  {
    private bool _visibleLiveColumn = true;
    public bool VisibleLiveColumn
    {
      get
      {
        return _visibleLiveColumn;
      }

      set
      {
        _visibleLiveColumn= value;
        dataGridView.Columns[0].Visible = _visibleLiveColumn;
      }
    }

    IEnumerable<DataElementBase> variables;

    private BindingList<DataElementBase> _variables = new BindingList<DataElementBase>();
    BindingSource _bindingSource = new BindingSource();
    public event Action<DataElementBase> VariableChanged;
    private bool _isUserEditing = false;

    public void InitializeViewer()
    {
      dataGridView.AllowUserToAddRows = false;
      dataGridView.AllowUserToDeleteRows = false;
      dataGridView.AllowUserToResizeRows = false;
      dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
      dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      //this.Location = new System.Drawing.Point(12, 12);
      dataGridView.RowHeadersVisible = false;

      InitializeGrid();
    }

    private void InitializeGrid()
    {
      dataGridView.AutoGenerateColumns = false;

      dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
      dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

      dataGridView.AllowUserToOrderColumns = true;
      dataGridView.AllowUserToResizeColumns = true;

      var checkBoxColumn = new DataGridViewCheckBoxColumn
      {
        HeaderText = "Watch",
        DataPropertyName = "Watch",
        ReadOnly = false,
        AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
      };
      dataGridView.Columns.Add(checkBoxColumn);

      dataGridView.Columns.Add(new DataGridViewTextBoxColumn
      {
        HeaderText = "Name",
        DataPropertyName = "DisplayName",
        ReadOnly = true,
        AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
      });

      dataGridView.Columns.Add(new DataGridViewTextBoxColumn
      {
        HeaderText = "Type",
        DataPropertyName = "Type",
        ReadOnly = true,
        AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
      });

      dataGridView.Columns.Add(new DataGridViewTextBoxColumn
      {
        HeaderText = "Value",
        DataPropertyName = "Value",
        ReadOnly = false,
        AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells
      });

      _bindingSource.DataSource = _variables;
      dataGridView.DataSource = _bindingSource;

      // Обработчики событий
      dataGridView.CellBeginEdit += (s, e) => { _isUserEditing = true; };
      dataGridView.CellEndEdit += OnCellEndEdit;
      dataGridView.CellClick += OnCellClick;
      dataGridView.CellFormatting += OnCellFormatting;
      dataGridView.CurrentCellDirtyStateChanged += OnCurrentCellDirtyStateChanged;
      dataGridView.DataError += OnDataError;
    }

    private void OnCurrentCellDirtyStateChanged(object sender, EventArgs e)
    {
      if (dataGridView.CurrentCell is DataGridViewCheckBoxCell && dataGridView.IsCurrentCellDirty)
      {
        // Применяем изменения сразу
        dataGridView.CommitEdit(DataGridViewDataErrorContexts.Commit);

        int rowIndex = dataGridView.CurrentCell.RowIndex;
        if (rowIndex >= 0 && rowIndex < _variables.Count)
        {
          var variable = _variables[rowIndex];
          if (variable is DataElementUnion)
            dataGridView.Refresh(); // Обновляем отображение компонента
        }
      }
    }

    private void OnDataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      // Подсвечиваем ячейку красным цветом
      var cell = dataGridView[e.ColumnIndex, e.RowIndex];
      if (cell != null)
      {
        cell.Style.BackColor = Color.Red; // Красный фон
        Task.Delay(500).ContinueWith(_ =>
        {
          // Возвращаем цвет ячейки в основной поток UI
          this.Invoke((Action)(() =>
          {
            cell.Style.BackColor = dataGridView.DefaultCellStyle.BackColor;
          }));
        });
      }

      // Проигрываем звуковой сигнал
      System.Media.SystemSounds.Exclamation.Play();

      // Пропускаем обработку ошибки
      e.ThrowException = false;
    }

    private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
      if (e.RowIndex < 0)
        return;

      var variable = _variables[e.RowIndex];

      /*
      // Настройка выравнивания для ячейки с CheckBox
      if (e.ColumnIndex == 0) // Столбец CheckBox
      {
        var cell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex] as DataGridViewCheckBoxCell;
        if (variable is DataElementUnion)
        {
          // Для групп (DataElementUnion) выравниваем по левому краю
          //cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
          //cell.ThreeState = true;
        }
        else
        {
          // Для обычных переменных оставляем стандартное выравнивание
          //cell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
      }
      */


      // Подсветка ячейки Value, если ValueChanged == true
      if (e.ColumnIndex == 3)
      {
        if (variable.ValueChanged)
        {
          e.CellStyle.BackColor = Color.LightGreen; // Зеленый фон
          e.CellStyle.Font = new Font(dataGridView.DefaultCellStyle.Font, FontStyle.Bold); // Жирный шрифт
        }
      }

      if (variable is DataElementUnion && e.ColumnIndex == 3)
      {
        dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].ReadOnly = true; // Значение только для чтения
      }
    }

    private void OnCellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
      _isUserEditing = false;

      if (e.RowIndex < 0)
        return;

      if (e.ColumnIndex == 3) //Колонка Value
      {
        var variable = _variables[e.RowIndex];
        string newValue = dataGridView[e.ColumnIndex, e.RowIndex].Value.ToString();

        try
        {
          //variable.SetValue(newValue);
          VariableChanged?.Invoke(variable);
        }
        catch (Exception ex)
        {
          MessageBox.Show($"Invalid value: {ex.Message}");
          dataGridView[e.ColumnIndex, e.RowIndex].Value = variable.Value;
        }
      }
      else if (e.ColumnIndex == 0) //Колонка Watch
      {
        var variable = _variables[e.RowIndex];
        if (variable is DataElementUnion elementUnion)
          dataGridView.Refresh();
      }
    }

    private void OnCellClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.RowIndex < 0)
        return;

      if (e.ColumnIndex == 1) // Щелчок по имени переменной
      {
        var variable = _variables[e.RowIndex];
        if (variable is DataElementUnion elementUnion)
        {
          try
          {
            // Отключаем уведомления
            _bindingSource.RaiseListChangedEvents = false;

            if (elementUnion.IsExpanded)
            {
              CollapseArray(elementUnion);
              elementUnion.IsExpanded = false;
            }
            else
            {
              ExpandArray(elementUnion, e.RowIndex);
              elementUnion.IsExpanded = true;
            }
          }
          finally
          {
            // Включаем уведомления и обновляем DataGridView
            _bindingSource.RaiseListChangedEvents = true;
            _bindingSource.ResetBindings(false);
          }
        }
      }
    }


    public void LoadVariables(IEnumerable<DataElementBase> variables)
    {
      this.variables = variables;
      _variables.Clear();

      foreach (var variable in variables)
      {
        if (variable is DataElementUnion elementUnion)
        {
          elementUnion.IsExpanded = false;
          variable.DisplayName = "▶ " + elementUnion.Name;
        }
        else
        {
          variable.DisplayName = variable.Name; // Для простых переменных
        }

        _variables.Add(variable);
      }
    }


    private int ExpandArray(DataElementUnion elementUnion, int rowIndex)
    {
      int currentIndex = rowIndex + 1;

      foreach (var element in elementUnion.Elements)
      {
        // Устанавливаем уровень вложенности элемента
        element.IndentLevel = elementUnion.IndentLevel + 1;

        // Формируем отображаемое имя с отступом
        element.DisplayName = new string(' ', element.IndentLevel * 4) +
                              (element is DataElementUnion ? "▶ " : "") +
                              element.Name;

        // Вставляем элемент на текущую позицию
        _variables.Insert(currentIndex, element);

        // Если это вложенный Union и он уже был развернут
        if (element is DataElementUnion nestedUnion && nestedUnion.IsExpanded)
        {
          // Раскрываем его рекурсивно, обновляя индекс текущей позиции
          currentIndex = ExpandArray(nestedUnion, currentIndex);
        }

        // Увеличиваем индекс текущей позиции
        currentIndex++;
      }

      // Обновляем значок для родительского элемента
      elementUnion.DisplayName = new string(' ', elementUnion.IndentLevel * 4) + "▼ " + elementUnion.Name;

      // Обновляем состояние
      elementUnion.IsExpanded = true;

      dataGridView.Refresh();

      // Возвращаем последний обработанный индекс
      return currentIndex - 1;
    }



    private void CollapseArray(DataElementUnion elementUnion)
    {
      foreach (var element in elementUnion.Elements)
      {
        if (element is DataElementUnion nestedUnion && nestedUnion.IsExpanded)
        {
          // Сначала сворачиваем дочерний Union
          CollapseArray(nestedUnion);
          nestedUnion.IsExpanded = true; // Сохраняем состояние как "развернутый"
        }

        _variables.Remove(element); // Удаляем из общего списка
      }

      // Обновляем значок для родительского элемента
      elementUnion.DisplayName = new string(' ', elementUnion.IndentLevel * 4) + "▶ " + elementUnion.Name;

      // Обновляем состояние
      elementUnion.IsExpanded = false;
    }

    public void UpdateValues(VariablesDump dump)
    {
      if (variables == null)
        return;

      foreach (var variable in variables)
        variable.UpdateValue(dump);

      dataGridView.Refresh();
    }

    public DataElementBase[] GetWatchVariables()
    {
      List<DataElementBase> r = new List<DataElementBase>();

      foreach (var variable in _variables)
        if (!(variable is DataElementUnion) && variable.Watch)
          r.Add(variable);

      return r.ToArray();
    }

  }
}
