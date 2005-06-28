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
using System.Text.RegularExpressions;
using Altaxo.Data;
using Altaxo.Graph;

namespace Altaxo.Worksheet.GUI
{
  #region Interfaces
  public interface IParametrizedFunctionScriptView
  {
    IParametrizedFunctionScriptViewEventSink Controller {get; set; }
    
    string ScriptText { get; set; }

    int ScriptCursorLocation { set; }
    void SetScriptCursorLocation(int line, int column);
    void MarkText(int pos1, int pos2);

    void SetParameterText(string text, bool enable);
    void SetNumberOfParameters(int numberOfParameters, bool enable);

    /// <summary>
    /// Sets the working name of the script. Should be set to a unique name
    /// ending in ".cs" to signal that this is a C# script.
    /// </summary>
    string ScriptName { set; }

    System.Windows.Forms.Form Form    { get ; }

    void ClearCompilerErrors();

    void AddCompilerError(string s);
  }

  public interface IParametrizedFunctionScriptViewEventSink
  {
    void EhView_Execute();
    void EhView_Compile();
    void EhView_Update();
    void EhView_Cancel();
    void EhView_GotoCompilerError(string message);

    void EhView_ParameterChanged(int numberOfParameters, bool userDefinedParameters, string parameterNames);
  }


  #endregion

  /// <summary>
  /// Summary description for TableScriptController.
  /// </summary>
  public class ParametrizedFunctionScriptController : IParametrizedFunctionScriptViewEventSink, Main.GUI.IMVCAController
  {
    protected ScriptExecutionHandler m_ScriptExecution;
    public IParametrizedFunctionDDScriptText m_Script;
     public IParametrizedFunctionDDScriptText m_TempScript;

    protected IParametrizedFunctionScriptView m_View;

    public ParametrizedFunctionScriptController(IParametrizedFunctionDDScriptText script)
    {
        this.m_Script = script;
      if(m_Script!=null)
        m_TempScript = (IParametrizedFunctionDDScriptText)m_Script.Clone();

      SetElements(true);

    }

  


    protected void SetElements(bool bInit)
    {
      if(bInit)
      {
      }

      if(null!=View)
      {
        View.ScriptText= m_TempScript.ScriptText;
        View.ScriptName = m_TempScript.ScriptName;
        View.ScriptCursorLocation = m_TempScript.UserAreaScriptOffset;

        if(m_TempScript.IsUsingUserDefinedParameterNames)
        {
          System.Text.StringBuilder stb = new System.Text.StringBuilder();
          for(int i=0;i<m_TempScript.NumberOfParameters;i++)
          {
            if(i!=0)
              stb.Append(';');
            stb.Append(m_TempScript.UserDefinedParameterNames[i]);
          }
          View.SetParameterText(stb.ToString(),true);
        }
        else
        {
          View.SetNumberOfParameters(m_TempScript.NumberOfParameters,true);
        }
      }
    }

    public IParametrizedFunctionScriptView View
    {
      get
      {
        return m_View;
      }
      set
      {
        if(null!=m_View)
          m_View.Controller = null;
        
        m_View = value;

        if(null!=m_View)
        {
          m_View.Controller = this;
          SetElements(false); // set only the view elements, dont't initialize the variables
        }
      }
    }

    public object ViewObject
    {
      get { return View; }
      set { View = value as IParametrizedFunctionScriptView; }
    }

   

    public void EhView_Execute()
    {
      EhView_Compile();
    }

    private Regex compilerErrorRegex = new Regex(@".*\((?<line>\d+),(?<column>\d+)\) : (?<msg>.+)",RegexOptions.Compiled);
    public void EhView_Compile()
    {
      m_TempScript.ScriptText = View.ScriptText;

      View.ClearCompilerErrors();

      bool bSucceeded = m_TempScript.Compile();

      if(!bSucceeded)
      {
        foreach(string s in m_TempScript.Errors)
        {
          string news = compilerErrorRegex.Match(s).Result("(${line},${column}) : ${msg}");
          View.AddCompilerError(news);
        }
  

        System.Windows.Forms.MessageBox.Show(View.Form, "There were compilation errors","No success");
        return;
      }
      else
      {
        View.AddCompilerError(DateTime.Now.ToLongTimeString() + " : Compilation successful.");
      }


    }

    public void EhView_GotoCompilerError(string msg)
    {
      try
      {
        Match match = compilerErrorRegex.Match(msg);
        string sline = match.Result("${line}");
        string scol = match.Result("${column}");
        int line = int.Parse(sline);
        int col  = int.Parse(scol);

        if(View!=null)
          View.SetScriptCursorLocation(line-1,col-1);

      }
      catch(Exception)
      {
      }
    }

    public void EhView_Update()
    {
      m_TempScript.ScriptText = View.ScriptText;
      // this.m_DataTable.TableScript = m_TableScript;
      this.View.Form.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.View.Form.Close();
    }

    public void EhView_Cancel()
    {
      this.View.Form.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.View.Form.Close();
    }

    bool IsValidParameterName(string name)
    {
      if(name==null)
        return false;
      if(name==string.Empty)
        return false;
      if(!char.IsLetter(name[0]))
        return false;
      for(int i=1;i<name.Length;i++)
        if(!char.IsLetterOrDigit(name[i]))
          return false;

      return true;
    }

    public void EhView_ParameterChanged(int numberOfParameters, bool userDefinedParameters, string parameterNames)
    {
      if(userDefinedParameters)
      {
        // parse the parameter string
        string[] parameters = parameterNames.Split(new char[]{'.',',',';',':'});
        // test if all parameters start with a numeric character and have only
        // alphanumeric characters
        bool[] parameterValid = new bool[parameters.Length];
        int numberValidParameters=0;
        for(int i=0;i<parameters.Length;i++)
        {
          parameterValid[i] = IsValidParameterName(parameters[i]);
          ++numberValidParameters;
        }

        if(null!=View)
          View.SetNumberOfParameters(numberValidParameters,false);

        string[] validParameterNames = new string[numberValidParameters];
        for(int i=0,j=0;j<parameters.Length;j++)
        {
          if(parameterValid[j])
            validParameterNames[i++] = parameters[j];
        }

        m_TempScript.UserDefinedParameterNames = validParameterNames;

      }
      else // no user defined Parameters
      {
        System.Text.StringBuilder stb = new System.Text.StringBuilder();
        int i;
        int max = Math.Min(numberOfParameters,9); 
        for(i=0;i<max ;i++)
        {
          stb.Append(string.Format("P[{0}]",i));
          if((i+1)<max)
            stb.Append(',');
        }
        if(numberOfParameters>max)
        {
          stb.Append("..");
          stb.Append(string.Format("P[{0}]",numberOfParameters-1));
        }
        if(null!=View)
          View.SetParameterText(stb.ToString(),false);

        m_TempScript.NumberOfParameters = numberOfParameters;
      }

     
    }
    #region IMVCController Members

    public object ModelObject
    {
      get
      {
        return m_Script;
      }
    }

    #endregion

    #region IApplyController Members

    public bool Apply()
    {
      this.EhView_Compile();
      if(m_TempScript.Errors.Length>0)
        return false;
      m_Script = (IParametrizedFunctionDDScriptText)m_TempScript.Clone();
      return true;

    }

    #endregion
  }
}
