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

#nullable disable warnings
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Controls;
using Altaxo.CodeEditing.CompilationHandling;
using Altaxo.CodeEditing.ExternalHelp;
using Altaxo.Main.Services;
using Altaxo.Main.Services.ScriptCompilation;

namespace Altaxo.Gui.Scripting
{
  /// <summary>
  /// Interaction logic for SDPureScriptControlWpf.xaml
  /// </summary>
  [UserControlPriority(1)]
  public partial class SDPureScriptControlWpf : UserControl, IScriptView, IViewRequiresSpecialShellWindow
  {
    /// <summary>
    /// The factory used to create code editor instances.
    /// </summary>
    protected static CodeEditing.CodeTextEditorFactory _factory;
    private Altaxo.Gui.CodeEditing.CodeEditorWithDiagnostics _codeView;

    /// <summary>
    /// The application domain used for the help viewer.
    /// </summary>
    protected static AppDomain _helpViewerAppDomain;
    /// <summary>
    /// The starter used to launch the help viewer.
    /// </summary>
    protected static Altaxo.Gui.HelpViewing.HelpViewerStarter _helpViewerStarter;
    /// <summary>
    /// The main thread of the help viewer.
    /// </summary>
    protected static Thread _helpViewerMainThread;

    /// <summary>
    /// Not used here because this is handled by the view.
    /// </summary>
    /// <inheritdoc/>
    public event Action<string> CompilerMessageClicked
    {
      add { }
      remove { }
    }


    static SDPureScriptControlWpf()
    {
      _factory = new CodeEditing.CodeTextEditorFactory();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SDPureScriptControlWpf"/> class.
    /// </summary>
    public SDPureScriptControlWpf()
    {
      InitializeComponent();

      Unloaded += (s, e) => UninitializeEditor();
    }

    private static IEnumerable<Assembly> GetReferencedAssemblies()
    {
      return Altaxo.Settings.Scripting.ReferencedAssemblies.All;
    }

    private void InitializeEditor(string initialText, string scriptName)
    {
      _codeView = _factory.NewCodeEditorWithDiagnostics(initialText, GetReferencedAssemblies());
      _codeView.Name = "edFormula";
      _codeView.Adapter.ExternalHelpRequired += EhExternalHelpRequired;
      Content = _codeView;
    }

    private void UninitializeEditor()
    {
      if (_codeView?.Adapter is { } adapter)
        adapter.ExternalHelpRequired -= EhExternalHelpRequired;
      _factory?.Uninitialize(_codeView);
      _codeView = null;
      Content = null;
    }

    private static void EhExternalHelpRequired(ExternalHelpItem helpItem)
    {
      if (helpItem.GetOneOfTheseAssembliesOrNull(Altaxo.Settings.Scripting.ReferencedAssemblies.AssembliesIncludedInClassReference) is null)
      {
        ShowMicrosoftClassReferenceHelp(helpItem);
      }
      else
      {
        string chmFileName = FileUtility.ApplicationRootPath +
          Path.DirectorySeparatorChar + "doc" +
          Path.DirectorySeparatorChar + "help" +
          Path.DirectorySeparatorChar + StringParser.Parse("${AppName}ClassRef.chm");
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

    /// <summary>
    /// Shows Microsoft documentation for the requested help item.
    /// </summary>
    /// <param name="helpItem">The help item that identifies the requested documentation.</param>
    protected static void ShowMicrosoftClassReferenceHelp(ExternalHelpItem helpItem)
    {
      string url = "https://docs.microsoft.com/en-us/dotnet/api/";

      for (int i = 0; i < helpItem.TypeNameParts.Count; ++i)
      {
        url += helpItem.TypeNameParts[i];
        if (i < helpItem.TypeNameParts.Count - 1)
          url += ".";
      }

      if (helpItem.NumberOfGenericArguments != 0)
      {
        url += "-" + helpItem.NumberOfGenericArguments.ToString(System.Globalization.CultureInfo.InvariantCulture);
      }

      if (!(helpItem.MemberName is null))
      {
        url += ".";
        if (helpItem.MemberName == ".ctor")
          url += "-ctor";
        else
          url += helpItem.MemberName;
      }

      // Invoke standard browser of the system
      System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
    }

    /// <summary>
    /// Shows the Altaxo class reference for the requested help item on the web.
    /// </summary>
    /// <param name="helpItem">The help item that identifies the requested documentation.</param>
    protected static void ShowAltaxoClassRefHelpFromWeb(ExternalHelpItem helpItem)
    {
      string url = "https://altaxo.github.io/AltaxoClassReference/html/" + helpItem.DocumentationReferenceIdentifier + ".htm";

      // Invoke standard browser of the system
      System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url) { UseShellExecute = true });
    }

    /// <summary>
    /// Shows the Altaxo class reference help topic from a CHM help file.
    /// </summary>
    /// <param name="chmFileName">The path to the CHM help file.</param>
    /// <param name="chmTopic">The topic within the CHM help file.</param>
    protected static void ShowAltaxoClassRefHelpFromChmFile(string chmFileName, string chmTopic)
    {
      if (_helpViewerAppDomain is null)
      {
        _helpViewerAppDomain = AppDomain.CreateDomain("AltaxoHelpViewer");
      }
      if (_helpViewerStarter is null || _helpViewerMainThread is null || !_helpViewerMainThread.IsAlive)
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

    /// <inheritdoc/>
    public string ScriptText
    {
      get
      {
        return _codeView.DocumentText;
      }
      set
      {
        if (_codeView is null)
        {
          string scriptName = System.Guid.NewGuid().ToString() + ".cs";
          InitializeEditor(value, scriptName);
        }
        else if (_codeView.DocumentText != value)
        {
          _codeView.DocumentText = value;
        }
      }
    }

    /// <inheritdoc/>
    public int ScriptCursorLocation
    {
      set
      {
        _codeView.SetCaretOffsetWithScrolling(value);
      }
    }

    /// <inheritdoc/>
    public int InitialScriptCursorLocation
    {
      set
      {
        // do nothing here, because folding is active
      }
    }

    /// <inheritdoc/>
    public Type TypeOfShellWindowRequired => typeof(Altaxo.Gui.Scripting.ScriptExecutionDialog);

    /// <inheritdoc/>
    public void SetScriptCursorLocation(int line, int column)
    {
      _codeView.SetCaretOffsetWithScrolling(line, column);
    }

    /// <inheritdoc/>
    public void MarkText(int pos1, int pos2)
    {
      _codeView.MarkText(pos1, pos2);
    }

    #endregion IPureScriptView Members

    /// <summary>
    /// Converts the specified string content to a byte array.
    /// </summary>
    /// <param name="fileContent">The string content to convert.</param>
    /// <returns>The UTF-8 encoded byte array for the provided content.</returns>
    public static byte[] StringToByte(string fileContent)
    {
      var memoryStream = new MemoryStream();
      TextWriter tw = new StreamWriter(memoryStream);
      tw.Write(fileContent);
      tw.Flush();
      return memoryStream.ToArray();
    }

    /// <inheritdoc/>
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
