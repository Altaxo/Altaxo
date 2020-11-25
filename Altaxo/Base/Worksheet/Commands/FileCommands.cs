#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.IO;
using Altaxo.Data;

namespace Altaxo.Worksheet
{
  /// <summary>
  /// Routines for saving the worksheet layout and corresponding table.
  /// </summary>
  public static class FileCommands
  {
    /// <summary>
    /// Saves the worksheet (data table and the corresponding layout, including scripts) into a xml file.
    /// </summary>
    /// <param name="worksheet">The worksheet to save.</param>
    /// <param name="myStream">The stream where the xml data are to save into.</param>
    /// <param name="saveAsTemplate">If true, the data are not saved, but only the layout of the worksheet (columns, property columns, scripts). If false, everything including the data is saved.</param>
    public static void Save(this WorksheetLayout worksheet, System.IO.Stream myStream, bool saveAsTemplate)
    {
      var info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
      if (saveAsTemplate)
      {
        info.SetProperty(DataTable.SerializationInfoProperty_SaveAsTemplate, "true");
      }
      info.BeginWriting(myStream);

      // TODO there is an issue with TableLayout that prevents a nice deserialization
      // this is because TableLayout stores the name of its table during serialization
      // onto deserialization this works well if the entire document is restored, but
      // doesn't work if only a table and its layout is to be restored. In this case, the layout
      // references the already present table with the same name in the document instead of the table
      // deserialized. Also, the GUID isn't unique if the template is deserialized more than one time.

      var tableAndLayout =
        new Altaxo.Worksheet.TablePlusLayout(worksheet.DataTable, worksheet);
      info.AddValue("TablePlusLayout", tableAndLayout);
      info.EndWriting();
    }

    /// <summary>
    /// Shows the SaveAs dialog and then saves the worksheet (data table and the corresponding layout, including scripts) into a xml file.
    /// </summary>
    /// <param name="worksheet">The worksheet to save.</param>
    /// <param name="saveAsTemplate">If true, the data are not saved, but only the layout of the worksheet (columns, property columns, scripts). If false, everything including the data is saved.</param>
    public static void ShowSaveAsDialog(this WorksheetLayout worksheet, bool saveAsTemplate)
    {
      var options = new Altaxo.Gui.SaveFileOptions();
      options.AddFilter("*.axowks", "Altaxo worksheet files (*.axowks)");
      options.AddFilter("*.*", "All files (*.*)");
      options.FilterIndex = 0;
      options.RestoreDirectory = true;

      if (Current.Gui.ShowSaveFileDialog(options))
      {
        using (Stream myStream = new System.IO.FileStream(options.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
        {
          Save(worksheet, myStream, saveAsTemplate);
          myStream.Close();
        }
      }
    }
  }
}
