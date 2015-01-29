#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

#endregion Copyright

using ICSharpCode.AvalonEdit.AddIn;
using ICSharpCode.SharpDevelop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Scripting
{
	/// <summary>
	/// Interaction logic for SDPureScriptControlWpf.xaml
	/// </summary>
	[UserControlForController(typeof(IPureScriptViewEventSink), 120)]
	public partial class SDPureScriptControlWpf : UserControl, IPureScriptView
	{
		private AvalonEditViewContent _editViewContent;
		private ICSharpCode.AvalonEdit.AddIn.CodeEditor _codeView;

		public SDPureScriptControlWpf()
		{
			InitializeComponent();
		}

		private void InitializeEditor(string initialText, string scriptName)
		{
			// The trick is here to create an untitled file, so that the binary content is used,
			// but at the same time to give the file an unique name in order to get processed by the parser
			var openFile = FileService.CreateUntitledOpenedFile(scriptName, StringToByte(initialText));

			_editViewContent = new AvalonEditViewContent(openFile);
			this._codeView = _editViewContent.CodeEditor;

			this._codeView.IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(edFormula_IsVisibleChanged);
			this._codeView.Name = "edFormula";
			this.Content = _codeView;
		}

		private bool _registered;

		private void Register()
		{
			if (!_registered)
			{
				_registered = true;
				ParserService.RegisterModalContent(_editViewContent);
			}
		}

		private void Unregister()
		{
			ParserService.UnregisterModalContent();
			_registered = false;
		}

		#region IPureScriptView Members

		private IPureScriptViewEventSink _controller;

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
				_codeView.PrimaryTextEditor.ScrollToLine(location.Line);
			}
		}

		public int InitialScriptCursorLocation
		{
			set
			{
				// do nothing here, because folding is active
			}
		}

		/// <summary>
		/// Sets the cursor location inside the script and focuses on the text. Line and column are starting with 1.
		/// </summary>
		/// <param name="line">Script line (1-based).</param>
		/// <param name="column">Script column (1-based).</param>
		public void SetScriptCursorLocation(int line, int column)
		{
			/* to mark the word that causes the error
			var offset = _codeView.Document.GetOffset(line, column);
			var textLine = _codeView.Document.GetLineByNumber(line);
			var lineStartOffset = textLine.Offset;
			var lineLength = textLine.Length;
			string stringAfterCursor = _codeView.Document.GetText(offset, lineLength - (offset - lineStartOffset));
			string stringBeforeCursor = _codeView.Document.GetText(lineStartOffset, (offset - lineStartOffset));
			// _codeView.PrimaryTextEditor.TextArea.Selection = new ICSharpCode.AvalonEdit.Editing.SimpleSelection();
			*/

			_codeView.PrimaryTextEditor.TextArea.Caret.Location = new ICSharpCode.AvalonEdit.Document.TextLocation(line, column);
			_codeView.PrimaryTextEditor.ScrollToLine(line);
			_codeView.PrimaryTextEditor.TextArea.Focus();
		}

		public void MarkText(int pos1, int pos2)
		{
			_codeView.PrimaryTextEditor.TextArea.Selection = ICSharpCode.AvalonEdit.Editing.Selection.Create(_codeView.PrimaryTextEditor.TextArea, pos1, pos2);
		}

		#endregion IPureScriptView Members

		private Window _parentForm;

		private void edFormula_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			if (_codeView.IsVisible)
			{
				if (null == _parentForm)
				{
					_parentForm = Window.GetWindow(this);
					_parentForm.Closing += _parentForm_Closing;

					Register();
				}
			}
		}

		private void _parentForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_parentForm.Closing -= _parentForm_Closing;
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