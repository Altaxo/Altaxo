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
using Altaxo.Main.Services;

namespace Altaxo.Scripting
{
  #region interface


  public interface IPureScriptText : ICloneable
  {
    /// <summary>
    /// Get / sets the script text
    /// </summary>
    string ScriptText
    {
      get; set;
    }
  }

  public class ScriptText
  {
  }

  /// <summary>
  /// Interface to a script, e.g. a table or column script
  /// </summary>
  public interface IScriptText : IPureScriptText
  {


    string ScriptName
    {
      get;
    }
 
    object ScriptObject
    {
      get;
    }



    /// <summary>
    /// Gives the type of the script object (full name), which is created after successfull compilation.
    /// </summary>
    string ScriptObjectType { get; }

    /// <summary>
    /// Gets the code header, i.e. the leading script text. This includes the using statements
    /// </summary>
    string CodeHeader
    {
      get;
    }

    /// <summary>
    /// Gets the line before the user code starts
    /// </summary>
    string CodeStart
    {
      get;
    }

    /// <summary>
    /// Gets the default code (i.e. an code example)
    /// </summary>
    string CodeUserDefault
    {
      get;
    }

    /// <summary>
    /// Gets the line after the user code ends
    /// </summary>
    string CodeEnd
    {
      get;
    }


    /// <summary>
    /// Get the ending text of the script, dependent on the ScriptStyle.
    /// </summary>
    string CodeTail
    {
      get;
    }
  

    /// <summary>
    /// Gets the index in the script (considered as string), where the
    /// user area starts. This is momentarily behind the comment line
    /// " ----- add your script below this line ------"
    /// </summary>
    int UserAreaScriptOffset
    {
      get;
    }

   
    /// <summary>
    /// Does the compilation of the script into an assembly.
    /// If it was not compiled before or is dirty, it is compiled first.
    /// From the compiled assembly, a new instance of the newly created script class is created
    /// and stored in m_ScriptObject.
    /// </summary>
    /// <returns>True if successfully compiles, otherwise false.</returns>
    bool Compile();
  
    
    /// <summary>
    /// Returns the compiler errors as array of strings.
    /// </summary>
    string[] Errors
    {
      get;
    }

    
  
    /// <summary>
    /// Copies the content of a script to here.
    /// </summary>
    /// <param name="script">The script to copy from.</param>
    /// <param name="forModification">If false, the script incl. compiled assembly is copied. If true,
    /// the compiled assembly is not copied, so that the script text can be modified.</param>
    void CopyFrom(IScriptText script, bool forModification);


    /// <summary>
    /// This clones the script so that the text can be modified.
    /// </summary>
    /// <returns></returns>
    IScriptText CloneForModification();
   
    /// <summary>
    /// Returns true when the script text can not be modified. Use <see cref="CloneForModification" /> to
    /// obtain a writable copy.
    /// </summary>
    bool IsReadOnly
    {
      get;
    }
  }

  #endregion

