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
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;


namespace Altaxo.Main.Services
{
  public interface IScriptCompilerResult
  {
    System.Reflection.Assembly ScriptAssembly { get; }
    int ScriptTextCount { get; }
    string ScriptText(int i);
    string ScriptTextHash { get; }
  }
 

  /// <summary>
  /// Summary description for ScriptCompilerService.
  /// </summary>
  public class ScriptCompilerService
  {
    #region internal classes
    #region FileHash
    public struct FileHash : IComparable
    {
      public ulong Lo;
      public ulong Hi;

      public FileHash(byte[] hash)
        : this(hash,hash.Length)
      {
      }
      public FileHash(byte[] hash, int len)
      {
        if(hash==null)
        {
          Lo=0;
          Hi=0;
        }
        else if(len==16)
        {
          Lo = System.BitConverter.ToUInt64(hash,0);
          Hi = System.BitConverter.ToUInt64(hash,8);
        }
        else
        {
          throw new ArgumentException("Unexpected hash length of " + hash.Length);
        }
      
      }


      public static FileHash FromBinHexRepresentation(string binhex)
      {
        if(binhex.Length!=32)
          throw new ArgumentException("BinHexRepresentation must have a length of 32");

        FileHash hash;
        hash.Hi = ulong.Parse(binhex.Substring( 0,16),System.Globalization.NumberStyles.AllowHexSpecifier);
        hash.Lo = ulong.Parse(binhex.Substring(16,16),System.Globalization.NumberStyles.AllowHexSpecifier);
        return hash;
      }


      public bool Valid 
      {
        get 
        { 
          return Lo!=0 || Hi!=0;
        }
      }

      public override bool Equals(object obj)
      {
        return (obj is FileHash) && (this==(FileHash)obj);
      }

      public override int GetHashCode()
      {
        return Lo.GetHashCode() + Hi.GetHashCode();
      }


      public string BinHexRepresentation
      {
        get
        {
          return Hi.ToString("X16")+Lo.ToString("X16");
        }
      }
      public string MediumFileName
      {
        get
        {
          return string.Format("X{0}.XXX", BinHexRepresentation);
        }
      }
      public static bool operator ==(FileHash a, FileHash b)
      {
        return  a.Hi==b.Hi && a.Lo==b.Lo ;
      }

      public static bool operator !=(FileHash a, FileHash b)
      {
        return a.Hi!=b.Hi || a.Lo!=b.Lo;
      }

      public static bool operator <(FileHash a, FileHash b)
      {
        return a.Hi<b.Hi || a.Lo<b.Lo;
      }

      public static bool operator >(FileHash a, FileHash b)
      {
        return a.Hi>b.Hi || a.Lo>b.Lo;
      }
      #region IComparable Members

      public int CompareTo(object obj)
      {
        if(obj is FileHash)
        {
          return this==(FileHash)obj ? 0 : (this>(FileHash)obj ? 1 : -1);
        }
        else if(obj==null)
        {
          return 0;
        }
        else
          throw new ArgumentException("Argument is not of expected type, but of type " + obj.GetType().ToString());
      }

      #endregion
    }
    #endregion

    #region ScriptCompilerResult
    private class ScriptCompilerResult : IScriptCompilerResult
    {
      Assembly _scriptAssembly;
      string[] _scriptText;
      string _scriptTextHash;

      public ScriptCompilerResult(string[] scriptText, string scriptTextHash, Assembly scriptAssembly)
      {
        _scriptText = (string[])scriptText.Clone();
        _scriptAssembly = scriptAssembly;
        _scriptTextHash = scriptTextHash;
      }

     


      public static string ComputeScriptTextHash(string[] scripts)
      {
        Array.Sort(scripts);
        int len = 0;
        for(int i=0;i<scripts.Length;i++)
          len += scripts[i].Length;

        byte[] hash=null;

        using(System.IO.MemoryStream stream = new System.IO.MemoryStream(len))
        {
          using(System.IO.StreamWriter sw = new System.IO.StreamWriter(stream,System.Text.Encoding.Unicode))
          {
            for(int i=0;i<scripts.Length;i++)
            {
              sw.Write(scripts[i]);
            }
            sw.Flush();

            sw.BaseStream.Seek(0,System.IO.SeekOrigin.Begin);
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            hash = md5.ComputeHash(sw.BaseStream);
            sw.Close();
          }
        }
        return new FileHash(hash).BinHexRepresentation;
      }

      #region IScriptCompilerResult Members

      public string ScriptTextHash 
      {
        get 
        {
          return _scriptTextHash;
        }
      }

      public Assembly ScriptAssembly
      {
        get
        {
          return _scriptAssembly;
        }
      }

      public int ScriptTextCount
      {
        get
        {
          return _scriptText.Length;
        }
      }

      public string ScriptText(int i)
      {
        return _scriptText[i];
      }

      #endregion
    }
  
    #endregion
    #endregion

    static System.Collections.Hashtable _compilerResultsByTextHash = new System.Collections.Hashtable();
    static System.Collections.Hashtable _compilerResultsByAssembly = new System.Collections.Hashtable();


    public IScriptCompilerResult GetCompilerResult(Assembly ass)
    {
      if(_compilerResultsByAssembly.ContainsKey(ass))
        return (IScriptCompilerResult)_compilerResultsByAssembly[ass];
      else
        return null;
    }

    /// <summary>
    /// Computes the Script text hash of a single script text.
    /// </summary>
    /// <param name="scriptText">The script text.</param>
    /// <returns>A hash string which unique identifies the script text.</returns>
    public static string ComputeScriptTextHash(string scriptText)
    {
      return ScriptCompilerResult.ComputeScriptTextHash(new string[]{scriptText});
    }

    /// <summary>
    /// Does the compilation of the script into an assembly. The assembly is stored together with
    /// the read-only source code and returned as result. As list of compiled source codes is maintained by this class.
    /// If you provide a text that was already compiled before, the already compiled assembly is returned instead
    /// of a freshly compiled assembly.
    /// </summary>
    /// <returns>True if successfully compiles, otherwise false.</returns>
    public static IScriptCompilerResult Compile(string[] scriptText, out string[] errors)
    {
     
      string scriptTextHash = ScriptCompilerResult.ComputeScriptTextHash(scriptText);
      if(_compilerResultsByTextHash.Contains(scriptTextHash))
      {
        errors = null;
        return (ScriptCompilerResult)_compilerResultsByTextHash[scriptTextHash];
      }
  
     
      Microsoft.CSharp.CSharpCodeProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider();

      // For Visual Basic Compiler try this :
      //Microsoft.VisualBasic.VBCodeProvider

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

      CompilerResults results;
      if(scriptText.Length==1)
        results = codeProvider.CompileAssemblyFromSource(parameters, scriptText[0]);
      else
        results = codeProvider.CompileAssemblyFromSource(parameters, scriptText);

      if (results.Errors.Count > 0) 
      {
        errors = new string[results.Errors.Count];
        int i=0;
        foreach (CompilerError err in results.Errors) 
        {
          errors[i++] = err.ToString();
        }

        return null;
      }
      else  
      {
        ScriptCompilerResult result = new ScriptCompilerResult(scriptText, scriptTextHash, results.CompiledAssembly);

        _compilerResultsByTextHash.Add(scriptTextHash,result);
        _compilerResultsByAssembly.Add(result.ScriptAssembly,result);
        errors=null;
        return result;
      }
    }
  }
}
