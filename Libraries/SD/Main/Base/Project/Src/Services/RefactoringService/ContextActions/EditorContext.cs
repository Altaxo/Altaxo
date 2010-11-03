﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Visitors;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.NRefactoryResolver;
using ICSharpCode.SharpDevelop.Editor;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Helper class for <see cref="IContextActionsProvider.GetAvailableActions"></see>.
	/// Never keep long-lived references to this class
	/// - the AST serves as one-time cache and does not get updated when editor text changes.
	/// </summary>
	public class EditorContext
	{
		public ITextEditor Editor { get; private set; }
		int CaretLine { get; set; }
		int CaretColumn { get; set; }
		
		/// <summary>
		/// Language independent.
		/// </summary>
		public ExpressionResult CurrentExpression { get; private set; }
		/// <summary>
		/// Language independent.
		/// </summary>
		public ResolveResult CurrentSymbol { get; private set; }
		
		/// <summary>
		/// Language independent.
		/// </summary>
		public ParseInformation CurrentParseInformation { get; private set; }
		
		public IProjectContent ProjectContent { 
			get {
				if (CurrentParseInformation != null)
					return CurrentParseInformation.CompilationUnit.ProjectContent;
				else
					return null;
			}
		}
		
		public IDocumentLine CurrentLine { get; private set; }
		/// <summary>
		/// Only available for C# and VB.
		/// </summary>
		public INode CurrentLineAST { get; private set; }
		/// <summary>
		/// Only available for C# and VB.
		/// </summary>
		public INode CurrentMemberAST { get; private set; }
		/// <summary>
		/// Only available for C# and VB.
		/// </summary>
		public INode CurrentElement { get; private set; }
		
		NRefactoryResolver Resolver { get; set; }
		
		public EditorContext(ITextEditor editor)
		{
			if (editor == null)
				throw new ArgumentNullException("editor");
			this.Editor = editor;
			this.CaretLine = editor.Caret.Line;
			this.CaretColumn = editor.Caret.Column;
			if (CaretColumn > 1 && editor.Document.GetText(editor.Document.PositionToOffset(CaretLine, CaretColumn - 1), 1) == ";") {
				// If caret is just after ';', pretend that caret is before ';'
				// (works well e.g. for this.Foo();(*caret*) - we want to get "this.Foo()")
				// This is equivalent to pretending that ; don't exist, and actually it's not such a bad idea.
				CaretColumn -= 1;
			}
			
			this.CurrentExpression = GetExpressionAtCaret(editor);
			this.CurrentSymbol = ResolveExpression(CurrentExpression, editor, CaretLine, CaretColumn);
			this.CurrentParseInformation = ParserService.GetExistingParseInformation(editor.FileName);
			
			this.CurrentLine = editor.Document.GetLine(CaretLine);
			this.CurrentLineAST = GetCurrentLineAst(this.CurrentLine, editor);
			
			this.CurrentMemberAST = GetCurrentMemberAST(editor);
			
			this.CurrentElement = FindInnermostNode(this.CurrentMemberAST, new Location(CaretColumn, CaretLine));
			
//			DebugLog();
		}
		
		void DebugLog()
		{
			ICSharpCode.Core.LoggingService.Debug(string.Format(
				@"
	
	Context actions :
	ExprAtCaret: {0}
	----------------------
	SymbolAtCaret: {1}
	----------------------
	CurrentLineAST: {2}
	----------------------
	AstNodeAtCaret: {3}
	----------------------
	CurrentMemberAST: {4}
	----------------------",
				CurrentExpression, CurrentSymbol, CurrentLineAST,
				CurrentElement == null ? "" : CurrentElement.ToString().TakeStartEllipsis(400),
				CurrentMemberAST == null ? "" : CurrentMemberAST.ToString().TakeStartEllipsis(400)));
		}
		
		public TNode GetCurrentElement<TNode>() where TNode : class, INode
		{
			if (this.CurrentElement is TNode)
				return (TNode)this.CurrentElement;
			return null;
		}
		
		public TNode GetContainingElement<TNode>() where TNode : class, INode
		{
			var node = this.CurrentElement;
			while(node != null)
			{
				if (node is TNode)
					return (TNode)node;
				node = node.Parent;
			}
			return null;
		}
		
		Dictionary<Type, object> cachedValues = new Dictionary<Type, object>();
		
		public T GetCached<T>() where T : IContextActionCache, new()
		{
			Type t = typeof(T);
			if (cachedValues.ContainsKey(t)) {
				return (T)cachedValues[t];
			} else {
				T cached = new T();
				cached.Initialize(this);
				cachedValues[t] = cached;
				return cached;
			}
		}
		
		public static INode FindInnermostNode(INode node, Location position)
		{
			if (node == null)
				return null;
			var findInnermostVisitor = new FindInnermostNodeByRangeVisitor(position);
			node.AcceptVisitor(findInnermostVisitor, null);
			return findInnermostVisitor.InnermostNode;
		}
		
		public static INode FindInnermostNodeContainingSelection(INode node, Location start, Location end)
		{
			if (node == null)
				return null;
			var findInnermostVisitor = new FindInnermostNodeByRangeVisitor(start, end);
			node.AcceptVisitor(findInnermostVisitor, null);
			return findInnermostVisitor.InnermostNode;
		}
		
		class FindInnermostNodeByRangeVisitor : NodeTrackingAstVisitor
		{
			public Location RangeStart { get; private set; }
			public Location RangeEnd { get; private set; }
			public INode InnermostNode { get; private set; }
			
			public FindInnermostNodeByRangeVisitor(Location caretPosition) : this(caretPosition, caretPosition)
			{
			}
			
			public FindInnermostNodeByRangeVisitor(Location selectionStart, Location selectionEnd)
			{
				this.RangeStart = selectionStart;
				this.RangeEnd = selectionEnd;
			}
			
			protected override void BeginVisit(INode node)
			{
				if (node.StartLocation <= RangeStart && node.EndLocation >= RangeEnd) {
					// the node visited last will be the innermost
					this.InnermostNode = node;
				}
				base.BeginVisit(node);
			}
		}

		ResolveResult ResolveExpression(ExpressionResult expression, ITextEditor editor, int caretLine, int caretColumn)
		{
			return ParserService.Resolve(expression, caretLine, caretColumn, editor.FileName, editor.Document.Text);
		}

		ExpressionResult GetExpressionAtCaret(ITextEditor editor)
		{
			ExpressionResult expr = ParserService.FindFullExpression(CaretLine, CaretColumn, editor.Document, editor.FileName);
			// if no expression, look one character back (works better with method calls - Foo()(*caret*))
			if (string.IsNullOrWhiteSpace(expr.Expression) && CaretColumn > 1)
				expr = ParserService.FindFullExpression(CaretLine, CaretColumn - 1, editor.Document, editor.FileName);
			return expr;
		}
		
		
		INode GetCurrentLineAst(IDocumentLine currentLine, ITextEditor editor)
		{
			if (currentLine == null)
				return null;
			var snippetParser = GetSnippetParser(editor);
			if (snippetParser == null)
				return null;
			//try	{
				return snippetParser.Parse(currentLine.Text);
			//}
			//catch {
			//	return null;
			//}
		}
		
		SnippetParser GetSnippetParser(ITextEditor editor)
		{
			var lang = GetEditorLanguage(editor);
			if (lang != null) {
				return new SnippetParser(lang.Value);
			}
			return null;
		}
		
		public static SupportedLanguage? GetEditorLanguage(ITextEditor editor)
		{
			if (editor == null || editor.Language == null)
				return null;
			if (editor.Language.Properties == LanguageProperties.CSharp)
				return SupportedLanguage.CSharp;
			if (editor.Language.Properties == LanguageProperties.VBNet)
				return SupportedLanguage.VBNet;
			return null;
		}
		
		
		INode GetCurrentMemberAST(ITextEditor editor)
		{
			//try {
				var resolver = GetInitializedNRefactoryResolver(editor, this.CaretLine, this.CaretColumn);
				if (resolver == null)
					return null;
				return resolver.ParseCurrentMember(editor.Document.Text);
			//} catch {
			//	return null;
			//}
		}
		
		NRefactoryResolver GetInitializedNRefactoryResolver(ITextEditor editor, int caretLine, int caretColumn)
		{
			if (editor == null || editor.Language == null)
				return null;
			try
			{
				var resolver = new NRefactoryResolver(editor.Language.Properties);
				resolver.Initialize(ParserService.GetParseInformation(editor.FileName), caretLine, caretColumn);
				return resolver;
			}
			catch(NotSupportedException)
			{
				return null;
			}
		}
	}
}
