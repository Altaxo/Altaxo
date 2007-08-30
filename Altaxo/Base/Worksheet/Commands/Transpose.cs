using System;
using System.Collections.Generic;
using System.Text;

using Altaxo.Data;

namespace Altaxo.Worksheet.Commands
{
  /// <summary>
  /// Contains methods to transpose a worksheet.
  /// </summary>
  public static class Transpose
  {
    /// <summary>
    /// Tests if the transpose of a table is possible.
    /// </summary>
    /// <param name="table">Table to test.</param>
    /// <param name="numConvertedDataColumns">Number of data columns (beginning from index 0) that will be converted to property columns.</param>
    /// <param name="indexOfProblematicColumn">On return, if transpose is not possible, will give the index of the first column which differs in type from the first transposed data column.</param>
    /// <returns>True when the transpose is possible without problems, false otherwise.</returns>
    public static bool IsTransposePossible(DataTable table, int numConvertedDataColumns, out int indexOfProblematicColumn)
    {
      if (numConvertedDataColumns < 0)
        throw new ArgumentOutOfRangeException("numConvertedDataColumns is less than zero");
      
      indexOfProblematicColumn = 0;
      if (numConvertedDataColumns >= table.DataColumnCount)
        return true; // when all columns convert to property columns, that will be no problem

      System.Type masterColumnType = table[numConvertedDataColumns].GetType();

      for (int i = numConvertedDataColumns+1; i < table.DataColumnCount; i++)
      {
        if (table[i].GetType() != masterColumnType)
        {
          indexOfProblematicColumn = i;
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Transpose a worksheet.
    /// </summary>
    /// <param name="table">The table to transpose.</param>
    /// <param name="numConvertedDataColumns">Number of data columns that will be converted to property columns.</param>
    /// <param name="numConvertedPropertyColumns">Number of property columns that will become data columns.</param>
    /// <param name="allowUserInteraction">If set to true, and transpose is not possible without problems, the user will be ask to cancel the transpose. 
    /// If set to false, the transpose will be performed anyway. (But you can ask if transpose is possible by calling <c>IsTransposePossible</c>.
    /// </param>
    /// <returns>Null if the transpose was performed without problems, otherwise a error message would be given.</returns>
    static public string DoTranspose(DataTable table, int numConvertedDataColumns, int numConvertedPropertyColumns, bool allowUserInteraction)
    {
      int datacols = Math.Min(table.DataColumnCount, numConvertedDataColumns);
      int propcols = Math.Min(table.PropertyColumnCount, numConvertedPropertyColumns);

      // test if the transpose is possible
      int indexDifferentColumn;
      if (!IsTransposePossible(table, datacols, out indexDifferentColumn))
      {
       

        if (allowUserInteraction)
        {
          string message = string.Format("The columns to transpose have not all the same type. The type of column[{0}] ({1}) differs from the type of column[{2}] ({3}). Continue anyway?",
         indexDifferentColumn,
         table[indexDifferentColumn].GetType(),
         datacols,
         table[datacols].GetType());

         bool result = Current.Gui.YesNoMessageBox(message, "Attention", false);
          if (result == false)
            return "Cancelled by user";
        }
      }


      string error = table.Transpose(datacols, propcols);
      if (error != null && allowUserInteraction)
      {
        Current.Gui.ErrorMessageBox(error);
      }


      return error;
    }
  }
}
