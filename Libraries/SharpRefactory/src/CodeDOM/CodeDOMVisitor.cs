// CodeDOMVisitor.cs
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
using System.Reflection;
using System.CodeDom;
using System.Text;
using System.Collections;

using ICSharpCode.SharpRefactory.Parser.AST;

namespace ICSharpCode.SharpRefactory.Parser
{
	public class CodeDOMVisitor : IASTVisitor
	{
		Stack namespaceDeclarations = new Stack();
		Stack typeDeclarations     = new Stack();
		CodeMemberMethod currentMethod = null;
		TypeDeclaration currentTypeDeclaration;
		
		public CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
		public ArrayList namespaces = new ArrayList();
		
		static string[,] typeConversionList = new string[,] {
			{"System.Void",    "void"},
			{"System.Object",  "object"},
			{"System.Boolean", "bool"},
			{"System.Byte",    "byte"},
			{"System.SByte",   "sbyte"},
			{"System.Char",    "char"},
			{"System.Enum",    "enum"},
			{"System.Int16",   "short"},
			{"System.Int32",   "int"},
			{"System.Int64",   "long"},
			{"System.UInt16",  "ushort"},
			{"System.UInt32",  "uint"},
			{"System.UInt64",  "ulong"},
			{"System.Single",  "float"},
			{"System.Double",  "double"},
			{"System.Decimal", "decimal"},
			{"System.String",  "string"}
		};
		
		static Hashtable typeConversionTable = new Hashtable();
		
		static CodeDOMVisitor()
		{
			for (int i = 0; i < typeConversionList.GetLength(0); ++i) {
				typeConversionTable[typeConversionList[i, 1]] = typeConversionList[i, 0];
			}
		}
		string ConvType(string type) 
		{
			if (typeConversionTable[type] != null) {
				return typeConversionTable[type].ToString();
			}
			return type;
		}

#region ICSharpCode.SharpRefactory.Parser.IASTVisitor interface implementation
		public object Visit(INode node, object data)
		{
			return null;
		}
		
		public object Visit(CompilationUnit compilationUnit, object data)
		{
			CodeNamespace globalNamespace = new CodeNamespace("Global");
			namespaces.Add(globalNamespace);
			namespaceDeclarations.Push(globalNamespace);
			compilationUnit.AcceptChildren(this, data);
			codeCompileUnit.Namespaces.Add(globalNamespace);
			return globalNamespace;
		}
		
		public object Visit(NamespaceDeclaration namespaceDeclaration, object data)
		{
			CodeNamespace currentNamespace = new CodeNamespace(namespaceDeclaration.NameSpace);
			namespaces.Add(currentNamespace);
			// add imports from mother namespace
			foreach (CodeNamespaceImport import in ((CodeNamespace)namespaceDeclarations.Peek()).Imports) {
				currentNamespace.Imports.Add(import);
			}
			namespaceDeclarations.Push(currentNamespace);
			namespaceDeclaration.AcceptChildren(this, data);
			namespaceDeclarations.Pop();
			codeCompileUnit.Namespaces.Add(currentNamespace);
			
			// TODO : Nested namespaces allowed in CodeDOM ? Doesn't seem so :(
			return null;
		}
		
		public object Visit(UsingDeclaration usingDeclaration, object data)
		{
			((CodeNamespace)namespaceDeclarations.Peek()).Imports.Add(new CodeNamespaceImport(usingDeclaration.Namespace));
			return null;
		}
		
		public object Visit(UsingAliasDeclaration usingAliasDeclaration, object data)
		{
			return null;
		}
		
		public object Visit(AttributeSection attributeSection, object data)
		{
			return null;
		}
		
		public object Visit(TypeDeclaration typeDeclaration, object data)
		{
			this.currentTypeDeclaration = typeDeclaration;
			CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(typeDeclaration.Name);
			codeTypeDeclaration.IsClass     = typeDeclaration.Type == Types.Class;
			codeTypeDeclaration.IsEnum      = typeDeclaration.Type == Types.Enum;
			codeTypeDeclaration.IsInterface = typeDeclaration.Type == Types.Interface;
			codeTypeDeclaration.IsStruct    = typeDeclaration.Type == Types.Struct;
			
