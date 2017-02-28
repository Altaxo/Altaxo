#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing
{
	public class ClassDeclarationVisitor : CSharpSyntaxRewriter
	{
		public List<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

		public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
		{
			node = (ClassDeclarationSyntax)base.VisitClassDeclaration(node);
			Classes.Add(node); // save your visited classes
			return node;
		}

		public static IEnumerable<ClassDeclarationSyntax> GetAllClasses(SyntaxNode rootNode)
		{
			var visitor = new ClassDeclarationVisitor();
			visitor.Visit(rootNode);
			return visitor.Classes;
		}
	}

	public class MethodVisitor : CSharpSyntaxRewriter
	{
		public List<SyntaxNode> Symbols { get; } = new List<SyntaxNode>();

		public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
		{
			Symbols.Add(node);
			return base.VisitMethodDeclaration(node);
		}

		public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
		{
			Symbols.Add(node);
			return base.VisitPropertyDeclaration(node);
		}

		public override SyntaxNode VisitEventDeclaration(EventDeclarationSyntax node)
		{
			Symbols.Add(node);
			return base.VisitEventDeclaration(node);
		}

		public override SyntaxNode VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
		{
			Symbols.Add(node);
			return base.VisitEventFieldDeclaration(node);
		}

		public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
		{
			Symbols.Add(node);
			return base.VisitConstructorDeclaration(node);
		}

		public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			Symbols.Add(node);
			return base.VisitFieldDeclaration(node);
		}
	}
}