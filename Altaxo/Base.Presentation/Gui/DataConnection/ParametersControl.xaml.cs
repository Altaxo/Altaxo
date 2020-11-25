#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Altaxo.DataConnection;

namespace Altaxo.Gui.DataConnection
{
  /// <summary>
  /// Interaction logic for ParametersControl.xaml
  /// </summary>
  public partial class ParametersControl : UserControl, IParametersView
  {
    private CultureInfo _invariant = CultureInfo.InvariantCulture;
    private DateTimeStyles _dateStyle = DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal;
    private NumberStyles _numberStyle = NumberStyles.Any;
    private Control _focus;

    public ParametersControl()
    {
      InitializeComponent();
    }

    public void SetParametersSource(List<System.Data.OleDb.OleDbParameter> parms)
    {
      int nRow = -1;
      foreach (var p in parms)
      {
        _grid.RowDefinitions.Add(new RowDefinition());
        ++nRow;

        // create label
        var lbl = new Label
        {
          Content = CleanupName(p.ParameterName)
        };
        lbl.SetValue(Grid.RowProperty, nRow);
        _grid.Children.Add(lbl);

        // create input control
        Control ctl = GetControl(p);
        ctl.Tag = p;
        ctl.SetValue(Grid.RowProperty, nRow);
        ctl.SetValue(Grid.ColumnProperty, 1);
        _grid.Children.Add(ctl);
        if (_focus is null)
        {
          _focus = ctl;
        }
      }
    }

    public void ReadParameter()
    {
      foreach (Control ctl in _grid.Children)
      {
        var p = ctl.Tag as OleDbParameter;
        if (p is not null)
        {
          var chk = ctl as CheckBox;
          if (chk is not null)
          {
            p.Value = chk.IsChecked.ToString();
            continue;
          }
          var dtp = ctl as DatePicker;
          if (dtp is not null)
          {
            p.Value = dtp.SelectedDate.Value.ToString(_invariant);
            continue;
          }
          var textBox = ctl as TextBox;
          if (textBox is not null)
          {
            p.Value = textBox.Text;
            continue;
          }
        }
      }
    }

    // clean up a name (remove brackets etc)
    private static string CleanupName(string name)
    {
      name = name.Replace('_', ' ');
      name = name.Trim();
      var pos = name.LastIndexOf('!');
      if (pos > -1)
      {
        name = name.Substring(pos + 1);
      }
      if (name.Length > 1 && name[0] == '[' && name[name.Length - 1] == ']')
      {
        name = name.Substring(1, name.Length - 2);
      }
      return name;
    }

    // creates a control of appropriate type for the parameter.
    // note: parameter values are stored as invariant strings.
    private Control GetControl(OleDbParameter p)
    {
      var value = p.Value as string;
      var type = OleDbSchema.GetType(p.OleDbType);
      if (OleDbSchema.IsNumeric(type))
      {
        var num = new Altaxo.Gui.Common.DecimalUpDown
        {
          Minimum = decimal.MinValue,
          Maximum = decimal.MaxValue
        };
        decimal dec = 0;
        if (!string.IsNullOrEmpty(value))
        {
          decimal.TryParse(value, _numberStyle, _invariant, out dec);
        }
        num.Value = dec;
        return num;
      }
      if (type == typeof(DateTime))
      {
        var dtp = new DatePicker
        {
          SelectedDateFormat = p.OleDbType == OleDbType.Filetime
            ? DatePickerFormat.Long
            : DatePickerFormat.Short
        };
        DateTime dt = DateTime.Now;
        if (!string.IsNullOrEmpty(value))
        {
          DateTime.TryParse(value, _invariant, _dateStyle, out dt);
        }
        dtp.SelectedDate = dt;
        return dtp;
      }
      if (type == typeof(bool))
      {
        var chk = new CheckBox();
        bool b = false;
        if (!string.IsNullOrEmpty(value))
        {
          bool.TryParse(value as string, out b);
        }
        chk.IsChecked = b;
        return chk;
      }

      // default: textbox
      var tb = new TextBox
      {
        Text = value
      };
      return tb;
    }
  }
}
