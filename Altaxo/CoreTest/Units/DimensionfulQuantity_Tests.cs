#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2023 Dr. Dirk Lellinger
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

using Xunit;

namespace Altaxo.Units
{
  public class DimensionfulQuantity_Tests
  {
    /// <summary>
    /// Tests the equality and comparisons.
    /// This is done by comparing the behavior with that of double.
    /// </summary>
    [Fact]
    public void Test_EqualityAndComparisons()
    {
      var testVector = new (double da, double db)[]
        {
          (6, 6),

          (6, 7),
          (7, 6),

          (double.NaN, double.NaN),
          (double.NaN, 6),
          (6, double.NaN),

          (double.PositiveInfinity, 6),
          (6, double.PositiveInfinity),
          (double.PositiveInfinity, double.PositiveInfinity),

          (double.NegativeInfinity, 6),
          (6, double.NegativeInfinity),
          (double.NegativeInfinity, double.NegativeInfinity),

          (double.NegativeInfinity, double.PositiveInfinity),
          (double.PositiveInfinity, double.NegativeInfinity),

          (double.NaN, double.PositiveInfinity),
          (double.PositiveInfinity, double.NaN),

          (double.NaN, double.NegativeInfinity),
          (double.NegativeInfinity, double.NaN),

        };

      foreach (var i in testVector)
      {
        var da = i.da;
        var db = i.db;
        var qa = new DimensionfulQuantity(da, SIPrefix.None, Length.Meter.Instance);
        var qb = new DimensionfulQuantity(db, SIPrefix.None, Length.Meter.Instance);

        Assert.Equal(qa == qb, da == db);
        Assert.Equal(qa != qb, da != db);

        Assert.Equal(qa.Equals(qb), da.Equals(db));
        Assert.Equal(qa.Equals((object)qb), da.Equals((object)db));
        Assert.Equal(object.Equals((object)qa, (object)qb), object.Equals((object)da, (object)db));

        Assert.Equal(qa.CompareTo(qb), da.CompareTo(db));
        Assert.Equal(qa.CompareTo((object)qb), da.CompareTo((object)db));
        Assert.Equal(qa < qb, da < db);
        Assert.Equal(qa <= qb, da <= db);
        Assert.Equal(qa > qb, da > db);
        Assert.Equal(qa >= qb, da >= db);

        Assert.Equal(qa.GetHashCode() == qb.GetHashCode(), da.GetHashCode() == db.GetHashCode());
      }
    }
  }
}
