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

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using Altaxo.Main.Services;
using Altaxo.Main.Services.ScriptCompilation;

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
      get;
      set;
    }
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

    object? ScriptObject
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

    bool SetCompilerResult(IScriptCompilerResult result);

    /// <summary>
    /// Returns the compiler errors as array of strings.
    /// </summary>
    IReadOnlyList<ICompilerDiagnostic> Errors
    {
      get;
    }

    /// <summary>
    /// Clears the errors.
    /// </summary>
    void ClearErrors();

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

  #endregion interface

  /// <summary>
  /// Holds the text, the module (=executable), and some properties of a column script.
  /// </summary>
  public abstract class AbstractScript
    :
    Main.SuspendableDocumentLeafNodeWithEventArgs,
    ICloneable,
    IScriptText
  {
    /// <summary>
    /// The text of the column script.
    /// </summary>
    public string? _scriptText; // the text of the script

    /// <summary>
    /// The result of the successfull compiler run. After this variable is set, the script text must not be changed!
    /// </summary>
    [NonSerialized()]
    public IScriptCompilerResult? _compilerResult;

    /// <summary>
    /// True when the text changed from last time this flag was reseted.
    /// </summary>
    [NonSerialized()]
    protected bool _isDirty; // true when text changed, can be reseted

    /// <summary>
    /// True when the script text was already tried to compile.
    /// </summary>
    [NonSerialized()]
    protected bool _wasTriedToCompile;

    /// <summary>
    /// The script object. This is a instance of the newly created script class.
    /// </summary>
    [NonSerialized()]
    protected object? _scriptObject; // the compiled and created script object

    /// <summary>
    /// The name of the script. This is set to a arbitrary unique name ending in ".cs".
    /// </summary>
    [NonSerialized()]
    protected string? _scriptName;

    /// <summary>
    /// Holds error messages created by the compiler.
    /// </summary>
    /// <remarks>The column script is compiled, if it is dirty. This can happen not only
    /// during the use of the column script dialog, but anytime when you changed the script text and
    /// try to execute the script. That's the reason for holding the compiler error messages
    /// here and not in the script dialog.</remarks>
    [NonSerialized()]
    protected IReadOnlyList<ICompilerDiagnostic> _errors = ImmutableArray<ICompilerDiagnostic>.Empty;

    #region Serialization

    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoBase", "Altaxo.Data.AbstractScript", 0)]
    [Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(Altaxo.Scripting.AbstractScript), 1)]
    private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
    {
      public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
      {
        var s = (AbstractScript)obj;

        info.AddValue("Text", s._scriptText);
      }

      public object Deserialize(object? o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object? parent)
      {
        var s = (AbstractScript)(o ?? new ArgumentNullException(nameof(o)));
        s._scriptText = info.GetString("Text");
        return s;
      }
    }

    #endregion Serialization

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
      : this(from, false)
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
      if (ReferenceEquals(this, from))
        return;

      var oldScriptText = _scriptText;
      _scriptText = from._scriptText;
      _isDirty = from._isDirty;

      _wasTriedToCompile = forModification ? false : from._wasTriedToCompile;

      _errors = from._errors; // immutable

      _compilerResult = forModification ? null : from._compilerResult; // (not cloning is intented here)

      _scriptObject = CreateNewScriptObject(); // we create a new script object, because we are unable to clone it

      if (oldScriptText != _scriptText)
        EhSelfChanged(EventArgs.Empty);
    }

    void IScriptText.CopyFrom(IScriptText from, bool forModification)
    {
      CopyFrom((AbstractScript)from, forModification);
    }

    /// <summary>
    /// Returns the compiler errors as array of strings.
    /// </summary>
    public IReadOnlyList<ICompilerDiagnostic> Errors
    {
      get { return _errors; }
    }

    public string GetErrorsAsString()
    {
      var stb = new StringBuilder();

      if (_errors is not null)
      {
        foreach (var err in _errors)
        {
          stb.AppendFormat("Line {0}, Col {1} {2}: {3}\r\n", err.Line, err.Column, err.SeverityText, err.MessageText);
        }
      }
      return stb.ToString();
    }

    public void ClearErrors()
    {
      _errors = ImmutableArray<ICompilerDiagnostic>.Empty;
    }

    public Assembly? ScriptAssembly
    {
      get
      {
        return _compilerResult is IScriptCompilerSuccessfulResult successResult ? successResult.ScriptAssembly : null;
      }
    }

    public object? ScriptObject
    {
      get
      {
        return _scriptObject;
      }
    }

    /// <summary>
    /// True if the column script is dirty, i.e. the text changed since the last reset of IsDirty.
    /// </summary>
    public bool IsDirty
    {
      get { return _isDirty; }
      set { _isDirty = value; }
    }

    public static string GenerateScriptName()
    {
      return System.Guid.NewGuid().ToString();
    }

    public string ScriptName
    {
      get
      {
        if (_scriptName is null)
          _scriptName = GenerateScriptName();

        return _scriptName + ".cs";
      }
    }

    public bool IsReadOnly
    {
      get
      {
        return _compilerResult is not null;
      }
    }

    /// <summary>
    /// Get / sets the script text
    /// </summary>
    public virtual string ScriptText
    {
      get
      {
        if (_compilerResult is not null)
        {
          return _compilerResult.ScriptText(0);
        }
        if (_scriptText is null)
        {
          _scriptText = CodeHeader + CodeStart + CodeUserDefault + CodeEnd + CodeTail;
        }
        return _scriptText;
      }
      set
      {
        if (IsReadOnly)
          throw new ArgumentException("After successfull compilation, the script text can not be changed any more");
        else
        {
          if (_scriptText != value)
          {
            _scriptText = value;
            _isDirty = true;
            _wasTriedToCompile = false;
            EhSelfChanged(EventArgs.Empty);
          }
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
        if (_compilerResult is not null)
        {
          return _compilerResult.ScriptTextHash;
        }
        if (_scriptText is null)
        {
          _scriptText = CodeHeader + CodeStart + CodeUserDefault + CodeEnd + CodeTail;
        }
        return ScriptCompilerService.ComputeScriptTextHash(new string[] { _scriptText });
      }
    }

    public override bool Equals(object? obj)
    {
      return obj is AbstractScript from && this.ScriptText == from.ScriptText;
    }

    public override int GetHashCode()
    {
      return ScriptText.GetHashCode();
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
        if (ScriptText is null)
          return 0;

        int pos = ScriptText.IndexOf(CodeStart);

        return pos < 0 ? 0 : pos + CodeStart.Length;
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
      var result = (AbstractScript)Clone();
      result._compilerResult = null;
      result._scriptObject = null;
      return result;
    }

    /// <summary>
    /// This ensures that it was tried to compile the script. If the script object is <c>null</c>, and it was not already tried to compile the
    /// script, the script will be compiled by this function, and the script object will be created if the comilation was sucessfull. The flag isTriedToCompile is set
    /// to <c>true</c> then (independent on the success of the compilation) to avoid subsequent tries to compile the code.
    /// </summary>
    public void MakeSureWasTriedToCompile()
    {
      if (_scriptObject is null)
      {
        if (!_wasTriedToCompile)
          Compile();
      }
    }

    /// <summary>
    /// Creates a new script object from the compiled assembly.
    /// </summary>
    /// <returns>The new script object. If creation fails, an error is set.</returns>
    private object? CreateNewScriptObject()
    {
      object? scriptObject = null;
      var assembly = ScriptAssembly;

      if (assembly is not null)
      {
        try
        {
          scriptObject = assembly.CreateInstance(ScriptObjectType);

          if (scriptObject is null)
          {
            _errors = ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Error, string.Format("Unable to create scripting object  (expected type: {0}), please verify namespace and class name!\n", ScriptObjectType)));
          }
        }
        catch (Exception ex)
        {
          _errors = ImmutableArray.Create(new CompilerDiagnostic(null, null, DiagnosticSeverity.Error, string.Format("Exception during creation of scripting object: {0}\n", ex.Message)));
        }
      }
      return scriptObject;
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
      _wasTriedToCompile = true;

      if (_compilerResult is not null)
        return true;

      var scriptCompilerResult = ScriptCompilerService.Compile(new string[] { ScriptText });

      return SetCompilerResult(scriptCompilerResult);
    }

    public virtual bool SetCompilerResult(IScriptCompilerResult result)
    {
      _scriptText = result.ScriptText(0);
      _compilerResult = result;
      bool bSucceeded = (_compilerResult is IScriptCompilerSuccessfulResult);

      if (_compilerResult is IScriptCompilerSuccessfulResult)
      {
        _scriptObject = CreateNewScriptObject();
        return true;
      }
      else if (_compilerResult is IScriptCompilerFailedResult failedCompilerResult) // compiler result was not successful
      {
        _errors = failedCompilerResult.CompileErrors;
        return false;
      }
      else
      {
        throw new InvalidProgramException("Unkown compiler result");
      }
    }
  } // end of class AbstractScript
}
