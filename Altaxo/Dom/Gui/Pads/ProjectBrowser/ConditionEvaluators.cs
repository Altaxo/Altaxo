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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.AddInItems;

namespace Altaxo.Gui.Pads.ProjectBrowser
{
  /// <summary>
  /// This helps in conditions where the number of selected data columns and the type of the item cares.
  /// Two attributes are used: selcount and itemtype. The value of selcount is one of the comparer chars: (&lt; = &gt;)
  /// and an integer number, for instance: =1. The value of itemtype, if set, is the class name of the items, that should
  /// be selected.
  /// </summary>
  public class ListItemSelectionEvaluator : IConditionEvaluator
  {
    /// <summary>
    /// Evaluates if the chosen condition is true.
    /// </summary>
    /// <param name="caller"></param>
    /// <param name="condition"></param>
    /// <returns>True if the condition is valid.</returns>
    public bool IsValid(object caller, Condition condition)
    {
      bool result;

      string expectedSelCount = condition.Properties["selcount"].ToLowerInvariant();
      result = EvaluateSelCount(caller, expectedSelCount);
      if (false == result)
        return result;

      string expectedItemType = condition.Properties["selitems"];
      result = EvaluateItemType(caller, expectedItemType);
      if (false == result)
        return result;

      return true;
    }

    private bool EvaluateSelCount(object caller, string cond)
    {
      if (caller is ProjectBrowseController projectBrowseController)
      {
        // the first character of cond have to be <, = or >
        // the rest of cond should be an integer
        if (string.IsNullOrEmpty(cond))
          return true; // no restriction concerning the number of items
        if (cond.Length <= 1)
          return false;

        if (!int.TryParse(cond.Substring(1), out var condNumber))
          return false;

        int currItems = projectBrowseController.GetNumberOfSelectedListItems();

        switch (cond[0])
        {
          case '<':
            return currItems < condNumber;

          case '=':
            return currItems == condNumber;

          case '>':
            return currItems > condNumber;

          default:
            return false;
        }
      }
      else
      {
        return false;
      }
    }

    private bool EvaluateItemType(object caller, string expectedItemType)
    {
      // the first character of cond have to be <, = or >
      // the rest of cond should be an integer
      if (string.IsNullOrEmpty(expectedItemType))
        return true; // no restriction concerning the number of items

      IEnumerable<object> selItems = ((ProjectBrowseController)caller).GetSelectedListItems();
      if (expectedItemType != "Folder") // if folder, then do no expansion of selected items!
        selItems = Current.Project.Folders.GetExpandedProjectItemSet(selItems);

      var type = Type.GetType(expectedItemType, false, false);

      if (type is not null)
      {
        foreach (object item in selItems)
          if (!(type.IsAssignableFrom(item.GetType())))
            return false;
      }
      else// if type is null, we compare simply by name, ignoring the assembly
      {
        foreach (object item in selItems)
          if (expectedItemType != item.GetType().ToString())
            return false;
      }

      return true;
    }
  }
}
