#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Altaxo.Calc.LinearAlgebra;

namespace Altaxo.Calc.Ode.Obsolete
{
  // Options name should match if possible: http://www.mathworks.com/help/techdoc/ref/odeset.html

  /// <summary>Options for <see cref="GearsBDF"/> ODE solver.</summary>
  public class GearsBDFOptions
  {
    /// <summary>Gets or sets initial step for solution. Default value 0 means that initial step is computed automatically</summary>
    public double InitialStep { get; set; }

    /// <summary>Gets or sets absolute error tolerance used in automatic step size calculations. Default is 1e-6.</summary>
    public double AbsoluteTolerance { get; set; }

    /// <summary>Gets or sets relative error tolerance used in automatic step size calculations. Default is 1e-6.</summary>
    public double RelativeTolerance { get; set; }

    /// <summary>Gets or sets maximal step value.</summary>
    public double MaxStep { get; set; }

    /// <summary>Gets or sets minimal step value.</summary>
    public double MinStep { get; set; }

    /// <summary>Gets or sets maximal step scale factor.</summary>
    public double MaxScale { get; set; }

    /// <summary>Gets or sets minimal step scale factor.</summary>
    public double MinScale { get; set; }

    /// <summary>Gets or sets number of iterations in GearBDF method - isn't used in RK547M/// </summary>
    public int NumberOfIterations { get; set; }

    /// <summary>Default construction of an Options instance.</summary>
    public GearsBDFOptions()
    {
      InitialStep = 0.0d;
      AbsoluteTolerance = 1e-6;
      RelativeTolerance = 1e-3;
      MaxStep = double.MaxValue;
      MinStep = 0.0d;
      MaxScale = 1.1d;
      MinScale = 0.9d;
      NumberOfIterations = 5;
    }

    private static readonly GearsBDFOptions defaultOpts = new GearsBDFOptions();

    /// <summary>Gets default option set</summary>
    public static GearsBDFOptions Default
    {
      get { return defaultOpts; }
    }
  }
}
