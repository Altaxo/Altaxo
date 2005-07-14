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
using Altaxo.Main.GUI;

namespace Altaxo.Gui.Scripting
{
  #region Interfaces
  public interface IParametrizedFunctionScriptView
  {
    IParametrizedFunctionScriptViewEventSink Controller {get; set; }
    
    void Close(bool withOK);
    void SetScriptView(object scriptView);

    void SetCheckUseUserDefinedParameters(bool useUserDefParameters);
    void SetParameterText(string text, bool enable);
    void SetIndependentVariableText(string text);
    void SetDependentVariableText(string text);
    void SetNumberOfParameters(int numberOfParameters, bool enable);
  }

  public interface IParametrizedFunctionScriptViewEventSink
  {
    void EhView_NumberOfParameterChanged(int numParameter);
    void EhView_UserDefinedParameterCheckChanged(bool userDefinedParameters);
    void EhView_UserDefinedParameterTextChanged(string parameterNames);
    void EhView_IndependentVariableTextChanged(string val);
    void EhView_DependentVariableTextChanged(string val);
  }


  #endregion

  /// <summary>
  /// Summary description for TableScriptController.
  /// </summary>
  [UserControllerForObject(typeof(IParametrizedFunctionDDScriptText),300)]
  public class ParametrizedFunctionScriptController : IParametrizedFunctionScriptViewEventSink, Main.GUI.IMVCAController
  {
    protected ScriptExecutionHandler m_ScriptExecution;
    public IParametrizedFunctionDDScriptText m_Script;
    public IParametrizedFunctionDDScriptText m_TempScript;

    public IScriptController _scriptController;

    protected IParametrizedFunctionScriptView m_View;

    public ParametrizedFunctionScriptController(IParametrizedFunctionDDScriptText script)
    {
      this.m_Script = script;
      m_TempScript = (IParametrizedFunctionDDScriptText)m_Script.CloneForModification();

      SetElements(true);

    }

  


