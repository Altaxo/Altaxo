#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2012 Dr. Dirk Lellinger
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
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Common
{
  /// <summary>
  /// Limits the size of this border, and thus it's child, if the constraint for the size given as parameter in MeasureOverride is infinity. This is useful for limiting the height
  /// if ListBoxes etc. if they contain a lot of items. Simply place the ListBox in an instance of this class and set the <see cref="HeightGreedLimit"/> property.
  /// See remarks for more information, including the difference in usage between MaxHeight and HeightGreedLimit and MaxWidth and WidthGreedLimit.
  /// </summary>
  /// <remarks>
  /// <para>Some tests including to ListBoxes with 1000 items, embedded in a Grid, reveal the following:</para>
  /// <para>If the Grid is directly embedded in a Window, MeasureOverride is never called with Infinity, thus no problem.</para>
  /// <para>If the Grid is embedded in a UserControl, which is embedded in a Window, MeasureOverride is never called with Infinity, thus no problem.</para>
  /// <para>The only problem arises if Grid is embedded in a UserControl, and this UserControl is created <b>before</b> it is then embedded in the Window. Thus, during creation,
  /// UserControl has no parent, and MeasureOverride of the ListBoxes is called with both Width and Height set to Infinity. This causes that the ListBoxes reserve all space they can get.
  /// Additionally, it seems that they somehow remember this size, even if afterwards ArrangeOverride is called with finite boundaries.</para>
  /// <para>The solution is to limit the constraint values (width and/or height) in MeasureOverride, only (and really only) if MeasureOverride is called with infinite constraint values.
  /// </para>
  /// <para><b>Difference between MaxHeight and HeightGreedLimit or MaxWidth and WidthGreedLimit:</b></para>
  ///
  /// <para>With MinHeight (or MinWidth) you really set the limit for the Height or Width of the item. Even if the user increases the size of the parent window (for instance by mouse dragging), the item size is limited to this values.</para>
  /// <para>With HeightGreedLimit (or WidthGreedLimit) you limit the size during the creation stage of the user control. Thus, only the initial size will be limited, but the user can still increase the size of the items, for instance
  /// by increasing the size of the parent control.</para>
  /// </remarks>
  public class BorderWithSizeGreedLimit : Border
  {
    /// <summary>
    /// Property key for the <see cref="WidthGreedLimit"/> property.
    /// </summary>
    public static readonly DependencyProperty WidthGreedLimitProperty =
        DependencyProperty.Register("WidthGreedLimit", typeof(double), typeof(BorderWithSizeGreedLimit),
        new FrameworkPropertyMetadata(double.PositiveInfinity, new PropertyChangedCallback(EhWidthGreedLimitChanged), new CoerceValueCallback(CoerceWidthGreedLimit)));

    /// <summary>
    /// Property key for the <see cref="HeightGreedLimit"/> property.
    /// </summary>
    public static readonly DependencyProperty HeightGreedLimitProperty =
        DependencyProperty.Register("HeightGreedLimit", typeof(double), typeof(BorderWithSizeGreedLimit),
        new FrameworkPropertyMetadata(double.PositiveInfinity, new PropertyChangedCallback(EhHeightGreedLimitChanged), new CoerceValueCallback(CoerceHeightGreedLimit)));

    /// <summary>
    /// Gets/sets the limit for the width greed. This limits the initial width of the Border (and thus its child). The user is still able to increase the width of the Border above this value.
    /// </summary>
    public double WidthGreedLimit
    {
      get { return (double)GetValue(WidthGreedLimitProperty); }
      set { SetValue(WidthGreedLimitProperty, value); }
    }

    private static void EhWidthGreedLimitChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
    }

    private static object CoerceWidthGreedLimit(DependencyObject element, object value)
    {
      return Math.Max(0, (double)value);
    }

    /// <summary>
    /// Gets/sets the limit for the height greed. This limits the initial height of the Border (and thus its child). The user is still able to increase the height of the Border above this value.
    /// </summary>
    public double HeightGreedLimit
    {
      get { return (double)GetValue(HeightGreedLimitProperty); }
      set { SetValue(HeightGreedLimitProperty, value); }
    }

    private static void EhHeightGreedLimitChanged(DependencyObject element, DependencyPropertyChangedEventArgs args)
    {
    }

    private static object CoerceHeightGreedLimit(DependencyObject element, object value)
    {
      return Math.Max(0, (double)value);
    }

    /// <summary>
    /// Measures the child elements of a <see cref="T:System.Windows.Controls.Border"/> before they are arranged during the <see cref="M:System.Windows.Controls.Border.ArrangeOverride(System.Windows.Size)"/> pass.
    /// If (and only if) the constraint width (or height) is infinity, the constraint width (or height) is set to the WidthGreedLimit (or HeightGreedLimit) before calling MeasureOverride of the base class.
    /// </summary>
    /// <param name="constraint">An upper <see cref="T:System.Windows.Size"/> limit that cannot be exceeded.</param>
    /// <returns>
    /// The <see cref="T:System.Windows.Size"/> that represents the upper size limit of the element.
    /// </returns>
    protected override Size MeasureOverride(Size constraint)
    {
      //System.Diagnostics.Debug.Write(string.Format("BorderWithSizeGreedLimit Name={0} Measure Constraint={1}", Name, constraint));
      if (double.IsInfinity(constraint.Height))
        constraint.Height = HeightGreedLimit;

      if (double.IsInfinity(constraint.Width))
        constraint.Width = WidthGreedLimit;

      var result = base.MeasureOverride(constraint);
      //System.Diagnostics.Debug.WriteLine(", result={0}", result);

      return result;
    }
  }
}
