// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 3662 $</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory.Ast;

namespace ICSharpCode.NRefactory.Visitors
{
	public sealed class LocalLookupVariable
	{
		public readonly string Name;
		public readonly TypeReference TypeRef;
		public readonly Location StartPos;
		public readonly Location EndPos;
		public readonly bool IsConst;
		public readonly bool IsLoopVariable;
		public readonly Expression Initializer;
		public readonly LambdaExpression ParentLambdaExpression;
		
		public LocalLookupVariable(string name, TypeReference typeRef, Location startPos, Location endPos, bool isConst, bool isLoopVariable, Expression initializer, LambdaExpression parentLambdaExpression)
		{
			this.Name = name;
			this.TypeRef = typeRef;
			this.StartPos = startPos;
			this.EndPos = endPos;
			this.IsConst = isConst;
			this.IsLoopVariable = isLoopVariable;
			this.Initializer = initializer;
			this.ParentLambdaExpression = parentLambdaExpression;
		}
	}
	
	public sealed class LookupTableVisitor : AbstractAstVisitor
	{
		Dictionary<string, List<LocalLookupVariable>> variables;
		SupportedLanguage language;
		
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public Dictionary<string, List<LocalLookupVariable>> Variables {
			get {
				return variables;
			}
		}
		
		List<WithStatement> withStatements = new List<WithStatement>();
		
		public List<WithStatement> WithStatements {
			get {
				return withStatements;
			}
		}
		
		public LookupTableVisitor(SupportedLanguage language)
		{
			this.language = language;
			if (language == SupportedLanguage.VBNet) {
				variables = new Dictionary<string, List<LocalLookupVariable>>(StringComparer.InvariantCultureIgnoreCase);
			} else {
				variables = new Dictionary<string, List<LocalLookupVariable>>(StringComparer.InvariantCulture);
			}
		}
		
		public void AddVariable(TypeReference typeRef, string name,
		                        Location startPos, Location endPos, bool isConst,
		                        bool isLoopVariable, Expression initializer,
		                        LambdaExpression parentLambdaExpression)
		{
			if (name == null || name.Length == 0) {
				return;
			}
			List<LocalLookupVariable> list;
			if (!variables.ContainsKey(name)) {
				variables[name] = list = new List<LocalLookupVariable>();
			} else {
				list = (List<LocalLookupVariable>)variables[name];
			}
			list.Add(new LocalLookupVariable(name, typeRef, startPos, endPos, isConst, isLoopVariable, initializer, parentLambdaExpression));
		}
		
		public override object VisitWithStatement(WithStatement withStatement, object data)
		{
			withStatements.Add(withStatement);
			return base.VisitWithStatement(withStatement, data);
		}
		
		Stack<Location> endLocationStack = new Stack<Location>();
		
		Location CurrentEndLocation {
			get {
				return (endLocationStack.Count == 0) ? Location.Empty : endLocationStack.Peek();
			}
		}
		
		public override object VisitBlockStatement(BlockStatement blockStatement, object data)
		{
			endLocationStack.Push(blockStatement.EndLocation);
			base.VisitBlockStatement(blockStatement, data);
			endLocationStack.Pop();
			return null;
		}
		
		public override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			for (int i = 0; i < localVariableDeclaration.Variables.Count; ++i) {
				VariableDeclaration varDecl = (VariableDeclaration)localVariableDeclaration.Variables[i];
				
				AddVariable(localVariableDeclaration.GetTypeForVariable(i),
				            varDecl.Name,
				            localVariableDeclaration.StartLocation,
				            CurrentEndLocation,
				            (localVariableDeclaration.Modifier & Modifiers.Const) == Modifiers.Const,
				            false, varDecl.Initializer, null);
			}
			return base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
		}
		