			if (typeDeclaration.BaseTypes != null) {
				foreach (object o in typeDeclaration.BaseTypes) {
					codeTypeDeclaration.BaseTypes.Add(new CodeTypeReference(o.ToString()));
				}
			}
			
			typeDeclarations.Push(codeTypeDeclaration);
			typeDeclaration.AcceptChildren(this,data);
//			((INode)typeDeclaration.Children[0]).(this, data);
			
			typeDeclarations.Pop();
			
			((CodeNamespace)namespaceDeclarations.Peek()).Types.Add(codeTypeDeclaration);
			
			return null;
		}
		
		public object Visit(DelegateDeclaration delegateDeclaration, object data)
		{
//			CodeTypeDelegate codeTypeDelegate = new CodeTypeDelegate(delegateDeclaration.Name);
//			codeTypeDelegate.Parameters
//			
//			((CodeNamespace)namespaceDeclarations.Peek()).Types.Add(codeTypeDelegate);
			return null;
		}
		
		public object Visit(VariableDeclaration variableDeclaration, object data)
		{
			return null;
		}
		
		public object Visit(FieldDeclaration fieldDeclaration, object data)
		{
			for (int i = 0; i < fieldDeclaration.Fields.Count; ++i) {
				VariableDeclaration field = (VariableDeclaration)fieldDeclaration.Fields[i];
				
				CodeMemberField memberField = new CodeMemberField(new CodeTypeReference(ConvType(fieldDeclaration.TypeReference.Type)), field.Name);
				if (field.Initializer != null) {
					memberField.InitExpression =  (CodeExpression)((INode)field.Initializer).AcceptVisitor(this, data);
				}
				
				((CodeTypeDeclaration)typeDeclarations.Peek()).Members.Add(memberField);
			}
			
			return null;
		}
		
		public object Visit(MethodDeclaration methodDeclaration, object data)
		{
			CodeMemberMethod memberMethod = new CodeMemberMethod();
			memberMethod.Name = methodDeclaration.Name;
			currentMethod = memberMethod;
			((CodeTypeDeclaration)typeDeclarations.Peek()).Members.Add(memberMethod);
//			if (memberMethod.Name == "InitializeComponent") {
				methodDeclaration.Body.AcceptChildren(this, data);
//			}
			currentMethod = null;
			return null;
		}
		
		public object Visit(PropertyDeclaration propertyDeclaration, object data)
		{
			return null;
		}
		
		public object Visit(PropertyGetRegion propertyGetRegion, object data)
		{
			return null;
		}
		
		public object Visit(PropertySetRegion PropertySetRegion, object data)
		{
			return null;
		}
		
		public object Visit(EventDeclaration eventDeclaration, object data)
		{
			return null;
		}
		
		public object Visit(EventAddRegion eventAddRegion, object data)
		{
			return null;
		}
		
		public object Visit(EventRemoveRegion eventRemoveRegion, object data)
		{
			return null;
		}
		
		public object Visit(ConstructorDeclaration constructorDeclaration, object data)
		{
			CodeMemberMethod memberMethod = new CodeConstructor();
			currentMethod = memberMethod;
			((CodeTypeDeclaration)typeDeclarations.Peek()).Members.Add(memberMethod);
//			constructorDeclaration.AcceptChildren(this, data);
			currentMethod = null;
			return null;
		}
		
		public object Visit(DestructorDeclaration destructorDeclaration, object data)
		{
			return null;
		}
		
		public object Visit(OperatorDeclaration operatorDeclaration, object data)
		{
			return null;
		}
		
		public object Visit(IndexerDeclaration indexerDeclaration, object data)
		{
			return null;
		}
		
		public object Visit(BlockStatement blockStatement, object data)
		{
			blockStatement.AcceptChildren(this, data);
			return null;
		}
		
