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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Altaxo.Collections;

namespace Altaxo.Serialization.Galactic
{
  namespace Options
  {

    /// <summary>
    /// Creating a spectrum either from a row or from a column.
    /// </summary>
    /// <remarks>Choosing creating a spectrum from a row, the values of a single table row (or parts of it) are used to create
    /// a spectrum, i.e. the y-values of the spectrum. In this case the x-values can come from a numeric property column.<para/>
    /// Creating a spectrum from a column (or parts of a column, i.e. some rows of it) means the y-values
    /// of the spectrum stem from a single column of the data table. The x-values of the spectrum then have
    /// to come from another (numeric) column of the table.</remarks>
    public enum CreateSpectrumFrom
    {
      /// <summary>The y-values of one spectrum stem from one single table row.</summary>
      Row,
      /// <summary>The y-values of one spectrum stem from one single table column.</summary>
      Column 
    };

    /// <summary>Designates the source of the x data values.</summary>
    public enum XValuesFrom 
    {
      /// <summary>The x data values are continuous numbers starting from 1.</summary>
      ContinuousNumber,
      /// <summary>The x data values are from a (numeric) column.</summary>
      Column 
    };

    /// <summary>The option for file name extension.</summary>
    public enum ExtendFileNameWith 
    {
      /// <summary>The file name is extended with a continuously increasing number.</summary>
      ContinuousNumber, 
      /// <summary>The file name is extended by the contents of a data column.</summary>
      Column
    };
  }


  /// <summary>
  /// The controller class which controls the <see cref="ExportGalacticSpcFileDialog"/> dialog.
  /// </summary>
  public class ExportGalacticSpcFileDialogController
  {
    /// <summary>
    /// The dialog to control.
    /// </summary>
    private ExportGalacticSpcFileDialog m_Form;
    /// <summary>The table where the data stems from.</summary>
    protected Altaxo.Data.DataTable m_Table;

    /// <summary>The selected rows of the table (only this data are exported).</summary>
    protected IAscendingIntegerCollection m_SelectedRows;

    /// <summary>The selected columns of the table (only this data are exported).</summary>
    protected IAscendingIntegerCollection m_SelectedColumns;

    /// <summary>Stores if one spectrum is created from one table row or from one table column (<see cref="Options.CreateSpectrumFrom"/>).</summary>
    protected Options.CreateSpectrumFrom m_CreateSpectrumFrom;

    /// <summary>Stores if the x values stem from continuous numbering or from a data column (<see cref="Options.XValuesFrom"/></summary>
    protected Options.XValuesFrom m_XValuesFrom;

    /// <summary>Stores if the filename is extended by a continuous number or by the contents of a data column (<see cref="Options.ExtendFileNameWith"/></summary>
    protected Options.ExtendFileNameWith m_ExtendFileNameWith;

    /// <summary>
    /// Creates the controller for the Galactic SPC file export dialog.
    /// </summary>
    /// <param name="dlg">The dialog for which this controller is created.</param>
    /// <param name="table">The data table which is about to be exported.</param>
    /// <param name="selectedRows">The selected rows of the data table.</param>
    /// <param name="selectedColumns">The selected columns of the data table.</param>
    public ExportGalacticSpcFileDialogController(
      ExportGalacticSpcFileDialog dlg,
      Altaxo.Data.DataTable table,
      IAscendingIntegerCollection selectedRows,
      IAscendingIntegerCollection selectedColumns)
    {
      m_Form = dlg;
      m_Table = table;
      m_SelectedRows = selectedRows;
      m_SelectedColumns = selectedColumns;

      InitializeElements();
    }


    /// <summary>
    /// Initialize the elements of the dialog.
    /// </summary>
    public void InitializeElements()
    {
      this.m_CreateSpectrumFrom = Options.CreateSpectrumFrom.Row;
      m_Form.CreateSpectrumFromRow=true;

      this.m_XValuesFrom = Options.XValuesFrom.ContinuousNumber;
      m_Form.XValuesContinuousNumber=true;

      this.m_ExtendFileNameWith = Options.ExtendFileNameWith.ContinuousNumber;
      m_Form.ExtendFileName_ContinuousNumber=true;
      m_Form.BasicFileName="";
    }


