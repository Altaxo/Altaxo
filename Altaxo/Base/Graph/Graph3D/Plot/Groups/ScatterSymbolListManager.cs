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
using Altaxo.Graph.Graph3D.Plot.Styles;
using Altaxo.Main;

namespace Altaxo.Graph.Graph3D.Plot.Groups
{
  public class ScatterSymbolListManager : StyleListManagerBaseForClasses<ScatterSymbolList, IScatterSymbol, StyleListManagerBaseEntryValue<ScatterSymbolList, IScatterSymbol>>
  {
    public static readonly Main.Properties.PropertyKey<ScatterSymbolListBag> PropertyKeyUserDefinedScatterSymbolLists;

    private static ScatterSymbolListManager _instance;

    static ScatterSymbolListManager()
    {
      PropertyKeyUserDefinedScatterSymbolLists =
        new Main.Properties.PropertyKey<ScatterSymbolListBag>(
        "28A88605-7595-4107-BA5A-E732C7D3819C",
        "Graph3D\\UserDefinedScatterSymbolLists",
        Main.Properties.PropertyLevel.Application,
        () => new ScatterSymbolListBag(Enumerable.Empty<ScatterSymbolList>()));

      Instance = new ScatterSymbolListManager();
    }

    protected ScatterSymbolListManager()
      : base(
          (list, level) => new StyleListManagerBaseEntryValue<ScatterSymbolList, IScatterSymbol>(list, level),
          new ScatterSymbolList("BuiltinDefault", new IScatterSymbol[] {
          new Styles.ScatterSymbols.Cube(),
          new Styles.ScatterSymbols.Sphere(),
          new Styles.ScatterSymbols.TetrahedronUp(),
          new Styles.ScatterSymbols.TetrahedronDown()
      })
          )

    {
      Current.PropertyService.UserSettings.TryGetValue(PropertyKeyUserDefinedScatterSymbolLists, out var userStyleLists);
      if (userStyleLists is not null)
      {
        foreach (var list in userStyleLists.StyleLists)
        {
          InternalTryRegisterList(list, ItemDefinitionLevel.UserDefined, out var dummy, false);
        }
      }

      RebuildListEntryToListDictionary();
    }

    public static ScatterSymbolListManager Instance
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

    public override ScatterSymbolList CreateNewList(string name, IEnumerable<IScatterSymbol> symbols)
    {
      return new ScatterSymbolList(name, symbols);
    }

    #region User defined lists

    protected override void OnUserDefinedListAddedChangedRemoved([AllowNull] ScatterSymbolList list)
    {
      var listBag = new ScatterSymbolListBag(_allLists.Values.Where(entry => entry.Level == ItemDefinitionLevel.UserDefined).Select(entry => entry.List));
      Current.PropertyService.UserSettings.SetValue(PropertyKeyUserDefinedScatterSymbolLists, listBag);
    }

    #endregion User defined lists
  }
}
