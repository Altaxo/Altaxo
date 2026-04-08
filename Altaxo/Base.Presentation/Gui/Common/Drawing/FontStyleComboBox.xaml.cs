#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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
using Altaxo.Drawing;
using Altaxo.Graph;

namespace Altaxo.Gui.Common.Drawing
{
  /// <summary>
  /// Interaction logic for FontStyleComboBox.xaml
  /// </summary>
  public partial class FontStyleComboBox : UserControl
  {
    /// <summary>
    /// Occurs when <see cref="SelectedFontStyle"/> changes.
    /// </summary>
    public event DependencyPropertyChangedEventHandler? SelectedFontStyleChanged;

    private Altaxo.Main.TemporaryDisabler _eventDisabler = new Altaxo.Main.TemporaryDisabler(EhEventsReenabled);

    /// <summary>
    /// Initializes a new instance of the <see cref="FontStyleComboBox"/> class.
    /// </summary>
    public FontStyleComboBox()
    {
      InitializeComponent();
    }

    #region Dependency property

    private const string _nameOfValueProp = "SelectedFontStyle";

    /// <summary>
    /// Gets or sets the selected font style.
    /// </summary>
    public FontXStyle SelectedFontStyle
    {
      get { return (FontXStyle)GetValue(SelectedFontStyleProperty); }
      set { SetValue(SelectedFontStyleProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="SelectedFontStyle"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SelectedFontStyleProperty =
        DependencyProperty.Register(_nameOfValueProp, typeof(FontXStyle), typeof(FontStyleComboBox),
        new FrameworkPropertyMetadata(FontXStyle.Regular, EhSelectedFontStyleChanged) { BindsTwoWayByDefault=true});

    private static void EhSelectedFontStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      ((FontStyleComboBox)obj).OnSelectedFontStyleChanged(obj, args);
    }

    /// <summary>
    /// Raises the <see cref="SelectedFontStyleChanged"/> event.
    /// </summary>
    /// <param name="obj">The dependency object whose font style changed.</param>
    /// <param name="args">The event arguments.</param>
    protected virtual void OnSelectedFontStyleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (SelectedFontStyleChanged is not null)
        SelectedFontStyleChanged(obj, args);

      using (var token = _eventDisabler.SuspendGetToken())
      {
        UpdateChecks(SelectedFontStyle);
      }
    }

    #endregion Dependency property

    private static void EhEventsReenabled()
    {
    }

    private void UpdateChecks(FontXStyle style)
    {
      _guiBold.IsChecked = style.HasFlag(FontXStyle.Bold);
      _guiItalic.IsChecked = style.HasFlag(FontXStyle.Italic);
      _guiUnderline.IsChecked = style.HasFlag(FontXStyle.Underline);
      _guiStrikeout.IsChecked = style.HasFlag(FontXStyle.Strikeout);
    }

    private void EhCheckChanged(object sender, RoutedEventArgs e)
    {
      if (!_eventDisabler.IsSuspended)
      {
        FontXStyle newStyle = FontXStyle.Regular;
        if (true == _guiBold.IsChecked)
          newStyle |= FontXStyle.Bold;
        if (true == _guiItalic.IsChecked)
          newStyle |= FontXStyle.Italic;
        if (true == _guiUnderline.IsChecked)
          newStyle |= FontXStyle.Underline;
        if (true == _guiStrikeout.IsChecked)
          newStyle |= FontXStyle.Strikeout;

        SetValue(SelectedFontStyleProperty, newStyle);
      }
    }
  }
}
