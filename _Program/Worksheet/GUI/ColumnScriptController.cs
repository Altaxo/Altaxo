using System;
using Altaxo.Data;

namespace Altaxo.Worksheet.GUI
{
	#region Interfaces
	public interface IColumnScriptView
	{
		IColumnScriptController Controller {get;	set; }
	
		void EnableRowFrom(bool bEnab);
		void EnableRowCondition(bool bEnab);
		void EnableRowTo(bool bEnab);
		void EnableRowInc(bool bEnab);

		string RowFromText	{	set; }
		string RowConditionText	{	set; }
		
		string RowToText { set; }
		
		string RowIncText	{	set ; }
		
		string FormulaText	{	get; set; }
		
		string CodeHeadText	{	set ; }
			
		string CodeStartText	{	set; }
		
		string CodeTailText	{	set ; }
		

		System.Windows.Forms.Form Form		{	get ; }
		

		void ClearCompilerErrors();

		void AddCompilerError(string s);

		void InitializeScriptStyle(Altaxo.Data.ColumnScript.ScriptStyle style);
	}

	public interface IColumnScriptController
	{
		
	
		void EhView_DoIt_Click();

		void EhView_ScriptStyleChanged(Altaxo.Data.ColumnScript.ScriptStyle style);
		void EhView_TextChanged_RowFrom(string text);

		void EhView_TextChanged_RowTo(string text);

		void EhView_TextChanged_RowInc(string text);

		void EhView_TextChanged_RowCondition(string text);
	}
	#endregion

	/// <summary>
	/// Summary description for ColumnScriptController.
	/// </summary>
	public class ColumnScriptController : IColumnScriptController
	{
		private Altaxo.Data.DataTable dataTable;
		private Altaxo.Data.DataColumn dataColumn;
		public ColumnScript columnScript;

		private IColumnScriptView m_View;

		public ColumnScriptController(Altaxo.Data.DataTable dataTable, Altaxo.Data.DataColumn dataColumn, ColumnScript _columnScript)
		{
			this.dataTable = dataTable;
			this.dataColumn = dataColumn;



			if(null!=_columnScript)
			{
				this.columnScript = (ColumnScript)_columnScript.Clone();
			}
			else
			{
				this.columnScript = new ColumnScript();
			}



			if(null==columnScript.ForFrom)
				columnScript.ForFrom = "0";
			if(null==columnScript.ForCondition)
				columnScript.ForCondition = "<";
			if(null==columnScript.ForEnd)
				columnScript.ForEnd = "col.RowCount";
			if(null==columnScript.ForInc)
				columnScript.ForInc = "++";
			if(null==columnScript.ScriptBody)
				columnScript.ScriptBody="";

	

			SetElements(true);

		}


		protected void SetElements(bool bInit)
		{
			if(bInit)
			{
			}

			if(null!=View)
			{

				View.InitializeScriptStyle(columnScript.Style);

				View.EnableRowFrom (  columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues );
				View.EnableRowTo   ( columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues  );
				View.EnableRowCondition (  columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues );
				View.EnableRowInc  ( columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues );

				View.RowFromText = columnScript.ForFrom;
				View.RowConditionText = columnScript.ForCondition;
				View.RowToText = columnScript.ForEnd;
				View.RowIncText = columnScript.ForInc;
				View.FormulaText=columnScript.ScriptBody;

				SetCodeParts();
			}
		}

		private void SetCodeParts()
		{
			View.CodeHeadText = columnScript.CodeHeader;
			View.CodeStartText= columnScript.CodeStart;
			View.CodeTailText = columnScript.CodeTail;
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

		public void EhView_DoIt_Click()
		{
			columnScript.ScriptBody = View.FormulaText;

			View.ClearCompilerErrors();

			bool bSucceeded = columnScript.Compile();

			if(!bSucceeded)
			{
				foreach(string s in columnScript.Errors)
					View.AddCompilerError(s);

				System.Windows.Forms.MessageBox.Show(View.Form, "There were compilation errors","No success");
				return;
			}

			bSucceeded = columnScript.ExecuteWithSuspendedNotifications(dataColumn);
			if(!bSucceeded)
			{
				foreach(string s in columnScript.Errors)
					View.AddCompilerError(s);

				System.Windows.Forms.MessageBox.Show(View.Form, "There were execution errors","No success");
				return;
			}

			View.Form.DialogResult = System.Windows.Forms.DialogResult.OK;
			View.Form.Close();
		}


		public void EhView_ScriptStyleChanged(Altaxo.Data.ColumnScript.ScriptStyle style)
		{
			columnScript.Style = style;

			if(null!=View)
			{
				View.EnableRowFrom (  columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues );
				View.EnableRowTo   ( columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues  );
				View.EnableRowCondition (  columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues );
				View.EnableRowInc  ( columnScript.Style==ColumnScript.ScriptStyle.SetColumnValues );

				SetCodeParts();
			}
		}

		public void EhView_TextChanged_RowFrom(string text)
		{
			columnScript.ForFrom = text;
			SetCodeParts();
		}

		public void EhView_TextChanged_RowTo(string text)
		{
			columnScript.ForEnd = text;
			SetCodeParts();
		}

		public void EhView_TextChanged_RowInc(string text)
		{
			columnScript.ForInc = text;
			SetCodeParts();
		}

		public void EhView_TextChanged_RowCondition(string text)
		{
			columnScript.ForCondition = text;
			SetCodeParts();
		}
	}
}
