// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

// Originated from: SharpDevelop, AvalonEdit.Addin, Src/CodeEditorView.cs

// Modifications (C) Dr. D. Lellinger

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Altaxo.CodeEditing;
using Altaxo.Gui.CodeEditing.TextMarkerHandling;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using Microsoft.CodeAnalysis;

#if !NoBraceMatching
using Altaxo.Gui.CodeEditing.BraceMatching;
#endif

#if !NoCompletion
using Altaxo.CodeEditing.Completion;
#endif

#if !NoDiagnostics
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
#endif

#if !NoReferenceHighlighting
using Microsoft.CodeAnalysis.DocumentHighlighting;
#endif


/// <summary>
///
/// </summary>
namespace Altaxo.Gui.CodeEditing
{
  public delegate void ToolTipRequestEventHandler(object sender, ToolTipRequestEventArgs args);

  public class CodeEditorView : TextEditor, ICaretOffsetProvider
  {
    private readonly SynchronizationContext _syncContext;

    /// <summary>
    /// Gui component that shows the completion items.
    /// </summary>
    private CustomCompletionWindow _completionWindow;

    /// <summary>
    /// Gui component, the insight window, that shows the arguments of function calls.
    /// </summary>
    private OverloadInsightWindow _insightWindow;

    /// <summary>
    /// Gui component responsible for showing and managing the foldings in the text editor.
    /// </summary>
    protected FoldingManager _foldingManager;

#if !NoBraceMatching
    /// <summary>Gui component that highlights pairs of matching braces or other pairing items.</summary>
    private BracketHighlightRenderer _bracketHighlightRenderer;
#endif

    /// <summary>
    /// Gui component responsible for wriggles under the text that show pre-diagnostics.
    /// </summary>
    private readonly TextMarkerService _textMarkerService;

    public ICodeEditorViewAdapter _adapter;

    /// <summary>
    /// Gets or sets the adapter. The adapter is responsible for all the high level functions that make out the code editor,
    /// like completion, insight in function arguments, folding etc.
    /// </summary>
    /// <value>
    /// The adapter.
    /// </value>
    public ICodeEditorViewAdapter Adapter
    {
      get
      {
        return _adapter;
      }
      set
      {
        if (null != _adapter)
        {
#if !NoSemanticHighlighting
          TextArea.TextView.LineTransformers.Remove(_adapter.HighlightingColorizer);
#endif
          TextArea.IndentationStrategy = null;

#if !NoDiagnostics
          _adapter.DiagnosticsUpdated -= EhDiagnosticsUpdated;
#endif

#if !NoFolding
          _adapter.SyntaxTreeChanged -= EhSyntaxTreeChanged_UpdateFoldings;
#endif
        }
        _adapter = value;

        if (null != _adapter)
        {
#if !NoSemanticHighlighting
          TextArea.TextView.LineTransformers.Insert(0, _adapter.HighlightingColorizer);
#endif

          TextArea.IndentationStrategy = _adapter.IndentationStrategy;

#if !NoDiagnostics
          _adapter.DiagnosticsUpdated += EhDiagnosticsUpdated;
#endif

#if !NoFolding
          _adapter.SyntaxTreeChanged += EhSyntaxTreeChanged_UpdateFoldings;
#endif

          // now use the adapter to do all the little things

#if !NoFolding
          // foldings
          var newFoldings = _adapter?.GetNewFoldings();
          _foldingManager.UpdateFoldings(newFoldings, -1);
#endif
        }
      }
    }

    public CodeEditorView() : this(null)
    {
    }

