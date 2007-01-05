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



namespace Altaxo.Gui.Graph
{
  [UserControlForController(typeof(IPlottingRangeViewEventSink))]
  public partial class PlottingRangeControl : UserControl, IPlottingRangeView
  {
    public PlottingRangeControl()
    {
      InitializeComponent();
    }

    IPlottingRangeViewEventSink _controller;
    public IPlottingRangeViewEventSink Controller
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

    /// <summary>
    /// Initializes the view.
    /// </summary>
    /// <param name="from">First value of plot range.</param>
    /// <param name="to">Last value of plot range.</param>
    /// <param name="isInfinity">True if the plot range is infinite large.</param>
    public void Initialize(int from, int to, bool isInfinity)
    {
      _edFrom.Text = from.ToString();
      if (isInfinity)
        _edTo.Text = string.Empty;
      else
        _edTo.Text = to.ToString();
    }

    private void _edFrom_Validating(object sender, CancelEventArgs e)
    {
      if (_controller != null)
        _controller.EhView_Changed(_edFrom.Text, _edTo.Text);

    }

    private void _edTo_Validating(object sender, CancelEventArgs e)
    {
      if (_controller != null)
        _controller.EhView_Changed(_edFrom.Text, _edTo.Text);
    }
  }
}
