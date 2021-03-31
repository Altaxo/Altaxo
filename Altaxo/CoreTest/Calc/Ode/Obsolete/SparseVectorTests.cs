#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using Xunit;

namespace Altaxo.Calc.Ode.Obsolete
{

  public class SparseVectorTests
  {
    private const double Eps = 1e-10;

    [Fact]
    public void ElementAccessorTest()
    {
      var sv = new SparseVector(1000);
      for (var i = 0; i < sv.Length / 2; i++)
      {
        sv[i] = i * i;
        sv[sv.Length - i - 1] = sv.Length - i - 1;
        for (var j = 0; j <= i; j++)
        {
          AssertEx.Equal(sv[i], i * i, Eps);
          Assert.Equal(sv[sv.Length - i - 1], sv.Length - i - 1);
        }
      }
    }
  }
}