    public CodeEditorView(TextEditorOptions options)
    {
      _syncContext = SynchronizationContext.Current;
      Options = options ?? new TextEditorOptions
      {
        ConvertTabsToSpaces = true,
        AllowScrollBelowDocument = true,
        IndentationSize = 4,
        EnableEmailHyperlinks = false,
      };

      ShowLineNumbers = true;

      MouseHover += OnMouseHover;
      MouseHoverStopped += OnMouseHoverStopped;
      TextArea.TextView.VisualLinesChanged += OnVisualLinesChanged;

#if !NoCompletion
      TextArea.TextEntering += OnTextEntering;
#endif

      TextArea.TextEntered += OnTextEntered;

      TextArea.MouseWheel += OnTextArea_MouseWheel;

      ToolTipService.SetInitialShowDelay(this, 0);
      // SearchReplacePanel.Install(this);

      var commandBindings = TextArea.CommandBindings;
      var deleteLineCommand = commandBindings.OfType<CommandBinding>().FirstOrDefault(x => x.Command == AvalonEditCommands.DeleteLine);
      if (deleteLineCommand != null)
      {
        commandBindings.Remove(deleteLineCommand);
      }

      _foldingManager = FoldingManager.Install(TextArea);

#if !NoQuickInfo
      AsyncToolTipRequest = AsyncToolTipRequestDefaultImpl;
#endif

#if !NoBraceMatching
      {
        // responsible for rendering brace matches
        _bracketHighlightRenderer = new BracketHighlightRenderer(TextArea.TextView);
        TextArea.Caret.PositionChanged += HighlightBrackets;
      }
#endif

      _textMarkerService = new TextMarkerService(this);
      // _errorMargin = new ErrorMargin { Visibility = Visibility.Collapsed, MarkerBrush = TryFindResource("ExceptionMarker") as Brush, Width = 10 };
      TextArea.TextView.BackgroundRenderers.Add(_textMarkerService);
      TextArea.TextView.LineTransformers.Add(_textMarkerService);
      //this.TextArea.LeftMargins.Insert(0, _errorMargin);
      //this.PreviewMouseWheel += EditorOnPreviewMouseWheel;
      //this.TextArea.Caret.PositionChanged += CaretOnPositionChanged;
#if !NoReferenceHighlighting
      ReferencesHighlightRenderer_Initialize();
#endif

      BuildTextAreaContextMenu();
    }

    /// <summary>
    /// Builds the context menu for the text area.
    /// </summary>
    /// <returns>The context menu for the text area (can be used to chain the building).</returns>
    protected virtual ContextMenu BuildTextAreaContextMenu()
    {
      var contextMenu = TextArea.ContextMenu ?? (TextArea.ContextMenu = new ContextMenu());

      MenuItem menuItem;
      menuItem = new MenuItem { Header = "Format all" };
      menuItem.Click += EhFormatCodeTextAll;

      contextMenu.Items.Add(menuItem);

      return contextMenu;
    }

    private void OnTextArea_MouseWheel(object sender, MouseWheelEventArgs e)
    {
      if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
      {
        TextArea.FontSize *= Math.Exp(0.0002 * e.Delta);
        e.Handled = true;
      }
    }

    private async void EhFormatCodeTextAll(object sender, RoutedEventArgs e)
    {
      var adapter = _adapter;
      if (null != adapter)
      {
        await adapter.FormatDocument();
      }
    }

    public virtual void Dispose()
    {
    }

    #region Code Completion

    public static readonly DependencyProperty CompletionBackgroundProperty = DependencyProperty.Register(
      nameof(CompletionBackground),
      typeof(Brush),
      typeof(CodeEditorView),
      new FrameworkPropertyMetadata(CreateDefaultCompletionBackground()));

    private ToolTip _toolTip;

    private static SolidColorBrush CreateDefaultCompletionBackground()
    {
      var defaultCompletionBackground = new SolidColorBrush(Color.FromRgb(240, 240, 240));
      defaultCompletionBackground.Freeze();
      return defaultCompletionBackground;
    }

    public Brush CompletionBackground
    {
      get { return (Brush)GetValue(CompletionBackgroundProperty); }
      set { SetValue(CompletionBackgroundProperty, value); }
    }

    #endregion Code Completion

    #region Diagnostics (wriggles under the code text)
#if !NoDiagnostics

    internal void EhDiagnosticsUpdated(DiagnosticsUpdatedArgs a)
    {
      _syncContext.Post(o => ProcessDiagnostics(a), null);
    }

