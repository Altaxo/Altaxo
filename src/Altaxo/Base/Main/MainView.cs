/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using Altaxo.Serialization;

namespace Altaxo
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainView : System.Windows.Forms.Form , IMainView
	{

		private static MainView sm_theApplication=null;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Holds the controller instance that controls this Form.
		/// </summary>
		private IMainController m_Ctrl; 
	
		public MainView()
			{
				sm_theApplication = this;
			
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// MainView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(544, 342);
			this.IsMdiContainer = true;
			this.Name = "MainView";
			this.Text = "Altaxo";

		}
		#endregion


		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_Closing(e);

			base.OnClosing(e);
		}

		protected override void OnClosed(System.EventArgs e)
		{
			if(null!=m_Ctrl)
				m_Ctrl.EhView_Closed(e);
		}


		/// <summary>
		/// Catch WndProc is neccessary here to capture the close event, since the MDI childs should not receive it.
		/// </summary>
		/// <param name="m">The windows message.</param>
		protected override void WndProc(ref System.Windows.Forms.Message m)
				 
		{
		const int SC_CLOSE = 0xF060;
		const int WM_SYSCOMMAND = 0x0112;
			const int WM_QUERYENDSESSION = 0x0011;
			const int WM_ENDSESSION      = 0x0016;

	
			// Test if the user clicked the closing button (X)
			if    ( m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_CLOSE && null!=m_Ctrl)
			{
				m_Ctrl.EhView_CloseMessage();
			}
				// Next, test for the PC shutting down.
			else if ( (m.Msg == WM_QUERYENDSESSION ||  m.Msg == WM_ENDSESSION) && null!=m_Ctrl)
			{
				m_Ctrl.EhView_CloseMessage();
			}

			// now handle the message
			base.WndProc(ref m);
		}


		#region IMainView Members

		/// <summary>
		/// Returns the Windows forms (i.e. in almost all cases - itself).
		/// </summary>
		public System.Windows.Forms.Form Form
		{
			get { return this; }
		}

		/// <summary>
		/// Sets the contoller for this view.
		/// </summary>
		public Altaxo.IMainController Controller 
		{
			set { m_Ctrl = value; }
		}


		/// <summary>
		/// Sets the main menu for the main window
		/// </summary>
		public System.Windows.Forms.MainMenu MainViewMenu
		{
			set { this.Menu = value; } // do not clone the menu here, since it is controlled by the controller
		}
		#endregion

		}
}
