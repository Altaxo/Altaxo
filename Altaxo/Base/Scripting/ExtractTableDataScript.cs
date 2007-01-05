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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Altaxo.Serialization;
using Altaxo.Data;

namespace Altaxo.Scripting
{

  public interface IExtractTableDataScriptText : IScriptText
  {
    /// <summary>
    /// Executes the script. If no instance of the script object exists, a error message will be stored and the return value is false.
    /// If the script object exists, the function "IsRowIncluded" will be called for every row in the source tables data column collection.
    /// If this function returns true, the corresponding row will be copyied to a new data table.
    /// </summary>
    /// <param name="myTable">The data table this script is working on.</param>
    /// <returns>True if executed without exceptions, otherwise false.</returns>
    /// <remarks>If exceptions were thrown during execution, the exception messages are stored
    /// inside the column script and can be recalled by the Errors property.</remarks>
    bool Execute(Altaxo.Data.DataTable myTable);
  }
 
  /// <summary>
  /// Holds the text, the module (=executable), and some properties of a property column script. 
  /// </summary>
 
  public class ExtractTableDataScript : AbstractScript, IExtractTableDataScriptText
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Data.ExtractTableDataScript",0)]
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Scripting.ExtractTableDataScript), 1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        AbstractScript s = (AbstractScript)obj;
    
        info.AddBaseValueEmbedded(s,typeof(AbstractScript));
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        ExtractTableDataScript s = null!=o ? (ExtractTableDataScript)o : new ExtractTableDataScript();
        
        // deserialize the base class
        info.GetBaseValueEmbedded(s,typeof(AbstractScript),parent);
        
        return s;
      }
    }

  
    #endregion


    /// <summary>
    /// Creates an empty column script. Default Style is "Set Column".
    /// </summary>
    public ExtractTableDataScript()
    {
    }

    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="b">The script to copy from.</param>
    public ExtractTableDataScript(ExtractTableDataScript b)
      : this(b,false)
    {
    }

    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="b">The script to copy from.</param>
    /// <param name="forModification">If true, the new script text can be modified.</param>
    public ExtractTableDataScript(ExtractTableDataScript b, bool forModification)
      : base(b, forModification)
    {
    }
    /// <summary>
    /// Gives the type of the script object (full name), which is created after successfull compilation.
    /// </summary>
    public override string ScriptObjectType
    {
      get { return "Altaxo.Calc.ExtractWorksheetDataScript"; }
    }

    /// <summary>
    /// Gets the code header, i.e. the leading script text. It depends on the ScriptStyle.
    /// </summary>
    public override string CodeHeader
    {
      get
      {
        return
          "#region ScriptHeader\r\n"+
          "using System;\r\n" + 
          "using Altaxo;\r\n" + 
          "using Altaxo.Calc;\r\n" + 
          "using Altaxo.Data;\r\n" + 
          "namespace Altaxo.Calc\r\n" + 
          "{\r\n" + 
          "\tpublic class ExtractWorksheetDataScript : Altaxo.Calc.ExtractTableValuesExeBase\r\n" +
          "\t{\r\n"+
          "\t\tpublic override bool IsRowIncluded(Altaxo.Data.DataTable mytable, int i)\r\n" +
          "\t\t{\r\n" +
          "\t\t\tAltaxo.Data.DataColumnCollection  col = mytable.DataColumns;\r\n" +
          "\t\t\tAltaxo.Data.DataColumnCollection pcol = mytable.PropertyColumns;\r\n"+ 
          "\t\t\tAltaxo.Data.DataTableCollection table = Altaxo.Data.DataTableCollection.GetParentDataTableCollectionOf(mytable);\r\n";
      }
    }

    public override string CodeStart
    {
      get
      {
        return
          "#endregion\r\n"+
          "\t\t\t// ----- add your script below this line -----\r\n";
      }
    }

    public override string CodeUserDefault
    {
      get
      {
        return
          "\t\t\t\r\n" + 
          "\t\t\treturn col[\"B\"][i]>0;\r\n" +
          "\t\t\t\r\n"
          ;
      }
    }

    public override string CodeEnd
    {
      get
      {
        return
          "\t\t\t// ----- add your script above this line -----\r\n"+
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
          
          "\t\t} // method\r\n" +
          "\t} // class\r\n" + 
          "} //namespace\r\n"+
          "#endregion\r\n";
      }
    }



    /// <summary>
    /// Clones the script.
    /// </summary>
    /// <returns>The cloned object.</returns>
    public override object Clone()
    {
      return new ExtractTableDataScript(this,true);
    }
    

    /// <summary>
    /// Executes the script. If no instance of the script object exists, a error message will be stored and the return value is false.
    /// If the script object exists, the Execute function of this script object is called.
    /// </summary>
    /// <param name="myTable">The data table this script is working on.</param>
    /// <returns>True if executed without exceptions, otherwise false.</returns>
    /// <remarks>If exceptions were thrown during execution, the exception messages are stored
    /// inside the column script and can be recalled by the Errors property.</remarks>
    public bool Execute(Altaxo.Data.DataTable myTable)
    {
      if(null==m_ScriptObject)
      {
        m_Errors = new string[1]{"Script Object is null"};
        return false;
      }


      DataTable clonedTable = (DataTable)myTable.Clone();
      clonedTable.DataColumns.RemoveRowsAll();


      Altaxo.Collections.AscendingIntegerCollection rowsToCopy = new Altaxo.Collections.AscendingIntegerCollection();

      int len = myTable.DataRowCount;

    

      try
      {
        Altaxo.Calc.ExtractTableValuesExeBase scriptObject = (Altaxo.Calc.ExtractTableValuesExeBase)m_ScriptObject;
        for(int i=0;i<len;i++)
        {
          if(scriptObject.IsRowIncluded(myTable,i))
            rowsToCopy.Add(i);
        }
      }
      catch(Exception ex)
      {
        m_Errors = new string[1];
        m_Errors[0] = ex.ToString();
        return false;
      }

      for(int i=myTable.DataColumns.ColumnCount-1;i>=0;i--)
      {
        for(int j=rowsToCopy.Count-1;j>=0;j--)
        {
          clonedTable.DataColumns[i][j] = myTable.DataColumns[i][rowsToCopy[j]];
        }
      }

      Current.Project.DataTableCollection.Add(clonedTable);
      Current.ProjectService.OpenOrCreateWorksheetForTable(clonedTable);

      return true;
    }
  } // end of class PropertyColumnScript
}
