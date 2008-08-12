﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2364 $</version>
// </file>

using System;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.TextEditor.Undo;

namespace ICSharpCode.SharpDevelop.Tests.Utils
{
	public class MockDocument : IDocument
	{
		ITextBufferStrategy textBufferStrategy;
		
		public MockDocument()
		{
		}
		
		public event EventHandler UpdateCommited;
		public event DocumentEventHandler DocumentAboutToBeChanged;
		public event DocumentEventHandler DocumentChanged;
		public event EventHandler TextContentChanged;
		
		public ITextEditorProperties TextEditorProperties {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public UndoStack UndoStack {
			get {
				throw new NotImplementedException();
			}
		}
		
		public bool ReadOnly {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public IFormattingStrategy FormattingStrategy {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public ITextBufferStrategy TextBufferStrategy {
			get {
				return textBufferStrategy;
			}
			set {
				textBufferStrategy = value;
			}
		}
		
		public FoldingManager FoldingManager {
			get {
				throw new NotImplementedException();
			}
		}
		
		public IHighlightingStrategy HighlightingStrategy {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public BookmarkManager BookmarkManager {
			get {
				throw new NotImplementedException();
			}
		}
		
		public ICustomLineManager CustomLineManager {
			get {
				throw new NotImplementedException();
			}
		}
		
		public MarkerStrategy MarkerStrategy {
			get {
				throw new NotImplementedException();
			}
		}
		
		public System.Collections.Generic.List<LineSegment> LineSegmentCollection {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int TotalNumberOfLines {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string TextContent {
			get {
				throw new NotImplementedException();
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public int TextLength {
			get {
				return textBufferStrategy.Length;
			}
		}
		
		public System.Collections.Generic.List<ICSharpCode.TextEditor.TextAreaUpdate> UpdateQueue {
			get {
				throw new NotImplementedException();
			}
		}
		
		public int GetLineNumberForOffset(int offset)
		{
			throw new NotImplementedException();
		}
		
		public LineSegment GetLineSegmentForOffset(int offset)
		{
			throw new NotImplementedException();
		}
		
		public LineSegment GetLineSegment(int lineNumber)
		{
			throw new NotImplementedException();
		}
		
		public int GetFirstLogicalLine(int lineNumber)
		{
			throw new NotImplementedException();
		}
		
		public int GetLastLogicalLine(int lineNumber)
		{
			throw new NotImplementedException();
		}
		
		public int GetVisibleLine(int lineNumber)
		{
			throw new NotImplementedException();
		}
		
		public int GetNextVisibleLineAbove(int lineNumber, int lineCount)
		{
			throw new NotImplementedException();
		}
		
		public int GetNextVisibleLineBelow(int lineNumber, int lineCount)
		{
			throw new NotImplementedException();
		}
		
		public void Insert(int offset, string text)
		{
			throw new NotImplementedException();
		}
		
		public void Remove(int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		public void Replace(int offset, int length, string text)
		{
			throw new NotImplementedException();
		}
		
		public char GetCharAt(int offset)
		{
			throw new NotImplementedException();
		}
		
		public string GetText(int offset, int length)
		{
			throw new NotImplementedException();
		}
		
		public string GetText(ISegment segment)
		{
			throw new NotImplementedException();
		}
		
		public System.Drawing.Point OffsetToPosition(int offset)
		{
			throw new NotImplementedException();
		}
		
		public int PositionToOffset(System.Drawing.Point p)
		{
			throw new NotImplementedException();
		}
		
		public void RequestUpdate(ICSharpCode.TextEditor.TextAreaUpdate update)
		{
			throw new NotImplementedException();
		}
		
		public void CommitUpdate()
		{
			throw new NotImplementedException();
		}
		
		public void UpdateSegmentListOnDocumentChange<T>(System.Collections.Generic.List<T> list, DocumentEventArgs e) where T : ISegment
		{
			throw new NotImplementedException();
		}
		
		void OnUpdateCommited()
		{
			if (UpdateCommited != null) {
			}
		}
		
		void OnDocumentAboutToBeChanged()
		{
			if (DocumentAboutToBeChanged != null) {
			}
		}

		void OnDocumentChanged()
		{
			if (DocumentChanged != null) {
			}
		}
		
		void OnTextContentChanged()
		{
			if (TextContentChanged != null) {
			}
		}
	}
}