    /// <summary>
    /// Fills the "x values column combobox" with the appropriate column names.
    /// </summary>
    /// <remarks>The column names are either from property columns (if a spectrum is from a row) or from a table column
    ///  (if the spectrum is from a column).<para/>
    /// In either case, only DataColumns that have the <see cref="Altaxo.Data.INumericColumn"/> interface are shown in the combobox.</remarks>
    public void FillXValuesColumnBox()
    {
      Altaxo.Data.DataColumnCollection colcol;
      if(this.m_CreateSpectrumFrom==Options.CreateSpectrumFrom.Row)
        colcol = m_Table.PropCols;
      else
        colcol = m_Table.DataColumns;

      // count the number of numeric columns
      int numnumeric=0;
      for(int i=0;i<colcol.ColumnCount;i++)
        if(colcol[i] is Altaxo.Data.INumericColumn)
          ++numnumeric;

      // Fill the Combo Box with Column names
      string[] colnames = new string[numnumeric];
      for(int i=0,j=0;i<colcol.ColumnCount;i++)
        if(colcol[i] is Altaxo.Data.INumericColumn)
          colnames[j++]= colcol.GetColumnName(i);
        
      // now set the contents of the combo box
      m_Form.FillXValuesColumnBox(colnames);
      m_Form.EnableXValuesColumnBox=true;
    }

    /// <summary>
    /// This fills the "Extend file name by column" combobox with appropriate column names.
    /// </summary>
    /// <remarks>The columns shown are either table columns (if a spectrum is a single row), or
    /// a property column (if a spectrum is a single column).</remarks>
    public void FillExtFileNameColumnBox()
    {
      Altaxo.Data.DataColumnCollection colcol;
      if(this.m_CreateSpectrumFrom==Options.CreateSpectrumFrom.Row)
        colcol = m_Table.DataColumns;
      else
        colcol = m_Table.PropCols;

      // Fill the Combo Box with Column names
      string[] colnames = new string[colcol.ColumnCount];
      for(int i=0;i<colcol.ColumnCount;i++)
        colnames[i]= colcol.GetColumnName(i);
        
      // now set the contents of the combo box
      m_Form.FillExtFileNameColumnBox(colnames);
      m_Form.EnableExtFileNameColumnBox=true;
    }


    /// <summary>
    /// This opens the "Save as" dialog box to choose a basic file name for exporting.
    /// </summary>
    public void ChooseBasicFileNameAndPath()
    {
      SaveFileDialog saveFileDialog = new SaveFileDialog();
 
      saveFileDialog.Filter = "All files (*.*)|*.*"  ;
      saveFileDialog.FilterIndex = 1 ;
      saveFileDialog.RestoreDirectory = true ;
 
      if(saveFileDialog.ShowDialog(m_Form) == DialogResult.OK)
      {
        m_Form.BasicFileName = saveFileDialog.FileName;
      }
    }

    /// <summary>
    /// Called if a change in the options "Create spectrum from" occurs.
    /// </summary>
    public void EhChange_CreateSpectrumFrom()
    {
      Options.CreateSpectrumFrom oldValue = m_CreateSpectrumFrom;

      if(m_Form.CreateSpectrumFromRow)
        m_CreateSpectrumFrom = Options.CreateSpectrumFrom.Row;
      else if(m_Form.CreateSpectrumFromColumn)
        m_CreateSpectrumFrom = Options.CreateSpectrumFrom.Column;
    
      if(m_CreateSpectrumFrom!=oldValue)
      {
        if(this.m_XValuesFrom == Options.XValuesFrom.Column)
          this.FillXValuesColumnBox();

        if(this.m_ExtendFileNameWith == Options.ExtendFileNameWith.Column)
          this.FillExtFileNameColumnBox();
      }
    }

    /// <summary>
    /// Called if a change in the options "X values from" occured.
    /// </summary>
    public void EhChange_XValuesFromOptions()
    {
      Options.XValuesFrom oldValue = m_XValuesFrom;

      if(m_Form.XValuesContinuousNumber)
        m_XValuesFrom = Options.XValuesFrom.ContinuousNumber;
      else if(m_Form.XValuesFromColumn)
        m_XValuesFrom = Options.XValuesFrom.Column;

      if(m_XValuesFrom != oldValue)
      {
        if(m_XValuesFrom==Options.XValuesFrom.Column)
        {
          this.FillXValuesColumnBox();
        }
        else
        {
          m_Form.EnableXValuesColumnBox=false;
        }
      }
    }


