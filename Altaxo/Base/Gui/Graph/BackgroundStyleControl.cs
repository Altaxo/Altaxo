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

using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Graph
{

  [UserControlForController(typeof(IBackgroundStyleViewEventSink))]
  public partial class BackgroundStyleControl : UserControl, IBackgroundStyleView
  {
    public BackgroundStyleControl()
    {
      InitializeComponent();
    }

    IBackgroundStyleViewEventSink _controller;
    public IBackgroundStyleViewEventSink Controller
    {
      get
      {
        return _controller;
      }
      set
      {
        _controller = value;
      }
    }

    
    private void EhBackgroundColor_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if (null != Controller)
      {
          Controller.EhView_BackgroundBrushChanged(_cbColors.Brush);
      }
    }

    private void _cbBackgroundStyle_SelectionChangeCommitted(object sender, System.EventArgs e)
    {
      if (null != Controller)
        Controller.EhView_BackgroundStyleChanged(this._cbStyles.SelectedIndex);
    }



    public static void InitComboBox(System.Windows.Forms.ComboBox box, string[] names, string name)
    {
      box.Items.Clear();
      box.Items.AddRange(names);
      box.SelectedItem = name;
      box.Text = name;
    }


  

    public void BackgroundBrush_Initialize(BrushX brush)
    {
      this._cbColors.Brush = brush;
    }

    /// <summary>
    /// Initializes the enable state of the background color combo box.
    /// </summary>
    public void BackgroundBrushEnable_Initialize(bool enable)
    {
      this._cbColors.Enabled = enable;
    }

    /// <summary>
    /// Initializes the background styles.
    /// </summary>
    /// <param name="names"></param>
    /// <param name="selection"></param>
    public void BackgroundStyle_Initialize(string[] names, int selection)
    {
      this._cbStyles.Items.AddRange(names);
      this._cbStyles.SelectedIndex = selection;
    }

  }
}