  /// <summary>
  /// Holds the text, the module (=executable), and some properties of a column script. 
  /// </summary>
  public abstract class AbstractScript
    :
    ICloneable, 
    System.Runtime.Serialization.IDeserializationCallback,
    IScriptText
  {
    /// <summary>
    /// The text of the column script.
    /// </summary>
    public string m_ScriptText; // the text of the script

    /// <summary>
    /// The result of the successfull compiler run. After this variable is set, the script text must not be changed!
    /// </summary>
    [NonSerialized()]
    public IScriptCompilerResult _compilerResult;

    /// <summary>
    /// True when the text changed from last time this flag was reseted.
    /// </summary>
    [NonSerialized()]
    protected bool  m_IsDirty; // true when text changed, can be reseted
    

    /// <summary>
    /// True when the script text was already tried to compile.
    /// </summary>
    [NonSerialized()]
    protected bool  m_WasTriedToCompile; 


    /// <summary>
    /// The script object. This is a instance of the newly created script class.
    /// </summary>
    [NonSerialized()]
    protected object m_ScriptObject; // the compiled and created script object

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
 

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase","Altaxo.Data.AbstractScript",0)]
      [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Scripting.AbstractScript), 1)]
      class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        AbstractScript s = (AbstractScript)obj;
    
        info.AddValue("Text",s.m_ScriptText);
      }
      public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
      {
        AbstractScript s = (AbstractScript)o;
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
    public AbstractScript()
    {
    }

    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="from">The script to copy from.</param>
    public AbstractScript(AbstractScript from)
      : this(from,false)
    {
    }


    /// <summary>
    /// Creates a column script as a copy from another script.
    /// </summary>
    /// <param name="from">The script to copy from.</param>
    /// <param name="forModification">If false, the script incl. compiled assembly is copied. If true,
    /// the compiled assembly is not copied, so that the script text can be modified.</param>
    public AbstractScript(AbstractScript from, bool forModification)
    {
      CopyFrom(from, forModification);
    }
    

    /// <summary>
    /// Copies the content of a script to here.
    /// </summary>
    /// <param name="from">The script to copy from.</param>
    /// <param name="forModification">If false, the script incl. compiled assembly is copied. If true,
    /// the compiled assembly is not copied, so that the script text can be modified.</param>
    public void CopyFrom(AbstractScript from, bool forModification)
    {
      this.m_ScriptText  = from.m_ScriptText;
      this.m_ScriptObject   = from.m_ScriptObject;
      this.m_IsDirty = from.m_IsDirty;
      
      this.m_WasTriedToCompile = forModification ? false : from.m_WasTriedToCompile;
      
      this.m_Errors   = null==from.m_Errors ? null: (string[])from.m_Errors.Clone();
      
      this._compilerResult = forModification ? null :  from._compilerResult; // (not cloning is intented here)
    }

    void IScriptText.CopyFrom(IScriptText from, bool forModification)
    {
      CopyFrom((AbstractScript)from,forModification);
    }

    /// <summary>
    /// Returns the compiler errors as array of strings.
    /// </summary>
    public string[] Errors
    {
      get { return m_Errors; }
    }

    public Assembly ScriptAssembly
    {
      get
      {
        return _compilerResult==null ? null : _compilerResult.ScriptAssembly;
      }
    }

    public object ScriptObject
    {
      get
      {
        return this.m_ScriptObject;
      }
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

    public bool IsReadOnly
    {
      get
      {
        return _compilerResult!=null;
      }
    }

    /// <summary>
    /// Get / sets the script text
    /// </summary>
    public virtual string ScriptText
    {
      get 
      { 
        if(null!=_compilerResult)
        {
          return _compilerResult.ScriptText(0);
        }
        if(null==m_ScriptText)
        {
          m_ScriptText = this.CodeHeader + this.CodeStart + this.CodeUserDefault + this.CodeEnd + this.CodeTail;
        }
        return m_ScriptText;
      }
      set
      {
        if(IsReadOnly)
          throw new ArgumentException("After successfull compilation, the script text can not be changed any more");
        else
        {
          m_ScriptText = value; 
          m_IsDirty=true;
          m_WasTriedToCompile = false;
        }
      }
    }


    /// <summary>
    /// Get the script text hash
    /// </summary>
    public virtual string ScriptTextHash
    {
      get 
      { 
        if(null!=_compilerResult)
        {
          return _compilerResult.ScriptTextHash;
        }
        if(null==m_ScriptText)
        {
          m_ScriptText = this.CodeHeader + this.CodeStart + this.CodeUserDefault + this.CodeEnd + this.CodeTail;
        }
        return Main.Services.ScriptCompilerService.ComputeScriptTextHash(m_ScriptText);
      }
    }

    public override bool Equals(object obj)
    {
      if(!(obj is AbstractScript))
        return base.Equals(obj);
      AbstractScript script = (AbstractScript)obj;
      return this.ScriptText.GetHashCode() == script.ScriptText.GetHashCode();
    }

    public override int GetHashCode()
    {
      return this.ScriptText.GetHashCode();
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
        if(null==ScriptText)
          return 0;
        
        int pos = ScriptText.IndexOf(this.CodeStart);

        return pos<0 ? 0 : pos+this.CodeStart.Length;
      }
    }


    /// <summary>
    /// Gives the type of the script object (full name), which is created after successfull compilation.
    /// </summary>
    public abstract string ScriptObjectType
    {
      get;
    }

    /// <summary>
    /// Gets the code header, i.e. the leading script text. It depends on the ScriptStyle.
    /// </summary>
    public abstract string CodeHeader
    {
      get;
    }

    public abstract string CodeStart
    {
      get;
    }

    public abstract string CodeUserDefault
    {
      get;
    }

    public abstract string CodeEnd
    {
      get;
    }

    /// <summary>
    /// Get the ending text of the script, dependent on the ScriptStyle.
    /// </summary>
    public abstract string CodeTail
    {
      get;
    }

    public abstract object Clone();
    public virtual IScriptText CloneForModification()
    {
      AbstractScript result = (AbstractScript)Clone();
      result._compilerResult = null;
      result.m_ScriptObject = null;
      return result;
    }
    
  

    /// <summary>
    /// This ensures that it was tried to compile the script. If the script object is <c>null</c>, and it was not already tried to compile the
    /// script, the script will be compiled by this function, and the script object will be created if the comilation was sucessfull. The flag isTriedToCompile is set
    /// to <c>true</c> then (independent on the success of the compilation) to avoid subsequent tries to compile the code.
    /// </summary>
    public void MakeSureWasTriedToCompile()
    {
      if (null == m_ScriptObject)
      {
        if(!this.m_WasTriedToCompile)
          Compile();
      }
    }

    /// <summary>
    /// Does the compilation of the script into an assembly.
    /// If it was not compiled before or is dirty, it is compiled first.
    /// From the compiled assembly, a new instance of the newly created script class is created
    /// and stored in m_ScriptObject.
    /// </summary>
    /// <returns>True if successfully compiles, otherwise false.</returns>
    public virtual bool Compile()
    {
      this.m_WasTriedToCompile = true;

      if(_compilerResult!=null)
        return true;

      _compilerResult = ScriptCompilerService.Compile(new string[]{ScriptText},out m_Errors);
      bool bSucceeded = (null!=_compilerResult);
    
      if(_compilerResult!=null)  
      {
        this.m_ScriptObject = null;

        try
        {
          this.m_ScriptObject = _compilerResult.ScriptAssembly.CreateInstance(this.ScriptObjectType);
          if(null==m_ScriptObject)
          {
            bSucceeded = false;
            m_Errors = new string[1];
            m_Errors[0] = string.Format("Unable to create scripting object  (expected type: {0}), please verify namespace and class name!\n",this.ScriptObjectType); 
          }
        }
        catch (Exception ex) 
        {
          bSucceeded = false;
          m_Errors = new string[1];
          m_Errors[0] = string.Format("Exception during creation of scripting object: {0}\n",ex.Message); 
        }
      }
      return bSucceeded;
    }


 

  } // end of class AbstractScript
}
