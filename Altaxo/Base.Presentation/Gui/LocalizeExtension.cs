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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using Altaxo.Gui.AddInItems;
using Altaxo.Main.Services;

namespace Altaxo.Gui
{
  /// <summary>
  /// Markup extension that retrieves localized resource strings.
  /// </summary>
  /// <example>
  /// Set the title of the main window using the string resource 'MainWindow.DialogName':
  /// <code>
  /// Title = "{axog:Localize MainWindow.DialogName}"
  /// </code>
  /// If the string used accessors to enable easy access by keyboard, use the following syntax:
  /// </example>
  ///  <code>
  /// Header = "{axog:Localize Menu.FileMenu.File UsesAccessors=True}"
  /// </code>
  [MarkupExtensionReturnType(typeof(string))]
  public sealed class LocalizeExtension : LanguageDependentExtension
  {
    public LocalizeExtension(string key)
    {
      this.key = key;
      UsesAccessors = true;
      UpdateOnLanguageChange = true;
    }

    public LocalizeExtension()
    {
      UsesAccessors = true;
      UpdateOnLanguageChange = true;
    }

    private string key;

    public string Key
    {
      get { return key; }
      set { key = value; }
    }

    /// <summary>
    /// Set whether the text uses accessors.
    /// If set to true (default), accessors will be converted to WPF syntax.
    /// </summary>
    public bool UsesAccessors { get; set; }

    public override string Value
    {
      get
      {
        try
        {
          string result = Altaxo.Current.ResourceService.GetString(key);
          if (UsesAccessors)
            result = MenuService.ConvertLabel(result);
          return result;
        }
        catch (ResourceNotFoundException)
        {
          return "{Localize:" + key + "}";
        }
      }
    }
  }
}