    private void ProcessDiagnostics(DiagnosticsUpdatedArgs args)
    {
      // Note: I have never seen here args with Kind == DiagnosticsRemoved
      _textMarkerService.RemoveAll(marker => Equals(args.Id, marker.Tag));

      if (args.Kind == DiagnosticsUpdatedKind.DiagnosticsCreated)
      {
        foreach (var diagnosticData in args.GetAllDiagnosticsRegardlessOfPushPullSetting())
        {
          if (diagnosticData.Severity == DiagnosticSeverity.Hidden || diagnosticData.IsSuppressed)
          {
            continue;
          }

          var textSpan = diagnosticData.DataLocation.SourceSpan;
          if (!textSpan.HasValue)
          {
            continue;
          }

          var marker = _textMarkerService.TryCreate(textSpan.Value.Start, textSpan.Value.Length);
          if (marker is not null)
          {
            marker.Tag = args.Id;
            marker.MarkerColor = GetDiagnosticsColor(diagnosticData);
            marker.ToolTip = diagnosticData.Message;
          }
        }
      }
    }

    private static Color GetDiagnosticsColor(Microsoft.CodeAnalysis.Diagnostics.DiagnosticData diagnosticData)
    {
      switch (diagnosticData.Severity)
      {
        case DiagnosticSeverity.Info:
          return Colors.LimeGreen;

        case DiagnosticSeverity.Warning:
          return Colors.DodgerBlue;

        case DiagnosticSeverity.Error:
          return Colors.Red;

        default:
          throw new ArgumentOutOfRangeException();
      }
    }
#endif
    #endregion Diagnostics (wriggles under the code text)

    #region Bracket Highlighting (matching brackets are highlighted)
#if !NoBraceMatching

    /// <summary>
    /// Highlights matching brackets.
    /// </summary>
    private async void HighlightBrackets(object sender, EventArgs e)
    {
      if (null != _adapter)
      {
        var result = await _adapter?.GetMatchingBracesAsync(Math.Max(0, TextArea.Caret.Offset - 1));
        _bracketHighlightRenderer.SetHighlight(result);
      }
    }

#endif
    #endregion Bracket Highlighting (matching brackets are highlighted)

    #region Reference highlighting (all identical items are highlighted)
#if !NoReferenceHighlighting

    /// <summary>
    /// Delays the Resolve check so that it does not get called too often when user holds an arrow.
    /// </summary>
    private DispatcherTimer _referenceHighlightRenderer_DelayMoveTimer;

    private const int _referenceHighlightRenderer_DelayMoveInMilliseconds = 100;

    /// <summary>
    /// Delays the Find references (and highlight) after the caret stays at one point for a while.
    /// </summary>
    private DispatcherTimer _referenceHighlightRenderer_DelayTimer;

    private const int _referenceHighlightRenderer_DelayInMilliseconds = 800;

    /// <summary>
    /// Maximum time for Find references. After this time it gets cancelled and no highlight is displayed.
    /// Useful for very large files.
    /// </summary>
    private const int _referenceHighlightRenderer_FindReferencesTimeoutInMilliSeconds = 200;

    private ReferenceHightlighting.ExpressionHighlightRenderer _referencesHighlightRenderer;

    private ImmutableArray<DocumentHighlights> _referencesHighlightRenderer_LastResolveResult;

