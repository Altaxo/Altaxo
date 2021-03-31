#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Altaxo.Calc.Ode.Obsolete
{
  // Options name should match if possible: http://www.mathworks.com/help/techdoc/ref/odeset.html

  /// <summary>ODE solver options</summary>
  public class Options
  {
    /// <summary>Gets or sets initial step for solution. Default value 0 means that initial step is computed automatically</summary>
    public double InitialStep { get; set; }

    /// <summary>Gets or sets absolute error tolerance used in automatic step size calculations. Default is 1e-6.</summary>
    public double AbsoluteTolerance { get; set; }

    /// <summary>Gets or sets relative error tolerance used in automatic step size calculations. Default is 1e-6.</summary>
    public double RelativeTolerance { get; set; }

    /// <summary>Gets or sets step value in output solution. Default value 0 means than all points are included to output</summary>
    public double OutputStep { get; set; }

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

    /// <summary>Gets or sets a dense Jacobian matrix</summary>
    public Matrix? Jacobian { get; set; }

    /// <summary>Gets or sets a sparse Jacobian matrix</summary>
    public SparseMatrix? SparseJacobian { get; set; }

    /// <summary>Default construction of an Options instance.</summary>
    public Options()
    {
      InitialStep = 0.0d;
      AbsoluteTolerance = 1e-6;
      RelativeTolerance = 1e-3;
      MaxStep = double.MaxValue;
      MinStep = 0.0d;
      MaxScale = 1.1d;
      MinScale = 0.9d;
      OutputStep = 0.0d;
      NumberOfIterations = 5;
    }

    private static readonly Options defaultOpts = new Options();

    /// <summary>Gets default option set</summary>
    public static Options Default
    {
      get { return defaultOpts; }
    }
  }

  /// <summary>Structure to represent solution point. Current point has form (t,x1,x2,...,xn), where
  /// n is problem's dimension</summary>
  public struct SolutionPoint
  {
    private Vector x; //Problem's phase variables
    private double t; //Current time

    /// <summary>Gets phase variables values in current time point</summary>
    public Vector X
    {
      get { return x; }
    }

    /// <summary>Gets current time</summary>
    public double T
    {
      get { return t; }
    }

    /// <summary>Create solution point from time value and phase vector</summary>
    /// <param name="t">Current time value</param>
    /// <param name="x">Current phase vector</param>
    internal SolutionPoint(double t, Vector x)
    {
      this.x = x;
      this.t = t;
    }
  }
}
