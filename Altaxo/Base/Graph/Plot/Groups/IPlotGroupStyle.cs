#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
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

#nullable enable
using System;
using System.Collections.Generic;
using System.Text;

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// Represents plotting properties that can be grouped together, for instance color or line style.
  /// </summary>
  public interface IPlotGroupStyle : Main.IDocumentLeafNode, ICloneable
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
    /// Prepares the next step after each plot item has been prepared.
    /// </summary>
    void PrepareStep();

    /// <summary>
    /// Determines whether this style can carry over during the <see cref="Step"/> operation. If yes, then the style can have children, i.e. other plot group
    /// styles that are incremented if this group style carries over.
    /// When <see cref="Step" /> can return a nonzero value, true is returned here.
    /// For instance, color or line style can be stepped and can carry over, so they return true here. In contrast, a bar graph group style
    /// can be stepped, but it cannot carry over, so false is returned here.
    /// </summary>
    bool CanCarryOver { get; }

    /// <summary>
    /// Increments/decrements the style. Returns +1 or -1 when the style was swapped around.
    /// </summary>
    /// <param name="step">Either +1 or -1 for increment or decrement.</param>
    /// <returns>Number of wraps.</returns>
    int Step(int step);

    /// <summary>
    /// Indicates whether this group style makes any action when calling <see cref="Step"/>. This property is true, if <see cref="IsStepEnabled"/> can be set to true.
    /// </summary>
    bool CanStep { get; }

    /// <summary>
    /// Gets or sets whether stepping is allowed.
    /// </summary>
    bool IsStepEnabled { get; set; }

    /// <summary>
    /// Returns <c>true</c> when this group style contains valid grouping data.
    /// You should set <see cref="IsInitialized"/> to <c>false</c> when <see cref="BeginPrepare"/> is called.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Transfers the core properties (but not the adminstrative properties like isStepEnabled) from another instance to this instance.
    /// </summary>
    /// <param name="from">The instance to copy the core properties from.</param>
    void TransferFrom(IPlotGroupStyle from);
  }
}
