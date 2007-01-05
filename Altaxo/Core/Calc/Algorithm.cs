#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Threading;

namespace Altaxo.Calc
{

  /// <summary>Define an algorithm.</summary>
  ///<remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.</para>
  /// <para>Adapted to Altaxo (C) 2005 D.Lellinger.</para>
  ///</remarks>
  public interface IAlgorithm
  {

    /// <summary>Computes the algorithm.</summary>
    void Compute();

    /// <summary>Informs user that that computation has began.</summary>
    event EventHandler BeginComputation;

    /// <summary>Informs user that that computation has finished.</summary>
    event EventHandler EndComputation;
  }

  /// <summary>
  /// Abstract Algorithm class. Subclasses need to only implement <code>InternalCompute</code>.
  /// </summary>
  ///<remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.</para>
  /// <para>Adapted to Altaxo (C) 2005 D.Lellinger.</para>
  ///</remarks>
  public abstract class Algorithm : IAlgorithm
  {
    private volatile bool computed = false;

    ///<summary>Informs user that that computation has began.</summary>
    public event EventHandler BeginComputation;

    ///<summary>Informs user that that computation has finished.</summary>
    public event EventHandler EndComputation;

    /// <summary>Handles <c>BeginComputation</c> events.</summary>
    /// <param name="e">event arguments</param>
    protected virtual void OnBeginComputation(EventArgs e)
    {
      if (BeginComputation != null)
      {
        BeginComputation(this, e);
      }
    }

    /// <summary>Handles <c>EndComputation</c> events.</summary>
    /// <param name="e">event arguments</param>
    protected virtual void OnEndComputation(EventArgs e)
    {
      if (EndComputation != null)
      {
        EndComputation(this, e);
      }
    }

    ///<summary>Specific to each algorithm </summary>
    protected abstract void InternalCompute();

    /// <summary>Computes the algorithm.</summary>
    public void Compute()
    {
      if (!computed)
      {
        lock (this)
        {
          if (!computed)
          {
            OnBeginComputation(EventArgs.Empty);
            InternalCompute();
            computed = true;
            OnEndComputation(EventArgs.Empty);
          }
        }
      }
    }
  }
}