    /// <summary>
    /// Called if a change in the options "Extend file name by" occured.
    /// </summary>
    public void EhChange_ExtendFileNameOptions()
    {
      Options.ExtendFileNameWith oldValue = this.m_ExtendFileNameWith;

      if(m_Form.ExtendFileName_ContinuousNumber)
        this.m_ExtendFileNameWith = Options.ExtendFileNameWith.ContinuousNumber;
      else if(m_Form.ExtendFileName_ByColumn)
        this.m_ExtendFileNameWith = Options.ExtendFileNameWith.Column;

      if(this.m_ExtendFileNameWith != oldValue)
      {
        if(this.m_ExtendFileNameWith==Options.ExtendFileNameWith.Column)
        {
          // now set the contents of the combo box
          this.FillExtFileNameColumnBox();
        }
        else
        {
          m_Form.EnableExtFileNameColumnBox=false;
        }

      }
    }

    /// <summary>
    /// Called if user presses the "Ok" button on the dialog box.
    /// </summary>
    public void EhOk()
    {
      if(this.m_CreateSpectrumFrom == Options.CreateSpectrumFrom.Row)
      {
        Altaxo.Data.INumericColumn xcol;

        if(this.m_XValuesFrom == Options.XValuesFrom.Column)
        {
          string colname = m_Form.XValuesColumnName;
          if(null==colname)
          {
            MessageBox.Show(m_Form, "No x-column selected", "Error");
            return;
          }

          xcol = this.m_Table.PropCols[colname] as Altaxo.Data.INumericColumn;
        }
        else // xvalues are continuous number
        {
          xcol = new Altaxo.Data.IndexerColumn();
        }

        Altaxo.Data.DataColumn extFileNameCol=null;
        if(this.m_ExtendFileNameWith == Options.ExtendFileNameWith.Column)
          extFileNameCol = m_Table[m_Form.ExtFileNameColumnName];



        int i,j;
        bool bUseRowSel = (null!=m_SelectedRows && this.m_SelectedRows.Count>0);
        int numOfSpectra = bUseRowSel ? m_SelectedRows.Count : m_Table.DataColumns.RowCount;

        for(j=0;j<numOfSpectra;j++)
        {
          i = bUseRowSel ? m_SelectedRows[j] : j;

          string filename = m_Form.BasicFileName;

          if(null!=extFileNameCol)
            filename += "_" + extFileNameCol[i].ToString();
          else
            filename += "_" + j.ToString();


          string error = Export.FromRow(filename,this.m_Table,i,xcol,this.m_SelectedColumns);

          if(null!=error)
          {
            MessageBox.Show(m_Form,error,"There were error(s) during export!");
            return;
          }
        }
        
        MessageBox.Show(m_Form, string.Format("Export of {0} spectra successfull.",numOfSpectra)); 
      }
      else if(this.m_CreateSpectrumFrom == Options.CreateSpectrumFrom.Column)
      {
        Altaxo.Data.INumericColumn xcol;

        if(this.m_XValuesFrom == Options.XValuesFrom.Column)
        {
          string colname = m_Form.XValuesColumnName;
          if(null==colname)
          {
            MessageBox.Show(m_Form, "No x-column selected", "Error");
            return;
          }

          xcol = this.m_Table.DataColumns[colname] as Altaxo.Data.INumericColumn;
        }
        else // xvalues are continuous number
        {
          xcol = new Altaxo.Data.IndexerColumn();
        }

        Altaxo.Data.DataColumn extFileNameCol=null;
        if(this.m_ExtendFileNameWith == Options.ExtendFileNameWith.Column)
          extFileNameCol = m_Table.PropCols[m_Form.ExtFileNameColumnName];



        int i,j;
        bool bUseColSel = (null!=m_SelectedColumns && this.m_SelectedColumns.Count>0);
        int numOfSpectra = bUseColSel ? m_SelectedColumns.Count : m_Table.DataColumns.ColumnCount;

        for(j=0;j<numOfSpectra;j++)
        {
          i = bUseColSel ? m_SelectedColumns[j] : j;

          string filename = m_Form.BasicFileName;

          if(null!=extFileNameCol)
            filename += extFileNameCol[i].ToString();
          else
            filename += j.ToString() + ".spc";


          string error = Export.FromColumn(filename,this.m_Table,i,xcol,this.m_SelectedRows);

          if(null!=error)
          {
            MessageBox.Show(m_Form,error,"There were error(s) during export!");
            return;
          }
        }
        
        MessageBox.Show(m_Form, string.Format("Export of {0} spectra successfull.",numOfSpectra)); 
      }
      m_Form.DialogResult = DialogResult.OK;
      m_Form.Close();
    }

