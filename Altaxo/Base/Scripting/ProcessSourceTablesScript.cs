#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2022 Dr. Dirk Lellinger
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
using System;
using System.Collections.Immutable;
using Altaxo.Collections;
using Altaxo.Data;
using Altaxo.Main.Services.ScriptCompilation;

namespace Altaxo.Scripting
{
  /// <summary>
  /// Holds the text, the module (=executable), and some properties of a script to process one or multiple source table data.
  /// </summary>
  public class ProcessSourceTablesScript
    :
    AbstractScript
  {
    #region Serialization

    /// <summary>
    /// 2022-08-26 Initial method.
    /// </summary>
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(ProcessSourceTablesScript), 0)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (ProcessSourceTablesScript)obj;

        info.AddValue("Text", s.ScriptText);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (ProcessSourceTablesScript?)o ?? new ProcessSourceTablesScript();

        s.ScriptText = info.GetString("Text");
        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates an empty script.
    /// </summary>
    public ProcessSourceTablesScript()
    {
    }

    /// <summary>
    /// Creates an script as a copy from another script.
    /// </summary>
    /// <param name="b">The script to copy from.</param>
    public ProcessSourceTablesScript(ProcessSourceTablesScript b)
      : this(b, false)
    {
    }

    /// <summary>
    /// Creates an script as a copy from another script.
    /// </summary>
    /// <param name="b">The script to copy from.</param>
    /// <param name="forModification">If true, the new script text can be modified.</param>
    public ProcessSourceTablesScript(ProcessSourceTablesScript b, bool forModification)
      : base(b, forModification)
    {
    }

    /// <summary>
    /// Gives the type of the script object (full name), which is created after successfull compilation.
    /// </summary>
    public override string ScriptObjectType
    {
      get { return "Altaxo.Calc.ProcessSourceTables"; }
    }

    /// <summary>
    /// Gets the code header, i.e. the leading script text. It depends on the ScriptStyle.
    /// </summary>
    public override string CodeHeader
    {
      get
      {
        return
          "#region ScriptHeader\r\n" +
          "using System;\r\n" +
          "using System.Collections.Generic;\r\n" +
          "using System.IO;\r\n" +
          "using System.Linq;\r\n" +
          "using Altaxo;\r\n" +
          "using Altaxo.Calc.LinearAlgebra;\r\n" +
          "using Altaxo.Collections;\r\n" +
          "using Altaxo.Data;\r\n" +
          "\r\n" +
          "namespace Altaxo.Calc\r\n" +
          "{\r\n" +
          "\tpublic class ProcessSourceTables : Altaxo.Calc.ProcessSourceTablesScriptExeBase\r\n" +
          "\t{\r\n" +
          "\r\n" +
          "\t\tpublic override void Execute(Altaxo.Data.DataTable mytable, IReadOnlyListDictionary<string, DataTable> sourceTables, IProgressReporter reporter)\r\n" +
          "\t\t{\r\n" +
          "\t\t\tAltaxo.Data.DataColumnCollection  col = mytable.DataColumns;\r\n" +
          "\t\t\tAltaxo.Data.DataColumnCollection pcol = mytable.PropertyColumns;\r\n" +
          "\t\t\tAltaxo.Data.DataTableCollection table = Altaxo.Data.DataTableCollection.GetParentDataTableCollectionOf(mytable);\r\n" +
          "\t\t\tmytable.PropertyColumns.RemoveColumnsAll();\r\n" +
          "\t\t\tmytable.DataColumns.RemoveColumnsAll();\r\n";
      }
    }

    public override string CodeStart
    {
      get
      {
        return
          "#endregion\r\n" +
          "\t\t\t// ----- add your script below this line -----\r\n";
      }
    }