		public object Visit(StatementExpression statementExpression, object data)
		{
			CodeExpression expr = (CodeExpression)statementExpression.Expression.AcceptVisitor(this, data);
			if (expr == null) {
				if (!(statementExpression.Expression is AssignmentExpression)) {
					Console.WriteLine("NULL EXPRESSION : " + statementExpression.Expression);
				}
			} else {
				currentMethod.Statements.Add(new CodeExpressionStatement(expr));
			}
			
			return null;
		}
		
		public string Convert(TypeReference typeRef)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(ConvType(typeRef.Type));
			
			for (int i = 0; i < typeRef.PointerNestingLevel; ++i) {
				builder.Append('*');
			}
			
			
			for (int i = 0; i < typeRef.RankSpecifier.Length; ++i) {
				builder.Append('[');
				for (int j = 1; j < typeRef.RankSpecifier[i]; ++j) {
					builder.Append(',');
				}
				builder.Append(']');
			}
			
			return builder.ToString();
		}
		
		public object Visit(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			CodeTypeReference type = new CodeTypeReference(Convert(localVariableDeclaration.Type));
			
			foreach (VariableDeclaration var in localVariableDeclaration.Variables) {
				if (var.Initializer != null) {
					currentMethod.Statements.Add(new CodeVariableDeclarationStatement(type,
					                                                                  var.Name,
					                                                                  (CodeExpression)((INode)var.Initializer).AcceptVisitor(this, data)));
				} else {
					currentMethod.Statements.Add(new CodeVariableDeclarationStatement(type,
					                                                                  var.Name));
				}
			}
			return null;
		}
		
		public object Visit(EmptyStatement emptyStatement, object data)
		{
			return null;
		}
		
		public object Visit(ReturnStatement returnStatement, object data)
		{
			return null;
		}
		
		public object Visit(IfStatement ifStatement, object data)
		{
			return null;
		}
		
		public object Visit(IfElseStatement ifElseStatement, object data)
		{
			return null;
		}
		
		public object Visit(WhileStatement whileStatement, object data)
		{
			return null;
		}
		
		public object Visit(DoWhileStatement doWhileStatement, object data)
		{
			return null;
		}
		
		public object Visit(ForStatement forStatement, object data)
		{
			return null;
		}
		
		public object Visit(LabelStatement labelStatement, object data)
		{
			return null;
		}
		
		public object Visit(GotoStatement gotoStatement, object data)
		{
			return null;
		}
		
		public object Visit(SwitchStatement switchStatement, object data)
		{
			return null;
		}
		
		public object Visit(BreakStatement breakStatement, object data)
		{
			return null;
		}
		
		public object Visit(ContinueStatement continueStatement, object data)
		{
			return null;
		}
		
		public object Visit(GotoCaseStatement gotoCaseStatement, object data)
		{
			return null;
		}
		
		public object Visit(ForeachStatement foreachStatement, object data)
		{
			return null;
		}
		
		public object Visit(LockStatement lockStatement, object data)
		{
			return null;
		}
		
		public object Visit(UsingStatement usingStatement, object data)
		{
			return null;
		}
		
		public object Visit(TryCatchStatement tryCatchStatement, object data)
		{
			return null;
		}
		
		public object Visit(ThrowStatement throwStatement, object data)
		{
			return new CodeThrowExceptionStatement((CodeExpression)throwStatement.ThrowExpression.AcceptVisitor(this, data));
		}
		
		public object Visit(FixedStatement fixedStatement, object data)
		{
			return null;
		}
		
		public object Visit(PrimitiveExpression expression, object data)
		{
//			if (expression.Value is string) {
//				return new CodePrimitiveExpression(expression.Value);
//			} else if (expression.Value is char) {
//				return new CodePrimitiveExpression((char)expression.Value);
//			} else if (expression.Value == null) {
//				return new CodePrimitiveExpression(null);
//			}
			return new CodePrimitiveExpression(expression.Value);
		}
		