    protected void SetElements(bool bInit)
    {
      if(bInit)
      {
        _scriptController = new ScriptController(m_TempScript);
        _scriptController.ViewObject = new ScriptControl();

        //View.ScriptName = m_TempScript.ScriptName;
        //_scriptController.ScriptCursorLocation = m_TempScript.UserAreaScriptOffset;
      }

      if(null!=View)
      {
        View.SetScriptView(_scriptController.ViewObject);
        View.SetCheckUseUserDefinedParameters(m_TempScript.IsUsingUserDefinedParameterNames);
        View.SetNumberOfParameters(m_TempScript.NumberOfParameters,!m_TempScript.IsUsingUserDefinedParameterNames);
        View.SetParameterText(this.GetParametersAsLine(),m_TempScript.IsUsingUserDefinedParameterNames);
        View.SetIndependentVariableText(this.GetIndependentVariablesAsLine());
        View.SetDependentVariableText(this.GetDependentVariablesAsLine());
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

   


    bool IsValidVariableName(string name)
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

    string GetParametersAsLine()
    {
      System.Text.StringBuilder stb = new System.Text.StringBuilder();
      int i;
      int numberOfParameters = m_TempScript.NumberOfParameters;
      bool userDef = m_TempScript.IsUsingUserDefinedParameterNames;

      if (userDef)
      {
        for (i = 0; i < numberOfParameters; ++i)
        {
          stb.Append(m_TempScript.ParameterName(i));
          if ((i + 1) < numberOfParameters)
            stb.Append(", ");
        }
      }
      else
      {
        int max = Math.Min(numberOfParameters, 9);
        for (i = 0; i < max; i++)
        {
          stb.Append(m_TempScript.ParameterName(i));

          if ((i + 1) < max)
            stb.Append(", ");
        }
        if (numberOfParameters > max)
        {
          stb.Append("..");
          stb.Append(m_TempScript.ParameterName(numberOfParameters - 1));
        }
      }
      return stb.ToString();
    }

    public string GetIndependentVariablesAsLine()
    {
       System.Text.StringBuilder stb = new System.Text.StringBuilder();
      for (int i = 0; i < m_TempScript.NumberOfIndependentVariables; ++i)
      {
        stb.Append(m_TempScript.IndependentVariableName(i));
        if ((i + 1) < m_TempScript.NumberOfIndependentVariables)
          stb.Append(", ");
      }
      return stb.ToString();
    }

    public string GetDependentVariablesAsLine()
    {
      System.Text.StringBuilder stb = new System.Text.StringBuilder();
      for (int i = 0; i < m_TempScript.NumberOfDependentVariables; ++i)
      {
        stb.Append(m_TempScript.DependentVariableName(i));
        if ((i + 1) < m_TempScript.NumberOfDependentVariables)
          stb.Append(", ");
      }
      return stb.ToString();
    }

    public void EhView_NumberOfParameterChanged(int numParameter)
    {
      _scriptController.Update();

      m_TempScript.NumberOfParameters = numParameter;
      if(View!=null)
        View.SetParameterText(GetParametersAsLine(),m_TempScript.IsUsingUserDefinedParameterNames);

      _scriptController.SetText(m_TempScript.ScriptText);
    }
    public void EhView_UserDefinedParameterCheckChanged(bool userDefinedParameters)
    {
      _scriptController.Update();

      m_TempScript.IsUsingUserDefinedParameterNames = userDefinedParameters;
      if (View != null)
        View.SetParameterText(GetParametersAsLine(),m_TempScript.IsUsingUserDefinedParameterNames);
      
      _scriptController.SetText(m_TempScript.ScriptText);
    }


    public void EhView_UserDefinedParameterTextChanged( string parameterNames)
    {
     

      if(m_TempScript.IsUsingUserDefinedParameterNames)
      {
        _scriptController.Update();

        // parse the parameter string
        string[] parameters = parameterNames.Split(new char[]{'.',',',';',':',' '});
        // test if all parameters start with a numeric character and have only
        // alphanumeric characters
        bool[] parameterValid = new bool[parameters.Length];
        int numberValidParameters=0;
        for(int i=0;i<parameters.Length;i++)
        {
          parameterValid[i] = IsValidVariableName(parameters[i]);
          if(parameterValid[i])
            ++numberValidParameters;
        }

        string[] validParameterNames = new string[numberValidParameters];
        for(int i=0,j=0;j<parameters.Length;j++)
        {
          if(parameterValid[j])
            validParameterNames[i++] = parameters[j];
        }

        m_TempScript.UserDefinedParameterNames = validParameterNames;
        if (null != View)
        {
          View.SetNumberOfParameters(numberValidParameters, false);
          View.SetParameterText(GetParametersAsLine(), m_TempScript.IsUsingUserDefinedParameterNames);
        }
        _scriptController.SetText(m_TempScript.ScriptText);
      }
    }

    public void EhView_IndependentVariableTextChanged(string val)
    {
      _scriptController.Update();


      // parse the parameter string
      string[] vars = val.Split(new char[]{'.',',',';',':',' '});
      // test if all parameters start with a numeric character and have only
      // alphanumeric characters
      bool[] valid = new bool[vars.Length];
      int numberValid=0;
      for(int i=0;i<vars.Length;i++)
      {
        valid[i] = IsValidVariableName(vars[i]);
        if(valid[i])
          ++numberValid;
      }

      string[] validNames = new string[numberValid];
      for(int i=0,j=0; j<vars.Length; j++)
      {
        if(valid[j])
          validNames[i++] = vars[j];
      }

      m_TempScript.IndependentVariablesNames = validNames;

      _scriptController.SetText(m_TempScript.ScriptText);
    }

    public void EhView_DependentVariableTextChanged(string val)
    {
      _scriptController.Update();


      // parse the parameter string
      string[] vars = val.Split(new char[]{'.',',',';',':',' '});
      // test if all parameters start with a numeric character and have only
      // alphanumeric characters
      bool[] valid = new bool[vars.Length];
      int numberValid=0;
      for(int i=0;i<vars.Length;i++)
      {
        valid[i] = IsValidVariableName(vars[i]);
        if(valid[i])
          ++numberValid;
      }

      string[] validNames = new string[numberValid];
      for(int i=0,j=0; j<vars.Length; j++)
      {
        if(valid[j])
          validNames[i++] = vars[j];
      }

      m_TempScript.DependentVariablesNames = validNames;

      _scriptController.SetText(m_TempScript.ScriptText);
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
      if(this._scriptController.Apply())
      {
       
        m_TempScript = (IParametrizedFunctionDDScriptText)_scriptController.ModelObject;
       
        m_Script = m_TempScript;
        m_TempScript = (IParametrizedFunctionDDScriptText)m_Script.CloneForModification();
        return true;
      }
      else
      {
        return false;
      }
    }

    #endregion
  }
}