    public override string CodeUserDefault
    {
      get
      {
        return
          "\t\t\t\r\n" +
          "\t\t\t// in this script, please reference only those tables that are provided in variable 'sourceTables'\r\n" +
          "\t\t\t// the tables can either be accessed by name, or by index, as in the next line:\r\n" +
          "\t\t\tvar srcT = sourceTables[0];\r\n" +
          "\t\t\t\r\n" +
          "\t\t\t// all columns in this tables must be created programatically (because existing columns are deleted before executing this script)\r\n" +
          "\t\t\t// for instance, this creates column \"A\" as x-column and column \"B\" as column for the dependent variable:\r\n" +
          "\t\t\tvar colX = col.EnsureExistence(\"A\", typeof(DoubleColumn), ColumnKind.X, 0);\r\n" +
          "\t\t\tvar colY = col.EnsureExistence(\"B\", typeof(DoubleColumn), ColumnKind.V, 0);\r\n" +
          "\t\t\t\r\n" +
          "\t\t\t\r\n"
          ;
      }
    }

    public override string CodeEnd
    {
      get
      {
        return
          "\t\t\t// ----- add your script above this line -----\r\n" +
          "#region ScriptFooter\r\n";
      }
    }

    /// <summary>
    /// Get the ending text of the script, dependent on the ScriptStyle.
    /// </summary>
    public override string CodeTail
    {
      get
      {
        return
          "\t\t} // Execute method\r\n" +
          "\t} // class\r\n" +
          "} //namespace\r\n" +
          "#endregion\r\n";
      }
    }

    /// <summary>
    /// Clones the script.
    /// </summary>
    /// <returns>The cloned object.</returns>
    public override object Clone()
    {
      return new ProcessSourceTablesScript(this, true);
    }

    /// <summary>
    /// Executes the script. If no instance of the script object exists, the script is compiled. If thereafter no script object exists, a error message will be stored and the return value is false.
    /// If the script object exists, the Execute function of this script object is called.
    /// </summary>
    /// <param name="myTable">The data table this script is working on.</param>
    /// <param name="sourceTables">The source tables that will be processed by this script.</param>
    /// <param name="reporter">Progress reporter that can be used by the script to report the progress of its work.</param>
    /// <returns>True if executed without exceptions, otherwise false.</returns>
    /// <remarks>If exceptions were thrown during execution, the exception messages are stored
    /// inside the column script and can be recalled by the Errors property.</remarks>
    public bool Execute(Altaxo.Data.DataTable myTable, IReadOnlyListDictionary<string, DataTable> sourceTables, IProgressReporter reporter)
    {
      return Execute(myTable, sourceTables, reporter, true);
    }

    /// <summary>
    /// Executes the script. If no instance of the script object exists, the script is compiled. If thereafter no script object exists, a error message will be stored and the return value is false.
    /// If the script object exists, the Execute function of this script object is called.
    /// </summary>
    /// <param name="myTable">The data table this script is working on.</param>
    /// <param name="sourceTables">The source tables that will be processed by this script.</param>
    /// <param name="reporter">Progress reporter that can be used by the script to report the progress of its work.</param>
    /// <remarks>No exceptions are catched here. This function is therefore intended for being called by another script.</remarks>
    public void ExecuteWithoutExceptionCatching(Altaxo.Data.DataTable myTable, IReadOnlyListDictionary<string, DataTable> sourceTables, IProgressReporter reporter)
    {
      Execute(myTable, sourceTables, reporter, false);
    }

