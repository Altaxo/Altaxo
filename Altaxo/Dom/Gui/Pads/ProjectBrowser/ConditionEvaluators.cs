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
  /// Evaluates conditions based on the number and type of selected project browser list items.
  /// Two attributes are used: <c>selcount</c> and <c>itemtype</c>. The value of <c>selcount</c> starts with one of
  /// the comparison characters (<c>&lt;</c>, <c>=</c>, <c>&gt;</c>) followed by an integer, for example <c>=1</c>.
  /// The value of <c>itemtype</c>, if set, is the class name of the items that should be selected.
  /// </summary>
  public class ListItemSelectionEvaluator : IConditionEvaluator
  {
    /// <inheritdoc/>
    public bool IsValid(object parameter, Condition condition)
    {
      bool result;

      string expectedSelCount = condition.Properties["selcount"].ToLowerInvariant();
      result = EvaluateSelCount(parameter, expectedSelCount);
      if (false == result)
        return result;

      string expectedItemType = condition.Properties["selitems"];
      result = EvaluateItemType(parameter, expectedItemType);
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
