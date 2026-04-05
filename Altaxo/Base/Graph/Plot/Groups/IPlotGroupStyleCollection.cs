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

namespace Altaxo.Graph.Plot.Groups
{
  /// <summary>
  /// Bundles plot group styles into a collection. It is allowed only to add a single instance of every plot group class.
  /// </summary>
  public interface IPlotGroupStyleCollection : IEnumerable<IPlotGroupStyle>, Main.ICopyFrom
  {
    /// <summary>
    /// Adds a group style as child of an already present group style.
    /// </summary>
    /// <param name="groupStyle">The group style to add.</param>
    /// <param name="parentGroupStyleType">The type of the group style that is the parent of the group style to add. A group style of this type must be already present in the collection.</param>
    void Add(IPlotGroupStyle groupStyle, Type parentGroupStyleType);

    /// <summary>
    /// Adds a group style to this collection.
    /// </summary>
    /// <param name="groupStyle">The group style to add.</param>
    void Add(IPlotGroupStyle groupStyle);

    /// <summary>
    /// Removes all group styles from the collection.
    /// </summary>
    void Clear();

    /// <summary>
    /// Removes a group style of the given type from the collection.
    /// </summary>
    /// <param name="groupType">The type of the group style to remove.</param>
    void RemoveType(Type groupType);

    /// <summary>
    /// Test whether or not a group style of the given type is present in this collection.
    /// </summary>
    /// <param name="groupStyleType">Type of the group style.</param>
    /// <returns>True if a group style of the given type is contained in the collection.</returns>
    bool ContainsType(Type groupStyleType);

    /// <summary>
    /// Number of group styles in the collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Tests if the group style of the given type has a child group style and returns the type of the child group style (or null if no such child exists).
    /// </summary>
    /// <param name="groupStyleType">Type of the group style to test.</param>
    /// <returns>Type of the child group style, or null if no child exists.</returns>
    Type? GetTypeOfChild(Type groupStyleType);

    /// <summary>
    /// Returns the group style instance of the type given.
    /// </summary>
    /// <param name="groupStyleType">The type of the group style.</param>
    /// <returns>The instance of the given type, if it is contained in the collection. Throws an exeption, if no such instance (of the given type) is contained in the collection.</returns>
    IPlotGroupStyle GetPlotGroupStyle(Type groupStyleType);

    /// <summary>
    /// Returns the plot group strictness of this plot group, i.e. how the plot group is updated.
    /// </summary>
    PlotGroupStrictness PlotGroupStrictness { get; }

    /// <summary>
    /// Begins the preparation phase for all group styles in the collection.
    /// </summary>
    void BeginPrepare();

    /// <summary>
    /// Ends the preparation phase for all group styles in the collection.
    /// </summary>
    void EndPrepare();

    /// <summary>
    /// Begins the application phase for all group styles in the collection.
    /// </summary>
    void BeginApply();

    /// <summary>
    /// Ends the application phase for all group styles in the collection.
    /// </summary>
    void EndApply();

    /// <summary>
    /// Called before a group style of the specified type is applied.
    /// </summary>
    /// <param name="groupStyleType">The group style type that is about to be applied.</param>
    void OnBeforeApplication(Type groupStyleType);

    /// <summary>
    /// Advances preparation by one step for all group styles.
    /// </summary>
    void PrepareStep();

    /// <summary>
    /// Advances all step-enabled group styles.
    /// </summary>
    /// <param name="step">The step direction and magnitude, usually <c>+1</c> or <c>-1</c>.</param>
    void Step(int step);
  }
}
