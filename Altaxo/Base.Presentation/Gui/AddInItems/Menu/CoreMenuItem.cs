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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Altaxo.AddInItems;
using Altaxo.Main.Services;

namespace Altaxo.Gui.AddInItems
{
  /// <summary>
  /// A menu item representing an AddIn-Tree element.
  /// </summary>
  internal class CoreMenuItem : MenuItem, IStatusUpdate
  {
    protected readonly Codon _codon;
    protected readonly object _caller;
    protected readonly IReadOnlyCollection<ICondition> _conditions;

    /// <summary>Cached value of _codon's property 'usedatacontext'. If true, the data context of this menu item is used as the parameter of <see cref="UpdateStatus(object)"/> instead of <see cref="_caller"/>.</summary>
    internal bool _useDataContext;

    /// <summary>
    /// If true, UpdateStatus() sets the enabled flag.
    /// Used for type=Menu, but not for type=MenuItem - for menu items, Enabled is controlled by the WPF ICommand.
    /// </summary>
    internal bool _setEnabled;

    public CoreMenuItem(Codon codon, object caller, IReadOnlyCollection<ICondition> conditions)
    {
      _codon = codon;
      _caller = caller;
      _conditions = conditions;

      if (codon.Properties.Contains("icon"))
      {
        try
        {
          var image = new Image
          {
            Source = PresentationResourceService.GetBitmapSource(codon.Properties["icon"]),
            Height = 16
          };
          Icon = image;
        }
        catch (ResourceNotFoundException) { }
      }
      UpdateText();
    }

    public void UpdateText()
    {
      if (_codon != null)
      {
        Header = MenuService.ConvertLabel(StringParser.Parse(_codon.Properties["label"]));
      }
    }

    public virtual void UpdateStatus()
    {
      ConditionFailedAction result = Altaxo.AddInItems.Condition.GetFailedAction(_conditions, _useDataContext ? (DataContext ?? _caller) : _caller);
      if (result == ConditionFailedAction.Exclude)
        Visibility = Visibility.Collapsed;
      else
        Visibility = Visibility.Visible;
      if (_setEnabled)
        IsEnabled = result == ConditionFailedAction.Nothing;
    }
  }
}
