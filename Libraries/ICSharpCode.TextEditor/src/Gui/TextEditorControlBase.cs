// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Xml;
using System.Text;

using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Actions;
 
namespace ICSharpCode.TextEditor
{
	/// <summary>
	/// This class is used for a basic text area control
	/// </summary>
	[ToolboxItem(false)]
	public abstract class TextEditorControlBase : UserControl
	{
		string    currentFileName = null;
		int       updateLevel     = 0;
		IDocument document;
		
		/// <summary>
		/// This hashtable contains all editor keys, where
		/// the key is the key combination and the value the
		/// action.
		/// </summary>
		protected Hashtable editactions = new Hashtable();
		
		public ITextEditorProperties TextEditorProperties {
			get {
				return document.TextEditorProperties;
			}
			set {
				document.TextEditorProperties = value;
			}
		}
		
		/// <value>
		/// Current file's character encoding
		/// </value>
		public Encoding Encoding {
			get {
				return TextEditorProperties.Encoding;
			}
			set {
//				if (encoding != null && value != null && !encoding.Equals(value) && !CharacterEncoding.IsUnicode(value)) {
//					Byte[] bytes = encoding.GetBytes(Text);
//					Text = new String(value.GetChars(bytes));
//				}
				TextEditorProperties.Encoding = value;
			}
		}
		
		/// <value>
		/// The current file name
		/// </value>
		[Browsable(false)]
		[ReadOnly(true)]
		public string FileName {
			get {
				return currentFileName;
			}
			set {
				if (currentFileName != value) {
					currentFileName = value;
					OnFileNameChanged(EventArgs.Empty);
				}
			}
		}
		
		/// <value>
		/// true, if the textarea is updating it's status, while
		/// it updates it status no redraw operation occurs.
		/// </value>
		[Browsable(false)]
		public bool IsUpdating {
			get {
				return updateLevel > 0;
			}
		}
		
		/// <value>
		/// The current document
		/// </value>
		[Browsable(false)]
		public IDocument Document {
			get {
				return document;
			}
			set {
				document = value;
			}
		}
		
		[Browsable(true)]
		public override string Text {
			get {
				return Document.TextContent;
			}
			set {
				Document.TextContent = value;
			}
		}
		
		static Font ParseFont(string font)
		{
			string[] descr = font.Split(new char[]{',', '='});
			return new Font(descr[1], Single.Parse(descr[3]));
		}
		
		/// <value>
		/// If set to true the contents can't be altered.
		/// </value>
		[Browsable(false)]
		[ReadOnly(true)]
		public bool IsReadOnly {
			get {
				return Document.ReadOnly;
			}
			set {
				Document.ReadOnly = value;
			}
		}
		
		[Browsable(false)]
		public bool IsInUpdate {
			get {
				return this.updateLevel > 0; 
			}
		}
		
