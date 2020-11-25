#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using Altaxo.Drawing;
using Xunit;

namespace Altaxo.Graph.Gdi
{

  public class TestBrushX
  {
    protected void Tester(BrushX brush1, BrushX brush2, string comment)
    {
      Assert.True(brush1 == brush2, comment);
      Assert.False(brush1 != brush2, comment);
      Assert.True(brush1.Equals(brush2), comment);
      Assert.True(brush2.Equals(brush1), comment);
      Assert.True(object.Equals(brush1, brush2), comment);
      Assert.False(object.ReferenceEquals(brush1, brush2), comment);
      Assert.Equal(brush1.GetHashCode(), brush2.GetHashCode());
    }

    [Fact]
    public void TestHash_SolidBrush()
    {
      var brush1 = new BrushX(NamedColors.Green);
      var brush2 = new BrushX(NamedColors.Green);
      Tester(brush1, brush2, nameof(TestHash_SolidBrush));
    }

    [Fact]
    public void TestHash_LinearGradientBrush()
    {
      var brush1 = new BrushX(BrushType.LinearGradientBrush).WithColor(NamedColors.AliceBlue).WithBackColor(NamedColors.Red);
      var brush2 = new BrushX(BrushType.LinearGradientBrush).WithColor(NamedColors.AliceBlue).WithBackColor(NamedColors.Red);
      Tester(brush1, brush2, nameof(TestHash_LinearGradientBrush));
    }


  }
}
