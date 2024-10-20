﻿#region Copyright

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
using System;
using System.Collections.Immutable;
using Altaxo.Main.Services.ScriptCompilation;

namespace Altaxo.Scripting
{
  public interface IColumnScriptText : IScriptText, Main.IDocumentLeafNode
  {
    /// <summary>
    /// Executes the script. If no instance of the script object exists, a error message will be stored and the return value is false.
    /// If the script object exists, the data change notifications will be switched of (for all tables).
    /// Then the Execute function of this script object is called. Afterwards, the data changed notifications are switched on again.
    /// </summary>
    /// <param name="myColumn">The property column this script is working on.</param>
    /// <param name="reporter">Progress reporter that can be used by the script to report the progress of its work.</param>
    /// <returns>True if executed without exceptions, otherwise false.</returns>
    /// <remarks>If exceptions were thrown during execution, the exception messages are stored
    /// inside the column script and can be recalled by the Errors property.</remarks>
    bool ExecuteWithSuspendedNotifications(Altaxo.Data.DataColumn myColumn, IProgressReporter reporter);
  }

  /// <summary>
  /// Holds the text, the module (=executable), and some properties of a property column script.
  /// </summary>

  public class PropertyColumnScript : AbstractScript, IColumnScriptText
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.PropertyColumnScript", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Scripting.PropertyColumnScript), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AbstractScript)obj;

        info.AddBaseValueEmbedded(s, typeof(AbstractScript));
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        PropertyColumnScript s = o is not null ? (PropertyColumnScript)o : new PropertyColumnScript();

        // deserialize the base class
        info.GetBaseValueEmbedded(s, typeof(AbstractScript), parent);

