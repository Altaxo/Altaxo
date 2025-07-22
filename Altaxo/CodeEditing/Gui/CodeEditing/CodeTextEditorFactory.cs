#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Altaxo.CodeEditing;
using Microsoft.CodeAnalysis;

namespace Altaxo.Gui.CodeEditing
{
  /// <summary>
  /// Helper class to create instances of <see cref="CodeEditorView"/>, <see cref="CodeEditor"/> or <see cref="CodeEditorWithDiagnostics"/>.
  /// </summary>
  public class CodeTextEditorFactory
  {
    /// <summary>
    /// Gets the roslyn host. (it is set either by a parameter in the constructor, or a new roslyn host is created for this instance).
    /// </summary>
    /// <value>
    /// The roslyn host.
    /// </value>
    public RoslynHost RoslynHost { get; }

    /// <summary>
    /// Gets the working directory.
    /// </summary>
    /// <value>
    /// The working directory.
    /// </value>
    public string WorkingDirectory { get; } = Environment.CurrentDirectory;

    protected FontFamily _defaultFont = new FontFamily("Consolas");

    protected double _defaultFontSize = 12;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeTextEditorFactory"/> class. A new <see cref="RoslynHost"/> is created and referenced in this instance.
    /// </summary>
    public CodeTextEditorFactory() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeTextEditorFactory"/> class.
    /// </summary>
    /// <param name="host">The roslyn host. If this argument is null, a new instance of <see cref="RoslynHost"/> is created and stored in this instance.</param>
    public CodeTextEditorFactory(RoslynHost host)
    {
      RoslynHost = host ?? new RoslynHost(null);
    }

    /// <summary>
    /// Gets or sets the default font for the code editor.
    /// </summary>
    /// <value>
    /// The default font for the code editor.
    /// </value>
    /// <exception cref="ArgumentNullException">value</exception>
    public FontFamily DefaultFont
    {
      get
      {
        return _defaultFont;
      }
      set
      {
        if (null == value)
          throw new ArgumentNullException(nameof(value));
        _defaultFont = value;
      }
    }

    /// <summary>
    /// Gets or sets the default size of the font for the code editor.
    /// </summary>
    /// <value>
    /// The default size of the font for the code editor.
    /// </value>
    /// <exception cref="ArgumentException">value out of range - value</exception>
    public double DefaultFontSize
    {
      get
      {
        return _defaultFontSize;
      }
      set
      {
        if (!(_defaultFontSize > 0 && _defaultFontSize < double.MaxValue))
          throw new ArgumentException("value out of range", nameof(value));
        _defaultFontSize = value;
      }
    }

    /// <summary>
    /// Gets a new instance of the code editor view. This is the lowest level code editor control (only code editor, no quick class browser, no error message window).
    /// </summary>
    /// <param name="initialText">The initial code text.</param>
    /// <param name="referencedAssemblies">The  assemblies referenced by the code.</param>
    /// <returns>New instance of a code editor view.</returns>
    public CodeEditorView NewCodeEditorView(string initialText, IEnumerable<System.Reflection.Assembly> referencedAssemblies)
    {
      var workspace = new AltaxoWorkspaceForCSharpRegularDll(RoslynHost, WorkingDirectory, referencedAssemblies);
      RoslynHost.RegisterWorkspace(workspace);
      return NewCodeEditorView(workspace, initialText);
    }

    /// <summary>
    /// Gets a new instance of the code editor view. This is the lowest level code editor control.
    /// </summary>
    /// <param name="workspace">The workspace (contains solution, project and referenced assemblies).</param>
    /// <param name="initialText">The initial code text.</param>
    /// <param name="fontFamily">The font family used for the code editor.</param>
    /// <param name="fontSize">Size of the font used for the code editor.</param>
    /// <returns>New instance of the code editor view.</returns>
    /// <exception cref="ArgumentNullException">workspace</exception>
    public CodeEditorView NewCodeEditorView(AltaxoWorkspaceBase workspace, string initialText, FontFamily fontFamily = null, double? fontSize = null)
    {
      if (null == workspace)
        throw new ArgumentNullException(nameof(workspace));

      var editor = new CodeEditorView();
      editor.Document.Text = initialText;

      editor.FontFamily = fontFamily ?? _defaultFont;
      editor.FontSize = fontSize ?? _defaultFontSize;

      // create the source text container that is connected with this editor
      var sourceTextContainer = new RoslynSourceTextContainerAdapter(editor.Document, editor);

      var document = workspace.CreateAndOpenDocument(sourceTextContainer, sourceTextContainer.UpdateText);

      editor.Adapter = new CodeEditorViewAdapterCSharp(workspace, document.Id, sourceTextContainer);
      editor.Document.UndoStack.ClearAll();

      return editor;
    }

    /// <summary>
    /// Gets a new instance of a code editor. This is the medium level code editor control (code editor, splittable, with quick class browser, but without error message window).
    /// </summary>
    /// <param name="initialText">The initial code text.</param>
    /// <param name="referencedAssemblies">The  assemblies referenced by the code.</param>
    /// <returns>New instance of a code editor.</returns>
    public CodeEditor NewCodeEditor(string initialText, IEnumerable<System.Reflection.Assembly> referencedAssemblies)
    {
      var workspace = new AltaxoWorkspaceForCSharpRegularDll(RoslynHost, WorkingDirectory, referencedAssemblies);
      RoslynHost.RegisterWorkspace(workspace);
      return NewCodeEditor(workspace, initialText);
    }