		public object Visit(BinaryOperatorExpression expression, object data)
		{
			CodeBinaryOperatorType op = CodeBinaryOperatorType.Add;
			switch (expression.Op) {
				case BinaryOperatorType.Add:
					op = CodeBinaryOperatorType.Add;
					break;
				case BinaryOperatorType.BitwiseAnd:
					op = CodeBinaryOperatorType.BitwiseAnd;
					break;
				case BinaryOperatorType.BitwiseOr:
					op = CodeBinaryOperatorType.BitwiseOr;
					break;
				case BinaryOperatorType.LogicalAnd:
					op = CodeBinaryOperatorType.BooleanAnd;
					break;
				case BinaryOperatorType.LogicalOr:
					op = CodeBinaryOperatorType.BooleanOr;
					break;
				case BinaryOperatorType.Divide:
					op = CodeBinaryOperatorType.Divide;
					break;
				case BinaryOperatorType.GreaterThan:
					op = CodeBinaryOperatorType.GreaterThan;
					break;
				case BinaryOperatorType.GreaterThanOrEqual:
					op = CodeBinaryOperatorType.GreaterThanOrEqual;
					break;
				case BinaryOperatorType.Equality:
					op = CodeBinaryOperatorType.IdentityEquality;
					break;
				case BinaryOperatorType.InEquality:
					op = CodeBinaryOperatorType.IdentityInequality;
					break;
				case BinaryOperatorType.LessThan:
					op = CodeBinaryOperatorType.LessThan;
					break;
				case BinaryOperatorType.LessThanOrEqual:
					op = CodeBinaryOperatorType.LessThanOrEqual;
					break;
				case BinaryOperatorType.Modulus:
					op = CodeBinaryOperatorType.Modulus;
					break;
				case BinaryOperatorType.Multiply:
					op = CodeBinaryOperatorType.Multiply;
					break;
				case BinaryOperatorType.Subtract:
					op = CodeBinaryOperatorType.Subtract;
					break;
				case BinaryOperatorType.ValueEquality:
					op = CodeBinaryOperatorType.ValueEquality;
					break;
				case BinaryOperatorType.ShiftLeft:
					// CodeDOM suxx
					op = CodeBinaryOperatorType.Multiply;
					break;
				case BinaryOperatorType.ShiftRight:
					// CodeDOM suxx
					op = CodeBinaryOperatorType.Multiply;
					break;
				case BinaryOperatorType.IS:
					op = CodeBinaryOperatorType.IdentityEquality;
					break;
				case BinaryOperatorType.AS:
					op = CodeBinaryOperatorType.IdentityEquality;
					break;
				case BinaryOperatorType.ExclusiveOr:
					// CodeDOM suxx
					op = CodeBinaryOperatorType.BitwiseAnd;
					break;
			}
			return new CodeBinaryOperatorExpression((CodeExpression)expression.Left.AcceptVisitor(this, data),
			                                        op,
			                                        (CodeExpression)expression.Right.AcceptVisitor(this, data));
		}
		
		public object Visit(ParenthesizedExpression expression, object data)
		{
			return expression.Expression.AcceptVisitor(this, data);
		}
		
		public object Visit(InvocationExpression invocationExpression, object data)
		{
			Expression     target     = invocationExpression.TargetObject;
			CodeExpression targetExpr;
			string         methodName = null;
			if (target == null) {
				targetExpr = new CodeThisReferenceExpression();
			} else if (target is FieldReferenceExpression) {
				FieldReferenceExpression fRef = (FieldReferenceExpression)target;
				targetExpr = (CodeExpression)fRef.TargetObject.AcceptVisitor(this, data);
				methodName = fRef.FieldName;
			} else {
				targetExpr = (CodeExpression)target.AcceptVisitor(this, data);
			}
			return new CodeMethodInvokeExpression(targetExpr, methodName, GetExpressionList(invocationExpression.Parameters));
		}
		