        return s;
      }
    }

    #endregion Serialization

    /// <summary>
    /// Creates an empty column script. Default Style is "Set Column".
    /// </summary>
    public PropertyColumnScript()
    {
    }

    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="b">The script to copy from.</param>
    public PropertyColumnScript(PropertyColumnScript b)
      : this(b, false)
    {
    }

    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="b">The script to copy from.</param>
    /// <param name="forModification">If true, the new script text can be modified.</param>
    public PropertyColumnScript(PropertyColumnScript b, bool forModification)
      : base(b, forModification)
    {
    }

    /// <summary>
    /// Gives the type of the script object (full name), which is created after successfull compilation.
    /// </summary>
    public override string ScriptObjectType
    {
      get { return "Altaxo.Calc.SetPropertyColumnValues"; }
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
          "using System.Linq;\r\n" +
          "using Altaxo;\r\n" +
          "using Altaxo.Calc.LinearAlgebra;\r\n" +
          "using Altaxo.Data;\r\n" +
          "\r\n" +
          "namespace Altaxo.Calc\r\n" +
          "{\r\n" +
          "\tpublic class SetPropertyColumnValues : Altaxo.Calc.ColScriptExeBase\r\n" +
          "\t{\r\n" +
          "\t\tpublic override void Execute(Altaxo.Data.DataColumn mycol, IProgressReporter reporter)\r\n" +
          "\t\t{\r\n" +
          "\t\t\tAltaxo.Data.DataColumnCollection pcol = Altaxo.Data.DataColumnCollection.GetParentDataColumnCollectionOf(mycol);\r\n" +
          "\t\t\tAltaxo.Data.DataTable         mytable = Altaxo.Data.DataTable.GetParentDataTableOf(pcol);\r\n" +
          "\t\t\tAltaxo.Data.DataColumnCollection  col = mytable==null? null : mytable.DataColumns;\r\n" +
          "\t\t\tAltaxo.Data.DataTableCollection table = Altaxo.Data.DataTableCollection.GetParentDataTableCollectionOf(mytable);\r\n";
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
          "\t\t\tmycol.Data = pcol[\"B\"] - pcol[\"A\"];\r\n" +
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
      return new PropertyColumnScript(this, true);
    }

    /// <summary>
    /// Executes the script. If no instance of the script object exists, a error message will be stored and the return value is false.
    /// If the script object exists, the Execute function of this script object is called.
    /// </summary>
    /// <param name="myColumn">The data table this script is working on.</param>
    /// <param name="reporter">Progress reporter that can be used by the script to report the progress of its work.</param>
    /// <returns>True if executed without exceptions, otherwise false.</returns>
    /// <remarks>If exceptions were thrown during execution, the exception messages are stored
    /// inside the column script and can be recalled by the Errors property.</remarks>
    public bool Execute(Altaxo.Data.DataColumn myColumn, IProgressReporter reporter)
    {
      if (_scriptObject is null)
      {
        _errors = ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Error, "Script Object is null"));
        return false;
      }

      try
      {
        ((Altaxo.Calc.ColScriptExeBase)_scriptObject).Execute(myColumn, reporter);
      }
      catch (Exception ex)
      {
        _errors = ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Error, ex.ToString()));
        return false;
      }
      return true;
    }

    /// <summary>
    /// Executes the script. If no instance of the script object exists, a error message will be stored and the return value is false.
    /// If the script object exists, the data change notifications will be switched of (for all tables).
    /// Then the Execute function of this script object is called. Afterwards, the data changed notifications are switched on again.
    /// </summary>
    /// <param name="myColumn">The property column this script is working on.</param>
    /// <param name="reporter">Progress reporter that can be used by the script to report the progress of its work.</param>
    /// <returns>True if executed without exceptions, otherwise false.</returns>
    /// <remarks>If exceptions were thrown during execution, the exception messages are stored
    /// inside the column script and can be recalled by the Errors property.</remarks>
    public bool ExecuteWithSuspendedNotifications(Altaxo.Data.DataColumn myColumn, IProgressReporter reporter)
    {
      bool bSucceeded = true;
      Altaxo.Data.DataTableCollection? myDataSet;

      // first, test some preconditions
      if (_scriptObject is null)
      {
        _errors = ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Error, "Script Object is null"));
        return false;
      }

      var myColumnCollection = Altaxo.Data.DataColumnCollection.GetParentDataColumnCollectionOf(myColumn);

      var myTable = Altaxo.Data.DataTable.GetParentDataTableOf(myColumnCollection);

      myDataSet = myTable is null ? null : Altaxo.Data.DataTableCollection.GetParentDataTableCollectionOf(myTable);

      IDisposable? suspendToken = null;

      if (myDataSet is not null)
        suspendToken = myDataSet.SuspendGetToken();
      else if (myTable is not null)
        suspendToken = myTable.SuspendGetToken();
      else if (myColumnCollection is not null)
        suspendToken = myColumnCollection.SuspendGetToken();
      else
        suspendToken = myColumn.SuspendGetToken();

      try
      {
        ((Altaxo.Calc.ColScriptExeBase)_scriptObject).Execute(myColumn, reporter);
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

    /// <summary>
    /// Executes the script in the background with showing the background dialog. If no instance of the script object exists, a error message will be stored and the return value is false.
    /// If the script object exists, the data change notifications will be switched of (for all tables).
    /// Then the Execute function of this script object is called. Afterwards, the data changed notifications are switched on again.
    /// </summary>
    /// <param name="myColumn">The property column this script is working on.</param>
    /// <returns>True if executed without exceptions, otherwise false.</returns>
    /// <remarks>If exceptions were thrown during execution, the exception messages are stored
    /// inside the column script and can be recalled by the Errors property.</remarks>
    public Exception? ExecuteWithBackgroundDialogAndSuspendNotifications(Altaxo.Data.DataColumn myColumn)
    {
      return Current.Gui.ExecuteAsUserCancellable(1000, (reporter) => ExecuteWithSuspendedNotifications(myColumn, reporter));
    }
  } // end of class PropertyColumnScript
}
