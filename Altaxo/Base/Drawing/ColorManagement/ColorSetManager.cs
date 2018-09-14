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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Graph;
using Altaxo.Main;
using Altaxo.Serialization.Xml;

namespace Altaxo.Drawing.ColorManagement
{
  public class ColorSetManagerEntryValue : StyleListManagerBaseEntryValue<IColorSet, NamedColor>
  {
    public bool IsPlotColorSet { get; private set; }

    public ColorSetManagerEntryValue(IColorSet colorSet, Main.ItemDefinitionLevel level, bool isPlotColorSet)
      : base(colorSet, level)
    {
      IsPlotColorSet = isPlotColorSet;
    }
  }

  /// <summary>
  /// Manages the set of colors for the application. This class has only a single instance (see <see cref="Instance"/>).
  /// </summary>
  public class ColorSetManager : StyleListManagerBase<IColorSet, NamedColor, ColorSetManagerEntryValue>
  {
    public static readonly Main.Properties.PropertyKey<ColorSetBag> PropertyKeyUserDefinedColorSets;

    /// <summary>
    /// Stores the only instance of this class.
    /// </summary>
    private static ColorSetManager _instance;

    private IColorSet _builtinKnownColors;
    private IColorSet _builtinDarkPlotColors;

    static ColorSetManager()
    {
      PropertyKeyUserDefinedColorSets =
        new Main.Properties.PropertyKey<ColorSetBag>(
        "98C37A90-D75A-4058-B6C6-8C180F37DD71",
        "Graph\\Colors\\UserDefinedColors",
        Main.Properties.PropertyLevel.Application,
        () => new ColorSetBag(Enumerable.Empty<Tuple<IColorSet, bool>>()));

      Instance = new ColorSetManager();
    }

    private ColorSetManager()
      : base(
          (level, list) => new ColorSetManagerEntryValue(level, list, false),
          NamedColors.Instance
          )
    {
      _builtinKnownColors = NamedColors.Instance;

      _builtinDarkPlotColors = new ColorSet("PlotColorsDark", GetPlotColorsDark_Version0());
      _allLists.Add(_builtinDarkPlotColors.Name, new ColorSetManagerEntryValue(_builtinDarkPlotColors, Main.ItemDefinitionLevel.Builtin, true));

      Current.PropertyService.UserSettings.TryGetValue(PropertyKeyUserDefinedColorSets, out var userColorSets);
      if (null != userColorSets)
      {
        foreach (var userColorSet in userColorSets.ColorSets)
        {
          InternalTryRegisterList(userColorSet.Item1, ItemDefinitionLevel.UserDefined, out var dummy, false);
          if (userColorSet.Item2) // .IsPlotColorSet
            _allLists[dummy.Name] = new ColorSetManagerEntryValue(dummy, _allLists[dummy.Name].Level, true);
        }
      }
    }

    #region Buildin

    private static NamedColor[] GetPlotColorsDark_Version0() // Version 2012-09-10
    {
      return new NamedColor[]{
      NamedColors.Black,
      NamedColors.Red,
      NamedColors.Green,
      NamedColors.Blue,
      NamedColors.Magenta,
      NamedColors.Goldenrod,
      NamedColors.Coral
      };
    }

    #endregion Buildin

    /// <summary>
    /// Gets the (single) instance of this class.
    /// </summary>
    public static ColorSetManager Instance
    {
      get
      {
        return _instance;
      }
      set
      {
        if (null == value)
          throw new ArgumentNullException(nameof(value));

        if (null != _instance)
        {
          Current.IProjectService.ProjectClosed -= _instance.EhProjectClosed;
        }

        _instance = value;

        if (null != _instance)
        {
          Current.IProjectService.ProjectClosed += _instance.EhProjectClosed;
        }
      }
    }

    public override IColorSet CreateNewList(string name, IEnumerable<NamedColor> symbols)
    {
      return new ColorSet(name, symbols);
    }

    public override IColorSet GetParentList(NamedColor item)
    {
      return item.ParentColorSet;
    }

    /// <summary>
    /// Gets the builtin set of known colors.
    /// </summary>
    public IColorSet BuiltinKnownColors
    {
      get
      {
        return _builtinKnownColors;
      }
    }

    /// <summary>
    /// Gets the builtin set of dark plot colors.
    /// </summary>
    public IColorSet BuiltinDarkPlotColors
    {
      get
      {
        return _builtinDarkPlotColors;
      }
    }

    public bool IsPlotColorSet(IColorSet colorSet)
    {
      if (null == colorSet)
        return false;

      if (_allLists.TryGetValue(colorSet.Name, out var value))
        return value.IsPlotColorSet;

      return false;
    }

