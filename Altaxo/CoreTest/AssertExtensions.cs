using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Xunit
{
  public static class AssertEx
  {
    public static void Equal(string expected, string actual, string comment)
    {
      Assert.True(expected == actual, comment);
    }


    public static void Equal(int expected, int actual, string comment)
    {
      Assert.True(expected == actual, comment);
    }

    public static void Equal(uint expected, int actual)
    {
      Assert.True(expected == actual);
    }

    public static void Equal(uint expected, int actual, string comment)
    {
      Assert.True(expected == actual, comment);
    }

    public static void Equal(int expected, uint actual)
    {
      Assert.True(expected == actual);
    }

    public static void Equal(int expected, uint actual, string comment)
    {
      Assert.True(expected == actual, comment);
    }


    public static void Equal(ulong expected, long actual)
    {
      Assert.True(expected == (ulong)actual);
    }

    public static void Equal(ulong expected, long actual, string comment)
    {
      Assert.True(expected == (ulong)actual, comment);
    }

    public static void Equal(long expected, ulong actual)
    {
      Assert.True((ulong)expected == actual);
    }

    public static void Equal(long expected, ulong actual, string comment)
    {
      Assert.True((ulong)expected == actual, comment);
    }


    public static void AreEqual(double expected, double actual, double absError, double relError)
    {
      double delta = Math.Abs(absError) + Math.Abs(relError * expected);
      Assert.True(Math.Abs(expected - actual) <= delta);
    }

    public static void Equal(double expected, double actual, double absError, double relError, string comment)
    {
      double delta = Math.Abs(absError) + Math.Abs(relError * expected);
      Assert.True(Math.Abs(expected - actual) <= delta, comment);
    }

    public static void Equal(double expected, double actual, double absDeviation)
    {
      Assert.True(Math.Abs(expected - actual) <= absDeviation);
    }


    public static void Equal(double expected, double actual, double absDeviation, string comment)
    {
      if (double.IsNaN(expected))
        Assert.True(double.IsNaN(actual), comment);
      else
        Assert.True(Math.Abs(expected - actual) <= absDeviation, $"Expected: {expected} but was: {actual}; {comment}");
    }



    public static void Less(double expected, double actual)
    {
      Assert.True(expected < actual);
    }

    public static void Less(double expected, double actual, string comment)
    {
      Assert.True(expected < actual, comment);
    }

    public static void LessOrEqual(double expected, double actual)
    {
      Assert.True(expected <= actual);
    }
    public static void LessOrEqual(double expected, double actual, string comment)
    {
      Assert.True(expected <= actual, comment);
    }

    public static void Greater(double expected, double actual)
    {
      Assert.True(expected > actual);
    }
    public static void Greater(double expected, double actual, string comment)
    {
      Assert.True(expected > actual, comment);
    }

    public static void GreaterOrEqual(double expected, double actual)
    {
      Assert.True(expected >= actual);
    }
    public static void GreaterOrEqual(double expected, double actual, string comment)
    {
      Assert.True(expected >= actual, comment);
    }

    public static void NaN(double actual)
    {
      Assert.True(double.IsNaN(actual));
    }

    public static void NaN(double actual, string comment)
    {
      Assert.True(double.IsNaN(actual), comment);
    }

  }
}
