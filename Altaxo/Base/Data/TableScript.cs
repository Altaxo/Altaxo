#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
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

namespace Altaxo.Data
{
  /// <summary>
  /// Holds the text, the module (=executable), and some properties of a column script. 
  /// </summary>
  [SerializationSurrogate(0,typeof(Altaxo.Data.TableScript.SerializationSurrogate0))]
  [SerializationVersion(0)]
  [Serializable()]
  public class TableScript : ICloneable, System.Runtime.Serialization.IDeserializationCallback
  {
    /// <summary>
    /// The text of the column script.
    /// </summary>
    public string m_ScriptText; // the text of the script

    /// <summary>
    /// True when the text changed from last time this flag was reseted.
    /// </summary>
    [NonSerialized()]
    protected bool  m_IsDirty; // true when text changed, can be reseted

    /// <summary>
    /// False when the text has changed and was not compiled already.
    /// </summary>
    [NonSerialized()]
    protected bool m_Compiled; // false when text changed and not compiled already

    /// <summary>
    /// The assembly that holds the created script class.
    /// </summary>
    [NonSerialized()]
    protected System.Reflection.Assembly m_ScriptAssembly;

    /// <summary>
    /// The script object. This is a instance of the newly created script class, which is derived class of <see cref="Altaxo.Calc.ColScriptExeBase"/>
    /// </summary>
    [NonSerialized()]
    protected Altaxo.Calc.TableScriptExeBase m_ScriptObject; // the compiled and created script object

    /// <summary>
    /// The name of the script. This is set to a arbitrary unique name ending in ".cs".
    /// </summary>
    [NonSerialized()]
    protected string m_ScriptName;