    /// <summary>
    /// In the code editor, highlights all references to the expression under the caret (for better code readability).
    /// </summary>
    public void ReferencesHighlightRenderer_Initialize()
    {
      _referencesHighlightRenderer = new ReferenceHightlighting.ExpressionHighlightRenderer(TextArea.TextView);
      _referenceHighlightRenderer_DelayTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(_referenceHighlightRenderer_DelayInMilliseconds) };
      _referenceHighlightRenderer_DelayTimer.Stop();
      _referenceHighlightRenderer_DelayTimer.Tick += ReferencesHighlightRenderer_TimerTick;
      _referenceHighlightRenderer_DelayMoveTimer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(_referenceHighlightRenderer_DelayMoveInMilliseconds) };
      _referenceHighlightRenderer_DelayMoveTimer.Stop();
      _referenceHighlightRenderer_DelayMoveTimer.Tick += ReferencesHighlightRenderer_TimerMoveTick;
      TextArea.Caret.PositionChanged += CaretPositionChanged;
      // fixes SD-1873 - Unhandled WPF Exception when deleting text in text editor
      // clear highlights to avoid exceptions when trying to draw highlights in
      // locations that have been deleted already.
      Document.Changed += delegate
      { _referencesHighlightRenderer_LastResolveResult = ImmutableArray<DocumentHighlights>.Empty; ReferencesHighlightRenderer_ClearHighlight(); };
    }

    public void ReferencesHighlightRenderer_ClearHighlight()
    {
      _referencesHighlightRenderer.ClearHighlight();
    }

    /// <summary>
    /// In the current document, highlights all references to the expression
    /// which is currently under the caret (local variable, class, property).
    /// This gets called on every caret position change, so quite often.
    /// </summary>
    private void CaretPositionChanged(object sender, EventArgs e)
    {
      ReferencesHighlightRenderer_Restart(_referenceHighlightRenderer_DelayMoveTimer);
    }

    private async void ReferencesHighlightRenderer_TimerTick(object sender, EventArgs e)
    {
      var adapter = _adapter;
      if (null != adapter)
      {
        _referenceHighlightRenderer_DelayTimer.Stop();
        var referencesToBeHighlighted = await adapter.FindReferencesInCurrentFile(TextArea.Caret.Offset);
        _referencesHighlightRenderer.SetHighlight(referencesToBeHighlighted);
      }
    }

    private async void ReferencesHighlightRenderer_TimerMoveTick(object sender, EventArgs e)
    {
      _referenceHighlightRenderer_DelayMoveTimer.Stop();
      _referenceHighlightRenderer_DelayTimer.Stop();
      var resolveResult = await _adapter?.FindReferencesInCurrentFile(TextArea.Caret.Offset);
      if (resolveResult == null)
      {
        _referencesHighlightRenderer_LastResolveResult = ImmutableArray<DocumentHighlights>.Empty;
        _referencesHighlightRenderer.ClearHighlight();
        return;
      }
      // caret is over symbol and that symbol is different from the last time
      if (!AreSameResolveResults(resolveResult, _referencesHighlightRenderer_LastResolveResult))
      {
        _referencesHighlightRenderer_LastResolveResult = resolveResult;
        _referencesHighlightRenderer.ClearHighlight();
        _referenceHighlightRenderer_DelayTimer.Start();
      }
      else
      {
        // highlight stays the same, both timers are stopped (will start again when caret moves)
      }
    }

    /// <summary>
    /// Restarts a timer.
    /// </summary>
    private void ReferencesHighlightRenderer_Restart(DispatcherTimer timer)
    {
      timer.Stop();
      timer.Start();
    }

    /// <summary>
    /// Returns true if the 2 ResolveResults refer to the same symbol.
    /// So that when caret moves but stays inside the same symbol, symbol stays highlighted.
    /// </summary>
    private bool AreSameResolveResults(ImmutableArray<DocumentHighlights> resolveResult, ImmutableArray<DocumentHighlights> resolveResult2)
    {
      if (resolveResult.Length != resolveResult2.Length)
        return false;

      for (int i = 0; i < resolveResult.Length; ++i)
      {
        if (!AreSameDocumentHightlights(resolveResult[i], resolveResult2[i]))
          return false;
      }

      return true;
    }

    private bool AreSameDocumentHightlights(DocumentHighlights x, DocumentHighlights y)
    {
      if (x.Document != y.Document)
        return false;
      if (x.HighlightSpans.Length != y.HighlightSpans.Length)
        return false;

      for (int i = 0; i < x.HighlightSpans.Length; ++i)
      {
        if (!AreSameHighlightSpans(x.HighlightSpans[i], y.HighlightSpans[i]))
          return false;
      }

      return true;
    }

    private bool AreSameHighlightSpans(HighlightSpan x, HighlightSpan y)
    {
      return x.Kind == y.Kind && x.TextSpan == y.TextSpan;
    }

#endif
    #endregion Reference highlighting (all identical items are highlighted)

    #region Folding
#if !NoFolding
    private void EhSyntaxTreeChanged_UpdateFoldings(Document document, SyntaxTree syntaxTree)
    {
      Action action = () =>
      {
        // update foldings
        var newFoldings = _adapter?.GetNewFoldings();
        _foldingManager.UpdateFoldings(newFoldings, -1);
      };

      if (Dispatcher.CheckAccess())
        action();
      else
        Dispatcher.BeginInvoke(action, DispatcherPriority.Background);
    }
