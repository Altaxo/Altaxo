#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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
using Altaxo.Geometry;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Frames;
using Altaxo.Graph.Graph2D.Plot.Styles.ScatterSymbols.Insets;
using NUnit.Framework;

namespace Altaxo.Graph.Gdi.Plot.Styles.ScatterSymbols
{
  [TestFixture]
  public class TestSquare
  {
    [Test]
    public void TestGeneratedPolygons_WOFrame_WOInset()
    {

      var square = new Square();

      square.CalculatePolygons(null, out var framePolygon, out var insetPolygon, out var fillPolygon);

      Assert.IsNull(framePolygon);
      Assert.IsNull(insetPolygon);
      Assert.IsNotNull(fillPolygon);
    }

    [Test]
    public void TestGeneratedPolygons_WithFrame_WOInset()
    {

      var square = new Square().WithFrame(new ConstantThicknessFrame()).WithRelativeStructureWidth(0.125);

      square.CalculatePolygons(null, out var framePolygon, out var insetPolygon, out var fillPolygon);

      Assert.IsNotNull(framePolygon);
      Assert.IsNull(insetPolygon);
      Assert.IsNotNull(fillPolygon);
    }

    [Test]
    public void TestGeneratedPolygons_WOFrame_WithInset()
    {

      var square = new Square().WithInset(new VerticalBarInset()).WithRelativeStructureWidth(0.125);

      square.CalculatePolygons(null, out var framePolygon, out var insetPolygon, out var fillPolygon);

      Assert.IsNull(framePolygon);
      Assert.IsNotNull(insetPolygon);
      Assert.IsNotNull(fillPolygon);
    }

    [Test]
    public void TestGeneratedPolygons_WithFrame_WithInset()
    {

      var square = new Square()
                    .WithFrame(new ConstantThicknessFrame())
                    .WithInset(new VerticalBarInset())
                    .WithRelativeStructureWidth(0.125);

      square.CalculatePolygons(null, out var framePolygon, out var insetPolygon, out var fillPolygon);

      Assert.IsNotNull(framePolygon);
      Assert.IsNotNull(insetPolygon);
      Assert.IsNotNull(fillPolygon);
    }

    [Test]
    public void TestGeneratedPolygons_WithFrame_WithSquarePointInset()
    {

      var square = new Square()
                    .WithFrame(new ConstantThicknessFrame())
                    .WithInset(new SquarePointInset())
                    .WithRelativeStructureWidth(0.125);

      square.CalculatePolygons(null, out var framePolygon, out var insetPolygon, out var fillPolygon);

      Assert.IsNotNull(framePolygon);
      Assert.IsNotNull(insetPolygon);
      Assert.IsNotNull(fillPolygon);
    }
  }
}
