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

#if false

using System;
using Altaxo.Data;

namespace Altaxo.Worksheet.GUI
{
#region Interfaces
  public interface IColumnScriptView
  {
    IColumnScriptController Controller {get;  set; }
  
    void EnableRowFrom(bool bEnab);
    void EnableRowCondition(bool bEnab);
    void EnableRowTo(bool bEnab);
    void EnableRowInc(bool bEnab);

    string RowFromText  { set; }
    string RowConditionText { set; }
    
    string RowToText { set; }
    
    string RowIncText { set ; }
    
    string FormulaText  { get; set; }
    
    string CodeHeadText { set ; }
      
    string CodeStartText  { set; }
    
    string CodeTailText { set ; }
    

    System.Windows.Forms.Form Form    { get ; }
    

    void ClearCompilerErrors();

    void AddCompilerError(string s);

    void InitializeScriptStyle(Altaxo.Data.ColumnScript.ScriptStyle style);
  }

  public interface IColumnScriptController
  {
    
  
    void EhView_ScriptStyleChanged(Altaxo.Data.ColumnScript.ScriptStyle style);
    void EhView_TextChanged_RowFrom(string text);

    void EhView_TextChanged_RowTo(string text);

    void EhView_TextChanged_RowInc(string text);

    void EhView_TextChanged_RowCondition(string text);

    void EhView_Execute();
    void EhView_Compile();
    void EhView_Update();
    void EhView_Cancel();

  }
#endregion

  /// <summary>
  /// Summary description for ColumnScriptController.
  /// </summary>
  public class ColumnScriptController : IColumnScriptController
  {
    private Altaxo.Data.DataTable m_DataTable;
    private Altaxo.Data.DataColumn m_DataColumn;
    public ColumnScript m_ColumnScript;

    private IColumnScriptView m_View;

    public ColumnScriptController(Altaxo.Data.DataTable dataTable, Altaxo.Data.DataColumn dataColumn, ColumnScript columnScript)
    {
      this.m_DataTable = dataTable;
      this.m_DataColumn = dataColumn;



      if(null!=columnScript)
      {
        this.m_ColumnScript = (ColumnScript)columnScript.Clone();
      }
      else
      {
        this.m_ColumnScript = new ColumnScript();
      }



      if(null==m_ColumnScript.ForFrom)
        m_ColumnScript.ForFrom = "0";
      if(null==m_ColumnScript.ForCondition)
        m_ColumnScript.ForCondition = "<";
      if(null==m_ColumnScript.ForEnd)
        m_ColumnScript.ForEnd = "col.RowCount";
      if(null==m_ColumnScript.ForInc)
        m_ColumnScript.ForInc = "++";
      if(null==m_ColumnScript.ScriptBody)
        m_ColumnScript.ScriptBody="";

  

      SetElements(true);

    }


    protected void SetElements(bool bInit)
    {
      if(bInit)
      {
      }

      if(null!=View)
      {

        View.InitializeScriptStyle(m_ColumnScript.Style);

        View.EnableRowFrom (  m_ColumnScript.Style==ColumnScript.ScriptStyle.SetColumnValues );
        View.EnableRowTo   ( m_ColumnScript.Style==ColumnScript.ScriptStyle.SetColumnValues  );
        View.EnableRowCondition (  m_ColumnScript.Style==ColumnScript.ScriptStyle.SetColumnValues );
        View.EnableRowInc  ( m_ColumnScript.Style==ColumnScript.ScriptStyle.SetColumnValues );

        View.RowFromText = m_ColumnScript.ForFrom;
        View.RowConditionText = m_ColumnScript.ForCondition;
        View.RowToText = m_ColumnScript.ForEnd;
        View.RowIncText = m_ColumnScript.ForInc;
        View.FormulaText=m_ColumnScript.ScriptBody;

        SetCodeParts();
      }
    }

    private void SetCodeParts()
    {
      View.CodeHeadText = m_ColumnScript.CodeHeader;
      View.CodeStartText= m_ColumnScript.CodeStart;
      View.CodeTailText = m_ColumnScript.CodeTail;
    }


    public IColumnScriptView View
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
      set { View = value as IColumnScriptView; }
    }

  

    public void EhView_ScriptStyleChanged(Altaxo.Data.ColumnScript.ScriptStyle style)
    {
      m_ColumnScript.Style = style;

      if(null!=View)
      {
        View.EnableRowFrom (  m_ColumnScript.Style==ColumnScript.ScriptStyle.SetColumnValues );
        View.EnableRowTo   ( m_ColumnScript.Style==ColumnScript.ScriptStyle.SetColumnValues  );
        View.EnableRowCondition (  m_ColumnScript.Style==ColumnScript.ScriptStyle.SetColumnValues );
        View.EnableRowInc  ( m_ColumnScript.Style==ColumnScript.ScriptStyle.SetColumnValues );

        SetCodeParts();
      }
    }

    public void EhView_TextChanged_RowFrom(string text)
    {
      m_ColumnScript.ForFrom = text;
      SetCodeParts();
    }

    public void EhView_TextChanged_RowTo(string text)
    {
      m_ColumnScript.ForEnd = text;
      SetCodeParts();
    }

    public void EhView_TextChanged_RowInc(string text)
    {
      m_ColumnScript.ForInc = text;
      SetCodeParts();
    }

    public void EhView_TextChanged_RowCondition(string text)
    {
      m_ColumnScript.ForCondition = text;
      SetCodeParts();
    }

    public void EhView_Execute()
    {
      m_ColumnScript.ScriptBody = View.FormulaText;

      View.ClearCompilerErrors();

      bool bSucceeded = m_ColumnScript.Compile();

      if(!bSucceeded)
      {
        foreach(string s in m_ColumnScript.Errors)
          View.AddCompilerError(s);

        System.Windows.Forms.MessageBox.Show(View.Form, "There were compilation errors","No success");
        return;
      }

      bSucceeded = m_ColumnScript.ExecuteWithSuspendedNotifications(m_DataColumn);
      if(!bSucceeded)
      {
        foreach(string s in m_ColumnScript.Errors)
          View.AddCompilerError(s);

        System.Windows.Forms.MessageBox.Show(View.Form, "There were execution errors","No success");
        return;
      }

      m_DataTable.DataColumns.ColumnScripts[m_DataColumn] = m_ColumnScript;
      View.Form.DialogResult = System.Windows.Forms.DialogResult.OK;
      View.Form.Close();
    }

    public void EhView_Compile()
    {
      m_ColumnScript.ScriptBody = View.FormulaText;

      View.ClearCompilerErrors();

      bool bSucceeded = m_ColumnScript.Compile();

      if(!bSucceeded)
      {
        foreach(string s in m_ColumnScript.Errors)
          View.AddCompilerError(s);

        System.Windows.Forms.MessageBox.Show(View.Form, "There were compilation errors","No success");
        return;
      }

    }

    public void EhView_Update()
    {
      m_ColumnScript.ScriptBody = View.FormulaText;
      m_DataTable.DataColumns.ColumnScripts[m_DataColumn] = m_ColumnScript;
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

#endif