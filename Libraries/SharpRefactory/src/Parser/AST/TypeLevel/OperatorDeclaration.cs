// OperatorDeclaratoin.cs
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
using System.Collections;

namespace ICSharpCode.SharpRefactory.Parser.AST
{
	public class OperatorDeclaration : AbstractNode
	{
		OperatorDeclarator opratorDeclarator;
		Modifier           modifier;
		ArrayList          attributes = new ArrayList();
		BlockStatement     body;
		
		// TODO: delegate OperatorDeclarator Members
		public OperatorDeclarator OpratorDeclarator {
			get {
				return opratorDeclarator;
			}
			set {
				opratorDeclarator = value;
			}
		}
		
		public Modifier Modifier {
			get {
				return modifier;
			}
			set {
				modifier = value;
			}
		}
		
		public ArrayList Attributes {
			get {
				return attributes;
			}
			set {
				attributes = value;
			}
		}
		
		public BlockStatement Body {
			get {
				return body;
			}set {
				body = value;
			}
		}
		
		public OperatorDeclaration(OperatorDeclarator opratorDeclarator, Modifier Modifier, ArrayList attributes)
		{
			this.opratorDeclarator = opratorDeclarator;
			this.modifier          = Modifier;
			this.attributes        = attributes;
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
	
	public enum OperatorType {
		Unary,
		Binary,
		Implicit,
		Explicit
	}
	
	public class OperatorDeclarator
	{
		OperatorType operatorType;
		TypeReference typeReference;
		string overloadOperator;
		
		TypeReference firstParameterType;
		string firstParameterName;
		
		TypeReference secondParameterType;
		string secondParameterName;
		
		public OperatorType OperatorType {
			get {
				return operatorType;
			}
			set {
				operatorType = value;
			}
		}
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = value;
			}
		}
		public string OverloadOperator {
			get {
				return overloadOperator;
			}
			set {
				overloadOperator = value;
			}
		}
		public TypeReference FirstParameterType {
			get {
				return firstParameterType;
			}
			set {
				firstParameterType = value;
			}
		}
		public string FirstParameterName {
			get {
				return firstParameterName;
			}
			set {
				firstParameterName = value;
			}
		}
		public TypeReference SecondParameterType {
			get {
				return secondParameterType;
			}
			set {
				secondParameterType = value;
			}
		}
		public string SecondParameterName {
			get {
				return secondParameterName;
			}
			set {
				secondParameterName = value;
			}
		}
		
		public OperatorDeclarator(TypeReference typeReference, string overloadOperator, TypeReference firstParameterType, string firstParameterName)
		{
			operatorType = OperatorType.Unary;
			this.typeReference = typeReference;
			this.overloadOperator = overloadOperator;
			this.firstParameterType = firstParameterType;
			this.firstParameterName = firstParameterName;
		}
		
		public OperatorDeclarator(TypeReference typeReference, string overloadOperator, TypeReference firstParameterType, string firstParameterName, TypeReference secondParameterType, string secondParameterName)
		{
			operatorType = OperatorType.Binary;
			this.typeReference = typeReference;
			this.overloadOperator = overloadOperator;
			this.firstParameterType = firstParameterType;
			this.firstParameterName = firstParameterName;
			this.secondParameterType = secondParameterType;
			this.secondParameterName = secondParameterName;
		}
		
		public OperatorDeclarator(OperatorType operatorType,TypeReference typeReference,  string overloadOperator, TypeReference firstParameterType, string firstParameterName)
		{
			this.operatorType = operatorType;
			this.typeReference = typeReference;
			this.overloadOperator = overloadOperator;
			this.firstParameterType = firstParameterType;
			this.firstParameterName = firstParameterName;
		}
	}
}
