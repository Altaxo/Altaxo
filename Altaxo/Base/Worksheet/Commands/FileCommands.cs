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
using System.IO;
using Altaxo.Serialization.Ascii;
using Altaxo.Gui.Worksheet.Viewing;

namespace Altaxo.Worksheet.Commands
{
	/// <summary>
	/// Summary description for FileCommands.
	/// </summary>
	public class FileCommands
	{
		public static void Save(IWorksheetController ctrl, System.IO.Stream myStream, bool saveAsTemplate)
		{
			Altaxo.Serialization.Xml.XmlStreamSerializationInfo info = new Altaxo.Serialization.Xml.XmlStreamSerializationInfo();
			if (saveAsTemplate)
			{
				info.SetProperty("Altaxo.Data.DataColumn.SaveAsTemplate", "true");
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
			info.AddValue("TablePlusLayout", tableAndLayout);
			info.EndWriting();
		}

		public static void SaveAs(IWorksheetController ctrl, bool saveAsTemplate)
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
					Save(ctrl, myStream, saveAsTemplate);
					myStream.Close();
				}
			}
		}

		public static void ImportAscii(IWorksheetController ctrl, System.IO.Stream myStream)
		{
			AsciiImporter importer = new AsciiImporter(myStream);
			AsciiImportOptions recognizedOptions = importer.Analyze(30, new AsciiImportOptions());
			importer.ImportAscii(recognizedOptions, ctrl.Doc);
		}

		public static void ImportAsciiToMultipleWorksheets(IWorksheetController ctrl, string[] filenames)
		{
			int startrest = 0;

			Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order

			if (ctrl != null)
			{
				using (System.IO.Stream myStream = new System.IO.FileStream(filenames[0], System.IO.FileMode.Open, System.IO.FileAccess.Read))
				{
					ImportAscii(ctrl, myStream);
					myStream.Close();
					startrest = 1;
				}
			}

			// import also the other files, but this time we create new tables
			for (int i = startrest; i < filenames.Length; i++)
			{
				using (System.IO.Stream myStream = new System.IO.FileStream(filenames[i], System.IO.FileMode.Open, System.IO.FileAccess.Read))
				{
					Altaxo.Gui.Worksheet.Viewing.IWorksheetController newwkscontroller = Current.ProjectService.CreateNewWorksheet();
					ImportAscii(newwkscontroller, myStream);
					myStream.Close();
				}
			} // for all files

		}

		public static void ImportAsciiToSingleWorksheet(IWorksheetController ctrl, string[] filenames)
		{
			Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order
			AsciiImporter.ImportMultipleAscii(filenames, ctrl.Doc);
		}


		public static void ImportAscii(IWorksheetController ctrl)
		{
			ImportAscii(ctrl, true);
		}

		public static void ImportAscii(IWorksheetController ctrl, bool toMultipleWorksheets)
		{
			var options = new Altaxo.Gui.OpenFileOptions();
			options.AddFilter("*.csv;*.dat;*.txt", "Text files (*.csv;*.dat;*.txt)");
			options.AddFilter("*.*", "All files (*.*)");
			options.FilterIndex = 0;
			options.RestoreDirectory = true;
			options.Multiselect = true;

			if (Current.Gui.ShowOpenFileDialog(options) && options.FileNames.Length > 0)
			{
				if (toMultipleWorksheets)
					ImportAsciiToMultipleWorksheets(ctrl, options.FileNames);
				else
					ImportAsciiToSingleWorksheet(ctrl, options.FileNames);
			}

		}


		public static void ExportAscii(IWorksheetController ctrl)
		{

			var options = new Altaxo.Gui.SaveFileOptions();
			options.AddFilter("*.csv;*.dat;*.txt", "Text files (*.csv;*.dat;*.txt)");
			options.AddFilter("*.*", "All files (*.*)");
			options.FilterIndex = 0;
			options.RestoreDirectory = true;

			if (Current.Gui.ShowSaveFileDialog(options))
			{
				using (Stream myStream = new FileStream(options.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
				{
					try
					{
						Altaxo.Serialization.Ascii.AsciiExporter.ExportAscii(myStream, ctrl.DataTable, '\t');
					}
					catch (Exception ex)
					{
						Current.Gui.ErrorMessageBox("There was an error during ascii export, details follow:\n" + ex.ToString());
					}
					finally
					{
						myStream.Close();
					}
				}
			}
		}


		public static void ImportGalacticSPC(IWorksheetController ctrl)
		{
			Altaxo.Serialization.Galactic.Import.ShowDialog(ctrl.DataTable);
		}

		public static void ImportJcamp(IWorksheetController ctrl)
		{
			Altaxo.Serialization.Jcamp.Import.ShowDialog(ctrl.DataTable);
		}


		public static void ExportGalacticSPC(IWorksheetController ctrl)
		{
			var exportCtrl = new Altaxo.Gui.Worksheet.ExportGalacticSpcFileDialogController(ctrl.DataTable, ctrl.SelectedDataRows, ctrl.SelectedDataColumns);
			Current.Gui.ShowDialog(exportCtrl, "Export Galactic SPC format");
		}



		public delegate double ColorAmplitudeFunction(System.Drawing.Color c);
		public static double ColorToBrightness(System.Drawing.Color c)
		{
			return c.GetBrightness();
		}


		public static void ImportImage(IWorksheetController ctrl)
		{
			ImportImage(ctrl.DataTable);
		}


		public static void ImportImage(Altaxo.Data.DataTable table)
		{
			ColorAmplitudeFunction colorfunc;
			var options = new Altaxo.Gui.OpenFileOptions();
			options.AddFilter("*.bmp;*.jpg;*.png,*.tif", "Image files (*.bmp;*.jpg;*.png,*.tif)");
			options.AddFilter("*.*", "All files (*.*)");
			options.FilterIndex = 0;
			options.RestoreDirectory = true;

			if (Current.Gui.ShowOpenFileDialog(options))
			{
				using (Stream myStream = new FileStream(options.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
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
					for (int i = 0; i < sizex; i++)
					{
						Altaxo.Data.DoubleColumn dblcol = new Altaxo.Data.DoubleColumn();
						for (int j = sizey - 1; j >= 0; j--)
							dblcol[j] = colorfunc(bmp.GetPixel(i, j));

						table.DataColumns.Add(dblcol, table.DataColumns.FindUniqueColumnName(i.ToString())); // Spalte hinzufügen
					} // end for all x coordinaates

					table.Resume();

					myStream.Close();
				} // end using myStream
			}
		}

	}

}
