using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace Altaxo.Main
{
	#region Interfaces

	public interface ISingleValueFormView
	{
		/// <summary>
		/// Returns either the view itself if the view is a form, or the form where this view is contained into, if it is a control or so.
		/// </summary>
		System.Windows.Forms.Form Form { get; }
	
		string EditBoxContents { get; set; }

		/// <summary>
		/// Get / sets the controler of this view.
		/// </summary>
		ISingleValueFormController Controller { get; set; }
	}

	public interface ISingleValueFormController
	{
		void EhView_EditBoxValidating(System.ComponentModel.CancelEventArgs e);
	}

	#endregion

	#region Controller classes

	public class IntegerValueInputController : ISingleValueFormController
	{
		ISingleValueFormView m_View;


		IntegerValueInputController()
		{
		}

		ISingleValueFormView View
		{
			get { return m_View; }
			set
			{
				m_View = value;
				m_View.Controller = this;
			}
		}

		#region ISingleValueFormController Members

		public void EhView_EditBoxValidating(CancelEventArgs e)
		{
			int num;
			try
			{
				num = System.Convert.ToInt32(m_View.EditBoxContents);
				e.Cancel = false;
			}
			catch(Exception )
			{
				e.Cancel = true;
			}
		}
		#endregion
	} // end of class IntegerValueInputController





	public class TextValueInputController : ISingleValueFormController
	{
		ISingleValueFormView m_View;
		string m_InitialContents;
		string m_Contents;

		public delegate string TextValidatingHandler(string inputtext);

		public TextValidatingHandler m_ValidatingHandler;


		public TextValueInputController(string initialcontents,ISingleValueFormView view)
		{
			m_InitialContents = initialcontents;
			View = view;
		}


		ISingleValueFormView View
		{
			get { return m_View; }
			set
			{
				m_View = value;
				m_View.Controller = this;

				m_View.EditBoxContents = m_InitialContents;
			}
		}

		public string InputText
		{
			get { return m_Contents; }
		}

		public TextValidatingHandler ValidatingHandler
		{
			set { m_ValidatingHandler = value; }
		}

		public bool ShowDialog(System.Windows.Forms.Form owner)
		{
			return System.Windows.Forms.DialogResult.OK==m_View.Form.ShowDialog(owner);
		}


		#region ISingleValueFormController Members

		public void EhView_EditBoxValidating(CancelEventArgs e)
		{
			m_Contents = m_View.EditBoxContents;
			if(m_ValidatingHandler!=null)
			{
			
				string err = m_ValidatingHandler(m_Contents);
				if(null!=err)
				{
					e.Cancel = true;
					System.Windows.Forms.MessageBox.Show(this.View.Form,err,"Attention");
				}
			}
			else // if no validating handler, use some default validation
			{
				if(null==m_Contents || 0==m_Contents.Length)
				{
					e.Cancel = true;
				}
			}
		}

		#endregion
	} // end of class TextValueInputController
		
	#endregion
}
