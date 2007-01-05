#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
#endregion

using System;
using System.Windows.Forms;
using Altaxo.Serialization.Ascii;
using Altaxo.Worksheet.GUI;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Summary description for FileCommands.
  /// </summary>
  public class FileCommands
  {
    public static void Save(WorksheetController ctrl, System.IO.Stream myStream, bool saveAsTemplate)
    {
      Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
      if(saveAsTemplate)
      {
        info.SetProperty("Altaxo.Data.DataColumn.SaveAsTemplate","true");
      }
      info.BeginWriting(myStream);

      // TODO there is an issue with TableLayout that prevents a nice deserialization 
      // this is because TableLayout stores the name of its table during serialization
      // onto deserialization this works well if the entire document is restored, but
      // doesn't work if only a table and its layout is to be restored. In this case, the layout
      // references the already present table with the same name in the document instead of the table
      // deserialized. Also, the GUID isn't unique if the template is deserialized more than one time.

      Altaxo.Worksheet.TablePlusLayout tableAndLayout = 
        new Altaxo.Worksheet.TablePlusLayout(ctrl.DataTable, ctrl.WorksheetLayout);
      info.AddValue("TablePlusLayout",tableAndLayout);
      info.EndWriting();    
    }

    public static void SaveAs(WorksheetController ctrl,  bool saveAsTemplate)
    {
      System.IO.Stream myStream ;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
      saveFileDialog1.Filter = "Altaxo worksheet files (*.axowks)|*.axowks|All files (*.*)|*.*"  ;
      saveFileDialog1.FilterIndex = 1 ;
      saveFileDialog1.RestoreDirectory = true ;
 
      if(saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        if((myStream = saveFileDialog1.OpenFile()) != null)
        {
          Save(ctrl,myStream, saveAsTemplate);
          myStream.Close();
        }
      }
    }

    public static void ImportAscii(IWorksheetController ctrl, System.IO.Stream myStream)
    {
      AsciiImporter importer = new AsciiImporter(myStream);
      AsciiImportOptions recognizedOptions = importer.Analyze(30, new AsciiImportOptions());
      importer.ImportAscii(recognizedOptions,ctrl.Doc);
    }

    public static void ImportAsciiToMultipleWorksheets(WorksheetController ctrl, string[] filenames)
    {
      int startrest=0;

      Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order
        
      if(ctrl!=null)
      {
        using(System.IO.Stream myStream = new System.IO.FileStream(filenames[0],System.IO.FileMode.Open,System.IO.FileAccess.Read))
        {
          ImportAscii(ctrl, myStream);
          myStream.Close();
          startrest=1;
        }
      }

      // import also the other files, but this time we create new tables
      for(int i=startrest;i<filenames.Length;i++)
      {
        using(System.IO.Stream myStream = new System.IO.FileStream(filenames[i],System.IO.FileMode.Open,System.IO.FileAccess.Read))
        {
          Altaxo.Worksheet.GUI.IWorksheetController newwkscontroller = Current.ProjectService.CreateNewWorksheet();
          ImportAscii(newwkscontroller,myStream);
          myStream.Close();
        }
      } // for all files

    }

    public static void ImportAsciiToSingleWorksheet(WorksheetController ctrl, string[] filenames)
    {
      Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order
      AsciiImporter.ImportMultipleAscii(filenames, ctrl.Doc);
    }


     public static void ImportAscii(WorksheetController ctrl)
      {
       ImportAscii(ctrl,true);
      }

    public static void ImportAscii(WorksheetController ctrl, bool toMultipleWorksheets)
    {
      using(OpenFileDialog openFileDialog1 = new OpenFileDialog())
      {
        openFileDialog1.Filter = "Text files (*.csv;*.dat;*.txt)|*.csv;*.dat;*.txt|All files (*.*)|*.*" ;
        openFileDialog1.FilterIndex = 1 ;
        openFileDialog1.RestoreDirectory = true ;
        openFileDialog1.Multiselect = true;

        if(openFileDialog1.ShowDialog() == DialogResult.OK && openFileDialog1.FileNames.Length>0)
        {
          if (toMultipleWorksheets)
            ImportAsciiToMultipleWorksheets(ctrl, openFileDialog1.FileNames);
          else
            ImportAsciiToSingleWorksheet(ctrl, openFileDialog1.FileNames);
        }
      }
    }


    public static void ExportAscii(WorksheetController ctrl)
    {
      System.IO.Stream myStream ;
      SaveFileDialog saveFileDialog1 = new SaveFileDialog();

      saveFileDialog1.Filter = "Text files (*.csv;*.dat;*.txt)|*.csv;*.dat;*.txt|All files (*.*)|*.*";
      saveFileDialog1.FilterIndex = 1 ;
      saveFileDialog1.RestoreDirectory = true ;
 
      if(saveFileDialog1.ShowDialog() == DialogResult.OK)
      {
        if((myStream = saveFileDialog1.OpenFile()) != null)
        {
          try
          {
            Altaxo.Serialization.Ascii.AsciiExporter.ExportAscii(myStream, ctrl.DataTable,'\t');
          }
          catch(Exception ex)
          {
            System.Windows.Forms.MessageBox.Show(ctrl.View.TableViewWindow,"There was an error during ascii export, details follow:\n" + ex.ToString());
          }
          finally
          {
            myStream.Close();
          }
        }
      }
    }


    public static void ImportGalacticSPC(WorksheetController ctrl)
    {
      Altaxo.Serialization.Galactic.Import.ShowDialog(ctrl.View.TableViewForm, ctrl.DataTable);
    }


    public static void ExportGalacticSPC(WorksheetController ctrl)
    {
      Altaxo.Serialization.Galactic.ExportGalacticSpcFileDialog dlg =
        new Altaxo.Serialization.Galactic.ExportGalacticSpcFileDialog();

      dlg.Initialize(ctrl.DataTable,ctrl.SelectedDataRows,ctrl.SelectedDataColumns);

      dlg.ShowDialog(ctrl.View.TableViewWindow);
    }
 

    
    public delegate double ColorAmplitudeFunction(System.Drawing.Color c);
    public static double ColorToBrightness(System.Drawing.Color c) 
    {
      return c.GetBrightness();
    }


    public static void ImportImage(WorksheetController ctrl)
    {
      ImportImage(ctrl.DataTable);
    }


    public static void ImportImage(Altaxo.Data.DataTable table)
    {
      ColorAmplitudeFunction colorfunc;
      System.IO.Stream myStream;
      OpenFileDialog openFileDialog1 = new OpenFileDialog();

      openFileDialog1.InitialDirectory = "c:\\" ;
      openFileDialog1.Filter = "Image files (*.bmp;*.jpg;*.png,*.tif)|*.bmp;*.jpg;*.png,*.tif|All files (*.*)|*.*";
      openFileDialog1.FilterIndex = 1 ;
      openFileDialog1.RestoreDirectory = true ;

      if(openFileDialog1.ShowDialog() == DialogResult.OK)
      {
        if((myStream = openFileDialog1.OpenFile())!= null)
        {
          System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(myStream);

          int sizex = bmp.Width;
          int sizey = bmp.Height;
          //if(Format16bppGrayScale==bmp.PixelFormat)
          
          colorfunc = new ColorAmplitudeFunction(ColorToBrightness);
          // add here other function or the result of a dialog box
  
          // now add new columns to the worksheet, 
          // the name of the columns should preferabbly simply
          // the index in x direction

          table.Suspend();
          for(int i=0;i<sizex;i++)
          {
            Altaxo.Data.DoubleColumn dblcol = new Altaxo.Data.DoubleColumn();
            for(int j=sizey-1;j>=0;j--)
              dblcol[j] = colorfunc(bmp.GetPixel(i,j));

            table.DataColumns.Add(dblcol,table.DataColumns.FindUniqueColumnName(i.ToString())); // Spalte hinzufügen
          } // end for all x coordinaates

          table.Resume();

          myStream.Close();
          myStream=null;
        } // end if myStream was != null
      } 
    }

  }
  
}
