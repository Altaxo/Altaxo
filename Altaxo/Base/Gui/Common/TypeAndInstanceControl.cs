#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

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