#endif
    #endregion Folding

    protected override void OnKeyDown(KeyEventArgs e)
    {
      base.OnKeyDown(e);

#if !NoCompletion
      if (e.Key == Key.Space && e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Control))
      {
        e.Handled = true;
        var mode = e.KeyboardDevice.Modifiers.HasFlag(ModifierKeys.Shift)
            ? TriggerMode.SignatureHelp
            : TriggerMode.Completion;

        var task = ShowCompletion(mode);
      }
#endif

#if !NoExternalHelp
      // F1 - Get help
      if (e.Key == Key.F1 && null != _adapter)
      {
        _adapter.GetExternalHelpItemAndFireHelpEvent(CaretOffset);
      }
#endif

#if !NoRenaming
      // F2 - Rename symbol
      if (e.Key == Key.F2 && null != _adapter)
      {
        var topLevelWindow = GetTopLevelWindow(this);
        _adapter.RenameSymbol(CaretOffset, topLevelWindow, () => Focus());
      }
#endif

#if !NoGotoDefinition
      // F12 - GoToDefinition
      else if (e.Key == Key.F12 && null != _adapter)
      {
        int? jumpTo = _adapter.GoToDefinition(CaretOffset);
        if (jumpTo.HasValue)
        {
          JumpTo(jumpTo.Value);
        }
      }
#endif
    }

    private static Window GetTopLevelWindow(DependencyObject ctrl)
    {
      var result = ctrl;
      while (!(result is Window) && result != null)
      {
        result = VisualTreeHelper.GetParent(result);
      }
      return (Window)result;
    }

    private enum TriggerMode
    {
      Text,
      Completion,
      SignatureHelp
    }

    #region QuickInfo
#if !NoQuickInfo

    protected virtual async Task AsyncToolTipRequestDefaultImpl(ToolTipRequestEventArgs arg)
    {
      if (_adapter is { } adapter)
      {
        var info = await adapter.GetToolTipAsync(arg.Position);
        if (null != info)
        {
          arg.SetToolTip(info.Create());
        }
      }
    }
#endif

    public static readonly RoutedEvent ToolTipRequestEvent = EventManager.RegisterRoutedEvent("ToolTipRequest",
        RoutingStrategy.Bubble, typeof(ToolTipRequestEventHandler), typeof(CodeEditorView));

    public Func<ToolTipRequestEventArgs, Task> AsyncToolTipRequest { get; set; }

    public event ToolTipRequestEventHandler ToolTipRequest
    {
      add { AddHandler(ToolTipRequestEvent, value); }
      remove { RemoveHandler(ToolTipRequestEvent, value); }
    }

    #endregion QuickInfo

    private void OnVisualLinesChanged(object sender, EventArgs e)
    {
      if (_toolTip != null)
      {
        _toolTip.IsOpen = false;
      }
    }

    private void OnMouseHoverStopped(object sender, MouseEventArgs e)
    {
      if (_toolTip != null)
      {
        _toolTip.IsOpen = false;
        e.Handled = true;
      }
    }

    private async void OnMouseHover(object sender, MouseEventArgs e)
    {
      TextViewPosition? position;
      try
      {
        position = TextArea.TextView.GetPositionFloor(e.GetPosition(TextArea.TextView) + TextArea.TextView.ScrollOffset);
      }
      catch (ArgumentOutOfRangeException)
      {
        // TODO: check why this happens
        e.Handled = true;
        return;
      }
      var args = new ToolTipRequestEventArgs { InDocument = position.HasValue };
      if (!position.HasValue || position.Value.Location.IsEmpty)
      {
        return;
      }

      args.LogicalPosition = position.Value.Location;
      args.Position = Document.GetOffset(position.Value.Line, position.Value.Column);

      RaiseEvent(args);

      if (args.ContentToShow == null)
      {
#if !NoQuickInfo
        if (AsyncToolTipRequest != null)
        {
          await AsyncToolTipRequest.Invoke(args).ConfigureAwait(true);
        }
#endif
      }

      if (args.ContentToShow == null)
        return;

      if (_toolTip == null)
      {
        _toolTip = new ToolTip { MaxWidth = 400 };
        _toolTip.Closed += ToolTipClosed;
        ToolTipService.SetInitialShowDelay(_toolTip, 0);
      }
      _toolTip.PlacementTarget = this; // required for property inheritance

      var stringContent = args.ContentToShow as string;
      if (stringContent != null)
      {
        _toolTip.Content = new TextBlock
        {
          Text = stringContent,
          TextWrapping = TextWrapping.Wrap
        };
      }
      else
      {
        _toolTip.Content = args.ContentToShow;
      }

      e.Handled = true;
      _toolTip.IsOpen = true;
    }

    private void ToolTipClosed(object sender, EventArgs e)
    {
      _toolTip = null;
    }

    #region Open & Save File

    public void OpenFile(string fileName)
    {
      if (!File.Exists(fileName))
      {
        throw new FileNotFoundException(fileName);
      }

      _completionWindow?.Close();
      _insightWindow?.Close();

      Load(fileName);
      Document.FileName = fileName;
    }

    public bool SaveFile()
    {
      if (string.IsNullOrEmpty(Document.FileName))
      {
        return false;
      }

      Save(Document.FileName);
      return true;
    }

    #endregion Open & Save File


    private void OnTextEntered(object sender, TextCompositionEventArgs textCompositionEventArgs)
    {
#if !NoCompletion
      var task = ShowCompletion(TriggerMode.Text);
#endif

#if !NoLiveDocumentFormatting
      // Format document when entering closing curly brace or semicolon
      var lastChar = Document.GetCharAt(CaretOffset - 1);
      if (lastChar == '}' || lastChar == ';')
      {
        if (Adapter is ICodeEditorViewAdapter adapter)
        {
          adapter.FormatDocumentAfterEnteringTriggerChar(CaretOffset, lastChar);
        }
      }
#endif
    }

    #region Code Completion
