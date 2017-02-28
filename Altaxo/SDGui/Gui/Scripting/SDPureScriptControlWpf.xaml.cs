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

using Altaxo.CodeEditing.ExternalHelp;
using Altaxo.Main.Services.ScriptCompilation;
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Scripting
{
	/// <summary>
	/// Interaction logic for SDPureScriptControlWpf.xaml
	/// </summary>

	public partial class SDPureScriptControlWpf : UserControl, IScriptView
	{
		protected static CodeEditing.CodeTextEditorFactory _factory;

		private Window _parentForm;

		protected static Assembly[] _additionalReferencedAssemblies;

		private Altaxo.Gui.CodeEditing.CodeEditorWithDiagnostics _codeView;

		/// <summary>
		/// Not used here because this is handled by the view.
		/// </summary>
		public event Action<string> CompilerMessageClicked;

		static SDPureScriptControlWpf()
		{
			_factory = new CodeEditing.CodeTextEditorFactory();

			_additionalReferencedAssemblies = new Assembly[]
			{
				typeof(Altaxo.Calc.RMath).Assembly, // Core
				typeof(Altaxo.Data.DataTable).Assembly, // Base
				typeof(Altaxo.Gui.GuiHelper).Assembly, // Presentation
				typeof(SDPureScriptControlWpf).Assembly // SDGui
			};
		}

		public SDPureScriptControlWpf()
		{
			InitializeComponent();

			Unloaded += EhControl_Unloaded;
		}

		private void InitializeEditor(string initialText, string scriptName)
		{
			this._codeView = _factory.NewCodeEditorWithDiagnostics(initialText, _additionalReferencedAssemblies);
			this._codeView.IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(edFormula_IsVisibleChanged);
			this._codeView.Name = "edFormula";
			this._codeView.Adapter.ExternalHelpRequired += EhExternalHelpRequired;
			this.Content = _codeView;
		}

		private void UninitializeEditor()
		{
			_factory.Uninitialize(this._codeView);
			_codeView = null;
			this.Content = null;
		}

		private void EhControl_Unloaded(object sender, RoutedEventArgs e)
		{
			UninitializeEditor();
		}

		private void EhExternalHelpRequired(ExternalHelpItem helpItem)
		{
			if (null == helpItem.GetOneOfTheseAssembliesOrNull(_additionalReferencedAssemblies))
				return;

			string fileName = FileUtility.ApplicationRootPath +
				Path.DirectorySeparatorChar + "doc" +
				Path.DirectorySeparatorChar + "help" +
				Path.DirectorySeparatorChar + "AltaxoClassRef.chm";
			if (FileUtility.TestFileExists(fileName))
			{
				string topic = "html/" + helpItem.DocumentationReferenceIdentifier + ".htm";

				System.Windows.Forms.Help.ShowHelp(null, fileName, topic);
			}
		}

		#region IPureScriptView Members

		public string ScriptText
		{
			get
			{
				return this._codeView.DocumentText;
			}
			set
			{
				if (this._codeView == null)
				{
					string scriptName = System.Guid.NewGuid().ToString() + ".cs";
					InitializeEditor(value, scriptName);
				}
				else if (this._codeView.DocumentText != value)
				{
					this._codeView.DocumentText = value;
				}
			}
		}

		public int ScriptCursorLocation
		{
			set
			{
				_codeView.SetCaretOffsetWithScrolling(value);
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
			_codeView.SetCaretOffsetWithScrolling(line, column);
		}

		public void MarkText(int pos1, int pos2)
		{
			_codeView.MarkText(pos1, pos2);
		}

		#endregion IPureScriptView Members

		private void edFormula_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
		{
			if (_codeView.IsVisible)
			{
				if (null == _parentForm)
				{
					_parentForm = Window.GetWindow(this);
					_parentForm.Closing += _parentForm_Closing;
				}
			}
		}

		private void _parentForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			_parentForm.Closing -= _parentForm_Closing;
		}

		public static byte[] StringToByte(string fileContent)
		{
			MemoryStream memoryStream = new MemoryStream();
			TextWriter tw = new StreamWriter(memoryStream);
			tw.Write(fileContent);
			tw.Flush();
			return memoryStream.ToArray();
		}

		public IScriptCompilerResult Compile()
		{
			var result = _codeView.Compile(texts => new CodeTextsWithHash(texts).Hash, Altaxo.Settings.Scripting.ReferencedAssemblies.All);
			var scriptTextsWithHash = new CodeTextsWithHash(result.CodeText);

			if (result.CompiledAssembly != null)
			{
				return new ScriptCompilerSuccessfulResult(scriptTextsWithHash, result.CompiledAssembly);
			}
			else
			{
				return new ScriptCompilerFailedResult(scriptTextsWithHash,
					result.Diagnostics.Select(diag => new CompilerDiagnostic(diag.Line, diag.Column, (DiagnosticSeverity)diag.Severity, diag.MessageText)));
			}
		}

		public void SetCompilerErrors(IEnumerable<ICompilerDiagnostic> errors)
		{
		}
	}
}