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
using System.Collections.Generic;
using System.Linq;
using Altaxo.AddInItems;
using Altaxo.Graph.Gdi;
using Altaxo.Gui.AddInItems;

namespace Altaxo.Graph.Commands
{
  public class FontSizeChooser : ToolBarComboBox
  {
    public FontSizeChooser(Codon codon, object caller, IEnumerable<ICondition> conditions) : base(codon, caller, conditions)
    {
    }

    public override void InitializeContent()
    {
      IsEditable = true;
      Items.Add("8 pt");
      Items.Add("10 pt");
      Items.Add("12 pt");
      Items.Add("24 pt");
    }

    public override void Execute(object parameter)
    {
      if (string.IsNullOrEmpty(Text))
        return;

      var ctrl = Current.Workbench.ActiveViewContent as Altaxo.Gui.Graph.Gdi.Viewing.GraphController;

      if (ctrl is null)
        return;

      Altaxo.Serialization.LengthUnit unit = Altaxo.Serialization.LengthUnit.Point;
      if (Altaxo.Serialization.GUIConversion.IsDouble(Text, out var value) ||
        (Altaxo.Serialization.LengthUnit.TryParse(Text, out unit, out var number) &&
          Altaxo.Serialization.GUIConversion.IsDouble(number, out value)))
      {
        if (unit is not null)
          unit = Altaxo.Serialization.LengthUnit.Point;
        string normalizedEntry = Altaxo.Serialization.GUIConversion.ToString(value) + " " + unit.Shortcut;
        value *= (double)(unit.UnitInMeter / Altaxo.Serialization.LengthUnit.Point.UnitInMeter);

        ctrl.SetSelectedObjectsProperty(new RoutedSetterProperty<double>("FontSize", value));

        if (!Items.Contains(normalizedEntry))
          Items.Add(normalizedEntry);
      }
    }
  }

  public class StrokeWidthChooser : ToolBarComboBox
  {
    public StrokeWidthChooser(Codon codon, object caller, IEnumerable<ICondition> conditions) : base(codon, caller, conditions)
    {
    }

    public override void InitializeContent()
    {
      IsEditable = true;

      Items.Add("8 pt");
      Items.Add("10 pt");
      Items.Add("12 pt");
      Items.Add("24 pt");
    }

    public override void Execute(object parameter)
    {
      if (string.IsNullOrEmpty(Text))
        return;

      var ctrl = Current.Workbench.ActiveViewContent as Altaxo.Gui.Graph.Gdi.Viewing.GraphController;

      if (ctrl is null)
        return;

      Altaxo.Serialization.LengthUnit unit = Altaxo.Serialization.LengthUnit.Point;
      if (Altaxo.Serialization.GUIConversion.IsDouble(Text, out var value) ||
        (Altaxo.Serialization.LengthUnit.TryParse(Text, out unit, out var number) &&
          Altaxo.Serialization.GUIConversion.IsDouble(number, out value)))
      {
        if (unit is not null)
          unit = Altaxo.Serialization.LengthUnit.Point;
        string normalizedEntry = Altaxo.Serialization.GUIConversion.ToString(value) + " " + unit.Shortcut;
        value *= (double)(unit.UnitInMeter / Altaxo.Serialization.LengthUnit.Point.UnitInMeter);

        ctrl.SetSelectedObjectsProperty(new RoutedSetterProperty<double>("StrokeWidth", value));

        if (!Items.Contains(normalizedEntry))
          Items.Add(normalizedEntry);
      }
    }
  }

  public class FontFamilyChooser : ToolBarComboBox
  {
    public FontFamilyChooser(Codon codon, object caller, IEnumerable<ICondition> conditions) : base(codon, caller, conditions)
    {
    }

    public override void InitializeContent()
    {
      // Fill with all available font families
      foreach (var famName in GdiFontManager.EnumerateAvailableGdiFontFamilyNames().OrderBy(x => x))
        Items.Add(famName);
    }

    public override void Execute(object parameter)
    {
      if (string.IsNullOrEmpty(Text))
        return;

      var ctrl = Current.Workbench.ActiveViewContent as Altaxo.Gui.Graph.Gdi.Viewing.GraphController;
      if (ctrl is null)
        return;

      ctrl.SetSelectedObjectsProperty(new RoutedSetterProperty<string>("FontFamily", Text));
    }
  }
}
