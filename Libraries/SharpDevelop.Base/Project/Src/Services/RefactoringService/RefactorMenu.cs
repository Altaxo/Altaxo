﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1927 $</version>
// </file>

using System;
using System.Reflection;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Tests if the refactoring provider for the current document
	/// supports the specified option.
	/// </summary>
	/// <attribute name="supports">
	/// Same of the action that should be supported.
	/// "*" to test if refactoring is supported at all.
	/// </attribute>
	/// <example title="Test if refactoring is supported">
	/// &lt;Condition name="RefactoringProviderSupports" supports="*"&gt;
	/// </example>
	/// <example title="Test if managing imports is supported">
	/// &lt;Condition name="RefactoringProviderSupports" supports="FindUnusedUsingDeclarations"&gt;
	/// </example>
	public class RefactoringProviderSupportsConditionEvaluator : IConditionEvaluator
	{
		public bool IsValid(object caller, Condition condition)
		{
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null) {
				return false;
			}
			ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as ITextEditorControlProvider;
			if (provider == null)
				return false;
			LanguageProperties language = ParserService.CurrentProjectContent.Language;
			if (language == null)
				return false;
			if (string.IsNullOrEmpty(provider.TextEditorControl.FileName))
				return false;
			
			RefactoringProvider rp = language.RefactoringProvider;
			if (!rp.IsEnabledForFile(provider.TextEditorControl.FileName))
				return false;
			
			string supports = condition.Properties["supports"];
			if (supports == "*")
				return true;
			
			Type t = rp.GetType();
			try {
				return (bool)t.InvokeMember("Supports" + supports, BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, rp, null);
			} catch (Exception ex) {
				LoggingService.Warn(ex.ToString());
				return false;
			}
		}
	}
	
	public abstract class AbstractRefactoringCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			if (ParserService.LoadSolutionProjectsThreadRunning) {
				return;
			}
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null) {
				return;
			}
			ITextEditorControlProvider provider = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent as ITextEditorControlProvider;
			if (provider == null) return;
			LanguageProperties language = ParserService.CurrentProjectContent.Language;
			if (language == null) return;
			
			RefactoringProvider rp = language.RefactoringProvider;
			Run(provider.TextEditorControl, rp);
			provider.TextEditorControl.Refresh();
		}
		
		protected ResolveResult ResolveAtCaret(TextEditorControl textEditor)
		{
			string fileName = textEditor.FileName;
			IExpressionFinder expressionFinder = ParserService.GetExpressionFinder(fileName);
			if (expressionFinder == null) return null;
			Caret caret = textEditor.ActiveTextAreaControl.Caret;
			string content = textEditor.Document.TextContent;
			ExpressionResult expr = expressionFinder.FindFullExpression(content, caret.Offset);
			if (expr.Expression == null) return null;
			return ParserService.Resolve(expr, caret.Line + 1, caret.Column + 1, fileName, content);
		}
		
		protected abstract void Run(TextEditorControl textEditor, RefactoringProvider provider);
	}
	
	public class RemoveUnusedUsingsCommand : AbstractRefactoringCommand
	{
		protected override void Run(TextEditorControl textEditor, RefactoringProvider provider)
		{
			NamespaceRefactoringService.ManageUsings(textEditor.FileName, textEditor.Document, true, true);
		}
	}
	
	public class RenameCommand : AbstractRefactoringCommand
	{
		protected override void Run(TextEditorControl textEditor, RefactoringProvider provider)
		{
			ResolveResult rr = ResolveAtCaret(textEditor);
			if (rr is MixedResolveResult) rr = (rr as MixedResolveResult).PrimaryResult;
			if (rr is TypeResolveResult) {
				IClass c = (rr as TypeResolveResult).ResolvedClass;
				if (c == null) {
					ShowUnknownSymbolError();
				} else if (c.CompilationUnit.FileName == null) {
					ShowNoUserCodeError();
				} else {
					FindReferencesAndRenameHelper.RenameClass(c);
				}
			} else if (rr is MemberResolveResult) {
				Rename((rr as MemberResolveResult).ResolvedMember);
			} else if (rr is MethodResolveResult) {
				Rename((rr as MethodResolveResult).GetMethodIfSingleOverload());
			} else {
				ShowUnknownSymbolError();
			}
		}
		
		static void ShowUnknownSymbolError()
		{
			MessageService.ShowMessage("${res:SharpDevelop.Refactoring.CannotRenameElement}");
		}
		static void ShowNoUserCodeError()
		{
			MessageService.ShowMessage("${res:SharpDevelop.Refactoring.CannotRenameBecauseNotUserCode}");
		}
		
		static void Rename(IMember member)
		{
			if (member == null) {
				ShowUnknownSymbolError();
			} else if (member.DeclaringType.CompilationUnit.FileName == null) {
				ShowNoUserCodeError();
			} else {
				FindReferencesAndRenameHelper.RenameMember(member);
			}
		}
	}
}