		public override object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data)
		{
			foreach (ParameterDeclarationExpression p in anonymousMethodExpression.Parameters) {
				AddVariable(p.TypeReference, p.ParameterName,
				            anonymousMethodExpression.StartLocation, anonymousMethodExpression.EndLocation,
				            false, false, null, null);
			}
			return base.VisitAnonymousMethodExpression(anonymousMethodExpression, data);
		}
		
		public override object VisitLambdaExpression(LambdaExpression lambdaExpression, object data)
		{
			foreach (ParameterDeclarationExpression p in lambdaExpression.Parameters) {
				AddVariable(p.TypeReference, p.ParameterName,
				            lambdaExpression.StartLocation, lambdaExpression.ExtendedEndLocation,
				            false, false, null, lambdaExpression);
			}
			return base.VisitLambdaExpression(lambdaExpression, data);
		}
		
		public override object VisitQueryExpression(QueryExpression queryExpression, object data)
		{
			endLocationStack.Push(GetQueryVariableEndScope(queryExpression));
			base.VisitQueryExpression(queryExpression, data);
			endLocationStack.Pop();
			return null;
		}
		
		Location GetQueryVariableEndScope(QueryExpression queryExpression)
		{
			return queryExpression.EndLocation;
		}
		
		public override object VisitQueryExpressionFromClause(QueryExpressionFromClause fromClause, object data)
		{
			AddVariable(fromClause.Type, fromClause.Identifier,
			            fromClause.StartLocation, CurrentEndLocation,
			            false, true, fromClause.InExpression, null);
			return base.VisitQueryExpressionFromClause(fromClause, data);
		}
		
		public override object VisitQueryExpressionJoinClause(QueryExpressionJoinClause joinClause, object data)
		{
			if (string.IsNullOrEmpty(joinClause.IntoIdentifier)) {
				AddVariable(joinClause.Type, joinClause.Identifier,
				            joinClause.StartLocation, CurrentEndLocation,
				            false, true, joinClause.InExpression, null);
			} else {
				AddVariable(joinClause.Type, joinClause.Identifier,
				            joinClause.StartLocation, joinClause.EndLocation,
				            false, true, joinClause.InExpression, null);
				
				AddVariable(joinClause.Type, joinClause.IntoIdentifier,
				            joinClause.StartLocation, CurrentEndLocation,
				            false, false, joinClause.InExpression, null);
			}
			return base.VisitQueryExpressionJoinClause(joinClause, data);
		}
		
		public override object VisitQueryExpressionLetClause(QueryExpressionLetClause letClause, object data)
		{
			AddVariable(null, letClause.Identifier,
			            letClause.StartLocation, CurrentEndLocation,
			            false, false, letClause.Expression, null);
			return base.VisitQueryExpressionLetClause(letClause, data);
		}
		
		public override object VisitForNextStatement(ForNextStatement forNextStatement, object data)
		{
			if (forNextStatement.EmbeddedStatement.EndLocation.IsEmpty) {
				return base.VisitForNextStatement(forNextStatement, data);
			} else {
				endLocationStack.Push(forNextStatement.EmbeddedStatement.EndLocation);
				AddVariable(forNextStatement.TypeReference,
				            forNextStatement.VariableName,
				            forNextStatement.StartLocation,
				            forNextStatement.EndLocation,
				            false, false,
				            forNextStatement.Start,
				            null);
				
				base.VisitForNextStatement(forNextStatement, data);
				
				endLocationStack.Pop();
				return null;
			}
		}
		
		public override object VisitFixedStatement(FixedStatement fixedStatement, object data)
		{
			// uses LocalVariableDeclaration, we just have to put the end location on the stack
			if (fixedStatement.EmbeddedStatement.EndLocation.IsEmpty) {
				return base.VisitFixedStatement(fixedStatement, data);
			} else {
				endLocationStack.Push(fixedStatement.EmbeddedStatement.EndLocation);
				base.VisitFixedStatement(fixedStatement, data);
				endLocationStack.Pop();
				return null;
			}
		}
		
		public override object VisitForStatement(ForStatement forStatement, object data)
		{
			// uses LocalVariableDeclaration, we just have to put the end location on the stack
			if (forStatement.EmbeddedStatement.EndLocation.IsEmpty) {
				return base.VisitForStatement(forStatement, data);
			} else {
				endLocationStack.Push(forStatement.EmbeddedStatement.EndLocation);
				base.VisitForStatement(forStatement, data);
				endLocationStack.Pop();
				return null;
			}
		}
		
		public override object VisitUsingStatement(UsingStatement usingStatement, object data)
		{
			// uses LocalVariableDeclaration, we just have to put the end location on the stack
			if (usingStatement.EmbeddedStatement.EndLocation.IsEmpty) {
				return base.VisitUsingStatement(usingStatement, data);
			} else {
				endLocationStack.Push(usingStatement.EmbeddedStatement.EndLocation);
				base.VisitUsingStatement(usingStatement, data);
				endLocationStack.Pop();
				return null;
			}
		}
		
		public override object VisitSwitchSection(SwitchSection switchSection, object data)
		{
			if (language == SupportedLanguage.VBNet) {
				return VisitBlockStatement(switchSection, data);
			} else {
				return base.VisitSwitchSection(switchSection, data);
			}
		}
		
		public override object VisitForeachStatement(ForeachStatement foreachStatement, object data)
		{
			AddVariable(foreachStatement.TypeReference,
			            foreachStatement.VariableName,
			            foreachStatement.StartLocation,
			            foreachStatement.EndLocation,
			            false, true,
			            foreachStatement.Expression,
			            null);
			
			if (foreachStatement.Expression != null) {
				foreachStatement.Expression.AcceptVisitor(this, data);
			}
			if (foreachStatement.EmbeddedStatement == null) {
				return data;
			}
			return foreachStatement.EmbeddedStatement.AcceptVisitor(this, data);
		}
		
		public override object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data)
		{
			if (tryCatchStatement == null) {
				return data;
			}
			if (tryCatchStatement.StatementBlock != null) {
				tryCatchStatement.StatementBlock.AcceptVisitor(this, data);
			}
			if (tryCatchStatement.CatchClauses != null) {
				foreach (CatchClause catchClause in tryCatchStatement.CatchClauses) {
					if (catchClause != null) {
						if (catchClause.TypeReference != null && catchClause.VariableName != null) {
							AddVariable(catchClause.TypeReference,
							            catchClause.VariableName,
							            catchClause.StatementBlock.StartLocation,
							            catchClause.StatementBlock.EndLocation,
							            false, false, null, null);
						}
						catchClause.StatementBlock.AcceptVisitor(this, data);
					}
				}
			}
			if (tryCatchStatement.FinallyBlock != null) {
				return tryCatchStatement.FinallyBlock.AcceptVisitor(this, data);
			}
			return data;
		}
	}
}
