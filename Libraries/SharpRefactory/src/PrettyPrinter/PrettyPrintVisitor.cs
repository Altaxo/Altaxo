// PrettyPrintVisitor.cs
// Copyright (C) 2003 Mike Krueger (mike@icsharpcode.net)
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Text;
using System.Collections;
using System.Diagnostics;

using ICSharpCode.SharpRefactory.Parser;
using ICSharpCode.SharpRefactory.Parser.AST;

namespace ICSharpCode.SharpRefactory.PrettyPrinter
{
	public class PrettyPrintVisitor : IASTVisitor
	{
		Errors  errors = new Errors();
		OutputFormatter outputFormatter = new OutputFormatter();
		
		public string Text {
			get {
				return outputFormatter.Text;
			}
		}
		public Errors Errors {
			get {
				return errors;
			}
		}
		
		public object Visit(INode node, object data)
		{
			errors.Error(-1, -1, String.Format("Visited INode (should NEVER HAPPEN) Node was : {0} ", node));
			return node.AcceptChildren(this, data);
		}
		
		public object Visit(AttributeSection section, object data)
		{
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.OpenSquareBracket);
			if (section.AttributeTarget != null && section.AttributeTarget != String.Empty) {
				outputFormatter.PrintIdentifier(section.AttributeTarget);
				outputFormatter.PrintToken(Tokens.Colon);
				outputFormatter.Space();
			}
			Debug.Assert(section.Attributes != null);
			foreach (ICSharpCode.SharpRefactory.Parser.AST.Attribute a in section.Attributes) {
				outputFormatter.PrintIdentifier(a.Name);
				outputFormatter.PrintToken(Tokens.OpenParenthesis);
				this.AppendCommaSeparatedList(a.PositionalArguments);
				
				if (a.NamedArguments != null && a.NamedArguments.Count > 0) {
					if (a.PositionalArguments.Count > 0) {
						outputFormatter.PrintToken(Tokens.Comma);
						outputFormatter.Space();
					}
					for (int i = 0; i < a.NamedArguments.Count; ++i) {
						NamedArgument n = (NamedArgument)a.NamedArguments[i];
						outputFormatter.PrintIdentifier(n.Name);
						outputFormatter.Space();
						outputFormatter.PrintToken(Tokens.Assign);
						outputFormatter.Space();
						n.Expr.AcceptVisitor(this, data);
						if (i + 1 < a.NamedArguments.Count) {
							outputFormatter.PrintToken(Tokens.Comma);
							outputFormatter.Space();
						}
					}
				}
				outputFormatter.PrintToken(Tokens.CloseParenthesis);
			}
			outputFormatter.PrintToken(Tokens.CloseSquareBracket);
			outputFormatter.PrintTrailingComment();
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(CompilationUnit compilationUnit, object data)
		{
			compilationUnit.AcceptChildren(this, data);
			return null;
		}
		
