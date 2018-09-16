#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Altaxo.Graph.Gdi;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// Interaction logic for PenAllPropertiesControl.xaml
  /// </summary>
  public partial class PenAllPropertiesControl : UserControl, IPenAllPropertiesView
  {
    private PenControlsGlue _glue;

    public PenAllPropertiesControl()
    {
      InitializeComponent();

      _glue = new PenControlsGlue(true)
      {
        CbBrush = _cbBrush,
        CbLineThickness = _cbThickness,
        CbDashPattern = _cbDashStyle,
        CbDashCap = _cbDashCap,
        CbStartCap = _cbStartCap,
        CbStartCapAbsSize = _cbStartCapSize,
        CbStartCapRelSize = _edStartCapRelSize,
        CbEndCap = _cbEndCap,
        CbEndCapAbsSize = _cbEndCapSize,
        CbEndCapRelSize = _edEndCapRelSize,
        CbLineJoin = _cbLineJoin,
        CbMiterLimit = _cbMiterLimit,
        PreviewPanel = _previewPanel
      };
    }

    public PenX Pen
    {
      get
      {
        return _glue.Pen;
      }
      set
      {
        _glue.Pen = value;
      }
    }

    public bool ShowPlotColorsOnly
    {
      set { _glue.ShowPlotColorsOnly = value; }
    }
  }
}
