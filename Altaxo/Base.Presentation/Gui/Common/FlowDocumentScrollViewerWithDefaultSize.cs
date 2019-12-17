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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Common
{
  public class FlowDocumentScrollViewerWithDefaultSize : FlowDocumentScrollViewer
  {
    #region Dependency property

    public double DefaultWidth
    {
      get { return (double)GetValue(DefaultWidthProperty); }
      set { SetValue(DefaultWidthProperty, value); }
    }

    public double DefaultHeigth
    {
      get { return (double)GetValue(DefaultHeightProperty); }
      set { SetValue(DefaultHeightProperty, value); }
    }

    public static readonly DependencyProperty DefaultWidthProperty =
        DependencyProperty.Register("DefaultWidth", typeof(double), typeof(FlowDocumentScrollViewerWithDefaultSize),
        new FrameworkPropertyMetadata(100.0d));

    public static readonly DependencyProperty DefaultHeightProperty =
        DependencyProperty.Register("DefaultHeight", typeof(double), typeof(FlowDocumentScrollViewerWithDefaultSize),
        new FrameworkPropertyMetadata(100.0d));

    #endregion Dependency property

    protected override Size MeasureOverride(Size constraint)
    {
      double w = constraint.Width;
      double h = constraint.Height;

      if (!IsLoaded)
        return new Size(DefaultWidth, DefaultHeigth);
      else
        return new Size(Math.Min(ActualWidth, constraint.Width), Math.Min(ActualHeight, constraint.Height));
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      var result = base.ArrangeOverride(arrangeBounds);
      this.Document.PageWidth = result.Width;
      return result;
    }
  }
}
