#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvalonDock.Layout;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Helper class to ensure that tool panes are initially docked to their default positions
  /// </summary>
  public class LayoutUpdateStrategy : ILayoutUpdateStrategy
  {
    public static LayoutUpdateStrategy Instance { get; private set; } = new LayoutUpdateStrategy();

    public void AfterInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableShown)
    {
    }

    public void AfterInsertDocument(LayoutRoot layout, LayoutDocument anchorableShown)
    {
    }

    /// <summary>
    /// Ensure that tool panes are initially docked to their default positions.
    /// </summary>
    /// <param name="layout">The root of the docking layout.</param>
    /// <param name="anchorableToShow">The anchorable to show.</param>
    /// <param name="destinationContainer">The destination container.</param>
    /// <returns></returns>
    public bool BeforeInsertAnchorable(LayoutRoot layout, LayoutAnchorable anchorableToShow, ILayoutContainer destinationContainer)
    {
      if (anchorableToShow.Content is IPadContent padContent)
      {
        if (destinationContainer?.FindParent<LayoutFloatingWindow>() is not null)
          return false; // we do not handle bringing pads in a floating layout

        var defaultPosition = padContent.DefaultPosition;

        if (defaultPosition == DefaultPadPositions.Hidden)
        {
          layout.Hidden.Add(anchorableToShow); // Pads hidden by default are added to the hidden collection
          return true;
        }

        // search for the appropriate LayoutAnchorablePane
        LayoutAnchorablePane? destinationPane = null;

        AnchorSide? anchorSide = DefaultPadPosition_To_AnchorSide(defaultPosition);
        if (anchorSide.HasValue && ((destinationPane = layout.Descendents().OfType<LayoutAnchorablePane>().FirstOrDefault(p => p.GetSide() == anchorSide.Value)) is not null))
        {
          destinationPane.Children.Add(anchorableToShow);
          return true;
        }
      }

      return false; // no special strategy here
    }

    private AnchorSide? DefaultPadPosition_To_AnchorSide(DefaultPadPositions padPosition)
    {
      AnchorSide? result = null;
      switch (padPosition)
      {
        case DefaultPadPositions.Right:
          result = AnchorSide.Right;
          break;

        case DefaultPadPositions.Left:
          result = AnchorSide.Left;
          break;

        case DefaultPadPositions.Bottom:
          result = AnchorSide.Bottom;
          break;

        case DefaultPadPositions.Top:
          result = AnchorSide.Top;
          break;
      }
      return result;
    }

    public bool BeforeInsertDocument(LayoutRoot layout, LayoutDocument anchorableToShow, ILayoutContainer destinationContainer)
    {
      return false; // no special strategy here
    }
  }
}