		public object Visit(UsingDeclaration usingDeclaration, object data)
		{
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.Using);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(usingDeclaration.Namespace);
			outputFormatter.PrintToken(Tokens.Semicolon);
			outputFormatter.PrintTrailingComment();
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(UsingAliasDeclaration usingAliasDeclaration, object data)
		{
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.Using);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(usingAliasDeclaration.Alias);
			outputFormatter.Space();
			outputFormatter.PrintToken(Tokens.Assign);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(usingAliasDeclaration.Namespace);
			outputFormatter.PrintToken(Tokens.Semicolon);
			outputFormatter.PrintTrailingComment();
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(NamespaceDeclaration namespaceDeclaration, object data)
		{
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.Namespace);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(namespaceDeclaration.NameSpace);
			outputFormatter.NewLine();
			outputFormatter.PrintToken(Tokens.OpenCurlyBrace);
			outputFormatter.NewLine();
			++outputFormatter.IndentationLevel;
			namespaceDeclaration.AcceptChildren(this, data);
			--outputFormatter.IndentationLevel;
			
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.CloseCurlyBrace);
			outputFormatter.NewLine();
			return null;
		}
		
		object VisitModifier(Modifier modifier)
		{
			if ((modifier & Modifier.Unsafe) != 0) {
				outputFormatter.PrintToken(Tokens.Unsafe);
			}
			if ((modifier & Modifier.Public) != 0) {
				outputFormatter.PrintToken(Tokens.Public);
			}
			if ((modifier & Modifier.Private) != 0) {
				outputFormatter.PrintToken(Tokens.Private);
			}
			if ((modifier & Modifier.Protected) != 0) {
				outputFormatter.PrintToken(Tokens.Protected);
			}
			if ((modifier & Modifier.Static) != 0) {
				outputFormatter.PrintToken(Tokens.Static);
			}
			if ((modifier & Modifier.Internal) != 0) {
				outputFormatter.PrintToken(Tokens.Internal);
			}
			if ((modifier & Modifier.Override) != 0) {
				outputFormatter.PrintToken(Tokens.Override);
			}
			if ((modifier & Modifier.Abstract) != 0) {
				outputFormatter.PrintToken(Tokens.Abstract);
			}
			if ((modifier & Modifier.New) != 0) {
				outputFormatter.PrintToken(Tokens.New);
			}
			if ((modifier & Modifier.Sealed) != 0) {
				outputFormatter.PrintToken(Tokens.Sealed);
			}
			if ((modifier & Modifier.Extern) != 0) {
				outputFormatter.PrintToken(Tokens.Extern);
			}
			if ((modifier & Modifier.Const) != 0) {
				outputFormatter.PrintToken(Tokens.Const);
			}
			if ((modifier & Modifier.Readonly) != 0) {
				outputFormatter.PrintToken(Tokens.Readonly);
			}
			if ((modifier & Modifier.Volatile) != 0) {
				outputFormatter.PrintToken(Tokens.Volatile);
			}
			outputFormatter.Space();
			return null;
		}
				
		object VisitParamModifiers(ParamModifiers modifier)
		{
			switch (modifier) {
				case ParamModifiers.Out:
					outputFormatter.PrintToken(Tokens.Out);
					break;
				case ParamModifiers.Params:
					outputFormatter.PrintToken(Tokens.Params);
					break;
				case ParamModifiers.Ref:
					outputFormatter.PrintToken(Tokens.Ref);
					break;
			}
			outputFormatter.Space();
			return null;
		}
		
		object VisitAttributes(ArrayList attributes, object data)
		{
			if (attributes == null || attributes.Count <= 0) {
				return null;
			}
			foreach (AttributeSection section in attributes) {
				Visit(section, data);
			}
			return null;
		}
		
		object Visit(TypeReference type, object data)
		{
			outputFormatter.PrintIdentifier(type.Type);
			for (int i = 0; i < type.PointerNestingLevel; ++i) {
				outputFormatter.PrintToken(Tokens.Times);
			}
			if (type.IsArrayType) {
				for (int i = 0; i < type.RankSpecifier.Length; ++i) {
					outputFormatter.PrintToken(Tokens.OpenSquareBracket);
					for (int j = 1; j < type.RankSpecifier[i]; ++j) {
						outputFormatter.PrintToken(Tokens.Comma);
					}
					outputFormatter.PrintToken(Tokens.CloseSquareBracket);
				}
			}
			return null;
		}
		
		object VisitEnumMembers(TypeDeclaration typeDeclaration, object data)
		{
			foreach (FieldDeclaration fieldDeclaration in typeDeclaration.Children) {
				VariableDeclaration f = (VariableDeclaration)fieldDeclaration.Fields[0];
				VisitAttributes(fieldDeclaration.Attributes, data);
				outputFormatter.Indent();
				outputFormatter.PrintIdentifier(f.Name);
				if (f.Initializer != null) {
					outputFormatter.Space();
					outputFormatter.PrintToken(Tokens.Assign);
					outputFormatter.Space();
					f.Initializer.AcceptVisitor(this, data);
				}
				outputFormatter.PrintToken(Tokens.Comma);
				outputFormatter.NewLine();
			}
			return null;
		}
		
		public object Visit(TypeDeclaration typeDeclaration, object data)
		{
			VisitAttributes(typeDeclaration.Attributes, data);
			VisitModifier(typeDeclaration.Modifier);
			switch (typeDeclaration.Type) {
				case Types.Class:
					outputFormatter.PrintToken(Tokens.Class);
					break;
				case Types.Enum:
					outputFormatter.PrintToken(Tokens.Enum);
					break;
				case Types.Interface:
					outputFormatter.PrintToken(Tokens.Interface);
					break;
				case Types.Struct:
					outputFormatter.PrintToken(Tokens.Struct);
					break;
			}
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(typeDeclaration.Name);
			if (typeDeclaration.BaseTypes != null && typeDeclaration.BaseTypes.Count > 0) {
				outputFormatter.PrintToken(Tokens.Colon);
				for (int i = 0; i < typeDeclaration.BaseTypes.Count; ++i) {
					outputFormatter.Space();
					outputFormatter.PrintIdentifier(typeDeclaration.BaseTypes[i]);
				}
			}
			outputFormatter.NewLine();
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.OpenCurlyBrace);
			outputFormatter.NewLine();
			
			++outputFormatter.IndentationLevel;
			if (typeDeclaration.Type == Types.Enum) {
				VisitEnumMembers(typeDeclaration, data);
			} else {
				typeDeclaration.AcceptChildren(this, data);
			}
			--outputFormatter.IndentationLevel;
			outputFormatter.Indent();
			outputFormatter.PrintToken(Tokens.CloseCurlyBrace);
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			VisitAttributes(parameterDeclarationExpression.Attributes, data);
			VisitParamModifiers(parameterDeclarationExpression.ParamModifiers);
			Visit(parameterDeclarationExpression.TypeReference, data);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(parameterDeclarationExpression.ParameterName);
			return null;
		}
		
		public object Visit(DelegateDeclaration delegateDeclaration, object data)
		{
			VisitAttributes(delegateDeclaration.Attributes, data);
			VisitModifier(delegateDeclaration.Modifier);
			outputFormatter.PrintToken(Tokens.Delegate);
			outputFormatter.Space();
			Visit(delegateDeclaration.ReturnType, data);
			outputFormatter.Space();
			outputFormatter.PrintIdentifier(delegateDeclaration.Name);
			outputFormatter.PrintToken(Tokens.OpenParenthesis);
			AppendCommaSeparatedList(delegateDeclaration.Parameters);
			outputFormatter.PrintToken(Tokens.CloseParenthesis);
			outputFormatter.PrintToken(Tokens.Semicolon);
			outputFormatter.NewLine();
			return null;
		}
		
		public object Visit(VariableDeclaration variableDeclaration, object data)
		{
			outputFormatter.PrintIdentifier(variableDeclaration.Name);
			if (variableDeclaration.Initializer != null) {
				outputFormatter.Space();
				outputFormatter.PrintToken(Tokens.Assign);
				outputFormatter.Space();
				variableDeclaration.Initializer.AcceptVisitor(this, data);
			}
			return null;
		}
		
		public void AppendCommaSeparatedList(IList list)
		{
			if (list != null) {
				for (int i = 0; i < list.Count; ++i) {
					((INode)list[i]).AcceptVisitor(this, null);
					if (i + 1 < list.Count) {
						outputFormatter.PrintToken(Tokens.Comma);
						outputFormatter.Space();
					}
				}
			}
		}
		
		public object Visit(EventDeclaration eventDeclaration, object data)
		{
//			VisitAttributes(eventDeclaration.Attributes, data);
//			VisitModifier(eventDeclaration.Modifier);
//			text.Append("event ");
//			Visit(eventDeclaration.TypeReference, data);
//			text.Append(' ');
//			if (eventDeclaration.VariableDeclarators != null && eventDeclaration.VariableDeclarators.Count > 0) {
//				AppendCommaSeparatedList(eventDeclaration.VariableDeclarators);
//				text.Append(";\n");
//			} else {
//				text.Append(eventDeclaration.Name);
//				text.Append(" {\n");
//				++indentationLevel;
//				if (eventDeclaration.AddRegion != null) {
//					eventDeclaration.AddRegion.AcceptVisitor(this, data);
//				}
//				if (eventDeclaration.RemoveRegion != null) {
//					eventDeclaration.RemoveRegion.AcceptVisitor(this, data);
//				}
//				--indentationLevel;
//				Indent();
//				text.Append("}\n");
//			}
			return null;
		}
		
		public object Visit(EventAddRegion addRegion, object data)
		{
//			this.VisitAttributes(addRegion.Attributes, data);
//			Indent();
//			VisitAttributes(addRegion.Attributes, data);
//			text.Append("add {\n");
//			++indentationLevel;
//			addRegion.Block.AcceptVisitor(this, false);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(EventRemoveRegion removeRegion, object data)
		{
//			this.VisitAttributes(removeRegion.Attributes, data);
//			Indent();
//			VisitAttributes(removeRegion.Attributes, data);
//			text.Append("remove {\n");
//			++indentationLevel;
//			removeRegion.Block.AcceptVisitor(this, false);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(FieldDeclaration fieldDeclaration, object data)
		{
//			VisitAttributes(fieldDeclaration.Attributes, data);
//			VisitModifier(fieldDeclaration.Modifier);
//			Visit(fieldDeclaration.TypeReference, data);
//			text.Append(' ');
//			AppendCommaSeparatedList(fieldDeclaration.Fields);
//			text.Append(";\n");
			return null;
		}
		
		public object Visit(ConstructorDeclaration constructorDeclaration, object data)
		{
//			VisitAttributes(constructorDeclaration.Attributes, data);
//			VisitModifier(constructorDeclaration.Modifier);
//			text.Append(constructorDeclaration.Name);
//			text.Append('(');
//			AppendCommaSeparatedList(constructorDeclaration.Parameters);
//			text.Append(")\n");
//			Indent();
//			text.Append("{\n");
//			++indentationLevel;
//			constructorDeclaration.Body.AcceptChildren(this, data);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(DestructorDeclaration destructorDeclaration, object data)
		{
//			VisitAttributes(destructorDeclaration.Attributes, data);
//			VisitModifier(destructorDeclaration.Modifier);
//			text.Append('~');
//			text.Append(destructorDeclaration.Name);
//			text.Append('(');
//			text.Append(")\n");
//			Indent();
//			text.Append("{\n");
//			++indentationLevel;
//			destructorDeclaration.Body.AcceptChildren(this, data);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(MethodDeclaration methodDeclaration, object data)
		{
//			VisitAttributes(methodDeclaration.Attributes, data);
//			VisitModifier(methodDeclaration.Modifier);
//			Visit(methodDeclaration.TypeReference, data);
//			text.Append(' ');
//			text.Append(methodDeclaration.Name);
//			text.Append('(');
//			AppendCommaSeparatedList(methodDeclaration.Parameters);
//			text.Append(")\n");
//			Indent();
//			text.Append("{\n");
//			++indentationLevel;
//			methodDeclaration.Body.AcceptChildren(this, data);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(IndexerDeclaration indexerDeclaration, object data)
		{
//			VisitAttributes(indexerDeclaration.Attributes, data);
//			VisitModifier(indexerDeclaration.Modifier);
//			Visit(indexerDeclaration.TypeReference, data);
//			text.Append(' ');
//			if (indexerDeclaration.NamespaceName != null && indexerDeclaration.NamespaceName.Length > 0) {
//				text.Append(indexerDeclaration.NamespaceName);
//				text.Append('.');
//			}
//			text.Append("this[");
//			AppendCommaSeparatedList(indexerDeclaration.Parameters);
//			text.Append("]\n");
//			Indent();
//			text.Append("{\n");
//			++indentationLevel;
//			if (indexerDeclaration.GetRegion != null) {
//				indexerDeclaration.GetRegion.AcceptVisitor(this, data);
//			}
//			if (indexerDeclaration.SetRegion != null) {
//				indexerDeclaration.SetRegion.AcceptVisitor(this, data);
//			}
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(PropertyDeclaration propertyDeclaration, object data)
		{
//			VisitAttributes(propertyDeclaration.Attributes, data);
//			VisitModifier(propertyDeclaration.Modifier);
//			Visit(propertyDeclaration.TypeReference, data);
//			text.Append(' ');
//			text.Append(propertyDeclaration.Name);
//			text.Append(" {\n");
//			++indentationLevel;
//			if (propertyDeclaration.GetRegion != null) {
//				propertyDeclaration.GetRegion.AcceptVisitor(this, data);
//			}
//			if (propertyDeclaration.SetRegion != null) {
//				propertyDeclaration.SetRegion.AcceptVisitor(this, data);
//			}
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(PropertyGetRegion getRegion, object data)
		{
//			this.VisitAttributes(getRegion.Attributes, data);
//			Indent();
//			text.Append("get {\n");
//			++indentationLevel;
//			getRegion.Block.AcceptVisitor(this, false);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(PropertySetRegion setRegion, object data)
		{
//			this.VisitAttributes(setRegion.Attributes, data);
//			Indent();
//			text.Append("set {\n");
//			++indentationLevel;
//			setRegion.Block.AcceptVisitor(this, false);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		// TODO : Add operator declaration to the parser ... 
		public object Visit(OperatorDeclaration operatorDeclaration, object data)
		{
//			VisitAttributes(operatorDeclaration.Attributes, data);
//			VisitModifier(operatorDeclaration.Modifier);
////			Visit(operatorDeclaration.TypeReference, data);
//			text.Append(' ');
////			if (operatorDeclaration.OperatorType == OperatorType.Explicit) {
////				text.Append("explicit");
////			} else if (operatorDeclaration.OperatorType == OperatorType.Implicit) {
////				text.Append("implicit");
////			} else {
////				text.Append(operatorDeclaration.overloadOperator)
////			}
//			text.Append('(');
////			text.Append(operatorDeclaration.FirstParameterType);
//			text.Append(' ');
////			text.Append(operatorDeclaration.FirstParameterName);
////			if (operatorDeclaration.OperatorType == OperatorType.Binary) {
//				text.Append(", ");
////				text.Append(operatorDeclaration.SecondParameterType);
//				text.Append(' ');
////				text.Append(operatorDeclaration.SecondParameterName);
////			}
//			text.Append(")\n");
//			Indent();
//			text.Append("{\n");
//			++indentationLevel;
//			operatorDeclaration.AcceptChildren(this, data);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(EmptyStatement emptyStatement, object data)
		{
//			Indent();
//			text.Append(";\n");
			return null;
		}
		
		public object Visit(BlockStatement blockStatement, object data)
		{
//			bool appendBrace = true;
//			if (data is bool) {
//				appendBrace = (bool)data;
//			}
//			if (appendBrace) {
//				Indent();
//				text.Append("{\n");
//				++indentationLevel;
//			}
//			blockStatement.AcceptChildren(this, true);
//			if (appendBrace) {
//				--indentationLevel;
//				Indent();
//				text.Append("}\n");
//			}
			return null;
		}
		
		public object Visit(ForStatement forStatement, object data)
		{
//			Indent();
//			text.Append("for (");
//			if (forStatement.Initializers != null && forStatement.Initializers.Count > 0) {
//				foreach (INode node in forStatement.Initializers) {
//					node.AcceptVisitor(this, false);
//					text.Append(',');
//				}
//			}
//			text.Append(';');
//			if (forStatement.Condition != null) {
//				forStatement.Condition.AcceptVisitor(this, data);
//			}
//			text.Append(';');
//			if (forStatement.Iterator != null && forStatement.Iterator.Count > 0) {
//				foreach (INode node in forStatement.Iterator) {
//					node.AcceptVisitor(this, false);
//					text.Append(',');
//				}
//			}
//			text.Append(") {\n");
//			++indentationLevel;
//			if (forStatement.EmbeddedStatement is BlockStatement) {
//				Visit((BlockStatement)forStatement.EmbeddedStatement, false);
//			} else {
//				forStatement.EmbeddedStatement.AcceptVisitor(this, data);
//			}
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(ForeachStatement foreachStatement, object data)
		{
//			Indent();
//			text.Append("foreach (");
//			Visit(foreachStatement.TypeReference, data);
//			text.Append(' ');
//			text.Append(foreachStatement.VariableName);
//			text.Append(" in ");
//			foreachStatement.Expression.AcceptVisitor(this, data);
//			text.Append(") {\n");
//			++indentationLevel;
//			if (foreachStatement.EmbeddedStatement is BlockStatement) {
//				Visit((BlockStatement)foreachStatement.EmbeddedStatement, false);
//			} else {
//				foreachStatement.EmbeddedStatement.AcceptVisitor(this, data);
//			}
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(WhileStatement whileStatement, object data)
		{
//			Indent();
//			text.Append("while (");
//			whileStatement.Condition.AcceptVisitor(this, data);
//			text.Append(") {\n");
//			++indentationLevel;
//			if (whileStatement.EmbeddedStatement is BlockStatement) {
//				Visit((BlockStatement)whileStatement.EmbeddedStatement, false);
//			} else {
//				whileStatement.EmbeddedStatement.AcceptVisitor(this, data);
//			}
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(DoWhileStatement doWhileStatement, object data)
		{
//			Indent();
//			text.Append("do {\n");
//			++indentationLevel;
//			if (doWhileStatement.EmbeddedStatement is BlockStatement) {
//				Visit((BlockStatement)doWhileStatement.EmbeddedStatement, false);
//			} else {
//				doWhileStatement.EmbeddedStatement.AcceptVisitor(this, data);
//			}
//			--indentationLevel;
//			Indent();
//			text.Append("} while (");
//			doWhileStatement.Condition.AcceptVisitor(this, data);
//			text.Append(");\n");
			return null;
		}
		
		public object Visit(BreakStatement breakStatement, object data)
		{
//			Indent();
//			text.Append("break;\n");
			return null;
		}
		
		public object Visit(ContinueStatement continueStatement, object data)
		{
//			Indent();
//			text.Append("continue;\n");
			return null;
		}
		
		public object Visit(CheckedStatement checkedStatement, object data)
		{
//			Indent();
//			text.Append("checked {\n");
//			++indentationLevel;
//			checkedStatement.Block.AcceptVisitor(this, false);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(UncheckedStatement uncheckedStatement, object data)
		{
//			Indent();
//			text.Append("unchecked {\n");
//			++indentationLevel;
//			uncheckedStatement.Block.AcceptVisitor(this, false);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(FixedStatement fixedStatement, object data)
		{
//			Indent();
//			text.Append("fixed (");
//			Visit(fixedStatement.TypeReference, data);
//			text.Append(' ');
//			AppendCommaSeparatedList(fixedStatement.PointerDeclarators);
//			text.Append(") {\n");
//			++indentationLevel;
//			if (fixedStatement.EmbeddedStatement is BlockStatement) {
//				Visit((BlockStatement)fixedStatement.EmbeddedStatement, false);
//			} else {
//				fixedStatement.EmbeddedStatement.AcceptVisitor(this, data);
//			}
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(GotoCaseStatement gotoCaseStatement, object data)
		{
//			Indent();
//			text.Append("goto ");
//			if (gotoCaseStatement.IsDefaultCase) {
//				text.Append("default");
//			} else {
//				text.Append("case ");
//				gotoCaseStatement.CaseExpression.AcceptVisitor(this, data);
//			}
//			text.Append(";\n");
			return null;
		}
		
		public object Visit(GotoStatement gotoStatement, object data)
		{
//			Indent();
//			text.Append("goto ");
//			text.Append(gotoStatement.Label);
//			text.Append(";\n");
			return null;
		}
		
		public object Visit(IfElseStatement ifElseStatement, object data)
		{
//			Indent();
//			text.Append("if (");
//			ifElseStatement.Condition.AcceptVisitor(this,data);
//			text.Append(") {\n");
//			++indentationLevel;
//			ifElseStatement.EmbeddedStatement.AcceptVisitor(this,data);
//			--indentationLevel;
//			Indent();
//			text.Append("} else {\n");
//			++indentationLevel;
//			ifElseStatement.EmbeddedElseStatement.AcceptVisitor(this,data);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(IfStatement ifStatement, object data)
		{
//			Indent();
//			text.Append("if (");
//			ifStatement.Condition.AcceptVisitor(this,data);
//			text.Append(") {\n");
//			++indentationLevel;
//			ifStatement.EmbeddedStatement.AcceptVisitor(this,data);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(LabelStatement labelStatement, object data)
		{
//			Indent();
//			text.Append(labelStatement.Label);
//			text.Append(":\n");
			return null;
		}
		
		public object Visit(LockStatement lockStatement, object data)
		{
//			Indent();
//			text.Append("lock (");
//			lockStatement.LockExpression.AcceptVisitor(this, data);
//			text.Append(") {\n");
//			++indentationLevel;
//			lockStatement.EmbeddedStatement.AcceptVisitor(this, data);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(ReturnStatement returnStatement, object data)
		{
//			Indent();
//			text.Append("return");
//			if (returnStatement.ReturnExpression != null) {
//				text.Append(" ");
//				returnStatement.ReturnExpression.AcceptVisitor(this, data);
//			}
//			text.Append(";\n");
			return null;
		}
		
		public object Visit(SwitchStatement switchStatement, object data)
		{
//			Indent();
//			text.Append("switch (");
//			switchStatement.SwitchExpression.AcceptVisitor(this, data);
//			text.Append(") {\n");
//			++indentationLevel;
//			foreach (SwitchSection section in switchStatement.SwitchSections) {
//				Indent();
//				text.Append("case ");
//				
//				for (int i = 0; i < section.SwitchLabels.Count; ++i) {
//					Expression label = (Expression)section.SwitchLabels[i];
//					if (label == null) {
//						text.Append("default:");
//						continue;
//					}
//					label.AcceptVisitor(this, data);
//					text.Append(":\n");
//				}
//				
//				++indentationLevel;
//				section.AcceptVisitor(this, data);
//				--indentationLevel;
//			}
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(ThrowStatement throwStatement, object data)
		{
//			Indent();
//			text.Append("throw ");
//			throwStatement.ThrowExpression.AcceptVisitor(this, data);
//			text.Append(";\n");
			return null;
		}
		
		public object Visit(TryCatchStatement tryCatchStatement, object data)
		{
//			Indent();
//			text.Append("try {\n");
//			
//			++indentationLevel;
//			tryCatchStatement.StatementBlock.AcceptVisitor(this, data);
//			--indentationLevel;
//			
//			if (tryCatchStatement.CatchClauses != null) {
//				int generated = 0;
//				foreach (CatchClause catchClause in tryCatchStatement.CatchClauses) {
//					Indent();
//					text.Append("} catch (");
//					text.Append(catchClause.Type);
//					text.Append(' ');
//					if (catchClause.VariableName == null) {
//						text.Append("generatedExceptionVariable" + generated.ToString());
//						++generated;
//					} else {
//						text.Append(catchClause.VariableName);
//					}
//					text.Append(") {\n");
//					++indentationLevel;
//					catchClause.StatementBlock.AcceptVisitor(this, data);
//					--indentationLevel;
//				}
//			}
//			
//			if (tryCatchStatement.FinallyBlock != null) {
//				Indent();
//				text.Append("} finally {\n");
//				++indentationLevel;
//				tryCatchStatement.FinallyBlock.AcceptVisitor(this, data);
//				--indentationLevel;
//			}
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(UsingStatement usingStatement, object data)
		{
//			Indent();
//			text.Append("using (");
//			usingStatement.UsingStmnt.AcceptVisitor(this,data);
//			text.Append(") {\n");
//			++indentationLevel;
//			usingStatement.EmbeddedStatement.AcceptVisitor(this,data);
//			--indentationLevel;
//			Indent();
//			text.Append("}\n");
			return null;
		}
		
		public object Visit(LocalVariableDeclaration localVariableDeclaration, object data)
		{
//			Indent();
//			VisitModifier(localVariableDeclaration.Modifier);
//			text.Append(" ");
//			Visit(localVariableDeclaration.Type, data);
//			this.AppendCommaSeparatedList(localVariableDeclaration.Variables);
//			text.Append(";\n");
			return null;
		}
		
		public object Visit(StatementExpression statementExpression, object data)
		{
//			Indent();
//			statementExpression.Expression.AcceptVisitor(this, data);
//			text.Append(";\n");
			return null;
		}
		
#region Expressions
		public object Visit(ArrayCreateExpression arrayCreateExpression, object data)
		{
//			text.Append("new ");
//			Visit(arrayCreateExpression.CreateType, null);
//			foreach (object o in arrayCreateExpression.Parameters) {
//				text.Append("[");
//				if (o is int) {
//					int num = (int)o;
//					for (int i = 0; i < num; ++i) {
//						text.Append(",");
//					}
//				} else {
//					((Expression)o).AcceptVisitor(this, null);
//				}
//				text.Append("]");
//			}
//			
//			if (arrayCreateExpression.Parameters.Count == 0) {
//				text.Append("[]");
//			}
//			
//			if (arrayCreateExpression.ArrayInitializer != null) {
//				text.Append(" ");
//				arrayCreateExpression.ArrayInitializer.AcceptVisitor(this, null);
//			}
			return null;
		}
		
		public object Visit(ArrayInitializerExpression arrayCreateExpression, object data)
		{
//			text.Append("{");
//			this.AppendCommaSeparatedList(arrayCreateExpression.CreateExpressions);
//			text.Append("}");
			return null;
		}
		
		public object Visit(AssignmentExpression assignmentExpression, object data)
		{
//			assignmentExpression.Left.AcceptVisitor(this, data);
//			switch (assignmentExpression.Op) {
//				case AssignmentOperatorType.Assign:
//					text.Append(" = ");
//					break;
//				case AssignmentOperatorType.Add:
//					text.Append(" += ");
//					break;
//				case AssignmentOperatorType.Subtract:
//					text.Append(" -= ");
//					break;
//				case AssignmentOperatorType.Multiply:
//					text.Append(" *= ");
//					break;
//				case AssignmentOperatorType.Divide:
//					text.Append(" /= ");
//					break;
//				case AssignmentOperatorType.ShiftLeft:
//					text.Append(" <<= ");
//					break;
//				case AssignmentOperatorType.ShiftRight:
//					text.Append(" >>= ");
//					break;
//				case AssignmentOperatorType.ExclusiveOr:
//					text.Append(" ^= ");
//					break;
//				case AssignmentOperatorType.Modulus:
//					text.Append(" %= ");
//					break;
//				case AssignmentOperatorType.BitwiseAnd:
//					text.Append(" &= ");
//					break;
//				case AssignmentOperatorType.BitwiseOr:
//					text.Append(" |= ");
//					break;
//			}
//			assignmentExpression.Right.AcceptVisitor(this, data);
//			
			return null;
		}
		
		public object Visit(BaseReferenceExpression baseReferenceExpression, object data)
		{
//			text.Append("base");
			return null;
		}
		
		public object Visit(BinaryOperatorExpression binaryOperatorExpression, object data)
		{
//			binaryOperatorExpression.Left.AcceptVisitor(this, data);
//			switch (binaryOperatorExpression.Op) {
//				case BinaryOperatorType.Add:
//					text.Append(" + ");
//					break;
//				
//				case BinaryOperatorType.Subtract:
//					text.Append(" - ");
//					break;
//				
//				case BinaryOperatorType.Multiply:
//					text.Append(" * ");
//					break;
//				
//				case BinaryOperatorType.Divide:
//					text.Append(" / ");
//					break;
//				
//				case BinaryOperatorType.Modulus:
//					text.Append(" % ");
//					break;
//				
//				case BinaryOperatorType.ShiftLeft:
//					text.Append(" << ");
//					break;
//				
//				case BinaryOperatorType.ShiftRight:
//					text.Append(" >> ");
//					break;
//				
//				case BinaryOperatorType.BitwiseAnd:
//					text.Append(" & ");
//					break;
//				case BinaryOperatorType.BitwiseOr:
//					text.Append(" | ");
//					break;
//				case BinaryOperatorType.ExclusiveOr:
//					text.Append(" ^ ");
//					break;
//				
//				case BinaryOperatorType.LogicalAnd:
//					text.Append(" && ");
//					break;
//				case BinaryOperatorType.LogicalOr:
//					text.Append(" || ");
//					break;
//				
//				case BinaryOperatorType.AS:
//					text.Append(" as ");
//					break;
//				case BinaryOperatorType.IS:
//					text.Append(" is ");
//					break;
//				case BinaryOperatorType.Equality:
//					text.Append(" == ");
//					break;
//				case BinaryOperatorType.GreaterThan:
//					text.Append(" > ");
//					break;
//				case BinaryOperatorType.GreaterThanOrEqual:
//					text.Append(" >= ");
//					break;
//				case BinaryOperatorType.InEquality:
//					text.Append(" != ");
//					break;
//				case BinaryOperatorType.LessThan:
//					text.Append(" < ");
//					break;
//				case BinaryOperatorType.LessThanOrEqual:
//					text.Append(" <= ");
//					break;
//			}
//			
//			binaryOperatorExpression.Right.AcceptVisitor(this, data);
			return null;
		}
		
		public object Visit(CastExpression castExpression, object data)
		{
//			text.Append("(");
//			Visit(castExpression.CastTo, data);
//			text.Append(")");
//			castExpression.Expression.AcceptVisitor(this, data);
			return null;
		}
		
		public object Visit(CheckedExpression checkedExpression, object data)
		{
//			text.Append("checked(");
//			checkedExpression.Expression.AcceptVisitor(this, data);
//			text.Append(")");
			return null;
		}
		
		public object Visit(ConditionalExpression conditionalExpression, object data)
		{
//			conditionalExpression.TestCondition.AcceptVisitor(this, data);
//			text.Append(" ? ");
//			conditionalExpression.TrueExpression.AcceptVisitor(this, data);
//			text.Append(" : ");
//			conditionalExpression.FalseExpression.AcceptVisitor(this, data);
			return null;
		}
		
		public object Visit(DirectionExpression directionExpression, object data)
		{
//			switch (directionExpression.FieldDirection) {
//				case FieldDirection.Out:
//					text.Append("out ");
//					break;
//				case FieldDirection.Ref:
//					text.Append("ref ");
//					break;
//			}
//			directionExpression.Expression.AcceptVisitor(this, data);
			return null;
		}
		
		public object Visit(FieldReferenceExpression fieldReferenceExpression, object data)
		{
//			fieldReferenceExpression.TargetObject.AcceptVisitor(this, data);
//			text.Append(".");
//			text.Append(fieldReferenceExpression.FieldName);
			return null;
		}
		
		public object Visit(IdentifierExpression identifierExpression, object data)
		{
//			text.Append(identifierExpression.Identifier);
			return null;
		}
		
		public object Visit(IndexerExpression indexerExpression, object data)
		{
//			indexerExpression.TargetObject.AcceptVisitor(this, data);
//			text.Append("[");
//			AppendCommaSeparatedList(indexerExpression.Indices);
//			text.Append("]");
			return null;
		}
		
		public object Visit(InvocationExpression invocationExpression, object data)
		{
//			invocationExpression.TargetObject.AcceptVisitor(this, data);
//			text.Append("(");
//			AppendCommaSeparatedList(invocationExpression.Parameters);
//			text.Append(")");
			return null;
		}
		
		public object Visit(ObjectCreateExpression objectCreateExpression, object data)
		{
//			text.Append("new ");
//			this.Visit(objectCreateExpression.CreateType, data);
//			text.Append("(");
//			AppendCommaSeparatedList(objectCreateExpression.Parameters);
//			text.Append(")");
			return null;
		}
		
		public object Visit(ParenthesizedExpression parenthesizedExpression, object data)
		{
//			text.Append("(");
//			parenthesizedExpression.Expression.AcceptVisitor(this, data);
//			text.Append(")");
			return null;
		}
		
		public object Visit(PointerReferenceExpression pointerReferenceExpression, object data)
		{
//			pointerReferenceExpression.Expression.AcceptVisitor(this, data);
//			text.Append("->");
//			text.Append(pointerReferenceExpression.Identifier);
			return null;
		}
		
		public object Visit(PrimitiveExpression primitiveExpression, object data)
		{
//			text.Append(primitiveExpression.StringValue);
			return null;
		}
		
		public object Visit(SizeOfExpression sizeOfExpression, object data)
		{
//			text.Append("sizeof(");
//			Visit(sizeOfExpression.TypeReference, data);
//			text.Append(")");
			return null;
		}
		
		public object Visit(StackAllocExpression stackAllocExpression, object data)
		{
//			text.Append("stackalloc ");
//			Visit(stackAllocExpression.Type, data);
//			text.Append("[");
//			stackAllocExpression.Expression.AcceptVisitor(this, data);
//			text.Append("]");
			return null;
		}
		
		public object Visit(ThisReferenceExpression thisReferenceExpression, object data)
		{
//			text.Append("this");
			return null;
		}
		
		public object Visit(TypeOfExpression typeOfExpression, object data)
		{
//			text.Append("typeof(");
//			Visit(typeOfExpression.TypeReference, data);
//			text.Append(")");
			return null;
		}
		
		public object Visit(TypeReferenceExpression typeReferenceExpression, object data)
		{
//			Visit(typeReferenceExpression.TypeReference, data);
			return null;
		}
		
		public object Visit(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
//			switch (unaryOperatorExpression.Op) {
//				case UnaryOperatorType.BitNot:
//					text.Append("~");
//					break;
//				case UnaryOperatorType.Decrement:
//					text.Append("--");
//					break;
//				case UnaryOperatorType.Increment:
//					text.Append("++");
//					break;
//				case UnaryOperatorType.Minus:
//					text.Append("-");
//					break;
//				case UnaryOperatorType.Not:
//					text.Append("!");
//					break;
//				case UnaryOperatorType.Plus:
//					text.Append("+");
//					break;
//				case UnaryOperatorType.PostDecrement:
//					unaryOperatorExpression.Expression.AcceptVisitor(this, data);
//					text.Append("--");
//					return null;
//				case UnaryOperatorType.PostIncrement:
//					unaryOperatorExpression.Expression.AcceptVisitor(this, data);
//					text.Append("++");
//					return null;
//				case UnaryOperatorType.Star:
//					text.Append("*");
//					break;
//				case UnaryOperatorType.BitWiseAnd:
//					text.Append("&");
//					break;
//			}
//			unaryOperatorExpression.Expression.AcceptVisitor(this, data);
			return null;
		}
		
		public object Visit(UncheckedExpression uncheckedExpression, object data)
		{
//			text.Append("unchecked(");
//			uncheckedExpression.Expression.AcceptVisitor(this, data);
//			text.Append(")");
			return null;
		}
#endregion
	}
}
