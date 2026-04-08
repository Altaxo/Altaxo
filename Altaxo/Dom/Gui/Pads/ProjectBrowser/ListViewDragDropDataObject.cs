#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2014 Dr. Dirk Lellinger
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

namespace Altaxo.Gui.Pads.ProjectBrowser
{
  /// <summary>
  /// Provides drag-and-drop data for project browser list view items.
  /// </summary>
  public class ListViewDragDropDataObject : Altaxo.Serialization.Clipboard.IDataObject
  {
    /// <summary>
    /// Clipboard format for the current application instance identifier.
    /// </summary>
    public const string Format_ApplicationInstanceGuid = "Altaxo.Current.ApplicationInstanceGuid";
    /// <summary>
    /// Clipboard format for the project folder name.
    /// </summary>
    public const string Format_ProjectFolder = "Altaxo.Gui.Pads.ProjectBrowser.FolderName";
    /// <summary>
    /// Clipboard format for serialized project items.
    /// </summary>
    public const string Format_ItemList = "Altaxo.Gui.Pads.ProjectBrowser.ItemList";
    /// <summary>
    /// Clipboard format for serialized project item references.
    /// </summary>
    public const string Format_ItemReferenceList = "Altaxo.Gui.Pads.ProjectBrowser.ItemReferenceList";

    private List<string> _availableFormats;

    /// <summary>
    /// Gets or sets the project folder name associated with the drag-and-drop operation.
    /// </summary>
    public string FolderName { get; set; }

    /// <summary>
    /// Gets or sets the item list. This list either contains project items (for foreign applications) or DocNodeProxies (for our own application instance).
    /// </summary>
    /// <value>
    /// The item list.
    /// </value>
    public List<Altaxo.Main.IProjectItem> ItemList { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the item list was rendered.
    /// </summary>
    /// <value>
    /// <c>true</c> if item list was rendered; otherwise, <c>false</c>.
    /// </value>
    public bool ItemListWasRendered { get; protected set; }

    /// <inheritdoc/>
    public object GetData(string format, bool autoConvert)
    {
      object result = null;
      switch (format)
      {
        case Format_ProjectFolder:
          result = FolderName;
          break;

        case Format_ApplicationInstanceGuid:
          result = Current.ApplicationInstanceGuid.ToString();
          break;

        case Format_ItemList:
          {
            var items = new Altaxo.Main.Commands.ProjectItemCommands.ProjectItemClipboardList(ItemList, FolderName);
            var stb = Altaxo.Serialization.Clipboard.ClipboardSerialization.SerializeToStringBuilder(items);
            result = stb.ToString();
            ItemListWasRendered = true;
          }
          break;

        case Format_ItemReferenceList:
          {
            var itemReferenceList = new List<Altaxo.Main.DocNodeProxy>(ItemList.Select(x => new Altaxo.Main.DocNodeProxy(x)));
            var items = new Altaxo.Main.Commands.ProjectItemCommands.ProjectItemReferenceClipboardList(itemReferenceList, FolderName);
            var stb = Altaxo.Serialization.Clipboard.ClipboardSerialization.SerializeToStringBuilder(items);
            result = stb.ToString();
          }
          break;

        default:
          result = null;
          break;
      }

      return result;
    }

    /// <inheritdoc/>
    public object GetData(string format)
    {
      return GetData(format, true);
    }

    /// <inheritdoc/>
    public bool GetDataPresent(string format, bool autoConvert)
    {
      if (_availableFormats is null)
        SetFormats();

      return _availableFormats.Contains(format);
    }

    /// <inheritdoc/>
    public bool GetDataPresent(string format)
    {
      return GetDataPresent(format, true);
    }

    /// <inheritdoc/>
    public string[] GetFormats(bool autoConvert)
    {
      if (_availableFormats is null)
        SetFormats();

      return _availableFormats.ToArray();
    }

    /// <inheritdoc/>
    public string[] GetFormats()
    {
      return GetFormats(true);
    }

    /// <summary>
    /// Builds the list of clipboard formats currently available from this data object.
    /// </summary>
    public void SetFormats()
    {
      _availableFormats = new List<string>();

      if (FolderName is not null)
        _availableFormats.Add(Format_ProjectFolder);

      _availableFormats.Add(Format_ApplicationInstanceGuid);

      _availableFormats.Add(Format_ItemList);
      _availableFormats.Add(Format_ItemReferenceList);
    }

    #region Not implemented interface functions

    /// <inheritdoc/>
    public object GetData(Type format)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public bool GetDataPresent(Type format)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetData(string format, object data, bool autoConvert)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetData(Type format, object data)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetData(string format, object data)
    {
      throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public void SetData(object data)
    {
      throw new NotImplementedException();
    }

    #endregion Not implemented interface functions
  }
}
