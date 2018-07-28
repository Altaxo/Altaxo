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

using Altaxo.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Graph.Gdi
{
  /// <summary>
  /// Interaction logic for ArrangeLayersControl.xaml
  /// </summary>
  [UserControlForController(typeof(IArrangeLayersViewEventSink))]
  public partial class ArrangeLayersControl : UserControl, IArrangeLayersView
  {
    public ArrangeLayersControl()
    {
      InitializeComponent();
    }

    private void _edNumberOfRows_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
    {
      if (_controller != null)
        _controller.EhNumberOfRowsChanged(_edNumberOfRows.Value);
    }

    private void _edNumberOfColumns_Validating(object sender, RoutedPropertyChangedEventArgs<int> e)
    {
      if (_controller != null)
        _controller.EhNumberOfColumnsChanged(_edNumberOfColumns.Value);
    }

    private void _edRowSpacing_Validating(object sender, ValidationEventArgs<string> e)
    {
      bool Cancel = false;
      if (null != _controller)
        Cancel |= _controller.EhRowSpacingChanged(e.ValueToValidate);
      if (Cancel)
        e.AddError("The provided string could not be converted to a numeric value");
    }

    private void _edColumnSpacing_Validating(object sender, ValidationEventArgs<string> e)
    {
      bool Cancel = false;
      if (null != _controller)
        Cancel |= _controller.EhColumnSpacingChanged(e.ValueToValidate);
      if (Cancel)
        e.AddError("The provided string could not be converted to a numeric value");
    }

    private void _edTopMargin_Validating(object sender, ValidationEventArgs<string> e)
    {
      bool Cancel = false;
      if (_controller != null)
        Cancel |= _controller.EhTopMarginChanged(e.ValueToValidate);

      if (Cancel)
        e.AddError("The provided string could not be converted to a numeric value");
    }

    private void _edLeftMargin_Validating(object sender, ValidationEventArgs<string> e)
    {
      bool Cancel = false;
      if (_controller != null)
        Cancel |= _controller.EhLeftMarginChanged(e.ValueToValidate);

      if (Cancel)
        e.AddError("The provided string could not be converted to a numeric value");
    }

    private void _edBottomMargin_Validating(object sender, ValidationEventArgs<string> e)
    {
      bool Cancel = false;
      if (_controller != null)
        Cancel |= _controller.EhBottomMarginChanged(e.ValueToValidate);

      if (Cancel)
        e.AddError("The provided string could not be converted to a numeric value");
    }

    private void _edRightMargin_Validating(object sender, ValidationEventArgs<string> e)
    {
      bool Cancel = false;
      if (_controller != null)
        Cancel |= _controller.EhRightMarginChanged(e.ValueToValidate);

      if (Cancel)
        e.AddError("The provided string could not be converted to a numeric value");
    }

    private void _cbSuperfluousLayersAction_SelectionChangeCommitted(object sender, SelectionChangedEventArgs e)
    {
      if (_controller != null)
        _controller.EhSuperfluousLayersActionChanged((Altaxo.Collections.SelectableListNode)_cbSuperfluousLayersAction.SelectedItem);
    }

    #region IArrangeLayersView

    private IArrangeLayersViewEventSink _controller;

    public IArrangeLayersViewEventSink Controller
    {
      get
      {
        return _controller;
      }
      set
      {
        _controller = value;
      }
    }

    public void InitializeRowsColumns(int numRows, int numColumns)
    {
      this._edNumberOfRows.Value = numRows;
      this._edNumberOfColumns.Value = numColumns;
    }

    public void InitializeSpacing(double rowSpacing, double columnSpacing)
    {
      this._edRowSpacing.Text = GUIConversion.ToString(columnSpacing);
      this._edColumnSpacing.Text = GUIConversion.ToString(rowSpacing);
    }

    public void InitializeMargins(double top, double left, double bottom, double right)
    {
      this._edTopMargin.Text = GUIConversion.ToString(top);
      this._edBottomMargin.Text = GUIConversion.ToString(bottom);
      this._edRightMargin.Text = GUIConversion.ToString(right);
      this._edLeftMargin.Text = GUIConversion.ToString(left);
    }

    public void InitializeSuperfluosLayersQuestion(Collections.SelectableListNodeList list)
    {
      GuiHelper.Initialize(_cbSuperfluousLayersAction, list);
    }

    public void InitializeEnableConditions(bool rowSpacingEnabled, bool columnSpacingEnabled, bool superfluousEnabled)
    {
      _edRowSpacing.IsEnabled = rowSpacingEnabled;
      _edColumnSpacing.IsEnabled = columnSpacingEnabled;
      _cbSuperfluousLayersAction.IsEnabled = superfluousEnabled;
    }

    #endregion IArrangeLayersView
  }
}
