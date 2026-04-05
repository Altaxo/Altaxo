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

#nullable enable

namespace Altaxo.Gui
{
  /// <summary>
  /// This interface is meant for controllers that want to handle clipboard events.
  /// WPF AddIns should handle the routed commands 'Copy', 'Cut', 'Paste', 'Delete' and 'SelectAll' instead.
  /// </summary>
  public interface IClipboardHandler
  {
    /// <summary>
    /// Gets a value indicating whether cutting is enabled.
    /// </summary>
    bool EnableCut
    {
      get;
    }

    /// <summary>
    /// Gets a value indicating whether copying is enabled.
    /// </summary>
    bool EnableCopy
    {
      get;
    }

    /// <summary>
    /// Gets a value indicating whether pasting is enabled.
    /// </summary>
    bool EnablePaste
    {
      get;
    }

    /// <summary>
    /// Gets a value indicating whether deleting is enabled.
    /// </summary>
    bool EnableDelete
    {
      get;
    }

    /// <summary>
    /// Gets a value indicating whether selecting all is enabled.
    /// </summary>
    bool EnableSelectAll
    {
      get;
    }

    /// <summary>
    /// Cuts the current selection.
    /// </summary>
    void Cut();

    /// <summary>
    /// Copies the current selection.
    /// </summary>
    void Copy();

    /// <summary>
    /// Pastes clipboard content.
    /// </summary>
    void Paste();

    /// <summary>
    /// Deletes the current selection.
    /// </summary>
    void Delete();

    /// <summary>
    /// Selects all items.
    /// </summary>
    void SelectAll();
  }
}
