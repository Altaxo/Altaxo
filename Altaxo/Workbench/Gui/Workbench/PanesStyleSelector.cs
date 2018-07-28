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
  /// Selects a style for either a tool window or a document window.
  /// </summary>
  /// <seealso cref="System.Windows.Controls.StyleSelector" />
  public class PanesStyleSelector : StyleSelector
  {
    /// <summary>
    /// Gets or sets style for a tool window.
    /// </summary>
    /// <value>
    /// The style for a tool window.
    /// </value>
    public Style ToolStyle
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the style for a document window.
    /// </summary>
    /// <value>
    /// The style for a document window.
    /// </value>
    public Style DocumentStyle
    {
      get;
      set;
    }

    /// <summary>
    /// Selects the stylebased on the type of the (modelview) <paramref name="item"/>.
    /// If the provided item is a <see cref="PadDescriptor"/>, the <see cref="ToolStyle"/> is selected.
    /// If the provided item is a <see cref="IViewContent"/>, the <see cref="DocumentStyle"/> is selected.
    /// Otherwise the default template selection is called.
    /// </summary>
    /// <param name="item">The content.</param>
    /// <param name="container">The element to which the style will be applied.</param>
    /// <returns>
    /// Returns an application-specific style to apply; otherwise, null.
    /// </returns>
    public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
    {
      if (item is IPadContent) // IPadContent has precedence over IViewContent, because pads can be docked in the pad area as well as in the document area
        return ToolStyle;
      else if (item is IViewContent)
        return DocumentStyle;
      else
        return base.SelectStyle(item, container);
    }
  }
}
