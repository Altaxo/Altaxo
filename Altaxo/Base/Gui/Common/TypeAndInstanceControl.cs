using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Altaxo.Collections;

namespace Altaxo.Gui.Common
{
  public partial class TypeAndInstanceControl : UserControl, ITypeAndInstanceView
  {
    public TypeAndInstanceControl()
    {
      InitializeComponent();
    }

    #region ITypeAndInstanceView Members

    public string TypeLabel
    {
      set { _lblCSType.Text = value; }
    }

    public void InitializeTypeNames(List<ListNode> names, int selection)
    {
      _cbTypeChoice.Items.Clear();
      foreach (ListNode n in names)
        _cbTypeChoice.Items.Add(n);

      if (selection >= 0)
        _cbTypeChoice.SelectedIndex = selection;
    }

    public void SetInstanceControl(object instanceControl)
    {
      _panelForSubControl.Controls.Clear();

      Control ctrl = (Control)instanceControl;
      if (ctrl != null)
      {
        ctrl.Location = new Point(0, 0);
        _panelForSubControl.Controls.Add(ctrl);
      }
    }

    #endregion

    public event EventHandler TypeChoiceChanged;
    private void EhSelectionChangeCommitted(object sender, EventArgs e)
    {
      if (null != TypeChoiceChanged)
        TypeChoiceChanged(this, EventArgs.Empty);
    }
    public ListNode SelectedNode
    {
      get
      {
        return _cbTypeChoice.SelectedItem as ListNode;
      }
    }
  }

  public interface ITypeAndInstanceView
  {
    string TypeLabel { set; }
    void InitializeTypeNames(List<ListNode> names, int selection);
    void SetInstanceControl(object instanceControl);

    event EventHandler TypeChoiceChanged;
    ListNode SelectedNode { get; }
  }
}
