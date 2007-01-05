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

namespace Altaxo.Scripting
{
  /// <summary>
  /// Holds the text, the module (=executable), and some properties of a data column script. 
  /// </summary>
 
  public class DataColumnScript : AbstractScript, IColumnScriptText
  {
    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Data.DataColumnScript",1)]
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Scripting.DataColumnScript), 2)]
      class XmlSerializationSurrogate1 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        AbstractScript s = (AbstractScript)obj;
    
        info.AddBaseValueEmbedded(s,typeof(AbstractScript));
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DataColumnScript s = null!=o ? (DataColumnScript)o : new DataColumnScript();
        
        // deserialize the base class
        info.GetBaseValueEmbedded(s,typeof(AbstractScript),parent);
        
        return s;
      }
    }



    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Data.ColumnScript",0)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      /// <summary>
      /// ScriptStyle enumerates the style of the column script.
      /// </summary>
      public enum ScriptStyle 
      {
        /// <summary>
        /// ColumnValues are set by indexing the target column, i.e. targetcol[i]=...
        /// The values calculated by the script must therefore be scalar values.
        /// </summary>
        SetColumnValues, 
        /// <summary>
        /// ColumnValues are set by setting the column at once, i.e. targetcol=...<para/>
        /// The values calculated by the script must therefore be columns (1-dimensional arrays).
        /// </summary>
        SetColumn,
        /// <summary>
        /// With this style, you can write code outside the function <see cref="Altaxo.Calc.ColScriptExeBase.Execute"/>
        /// You can even define your own classes and functions for use by the column script.
        /// </summary>
        FreeStyle };

      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        throw new NotSupportedException("Serializing this old type is not supported any longer");
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        DataColumnScript s = null!=o ? (DataColumnScript)o : new DataColumnScript();
        

        ScriptStyle scriptStyle = (ScriptStyle)info.GetInt32("Style");
        string      scriptText = info.GetString("Text");
        string      rowFrom = info.GetString("From");
        string      rowCondition = info.GetString("Cond");
        string      rowTo = info.GetString("To");
        string      rowInc = info.GetString("Inc");

        s.ScriptText = scriptText;

        return s;
      }
    }
  
    #endregion


    /// <summary>
    /// Creates an empty column script. Default Style is "Set Column".
    /// </summary>
    public DataColumnScript()
    {
    }

    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="b">The script to copy from.</param>
    public DataColumnScript(DataColumnScript b)
      : this(b,true)
    {
    }

    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="b">The script to copy from.</param>
    /// <param name="forModification">If true, the new script text can be modified.</param>
    public DataColumnScript(DataColumnScript b, bool forModification)
      : base(b, forModification)
    {
    }

    /// <summary>
    /// Gives the type of the script object (full name), which is created after successfull compilation.
    /// </summary>
    public override string ScriptObjectType
    {
      get { return "Altaxo.Calc.SetDataColumnValues"; }
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
          "\tpublic class SetDataColumnValues : Altaxo.Calc.ColScriptExeBase\r\n" +
          "\t{\r\n"+
          "\t\tpublic override void Execute(Altaxo.Data.DataColumn mycol)\r\n" +
          "\t\t{\r\n" +
          "\t\t\tAltaxo.Data.DataColumnCollection   col = Altaxo.Data.DataColumnCollection.GetParentDataColumnCollectionOf(mycol);\r\n" +
          "\t\t\tAltaxo.Data.DataTable          mytable = Altaxo.Data.DataTable.GetParentDataTableOf(col);\r\n" +
          "\t\t\tAltaxo.Data.DataColumnCollection  pcol = mytable==null? null : mytable.PropertyColumns;\r\n" +
          "\t\t\tAltaxo.Data.DataTableCollection  table = Altaxo.Data.DataTableCollection.GetParentDataTableCollectionOf(mytable);\r\n"; 
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
          "\t\t\tmycol.Data = col[\"B\"] - col[\"A\"];\r\n" +
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
          "\t\t} // Execute method\r\n" +
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
      return new DataColumnScript(this,true);
    }
   


    /// <summary>
    /// Executes the script. If no instance of the script object exists, a error message will be stored and the return value is false.
    /// If the script object exists, the Execute function of this script object is called.
    /// </summary>
    /// <param name="myColumn">The data table this script is working on.</param>
    /// <returns>True if executed without exceptions, otherwise false.</returns>
    /// <remarks>If exceptions were thrown during execution, the exception messages are stored
    /// inside the column script and can be recalled by the Errors property.</remarks>
    public bool Execute(Altaxo.Data.DataColumn myColumn)
    {
      if(null==m_ScriptObject)
      {
        m_Errors = new string[1]{"Script Object is null"};
        return false;
      }

      try
      {
        ((Altaxo.Calc.ColScriptExeBase)m_ScriptObject).Execute(myColumn);
      }
      catch(Exception ex)
      {
        m_Errors = new string[1];
        m_Errors[0] = ex.ToString();
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
    /// <returns>True if executed without exceptions, otherwise false.</returns>
    /// <remarks>If exceptions were thrown during execution, the exception messages are stored
    /// inside the column script and can be recalled by the Errors property.</remarks>
    public bool ExecuteWithSuspendedNotifications(Altaxo.Data.DataColumn myColumn)
    {
      bool bSucceeded=true;
      Altaxo.Data.DataTableCollection   myDataSet=null;

      // first, test some preconditions
      if(null==m_ScriptObject)
      {
        m_Errors = new string[1]{"Script Object is null"};
        return false;
      }

      Altaxo.Data.DataColumnCollection myColumnCollection = Altaxo.Data.DataColumnCollection.GetParentDataColumnCollectionOf(myColumn);

      Altaxo.Data.DataTable myTable = Altaxo.Data.DataTable.GetParentDataTableOf(myColumnCollection);

      myDataSet = Altaxo.Data.DataTableCollection.GetParentDataTableCollectionOf(myTable);

      if(null!=myDataSet) 
        myDataSet.Suspend();
      else if(null!=myTable)
        myTable.Suspend();
      else if(null!=myColumnCollection)
        myColumnCollection.Suspend();
      else if(null!=myColumn)
        myColumn.Suspend();

      try
      {
        ((Altaxo.Calc.ColScriptExeBase)m_ScriptObject).Execute(myColumn);
      }
      catch(Exception ex)
      {
        bSucceeded = false;
        m_Errors = new string[1];
        m_Errors[0] = ex.ToString();
      }
      finally
      {
        if(null!=myDataSet) 
          myDataSet.Resume();
        else if(null!=myTable)
          myTable.Resume();
        else if(null!=myColumnCollection)
          myColumnCollection.Resume();
        else if(null!=myColumn)
          myColumn.Resume();
      }

      return bSucceeded; 
    }

  } // end of class DataColumnScript

}
