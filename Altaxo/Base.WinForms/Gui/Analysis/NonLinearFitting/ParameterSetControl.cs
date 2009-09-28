using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Altaxo.Gui.Analysis.NonLinearFitting
{
  public partial class ParameterSetControl : UserControl, IParameterSetView
  {
    public ParameterSetControl()
    {
      InitializeComponent();
    }


    public void Initialize(List<ParameterSetViewItem> list)
    {
      _grid.Rows.Clear();

      foreach (ParameterSetViewItem item in list)
      {
        _grid.Rows.Add(new object[] { item.Name, item.Value, item.Vary, item.Variance });
      }
    }

    public List<ParameterSetViewItem> GetList()
    {
      List<ParameterSetViewItem> list = new List<ParameterSetViewItem>();

      foreach (DataGridViewRow row in _grid.Rows)
      {
        ParameterSetViewItem item = new ParameterSetViewItem();

        item.Name = (string)row.Cells[0].Value;
        item.Value = (string)row.Cells[1].Value;
        item.Vary = (bool)row.Cells[2].Value;
        item.Variance = (string)row.Cells[3].Value;

        list.Add(item);
      }

      return list;
    }
  }
}
