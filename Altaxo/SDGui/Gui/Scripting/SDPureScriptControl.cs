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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;

using Altaxo.Gui;
using Altaxo.Gui.Scripting;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.AddIn;

namespace Altaxo.Gui.Scripting
{
	/// <summary>
	/// Summary description for PureScriptControl.
	/// </summary>
	//[UserControlForController(typeof(IPureScriptViewEventSink), 110)]
	public class SDPureScriptControl : System.Windows.Forms.UserControl, IPureScriptView
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Integration.ElementHost _wpfHost;

		AvalonEditViewContent _editViewContent;
		ICSharpCode.AvalonEdit.AddIn.CodeEditor _codeView;

		public SDPureScriptControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		void InitializeEditor(string initialText, string scriptName)
		{
			this.SuspendLayout();

			// The trick is here to create an untitled file, so that the binary content is used, 
			// but at the same time to give the file an unique name in order to get processed by the parser
			var openFile = FileService.CreateUntitledOpenedFile(scriptName, StringToByte(initialText));

			_editViewContent = new AvalonEditViewContent(openFile);
			this._codeView = _editViewContent.CodeEditor;

			this._codeView.IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(edFormula_IsVisibleChanged);
			this._codeView.Name = "edFormula";
			_wpfHost.Child = _codeView;

			//	Altaxo.Main.Services.ParserServiceConnector.RegisterScriptFileName(scriptName);
			//  _codeView.PrimaryTextEditor.Options.IndentationSize = 2;
			/*
			try
			{
				var strat = HighlightingManager.Instance.GetDefinition("C#");
				if (strat == null)
				{
					throw new Exception("Strategy can't be null");
				}
				_codeView.SyntaxHighlighting = strat;
				_codeView.PrimaryTextEditor.TextArea.IndentationStrategy = new ICSharpCode.AvalonEdit.Indentation.CSharp.CSharpIndentationStrategy(_codeView.PrimaryTextEditor.Options);
			}
			catch (HighlightingDefinitionInvalidException ex)
			{
			}
			*/


			this.ResumeLayout();
		}



		bool _registered;
		void Register()
		{
			if (!_registered)
			{
				_registered = true;
				ParserService.RegisterModalContent(_editViewContent);
			}

		}
		void Unregister()
		{

			ParserService.UnregisterModalContent();
			_registered = false;

		}




		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}

				if (_registered)
					Unregister();

				_editViewContent.Dispose();
				_editViewContent = null;

			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._wpfHost = new System.Windows.Forms.Integration.ElementHost();
			this.SuspendLayout();
			// 
			// _wpfHost
			// 
			this._wpfHost.Dock = System.Windows.Forms.DockStyle.Fill;
			this._wpfHost.Location = new System.Drawing.Point(0, 0);
			this._wpfHost.Name = "_wpfHost";
			this._wpfHost.Size = new System.Drawing.Size(300, 300);
			this._wpfHost.TabIndex = 0;
			this._wpfHost.Text = "elementHost1";
			this._wpfHost.Child = null;
			// 
			// SDPureScriptControl
			// 
			this.Controls.Add(this._wpfHost);
			this.Name = "SDPureScriptControl";
			this.Size = new System.Drawing.Size(300, 300);
			this.ResumeLayout(false);

		}
		#endregion

		#region IPureScriptView Members

		IPureScriptViewEventSink _controller;
		public IPureScriptViewEventSink Controller
		{
			get
			{
				return _controller;
			}
			set
			{
				_controller = value;
			}
		}

		public string ScriptText
		{
			get
			{
				return this._codeView.Document.Text;
			}
			set
			{
				if (this._codeView == null)
				{
					string scriptName = System.Guid.NewGuid().ToString() + ".cs";
					InitializeEditor(value, scriptName);
				}
				else if (this._codeView.Document.Text != value)
				{
					this._codeView.Document.Text = value;
				}
			}
		}

		public int ScriptCursorLocation
		{
			set
			{
				var location = _codeView.Document.GetLocation(value);
				_codeView.PrimaryTextEditor.TextArea.Caret.Location = location;

			}

		}

		public int InitialScriptCursorLocation
		{
			set
			{
				// do nothing here, because folding is active
			}

		}

		public void SetScriptCursorLocation(int line, int column)
		{
			_codeView.PrimaryTextEditor.TextArea.Caret.Location = new ICSharpCode.AvalonEdit.Document.TextLocation(line, column);
		}


		public void MarkText(int pos1, int pos2)
		{

		}




		#endregion

		Form _parentForm;
		void edFormula_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			if (_codeView.IsVisible)
			{
				if (null == _parentForm)
				{
					_parentForm = this.FindForm();
					_parentForm.Closing += new CancelEventHandler(_parentForm_Closing);

					Register();
				}
			}
		}

		private void _parentForm_Closing(object sender, CancelEventArgs e)
		{
			Unregister();
		}


		public static byte[] StringToByte(string fileContent)
		{
			MemoryStream memoryStream = new MemoryStream();
			TextWriter tw = new StreamWriter(memoryStream);
			tw.Write(fileContent);
			tw.Flush();
			return memoryStream.ToArray();
		}


	}
}