    /// <summary>
    /// Gets a new instance of the code editor. This is the medium level code editor control (code editor, splittable, with quick class browser, but without error message window).
    /// </summary>
    /// <param name="workspace">The workspace (contains solution, project and referenced assemblies).</param>
    /// <param name="initialText">The initial code text.</param>
    /// <param name="fontFamily">The font family used for the code editor.</param>
    /// <param name="fontSize">Size of the font used for the code editor.</param>
    /// <returns>New instance of the code editor view.</returns>
    /// <exception cref="ArgumentNullException">workspace</exception>
    public CodeEditor NewCodeEditor(AltaxoWorkspaceBase workspace, string initialText, FontFamily fontFamily = null, double? fontSize = null)
    {
      if (null == workspace)
        throw new ArgumentNullException(nameof(workspace));

      var codeEditor = new CodeEditor()
      {
        DocumentText = initialText
      };

      var editor = codeEditor.primaryTextEditor;
      editor.FontFamily = fontFamily ?? _defaultFont;
      editor.FontSize = fontSize ?? _defaultFontSize;

      // create the source text container that is connected with this editor
      var sourceTextContainer = new RoslynSourceTextContainerAdapter(codeEditor.Document, codeEditor);

      var document = workspace.CreateAndOpenDocument(sourceTextContainer, sourceTextContainer.UpdateText);

      codeEditor.Adapter = new CodeEditorViewAdapterCSharp(workspace, document.Id, sourceTextContainer);
      editor.Document.UndoStack.ClearAll();

      return codeEditor;
    }

    /// <summary>
    /// Gets a new instance of a code editor with diagnostics. This is the highest level code editor control (code editor, splittable, with quick class browser and error message window).
    /// </summary>
    /// <param name="initialText">The initial code text.</param>
    /// <param name="referencedAssemblies">The  assemblies referenced by the code.</param>
    /// <returns>New instance of a code editor with diagnostics.</returns>
    public CodeEditorWithDiagnostics NewCodeEditorWithDiagnostics(string initialText, IEnumerable<System.Reflection.Assembly> referencedAssemblies)
    {
      var workspace = new AltaxoWorkspaceForCSharpRegularDll(RoslynHost, WorkingDirectory, referencedAssemblies);
      RoslynHost.RegisterWorkspace(workspace);
      return NewCodeEditorWithDiagnostics(workspace, initialText);
    }

    /// <summary>
    /// Gets a new instance of the code editor with diagnostics. This is the highest level code editor control (code editor, splittable, with quick class browser and error message window).
    /// </summary>
    /// <param name="workspace">The workspace (contains solution, project and referenced assemblies).</param>
    /// <param name="initialText">The initial code text.</param>
    /// <param name="fontFamily">The font family used for the code editor.</param>
    /// <param name="fontSize">Size of the font used for the code editor.</param>
    /// <returns>New instance of the code editor with diagnostics.</returns>
    /// <exception cref="ArgumentNullException">workspace</exception>
    public CodeEditorWithDiagnostics NewCodeEditorWithDiagnostics(AltaxoWorkspaceBase workspace, string initialText, FontFamily fontFamily = null, double? fontSize = null)
    {
      if (null == workspace)
        throw new ArgumentNullException(nameof(workspace));

      var codeEditor = new CodeEditorWithDiagnostics()
      {
        DocumentText = initialText
      };

      var editor = codeEditor.primaryTextEditor;
      editor.FontFamily = fontFamily ?? _defaultFont;
      editor.FontSize = fontSize ?? _defaultFontSize;

      // create the source text container that is connected with this editor
      var sourceTextContainer = new RoslynSourceTextContainerAdapter(codeEditor.Document, codeEditor);
      var document = workspace.CreateAndOpenDocument(sourceTextContainer, sourceTextContainer.UpdateText);
      codeEditor.Adapter = new CodeEditorViewAdapterCSharp(workspace, document.Id, sourceTextContainer);

      editor.Document.UndoStack.ClearAll();

      return codeEditor;
    }

    /// <summary>
    /// Uninitializes the specified code editor with diagnostics and (if there is no other document open) removes the corresponding workspace from Roslyn.
    /// </summary>
    /// <param name="codeEditor">The code editor.</param>
    public void Uninitialize(CodeEditorWithDiagnostics codeEditor)
    {
      var adapter = codeEditor.Adapter;
      codeEditor.Adapter = null;
      if (null != adapter)
      {
        adapter.Workspace.CloseDocument(adapter.DocumentId);
        if (null == adapter.Workspace.GetOpenDocumentIds().FirstOrDefault()) // dispose workspace if it has no open documents
          adapter.Workspace.Dispose();
      }
    }

    /// <summary>
    /// Uninitializes the specified code editor and  (if there is no other document open)  removes the corresponding workspace from Roslyn.
    /// </summary>
    /// <param name="codeEditor">The code editor.</param>
    public void Uninitialize(CodeEditor codeEditor)
    {
      var adapter = codeEditor.Adapter;
      codeEditor.Adapter = null;
      if (null != adapter)
      {
        adapter.Workspace.CloseDocument(adapter.DocumentId);
        if (null == adapter.Workspace.GetOpenDocumentIds().FirstOrDefault()) // dispose workspace if it has no open documents
          adapter.Workspace.Dispose();
      }
    }

    /// <summary>
    /// Uninitializes the specified code editor view and (if there is no other document open) removes the corresponding workspace from Roslyn.
    /// </summary>
    /// <param name="codeEditor">The code editor.</param>
    public void Uninitialize(CodeEditorView codeEditor)
    {
      var adapter = codeEditor.Adapter;
      codeEditor.Adapter = null;
      if (null != adapter)
      {
        adapter.Workspace.CloseDocument(adapter.DocumentId);
        if (null == adapter.Workspace.GetOpenDocumentIds().FirstOrDefault()) // dispose workspace if it has no open documents
          adapter.Workspace.Dispose();
      }
    }
  }
}
