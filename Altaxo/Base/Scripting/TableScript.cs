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
  /// Holds the text, the module (=executable), and some properties of a column script. 
  /// </summary>
  [SerializationSurrogate(0,typeof(TableScript.SerializationSurrogate0))]
  [SerializationVersion(0)]
  [Serializable()]
  public class TableScript 
    :
    AbstractScript,    
    System.Runtime.Serialization.IDeserializationCallback
    
  {
   
 

    #region Serialization
    /// <summary>
    /// Responsible for serialization of the column script version 0.
    /// </summary>
    public class SerializationSurrogate0 : System.Runtime.Serialization.ISerializationSurrogate
    {
      /// <summary>
      /// Serializes the table script
      /// </summary>
      /// <param name="obj">The table script to serialize.</param>
      /// <param name="info">Serialization info.</param>
      /// <param name="context">Streaming context.</param>
      public void GetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context  )
      {
        TableScript s = (TableScript)obj;
        info.AddValue("Text",s.ScriptText);
      }

      /// <summary>
      /// Deserialized the column script.
      /// </summary>
      /// <param name="obj">The uninitialized column script instance.</param>
      /// <param name="info">Serialization info.</param>
      /// <param name="context">Streaming context.</param>
      /// <param name="selector">Surrogate selector.</param>
      /// <returns></returns>
      public object SetObjectData(object obj,System.Runtime.Serialization.SerializationInfo info,System.Runtime.Serialization.StreamingContext context,System.Runtime.Serialization.ISurrogateSelector selector)
      {
        TableScript s = (TableScript)obj;
        s.ScriptText = info.GetString("Text");
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Data.TableScript",0)]
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Scripting.TableScript), 1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        TableScript s = (TableScript)obj;
    
        info.AddValue("Text",s.ScriptText);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        TableScript s = null!=o ? (TableScript)o : new TableScript();

        s.ScriptText = info.GetString("Text");
        return s;
      }
    }

    /// <summary>
    /// Is called when deserialization has finished.
    /// </summary>
    /// <param name="obj"></param>
    public override void OnDeserialization(object obj)
    {
      base.OnDeserialization(obj);
    }
    #endregion


    /// <summary>
    /// Creates an empty column script. Default Style is "Set Column".
    /// </summary>
    public TableScript()
    {
    }

    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="b">The script to copy from.</param>
    public TableScript(TableScript b)
      : this(b,false)
    {
    }
    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="b">The script to copy from.</param>
    /// <param name="forModification">If true, the new script text can be modified.</param>
    public TableScript(TableScript b, bool forModification)
      : base(b, forModification)
    {
    }
  



    /// <summary>
    /// Gives the type of the script object (full name), which is created after successfull compilation.
    /// </summary>
    public override string ScriptObjectType
    {
      get { return "Altaxo.Calc.SetTableValues"; }
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
          "using Altaxo.Data;\r\n" + 
          "using Altaxo.Calc;\r\n" + 
          "namespace Altaxo.Calc\r\n" + 
          "{\r\n" + 
          "\tpublic class SetTableValues : Altaxo.Calc.TableScriptExeBase\r\n" +
          "\t{\r\n"+
          "\t\tpublic override void Execute(Altaxo.Data.DataTable mytable)\r\n" +
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
          "\t\t\tfor(int i=0;i<col.RowCount;i++)\r\n" +
          "\t\t\t\t{\r\n" +
          "\t\t\t\t// Add your code here\r\n" +
          "\t\t\t\t}\r\n" +
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
      return new TableScript(this,true);
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

      try
      {
        ((Altaxo.Calc.TableScriptExeBase)m_ScriptObject).Execute(myTable);
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
    /// <param name="myTable">The data table this script is working on.</param>
    /// <returns>True if executed without exceptions, otherwise false.</returns>
    /// <remarks>If exceptions were thrown during execution, the exception messages are stored
    /// inside the column script and can be recalled by the Errors property.</remarks>
    public bool ExecuteWithSuspendedNotifications(Altaxo.Data.DataTable myTable)
    {
      bool bSucceeded=true;
      Altaxo.Data.DataTableCollection   myDataSet=null;

      // first, test some preconditions
      if(null==m_ScriptObject)
      {
        m_Errors = new string[1]{"Script Object is null"};
        return false;
      }

      myDataSet = Altaxo.Data.DataTableCollection.GetParentDataTableCollectionOf(myTable);

      if(null!=myDataSet) 
        myDataSet.Suspend();
      else if(null!=myTable)
        myTable.Suspend();

      try
      {
        ((Altaxo.Calc.TableScriptExeBase)m_ScriptObject).Execute(myTable);
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
      }

      return bSucceeded; 
    }

  } // end of class TableScript
}
