#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Altaxo.AddInItems;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;

namespace Altaxo.Gui.AddInItems
{
  /// <summary>
  /// A tool bar button that opens a drop down menu.
  /// </summary>
  public class ToolBarComboBox : ComboBox, IStatusUpdate
  {
    private readonly Codon codon;
    private readonly object caller;
    private readonly IEnumerable<ICondition> conditions;

    public ToolBarComboBox(Codon codon, object caller, IEnumerable<ICondition> conditions)
    {
      ToolTipService.SetShowOnDisabled(this, true);

      this.codon = codon;
      this.caller = caller;
      this.conditions = conditions;

      if (codon.Properties.Contains("name"))
      {
        Name = codon.Properties["name"];
      }
      InitializeContent();
      UpdateText();
    }

    public void UpdateText()
    {
      if (codon.Properties.Contains("tooltip"))
      {
        ToolTip = StringParser.Parse(codon.Properties["tooltip"]);
      }
    }

    public void UpdateStatus()
    {
      if (Altaxo.AddInItems.Condition.GetFailedAction(conditions, caller) == ConditionFailedAction.Exclude)
        Visibility = Visibility.Collapsed;
      else
        Visibility = Visibility.Visible;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (e.Key == System.Windows.Input.Key.Enter)
      {
        if (CanExecute(null))
          Execute(null);
        e.Handled = true;
      }

      base.OnKeyDown(e);
    }

    public virtual void InitializeContent()
    {
    }

    public virtual bool CanExecute(object parameter)
    {
      return true;
    }

    public virtual void Execute(object parameter)
    {
    }
  }
}
