#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Copyright (C) bsargos, Software Developer, France
//    (see CodeProject article http://www.codeproject.com/Articles/16083/One-dimensional-root-finding-algorithms)
//    This source code file is licenced under the CodeProject open license (CPOL)
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2012 Dr. Dirk Lellinger
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
using System.Text;

namespace Altaxo.Calc.RootFinding
{
  /// <summary>
  /// Represents a numeric interval used by root-finding algorithms.
  /// </summary>
  public struct Range
  {
    private double Min, Max;

    /// <summary>
    /// Initializes a new instance of the <see cref="Range"/> struct.
    /// </summary>
    /// <param name="min">The lower bound of the range.</param>
    /// <param name="max">The upper bound of the range.</param>
    public Range(double min, double max)
    {
      Min = min;
      Max = max;
    }
  }

  /// <summary>
  /// Exception thrown by root-finding algorithms when a root cannot be found or the accuracy goal is not reached.
  /// </summary>
  public class RootFinderException : Exception
  {
    private int m_Iteration;
    private Range m_Range;
    private double m_Accuracy;

    /// <summary>
    /// Initializes a new instance of the <see cref="RootFinderException"/> class.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <param name="iteration">The iteration count at the time the error occurred.</param>
    /// <param name="range">The range that was searched for a root.</param>
    /// <param name="accuracy">The accuracy that was requested or achieved (depending on the algorithm).</param>
    public RootFinderException(string message, int iteration, Range range, double accuracy)
      : base(message)
    {
      m_Iteration = iteration;
      m_Range = range;
      m_Accuracy = accuracy;
    }

    /// <summary>
    /// Gets or sets the iteration count at the time the error occurred.
    /// </summary>
    public int Iteration
    {
      get { return m_Iteration; }
      set { m_Iteration = value; }
    }

    /// <summary>
    /// Gets or sets the range that was searched for a root.
    /// </summary>
    public Range Range
    {
      get { return m_Range; }
      set { m_Range = value; }
    }

    /// <summary>
    /// Gets or sets the accuracy that was requested or achieved (depending on the algorithm).
    /// </summary>
    public double Accuracy
    {
      get { return m_Accuracy; }
      set { m_Accuracy = value; }
    }
  }
}
