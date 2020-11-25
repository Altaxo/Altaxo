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
using System.Collections;
using Altaxo.Calc.LinearAlgebra;
using Xunit;

namespace AltaxoTest.Calc.LinearAlgebra
{
  
  public class FloatMatrixEnumeratorTest
  {
    private const double TOLERANCE = 0.001;

    //Test Current Method
    [Fact]
    public void Current()
    {
      var test = new FloatMatrix(new float[2, 2] { { 1f, 2f }, { 3f, 4f } });
      IEnumerator enumerator = test.GetEnumerator();
      bool movenextresult;

      movenextresult = enumerator.MoveNext();
      Assert.True(movenextresult);
      Assert.Equal(enumerator.Current, test[0, 0]);

      movenextresult = enumerator.MoveNext();
      Assert.True(movenextresult);
      Assert.Equal(enumerator.Current, test[1, 0]);

      movenextresult = enumerator.MoveNext();
      Assert.True(movenextresult);
      Assert.Equal(enumerator.Current, test[0, 1]);

      movenextresult = enumerator.MoveNext();
      Assert.True(movenextresult);
      Assert.Equal(enumerator.Current, test[1, 1]);

      movenextresult = enumerator.MoveNext();
      Assert.False(movenextresult);
    }

    //Test foreach
    [Fact]
    public void ForEach()
    {
      var test = new FloatMatrix(new float[2, 2] { { 1f, 2f }, { 3f, 4f } });
      foreach (float f in test)
        Assert.True(test.Contains(f));
    }

    //Test Current Exception with index=-1.
    [Fact]
    public void CurrentException()
    {
      Assert.Throws<InvalidOperationException>(() =>
      {
        var test = new FloatMatrix(new float[2, 2] { { 1f, 2f }, { 3f, 4f } });
        IEnumerator enumerator = test.GetEnumerator();
        object value = enumerator.Current;
      });
    }

    //Test Current Exception with index>length
    [Fact]
    public void CurrentException2()
    {
      Assert.Throws<InvalidOperationException>(() =>
      {
        var test = new FloatMatrix(new float[2, 2] { { 1f, 2f }, { 3f, 4f } });
        IEnumerator enumerator = test.GetEnumerator();
        enumerator.MoveNext();
        enumerator.MoveNext();
        enumerator.MoveNext();
        enumerator.MoveNext();
        enumerator.MoveNext();
        object value = enumerator.Current;
      });
    }
  }
}
