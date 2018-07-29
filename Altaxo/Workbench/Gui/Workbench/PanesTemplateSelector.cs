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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Selects a data template for either a tool window or a document window.
  /// </summary>
  /// <seealso cref="System.Windows.Controls.DataTemplateSelector" />
  public class PanesTemplateSelector : DataTemplateSelector
  {
    /// <summary>
    /// Gets or sets the data template for a tool window.
    /// </summary>
    /// <value>
    /// The data template for the tool window.
    /// </value>
    public DataTemplate ToolTemplate
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the data template for a document window.
    /// </summary>
    /// <value>
    /// The document template.
    /// </value>
    public DataTemplate DocumentTemplate
    {
      get;
      set;
    }

    /// <summary>
    /// Selects the data template based on the type of the (modelview) <paramref name="item"/>.
    /// If the provided item is a <see cref="PadDescriptor"/>, the <see cref="ToolTemplate"/> is selected.
    /// If the provided item is a <see cref="IViewContent"/>, the <see cref="DocumentTemplate"/> is selected.
    /// Otherwise the default template selection is called.
    /// </summary>
    /// <param name="item">The data object for which to select the template.</param>
    /// <param name="container">The data-bound object.</param>
    /// <returns>
    /// Returns a <see cref="T:System.Windows.DataTemplate" /> or null. The default value is null.
    /// </returns>
    public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
    {
      if (item is IPadContent) // Pad has precedence over IViewContent
        return ToolTemplate;
      else if (item is IViewContent)
        return DocumentTemplate;
      else
        return base.SelectTemplate(item, container);
    }
  }
}