		public object Visit(IdentifierExpression expression, object data)
		{
			if (IsField(expression.Identifier)) {
				return new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
				                                        expression.Identifier);
			}
			return new CodeVariableReferenceExpression(expression.Identifier);
		}
		
		public object Visit(TypeReferenceExpression typeReferenceExpression, object data)
		{
			return null;
		}
		
		public object Visit(UnaryOperatorExpression unaryOperatorExpression, object data)
		{
			switch (unaryOperatorExpression.Op) {
				case UnaryOperatorType.Minus:
					if (unaryOperatorExpression.Expression is PrimitiveExpression) {
						PrimitiveExpression expression = (PrimitiveExpression)unaryOperatorExpression.Expression;
						if (expression.Value is int) {
							return new CodePrimitiveExpression(- (int)expression.Value);
						}
						if (expression.Value is long) {
							return new CodePrimitiveExpression(- (long)expression.Value);
						}
						if (expression.Value is double) {
							return new CodePrimitiveExpression(- (double)expression.Value);
						}
						if (expression.Value is float) {
							return new CodePrimitiveExpression(- (float)expression.Value);
						}
						
					} 
					return  new CodeBinaryOperatorExpression(new CodePrimitiveExpression(0),
			                                        CodeBinaryOperatorType.Subtract,
			                                        (CodeExpression)unaryOperatorExpression.Expression.AcceptVisitor(this, data));
				case UnaryOperatorType.Plus:
					return unaryOperatorExpression.Expression.AcceptVisitor(this, data);
			}
			return null;
		}
		bool methodReference = false;
		public object Visit(AssignmentExpression assignmentExpression, object data)
		{
			if (assignmentExpression.Op == AssignmentOperatorType.Add) {
				
				methodReference = true;
				CodeExpression methodInvoker = (CodeExpression)assignmentExpression.Right.AcceptVisitor(this, null);
				methodReference = false;
					
				if (assignmentExpression.Left is IdentifierExpression) {
					currentMethod.Statements.Add(new CodeAttachEventStatement(new CodeEventReferenceExpression(new CodeThisReferenceExpression(), ((IdentifierExpression)assignmentExpression.Left).Identifier),
					                                                          methodInvoker));
				} else {
					FieldReferenceExpression fr = (FieldReferenceExpression)assignmentExpression.Left;
					
					currentMethod.Statements.Add(new CodeAttachEventStatement(new CodeEventReferenceExpression((CodeExpression)fr.TargetObject.AcceptVisitor(this, data), fr.FieldName),
					                                                          methodInvoker));
				}
			} else {
				if (assignmentExpression.Left is IdentifierExpression) {
					currentMethod.Statements.Add(new CodeAssignStatement((CodeExpression)assignmentExpression.Left.AcceptVisitor(this, null), (CodeExpression)assignmentExpression.Right.AcceptVisitor(this, null)));
				} else {
					currentMethod.Statements.Add(new CodeAssignStatement((CodeExpression)assignmentExpression.Left.AcceptVisitor(this, null), (CodeExpression)assignmentExpression.Right.AcceptVisitor(this, null)));
					
				}
			}
			return null;
		}
		public virtual object Visit(CheckedStatement checkedStatement, object data)
		{
			return null;
		}
		public virtual object Visit(UncheckedStatement uncheckedStatement, object data)
		{
			return null;
		}
		
		public object Visit(SizeOfExpression sizeOfExpression, object data)
		{
			return null;
		}
		
		public object Visit(TypeOfExpression typeOfExpression, object data)
		{
			return new CodeTypeOfExpression(ConvType(typeOfExpression.TypeReference.Type));
		}
		
		public object Visit(CheckedExpression checkedExpression, object data)
		{
			return null;
		}
		
		public object Visit(UncheckedExpression uncheckedExpression, object data)
		{
			return null;
		}
		
		public object Visit(PointerReferenceExpression pointerReferenceExpression, object data)
		{
			return null;
		}
		
		public object Visit(CastExpression castExpression, object data)
		{
			string typeRef = castExpression.CastTo.Type;
			
			
			return new CodeCastExpression(ConvType(typeRef), (CodeExpression)castExpression.Expression.AcceptVisitor(this, data));
		}
		
		public object Visit(StackAllocExpression stackAllocExpression, object data)
		{
			// TODO
			return null;
		}
		
		public object Visit(IndexerExpression indexerExpression, object data)
		{
			return new CodeIndexerExpression((CodeExpression)indexerExpression.TargetObject.AcceptVisitor(this, data), GetExpressionList(indexerExpression.Indices));
		}
		
		public object Visit(ThisReferenceExpression thisReferenceExpression, object data)
		{
			return new CodeThisReferenceExpression();
		}
		
		public object Visit(BaseReferenceExpression baseReferenceExpression, object data)
		{
			return new CodeBaseReferenceExpression();
		}
		
		public object Visit(ArrayCreateExpression arrayCreateExpression, object data)
		{
			if (arrayCreateExpression.ArrayInitializer == null) {
				if (arrayCreateExpression.Rank != null && arrayCreateExpression.Rank.Length > 0) {
					return new CodeArrayCreateExpression(ConvType(arrayCreateExpression.CreateType.Type),
					                                     arrayCreateExpression.Rank[0]);
				}
				return new CodeArrayCreateExpression(ConvType(arrayCreateExpression.CreateType.Type),
				                                     0);
			}
			return new CodeArrayCreateExpression(ConvType(arrayCreateExpression.CreateType.Type),
			                                     GetExpressionList(arrayCreateExpression.ArrayInitializer.CreateExpressions));
		}
		
		public object Visit(ObjectCreateExpression objectCreateExpression, object data)
		{
			return new CodeObjectCreateExpression(ConvType(objectCreateExpression.CreateType.Type),
			                                      objectCreateExpression.Parameters == null ? null : GetExpressionList(objectCreateExpression.Parameters));
		}
		
		public object Visit(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			return new CodeParameterDeclarationExpression(new CodeTypeReference(ConvType(parameterDeclarationExpression.TypeReference.Type)), parameterDeclarationExpression.ParameterName);
		}
		
		bool IsField(string type, string fieldName)
		{
			Type t       = null;
			Assembly asm = null;
			
			t = this.GetType(type);
			if (t == null)
			{
				asm = typeof(System.Drawing.Point).Assembly;
				t = asm.GetType(type);
			}
			
			if (t == null) {
				asm = typeof(System.Windows.Forms.Control).Assembly;
				t = asm.GetType(type);
			}
			
			if (t == null) {
				asm = typeof(System.String).Assembly;
				t = asm.GetType(type);
			}
			
			return t != null && t.GetField(fieldName) != null;
		}
		
		bool IsFieldReferenceExpression(FieldReferenceExpression fieldReferenceExpression)
		{
			if (fieldReferenceExpression.TargetObject is ThisReferenceExpression) {
				foreach (object o in this.currentTypeDeclaration.Children) {
					if (o is FieldDeclaration) {
						FieldDeclaration fd = (FieldDeclaration)o;
						foreach (VariableDeclaration field in fd.Fields) {
							if (fieldReferenceExpression.FieldName == field.Name) {
								return true;
							}
						}
					}
				}
			}
			return false; //Char.IsLower(fieldReferenceExpression.FieldName[0]);
		}
		
		public object Visit(FieldReferenceExpression fieldReferenceExpression, object data)
		{
			if (methodReference) {
				return new CodeMethodReferenceExpression((CodeExpression)fieldReferenceExpression.TargetObject.AcceptVisitor(this, data), fieldReferenceExpression.FieldName);
			}
			if (IsFieldReferenceExpression(fieldReferenceExpression)) {
				return new CodeFieldReferenceExpression((CodeExpression)fieldReferenceExpression.TargetObject.AcceptVisitor(this, data),
				                                        fieldReferenceExpression.FieldName);
			} else {
				if (fieldReferenceExpression.TargetObject is FieldReferenceExpression) {
					if (IsQualIdent((FieldReferenceExpression)fieldReferenceExpression.TargetObject)) {
						CodeTypeReferenceExpression typeRef = ConvertToIdentifier((FieldReferenceExpression)fieldReferenceExpression.TargetObject);
						if (IsField(typeRef.Type.BaseType, fieldReferenceExpression.FieldName)) {
							return new CodeFieldReferenceExpression(typeRef,
							                                           fieldReferenceExpression.FieldName);
						} else {
							return new CodePropertyReferenceExpression(typeRef,
							                                           fieldReferenceExpression.FieldName);
						}
					}
				}
				
				CodeExpression codeExpression = (CodeExpression)fieldReferenceExpression.TargetObject.AcceptVisitor(this, data);
				return new CodePropertyReferenceExpression(codeExpression,
				                                           fieldReferenceExpression.FieldName);
			}
		}
		public object Visit(DirectionExpression directionExpression, object data)
		{
			return null;
		}
		public object Visit(ArrayInitializerExpression arrayInitializerExpression, object data)
		{
			return null;
		}
		public object Visit(ConditionalExpression conditionalExpression, object data)
		{
			return null;
		}
