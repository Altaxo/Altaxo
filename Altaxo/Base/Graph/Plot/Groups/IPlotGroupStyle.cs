#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
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
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// Support of plotting properties, that can be grouped together, for instance color or line style.
  /// </summary>
  public interface IPlotGroupStyle : ICloneable
  {
    /// <summary>
    /// Called at the beginning of the preparation.
    /// </summary>
    void BeginPrepare();
    /// <summary>
    /// Called at the end of the preparation.
    /// </summary>
    void EndPrepare();

    /// <summary>
    /// PrepareStep is called every time after for each PlotItem Prepare is called.
    /// </summary>
    void PrepareStep();

    /// <summary>
    /// Determines if this style can have childs, i.e. other plot group
    /// styles that are incremented if this group style swaps over.
    /// When <see cref="Step" /> can return only zero, you should return here false. Otherwise
    /// you should return true.
    /// </summary>
    /// <returns></returns>
    bool CanHaveChilds();
    /// <summary>
    /// Increments/decrements the style. Returns true when the style was swapped around.
    /// </summary>
    /// <param name="step">Either +1 or -1 for increment or decrement.</param>
    /// <returns>Number of wraps.</returns>
    int Step(int step);

    /// <summary>
    /// Get/sets whether or not stepping is allowed.
    /// </summary>
    bool IsStepEnabled { get; set; }

    /// <summary>
    /// Return true when this group style contains valid grouping data.
    /// You should set IsInitialized to false when BeginPrepare is called.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Transfers the core properties (but not the adminstrative properties like isStepEnabled) from another instance to this instance.
    /// </summary>
    /// <param name="from">The instance to copy the core properties from.</param>
    void TransferFrom(IPlotGroupStyle from);
  }
}