    /// <summary>
    /// Holds error messages created by the compiler.
    /// </summary>
    /// <remarks>The column script is compiled, if it is dirty. This can happen not only
    /// during the use of the column script dialog, but anytime when you changed the script text and
    /// try to execute the script. That's the reason for holding the compiler error messages
    /// here and not in the script dialog.</remarks>
    [NonSerialized()]
    protected string[] m_Errors=null;



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
        Altaxo.Data.TableScript s = (Altaxo.Data.TableScript)obj;
        info.AddValue("Text",s.m_ScriptText);
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
        Altaxo.Data.TableScript s = (Altaxo.Data.TableScript)obj;
        s.m_ScriptText = info.GetString("Text");
        return s;
      }
    }

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Data.TableScript),0)]
      public class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        Altaxo.Data.TableScript s = (Altaxo.Data.TableScript)obj;
    
        info.AddValue("Text",s.m_ScriptText);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        Altaxo.Data.TableScript s = null!=o ? (Altaxo.Data.TableScript)o : new Altaxo.Data.TableScript();

        s.m_ScriptText = info.GetString("Text");
        return s;
      }
    }

    /// <summary>
    /// Is called when deserialization has finished.
    /// </summary>
    /// <param name="obj"></param>
    public virtual void OnDeserialization(object obj)
    {
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
    {
      this.m_ScriptText  = b.m_ScriptText;
      this.m_ScriptAssembly = b.m_ScriptAssembly;
      this.m_ScriptObject   = b.m_ScriptObject;
      this.m_IsDirty = b.m_IsDirty;
      this.m_Compiled = b.m_Compiled;
      this.m_Errors   = null==b.m_Errors ? null: (string[])b.m_Errors.Clone();
    }


    /// <summary>
    /// Returns the compiler errors as array of strings.
    /// </summary>
    public string[] Errors
    {
      get { return m_Errors; }
    }


    /// <summary>
    /// True if the column script is dirty, i.e. the text changed since the last reset of IsDirty.
    /// </summary>
    public bool IsDirty
    {
      get { return m_IsDirty; }
      set { m_IsDirty = value; }
    }
  
    public static string GenerateScriptName()
    {
      return System.Guid.NewGuid().ToString();
    }

    public string ScriptName
    {
      get
      {
        if(null==m_ScriptName)
          m_ScriptName = GenerateScriptName();

        return m_ScriptName + ".cs";
      }
    }

    /// <summary>
    /// Get / sets the script text
    /// </summary>
    public string ScriptText
    {
      get 
      { 
        if(null==m_ScriptText)
        {
          m_ScriptText = this.CodeHeader + this.CodeStart + this.CodeTail;
        }
        return m_ScriptText;
      }
      set
      {
        m_ScriptText = value; 
        m_IsDirty=true; 
        m_Compiled=false;
      }
    }

    /// <summary>
    /// Gets the index in the script (considered as string), where the
    /// user area starts. This is momentarily behind the comment line
    /// " ----- add your script below this line ------"
    /// </summary>
    public int UserAreaScriptOffset
    {
      get
      {
        if(null==m_ScriptText)
          return 0;
        
        int pos = m_ScriptText.IndexOf(this.CodeStart);

        return pos<0 ? 0 : pos+this.CodeStart.Length;
      }
    }

    /// <summary>
    /// Gets the code header, i.e. the leading script text. It depends on the ScriptStyle.
    /// </summary>
    public string CodeHeader
    {
      get
      {
        return
          "using Altaxo;\r\n" + 
          "using Altaxo.Data;\r\n" + 
          "namespace Altaxo.TableScripts\r\n" + 
          "{\r\n" + 
          "\tpublic class SetTableValues : Altaxo.Calc.TableScriptExeBase\r\n" +
          "\t{\r\n"+
          "\t\tpublic override void Execute(Altaxo.Data.DataTable table)\r\n" +
          "\t\t{\r\n" +
          "\t\t\tAltaxo.Data.DataColumnCollection col = table.DataColumns;\r\n" +
          "\t\t\tAltaxo.Data.DataTableCollection tables = Altaxo.Data.DataTableCollection.GetParentDataTableCollectionOf(table);\r\n"; 
      }
    }

    public string CodeStart
    {
      get
      {
        return
          "\t\t\t// ----- add your script below this line -----\r\n\t\t\t";
      }
    }

    /// <summary>
    /// Get the ending text of the script, dependent on the ScriptStyle.
    /// </summary>
    public string CodeTail
    {
      get
      {
        return 
          "\r\n\r\n\r\n\r\n\r\n" +
          "\t\t\t// ----- add your script above this line -----\r\n" +
          "\t\t} // Execute method\r\n" +
          "\t} // class\r\n" + 
          "} //namespace\r\n";
      }
    }



    /// <summary>
    /// Clones the script.
    /// </summary>
    /// <returns>The cloned object.</returns>
    public object Clone()
    {
      return new TableScript(this);
    }


    /// <summary>
    /// Does the compilation of the script into an assembly.
    /// If it was not compiled before or is dirty, it is compiled first.
    /// From the compiled assembly, a new instance of the newly created script class is created
    /// and stored in m_ScriptObject.
    /// </summary>
    /// <returns>True if successfully compiles, otherwise false.</returns>
    public bool Compile()
    {
      bool bSucceeded=true;

      // we need nothing to do if not dirty and assembly and object is valid
      if(this.m_Compiled && null!=m_ScriptAssembly && null!=m_ScriptObject)
      {
        return false; // keep the error since a compiler error was detected before
      }

      this.m_Compiled = true;

      Microsoft.CSharp.CSharpCodeProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();

      // For Visual Basic Compiler try this :
      //Microsoft.VisualBasic.VBCodeProvider

      System.CodeDom.Compiler.ICodeCompiler compiler = codeProvider.CreateCompiler();
      System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();

      parameters.GenerateInMemory = true;
      parameters.IncludeDebugInformation = true;
      // parameters.OutputAssembly = this.ScriptName;

      // Add available assemblies including the application itself 
      foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) 
      {
        if(!(asm is System.Reflection.Emit.AssemblyBuilder) && asm.Location!=null && asm.Location!=String.Empty)
          parameters.ReferencedAssemblies.Add(asm.Location);
      }

      CompilerResults results = compiler.CompileAssemblyFromSource(parameters, this.ScriptText);

      if (results.Errors.Count > 0) 
      {
        bSucceeded = false;
        this.m_ScriptAssembly = null;
        this.m_ScriptObject = null;

        m_Errors = new string[results.Errors.Count];
        int i=0;
        foreach (CompilerError err in results.Errors) 
        {
          m_Errors[i++] = err.ToString();
        }
      }
      else  
      {
        // try to execute application
        this.m_ScriptAssembly = results.CompiledAssembly;
        this.m_ScriptObject = null;

        try
        {
          this.m_ScriptObject = (Altaxo.Calc.TableScriptExeBase)results.CompiledAssembly.CreateInstance("Altaxo.TableScripts.SetTableValues");
          if(null==m_ScriptObject)
          {
            bSucceeded = false;
            m_Errors = new string[1];
            m_Errors[0] = "Unable to create Scripting object, have you missed it?\n"; 
          }
        }
        catch (Exception ex) 
        {
          bSucceeded = false;
          m_Errors = new string[1];
          m_Errors[0] = "Unable to create Scripting object, have you missed it?\n" + ex.ToString(); 
        }
      }
      return bSucceeded;
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
        m_ScriptObject.Execute(myTable);
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
        m_ScriptObject.Execute(myTable);
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
