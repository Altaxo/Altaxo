using System.Collections.Generic;
using Xunit;

namespace Altaxo.Science.Spectroscopy.Sanitizing
{
  public class RemoveZerosTest
  {
    private double[] CreateArray()
    {
      var arr = new double[32];
      for (int i = 0; i < arr.Length; i++) arr[i] = i + 1;
      return arr;
    }


    private IEnumerable<(bool b0, bool b1, bool b2)> GetAllStatesOf3Booleans()
    {
      for (int i = 0; i < 8; ++i)
      {
        yield return ((i & 0x01) != 0, (i & 0x02) != 0, (i & 0x04) != 0);
      }
    }

    [Fact]
    public void TestRemoveAtStart()
    {
      foreach (var (b0, b1, b2) in GetAllStatesOf3Booleans())
      {

        var x = CreateArray();
        var y = CreateArray();

        var rz = new RemoveZeros { RemoveZerosAtStartOfSpectrum = true, RemoveZerosAtEndOfSpectrum = b0, RemoveZerosInMiddleOfSpectrum = b1, SplitIntoSeparateRegions = b2 };
        for (int i = 0; i <= y.Length; ++i)
        {
          for (int j = 0; j < i; ++j)
            y[j] = 0;

          var (xx, yy, rr) = rz.Execute(x, y, null);

          Assert.Equal(y.Length - i, xx.Length);
          Assert.Equal(y.Length - i, yy.Length);

          if (i < y.Length)
          {
            Assert.Equal(x.Length, xx[^1]);
            Assert.Equal(y.Length, yy[^1]);

            Assert.Equal(i + 1, xx[0]);
            Assert.Equal(i + 1, yy[0]);
          }

          Assert.True(rr is null);
        }
      }
    }

    [Fact]
    public void TestRemoveAtEnd()
    {
      foreach (var (b0, b1, b2) in GetAllStatesOf3Booleans())
      {
        var x = CreateArray();
        var y = CreateArray();

        var rz = new RemoveZeros { RemoveZerosAtStartOfSpectrum = b0, RemoveZerosAtEndOfSpectrum = true, RemoveZerosInMiddleOfSpectrum = b1, SplitIntoSeparateRegions = b2 };
        for (int i = 0; i <= y.Length; ++i)
        {
          for (int j = 0; j < i; ++j)
            y[y.Length - 1 - j] = 0;

          var (xx, yy, rr) = rz.Execute(x, y, null);

          Assert.Equal(y.Length - i, xx.Length);
          Assert.Equal(y.Length - i, yy.Length);

          if (i < y.Length)
          {
            Assert.Equal(1, xx[0]);
            Assert.Equal(1, yy[0]);

            Assert.Equal(xx.Length, xx[^1]);
            Assert.Equal(yy.Length, yy[^1]);
          }

          Assert.True(rr is null);
        }
      }
    }

    [Fact]
    public void TestRemoveInMiddle()
    {
      foreach (var (b0, b1, b2) in GetAllStatesOf3Booleans())
      {
        foreach (var offset in new int[] { 1, 5, 11 })
        {
          var x = CreateArray();
          var y = CreateArray();

          var rz = new RemoveZeros { RemoveZerosAtStartOfSpectrum = b0, RemoveZerosAtEndOfSpectrum = b1, RemoveZerosInMiddleOfSpectrum = true, SplitIntoSeparateRegions = b2 };
          for (int i = 1; i < y.Length - offset; ++i)
          {
            for (int j = 0; j < i; ++j)
              y[j + offset] = 0;

            var (xx, yy, rr) = rz.Execute(x, y, null);

            Assert.Equal(y.Length - i, xx.Length);
            Assert.Equal(y.Length - i, yy.Length);

            if (i < y.Length)
            {
              Assert.Equal(1, xx[0]);
              Assert.Equal(1, yy[0]);

              Assert.Equal(x.Length, xx[^1]);
              Assert.Equal(y.Length, yy[^1]);
            }

            if (rz.SplitIntoSeparateRegions)
            {
              Assert.True(rr.Length == 1);
              Assert.Equal(offset, rr[0]);
            }
            else
            {
              Assert.True(rr is null);
            }
          }
        }
      }
    }
  }
}