#if !NoCompletion


    private async Task ShowCompletion(TriggerMode triggerMode)
    {
      var adapter = _adapter;
      if (adapter == null)
      {
        return;
      }

      GetCompletionDocument(out var offset);
      var completionChar = triggerMode == TriggerMode.Text ? Document.GetCharAt(offset - 1) : (char?)null;
      var results = await adapter.GetCompletionData(offset, completionChar,
                  triggerMode == TriggerMode.SignatureHelp).ConfigureAwait(true);
      if (results.OverloadProvider != null)
      {
        results.OverloadProvider.Refresh();

        if (_insightWindow != null && _insightWindow.IsVisible)
        {
          _insightWindow.Provider = results.OverloadProvider;
        }
        else
        {
          _insightWindow = new OverloadInsightWindow(TextArea)
          {
            Provider = results.OverloadProvider,
            Background = CompletionBackground,
            Style = TryFindResource(typeof(InsightWindow)) as Style
          };
          _insightWindow.Show();
          _insightWindow.Closed += (o, args) => _insightWindow = null;
        }
        return;
      }

      if (_completionWindow == null && results.CompletionData?.Any() == true)
      {
        _insightWindow?.Close();

        // Open code completion after the user has pressed dot:
        _completionWindow = new CustomCompletionWindow(TextArea)
        {
          MinWidth = 300,
          Background = CompletionBackground,
          CloseWhenCaretAtBeginning = triggerMode == TriggerMode.Completion,
          UseHardSelection = results.UseHardSelection
        };
        if (completionChar != null && char.IsLetterOrDigit(completionChar.Value))
        {
          _completionWindow.StartOffset -= 1;
        }

        var data = _completionWindow.CompletionList.CompletionData;
        ICompletionDataEx selected = null;
        foreach (var completion in results.CompletionData)
        {
          if (completion.IsSelected)
          {
            selected = completion;
          }
          data.Add(completion);
        }
        _completionWindow.CompletionList.SelectedItem = selected;
        _completionWindow.Show();
        _completionWindow.Closed += (o, args) => { _completionWindow = null; };
      }
    }

    private void OnTextEntering(object sender, TextCompositionEventArgs args)
    {
      if (args.Text.Length > 0 && _completionWindow != null)
      {
        if (!char.IsLetterOrDigit(args.Text[0]))
        {
          // Whenever a non-letter is typed while the completion window is open,
          // insert the currently selected element.
          _completionWindow.CompletionList.RequestInsertion(args);
        }
      }
      // Do not set e.Handled=true.
      // We still want to insert the character that was typed.
    }

    /// <summary>
    /// Gets the document used for code completion, can be overridden to provide a custom document
    /// </summary>
    /// <param name="offset"></param>
    /// <returns>The document of this text editor.</returns>
    protected virtual IDocument GetCompletionDocument(out int offset)
    {
      offset = CaretOffset;
      return Document;
    }