    /// <summary>
    /// Called if the user pressed the "Cancel" button on the dialog box.
    /// </summary>
    public void EhCancel()
    {
      m_Form.DialogResult = DialogResult.Cancel;
      m_Form.Close();
    }

  } // end of class ExportGalacticSpcFileDialogController



  /// <summary>
  /// This class hosts all routines neccessary to export Galactic SPC files
  /// </summary>
  public class Export
  {


    /// <summary>
    /// Exports a couple of x and y values into a non-evenly spaced Galactic SPC file.
    /// </summary>
    /// <param name="xvalues">The x values of the spectrum.</param>
    /// <param name="yvalues">The y values of the spectrum.</param>
    /// <param name="filename">The filename where to export to.</param>
    /// <returns>Null if successful, otherwise an error description.</returns>
    public static string FromArrays(double [] xvalues, double [] yvalues, string filename)
    {
      int len = xvalues.Length<yvalues.Length ? xvalues.Length:yvalues.Length;

      if(len==0)
      {
        return "Nothing to export - either x-value or y-value array is empty!";
      }

      System.IO.Stream stream=null;

      try
      {
        stream = new System.IO.FileStream(filename,System.IO.FileMode.CreateNew);
        System.IO.BinaryWriter binwriter = new System.IO.BinaryWriter(stream);


        binwriter.Write((byte)0x80); // ftflgs : not-evenly spaced data
        binwriter.Write((byte)0x4B); // fversn : new version
        binwriter.Write((byte)0x00); // fexper : general experimental technique
        binwriter.Write((byte)0x80); // fexp   : fractional scaling exponent (0x80 for floating point)

        binwriter.Write((System.Int32)len); // fnpts  : number of points

        binwriter.Write((double)xvalues[0]); // ffirst : first x-value
        binwriter.Write((double)xvalues[len-1]); // flast : last x-value
        binwriter.Write((System.Int32)1); // fnsub : 1 (one) subfile only
      
        binwriter.Write((byte)0); //  Type of X axis units (see definitions below) 
        binwriter.Write((byte)0); //  Type of Y axis units (see definitions below) 
        binwriter.Write((byte)0); // Type of Z axis units (see definitions below)
        binwriter.Write((byte)0); // Posting disposition (see GRAMSDDE.H)

        binwriter.Write(new byte[0x1E0]); // writing rest of SPC header


        // ---------------------------------------------------------------------
        //   following the x-values array
        // ---------------------------------------------------------------------

        for(int i=0;i<len;i++)
          binwriter.Write((float)xvalues[i]);

        // ---------------------------------------------------------------------
        //   following the y SUBHEADER
        // ---------------------------------------------------------------------

        binwriter.Write((byte)0); // subflgs : always 0
        binwriter.Write((byte)0x80); // subexp : y-values scaling exponent (set to 0x80 means floating point representation)
        binwriter.Write((System.Int16)0); // subindx :  Integer index number of trace subfile (0=first)

        binwriter.Write((float)0); // subtime;   Floating time for trace (Z axis corrdinate) 
        binwriter.Write((float)0); // subnext;   Floating time for next trace (May be same as beg) 
        binwriter.Write((float)0); // subnois;   Floating peak pick noise level if high byte nonzero 

        binwriter.Write((System.Int32)0); // subnpts;  Integer number of subfile points for TXYXYS type 
        binwriter.Write((System.Int32)0); // subscan; Integer number of co-added scans or 0 (for collect) 
        binwriter.Write((float)0);        // subwlevel;  Floating W axis value (if fwplanes non-zero) 
        binwriter.Write((System.Int32)0); // subresv[4];   Reserved area (must be set to zero) 


        // ---------------------------------------------------------------------
        //   following the y-values array
        // ---------------------------------------------------------------------

        for(int i=0;i<len;i++)
          binwriter.Write((float)yvalues[i]);
      }
      catch(Exception e)
      {
        return e.ToString();
      }
      finally
      {
        if(null!=stream)
          stream.Close();
      }
      
      return null;
    }


