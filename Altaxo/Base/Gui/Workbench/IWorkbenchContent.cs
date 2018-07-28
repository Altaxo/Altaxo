using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Common base interface for both <see cref="IPadContent"/> and <see cref="IViewContent"/>.
  /// </summary>
  public interface IWorkbenchContent
    :
    IMVCController,
    INotifyPropertyChanged, // in order to get noted when IsSelected or other properties changed
    IServiceProvider
  {
    /// <summary>
    /// Gets the control which has focus initially.
    /// </summary>
    object InitiallyFocusedControl
    {
      get;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this pad is visible at all. This means, that if this property is false,
    /// the pad will not be shown, even the title is not shown anywhere.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is visible; otherwise, <c>false</c>.
    /// </value>
    bool IsVisible { get; set; }

    bool IsSelected { get; set; }

    bool IsActive { get; set; }

    /// <summary>
    /// Returns the title of the pad (<see cref="IPadContent"/>), or the text on the tab page of the document window (<see cref="IViewContent"/>).
    /// </summary>
    string Title { get; }
  }
}
