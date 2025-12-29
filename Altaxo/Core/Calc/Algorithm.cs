#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Copyright (c) 2003-2004, dnAnalytics. All rights reserved.
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

namespace Altaxo.Calc
{
  /// <summary>
  /// Defines an algorithm that can be computed and notifies about the begin/end of the computation.
  /// </summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.</para>
  /// <para>Adapted to Altaxo (C) 2005 D.Lellinger.</para>
  /// </remarks>
  public interface IAlgorithm
  {
    /// <summary>
    /// Computes the algorithm.
    /// </summary>
    void Compute();

    /// <summary>
    /// Informs the user that the computation has begun.
    /// </summary>
    event EventHandler BeginComputation;

    /// <summary>
    /// Informs the user that the computation has finished.
    /// </summary>
    event EventHandler EndComputation;
  }

  /// <summary>
  /// Abstract algorithm class.
  /// Subclasses need to implement only <c>InternalCompute</c>.
  /// </summary>
  /// <remarks>
  /// <para>Copyright (c) 2003-2004, dnAnalytics Project. All rights reserved.</para>
  /// <para>Adapted to Altaxo (C) 2005 D.Lellinger.</para>
  /// </remarks>
  public abstract class Algorithm : IAlgorithm
  {
    private volatile bool computed = false;

    /// <inheritdoc/>
    public event EventHandler? BeginComputation;

    /// <inheritdoc/>
    public event EventHandler? EndComputation;

    /// <summary>
    /// Raises the <see cref="BeginComputation"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnBeginComputation(EventArgs e)
    {
      BeginComputation?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="EndComputation"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnEndComputation(EventArgs e)
    {
      EndComputation?.Invoke(this, e);
    }

    /// <summary>
    /// Performs the algorithm-specific computation.
    /// </summary>
    /// <remarks>
    /// Subclasses implement this method to provide their computation logic. This method is called by <see cref="Compute"/>.
    /// </remarks>
    protected abstract void InternalCompute();

    /// <inheritdoc/>
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
