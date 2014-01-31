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

using Altaxo.Collections;
using Altaxo.Gui.Worksheet.Viewing;
using Altaxo.Serialization.Ascii;
using System;
using System.IO;

namespace Altaxo.Data
{
	/// <summary>
	/// Extensions for saving, exporting, and importing files.
	/// </summary>
	public static class FileCommands
	{
		/// <summary>
		/// Imports Ascii data from a stream into the data table.
		/// </summary>
		/// <param name="dataTable">The table where to import into.</param>
		/// <param name="myStream">The stream to import from.</param>
		/// <param name="streamOriginHint">Designates a short hint where the provided stream originates from. Can be <c>Null</c> if the origin is unknown.</param>
		public static void ImportAscii(this DataTable dataTable, System.IO.Stream myStream, string streamOriginHint)
		{
			AsciiImporter.Import(myStream, streamOriginHint, dataTable);
		}

		/// <summary>
		/// Imports multiple Ascii files into the provided table and additionally created tables.
		/// </summary>
		/// <param name="dataTable">The data table where the first ascii file is imported to. Can be null.</param>
		/// <param name="filenames">The names of the files to import.</param>
		/// <param name="importOptions">Options used to import ASCII. This parameter can be <c>null</c>. In this case the options are determined by analysis of each file.</param>
		public static void ImportAsciiToMultipleWorksheets(this DataTable dataTable, string[] filenames, AsciiImportOptions importOptions)
		{
			AsciiImporter.ImportAsciiToMultipleWorksheets(filenames, dataTable, importOptions);
		}

		/// <summary>
		/// Imports multiple Ascii files into a single data table, horizontally, i.e. in subsequent columns.
		/// </summary>
		/// <param name="dataTable">The data table where to import the data.</param>
		/// <param name="filenames">The files names. The names will be sorted before use.</param>
		/// <param name="importOptions">Options used to import ASCII. This parameter can be <c>null</c>. In this case the options are determined by analysis of each file.</param>
		public static void ImportAsciiToSingleWorksheetHorizontally(this DataTable dataTable, string[] filenames, AsciiImportOptions importOptions)
		{
			Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order
			AsciiImporter.ImportMultipleAsciiHorizontally(filenames, dataTable, importOptions);
		}

		/// <summary>
		/// Imports multiple Ascii files into a single data table, vertically, i.e. in subsequent rows.
		/// </summary>
		/// <param name="dataTable">The data table where to import the data.</param>
		/// <param name="filenames">The files names. The names will be sorted before use.</param>
		/// <param name="importOptions">Options used to import ASCII. This parameter can be <c>null</c>. In this case the options are determined by analysis of each file.</param>
		public static void ImportAsciiToSingleWorksheetVertically(this DataTable dataTable, string[] filenames, AsciiImportOptions importOptions)
		{
			Array.Sort(filenames); // Windows seems to store the filenames reverse to the clicking order or in arbitrary order
			AsciiImporter.ImportMultipleAsciiVertically(filenames, dataTable, importOptions);
		}

		/// <summary>
		/// Asks for file name(s) and imports the file(s) into multiple worksheets.
		/// </summary>
		/// <param name="dataTable">The data table to import to. Can be null (in this case the data are imported into a new data table).</param>
		public static void ShowImportAsciiDialog(this DataTable dataTable)
		{
			ShowImportAsciiDialog(dataTable, true, false);
		}

		/// <summary>
		/// Asks for file name(s) and imports the file(s) into multiple worksheets.
		/// </summary>
		/// <param name="dataTable">The data table to import to. Can be null (in this case the data are imported into a new data table).</param>
		public static void ShowImportAsciiDialogAndOptions(this DataTable dataTable)
		{
			ShowImportAsciiDialogAndOptions(dataTable, true, false);
		}

