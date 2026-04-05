#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2016 Dr. Dirk Lellinger
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

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Altaxo.Drawing;
using Altaxo.Drawing.D3D;
using Altaxo.Graph;
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Main;

namespace Altaxo.Drawing.DashPatternManagement
{
  /// <summary>
  /// Manages registered dash pattern lists for the application.
  /// </summary>
  public class DashPatternListManager : StyleListManagerBaseForClasses<DashPatternList, IDashPattern, StyleListManagerBaseEntryValue<DashPatternList, IDashPattern>>
  {
    /// <summary>
    /// Property key used to persist user-defined dash pattern lists.
    /// </summary>
    public static readonly Main.Properties.PropertyKey<DashPatternListBag> PropertyKeyUserDefinedDashPatternLists;

    private static DashPatternListManager _instance;

    static DashPatternListManager()
    {
      PropertyKeyUserDefinedDashPatternLists =
        new Main.Properties.PropertyKey<DashPatternListBag>(
        "6C8F87E2-F80A-458E-A5C5-DFF92EBDBA90",
        "Graph3D\\UserDefinedDashPatternLists",
        Main.Properties.PropertyLevel.Application,
        () => new DashPatternListBag(Enumerable.Empty<DashPatternList>()));

      Instance = new DashPatternListManager();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DashPatternListManager"/> class.
    /// </summary>
    protected DashPatternListManager()
      : base(
          (list, level) => new StyleListManagerBaseEntryValue<DashPatternList, IDashPattern>(list, level),
          new DashPatternList("BuiltinDefault", new IDashPattern[] {
          new Drawing.DashPatterns.Solid(),
          new Drawing.DashPatterns.Dash(),
          new Drawing.DashPatterns.Dot(),
          new Drawing.DashPatterns.DashDot(),
          new Drawing.DashPatterns.DashDotDot(),
      })
          )

    {
      Current.PropertyService.UserSettings.TryGetValue(PropertyKeyUserDefinedDashPatternLists, out var userStyleLists);
      if (userStyleLists is not null)
      {
        foreach (var list in userStyleLists.StyleLists)
        {
          InternalTryRegisterList(list, ItemDefinitionLevel.UserDefined, out var dummy, false);
        }
      }

      RebuildListEntryToListDictionary();
    }

    /// <summary>
    /// Gets the buildin default solid dash pattern belonging to the BuildinDefault list.
    /// </summary>
    /// <value>
    /// The buildin default solid dash pattern belonging to the BuildinDefault list.
    /// </value>
    public IDashPattern BuiltinDefaultSolid { get { return BuiltinDefault[0]; } }

    /// <summary>
    /// Gets the built-in default dash pattern belonging to the BuildinDefault list.
    /// </summary>
    public IDashPattern BuiltinDefaultDash { get { return BuiltinDefault[1]; } }

    /// <summary>
    /// Gets the built-in default dot dash pattern belonging to the BuildinDefault list.
    /// </summary>
    public IDashPattern BuiltinDefaultDot { get { return BuiltinDefault[2]; } }

    /// <summary>
    /// Gets the built-in default dash-dot pattern belonging to the BuildinDefault list.
    /// </summary>
    public IDashPattern BuiltinDefaultDashDot { get { return BuiltinDefault[3]; } }

    /// <summary>
    /// Gets the built-in default dash-dot-dot pattern belonging to the BuildinDefault list.
    /// </summary>
    public IDashPattern BuiltinDefaultDashDotDot { get { return BuiltinDefault[4]; } }

    /// <summary>
    /// Gets or sets the singleton dash pattern list manager instance.
    /// </summary>
    public static DashPatternListManager Instance
    {
      get
      {
        return _instance;
      }
      [MemberNotNull(nameof(_instance))]
      set
      {
        if (value is null)
          throw new ArgumentNullException(nameof(Instance));

        if (_instance is not null)
          Current.IProjectService.ProjectClosed -= _instance.EhProjectClosed;

        _instance = value;

        Current.IProjectService.ProjectClosed += _instance.EhProjectClosed;
      }
    }

    /// <inheritdoc/>
    public override DashPatternList CreateNewList(string name, IEnumerable<IDashPattern> symbols)
    {
      return new DashPatternList(name, symbols);
    }

    /// <inheritdoc/>
    protected override void OnUserDefinedListAddedChangedRemoved(DashPatternList? list)
    {
      var listBag = new DashPatternListBag(_allLists.Values.Where(entry => entry.Level == ItemDefinitionLevel.UserDefined).Select(entry => entry.List));
      Current.PropertyService.UserSettings.SetValue(PropertyKeyUserDefinedDashPatternLists, listBag);
    }
  }
}
