﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.NRefactory.Ast;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Commands
{
	public abstract class CodeGeneratorBase
	{
		ArrayList content = new ArrayList();
		protected IClass currentClass;
		protected ICSharpCode.SharpDevelop.Dom.Refactoring.CodeGenerator codeGen;
		protected ClassFinder classFinderContext;
		
		public void Initialize(IClass currentClass)
		{
			this.currentClass = currentClass.GetCompoundClass();
			this.codeGen = currentClass.ProjectContent.Language.CodeGenerator;
			this.classFinderContext = new ClassFinder(currentClass, currentClass.Region.BeginLine + 1, 0);
			this.InitContent();
		}
		
		protected virtual void InitContent()
		{
		}
		
		public abstract string CategoryName {
			get;
		}
		
		public virtual string Hint {
			get {
				return "no hint";
			}
		}
		
		public abstract int ImageIndex {
			get;
		}
		
		public virtual bool IsActive {
			get {
				return content.Count > 0;
			}
		}
		
		public ArrayList Content {
			get {
				return content;
			}
		}
		
		protected TypeReference ConvertType(IReturnType type)
		{
			return CodeGenerator.ConvertType(type, classFinderContext);
		}
		
		public virtual void GenerateCode(TextArea textArea, IList items)
		{
			List<AbstractNode> nodes = new List<AbstractNode>();
			GenerateCode(nodes, items);
			codeGen.InsertCodeInClass(currentClass, new TextEditorDocument(textArea.Document), textArea.Caret.Line, nodes.ToArray());
			ParserService.ParseCurrentViewContent();
		}
		
		public abstract void GenerateCode(List<AbstractNode> nodes, IList items);
	}
}
