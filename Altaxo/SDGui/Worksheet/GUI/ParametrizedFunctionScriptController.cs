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

namespace Altaxo.Worksheet.GUI
{
  #region Interfaces
  public interface IParametrizedFunctionScriptView
  {
    IParametrizedFunctionScriptViewEventSink Controller {get; set; }
    
    void Close(bool withOK);
    void SetScriptView(object scriptView);

    void SetParameterText(string text, bool enable);
    void SetNumberOfParameters(int numberOfParameters, bool enable);
  }

  public interface IParametrizedFunctionScriptViewEventSink
  {
    void EhView_ParameterChanged(int numberOfParameters, bool userDefinedParameters, string parameterNames);
  }


  #endregion

  /// <summary>
  /// Summary description for TableScriptController.
  /// </summary>
  [UserControllerForObject(typeof(IParametrizedFunctionDDScriptText))]
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
      if(m_Script!=null)
        m_TempScript = (IParametrizedFunctionDDScriptText)m_Script.Clone();

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
      if(this._scriptController.Apply())
      {

        m_Script = (IParametrizedFunctionDDScriptText)m_TempScript.Clone();
        return true;
      }
      else
        return false;
    }

    #endregion
  }
}
