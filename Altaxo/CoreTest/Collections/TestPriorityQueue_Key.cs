#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using Altaxo.Collections;
using Xunit;

namespace AltaxoTest.Collections
{

  public class TestPriorityQueue_Key
  {
    [Fact]
    public void TestOrder()
    {
      const int numberOfElements = 10000;
      var rnd = new System.Random();

      var queue = new PriorityQueue<int>();

      for (int i = 0; i < numberOfElements; ++i)
        queue.Enqueue(100 + rnd.Next(numberOfElements));

      Assert.Equal(numberOfElements, queue.Count);

      int previous = int.MinValue;
      for (int i = 0; i < numberOfElements; ++i)
      {
        int curr = queue.Dequeue();
        AssertEx.GreaterOrEqual(curr, previous);
        previous = curr;
      }

      Assert.Equal(0, queue.Count);
    }
  }
}
