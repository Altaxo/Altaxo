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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows;
using System.Windows.Media;

namespace Altaxo.Gui
{
  // Create a host visual derived from the FrameworkElement class.
  // This class provides layout, event handling, and container support for one drawing.
  /// <summary>
  /// Hosts a single drawing visual inside a <see cref="FrameworkElement"/>.
  /// </summary>
  public class VisualHost : FrameworkElement
  {
    // Create a collection of child visual objects.
    private DrawingVisual _child;

    private Action<DrawingContext> _renderMethod;

    /// <summary>
    /// Initializes a new instance of the <see cref="VisualHost"/> class.
    /// </summary>
    public VisualHost()
      : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VisualHost"/> class.
    /// </summary>
    /// <param name="renderMethod">The delegate used to render the drawing.</param>
    public VisualHost(Action<DrawingContext> renderMethod)
    {
      _child = new DrawingVisual();
      _renderMethod = renderMethod;
    }

    /// <summary>
    /// Opens the drawing context for the hosted visual.
    /// </summary>
    /// <returns>The drawing context for the hosted visual.</returns>
    public DrawingContext OpenDrawingContext()
    {
      return _child.RenderOpen();
    }

    /// <summary>
    /// Invalidates the drawing. As a result, the render routine is called (asynchronously).
    /// </summary>
    public void InvalidateDrawing()
    {
      // Only this three commands: visibility=false, InvalidateVisual() and visibility=true will result in invalidating the VisualHost and thus a redrawing
      Visibility = Visibility.Hidden;
      InvalidateVisual();
      Visibility = Visibility.Visible;
    }

    // Provide a required override for the VisualChildrenCount property.
    /// <inheritdoc/>
    protected override int VisualChildrenCount
    {
      get { return 1; }
    }

    // Provide a required override for the GetVisualChild method.
    /// <inheritdoc/>
    protected override Visual GetVisualChild(int index)
    {
      if (index != 0)
      {
        throw new ArgumentOutOfRangeException();
      }

      return _child;
    }

    /// <inheritdoc/>
    protected override void OnRender(DrawingContext drawingContext)
    {
      if (_renderMethod is not null)
        _renderMethod(drawingContext);

      base.OnRender(drawingContext);
    }
  }
}
