#region Copyright

/////////////////////////////////////////////////////////////////////////////
//
// BSD 3-Clause License
//
// Copyright(c) 2015-16, Jan Karger(Steven Kirk)
//
// All rights reserved.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GongSolutions.Wpf.DragDrop
{
  /// <summary>
  /// Provides access to the standard drop target adorner types.
  /// </summary>
  public class DropTargetAdorners
  {
    /// <summary>
    /// Gets the adorner type that highlights the target item.
    /// </summary>
    public static Type Highlight
    {
      get { return typeof(DropTargetHighlightAdorner); }
    }

    /// <summary>
    /// Gets the adorner type that shows an insertion marker.
    /// </summary>
    public static Type Insert
    {
      get { return typeof(DropTargetInsertionAdorner); }
    }
  }
}
