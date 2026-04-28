// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Altaxo.AddInItems;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;

namespace Altaxo.Gui.AddInItems
{
  /// <summary>
  /// A tool bar button that opens a drop down menu.
  /// </summary>
  internal sealed class ToolBarDropDownButton : DropDownButton, IStatusUpdate
  {
    private readonly Codon codon;
    private readonly object caller;
    private readonly IEnumerable<ICondition> conditions;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolBarDropDownButton"/> class.
    /// </summary>
    /// <param name="codon">The add-in codon.</param>
    /// <param name="caller">The caller object.</param>
    /// <param name="subMenu">The submenu items.</param>
    /// <param name="conditions">The conditions that control visibility.</param>
    public ToolBarDropDownButton(Codon codon, object caller, IList subMenu, IEnumerable<ICondition> conditions)
    {
      ToolTipService.SetShowOnDisabled(this, true);

      this.codon = codon;
      this.caller = caller;
      this.conditions = conditions;

      Content = ToolBarService.CreateToolBarItemContent(codon);
      if (codon.Properties.Contains("name"))
      {
        Name = codon.Properties["name"];
      }
      DropDownMenu = MenuService.CreateContextMenu(subMenu);
      UpdateText();
    }

    /// <summary>
    /// Updates the text displayed by the toolbar drop-down button.
    /// </summary>
    public void UpdateText()
    {
      if (codon.Properties.Contains("tooltip"))
      {
        ToolTip = StringParser.Parse(codon.Properties["tooltip"]);
      }
    }

    /// <summary>
    /// Updates the toolbar button visibility.
    /// </summary>
    public void UpdateStatus()
    {
      if (Altaxo.AddInItems.Condition.GetFailedAction(conditions, caller) == ConditionFailedAction.Exclude)
        Visibility = Visibility.Collapsed;
      else
        Visibility = Visibility.Visible;
    }
  }
}
