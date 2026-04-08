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
  /// <summary>
  /// Rich text box that reports a configurable default size before it is loaded.
  /// </summary>
  public class RichTextBoxWithDefaultSize : RichTextBox
  {
    #region Dependency property

    /// <summary>
    /// Gets or sets the default width used before the control is loaded.
    /// </summary>
    public double DefaultWidth
    {
      get { return (double)GetValue(DefaultWidthProperty); }
      set { SetValue(DefaultWidthProperty, value); }
    }

    /// <summary>
    /// Gets or sets the default height used before the control is loaded.
    /// </summary>
    public double DefaultHeigth
    {
      get { return (double)GetValue(DefaultHeightProperty); }
      set { SetValue(DefaultHeightProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="DefaultWidth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultWidthProperty =
        DependencyProperty.Register("DefaultWidth", typeof(double), typeof(RichTextBoxWithDefaultSize),
        new FrameworkPropertyMetadata(100.0d));

    /// <summary>
    /// Identifies the <see cref="DefaultHeigth"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultHeightProperty =
        DependencyProperty.Register("DefaultHeight", typeof(double), typeof(RichTextBoxWithDefaultSize),
        new FrameworkPropertyMetadata(100.0d));

    #endregion Dependency property

    /// <inheritdoc/>
    protected override Size MeasureOverride(Size constraint)
    {
      double w = constraint.Width;
      double h = constraint.Height;

      if (!IsLoaded)
        return new Size(DefaultWidth, DefaultHeigth);
      else
        return new Size(ActualWidth, ActualHeight);
    }
  }
}
