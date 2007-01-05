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
  public partial class G2DCartesicCSControl : UserControl, IG2DCartesicCSView
  {
    public G2DCartesicCSControl()
    {
      InitializeComponent();
    }

    #region IG2DCartesicCSView Members

    public bool ExchangeXY
    {
      get
      {
        return _chkExchangeXY.Checked;
      }
      set
      {
        _chkExchangeXY.Checked = value;
      }
    }

    public bool ReverseX
    {
      get
      {
        return _chkXReverse.Checked;
      }
      set
      {
        _chkXReverse.Checked = value;
      }
    }

    public bool ReverseY
    {
      get
      {
        return _chkYReverse.Checked;
      }
      set
      {
        _chkYReverse.Checked = value;
      }
    }

    #endregion
  }

  public interface IG2DCartesicCSView
  {
    bool ExchangeXY { get; set; }
    bool ReverseX { get; set; }
    bool ReverseY { get; set; }
  }
}