#endif
    #endregion Code Completion

    #region Jumping into text positions

    public bool TryCloseExistingPopups(bool force)
    {
      bool canClose = true;

      if (null != _completionWindow)
      {
        _completionWindow.Close();
        canClose = true;
      }

      if (null != _insightWindow)
      {
        _insightWindow.Close();
        canClose = true;
      }

      if (null != _toolTip)
      {
        _toolTip.Visibility = Visibility.Collapsed;
        canClose = true;
      }

      return canClose;
    }

    public void JumpTo(int caretOffset)
    {
      var location = TextArea.Document.GetLocation(caretOffset);
      JumpTo(location.Line, location.Column);
    }

    public void JumpTo(int line, int column)
    {
      // closes Debugger popup on debugger step
      TryCloseExistingPopups(true);

      TextArea.ClearSelection();
      TextArea.Caret.Position = new TextViewPosition(line, column);
      // might have jumped to a different location if column was outside the valid range
      TextLocation actualLocation = TextArea.Caret.Location;
      if (ActualHeight > 0)
      {
        ScrollTo(actualLocation.Line, actualLocation.Column);
      }
      else
      {
        // we have to delay the scrolling if the text editor is not yet loaded
        Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(
          delegate
          {
            ScrollTo(actualLocation.Line, actualLocation.Column);
          }));
      }

      Focus();

      Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)DisplayCaretHighlightAnimation);
    }

    /// <summary>
    /// Responsible for displaying a small animation, that highlights the caret for 1 seconds in order to better find it.
    /// </summary>
    private async void DisplayCaretHighlightAnimation()
    {
      TextArea textArea = TextArea;

      var layer = AdornerLayer.GetAdornerLayer(textArea.TextView);

      if (layer == null)
        return;

      var adorner = new CaretHighlightAdorner(textArea);
      layer.Add(adorner);

      await Task.Delay(1000).ConfigureAwait(true);

      layer.Remove(adorner);
    }

    #endregion Jumping into text positions

    private class CustomCompletionWindow : CompletionWindow
    {
      private bool _isSoftSelectionActive;
      private KeyEventArgs _keyDownArgs;

      public CustomCompletionWindow(TextArea textArea) : base(textArea)
      {
        _isSoftSelectionActive = true;
        CompletionList.SelectionChanged += CompletionListOnSelectionChanged;
        CompletionList.ListBox.BorderThickness = new Thickness();
        CompletionList.ListBox.PreviewMouseDown += OnListBoxOnPreviewMouseDown;
      }

      private void OnListBoxOnPreviewMouseDown(object sender, MouseButtonEventArgs args)
      {
        _isSoftSelectionActive = false;
      }

      private void CompletionListOnSelectionChanged(object sender, SelectionChangedEventArgs args)
      {
        if (!UseHardSelection &&
            _isSoftSelectionActive && _keyDownArgs?.Handled != true
            && args.AddedItems?.Count > 0)
        {
          CompletionList.SelectedItem = null;
        }
      }

      protected override void OnKeyDown(KeyEventArgs e)
      {
        if (e.Key == Key.Home || e.Key == Key.End)
          return;

        _keyDownArgs = e;

        base.OnKeyDown(e);

        SetSoftSelection(e);
      }

      private void SetSoftSelection(RoutedEventArgs e)
      {
        if (e.Handled)
        {
          _isSoftSelectionActive = false;
        }
      }

      // ReSharper disable once MemberCanBePrivate.Local
      public bool UseHardSelection { get; set; }
    }
  }
}