    /// <summary>
    /// Executes the script. If no instance of the script object exists, the script is compiled. If thereafter no script object exists, a error message will be stored and the return value is false.
    /// If the script object exists, the Execute function of this script object is called.
    /// </summary>
    /// <param name="myTable">The data table this script is working on.</param>
    /// <param name="sourceTables">The source tables that will be processed by this script.</param>
    /// <param name="reporter">Progress reporter that can be used by the script to report the progress of its work.</param>
    /// <param name="catchExceptionsAndStoreThemInThisScript">If true, exceptions during the script execution are catched and stored here for further investigation by the user.
    /// If you call this script from another script, you should set this parameter to false in order to see the execution errors in your script.</param>
    /// <returns>True if executed without exceptions, otherwise false.</returns>
    /// <remarks>If exceptions were thrown during execution, the exception messages are stored
    /// inside the column script and can be recalled by the Errors property.</remarks>
    public bool Execute(Altaxo.Data.DataTable myTable, IReadOnlyListDictionary<string, DataTable> sourceTables, IProgressReporter reporter, bool catchExceptionsAndStoreThemInThisScript)
    {
      if (_scriptObject is null && !_wasTriedToCompile)
        Compile();

      if (_scriptObject is null)
      {
        if (catchExceptionsAndStoreThemInThisScript)
        {
          _errors = ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Error, "Script Object is null"));
          return false;
        }
        else
        {
          if (_errors is not null && _errors.Count > 0)
          {
            throw new InvalidOperationException("The script object is null because of compilation errors:\r\n" + GetErrorsAsString());
          }
          else
          {
            throw new InvalidOperationException("The script object is null");
          }
        }
      }

      if (catchExceptionsAndStoreThemInThisScript)
      {
        try
        {
          ((Altaxo.Calc.ProcessSourceTablesScriptExeBase)_scriptObject).Execute(myTable, sourceTables, reporter);
        }
        catch (Exception ex)
        {
          _errors = ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Error, ex.ToString()));
          return false;
        }
      }
      else // Execution without catching the exceptions
      {
        ((Altaxo.Calc.ProcessSourceTablesScriptExeBase)_scriptObject).Execute(myTable, sourceTables, reporter);
      }

      return true;
    }

    /// <summary>
    /// Executes the script. If no instance of the script object exists, a error message will be stored and the return value is false.
    /// If the script object exists, the data change notifications will be switched of (for all tables).
    /// Then the Execute function of this script object is called. Afterwards, the data changed notifications are switched on again.
    /// </summary>
    /// <param name="myTable">The data table this script is working on.</param>
    /// <param name="sourceTables">The source tables that will be processed by this script.</param>
    /// <param name="reporter">Progress reporter that can be used by the script to report the progress of its work.</param>
    /// <returns>True if executed without exceptions, otherwise false.</returns>
    /// <remarks>If exceptions were thrown during execution, the exception messages are stored
    /// inside the column script and can be recalled by the Errors property.</remarks>
    public bool ExecuteWithSuspendedNotifications(Altaxo.Data.DataTable myTable, IReadOnlyListDictionary<string, DataTable> sourceTables, Altaxo.IProgressReporter reporter)
    {
      bool bSucceeded = true;
      Altaxo.Data.DataTableCollection? myDataSet = null;

      if (_scriptObject is null && !_wasTriedToCompile)
        Compile();

      // first, test some preconditions
      if (_scriptObject is null)
      {
        _errors = ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Error, "Script Object is null"));
        return false;
      }

      myDataSet = Altaxo.Data.DataTableCollection.GetParentDataTableCollectionOf(myTable);

      IDisposable? suspendToken = null;

      if (myDataSet is not null)
        suspendToken = myDataSet.SuspendGetToken();
      else
        suspendToken = myTable.SuspendGetToken();

      try
      {
        ((Altaxo.Calc.ProcessSourceTablesScriptExeBase)_scriptObject).Execute(myTable, sourceTables, reporter);
      }
      catch (Exception ex)
      {
        bSucceeded = false;
        _errors = ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Error, ex.ToString()));
      }
      finally
      {
        if (suspendToken is not null)
          suspendToken.Dispose();
      }

      return bSucceeded;
    }

    public Exception? ExecuteWithBackgroundDialogAndSuspendNotifications(Altaxo.Data.DataTable myTable, IReadOnlyListDictionary<string, DataTable> sourceTables)
    {
      return Current.Gui.ExecuteAsUserCancellable(1000, (reporter) => ExecuteWithSuspendedNotifications(myTable, sourceTables, reporter));
    }
  } // end of class
}