    public void DeclareAsPlotColorList(IColorSet colorSet)
    {
      if (null == colorSet)
        throw new ArgumentNullException(nameof(colorSet));
      if (!_allLists.ContainsKey(colorSet.Name))
        throw new ArgumentException("Provided ColorSet is not registered in ColorSetManager", nameof(colorSet));

      var value = _allLists[colorSet.Name];
      if (!value.IsPlotColorSet)
      {
        _allLists[colorSet.Name] = new ColorSetManagerEntryValue(colorSet, value.Level, true);
        OnListAdded(colorSet, value.Level);
      }
    }

    #region Deserialization of colors

    public bool TryFindColorSetContaining(AxoColor color, out IColorSet value)
    {

      foreach (Main.ItemDefinitionLevel level in Enum.GetValues(typeof(Main.ItemDefinitionLevel)))
      {
        foreach (var entry in _allLists)
        {
          if (entry.Value.Level != level)
            continue;

          if (entry.Value.List.TryGetValue(color, out var namedColor))
          {
            value = entry.Value.List;
            return true;
          }
        }
      }

      value = null;
      return false;
    }

    public bool TryFindColorSetContaining(AxoColor colorValue, string colorName, out IColorSet value)
    {

      foreach (Main.ItemDefinitionLevel level in Enum.GetValues(typeof(Main.ItemDefinitionLevel)))
      {
        foreach (var entry in _allLists)
        {
          if (entry.Value.Level != level)
            continue;

          if (entry.Value.List.TryGetValue(colorValue, colorName, out var namedColor))
          {
            value = entry.Value.List;
            return true;
          }
        }
      }

      value = null;
      return false;
    }

    public NamedColor GetDeserializedColorWithNoSet(AxoColor color, string name)
    {
      // test if it is a standard color
      if (_builtinKnownColors.TryGetValue(name, out var foundColor) && color.Equals(foundColor.Color)) // if the color is known by this name, and the color value matches
        return foundColor; // then return this found color

      if (_builtinKnownColors.TryGetValue(color, out foundColor)) // if only the color value matches, then return the found color, even if it has another name than the deserialized color
        return foundColor;

      // Note that name for a deserialized color can be null or empty. If this is the case, use the constructor without name
      return string.IsNullOrEmpty(name) ? new NamedColor(color) : new NamedColor(color, name); // if it is not a known color, then return the color without a color set as parent
    }

    public NamedColor GetDeserializedColorFromBuiltinSet(AxoColor color, string colorName, IColorSet builtinColorSet)
    {
      return new NamedColor(color, colorName, builtinColorSet);
    }

    public NamedColor GetDeserializedColorFromLevelAndSetName(Altaxo.Serialization.Xml.IXmlDeserializationInfo deserializationInfo, AxoColor colorValue, string colorName, string colorSetName)
    {

      // first have a look in the rename dictionary - maybe our color set has been renamed during deserialization
      var renameDictionary = deserializationInfo?.GetPropertyOrDefault<Dictionary<string, string>>(DeserializationRenameDictionaryKey);
      if (null != renameDictionary && renameDictionary.ContainsKey(colorSetName))
        colorSetName = renameDictionary[colorSetName];

      if (_allLists.TryGetValue(colorSetName, out var foundSet)) // if a set with the give name and level was found
      {
        if (foundSet.List.TryGetValue(colorName, out var foundColor) && colorValue.Equals(foundColor.Color)) // if the color is known by this name, and the color value matches
          return foundColor;                                                                  // then return this found color
        if (foundSet.List.TryGetValue(colorValue, out foundColor)) // if only the color value matches,
          return foundColor;                            // then return the found color, even if it has another name than the deserialized color

        // set was found, but color is not therein -> return a color without set (or use the first set where the color could be found
        TryFindColorSetContaining(colorValue, colorName, out var cset);
        var result = new NamedColor(colorValue, colorName, cset);
        return result;
      }
      else // the color set with the given name was not found by name
      {
        TryFindColorSetContaining(colorValue, colorName, out var cset);
        var result = new NamedColor(colorValue, colorName, cset);
        return result;
      }
    }

    #endregion Deserialization of colors

    #region User defined color sets

    protected override void OnUserDefinedListAddedChangedRemoved(IColorSet list)
    {
      var colorSetBag = new ColorSetBag(_allLists.Values.Where(entry => entry.Level == ItemDefinitionLevel.UserDefined).Select(entry => new Tuple<IColorSet, bool>(entry.List, entry.IsPlotColorSet)));
      Current.PropertyService.UserSettings.SetValue(PropertyKeyUserDefinedColorSets, colorSetBag);
    }

    #endregion User defined color sets
  }
}
