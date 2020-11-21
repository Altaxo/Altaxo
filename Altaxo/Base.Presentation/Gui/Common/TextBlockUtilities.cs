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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Common
{
  public class TextBlockUtilities
  {
    /// <summary>
    /// Gets the value of the AutoTooltipProperty dependency property
    /// </summary>
    public static bool GetAutoTooltip(DependencyObject obj)
    {
      return (bool)obj.GetValue(AutoTooltipProperty);
    }

    /// <summary>
    /// Sets the value of the AutoTooltipProperty dependency property
    /// </summary>
    public static void SetAutoTooltip(DependencyObject obj, bool value)
    {
      obj.SetValue(AutoTooltipProperty, value);
    }

    /// <summary>
    /// Identified the attached AutoTooltip property. When true, this will set the TextBlock TextTrimming
    /// property to WordEllipsis, and display a tooltip with the full text whenever the text is trimmed.
    /// </summary>
    public static readonly DependencyProperty AutoTooltipProperty = DependencyProperty.RegisterAttached("AutoTooltip",
            typeof(bool), typeof(TextBlockUtilities), new PropertyMetadata(false, OnAutoTooltipPropertyChanged));

    private static void OnAutoTooltipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var textBlock = d as TextBlock;
      if (textBlock is null)
        return;

      if (e.NewValue.Equals(true))
      {
        ComputeAutoTooltip(textBlock);
        textBlock.SizeChanged += EhTextBlockSizeChanged;
        var dpd = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
        dpd.AddValueChanged(d, EhTextBlockTextChanged);
      }
      else
      {
        textBlock.SizeChanged -= EhTextBlockSizeChanged;
        var dpd = DependencyPropertyDescriptor.FromProperty(TextBlock.TextProperty, typeof(TextBlock));
        dpd.RemoveValueChanged(d, EhTextBlockTextChanged);
      }
    }

    private static void EhTextBlockSizeChanged(object sender, EventArgs e)
    {
      var textBlock = sender as TextBlock;
      ComputeAutoTooltip(textBlock);
    }

    private static void EhTextBlockTextChanged(object? sender, EventArgs e)
    {
      var textBlock = sender as TextBlock;
      ComputeAutoTooltip(textBlock);
    }

    /// <summary>
    /// Assigns the ToolTip for the given TextBlock based on whether the text is trimmed
    /// </summary>
    private static void ComputeAutoTooltip(TextBlock textBlock)
    {
      textBlock.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
      var width = textBlock.DesiredSize.Width;

      if (textBlock.ActualWidth < width)
      {
        ToolTipService.SetToolTip(textBlock, textBlock.Text);
      }
      else
      {
        ToolTipService.SetToolTip(textBlock, null);
      }
    }
  }
}
