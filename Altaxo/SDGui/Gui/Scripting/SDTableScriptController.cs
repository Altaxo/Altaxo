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
using System.Text.RegularExpressions;
using Altaxo.Data;
using Altaxo.Gui.Scripting;
using Altaxo.Scripting;

namespace Altaxo.Gui.Scripting
{
  #region Interfaces
  public interface ITableScriptView
  {
    ITableScriptController Controller {get; set; }
    
    string ScriptText { get; set; }

    int ScriptCursorLocation { set; }
    void SetScriptCursorLocation(int line, int column);
    void MarkText(int pos1, int pos2);

    /// <summary>
    /// Sets the working name of the script. Should be set to a unique name
    /// ending in ".cs" to signal that this is a C# script.
    /// </summary>
    string ScriptName { set; }

    System.Windows.Forms.Form Form    { get ; }

    void ClearCompilerErrors();

    void AddCompilerError(string s);
  }

  public interface ITableScriptController
  {
    void EhView_Execute();
    void EhView_Compile();
    void EhView_Update();
    void EhView_Cancel();
    void EhView_GotoCompilerError(string message);
  }



 

  #endregion

  /// <summary>
  /// Summary description for TableScriptController.
  /// </summary>
  public class SDTableScriptController : ITableScriptController
  {
    protected Altaxo.Data.DataTable m_DataTable;
    protected ScriptExecutionHandler m_ScriptExecution;
    public IScriptText m_TableScript;

    protected ITableScriptView m_View;

    public SDTableScriptController(Altaxo.Data.DataTable dataTable, TableScript tableScript)
    {
      this.m_DataTable = dataTable;

      if(null!=tableScript)
      {
        this.m_TableScript = (IScriptText)tableScript.Clone();
      }
      else
      {
        this.m_TableScript = new TableScript();
      }

      SetElements(true);

    }

    public SDTableScriptController(ScriptExecutionHandler exec, IScriptText script)
    {
      this.m_ScriptExecution = exec;

      if(null!=script)
      {
        this.m_TableScript = (IScriptText)script.Clone();
      }
      else
      {
        throw new ArgumentNullException();
      }

      SetElements(true);

    }


    protected void SetElements(bool bInit)
    {
      if(bInit)
      {
      }

      if(null!=View)
      {
        View.ScriptText= m_TableScript.ScriptText;
        View.ScriptName = m_TableScript.ScriptName;
        View.ScriptCursorLocation = m_TableScript.UserAreaScriptOffset;
      }
    }

    public ITableScriptView View
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
      set { View = value as ITableScriptView; }
    }

    public void EhView_Execute()
    {
      if(m_TableScript.ScriptText != View.ScriptText)
      {
        m_TableScript = m_TableScript.CloneForModification();
      }
      m_TableScript.ScriptText = View.ScriptText;

      View.ClearCompilerErrors();

      bool bSucceeded = m_TableScript.Compile();

      if(!bSucceeded)
      {
        foreach(string s in m_TableScript.Errors)
          View.AddCompilerError(s);

        System.Windows.Forms.MessageBox.Show(View.Form, "There were compilation errors","No success");
        return;
      }
      else
      {
        View.AddCompilerError(DateTime.Now.ToLongTimeString() + " : Compilation successful.");
      }

      if(m_DataTable!=null)
        bSucceeded = ((TableScript)m_TableScript).ExecuteWithSuspendedNotifications(m_DataTable);
      else if(m_ScriptExecution!=null)
        bSucceeded = m_ScriptExecution(m_TableScript);

      if(!bSucceeded)
      {
        foreach(string s in m_TableScript.Errors)
          View.AddCompilerError(s);

        System.Windows.Forms.MessageBox.Show(View.Form, "There were execution errors","No success");
        return;
      }

      // this.m_DataTable.TableScript = m_TableScript;
      View.Form.DialogResult = System.Windows.Forms.DialogResult.OK;
      View.Form.Close();
    }


    private Regex compilerErrorRegex = new Regex(@".*\((?<line>\d+),(?<column>\d+)\) : (?<msg>.+)",RegexOptions.Compiled);
    public void EhView_Compile()
    {
      if(m_TableScript.ScriptText != View.ScriptText)
      {
        m_TableScript = m_TableScript.CloneForModification();
      }
      m_TableScript.ScriptText = View.ScriptText;


      View.ClearCompilerErrors();

      bool bSucceeded = m_TableScript.Compile();

      if(!bSucceeded)
      {
        foreach(string s in m_TableScript.Errors)
        {
          System.Text.RegularExpressions.Match match = compilerErrorRegex.Match(s);
          if(match.Success)
          {
            string news = match.Result("(${line},${column}) : ${msg}");
          
            View.AddCompilerError(news);
          }
          else
          {
            View.AddCompilerError(s);
          }
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
      if(m_TableScript.ScriptText != View.ScriptText)
      {
        m_TableScript = m_TableScript.CloneForModification();
      }
      m_TableScript.ScriptText = View.ScriptText;

      // this.m_DataTable.TableScript = m_TableScript;
      this.View.Form.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.View.Form.Close();
    }

    public void EhView_Cancel()
    {
      this.View.Form.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.View.Form.Close();
    }
  }
}