		/// <value>
		/// supposedly this is the way to do it according to .NET docs,
		/// as opposed to setting the size in the constructor
		/// </value>
		protected override Size DefaultSize {
			get {
				return new Size(100, 100);
			}
		}
		
#region Document Properties
		/// <value>
		/// If true spaces are shown in the textarea
		/// </value>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("If true spaces are shown in the textarea")]
		public bool ShowSpaces {
			get {
				return document.TextEditorProperties.ShowSpaces;
			}
			set {
				document.TextEditorProperties.ShowSpaces = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// If true antialiased fonts are used inside the textarea
		/// </value>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("If true antialiased fonts are used inside the textarea")]
		public bool UseAntiAliasFont {
			get { 
				return document.TextEditorProperties.UseAntiAliasedFont;
			}
			set { 
				document.TextEditorProperties.UseAntiAliasedFont = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// If true tabs are shown in the textarea
		/// </value>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("If true tabs are shown in the textarea")]
		public bool ShowTabs {
			get { 
				return document.TextEditorProperties.ShowTabs;
			}
			set {
				document.TextEditorProperties.ShowTabs = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// If true EOL markers are shown in the textarea
		/// </value>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("If true EOL markers are shown in the textarea")]
		public bool ShowEOLMarkers {
			get {
				return document.TextEditorProperties.ShowEOLMarker;
			}
			set {
				document.TextEditorProperties.ShowEOLMarker = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// If true the horizontal ruler is shown in the textarea
		/// </value>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("If true the horizontal ruler is shown in the textarea")]
		public bool ShowHRuler {
			get { 
				return document.TextEditorProperties.ShowHorizontalRuler;
			}
			set { 
				document.TextEditorProperties.ShowHorizontalRuler = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// If true the vertical ruler is shown in the textarea
		/// </value>
		[Category("Appearance")]
		[DefaultValue(false)]
		[Description("If true the vertical ruler is shown in the textarea")]
		public bool ShowVRuler {
			get {
				return document.TextEditorProperties.ShowVerticalRuler;
			}
			set {
				document.TextEditorProperties.ShowVerticalRuler = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// The row in which the vertical ruler is displayed
		/// </value>
		[Category("Appearance")]
		[DefaultValue(80)]
		[Description("The row in which the vertical ruler is displayed")]
		public int VRulerRow {
			get {
				return document.TextEditorProperties.VerticalRulerRow;
			}
			set {
				document.TextEditorProperties.VerticalRulerRow = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// If true line numbers are shown in the textarea
		/// </value>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("If true line numbers are shown in the textarea")]
		public bool ShowLineNumbers {
			get {
				return document.TextEditorProperties.ShowLineNumbers;
			}
			set {
				document.TextEditorProperties.ShowLineNumbers = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// If true invalid lines are marked in the textarea
		/// </value>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("If true invalid lines are marked in the textarea")]
		public bool ShowInvalidLines {
			get { 
				return document.TextEditorProperties.ShowInvalidLines;
			}
			set { 
				document.TextEditorProperties.ShowInvalidLines = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// If true folding is enabled in the textarea
		/// </value>
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("If true folding is enabled in the textarea")]
		public bool EnableFolding {
			get { 
				return document.TextEditorProperties.EnableFolding;
			}
			set {
				document.TextEditorProperties.EnableFolding = value;
				OptionsChanged();
			}
		}
		
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("If true matching brackets are highlighted")]
		public bool ShowMatchingBracket {
			get { 
				return document.TextEditorProperties.ShowMatchingBracket;
			}
			set {
				document.TextEditorProperties.ShowMatchingBracket = value;
				OptionsChanged();
			}
		}
		
		[Category("Appearance")]
		[DefaultValue(true)]
		[Description("If true the icon bar is displayed")]
		public bool IsIconBarVisible {
			get { 
				return document.TextEditorProperties.IsIconBarVisible;
			}
			set {
				document.TextEditorProperties.IsIconBarVisible = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// The width in spaces of a tab character
		/// </value>
		[Category("Appearance")]
		[DefaultValue(4)]
		[Description("The width in spaces of a tab character")]
		public int TabIndent {
			get { 
				return document.TextEditorProperties.TabIndent;
			}
			set { 
				document.TextEditorProperties.TabIndent = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// The line viewer style
		/// </value>
		[Category("Appearance")]
		[DefaultValue(LineViewerStyle.None)]
		[Description("The line viewer style")]
		public LineViewerStyle LineViewerStyle {
			get { 
				return document.TextEditorProperties.LineViewerStyle;
			}
			set { 
				document.TextEditorProperties.LineViewerStyle = value;
				OptionsChanged();
			}
		}

		/// <value>
		/// The indent style
		/// </value>
		[Category("Behavior")]
		[DefaultValue(IndentStyle.Smart)]
		[Description("The indent style")]
		public IndentStyle IndentStyle {
			get { 
				return document.TextEditorProperties.IndentStyle;
			}
			set { 
				document.TextEditorProperties.IndentStyle = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// if true spaces are converted to tabs
		/// </value>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Converts tabs to spaces while typing")]
		public bool ConvertTabsToSpaces {
			get { 
				return document.TextEditorProperties.ConvertTabsToSpaces;
			}
			set { 
				document.TextEditorProperties.ConvertTabsToSpaces = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// if true spaces are converted to tabs
		/// </value>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Creates a backup copy for overwritten files")]
		public bool CreateBackupCopy {
			get { 
				return document.TextEditorProperties.CreateBackupCopy;
			}
			set { 
				document.TextEditorProperties.CreateBackupCopy = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// if true spaces are converted to tabs
		/// </value>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Hide the mouse cursor while typing")]
		public bool HideMouseCursor {
			get { 
				return document.TextEditorProperties.HideMouseCursor;
			}
			set { 
				document.TextEditorProperties.HideMouseCursor = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// if true spaces are converted to tabs
		/// </value>
		[Category("Behavior")]
		[DefaultValue(false)]
		[Description("Allows the caret to be places beyonde the end of line")]
		public bool AllowCaretBeyondEOL {
			get { 
				return document.TextEditorProperties.AllowCaretBeyondEOL;
			}
			set { 
				document.TextEditorProperties.AllowCaretBeyondEOL = value;
				OptionsChanged();
			}
		}
		
		/// <value>
		/// The base font of the text area. No bold or italic fonts
		/// can be used because bold/italic is reserved for highlighting
		/// purposes.
		/// </value>
		[Browsable(true)]
		[Description("The base font of the text area. No bold or italic fonts can be used because bold/italic is reserved for highlighting purposes.")]
		public override Font Font {
			get {
				return document.TextEditorProperties.Font;
			}
			set {
				document.TextEditorProperties.Font = value;
				OptionsChanged();
			}
		}

#endregion
		
		public TextEditorControlBase()
		{
			GenerateDefaultActions();
		}
		
				
		internal IEditAction GetEditAction(Keys keyData)
		{
			return (IEditAction)editactions[keyData];
		}

		void GenerateDefaultActions()
		{
			editactions[Keys.Left] = new CaretLeft();
			editactions[Keys.Left | Keys.Shift] = new ShiftCaretLeft();
			editactions[Keys.Left | Keys.Control] = new WordLeft();
			editactions[Keys.Left | Keys.Control | Keys.Shift] = new ShiftWordLeft();
			editactions[Keys.Right] = new CaretRight();
			editactions[Keys.Right | Keys.Shift] = new ShiftCaretRight();
			editactions[Keys.Right | Keys.Control] = new WordRight();
			editactions[Keys.Right | Keys.Control | Keys.Shift] = new ShiftWordRight();
			editactions[Keys.Up] = new CaretUp();
			editactions[Keys.Up | Keys.Shift] = new ShiftCaretUp();
			editactions[Keys.Up | Keys.Control] = new ScrollLineUp();
			editactions[Keys.Down] = new CaretDown();
			editactions[Keys.Down | Keys.Shift] = new ShiftCaretDown();
			editactions[Keys.Down | Keys.Control] = new ScrollLineDown();
			
			editactions[Keys.Insert] = new ToggleEditMode();
			editactions[Keys.Insert | Keys.Control] = new Copy();
			editactions[Keys.Insert | Keys.Shift] = new Paste();
			editactions[Keys.Delete] = new Delete();
			editactions[Keys.Delete | Keys.Shift] = new Cut();
			editactions[Keys.Home] = new Home();
			editactions[Keys.Home | Keys.Shift] = new ShiftHome();
			editactions[Keys.Home | Keys.Control] = new MoveToStart();
			editactions[Keys.Home | Keys.Control | Keys.Shift] = new ShiftMoveToStart();
			editactions[Keys.End] = new End();
			editactions[Keys.End | Keys.Shift] = new ShiftEnd();
			editactions[Keys.End | Keys.Control] = new MoveToEnd();
			editactions[Keys.End | Keys.Control | Keys.Shift] = new ShiftMoveToEnd();
			editactions[Keys.PageUp] = new MovePageUp();
			editactions[Keys.PageUp | Keys.Shift] = new ShiftMovePageUp();
			editactions[Keys.PageDown] = new MovePageDown();
			editactions[Keys.PageDown | Keys.Shift] = new ShiftMovePageDown();
			
			editactions[Keys.Return] = new Return();
			editactions[Keys.Tab] = new Tab();
			editactions[Keys.Tab | Keys.Shift] = new ShiftTab();
			editactions[Keys.Back] = new Backspace();
			editactions[Keys.Back | Keys.Shift] = new Backspace();
			
			editactions[Keys.X | Keys.Control] = new Cut();
			editactions[Keys.C | Keys.Control] = new Copy();
			editactions[Keys.V | Keys.Control] = new Paste();
			
			editactions[Keys.A | Keys.Control] = new SelectWholeDocument();
			editactions[Keys.Escape] = new ClearAllSelections();
			
			editactions[Keys.Divide | Keys.Control] = new ToggleComment();
			editactions[Keys.OemQuestion | Keys.Control] = new ToggleComment();
			
			editactions[Keys.Back | Keys.Alt]  = new Actions.Undo();
			editactions[Keys.Z | Keys.Control] = new Actions.Undo();
			editactions[Keys.Y | Keys.Control] = new Redo();
			
			editactions[Keys.Delete | Keys.Control] = new DeleteWord();
			editactions[Keys.Back | Keys.Control]   = new WordBackspace();
			editactions[Keys.D | Keys.Control]      = new DeleteLine();
			editactions[Keys.D | Keys.Shift | Keys.Control]      = new DeleteToLineEnd();
			
			editactions[Keys.B | Keys.Control]      = new GotoMatchingBrace();
			
		}
		
		/// <remarks>
		/// Call this method before a long update operation this
		/// 'locks' the text area so that no screen update occurs.
		/// </remarks>
		public virtual void BeginUpdate()
		{
			++updateLevel;
		}
		
		/// <remarks>
		/// Call this method to 'unlock' the text area. After this call
		/// screen update can occur. But no automatical refresh occurs you
		/// have to commit the updates in the queue.
		/// </remarks>
		public virtual void EndUpdate()
		{
			Debug.Assert(updateLevel > 0);
			updateLevel = Math.Max(0, updateLevel - 1);
		}
		
		public void LoadFile(string fileName)
		{
			LoadFile(fileName, true);
		}
		/// <remarks>
		/// Loads a file given by fileName
		/// </remarks>
		public void LoadFile(string fileName, bool autoLoadHighlighting)
		{
			BeginUpdate();
			document.TextContent = String.Empty;
			document.UndoStack.ClearAll();
			document.BookmarkManager.Clear();
			if (autoLoadHighlighting) {
				document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(fileName);
			}
			
			StreamReader stream;
			if (Encoding != null) {
				stream = new StreamReader(fileName, Encoding);
			} else {
				stream = new StreamReader(fileName);
			}
			Document.TextContent = stream.ReadToEnd();
			stream.Close();
			
			this.FileName = fileName;
			OptionsChanged();
			Document.UpdateQueue.Clear();
			EndUpdate();
			
			Refresh();
		}
		
		/// <remarks>
		/// Saves a file given by fileName
		/// </remarks>
		public void SaveFile(string fileName)
		{
			if (document.TextEditorProperties.CreateBackupCopy) {
				MakeBackupCopy(fileName);
			}
			
			StreamWriter stream;
			if (Encoding != null && Encoding.CodePage != 65001) {
				stream = new StreamWriter(fileName, false, Encoding);
			} else {
				stream = new StreamWriter(fileName, false);
			}
			
			foreach (LineSegment line in Document.LineSegmentCollection) {
				stream.Write(Document.GetText(line.Offset, line.Length));
				stream.Write(document.TextEditorProperties.LineTerminator);
			}
			
			stream.Close();
			
			this.FileName = fileName;
		}
		
		void MakeBackupCopy(string fileName) 
		{
			try {
				if (File.Exists(fileName)) {
					string backupName = fileName + ".bak";
					if (File.Exists(backupName)) {
						File.Delete(backupName);
					}
					File.Copy(fileName, backupName);
				}
			} catch (Exception) {
//				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
//				messageService.ShowError(e, "Can not create backup copy of " + fileName);
			}
		}
		
		public abstract void OptionsChanged();
		
		/// <remarks>
		/// Overwritten refresh method that locks if the control is in
		/// an update cycle.
		/// </remarks>
		public override void Refresh()
		{
			if (IsUpdating) {
				return;
			}
			base.Refresh();
		}
		
		protected virtual void OnFileNameChanged(EventArgs e)
		{
			if (FileNameChanged != null) {
				FileNameChanged(this, e);
			}
		}
		
		protected virtual void OnChanged(EventArgs e)
		{
			if (Changed != null) {
				Changed(this, e);
			}
		}
		
		public event EventHandler FileNameChanged;
		public event EventHandler Changed;
	}
}
