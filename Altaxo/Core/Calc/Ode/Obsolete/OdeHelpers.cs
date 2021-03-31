#region Copyright

// Copyright Microsoft Research in collaboration with Moscow State University
// Microsoft Research License, see license file "MSR-LA - Open Solving Library for ODEs.rtf"
// This file originates from project OSLO - Open solving libraries for ODEs - 1.1

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;

namespace Altaxo.Calc.Ode.Obsolete
{
  public static class OdeHelpers
  {
    /// <summary>Extracts points with time less than or equal to <paramref name="to"/></summary>
    /// <param name="solution">Solution</param>
    /// <param name="to">Finish time</param>
    /// <returns>New sequence of solution points</returns>
    public static IEnumerable<SolutionPoint> SolveTo(this IEnumerable<SolutionPoint> solution, double to)
    {
      return solution.TakeWhile(p => p.T <= to);
    }

    /// <summary>Extracts points with time between <paramref name="from"/> and <paramref name="to"/> points.</summary>
    /// <param name="solution">Solution</param>
    /// <param name="from">Start time</param>
    /// <param name="to">Finish time</param>
    /// <returns>New sequence of solution points</returns>
    public static IEnumerable<SolutionPoint> SolveFromTo(this IEnumerable<SolutionPoint> solution, double from, double to)
    {
      return solution.SkipWhile(p => p.T < from).TakeWhile(p => p.T <= to);
    }

    /// <summary>Extracts points with time between <paramref name="from"/> and <paramref name="to"/> points and interpolates
    /// points with specified time step.</summary>
    /// <param name="solution">Solution</param>
    /// <param name="from">Start time</param>
    /// <param name="to">Finish time</param>
    /// <param name="step">Time step</param>
    /// <returns>New sequence of solution points at i * step</returns>
    public static IEnumerable<SolutionPoint> SolveFromToStep(this IEnumerable<SolutionPoint> solution, double from, double to, double step)
    {
      return solution.WithStep(step).SkipWhile(p => p.T < from).TakeWhile(p => p.T <= to);
    }

    /// <summary>Interpolates solution at points with specified time step</summary>
    /// <param name="solution">Solution</param>
    /// <param name="delta">Time step</param>
    /// <returns>New sequence of solution points at moments i * delta</returns>
    /// <remarks>Linear intepolation is used to find phase vector between two solution points</remarks>
    public static IEnumerable<SolutionPoint> WithStep(this IEnumerable<SolutionPoint> solution, double delta)
    {
      var en = solution.GetEnumerator();
      if (!en.MoveNext())
        yield break;
      SolutionPoint prev = en.Current;
      double n = Math.Ceiling(prev.T / delta);
      double tout = n * delta;
      if (tout == prev.T)
        yield return prev;
      n++;
      tout = n * delta;
      while (true)
      {
        if (!en.MoveNext())
          yield break;
        SolutionPoint current = en.Current;
        while (current.T >= tout)
        {
          yield return new SolutionPoint(tout, Vector.Lerp(tout, prev.T, prev.X, current.T, current.X));
          n++;
          tout = n * delta;
        }
        prev = current;
      }
    }

    /// <summary>Modifies solution by adding time step between consequtive points at the end of
    /// system state vector</summary>
    /// <param name="solution">Solution sequence</param>
    /// <returns>Solution with appended time step</returns>
    public static IEnumerable<SolutionPoint> AppendStep(this IEnumerable<SolutionPoint> solution)
    {
      var en = solution.GetEnumerator();
      if (!en.MoveNext())
        yield break;
      SolutionPoint prev = en.Current;
      double[] temp = new double[prev.X.Length + 1];
      Array.Copy(prev.X, temp, prev.X.Length);
      yield return new SolutionPoint(prev.T, new Vector((double[])temp.Clone()));
      while (en.MoveNext())
      {
        SolutionPoint curr = en.Current;
        Array.Copy(curr.X, temp, curr.X.Length);
        temp[curr.X.Length] = curr.T - prev.T;
        yield return new SolutionPoint(curr.T, new Vector((double[])temp.Clone()));
        prev = curr;
      }
      yield break;
    }
  }
}
