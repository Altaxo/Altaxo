using System;
using System.Drawing;
using System.Collections;

using ICSharpCode.SharpRefactory.Parser.AST;

namespace ICSharpCode.SharpRefactory.Parser
{
	public class LocalLookupVariable
	{
		TypeReference typeRef;
		Point         startPos;
		Point         endPos;
		
		public TypeReference TypeRef {
			get {
				return typeRef;
			}
		}
		public Point StartPos {
			get {
				return startPos;
			}
		}
		public Point EndPos {
			get {
				return endPos;
			}
		}
		
		public LocalLookupVariable(TypeReference typeRef, Point startPos, Point endPos)
		{
			this.typeRef = typeRef;
			this.startPos = startPos;
			this.endPos = endPos;
		}
	}
	
	public class LookupTableVisitor : AbstractASTVisitor
	{
		public Hashtable variables = new Hashtable();
		
		public void AddVariable(TypeReference typeRef, string name, Point startPos, Point endPos)
		{
			ArrayList list;
			if (variables[name] == null) {
				variables[name] = list = new ArrayList();
			} else {
				list = (ArrayList)variables[name];
			}
			list.Add(new LocalLookupVariable(typeRef, startPos, endPos));
		}
		
		public override object Visit(LocalVariableDeclaration localVariableDeclaration, object data)
		{
			foreach (VariableDeclaration varDecl in localVariableDeclaration.Variables) {
				AddVariable(localVariableDeclaration.Type, 
				            varDecl.Name,
				            localVariableDeclaration.StartLocation,
				            CurrentBlock == null ? new Point(-1, -1) : CurrentBlock.EndLocation);
			}
			return data;
		}
		
		// ForStatement and UsingStatement use a LocalVariableDeclaration,
		// so they don't need to be visited separately
		
		public override object Visit(ForeachStatement foreachStatement, object data)
		{
			AddVariable(foreachStatement.TypeReference,
			            foreachStatement.VariableName,
			            foreachStatement.StartLocation,
			            foreachStatement.EndLocation);
			return data;
		}
	}
}