    /// <summary>
    /// Exports to a single SPC spectrum from a single table row.
    /// </summary>
    /// <param name="filename">The name of the file where to export to.</param>
    /// <param name="table">The table from which to export.</param>
    /// <param name="rownumber">The number of the table row that contains the data to export.</param>
    /// <param name="xcolumn">The x column that contains the x data.</param>
    /// <param name="selectedColumns">The columns that where selected in the table, i.e. the columns which are exported. If this parameter is null
    /// or no columns are selected, then all data of a row will be exported.</param>
    /// <returns>Null if export was successfull, error description otherwise.</returns>
    public static string FromRow(
      string filename,
      Altaxo.Data.DataTable table,
      int rownumber, 
      Altaxo.Data.INumericColumn xcolumn,
      IAscendingIntegerCollection selectedColumns)
    {

      // test that all x and y cells have numeric values
      bool bUseSel = null!=selectedColumns && selectedColumns.Count>0;
      int spectrumlen = (bUseSel)? selectedColumns.Count : table.DataColumns.ColumnCount ;

      int i,j;

      for(j=0;j<spectrumlen;j++)
      {
        i = bUseSel ? selectedColumns[j] : j;

        if(xcolumn[i] == Double.NaN)
          return string.Format("X column at index {i} has no numeric value!",i);

        if(!(table[i] is Altaxo.Data.INumericColumn))
          return string.Format("Table column[{0}] ({1}) is not a numeric column!",i,table[i].FullName);

        if(((Altaxo.Data.INumericColumn)table[i])[rownumber] == Double.NaN)
          return string.Format("Table cell [{0},{1}] (column {2}) has no numeric value!",i,rownumber,table[i].FullName);
      }


      // this first test was successfull, so start exporting now
  
      double[] xvalues = new double[spectrumlen];
      double[] yvalues = new double[spectrumlen];

      for(j=0;j<spectrumlen;j++)
      {
        i = bUseSel ? selectedColumns[j] : j;
        xvalues[j]= xcolumn[i];
        yvalues[j]= ((Altaxo.Data.INumericColumn)table[i])[rownumber];

      }
      return FromArrays(xvalues,yvalues,filename);
    }

    /// <summary>
    /// Exports to a single SPC spectrum from a single table column.
    /// </summary>
    /// <param name="filename">The name of the file where to export to.</param>
    /// <param name="table">The table from which to export.</param>
    /// <param name="columnnumber">The number of the table column that contains the data to export.</param>
    /// <param name="xcolumn">The x column that contains the x data.</param>
    /// <param name="selectedRows">The rows that where selected in the table, i.e. the rows which are exported. If this parameter is null
    /// or no rows are selected, then all data of a column will be exported.</param>
    /// <returns>Null if export was successfull, error description otherwise.</returns>
    public static string FromColumn(
      string filename,
      Altaxo.Data.DataTable table,
      int columnnumber, 
      Altaxo.Data.INumericColumn xcolumn,
      IAscendingIntegerCollection selectedRows)
    {

      if(!(table.DataColumns[columnnumber] is Altaxo.Data.INumericColumn))
        return string.Format("Table column[{0}] ({1}) is not a numeric column!",columnnumber,table.DataColumns[columnnumber].FullName);



      // test that all x and y cells have numeric values
      bool bUseSel = null!=selectedRows && selectedRows.Count>0;
      int spectrumlen = (bUseSel)? selectedRows.Count : table.DataColumns[columnnumber].Count;

      int i,j;

      for(j=0;j<spectrumlen;j++)
      {
        i = bUseSel ? selectedRows[j] : j;

        if(xcolumn[i] == Double.NaN)
          return string.Format("X column at index {i} has no numeric value!",i);

        if(((Altaxo.Data.INumericColumn)table.DataColumns[columnnumber])[i] == Double.NaN)
          return string.Format("Table cell [{0},{1}] (column {2}) has no numeric value!",columnnumber,i,table.DataColumns[columnnumber].FullName);
      }


      // this first test was successfull, so start exporting now
  
      double[] xvalues = new double[spectrumlen];
      double[] yvalues = new double[spectrumlen];

      for(j=0;j<spectrumlen;j++)
      {
        i = bUseSel ? selectedRows[j] : j;
        xvalues[j]= xcolumn[i];
        yvalues[j]= ((Altaxo.Data.INumericColumn)table.DataColumns[columnnumber])[i];

      }
      return FromArrays(xvalues,yvalues,filename);
    }

  } // end of class Export
}
