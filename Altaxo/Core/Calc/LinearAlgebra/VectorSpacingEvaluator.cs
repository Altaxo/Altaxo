#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2005 Dr. Dirk Lellinger
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
#endregion

using System;

namespace Altaxo.Calc.LinearAlgebra
{
  
  /// <summary>
  /// Statistics of the spacing between adjacent vector elements. The spaces are defined in the forward direction, i.e. as vec[i+1]-vec[i].
  /// </summary>
  public class VectorSpacingEvaluator
  {
    int _numvalidsteps=0;
    int _numtotalsteps=0;
    double _stepmin = double.PositiveInfinity;
    double _stepmax = double.NegativeInfinity;
    double _sumsteps = 0;

    /// <summary>
    /// Constructor. Takes an read only vector and evaluates the spaces between
    /// the vector elements.
    /// </summary>
    /// <param name="vec">The vector.</param>
    public VectorSpacingEvaluator(IROVector vec)
    {
      int lower = vec.LowerBound;
      int upper = vec.UpperBound;

      _numtotalsteps = upper-lower;
      for(int i=lower;i<upper;i++)
      {
        double step = vec[i+1]-vec[i];
        
        if(!double.IsNaN(step))
        {
          _numvalidsteps++;

          if(step>_stepmax)
            _stepmax = step;
          if(step<_stepmin)
            _stepmin = step;

          _sumsteps += step;
        }
      }

      // if all steps are valid, we calculate sumsteps from the boundaries
      // to enhance the accuracy.
      if(_numvalidsteps>0 && _numtotalsteps == _numvalidsteps)
        _sumsteps = vec[upper] - vec[lower];
    }


    /// <summary>
    /// True if all elements are mononton increasing and there are no invalid spaces.
    /// </summary>
    public bool IsMonotonicallyIncreasing
    {
      get
      {
        return _numvalidsteps == _numtotalsteps && _stepmin>=0 && _stepmax>0;
      }
    }

    /// <summary>
    /// True if all elements are mononton decreasing and there are no invalid spaces.
    /// </summary>
    public bool IsMonotonicallyDecreasing
    {
      get
      {
        return _numvalidsteps == _numtotalsteps && _stepmin<0 && _stepmax<=0;
      }
    }


    /// <summary>
    /// True if all elements are strongly mononton increasing and there are no invalid spaces.
    /// </summary>
    public bool IsStrictlyMonotonicIncreasing
    {
      get
      {
        return _numvalidsteps == _numtotalsteps && _stepmin>0 && _stepmax>0;
      }
    }

    /// <summary>
    /// True if all elements are strongly mononton decreasing and there are no invalid spaces.
    /// </summary>
    public bool IsStrictlyMonotonicDecreasing
    {
      get
      {
        return _numvalidsteps == _numtotalsteps && _stepmin<0 && _stepmax<0;
      }
    }

    /// <summary>
    /// True if all elements are strictly equally spaced. Due to the limited accuracy of floating
    /// point arithmetic, this is in generally only fulfilled when having integer vector elements. 
    /// Otherwise, please use <see cref="RelativeSpaceDeviation" /> to calculate the space deviation.
    /// </summary>
    public bool IsStrictlyEquallySpaced
    {
      get 
      {
        return _numvalidsteps == _numtotalsteps && _stepmin==_stepmax;
      }
    }

    /// <summary>
    /// True if there are invalid spaces, i.e. vector elements that are not numbers.
    /// </summary>
    public bool HasInvalidSpaces
    {
      get
      {
        return _numvalidsteps != _numtotalsteps;
      }
    }

    /// <summary>
    /// Returns true if the vector has at least one valid space. This is fulfilled if
    /// there is one adjacent pair of vector elements, which are both valid numbers.
    /// </summary>
    public bool HasValidSpaces
    {
      get
      {
        return _numvalidsteps>0;
      }
    }

    /// <summary>
    /// Calculates the relative deviation of spaces. The return value is a positive
    /// number, which indicates the relative space deviation. In case the deviation is
    /// undefined, the return value is PositiveInfinity.
    /// </summary>
    public double RelativeSpaceDeviation
    {
      get
      {
        if(_stepmin==0 && _stepmax==0)
          return 0;
        if(_stepmin==0 || _stepmax==0)
          return double.PositiveInfinity;
        if(double.IsInfinity(_stepmin) || double.IsInfinity(_stepmax))
          return double.PositiveInfinity;
        return Math.Abs(_stepmax-_stepmin)/Math.Min(Math.Abs(_stepmin),Math.Abs(_stepmax));
      }
    }

    /// <summary>
    /// Returns the mean value of the valid spaces.
    /// </summary>
    public double SpaceMeanValue
    {
      get
      {
        return _sumsteps/_numvalidsteps;
      }
    }



  }
}
