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

using Altaxo.AddInItems;
using Altaxo.CodeEditing.CompilationHandling;
using Altaxo.CodeEditing.ExternalHelp;
using Altaxo.Main.Services;
using Altaxo.Main.Services.ScriptCompilation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Altaxo.Gui.Scripting
{
	/// <summary>
	/// Interaction logic for SDPureScriptControlWpf.xaml
	/// </summary>

	public partial class SDPureScriptControlWpf : UserControl, IScriptView, IViewRequiresSpecialShellWindow
	{
		protected static CodeEditing.CodeTextEditorFactory _factory;
		protected static Assembly[] _additionalReferencedAssemblies;
		private Altaxo.Gui.CodeEditing.CodeEditorWithDiagnostics _codeView;

		protected static AppDomain _helpViewerAppDomain;
		protected static Altaxo.Gui.HelpViewing.HelpViewerStarter _helpViewerStarter;
		protected static Thread _helpViewerMainThread;

		protected static bool _isFrameworkVersion47Installed; // If .NET Framework 4.7 is installed, we must not include System.ValueTuple in the list of referenced DLLs.

		/// <summary>
		/// Not used here because this is handled by the view.
		/// </summary>
		public event Action<string> CompilerMessageClicked;

		static SDPureScriptControlWpf()
		{
			_factory = new CodeEditing.CodeTextEditorFactory();

			var additionalReferencedAssemblies = new HashSet<Assembly>()
			{
				typeof(Altaxo.Calc.RMath).Assembly, // Core
				typeof(Altaxo.Data.DataTable).Assembly, // Base
				typeof(Altaxo.Gui.GuiHelper).Assembly, // Presentation
				typeof(SDPureScriptControlWpf).Assembly // SDGui
			};

			IList<string> additionalUserAssemblyNames = AddInTree.BuildItems<string>("/Altaxo/CodeEditing/AdditionalAssemblyReferences", null, false);

			foreach (var additionalUserAssemblyName in additionalUserAssemblyNames)
			{
				Assembly additionalAssembly = null;
				try
				{
					additionalAssembly = Assembly.Load(additionalUserAssemblyName);
					additionalReferencedAssemblies.Add(additionalAssembly);
				}
				catch (Exception ex)
				{
					Current.MessageService.ShowWarningFormatted("Assembly with name '{0}' that was given in {1} could not be loaded. Error: {2}", additionalUserAssemblyName, "/Altaxo/CodeEditing/AdditionalAssemblyReferences", ex.Message);
				}
			}

			_additionalReferencedAssemblies = additionalReferencedAssemblies.ToArray();

			_isFrameworkVersion47Installed = Altaxo.Serialization.AutoUpdates.NetFrameworkVersionDetermination.IsVersion47Installed();
		}

		public SDPureScriptControlWpf()
		{
			InitializeComponent();

			Unloaded += (s, e) => UninitializeEditor();
		}

		private static IEnumerable<Assembly> GetReferencedAssemblies()
		{
			var referencedAssemblies = Altaxo.Settings.Scripting.ReferencedAssemblies.All;
			if (_isFrameworkVersion47Installed)
			{
				referencedAssemblies = Altaxo.Settings.Scripting.ReferencedAssemblies.All.Where(ass => !(ass.GetName().Name.ToUpperInvariant().StartsWith("SYSTEM.VALUETUPLE")));
			}

			return referencedAssemblies;
		}

		private void InitializeEditor(string initialText, string scriptName)
		{
			this._codeView = _factory.NewCodeEditorWithDiagnostics(initialText, GetReferencedAssemblies());
			this._codeView.Name = "edFormula";
			this._codeView.Adapter.ExternalHelpRequired += EhExternalHelpRequired;
			this.Content = _codeView;
		}

		private void UninitializeEditor()
		{
			this._codeView.Adapter.ExternalHelpRequired -= EhExternalHelpRequired;
			_factory?.Uninitialize(this._codeView);
			_codeView = null;
			CompilerMessageClicked = null;
			this.Content = null;
		}

		private static void EhExternalHelpRequired(ExternalHelpItem helpItem)
		{
			if (null == helpItem.GetOneOfTheseAssembliesOrNull(_additionalReferencedAssemblies))
			{
				ShowMicrosoftClassReferenceHelp(helpItem);
			}
			else
			{
				string chmFileName = FileUtility.ApplicationRootPath +
					Path.DirectorySeparatorChar + "doc" +
					Path.DirectorySeparatorChar + "help" +
					Path.DirectorySeparatorChar + "AltaxoClassRef.chm";
				if (System.IO.File.Exists(chmFileName))
				{
					string topic = "html/" + helpItem.DocumentationReferenceIdentifier + ".htm";
					ShowAltaxoClassRefHelpFromChmFile(chmFileName, topic);
				}
				else
				{
					ShowAltaxoClassRefHelpFromWeb(helpItem);
				}
			}
		}

		protected static void ShowMicrosoftClassReferenceHelp(ExternalHelpItem helpItem)
		{
			string url = "https://msdn.microsoft.com/library/";

			for (int i = 0; i < helpItem.NameParts.Count; ++i)
			{
				url += helpItem.NameParts[i];
				if (i < helpItem.NameParts.Count - 1)
					url += ".";
			}

			// Invoke standard browser of the system
			System.Diagnostics.Process.Start(url);
		}

		protected static void ShowAltaxoClassRefHelpFromWeb(ExternalHelpItem helpItem)
		{
			string url = "http://altaxo.sourceforge.net/AltaxoClassRef/html/" + helpItem.DocumentationReferenceIdentifier + ".htm";

			// Invoke standard browser of the system
			System.Diagnostics.Process.Start(url);
		}

		protected static void ShowAltaxoClassRefHelpFromChmFile(string chmFileName, string chmTopic)
		{
			if (null == _helpViewerAppDomain)
			{
				_helpViewerAppDomain = AppDomain.CreateDomain("AltaxoHelpViewer");
			}
			if (null == _helpViewerStarter || null == _helpViewerMainThread || !_helpViewerMainThread.IsAlive)
			{
				_helpViewerStarter = (Altaxo.Gui.HelpViewing.HelpViewerStarter)_helpViewerAppDomain.CreateInstanceAndUnwrap("AltaxoHelpViewer", typeof(Altaxo.Gui.HelpViewing.HelpViewerStarter).FullName);
				_helpViewerMainThread = new Thread(_helpViewerStarter.Start);
				_helpViewerMainThread.SetApartmentState(ApartmentState.STA); // required to show a hidden Windows Forms
				_helpViewerMainThread.IsBackground = true; // we want HelpViewer to be ended if Altaxo ends
				_helpViewerMainThread.Start();

				// wait until the Helper App is loaded
				bool isLoaded = false;
				while (!isLoaded)
				{
					_helpViewerStarter.GetState(out var isDisp, out isLoaded);
					Thread.Sleep(50);
				}
			}

			// show the help topic
			_helpViewerStarter.ShowHelpTopic(chmFileName, chmTopic);
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

		public Type TypeOfShellWindowRequired => typeof(Altaxo.Gui.Scripting.ScriptExecutionDialog);

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
			var result = _codeView.Compile(texts => new CodeTextsWithHash(texts).Hash, GetReferencedAssemblies());
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
			var arr = ImmutableArray.Create<AltaxoDiagnostic>();
			arr = arr.AddRange(ConvertToAltaxoDiagnostic(errors));
			_codeView.SetDiagnosticMessages(arr);
		}

		private static IEnumerable<AltaxoDiagnostic> ConvertToAltaxoDiagnostic(IEnumerable<ICompilerDiagnostic> diagnostics)
		{
			foreach (var d in diagnostics ?? Enumerable.Empty<ICompilerDiagnostic>())
			{
				yield return new AltaxoDiagnostic(d.Line, d.Column, null, (int)d.Severity, d.SeverityText, d.MessageText);
			}
		}
	}
}