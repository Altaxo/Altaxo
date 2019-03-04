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
    /// 
    /// Gets or sets the visibility of a pad or document.
    /// If false for a pad, the pad is not visible. If false for a document, the document tab header is not visible (but the document itself maybe visible).
    /// If true for a pad, the pad may be visible or is collapsed. If true for a document, the document tab header is visible (if it fits in the bar),
    /// and the document is visible, if it is selected, too.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance is visible; otherwise, <c>false</c>.
    /// </value>
    bool IsVisible { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this pad or document is selected.
    /// If this is a document, it is selected if the tab of the document is selected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
    /// </value>
    bool IsSelected { get; set; }

    bool IsActive { get; set; }

    /// <summary>
    /// Returns the title of the pad (<see cref="IPadContent"/>), or the text on the tab page of the document window (<see cref="IViewContent"/>).
    /// </summary>
    string Title { get; }
  }
}
