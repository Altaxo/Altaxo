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

using Altaxo.Main.Properties;
using Altaxo.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Gui
{
  public static class RelationEnvironment
  {
    private static QuantityWithUnitGuiEnvironment _instance;

    static RelationEnvironment()
    {
      _instance = new QuantityWithUnitGuiEnvironment(GuiRelationUnits.Collection)
      {
        DefaultUnit = Current.PropertyService.GetValue(PropertyKeyDefaultUnit, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin) //   new Units.PrefixedUnit(Units.SIPrefix.None, Altaxo.Units.Dimensionless.Unity.Instance)
      };

      _instance.DefaultUnitChanged += EhDefaultUnitChanged;
    }

    /// <summary>
    /// Gets the common position environment for all position boxes.
    /// </summary>
    public static QuantityWithUnitGuiEnvironment Instance
    {
      get
      {
        return _instance;
      }
    }

    private static void EhDefaultUnitChanged(object sender, EventArgs e)
    {
      Current.PropertyService.SetValue(PropertyKeyDefaultUnit, _instance.DefaultUnit);
    }

    public static readonly PropertyKey<IPrefixedUnit> PropertyKeyDefaultUnit =
      new PropertyKey<IPrefixedUnit>(
      "3D70027C-2582-4112-BD72-BF2CAC395939",
      "Units\\DefaultRelationUnit",
      PropertyLevel.Application,
      () => new PrefixedUnit(SIPrefix.None, Altaxo.Units.Dimensionless.Unity.Instance));
  }

  public static class AngleEnvironment
  {
    private static QuantityWithUnitGuiEnvironment _instance;

    static AngleEnvironment()
    {
      _instance = new QuantityWithUnitGuiEnvironment(GuiAngleUnits.Collection)
      {
        DefaultUnit = Current.PropertyService.GetValue(PropertyKeyDefaultUnit, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin)
      };
      _instance.DefaultUnitChanged += EhDefaultUnitChanged;
    }

    /// <summary>
    /// Gets the common position environment for all position boxes.
    /// </summary>
    public static QuantityWithUnitGuiEnvironment Instance
    {
      get
      {
        return _instance;
      }
    }

    private static void EhDefaultUnitChanged(object sender, EventArgs e)
    {
      Current.PropertyService.SetValue(PropertyKeyDefaultUnit, _instance.DefaultUnit);
    }

    public static readonly PropertyKey<IPrefixedUnit> PropertyKeyDefaultUnit =
      new PropertyKey<IPrefixedUnit>(
      "CB04CD5A-2A34-451A-A5FA-00C9C9C000A0",
      "Units\\DefaultAngleUnit",
      PropertyLevel.Application,
      () => new PrefixedUnit(SIPrefix.None, Altaxo.Units.Angle.Degree.Instance));
  }

  public static class PositionEnvironment
  {
    private static QuantityWithUnitGuiEnvironment _instance;

    static PositionEnvironment()
    {
      _instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
      {
        DefaultUnit = Current.PropertyService.GetValue(PropertyKeyDefaultUnit, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin)
      };
      _instance.DefaultUnitChanged += EhDefaultUnitChanged;
    }

    /// <summary>
    /// Gets the common position environment for all position boxes.
    /// </summary>
    public static QuantityWithUnitGuiEnvironment Instance
    {
      get
      {
        return _instance;
      }
    }

    private static void EhDefaultUnitChanged(object sender, EventArgs e)
    {
      Current.PropertyService.SetValue(PropertyKeyDefaultUnit, _instance.DefaultUnit);
    }

    public static readonly PropertyKey<IPrefixedUnit> PropertyKeyDefaultUnit =
    new PropertyKey<IPrefixedUnit>(
    "8979E39B-5D9C-4C50-966D-5B64EEEB97F2",
    "Units\\DefaultPositionUnit",
    PropertyLevel.Application,
    () => new PrefixedUnit(SIPrefix.None, Altaxo.Units.Length.Point.Instance));
  }

  public static class SizeEnvironment
  {
    private static QuantityWithUnitGuiEnvironment _instance;

    static SizeEnvironment()
    {
      _instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
      {
        DefaultUnit = Current.PropertyService.GetValue(PropertyKeyDefaultUnit, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin)
      };
      _instance.DefaultUnitChanged += EhDefaultUnitChanged;
    }

    /// <summary>
    /// Gets the common size environment for all size boxes.
    /// </summary>
    public static QuantityWithUnitGuiEnvironment Instance
    {
      get
      {
        return _instance;
      }
    }

    private static void EhDefaultUnitChanged(object sender, EventArgs e)
    {
      Current.PropertyService.SetValue(PropertyKeyDefaultUnit, _instance.DefaultUnit);
    }

    public static readonly PropertyKey<IPrefixedUnit> PropertyKeyDefaultUnit =
    new PropertyKey<IPrefixedUnit>(
    "30AC5D99-A5B8-4358-A978-4DC530B7E601",
    "Units\\DefaultSizeUnit",
    PropertyLevel.Application,
    () => new PrefixedUnit(SIPrefix.None, Altaxo.Units.Length.Point.Instance));
  }

  public static class FontSizeEnvironment
  {
    private static QuantityWithUnitGuiEnvironment _instance;

    static FontSizeEnvironment()
    {
      _instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
      {
        DefaultUnit = Current.PropertyService.GetValue(PropertyKeyDefaultUnit, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin)
      };
      _instance.DefaultUnitChanged += EhDefaultUnitChanged;
    }

    /// <summary>
    /// Gets the common position environment for all position boxes.
    /// </summary>
    public static QuantityWithUnitGuiEnvironment Instance
    {
      get
      {
        return _instance;
      }
    }

    private static void EhDefaultUnitChanged(object sender, EventArgs e)
    {
      Current.PropertyService.SetValue(PropertyKeyDefaultUnit, _instance.DefaultUnit);
    }

    public static readonly PropertyKey<IPrefixedUnit> PropertyKeyDefaultUnit =
    new PropertyKey<IPrefixedUnit>(
    "A4C55ABB-3499-4A01-B10E-E5CB91679B7E",
    "Units\\DefaultFontSizeUnit",
    PropertyLevel.Application,
    () => new PrefixedUnit(SIPrefix.None, Altaxo.Units.Length.Point.Instance));
  }

  public static class LineCapSizeEnvironment
  {
    private static QuantityWithUnitGuiEnvironment _instance;

    static LineCapSizeEnvironment()
    {
      _instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
      {
        DefaultUnit = Current.PropertyService.GetValue(PropertyKeyDefaultUnit, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin)
      };
      _instance.DefaultUnitChanged += EhDefaultUnitChanged;
    }

    /// <summary>
    /// Gets the common position environment for all position boxes.
    /// </summary>
    public static QuantityWithUnitGuiEnvironment Instance
    {
      get
      {
        return _instance;
      }
    }

    private static void EhDefaultUnitChanged(object sender, EventArgs e)
    {
      Current.PropertyService.SetValue(PropertyKeyDefaultUnit, _instance.DefaultUnit);
    }

    public static readonly PropertyKey<IPrefixedUnit> PropertyKeyDefaultUnit =
    new PropertyKey<IPrefixedUnit>(
    "0091F3B2-8996-42A6-8426-60E9919ABCC4",
    "Units\\DefaultLineCapSizeUnit",
    PropertyLevel.Application,
    () => new PrefixedUnit(SIPrefix.None, Altaxo.Units.Length.Point.Instance));
  }

  public static class LineThicknessEnvironment
  {
    private static QuantityWithUnitGuiEnvironment _instance;

    static LineThicknessEnvironment()
    {
      _instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
      {
        DefaultUnit = Current.PropertyService.GetValue(PropertyKeyDefaultUnit, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin)
      };
      _instance.DefaultUnitChanged += EhDefaultUnitChanged;
    }

    /// <summary>
    /// Gets the common position environment for all position boxes.
    /// </summary>
    public static QuantityWithUnitGuiEnvironment Instance
    {
      get
      {
        return _instance;
      }
    }

    private static void EhDefaultUnitChanged(object sender, EventArgs e)
    {
      Current.PropertyService.SetValue(PropertyKeyDefaultUnit, _instance.DefaultUnit);
    }

    public static readonly PropertyKey<IPrefixedUnit> PropertyKeyDefaultUnit =
    new PropertyKey<IPrefixedUnit>(
    "5160F793-463A-484A-9E1B-67B22482C56C",
    "Units\\DefaultLineThicknessUnit",
    PropertyLevel.Application,
    () => new PrefixedUnit(SIPrefix.None, Altaxo.Units.Length.Point.Instance));
  }

  public static class MiterLimitEnvironment
  {
    private static QuantityWithUnitGuiEnvironment _instance;

    static MiterLimitEnvironment()
    {
      _instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
      {
        DefaultUnit = Current.PropertyService.GetValue(PropertyKeyDefaultUnit, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin)
      };
      _instance.DefaultUnitChanged += EhDefaultUnitChanged;
    }

    /// <summary>
    /// Gets the common position environment for all position boxes.
    /// </summary>
    public static QuantityWithUnitGuiEnvironment Instance
    {
      get
      {
        return _instance;
      }
    }

    private static void EhDefaultUnitChanged(object sender, EventArgs e)
    {
      Current.PropertyService.SetValue(PropertyKeyDefaultUnit, _instance.DefaultUnit);
    }

    public static readonly PropertyKey<IPrefixedUnit> PropertyKeyDefaultUnit =
    new PropertyKey<IPrefixedUnit>(
    "C04AA492-9328-4D07-985D-57A20A708495",
    "Units\\DefaultMiterLimitUnit",
    PropertyLevel.Application,
    () => new PrefixedUnit(SIPrefix.None, Altaxo.Units.Length.Point.Instance));
  }

  public static class PaperMarginEnvironment
  {
    private static QuantityWithUnitGuiEnvironment _instance;

    static PaperMarginEnvironment()
    {
      _instance = new QuantityWithUnitGuiEnvironment(GuiLengthUnits.Collection)
      {
        DefaultUnit = Current.PropertyService.GetValue(PropertyKeyDefaultUnit, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin)
      };
      _instance.DefaultUnitChanged += EhDefaultUnitChanged;
    }

    /// <summary>
    /// Gets the common position environment for all position boxes.
    /// </summary>
    public static QuantityWithUnitGuiEnvironment Instance
    {
      get
      {
        return _instance;
      }
    }

    private static void EhDefaultUnitChanged(object sender, EventArgs e)
    {
      Current.PropertyService.SetValue(PropertyKeyDefaultUnit, _instance.DefaultUnit);
    }

    public static readonly PropertyKey<IPrefixedUnit> PropertyKeyDefaultUnit =
    new PropertyKey<IPrefixedUnit>(
    "F0D18F89-F159-4DC9-B20B-ACDFC14EF1D1",
    "Units\\DefaultPaperMarginUnit",
    PropertyLevel.Application,
    () => new PrefixedUnit(SIPrefix.None, Altaxo.Units.Length.Point.Instance));
  }

  public static class TimeEnvironment
  {
    private static QuantityWithUnitGuiEnvironment _instance;

    static TimeEnvironment()
    {
      _instance = new QuantityWithUnitGuiEnvironment(GuiTimeUnits.Collection)
      {
        DefaultUnit = Current.PropertyService.GetValue(PropertyKeyDefaultUnit, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin)
      };
      _instance.DefaultUnitChanged += EhDefaultUnitChanged;
    }

    /// <summary>
    /// Gets the common size environment for all size boxes.
    /// </summary>
    public static QuantityWithUnitGuiEnvironment Instance
    {
      get
      {
        return _instance;
      }
    }

    private static void EhDefaultUnitChanged(object sender, EventArgs e)
    {
      Current.PropertyService.SetValue(PropertyKeyDefaultUnit, _instance.DefaultUnit);
    }

    public static readonly PropertyKey<IPrefixedUnit> PropertyKeyDefaultUnit =
    new PropertyKey<IPrefixedUnit>(
    "4F742DC7-60DE-4FAB-ABF7-F72E489E6A6E",
    "Units\\DefaultTimeUnit",
    PropertyLevel.Application,
    () => new PrefixedUnit(SIPrefix.None, Altaxo.Units.Time.Second.Instance));
  }
}