		/// <summary>
		/// Asks for file name(s) and imports the file(s) into one or multiple worksheets.
		/// </summary>
		/// <param name="dataTable">The data table to import to. Can be null if <paramref name="toMultipleWorksheets"/> is set to <c>true</c>.</param>
		/// <param name="toMultipleWorksheets">If true, multiple files are imported into multiple worksheets. New worksheets were then created automatically.</param>
		/// <param name="vertically">If <c>toMultipleWorksheets</c> is false, and this option is true, the data will be exported vertically (in the same columns) instead of horizontally.</param>
		public static void ShowImportAsciiDialog(this DataTable dataTable, bool toMultipleWorksheets, bool vertically)
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
					ImportAsciiToMultipleWorksheets(dataTable, options.FileNames, null);
				else
				{
					if (vertically)
						ImportAsciiToSingleWorksheetVertically(dataTable, options.FileNames, null);
					else
						ImportAsciiToSingleWorksheetHorizontally(dataTable, options.FileNames, null);
				}
			}
		}

		/// <summary>
		/// Asks for file name(s) and imports the file(s) into one or multiple worksheets. After the user chooses one or multiple files, one of the chosen files is used for analysis.
		/// The result of the structure analysis of this file is then presented to the user. The user may change some of the options and starts a re-analysis. Finally, the import options
		/// are confirmed by the user and the import process can start.
		/// </summary>
		/// <param name="dataTable">The data table to import to. Can be null if <paramref name="toMultipleWorksheets"/> is set to <c>true</c>.</param>
		/// <param name="toMultipleWorksheets">If true, multiple files are imported into multiple worksheets. New worksheets were then created automatically.</param>
		/// <param name="vertically">If <c>toMultipleWorksheets</c> is false, and this option is true, the data will be exported vertically (in the same columns) instead of horizontally.</param>
		public static void ShowImportAsciiDialogAndOptions(this DataTable dataTable, bool toMultipleWorksheets, bool vertically)
		{
			var options = new Altaxo.Gui.OpenFileOptions();
			options.AddFilter("*.csv;*.dat;*.txt", "Text files (*.csv;*.dat;*.txt)");
			options.AddFilter("*.*", "All files (*.*)");
			options.FilterIndex = 0;
			options.RestoreDirectory = true;
			options.Multiselect = true;

			if (Current.Gui.ShowOpenFileDialog(options) && options.FileNames.Length > 0)
			{
				AsciiImportOptions importOptions;
				var analysisOptions = dataTable.GetPropertyValue(AsciiDocumentAnalysisOptions.PropertyKeyAsciiDocumentAnalysisOptions, null);
				if (!ShowAsciiImportOptionsDialog(options.FileName, analysisOptions, out importOptions))
					return;

				if (toMultipleWorksheets)
				{
					ImportAsciiToMultipleWorksheets(dataTable, options.FileNames, importOptions);
				}
				else
				{
					if (vertically)
						ImportAsciiToSingleWorksheetVertically(dataTable, options.FileNames, importOptions);
					else
						ImportAsciiToSingleWorksheetHorizontally(dataTable, options.FileNames, importOptions);
				}
			}
		}

		/// <summary>
		/// Shows the ASCII analysis dialog.
		/// </summary>
		/// <param name="fileName">Name of the file to analyze.</param>
		/// <param name="importOptions">On return, contains the ASCII import options the user has confirmed.</param>
		/// <param name="analysisOptions">Options that specify how many lines are analyzed, and what number formats and date/time formats will be tested.</param>
		/// <returns><c>True</c> if the user confirms this dialog (clicks OK). False if the user cancels this dialog.</returns>
		public static bool ShowAsciiImportOptionsDialog(string fileName, AsciiDocumentAnalysisOptions analysisOptions, out AsciiImportOptions importOptions)
		{
			importOptions = new AsciiImportOptions();

			using (FileStream str = AsciiImporter.GetAsciiInputFileStream(fileName))
			{
				importOptions = AsciiDocumentAnalysis.Analyze(new AsciiImportOptions(), str, analysisOptions);
				object[] args = new object[] { importOptions, str };
				var controller = (Altaxo.Gui.IMVCAController)Current.Gui.GetControllerAndControl(args, typeof(Altaxo.Gui.IMVCAController), Gui.UseDocument.Directly);

				if (!Current.Gui.ShowDialog(controller, "Choose ASCII import options"))
					return false;

				importOptions = (AsciiImportOptions)controller.ModelObject;
				return true;
			}
		}

		/// <summary>
		/// Asks for a file name and exports the table data into that file as Ascii.
		/// </summary>
		/// <param name="dataTable">DataTable to export.</param>
		public static void ShowExportAsciiDialog(this DataTable dataTable)
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
						Altaxo.Serialization.Ascii.AsciiExporter.ExportAscii(myStream, dataTable, '\t');
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

		/// <summary>
		/// Asks for file names, and imports one or more Galactic SPC files into a single data table.
		/// </summary>
		/// <param name="dataTable">Data table to import to.</param>
		public static void ShowImportGalacticSPCDialog(this DataTable dataTable)
		{
			Altaxo.Serialization.Galactic.Import.ShowDialog(dataTable);
		}

		/// <summary>
		/// Asks for file names, and imports one or more Jcamp files into a single data table.
		/// </summary>
		/// <param name="dataTable">Data table to import to.</param>
		public static void ShowImportJcampDialog(this DataTable dataTable)
		{
			Altaxo.Serialization.Jcamp.Import.ShowDialog(dataTable);
		}

		/// <summary>
		/// Shows the dialog for Galactic SPC file export, and exports the data of the table using the options provided in that dialog.
		/// </summary>
		/// <param name="dataTable">DataTable to export.</param>
		/// <param name="selectedDataRows">Rows to export (can be null - then all rows will be considered for export).</param>
		/// <param name="selectedDataColumns">Columns to export (can be null - then all columns will be considered for export).</param>
		public static void ShowExportGalacticSPCDialog(this DataTable dataTable, IAscendingIntegerCollection selectedDataRows, IAscendingIntegerCollection selectedDataColumns)
		{
			var exportCtrl = new Altaxo.Gui.Worksheet.ExportGalacticSpcFileDialogController(dataTable, selectedDataRows, selectedDataColumns);
			Current.Gui.ShowDialog(exportCtrl, "Export Galactic SPC format");
		}

		public delegate double ColorAmplitudeFunction(System.Drawing.Color c);

		public static double ColorToBrightness(System.Drawing.Color c)
		{
			return c.GetBrightness();
		}

		/// <summary>
		/// Asks for a file name of an image file, and imports the image data into a data table.
		/// </summary>
		/// <param name="table">The table to import to.</param>
		public static void ShowImportImageDialog(this DataTable table)
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