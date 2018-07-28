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

using Altaxo.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Gdi.Shapes
{
  /// <summary>
  /// Interaction logic for RegularPolygonControl.xaml
  /// </summary>
  public partial class RegularPolygonControl : UserControl, IRegularPolygonView
  {
    public RegularPolygonControl()
    {
      InitializeComponent();
    }

    public IClosedPathShapeView ShapeGraphicView
    {
      get { return _guiShapeControl; }
    }

    public int Vertices
    {
      get
      {
        return _guiNumberOfVertices.Value;
      }
      set
      {
        _guiNumberOfVertices.Value = value;
      }
    }

    public double CornerRadiusPt
    {
      get
      {
        return _guiCornerRadius.SelectedQuantity.AsValueIn(Altaxo.Units.Length.Point.Instance);
      }
      set
      {
        _guiCornerRadius.SelectedQuantity = new DimensionfulQuantity(value, Altaxo.Units.Length.Point.Instance).AsQuantityIn(_guiCornerRadius.UnitEnvironment.DefaultUnit);
      }
    }
  }
}