#endregion
		bool IsQualIdent(FieldReferenceExpression fieldReferenceExpression)
		{
			while (fieldReferenceExpression.TargetObject is FieldReferenceExpression) {
				fieldReferenceExpression = (FieldReferenceExpression)fieldReferenceExpression.TargetObject;
			}
			return fieldReferenceExpression.TargetObject is IdentifierExpression;
		}
		
		bool IsField(string identifier)
		{
			foreach (INode node in currentTypeDeclaration.Children) {
				if (node is FieldDeclaration) {
					FieldDeclaration fd = (FieldDeclaration)node;
					if (fd.GetVariableDeclaration(identifier) != null) {
						return true;
					}
				}
			}
			return false;
		}
		
		CodeTypeReferenceExpression ConvertToIdentifier(FieldReferenceExpression fieldReferenceExpression)
		{
//			CodeFieldReferenceExpression  cpre = new CodeFieldReferenceExpression (); 
//			CodeFieldReferenceExpression firstCpre = cpre,newCpre;
			string type = String.Empty;
			
			while (fieldReferenceExpression.TargetObject is FieldReferenceExpression) {
//				newCpre = new CodeFieldReferenceExpression(); 
//				Console.WriteLine(fieldReferenceExpression.FieldName);
//				cpre.FieldName  = fieldReferenceExpression.FieldName;
//				cpre.TargetObject = newCpre;
//				cpre = newCpre;
				type = "."  + fieldReferenceExpression.FieldName + type;
				fieldReferenceExpression = (FieldReferenceExpression)fieldReferenceExpression.TargetObject;
			}
			type = "."  + fieldReferenceExpression.FieldName + type;
//			newCpre = new CodeFieldReferenceExpression(); 
//			Console.WriteLine(fieldReferenceExpression.FieldName);
//			cpre.FieldName  = fieldReferenceExpression.FieldName;
//			cpre.TargetObject = newCpre;
//			cpre = newCpre;
				
			if (fieldReferenceExpression.TargetObject is IdentifierExpression) {
				return new CodeTypeReferenceExpression(((IdentifierExpression)fieldReferenceExpression.TargetObject).Identifier + type);
//				cpre.TargetObject =
//				return firstCpre;
			} else {
				throw new Exception();
			}
		}
		
		CodeExpression[] GetExpressionList(ArrayList expressionList)
		{
			if (expressionList == null) {
				return new CodeExpression[0];
			}
			CodeExpression[] list = new CodeExpression[expressionList.Count];
			for (int i = 0; i < expressionList.Count; ++i) {
				list[i] = (CodeExpression)((Expression)expressionList[i]).AcceptVisitor(this, null);
				if (list[i] == null) {
					list[i] = new CodePrimitiveExpression(0);
				}
			}
			return list;
		}

		Type GetType(string typeName)
		{
			foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) 
			{
				Type type = asm.GetType(typeName);
				if (type != null) 
				{
					return type;
				}
			}
			return Type.GetType(typeName);
		}

	}
}
