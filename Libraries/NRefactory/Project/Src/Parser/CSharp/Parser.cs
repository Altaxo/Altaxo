
#line  1 "cs.ATG" 
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Ast;
using ASTAttribute = ICSharpCode.NRefactory.Ast.Attribute;
using Types = ICSharpCode.NRefactory.Ast.ClassType;
/*
  Parser.frame file for NRefactory.
 */
using System;
using System.Reflection;

namespace ICSharpCode.NRefactory.Parser.CSharp {



partial class Parser : AbstractParser
{
	const int maxT = 125;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  18 "cs.ATG" 


/*

*/

	void CS() {

#line  159 "cs.ATG" 
		lexer.NextToken(); /* get the first token */
		compilationUnit = new CompilationUnit(); 
		while (la.kind == 120) {
			UsingDirective();
		}
		while (
#line  163 "cs.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(0);
	}

	void UsingDirective() {

#line  170 "cs.ATG" 
		string qualident = null; TypeReference aliasedType = null;
		
		Expect(120);

#line  173 "cs.ATG" 
		Location startPos = t.Location; 
		Qualident(
#line  174 "cs.ATG" 
out qualident);
		if (la.kind == 3) {
			lexer.NextToken();
			NonArrayType(
#line  175 "cs.ATG" 
out aliasedType);
		}
		Expect(11);

#line  177 "cs.ATG" 
		if (qualident != null && qualident.Length > 0) {
		 INode node;
		 if (aliasedType != null) {
		     node = new UsingDeclaration(qualident, aliasedType);
		 } else {
		     node = new UsingDeclaration(qualident);
		 }
		 node.StartLocation = startPos;
		 node.EndLocation   = t.EndLocation;
		 compilationUnit.AddChild(node);
		}
		
	}

	void GlobalAttributeSection() {
		Expect(18);

#line  193 "cs.ATG" 
		Location startPos = t.Location; 
		Expect(1);

#line  194 "cs.ATG" 
		if (t.val != "assembly") Error("global attribute target specifier (\"assembly\") expected");
		string attributeTarget = t.val;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(9);
		Attribute(
#line  199 "cs.ATG" 
out attribute);

#line  199 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  200 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  200 "cs.ATG" 
out attribute);

#line  200 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  202 "cs.ATG" 
		AttributeSection section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		compilationUnit.AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  293 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		ModifierList m = new ModifierList();
		string qualident;
		
		if (la.kind == 87) {
			lexer.NextToken();

#line  299 "cs.ATG" 
			Location startPos = t.Location; 
			Qualident(
#line  300 "cs.ATG" 
out qualident);

#line  300 "cs.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			compilationUnit.AddChild(node);
			compilationUnit.BlockStart(node);
			
			Expect(16);
			while (la.kind == 120) {
				UsingDirective();
			}
			while (StartOf(1)) {
				NamespaceMemberDecl();
			}
			Expect(17);
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  309 "cs.ATG" 
			node.EndLocation   = t.EndLocation;
			compilationUnit.BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 18) {
				AttributeSection(
#line  313 "cs.ATG" 
out section);

#line  313 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  314 "cs.ATG" 
m);
			}
			TypeDecl(
#line  315 "cs.ATG" 
m, attributes);
		} else SynErr(126);
	}

	void Qualident(
#line  437 "cs.ATG" 
out string qualident) {
		Expect(1);

#line  439 "cs.ATG" 
		qualidentBuilder.Length = 0; qualidentBuilder.Append(t.val); 
		while (
#line  440 "cs.ATG" 
DotAndIdent()) {
			Expect(15);
			Expect(1);

#line  440 "cs.ATG" 
			qualidentBuilder.Append('.');
			qualidentBuilder.Append(t.val); 
			
		}

#line  443 "cs.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void NonArrayType(
#line  552 "cs.ATG" 
out TypeReference type) {

#line  554 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (la.kind == 1 || la.kind == 90 || la.kind == 107) {
			ClassType(
#line  559 "cs.ATG" 
out type, false);
		} else if (StartOf(4)) {
			SimpleType(
#line  560 "cs.ATG" 
out name);

#line  560 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 122) {
			lexer.NextToken();
			Expect(6);

#line  561 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(127);
		if (la.kind == 12) {
			NullableQuestionMark(
#line  564 "cs.ATG" 
ref type);
		}
		while (
#line  566 "cs.ATG" 
IsPointer()) {
			Expect(6);

#line  567 "cs.ATG" 
			++pointer; 
		}

#line  569 "cs.ATG" 
		if (type != null) { type.PointerNestingLevel = pointer; } 
	}

	void Attribute(
#line  209 "cs.ATG" 
out ASTAttribute attribute) {

#line  210 "cs.ATG" 
		string qualident;
		string alias = null;
		
		if (
#line  214 "cs.ATG" 
la.kind == Tokens.Identifier && Peek(1).kind == Tokens.DoubleColon) {
			lexer.NextToken();

#line  215 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  218 "cs.ATG" 
out qualident);

#line  219 "cs.ATG" 
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		string name = (alias != null && alias != "global") ? alias + "." + qualident : qualident;
		
		if (la.kind == 20) {
			AttributeArguments(
#line  223 "cs.ATG" 
positional, named);
		}

#line  223 "cs.ATG" 
		attribute  = new ASTAttribute(name, positional, named);
	}

	void AttributeArguments(
#line  226 "cs.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  228 "cs.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(20);
		if (StartOf(5)) {
			if (
#line  236 "cs.ATG" 
IsAssignment()) {

#line  236 "cs.ATG" 
				nameFound = true; 
				lexer.NextToken();

#line  237 "cs.ATG" 
				name = t.val; 
				Expect(3);
			}
			Expr(
#line  239 "cs.ATG" 
out expr);

#line  239 "cs.ATG" 
			if (expr != null) {if(name == "") positional.Add(expr);
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 14) {
				lexer.NextToken();
				if (
#line  247 "cs.ATG" 
IsAssignment()) {

#line  247 "cs.ATG" 
					nameFound = true; 
					Expect(1);

#line  248 "cs.ATG" 
					name = t.val; 
					Expect(3);
				} else if (StartOf(5)) {

#line  250 "cs.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(128);
				Expr(
#line  251 "cs.ATG" 
out expr);

#line  251 "cs.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(21);
	}

	void Expr(
#line  1611 "cs.ATG" 
out Expression expr) {

#line  1612 "cs.ATG" 
		expr = null; Expression expr1 = null, expr2 = null; AssignmentOperatorType op; 
		UnaryExpr(
#line  1614 "cs.ATG" 
out expr);
		if (StartOf(6)) {
			AssignmentOperator(
#line  1617 "cs.ATG" 
out op);
			Expr(
#line  1617 "cs.ATG" 
out expr1);

#line  1617 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (
#line  1618 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			AssignmentOperator(
#line  1619 "cs.ATG" 
out op);
			Expr(
#line  1619 "cs.ATG" 
out expr1);

#line  1619 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (StartOf(7)) {
			ConditionalOrExpr(
#line  1621 "cs.ATG" 
ref expr);
			if (la.kind == 13) {
				lexer.NextToken();
				Expr(
#line  1622 "cs.ATG" 
out expr1);

#line  1622 "cs.ATG" 
				expr = new BinaryOperatorExpression(expr, BinaryOperatorType.NullCoalescing, expr1); 
			}
			if (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  1623 "cs.ATG" 
out expr1);
				Expect(9);
				Expr(
#line  1623 "cs.ATG" 
out expr2);

#line  1623 "cs.ATG" 
				expr = new ConditionalExpression(expr, expr1, expr2);  
			}
		} else SynErr(129);
	}

	void AttributeSection(
#line  260 "cs.ATG" 
out AttributeSection section) {

#line  262 "cs.ATG" 
		string attributeTarget = "";
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(18);

#line  268 "cs.ATG" 
		Location startPos = t.Location; 
		if (
#line  269 "cs.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 68) {
				lexer.NextToken();

#line  270 "cs.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 100) {
				lexer.NextToken();

#line  271 "cs.ATG" 
				attributeTarget = "return";
			} else {
				lexer.NextToken();

#line  272 "cs.ATG" 
				if (t.val != "field"    || t.val != "method" ||
				  t.val != "module"   || t.val != "param"  ||
				  t.val != "property" || t.val != "type")
				Error("attribute target specifier (event, return, field," +
				      "method, module, param, property, or type) expected");
				attributeTarget = t.val;
				
			}
			Expect(9);
		}
		Attribute(
#line  282 "cs.ATG" 
out attribute);

#line  282 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  283 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  283 "cs.ATG" 
out attribute);

#line  283 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  285 "cs.ATG" 
		section = new AttributeSection(attributeTarget, attributes);
		section.StartLocation = startPos;
		section.EndLocation = t.EndLocation;
		
	}

	void TypeModifier(
#line  639 "cs.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 88: {
			lexer.NextToken();

#line  641 "cs.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  642 "cs.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  643 "cs.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 83: {
			lexer.NextToken();

#line  644 "cs.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 95: {
			lexer.NextToken();

#line  645 "cs.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 118: {
			lexer.NextToken();

#line  646 "cs.ATG" 
			m.Add(Modifiers.Unsafe, t.Location); 
			break;
		}
		case 48: {
			lexer.NextToken();

#line  647 "cs.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 102: {
			lexer.NextToken();

#line  648 "cs.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 106: {
			lexer.NextToken();

#line  649 "cs.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 1: {
			lexer.NextToken();

#line  650 "cs.ATG" 
			if (t.val == "partial") { m.Add(Modifiers.Partial, t.Location); } else { Error("Unexpected identifier"); } 
			break;
		}
		default: SynErr(130); break;
		}
	}

	void TypeDecl(
#line  318 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  320 "cs.ATG" 
		TypeReference type;
		List<TypeReference> names;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		string name;
		List<TemplateDefinition> templates;
		
		if (la.kind == 58) {

#line  326 "cs.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  327 "cs.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			templates = newType.Templates;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			
			newType.Type = Types.Class;
			
			Expect(1);

#line  335 "cs.ATG" 
			newType.Name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  338 "cs.ATG" 
templates);
			}
			if (la.kind == 9) {
				ClassBase(
#line  340 "cs.ATG" 
out names);

#line  340 "cs.ATG" 
				newType.BaseTypes = names; 
			}
			while (
#line  343 "cs.ATG" 
IdentIsWhere()) {
				TypeParameterConstraintsClause(
#line  343 "cs.ATG" 
templates);
			}

#line  345 "cs.ATG" 
			newType.BodyStartLocation = t.EndLocation; 
			ClassBody();
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  347 "cs.ATG" 
			newType.EndLocation = t.Location; 
			compilationUnit.BlockEnd();
			
		} else if (StartOf(8)) {

#line  350 "cs.ATG" 
			m.Check(Modifiers.StructsInterfacesEnumsDelegates); 
			if (la.kind == 108) {
				lexer.NextToken();

#line  351 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.Type = Types.Struct; 
				
				Expect(1);

#line  358 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  361 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					StructInterfaces(
#line  363 "cs.ATG" 
out names);

#line  363 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (
#line  366 "cs.ATG" 
IdentIsWhere()) {
					TypeParameterConstraintsClause(
#line  366 "cs.ATG" 
templates);
				}

#line  369 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				StructBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  371 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 82) {
				lexer.NextToken();

#line  375 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Interface;
				
				Expect(1);

#line  382 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  385 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					InterfaceBase(
#line  387 "cs.ATG" 
out names);

#line  387 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (
#line  390 "cs.ATG" 
IdentIsWhere()) {
					TypeParameterConstraintsClause(
#line  390 "cs.ATG" 
templates);
				}

#line  392 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				InterfaceBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  394 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 67) {
				lexer.NextToken();

#line  398 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Enum;
				
				Expect(1);

#line  404 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 9) {
					lexer.NextToken();
					IntegralType(
#line  405 "cs.ATG" 
out name);

#line  405 "cs.ATG" 
					newType.BaseTypes.Add(new TypeReference(name)); 
				}

#line  407 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				EnumBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  409 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else {
				lexer.NextToken();

#line  413 "cs.ATG" 
				DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
				templates = delegateDeclr.Templates;
				delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
				
				if (
#line  417 "cs.ATG" 
NotVoidPointer()) {
					Expect(122);

#line  417 "cs.ATG" 
					delegateDeclr.ReturnType = new TypeReference("void", 0, null); 
				} else if (StartOf(9)) {
					Type(
#line  418 "cs.ATG" 
out type);

#line  418 "cs.ATG" 
					delegateDeclr.ReturnType = type; 
				} else SynErr(131);
				Expect(1);

#line  420 "cs.ATG" 
				delegateDeclr.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  423 "cs.ATG" 
templates);
				}
				Expect(20);
				if (StartOf(10)) {
					FormalParameterList(
#line  425 "cs.ATG" 
p);

#line  425 "cs.ATG" 
					delegateDeclr.Parameters = p; 
				}
				Expect(21);
				while (
#line  429 "cs.ATG" 
IdentIsWhere()) {
					TypeParameterConstraintsClause(
#line  429 "cs.ATG" 
templates);
				}
				Expect(11);

#line  431 "cs.ATG" 
				delegateDeclr.EndLocation = t.Location;
				compilationUnit.AddChild(delegateDeclr);
				
			}
		} else SynErr(132);
	}

	void TypeParameterList(
#line  2013 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2015 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		Expect(23);
		while (la.kind == 18) {
			AttributeSection(
#line  2019 "cs.ATG" 
out section);

#line  2019 "cs.ATG" 
			attributes.Add(section); 
		}
		Expect(1);

#line  2020 "cs.ATG" 
		templates.Add(new TemplateDefinition(t.val, attributes)); 
		while (la.kind == 14) {
			lexer.NextToken();
			while (la.kind == 18) {
				AttributeSection(
#line  2021 "cs.ATG" 
out section);

#line  2021 "cs.ATG" 
				attributes.Add(section); 
			}
			Expect(1);

#line  2022 "cs.ATG" 
			templates.Add(new TemplateDefinition(t.val, attributes)); 
		}
		Expect(22);
	}

	void ClassBase(
#line  446 "cs.ATG" 
out List<TypeReference> names) {

#line  448 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		ClassType(
#line  452 "cs.ATG" 
out typeRef, false);

#line  452 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  453 "cs.ATG" 
out typeRef, false);

#line  453 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void TypeParameterConstraintsClause(
#line  2026 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2027 "cs.ATG" 
		string name = ""; TypeReference type; 
		Expect(1);

#line  2029 "cs.ATG" 
		if (t.val != "where") Error("where expected"); 
		Expect(1);

#line  2030 "cs.ATG" 
		name = t.val; 
		Expect(9);
		TypeParameterConstraintsClauseBase(
#line  2032 "cs.ATG" 
out type);

#line  2033 "cs.ATG" 
		TemplateDefinition td = null;
		foreach (TemplateDefinition d in templates) {
			if (d.Name == name) {
				td = d;
				break;
			}
		}
		if ( td != null && type != null) { td.Bases.Add(type); }
		
		while (la.kind == 14) {
			lexer.NextToken();
			TypeParameterConstraintsClauseBase(
#line  2042 "cs.ATG" 
out type);

#line  2043 "cs.ATG" 
			td = null;
			foreach (TemplateDefinition d in templates) {
				if (d.Name == name) {
					td = d;
					break;
				}
			}
			if ( td != null && type != null) { td.Bases.Add(type); }
			
		}
	}

	void ClassBody() {

#line  457 "cs.ATG" 
		AttributeSection section; 
		Expect(16);
		while (StartOf(11)) {

#line  460 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 18) {
				AttributeSection(
#line  463 "cs.ATG" 
out section);

#line  463 "cs.ATG" 
				attributes.Add(section); 
			}
			MemberModifiers(
#line  464 "cs.ATG" 
m);
			ClassMemberDecl(
#line  465 "cs.ATG" 
m, attributes);
		}
		Expect(17);
	}

	void StructInterfaces(
#line  470 "cs.ATG" 
out List<TypeReference> names) {

#line  472 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  476 "cs.ATG" 
out typeRef, false);

#line  476 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  477 "cs.ATG" 
out typeRef, false);

#line  477 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void StructBody() {

#line  481 "cs.ATG" 
		AttributeSection section; 
		Expect(16);
		while (StartOf(12)) {

#line  484 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 18) {
				AttributeSection(
#line  487 "cs.ATG" 
out section);

#line  487 "cs.ATG" 
				attributes.Add(section); 
			}
			MemberModifiers(
#line  488 "cs.ATG" 
m);
			StructMemberDecl(
#line  489 "cs.ATG" 
m, attributes);
		}
		Expect(17);
	}

	void InterfaceBase(
#line  494 "cs.ATG" 
out List<TypeReference> names) {

#line  496 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  500 "cs.ATG" 
out typeRef, false);

#line  500 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  501 "cs.ATG" 
out typeRef, false);

#line  501 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void InterfaceBody() {
		Expect(16);
		while (StartOf(13)) {
			InterfaceMemberDecl();
		}
		Expect(17);
	}

	void IntegralType(
#line  661 "cs.ATG" 
out string name) {

#line  661 "cs.ATG" 
		name = ""; 
		switch (la.kind) {
		case 101: {
			lexer.NextToken();

#line  663 "cs.ATG" 
			name = "sbyte"; 
			break;
		}
		case 53: {
			lexer.NextToken();

#line  664 "cs.ATG" 
			name = "byte"; 
			break;
		}
		case 103: {
			lexer.NextToken();

#line  665 "cs.ATG" 
			name = "short"; 
			break;
		}
		case 119: {
			lexer.NextToken();

#line  666 "cs.ATG" 
			name = "ushort"; 
			break;
		}
		case 81: {
			lexer.NextToken();

#line  667 "cs.ATG" 
			name = "int"; 
			break;
		}
		case 115: {
			lexer.NextToken();

#line  668 "cs.ATG" 
			name = "uint"; 
			break;
		}
		case 86: {
			lexer.NextToken();

#line  669 "cs.ATG" 
			name = "long"; 
			break;
		}
		case 116: {
			lexer.NextToken();

#line  670 "cs.ATG" 
			name = "ulong"; 
			break;
		}
		case 56: {
			lexer.NextToken();

#line  671 "cs.ATG" 
			name = "char"; 
			break;
		}
		default: SynErr(133); break;
		}
	}

	void EnumBody() {

#line  510 "cs.ATG" 
		FieldDeclaration f; 
		Expect(16);
		if (la.kind == 1 || la.kind == 18) {
			EnumMemberDecl(
#line  513 "cs.ATG" 
out f);

#line  513 "cs.ATG" 
			compilationUnit.AddChild(f); 
			while (
#line  514 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				EnumMemberDecl(
#line  515 "cs.ATG" 
out f);

#line  515 "cs.ATG" 
				compilationUnit.AddChild(f); 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);
	}

	void Type(
#line  520 "cs.ATG" 
out TypeReference type) {
		TypeWithRestriction(
#line  522 "cs.ATG" 
out type, true, false);
	}

	void FormalParameterList(
#line  583 "cs.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  586 "cs.ATG" 
		ParameterDeclarationExpression p;
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		while (la.kind == 18) {
			AttributeSection(
#line  591 "cs.ATG" 
out section);

#line  591 "cs.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(14)) {
			FixedParameter(
#line  593 "cs.ATG" 
out p);

#line  593 "cs.ATG" 
			bool paramsFound = false;
			p.Attributes = attributes;
			parameter.Add(p);
			
			while (la.kind == 14) {
				lexer.NextToken();

#line  598 "cs.ATG" 
				attributes = new List<AttributeSection>(); if (paramsFound) Error("params array must be at end of parameter list"); 
				while (la.kind == 18) {
					AttributeSection(
#line  599 "cs.ATG" 
out section);

#line  599 "cs.ATG" 
					attributes.Add(section); 
				}
				if (StartOf(14)) {
					FixedParameter(
#line  601 "cs.ATG" 
out p);

#line  601 "cs.ATG" 
					p.Attributes = attributes; parameter.Add(p); 
				} else if (la.kind == 94) {
					ParameterArray(
#line  602 "cs.ATG" 
out p);

#line  602 "cs.ATG" 
					paramsFound = true; p.Attributes = attributes; parameter.Add(p); 
				} else SynErr(134);
			}
		} else if (la.kind == 94) {
			ParameterArray(
#line  605 "cs.ATG" 
out p);

#line  605 "cs.ATG" 
			p.Attributes = attributes; parameter.Add(p); 
		} else SynErr(135);
	}

	void ClassType(
#line  653 "cs.ATG" 
out TypeReference typeRef, bool canBeUnbound) {

#line  654 "cs.ATG" 
		TypeReference r; typeRef = null; 
		if (la.kind == 1) {
			TypeName(
#line  656 "cs.ATG" 
out r, canBeUnbound);

#line  656 "cs.ATG" 
			typeRef = r; 
		} else if (la.kind == 90) {
			lexer.NextToken();

#line  657 "cs.ATG" 
			typeRef = new TypeReference("object"); 
		} else if (la.kind == 107) {
			lexer.NextToken();

#line  658 "cs.ATG" 
			typeRef = new TypeReference("string"); 
		} else SynErr(136);
	}

	void TypeName(
#line  1956 "cs.ATG" 
out TypeReference typeRef, bool canBeUnbound) {

#line  1957 "cs.ATG" 
		List<TypeReference> typeArguments = null;
		string alias = null;
		string qualident;
		
		if (
#line  1962 "cs.ATG" 
la.kind == Tokens.Identifier && Peek(1).kind == Tokens.DoubleColon) {
			lexer.NextToken();

#line  1963 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  1966 "cs.ATG" 
out qualident);
		if (la.kind == 23) {
			TypeArgumentList(
#line  1967 "cs.ATG" 
out typeArguments, canBeUnbound);
		}

#line  1969 "cs.ATG" 
		if (alias == null) {
		typeRef = new TypeReference(qualident, typeArguments);
		} else if (alias == "global") {
			typeRef = new TypeReference(qualident, typeArguments);
			typeRef.IsGlobal = true;
		} else {
			typeRef = new TypeReference(alias + "." + qualident, typeArguments);
		}
		
		while (
#line  1978 "cs.ATG" 
DotAndIdent()) {
			Expect(15);

#line  1979 "cs.ATG" 
			typeArguments = null; 
			Qualident(
#line  1980 "cs.ATG" 
out qualident);
			if (la.kind == 23) {
				TypeArgumentList(
#line  1981 "cs.ATG" 
out typeArguments, canBeUnbound);
			}

#line  1982 "cs.ATG" 
			typeRef = new InnerClassTypeReference(typeRef, qualident, typeArguments); 
		}
	}

	void MemberModifiers(
#line  674 "cs.ATG" 
ModifierList m) {
		while (StartOf(15) || 
#line  692 "cs.ATG" 
la.kind == Tokens.Identifier && la.val == "partial") {
			if (la.kind == 48) {
				lexer.NextToken();

#line  677 "cs.ATG" 
				m.Add(Modifiers.Abstract, t.Location); 
			} else if (la.kind == 70) {
				lexer.NextToken();

#line  678 "cs.ATG" 
				m.Add(Modifiers.Extern, t.Location); 
			} else if (la.kind == 83) {
				lexer.NextToken();

#line  679 "cs.ATG" 
				m.Add(Modifiers.Internal, t.Location); 
			} else if (la.kind == 88) {
				lexer.NextToken();

#line  680 "cs.ATG" 
				m.Add(Modifiers.New, t.Location); 
			} else if (la.kind == 93) {
				lexer.NextToken();

#line  681 "cs.ATG" 
				m.Add(Modifiers.Override, t.Location); 
			} else if (la.kind == 95) {
				lexer.NextToken();

#line  682 "cs.ATG" 
				m.Add(Modifiers.Private, t.Location); 
			} else if (la.kind == 96) {
				lexer.NextToken();

#line  683 "cs.ATG" 
				m.Add(Modifiers.Protected, t.Location); 
			} else if (la.kind == 97) {
				lexer.NextToken();

#line  684 "cs.ATG" 
				m.Add(Modifiers.Public, t.Location); 
			} else if (la.kind == 98) {
				lexer.NextToken();

#line  685 "cs.ATG" 
				m.Add(Modifiers.ReadOnly, t.Location); 
			} else if (la.kind == 102) {
				lexer.NextToken();

#line  686 "cs.ATG" 
				m.Add(Modifiers.Sealed, t.Location); 
			} else if (la.kind == 106) {
				lexer.NextToken();

#line  687 "cs.ATG" 
				m.Add(Modifiers.Static, t.Location); 
			} else if (la.kind == 73) {
				lexer.NextToken();

#line  688 "cs.ATG" 
				m.Add(Modifiers.Fixed, t.Location); 
			} else if (la.kind == 118) {
				lexer.NextToken();

#line  689 "cs.ATG" 
				m.Add(Modifiers.Unsafe, t.Location); 
			} else if (la.kind == 121) {
				lexer.NextToken();

#line  690 "cs.ATG" 
				m.Add(Modifiers.Virtual, t.Location); 
			} else if (la.kind == 123) {
				lexer.NextToken();

#line  691 "cs.ATG" 
				m.Add(Modifiers.Volatile, t.Location); 
			} else {
				Expect(1);

#line  693 "cs.ATG" 
				m.Add(Modifiers.Partial, t.Location); 
			}
		}
	}

	void ClassMemberDecl(
#line  985 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  986 "cs.ATG" 
		Statement stmt = null; 
		if (StartOf(16)) {
			StructMemberDecl(
#line  988 "cs.ATG" 
m, attributes);
		} else if (la.kind == 27) {

#line  989 "cs.ATG" 
			m.Check(Modifiers.Destructors); Location startPos = t.Location; 
			lexer.NextToken();
			Expect(1);

#line  990 "cs.ATG" 
			DestructorDeclaration d = new DestructorDeclaration(t.val, m.Modifier, attributes); 
			d.Modifier = m.Modifier;
			d.StartLocation = m.GetDeclarationLocation(startPos);
			
			Expect(20);
			Expect(21);

#line  994 "cs.ATG" 
			d.EndLocation = t.EndLocation; 
			if (la.kind == 16) {
				Block(
#line  994 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(137);

#line  995 "cs.ATG" 
			d.Body = (BlockStatement)stmt;
			compilationUnit.AddChild(d);
			
		} else SynErr(138);
	}

	void StructMemberDecl(
#line  698 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  700 "cs.ATG" 
		string qualident = null;
		TypeReference type;
		Expression expr;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		TypeReference explicitInterface = null;
		
		if (la.kind == 59) {

#line  710 "cs.ATG" 
			m.Check(Modifiers.Constants); 
			lexer.NextToken();

#line  711 "cs.ATG" 
			Location startPos = t.Location; 
			Type(
#line  712 "cs.ATG" 
out type);
			Expect(1);

#line  712 "cs.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier | Modifiers.Const);
			fd.StartLocation = m.GetDeclarationLocation(startPos);
			VariableDeclaration f = new VariableDeclaration(t.val);
			fd.Fields.Add(f);
			
			Expect(3);
			Expr(
#line  717 "cs.ATG" 
out expr);

#line  717 "cs.ATG" 
			f.Initializer = expr; 
			while (la.kind == 14) {
				lexer.NextToken();
				Expect(1);

#line  718 "cs.ATG" 
				f = new VariableDeclaration(t.val);
				fd.Fields.Add(f);
				
				Expect(3);
				Expr(
#line  721 "cs.ATG" 
out expr);

#line  721 "cs.ATG" 
				f.Initializer = expr; 
			}
			Expect(11);

#line  722 "cs.ATG" 
			fd.EndLocation = t.EndLocation; compilationUnit.AddChild(fd); 
		} else if (
#line  726 "cs.ATG" 
NotVoidPointer()) {

#line  726 "cs.ATG" 
			m.Check(Modifiers.PropertysEventsMethods); 
			Expect(122);

#line  727 "cs.ATG" 
			Location startPos = t.Location; 
			if (
#line  728 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
				TypeName(
#line  729 "cs.ATG" 
out explicitInterface, false);

#line  730 "cs.ATG" 
				if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
				 } 
			} else if (la.kind == 1) {
				lexer.NextToken();

#line  733 "cs.ATG" 
				qualident = t.val; 
			} else SynErr(139);
			if (la.kind == 23) {
				TypeParameterList(
#line  736 "cs.ATG" 
templates);
			}
			Expect(20);
			if (StartOf(10)) {
				FormalParameterList(
#line  739 "cs.ATG" 
p);
			}
			Expect(21);

#line  740 "cs.ATG" 
			MethodDeclaration methodDeclaration = new MethodDeclaration(qualident,
			                                                         m.Modifier,
			                                                         new TypeReference("void"),
			                                                         p,
			                                                         attributes);
			methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
			methodDeclaration.EndLocation   = t.EndLocation;
			methodDeclaration.Templates = templates;
			if (explicitInterface != null)
			methodDeclaration.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
			compilationUnit.AddChild(methodDeclaration);
			compilationUnit.BlockStart(methodDeclaration);
			
			while (
#line  755 "cs.ATG" 
IdentIsWhere()) {
				TypeParameterConstraintsClause(
#line  755 "cs.ATG" 
templates);
			}
			if (la.kind == 16) {
				Block(
#line  757 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(140);

#line  757 "cs.ATG" 
			compilationUnit.BlockEnd();
			methodDeclaration.Body  = (BlockStatement)stmt;
			
		} else if (la.kind == 68) {

#line  761 "cs.ATG" 
			m.Check(Modifiers.PropertysEventsMethods); 
			lexer.NextToken();

#line  762 "cs.ATG" 
			EventDeclaration eventDecl = new EventDeclaration(null, null, m.Modifier, attributes, null);
			eventDecl.StartLocation = t.Location;
			compilationUnit.AddChild(eventDecl);
			compilationUnit.BlockStart(eventDecl);
			EventAddRegion addBlock = null;
			EventRemoveRegion removeBlock = null;
			
			Type(
#line  769 "cs.ATG" 
out type);

#line  769 "cs.ATG" 
			eventDecl.TypeReference = type; 
			if (
#line  770 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
				TypeName(
#line  771 "cs.ATG" 
out explicitInterface, false);

#line  772 "cs.ATG" 
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface); 

#line  773 "cs.ATG" 
				eventDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident)); 
			} else if (la.kind == 1) {
				lexer.NextToken();

#line  775 "cs.ATG" 
				qualident = t.val; 
			} else SynErr(141);

#line  777 "cs.ATG" 
			eventDecl.Name = qualident; eventDecl.EndLocation = t.EndLocation; 
			if (la.kind == 3) {
				lexer.NextToken();
				Expr(
#line  778 "cs.ATG" 
out expr);

#line  778 "cs.ATG" 
				eventDecl.Initializer = expr; 
			}
			if (la.kind == 16) {
				lexer.NextToken();

#line  779 "cs.ATG" 
				eventDecl.BodyStart = t.Location; 
				EventAccessorDecls(
#line  780 "cs.ATG" 
out addBlock, out removeBlock);
				Expect(17);

#line  781 "cs.ATG" 
				eventDecl.BodyEnd   = t.EndLocation; 
			}
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  784 "cs.ATG" 
			compilationUnit.BlockEnd();
			eventDecl.AddRegion = addBlock;
			eventDecl.RemoveRegion = removeBlock;
			
		} else if (
#line  790 "cs.ATG" 
IdentAndLPar()) {

#line  790 "cs.ATG" 
			m.Check(Modifiers.Constructors | Modifiers.StaticConstructors); 
			Expect(1);

#line  791 "cs.ATG" 
			string name = t.val; Location startPos = t.Location; 
			Expect(20);
			if (StartOf(10)) {

#line  791 "cs.ATG" 
				m.Check(Modifiers.Constructors); 
				FormalParameterList(
#line  792 "cs.ATG" 
p);
			}
			Expect(21);

#line  794 "cs.ATG" 
			ConstructorInitializer init = null;  
			if (la.kind == 9) {

#line  795 "cs.ATG" 
				m.Check(Modifiers.Constructors); 
				ConstructorInitializer(
#line  796 "cs.ATG" 
out init);
			}

#line  798 "cs.ATG" 
			ConstructorDeclaration cd = new ConstructorDeclaration(name, m.Modifier, p, init, attributes); 
			cd.StartLocation = startPos;
			cd.EndLocation   = t.EndLocation;
			
			if (la.kind == 16) {
				Block(
#line  803 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(142);

#line  803 "cs.ATG" 
			cd.Body = (BlockStatement)stmt; compilationUnit.AddChild(cd); 
		} else if (la.kind == 69 || la.kind == 79) {

#line  806 "cs.ATG" 
			m.Check(Modifiers.Operators);
			if (m.isNone) Error("at least one modifier must be set"); 
			bool isImplicit = true;
			Location startPos = Location.Empty;
			
			if (la.kind == 79) {
				lexer.NextToken();

#line  811 "cs.ATG" 
				startPos = t.Location; 
			} else {
				lexer.NextToken();

#line  811 "cs.ATG" 
				isImplicit = false; startPos = t.Location; 
			}
			Expect(91);
			Type(
#line  812 "cs.ATG" 
out type);

#line  812 "cs.ATG" 
			TypeReference operatorType = type; 
			Expect(20);
			Type(
#line  813 "cs.ATG" 
out type);
			Expect(1);

#line  813 "cs.ATG" 
			string varName = t.val; 
			Expect(21);

#line  814 "cs.ATG" 
			Location endPos = t.Location; 
			if (la.kind == 16) {
				Block(
#line  815 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();

#line  815 "cs.ATG" 
				stmt = null; 
			} else SynErr(143);

#line  818 "cs.ATG" 
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			parameters.Add(new ParameterDeclarationExpression(type, varName));
			OperatorDeclaration operatorDeclaration = new OperatorDeclaration(m.Modifier, 
			                                                                  attributes, 
			                                                                  parameters, 
			                                                                  operatorType,
			                                                                  isImplicit ? ConversionType.Implicit : ConversionType.Explicit
			                                                                  );
			operatorDeclaration.Body = (BlockStatement)stmt;
			operatorDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
			operatorDeclaration.EndLocation = endPos;
			compilationUnit.AddChild(operatorDeclaration);
			
		} else if (StartOf(17)) {
			TypeDecl(
#line  834 "cs.ATG" 
m, attributes);
		} else if (StartOf(9)) {
			Type(
#line  836 "cs.ATG" 
out type);

#line  836 "cs.ATG" 
			Location startPos = t.Location;  
			if (la.kind == 91) {

#line  838 "cs.ATG" 
				OverloadableOperatorType op;
				m.Check(Modifiers.Operators);
				if (m.isNone) Error("at least one modifier must be set");
				
				lexer.NextToken();
				OverloadableOperator(
#line  842 "cs.ATG" 
out op);

#line  842 "cs.ATG" 
				TypeReference firstType, secondType = null; string secondName = null; 
				Expect(20);
				Type(
#line  843 "cs.ATG" 
out firstType);
				Expect(1);

#line  843 "cs.ATG" 
				string firstName = t.val; 
				if (la.kind == 14) {
					lexer.NextToken();
					Type(
#line  844 "cs.ATG" 
out secondType);
					Expect(1);

#line  844 "cs.ATG" 
					secondName = t.val; 
				} else if (la.kind == 21) {
				} else SynErr(144);

#line  852 "cs.ATG" 
				Location endPos = t.Location; 
				Expect(21);
				if (la.kind == 16) {
					Block(
#line  853 "cs.ATG" 
out stmt);
				} else if (la.kind == 11) {
					lexer.NextToken();
				} else SynErr(145);

#line  855 "cs.ATG" 
				List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
				parameters.Add(new ParameterDeclarationExpression(firstType, firstName));
				if (secondType != null) {
					parameters.Add(new ParameterDeclarationExpression(secondType, secondName));
				}
				OperatorDeclaration operatorDeclaration = new OperatorDeclaration(m.Modifier,
				                                                                  attributes,
				                                                                  parameters,
				                                                                  type,
				                                                                  op);
				operatorDeclaration.Body = (BlockStatement)stmt;
				operatorDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
				operatorDeclaration.EndLocation = endPos;
				compilationUnit.AddChild(operatorDeclaration);
				
			} else if (
#line  872 "cs.ATG" 
IsVarDecl()) {

#line  873 "cs.ATG" 
				m.Check(Modifiers.Fields);
				FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
				fd.StartLocation = m.GetDeclarationLocation(startPos); 
				
				if (
#line  877 "cs.ATG" 
m.Contains(Modifiers.Fixed)) {
					VariableDeclarator(
#line  878 "cs.ATG" 
variableDeclarators);
					Expect(18);
					Expr(
#line  880 "cs.ATG" 
out expr);

#line  880 "cs.ATG" 
					if (variableDeclarators.Count > 0)
					variableDeclarators[variableDeclarators.Count-1].FixedArrayInitialization = expr; 
					Expect(19);
					while (la.kind == 14) {
						lexer.NextToken();
						VariableDeclarator(
#line  884 "cs.ATG" 
variableDeclarators);
						Expect(18);
						Expr(
#line  886 "cs.ATG" 
out expr);

#line  886 "cs.ATG" 
						if (variableDeclarators.Count > 0)
						variableDeclarators[variableDeclarators.Count-1].FixedArrayInitialization = expr; 
						Expect(19);
					}
				} else if (la.kind == 1) {
					VariableDeclarator(
#line  891 "cs.ATG" 
variableDeclarators);
					while (la.kind == 14) {
						lexer.NextToken();
						VariableDeclarator(
#line  892 "cs.ATG" 
variableDeclarators);
					}
				} else SynErr(146);
				Expect(11);

#line  894 "cs.ATG" 
				fd.EndLocation = t.EndLocation; fd.Fields = variableDeclarators; compilationUnit.AddChild(fd); 
			} else if (la.kind == 110) {

#line  897 "cs.ATG" 
				m.Check(Modifiers.Indexers); 
				lexer.NextToken();
				Expect(18);
				FormalParameterList(
#line  898 "cs.ATG" 
p);
				Expect(19);

#line  898 "cs.ATG" 
				Location endLocation = t.EndLocation; 
				Expect(16);

#line  899 "cs.ATG" 
				IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
				indexer.StartLocation = startPos;
				indexer.EndLocation   = endLocation;
				indexer.BodyStart     = t.Location;
				PropertyGetRegion getRegion;
				PropertySetRegion setRegion;
				
				AccessorDecls(
#line  906 "cs.ATG" 
out getRegion, out setRegion);
				Expect(17);

#line  907 "cs.ATG" 
				indexer.BodyEnd    = t.EndLocation;
				indexer.GetRegion = getRegion;
				indexer.SetRegion = setRegion;
				compilationUnit.AddChild(indexer);
				
			} else if (
#line  912 "cs.ATG" 
la.kind == Tokens.Identifier) {
				if (
#line  913 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
					TypeName(
#line  914 "cs.ATG" 
out explicitInterface, false);

#line  915 "cs.ATG" 
					if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
					qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
					 } 
				} else if (la.kind == 1) {
					lexer.NextToken();

#line  918 "cs.ATG" 
					qualident = t.val; 
				} else SynErr(147);

#line  920 "cs.ATG" 
				Location qualIdentEndLocation = t.EndLocation; 
				if (la.kind == 16 || la.kind == 20 || la.kind == 23) {
					if (la.kind == 20 || la.kind == 23) {

#line  924 "cs.ATG" 
						m.Check(Modifiers.PropertysEventsMethods); 
						if (la.kind == 23) {
							TypeParameterList(
#line  926 "cs.ATG" 
templates);
						}
						Expect(20);
						if (StartOf(10)) {
							FormalParameterList(
#line  927 "cs.ATG" 
p);
						}
						Expect(21);

#line  928 "cs.ATG" 
						MethodDeclaration methodDeclaration = new MethodDeclaration(qualident,
						                                                           m.Modifier, 
						                                                           type, 
						                                                           p, 
						                                                           attributes);
						if (explicitInterface != null)
							methodDeclaration.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
						methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
						methodDeclaration.EndLocation   = t.EndLocation;
						methodDeclaration.Templates = templates;
						compilationUnit.AddChild(methodDeclaration);
						                                      
						while (
#line  940 "cs.ATG" 
IdentIsWhere()) {
							TypeParameterConstraintsClause(
#line  940 "cs.ATG" 
templates);
						}
						if (la.kind == 16) {
							Block(
#line  941 "cs.ATG" 
out stmt);
						} else if (la.kind == 11) {
							lexer.NextToken();
						} else SynErr(148);

#line  941 "cs.ATG" 
						methodDeclaration.Body  = (BlockStatement)stmt; 
					} else {
						lexer.NextToken();

#line  944 "cs.ATG" 
						PropertyDeclaration pDecl = new PropertyDeclaration(qualident, type, m.Modifier, attributes); 
						if (explicitInterface != null)
						pDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
						      pDecl.StartLocation = m.GetDeclarationLocation(startPos);
						      pDecl.EndLocation   = qualIdentEndLocation;
						      pDecl.BodyStart   = t.Location;
						      PropertyGetRegion getRegion;
						      PropertySetRegion setRegion;
						   
						AccessorDecls(
#line  953 "cs.ATG" 
out getRegion, out setRegion);
						Expect(17);

#line  955 "cs.ATG" 
						pDecl.GetRegion = getRegion;
						pDecl.SetRegion = setRegion;
						pDecl.BodyEnd = t.EndLocation;
						compilationUnit.AddChild(pDecl);
						
					}
				} else if (la.kind == 15) {

#line  963 "cs.ATG" 
					m.Check(Modifiers.Indexers); 
					lexer.NextToken();
					Expect(110);
					Expect(18);
					FormalParameterList(
#line  964 "cs.ATG" 
p);
					Expect(19);

#line  965 "cs.ATG" 
					IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
					indexer.StartLocation = m.GetDeclarationLocation(startPos);
					indexer.EndLocation   = t.EndLocation;
					if (explicitInterface != null)
					indexer.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, "this"));
					      PropertyGetRegion getRegion;
					      PropertySetRegion setRegion;
					    
					Expect(16);

#line  973 "cs.ATG" 
					Location bodyStart = t.Location; 
					AccessorDecls(
#line  974 "cs.ATG" 
out getRegion, out setRegion);
					Expect(17);

#line  975 "cs.ATG" 
					indexer.BodyStart = bodyStart;
					indexer.BodyEnd   = t.EndLocation;
					indexer.GetRegion = getRegion;
					indexer.SetRegion = setRegion;
					compilationUnit.AddChild(indexer);
					
				} else SynErr(149);
			} else SynErr(150);
		} else SynErr(151);
	}

	void InterfaceMemberDecl() {

#line  1002 "cs.ATG" 
		TypeReference type;
		
		AttributeSection section;
		Modifiers mod = Modifiers.None;
		List<AttributeSection> attributes = new List<AttributeSection>();
		List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
		string name;
		PropertyGetRegion getBlock;
		PropertySetRegion setBlock;
		Location startLocation = new Location(-1, -1);
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		
		while (la.kind == 18) {
			AttributeSection(
#line  1015 "cs.ATG" 
out section);

#line  1015 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 88) {
			lexer.NextToken();

#line  1016 "cs.ATG" 
			mod = Modifiers.New; startLocation = t.Location; 
		}
		if (
#line  1019 "cs.ATG" 
NotVoidPointer()) {
			Expect(122);

#line  1019 "cs.ATG" 
			if (startLocation.X == -1) startLocation = t.Location; 
			Expect(1);

#line  1019 "cs.ATG" 
			name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  1020 "cs.ATG" 
templates);
			}
			Expect(20);
			if (StartOf(10)) {
				FormalParameterList(
#line  1021 "cs.ATG" 
parameters);
			}
			Expect(21);
			while (
#line  1022 "cs.ATG" 
IdentIsWhere()) {
				TypeParameterConstraintsClause(
#line  1022 "cs.ATG" 
templates);
			}
			Expect(11);

#line  1024 "cs.ATG" 
			MethodDeclaration md = new MethodDeclaration(name, mod, new TypeReference("void"), parameters, attributes);
			md.StartLocation = startLocation;
			md.EndLocation = t.EndLocation;
			md.Templates = templates;
			compilationUnit.AddChild(md);
			
		} else if (StartOf(18)) {
			if (StartOf(9)) {
				Type(
#line  1031 "cs.ATG" 
out type);

#line  1031 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				if (la.kind == 1) {
					lexer.NextToken();

#line  1033 "cs.ATG" 
					name = t.val; Location qualIdentEndLocation = t.EndLocation; 
					if (la.kind == 20 || la.kind == 23) {
						if (la.kind == 23) {
							TypeParameterList(
#line  1037 "cs.ATG" 
templates);
						}
						Expect(20);
						if (StartOf(10)) {
							FormalParameterList(
#line  1038 "cs.ATG" 
parameters);
						}
						Expect(21);
						while (
#line  1040 "cs.ATG" 
IdentIsWhere()) {
							TypeParameterConstraintsClause(
#line  1040 "cs.ATG" 
templates);
						}
						Expect(11);

#line  1041 "cs.ATG" 
						MethodDeclaration md = new MethodDeclaration(name, mod, type, parameters, attributes);
						md.StartLocation = startLocation;
						md.EndLocation = t.EndLocation;
						md.Templates = templates;
						compilationUnit.AddChild(md);
						
					} else if (la.kind == 16) {

#line  1048 "cs.ATG" 
						PropertyDeclaration pd = new PropertyDeclaration(name, type, mod, attributes); compilationUnit.AddChild(pd); 
						lexer.NextToken();

#line  1049 "cs.ATG" 
						Location bodyStart = t.Location;
						InterfaceAccessors(
#line  1049 "cs.ATG" 
out getBlock, out setBlock);
						Expect(17);

#line  1049 "cs.ATG" 
						pd.GetRegion = getBlock; pd.SetRegion = setBlock; pd.StartLocation = startLocation; pd.EndLocation = qualIdentEndLocation; pd.BodyStart = bodyStart; pd.BodyEnd = t.EndLocation; 
					} else SynErr(152);
				} else if (la.kind == 110) {
					lexer.NextToken();
					Expect(18);
					FormalParameterList(
#line  1052 "cs.ATG" 
parameters);
					Expect(19);

#line  1052 "cs.ATG" 
					Location bracketEndLocation = t.EndLocation; 

#line  1052 "cs.ATG" 
					IndexerDeclaration id = new IndexerDeclaration(type, parameters, mod, attributes); compilationUnit.AddChild(id); 
					Expect(16);

#line  1053 "cs.ATG" 
					Location bodyStart = t.Location;
					InterfaceAccessors(
#line  1053 "cs.ATG" 
out getBlock, out setBlock);
					Expect(17);

#line  1053 "cs.ATG" 
					id.GetRegion = getBlock; id.SetRegion = setBlock; id.StartLocation = startLocation;  id.EndLocation = bracketEndLocation; id.BodyStart = bodyStart; id.BodyEnd = t.EndLocation;
				} else SynErr(153);
			} else {
				lexer.NextToken();

#line  1056 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				Type(
#line  1056 "cs.ATG" 
out type);
				Expect(1);

#line  1056 "cs.ATG" 
				EventDeclaration ed = new EventDeclaration(type, t.val, mod, attributes, null);
				compilationUnit.AddChild(ed);
				
				Expect(11);

#line  1059 "cs.ATG" 
				ed.StartLocation = startLocation; ed.EndLocation = t.EndLocation; 
			}
		} else SynErr(154);
	}

	void EnumMemberDecl(
#line  1064 "cs.ATG" 
out FieldDeclaration f) {

#line  1066 "cs.ATG" 
		Expression expr = null;
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1072 "cs.ATG" 
out section);

#line  1072 "cs.ATG" 
			attributes.Add(section); 
		}
		Expect(1);

#line  1073 "cs.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl         = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		
		if (la.kind == 3) {
			lexer.NextToken();
			Expr(
#line  1078 "cs.ATG" 
out expr);

#line  1078 "cs.ATG" 
			varDecl.Initializer = expr; 
		}
	}

	void TypeWithRestriction(
#line  525 "cs.ATG" 
out TypeReference type, bool allowNullable, bool canBeUnbound) {

#line  527 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (la.kind == 1 || la.kind == 90 || la.kind == 107) {
			ClassType(
#line  532 "cs.ATG" 
out type, canBeUnbound);
		} else if (StartOf(4)) {
			SimpleType(
#line  533 "cs.ATG" 
out name);

#line  533 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 122) {
			lexer.NextToken();
			Expect(6);

#line  534 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(155);

#line  535 "cs.ATG" 
		List<int> r = new List<int>(); 
		if (
#line  537 "cs.ATG" 
allowNullable && la.kind == Tokens.Question) {
			NullableQuestionMark(
#line  537 "cs.ATG" 
ref type);
		}
		while (
#line  539 "cs.ATG" 
IsPointerOrDims()) {

#line  539 "cs.ATG" 
			int i = 0; 
			if (la.kind == 6) {
				lexer.NextToken();

#line  540 "cs.ATG" 
				++pointer; 
			} else if (la.kind == 18) {
				lexer.NextToken();
				while (la.kind == 14) {
					lexer.NextToken();

#line  541 "cs.ATG" 
					++i; 
				}
				Expect(19);

#line  541 "cs.ATG" 
				r.Add(i); 
			} else SynErr(156);
		}

#line  544 "cs.ATG" 
		if (type != null) {
		type.RankSpecifier = r.ToArray();
		type.PointerNestingLevel = pointer;
		  }
		
	}

	void SimpleType(
#line  572 "cs.ATG" 
out string name) {

#line  573 "cs.ATG" 
		name = String.Empty; 
		if (StartOf(19)) {
			IntegralType(
#line  575 "cs.ATG" 
out name);
		} else if (la.kind == 74) {
			lexer.NextToken();

#line  576 "cs.ATG" 
			name = "float"; 
		} else if (la.kind == 65) {
			lexer.NextToken();

#line  577 "cs.ATG" 
			name = "double"; 
		} else if (la.kind == 61) {
			lexer.NextToken();

#line  578 "cs.ATG" 
			name = "decimal"; 
		} else if (la.kind == 51) {
			lexer.NextToken();

#line  579 "cs.ATG" 
			name = "bool"; 
		} else SynErr(157);
	}

	void NullableQuestionMark(
#line  1987 "cs.ATG" 
ref TypeReference typeRef) {

#line  1988 "cs.ATG" 
		List<TypeReference> typeArguments = new List<TypeReference>(1); 
		Expect(12);

#line  1992 "cs.ATG" 
		if (typeRef != null) typeArguments.Add(typeRef);
		typeRef = new TypeReference("System.Nullable", typeArguments);
		
	}

	void FixedParameter(
#line  609 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  611 "cs.ATG" 
		TypeReference type;
		ParameterModifiers mod = ParameterModifiers.In;
		Location start = t.Location;
		
		if (la.kind == 92 || la.kind == 99) {
			if (la.kind == 99) {
				lexer.NextToken();

#line  617 "cs.ATG" 
				mod = ParameterModifiers.Ref; 
			} else {
				lexer.NextToken();

#line  618 "cs.ATG" 
				mod = ParameterModifiers.Out; 
			}
		}
		Type(
#line  620 "cs.ATG" 
out type);
		Expect(1);

#line  620 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, mod); p.StartLocation = start; p.EndLocation = t.Location; 
	}

	void ParameterArray(
#line  623 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  624 "cs.ATG" 
		TypeReference type; 
		Expect(94);
		Type(
#line  626 "cs.ATG" 
out type);
		Expect(1);

#line  626 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, ParameterModifiers.Params); 
	}

	void AccessorModifiers(
#line  629 "cs.ATG" 
out ModifierList m) {

#line  630 "cs.ATG" 
		m = new ModifierList(); 
		if (la.kind == 95) {
			lexer.NextToken();

#line  632 "cs.ATG" 
			m.Add(Modifiers.Private, t.Location); 
		} else if (la.kind == 96) {
			lexer.NextToken();

#line  633 "cs.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			if (la.kind == 83) {
				lexer.NextToken();

#line  634 "cs.ATG" 
				m.Add(Modifiers.Internal, t.Location); 
			}
		} else if (la.kind == 83) {
			lexer.NextToken();

#line  635 "cs.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			if (la.kind == 96) {
				lexer.NextToken();

#line  636 "cs.ATG" 
				m.Add(Modifiers.Protected, t.Location); 
			}
		} else SynErr(158);
	}

	void Block(
#line  1203 "cs.ATG" 
out Statement stmt) {
		Expect(16);

#line  1205 "cs.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.Location;
		compilationUnit.BlockStart(blockStmt);
		if (!ParseMethodBodies) lexer.SkipCurrentBlock(0);
		
		while (StartOf(20)) {
			Statement();
		}
		Expect(17);

#line  1212 "cs.ATG" 
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void EventAccessorDecls(
#line  1138 "cs.ATG" 
out EventAddRegion addBlock, out EventRemoveRegion removeBlock) {

#line  1139 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		Statement stmt;
		addBlock = null;
		removeBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1146 "cs.ATG" 
out section);

#line  1146 "cs.ATG" 
			attributes.Add(section); 
		}
		if (
#line  1148 "cs.ATG" 
IdentIsAdd()) {

#line  1148 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); 
			AddAccessorDecl(
#line  1149 "cs.ATG" 
out stmt);

#line  1149 "cs.ATG" 
			attributes = new List<AttributeSection>(); addBlock.Block = (BlockStatement)stmt; 
			while (la.kind == 18) {
				AttributeSection(
#line  1150 "cs.ATG" 
out section);

#line  1150 "cs.ATG" 
				attributes.Add(section); 
			}
			RemoveAccessorDecl(
#line  1151 "cs.ATG" 
out stmt);

#line  1151 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; 
		} else if (
#line  1152 "cs.ATG" 
IdentIsRemove()) {
			RemoveAccessorDecl(
#line  1153 "cs.ATG" 
out stmt);

#line  1153 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; attributes = new List<AttributeSection>(); 
			while (la.kind == 18) {
				AttributeSection(
#line  1154 "cs.ATG" 
out section);

#line  1154 "cs.ATG" 
				attributes.Add(section); 
			}
			AddAccessorDecl(
#line  1155 "cs.ATG" 
out stmt);

#line  1155 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); addBlock.Block = (BlockStatement)stmt; 
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1156 "cs.ATG" 
			Error("add or remove accessor declaration expected"); 
		} else SynErr(159);
	}

	void ConstructorInitializer(
#line  1234 "cs.ATG" 
out ConstructorInitializer ci) {

#line  1235 "cs.ATG" 
		Expression expr; ci = new ConstructorInitializer(); 
		Expect(9);
		if (la.kind == 50) {
			lexer.NextToken();

#line  1239 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.Base; 
		} else if (la.kind == 110) {
			lexer.NextToken();

#line  1240 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.This; 
		} else SynErr(160);
		Expect(20);
		if (StartOf(21)) {
			Argument(
#line  1243 "cs.ATG" 
out expr);

#line  1243 "cs.ATG" 
			if (expr != null) { ci.Arguments.Add(expr); } 
			while (la.kind == 14) {
				lexer.NextToken();
				Argument(
#line  1243 "cs.ATG" 
out expr);

#line  1243 "cs.ATG" 
				if (expr != null) { ci.Arguments.Add(expr); } 
			}
		}
		Expect(21);
	}

	void OverloadableOperator(
#line  1255 "cs.ATG" 
out OverloadableOperatorType op) {

#line  1256 "cs.ATG" 
		op = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 4: {
			lexer.NextToken();

#line  1258 "cs.ATG" 
			op = OverloadableOperatorType.Add; 
			break;
		}
		case 5: {
			lexer.NextToken();

#line  1259 "cs.ATG" 
			op = OverloadableOperatorType.Subtract; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1261 "cs.ATG" 
			op = OverloadableOperatorType.Not; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1262 "cs.ATG" 
			op = OverloadableOperatorType.BitNot; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1264 "cs.ATG" 
			op = OverloadableOperatorType.Increment; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1265 "cs.ATG" 
			op = OverloadableOperatorType.Decrement; 
			break;
		}
		case 112: {
			lexer.NextToken();

#line  1267 "cs.ATG" 
			op = OverloadableOperatorType.IsTrue; 
			break;
		}
		case 71: {
			lexer.NextToken();

#line  1268 "cs.ATG" 
			op = OverloadableOperatorType.IsFalse; 
			break;
		}
		case 6: {
			lexer.NextToken();

#line  1270 "cs.ATG" 
			op = OverloadableOperatorType.Multiply; 
			break;
		}
		case 7: {
			lexer.NextToken();

#line  1271 "cs.ATG" 
			op = OverloadableOperatorType.Divide; 
			break;
		}
		case 8: {
			lexer.NextToken();

#line  1272 "cs.ATG" 
			op = OverloadableOperatorType.Modulus; 
			break;
		}
		case 28: {
			lexer.NextToken();

#line  1274 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1275 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1276 "cs.ATG" 
			op = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1278 "cs.ATG" 
			op = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1279 "cs.ATG" 
			op = OverloadableOperatorType.Equality; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1280 "cs.ATG" 
			op = OverloadableOperatorType.InEquality; 
			break;
		}
		case 23: {
			lexer.NextToken();

#line  1281 "cs.ATG" 
			op = OverloadableOperatorType.LessThan; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1282 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1283 "cs.ATG" 
			op = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 22: {
			lexer.NextToken();

#line  1284 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThan; 
			if (la.kind == 22) {
				lexer.NextToken();

#line  1284 "cs.ATG" 
				op = OverloadableOperatorType.ShiftRight; 
			}
			break;
		}
		default: SynErr(161); break;
		}
	}

	void VariableDeclarator(
#line  1196 "cs.ATG" 
List<VariableDeclaration> fieldDeclaration) {

#line  1197 "cs.ATG" 
		Expression expr = null; 
		Expect(1);

#line  1199 "cs.ATG" 
		VariableDeclaration f = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1200 "cs.ATG" 
out expr);

#line  1200 "cs.ATG" 
			f.Initializer = expr; 
		}

#line  1200 "cs.ATG" 
		fieldDeclaration.Add(f); 
	}

	void AccessorDecls(
#line  1082 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1084 "cs.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>(); 
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		ModifierList modifiers = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1091 "cs.ATG" 
out section);

#line  1091 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 83 || la.kind == 95 || la.kind == 96) {
			AccessorModifiers(
#line  1092 "cs.ATG" 
out modifiers);
		}
		if (
#line  1094 "cs.ATG" 
IdentIsGet()) {
			GetAccessorDecl(
#line  1095 "cs.ATG" 
out getBlock, attributes);

#line  1096 "cs.ATG" 
			if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(22)) {

#line  1097 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1098 "cs.ATG" 
out section);

#line  1098 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 83 || la.kind == 95 || la.kind == 96) {
					AccessorModifiers(
#line  1099 "cs.ATG" 
out modifiers);
				}
				SetAccessorDecl(
#line  1100 "cs.ATG" 
out setBlock, attributes);

#line  1101 "cs.ATG" 
				if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (
#line  1103 "cs.ATG" 
IdentIsSet()) {
			SetAccessorDecl(
#line  1104 "cs.ATG" 
out setBlock, attributes);

#line  1105 "cs.ATG" 
			if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(22)) {

#line  1106 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1107 "cs.ATG" 
out section);

#line  1107 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 83 || la.kind == 95 || la.kind == 96) {
					AccessorModifiers(
#line  1108 "cs.ATG" 
out modifiers);
				}
				GetAccessorDecl(
#line  1109 "cs.ATG" 
out getBlock, attributes);

#line  1110 "cs.ATG" 
				if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1112 "cs.ATG" 
			Error("get or set accessor declaration expected"); 
		} else SynErr(162);
	}

	void InterfaceAccessors(
#line  1160 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1162 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		getBlock = null; setBlock = null;
		PropertyGetSetRegion lastBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1168 "cs.ATG" 
out section);

#line  1168 "cs.ATG" 
			attributes.Add(section); 
		}

#line  1169 "cs.ATG" 
		Location startLocation = la.Location; 
		if (
#line  1171 "cs.ATG" 
IdentIsGet()) {
			Expect(1);

#line  1171 "cs.ATG" 
			getBlock = new PropertyGetRegion(null, attributes); 
		} else if (
#line  1172 "cs.ATG" 
IdentIsSet()) {
			Expect(1);

#line  1172 "cs.ATG" 
			setBlock = new PropertySetRegion(null, attributes); 
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1173 "cs.ATG" 
			Error("set or get expected"); 
		} else SynErr(163);
		Expect(11);

#line  1176 "cs.ATG" 
		if (getBlock != null) { getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; }
		if (setBlock != null) { setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; }
		attributes = new List<AttributeSection>(); 
		if (la.kind == 1 || la.kind == 18) {
			while (la.kind == 18) {
				AttributeSection(
#line  1180 "cs.ATG" 
out section);

#line  1180 "cs.ATG" 
				attributes.Add(section); 
			}

#line  1181 "cs.ATG" 
			startLocation = la.Location; 
			if (
#line  1183 "cs.ATG" 
IdentIsGet()) {
				Expect(1);

#line  1183 "cs.ATG" 
				if (getBlock != null) Error("get already declared");
				else { getBlock = new PropertyGetRegion(null, attributes); lastBlock = getBlock; }
				
			} else if (
#line  1186 "cs.ATG" 
IdentIsSet()) {
				Expect(1);

#line  1186 "cs.ATG" 
				if (setBlock != null) Error("set already declared");
				else { setBlock = new PropertySetRegion(null, attributes); lastBlock = setBlock; }
				
			} else if (la.kind == 1) {
				lexer.NextToken();

#line  1189 "cs.ATG" 
				Error("set or get expected"); 
			} else SynErr(164);
			Expect(11);

#line  1192 "cs.ATG" 
			if (lastBlock != null) { lastBlock.StartLocation = startLocation; lastBlock.EndLocation = t.EndLocation; } 
		}
	}

	void GetAccessorDecl(
#line  1116 "cs.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1117 "cs.ATG" 
		Statement stmt = null; 
		Expect(1);

#line  1120 "cs.ATG" 
		if (t.val != "get") Error("get expected"); 

#line  1121 "cs.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1122 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(165);

#line  1123 "cs.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 

#line  1124 "cs.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
	}

	void SetAccessorDecl(
#line  1127 "cs.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1128 "cs.ATG" 
		Statement stmt = null; 
		Expect(1);

#line  1131 "cs.ATG" 
		if (t.val != "set") Error("set expected"); 

#line  1132 "cs.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1133 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(166);

#line  1134 "cs.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes); 

#line  1135 "cs.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
	}

	void AddAccessorDecl(
#line  1218 "cs.ATG" 
out Statement stmt) {

#line  1219 "cs.ATG" 
		stmt = null;
		Expect(1);

#line  1222 "cs.ATG" 
		if (t.val != "add") Error("add expected"); 
		Block(
#line  1223 "cs.ATG" 
out stmt);
	}

	void RemoveAccessorDecl(
#line  1226 "cs.ATG" 
out Statement stmt) {

#line  1227 "cs.ATG" 
		stmt = null;
		Expect(1);

#line  1230 "cs.ATG" 
		if (t.val != "remove") Error("remove expected"); 
		Block(
#line  1231 "cs.ATG" 
out stmt);
	}

	void VariableInitializer(
#line  1247 "cs.ATG" 
out Expression initializerExpression) {

#line  1248 "cs.ATG" 
		TypeReference type = null; Expression expr = null; initializerExpression = null; 
		if (StartOf(5)) {
			Expr(
#line  1250 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 16) {
			ArrayInitializer(
#line  1251 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 105) {
			lexer.NextToken();
			Type(
#line  1252 "cs.ATG" 
out type);
			Expect(18);
			Expr(
#line  1252 "cs.ATG" 
out expr);
			Expect(19);

#line  1252 "cs.ATG" 
			initializerExpression = new StackAllocExpression(type, expr); 
		} else SynErr(167);
	}

	void Statement() {

#line  1364 "cs.ATG" 
		TypeReference type;
		Expression expr;
		Statement stmt = null;
		Location startPos = la.Location;
		
		if (
#line  1372 "cs.ATG" 
IsLabel()) {
			Expect(1);

#line  1372 "cs.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val)); 
			Expect(9);
			Statement();
		} else if (la.kind == 59) {
			lexer.NextToken();
			Type(
#line  1375 "cs.ATG" 
out type);

#line  1375 "cs.ATG" 
			LocalVariableDeclaration var = new LocalVariableDeclaration(type, Modifiers.Const); string ident = null; var.StartLocation = t.Location; 
			Expect(1);

#line  1376 "cs.ATG" 
			ident = t.val; 
			Expect(3);
			Expr(
#line  1377 "cs.ATG" 
out expr);

#line  1377 "cs.ATG" 
			var.Variables.Add(new VariableDeclaration(ident, expr)); 
			while (la.kind == 14) {
				lexer.NextToken();
				Expect(1);

#line  1378 "cs.ATG" 
				ident = t.val; 
				Expect(3);
				Expr(
#line  1378 "cs.ATG" 
out expr);

#line  1378 "cs.ATG" 
				var.Variables.Add(new VariableDeclaration(ident, expr)); 
			}
			Expect(11);

#line  1379 "cs.ATG" 
			compilationUnit.AddChild(var); 
		} else if (
#line  1381 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1381 "cs.ATG" 
out stmt);
			Expect(11);

#line  1381 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else if (StartOf(23)) {
			EmbeddedStatement(
#line  1382 "cs.ATG" 
out stmt);

#line  1382 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else SynErr(168);

#line  1388 "cs.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.EndLocation;
		}
		
	}

	void Argument(
#line  1287 "cs.ATG" 
out Expression argumentexpr) {

#line  1289 "cs.ATG" 
		Expression expr;
		FieldDirection fd = FieldDirection.None;
		
		if (la.kind == 92 || la.kind == 99) {
			if (la.kind == 99) {
				lexer.NextToken();

#line  1294 "cs.ATG" 
				fd = FieldDirection.Ref; 
			} else {
				lexer.NextToken();

#line  1295 "cs.ATG" 
				fd = FieldDirection.Out; 
			}
		}
		Expr(
#line  1297 "cs.ATG" 
out expr);

#line  1297 "cs.ATG" 
		argumentexpr = fd != FieldDirection.None ? argumentexpr = new DirectionExpression(fd, expr) : expr; 
	}

	void ArrayInitializer(
#line  1317 "cs.ATG" 
out Expression outExpr) {

#line  1319 "cs.ATG" 
		Expression expr = null;
		ArrayInitializerExpression initializer = new ArrayInitializerExpression();
		
		Expect(16);
		if (StartOf(24)) {
			VariableInitializer(
#line  1324 "cs.ATG" 
out expr);

#line  1325 "cs.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			while (
#line  1326 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				VariableInitializer(
#line  1327 "cs.ATG" 
out expr);

#line  1328 "cs.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);

#line  1332 "cs.ATG" 
		outExpr = initializer; 
	}

	void AssignmentOperator(
#line  1300 "cs.ATG" 
out AssignmentOperatorType op) {

#line  1301 "cs.ATG" 
		op = AssignmentOperatorType.None; 
		if (la.kind == 3) {
			lexer.NextToken();

#line  1303 "cs.ATG" 
			op = AssignmentOperatorType.Assign; 
		} else if (la.kind == 38) {
			lexer.NextToken();

#line  1304 "cs.ATG" 
			op = AssignmentOperatorType.Add; 
		} else if (la.kind == 39) {
			lexer.NextToken();

#line  1305 "cs.ATG" 
			op = AssignmentOperatorType.Subtract; 
		} else if (la.kind == 40) {
			lexer.NextToken();

#line  1306 "cs.ATG" 
			op = AssignmentOperatorType.Multiply; 
		} else if (la.kind == 41) {
			lexer.NextToken();

#line  1307 "cs.ATG" 
			op = AssignmentOperatorType.Divide; 
		} else if (la.kind == 42) {
			lexer.NextToken();

#line  1308 "cs.ATG" 
			op = AssignmentOperatorType.Modulus; 
		} else if (la.kind == 43) {
			lexer.NextToken();

#line  1309 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseAnd; 
		} else if (la.kind == 44) {
			lexer.NextToken();

#line  1310 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseOr; 
		} else if (la.kind == 45) {
			lexer.NextToken();

#line  1311 "cs.ATG" 
			op = AssignmentOperatorType.ExclusiveOr; 
		} else if (la.kind == 46) {
			lexer.NextToken();

#line  1312 "cs.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
		} else if (
#line  1313 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			Expect(22);
			Expect(35);

#line  1314 "cs.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
		} else SynErr(169);
	}

	void LocalVariableDecl(
#line  1335 "cs.ATG" 
out Statement stmt) {

#line  1337 "cs.ATG" 
		TypeReference type;
		VariableDeclaration      var = null;
		LocalVariableDeclaration localVariableDeclaration; 
		
		Type(
#line  1342 "cs.ATG" 
out type);

#line  1342 "cs.ATG" 
		localVariableDeclaration = new LocalVariableDeclaration(type); localVariableDeclaration.StartLocation = t.Location; 
		LocalVariableDeclarator(
#line  1343 "cs.ATG" 
out var);

#line  1343 "cs.ATG" 
		localVariableDeclaration.Variables.Add(var); 
		while (la.kind == 14) {
			lexer.NextToken();
			LocalVariableDeclarator(
#line  1344 "cs.ATG" 
out var);

#line  1344 "cs.ATG" 
			localVariableDeclaration.Variables.Add(var); 
		}

#line  1345 "cs.ATG" 
		stmt = localVariableDeclaration; 
	}

	void LocalVariableDeclarator(
#line  1348 "cs.ATG" 
out VariableDeclaration var) {

#line  1349 "cs.ATG" 
		Expression expr = null; 
		Expect(1);

#line  1352 "cs.ATG" 
		var = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1352 "cs.ATG" 
out expr);

#line  1352 "cs.ATG" 
			var.Initializer = expr; 
		}
	}

	void EmbeddedStatement(
#line  1395 "cs.ATG" 
out Statement statement) {

#line  1397 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;
		
		if (la.kind == 16) {
			Block(
#line  1403 "cs.ATG" 
out statement);
		} else if (la.kind == 11) {
			lexer.NextToken();

#line  1405 "cs.ATG" 
			statement = new EmptyStatement(); 
		} else if (
#line  1407 "cs.ATG" 
UnCheckedAndLBrace()) {

#line  1407 "cs.ATG" 
			Statement block; bool isChecked = true; 
			if (la.kind == 57) {
				lexer.NextToken();
			} else if (la.kind == 117) {
				lexer.NextToken();

#line  1408 "cs.ATG" 
				isChecked = false;
			} else SynErr(170);
			Block(
#line  1409 "cs.ATG" 
out block);

#line  1409 "cs.ATG" 
			statement = isChecked ? (Statement)new CheckedStatement(block) : (Statement)new UncheckedStatement(block); 
		} else if (la.kind == 78) {
			lexer.NextToken();

#line  1411 "cs.ATG" 
			Statement elseStatement = null; 
			Expect(20);
			Expr(
#line  1412 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1413 "cs.ATG" 
out embeddedStatement);
			if (la.kind == 66) {
				lexer.NextToken();
				EmbeddedStatement(
#line  1414 "cs.ATG" 
out elseStatement);
			}

#line  1415 "cs.ATG" 
			statement = elseStatement != null ? new IfElseStatement(expr, embeddedStatement, elseStatement) : new IfElseStatement(expr, embeddedStatement); 

#line  1416 "cs.ATG" 
			if (elseStatement is IfElseStatement && (elseStatement as IfElseStatement).TrueStatement.Count == 1) {
			/* else if-section (otherwise we would have a BlockStatment) */
			(statement as IfElseStatement).ElseIfSections.Add(
			             new ElseIfSection((elseStatement as IfElseStatement).Condition,
			                               (elseStatement as IfElseStatement).TrueStatement[0]));
			(statement as IfElseStatement).ElseIfSections.AddRange((elseStatement as IfElseStatement).ElseIfSections);
			(statement as IfElseStatement).FalseStatement = (elseStatement as IfElseStatement).FalseStatement;
			} 
		} else if (la.kind == 109) {
			lexer.NextToken();

#line  1424 "cs.ATG" 
			List<SwitchSection> switchSections = new List<SwitchSection>(); 
			Expect(20);
			Expr(
#line  1425 "cs.ATG" 
out expr);
			Expect(21);
			Expect(16);
			SwitchSections(
#line  1426 "cs.ATG" 
switchSections);
			Expect(17);

#line  1427 "cs.ATG" 
			statement = new SwitchStatement(expr, switchSections); 
		} else if (la.kind == 124) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1429 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1431 "cs.ATG" 
out embeddedStatement);

#line  1431 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
		} else if (la.kind == 64) {
			lexer.NextToken();
			EmbeddedStatement(
#line  1432 "cs.ATG" 
out embeddedStatement);
			Expect(124);
			Expect(20);
			Expr(
#line  1433 "cs.ATG" 
out expr);
			Expect(21);
			Expect(11);

#line  1433 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.End); 
		} else if (la.kind == 75) {
			lexer.NextToken();

#line  1434 "cs.ATG" 
			List<Statement> initializer = null; List<Statement> iterator = null; 
			Expect(20);
			if (StartOf(5)) {
				ForInitializer(
#line  1435 "cs.ATG" 
out initializer);
			}
			Expect(11);
			if (StartOf(5)) {
				Expr(
#line  1436 "cs.ATG" 
out expr);
			}
			Expect(11);
			if (StartOf(5)) {
				ForIterator(
#line  1437 "cs.ATG" 
out iterator);
			}
			Expect(21);
			EmbeddedStatement(
#line  1438 "cs.ATG" 
out embeddedStatement);

#line  1438 "cs.ATG" 
			statement = new ForStatement(initializer, expr, iterator, embeddedStatement); 
		} else if (la.kind == 76) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1439 "cs.ATG" 
out type);
			Expect(1);

#line  1439 "cs.ATG" 
			string varName = t.val; Location start = t.Location;
			Expect(80);
			Expr(
#line  1440 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1441 "cs.ATG" 
out embeddedStatement);

#line  1441 "cs.ATG" 
			statement = new ForeachStatement(type, varName , expr, embeddedStatement); 
			statement.EndLocation = t.EndLocation;
			
		} else if (la.kind == 52) {
			lexer.NextToken();
			Expect(11);

#line  1445 "cs.ATG" 
			statement = new BreakStatement(); 
		} else if (la.kind == 60) {
			lexer.NextToken();
			Expect(11);

#line  1446 "cs.ATG" 
			statement = new ContinueStatement(); 
		} else if (la.kind == 77) {
			GotoStatement(
#line  1447 "cs.ATG" 
out statement);
		} else if (
#line  1448 "cs.ATG" 
IsYieldStatement()) {
			Expect(1);
			if (la.kind == 100) {
				lexer.NextToken();
				Expr(
#line  1448 "cs.ATG" 
out expr);

#line  1448 "cs.ATG" 
				statement = new YieldStatement(new ReturnStatement(expr)); 
			} else if (la.kind == 52) {
				lexer.NextToken();

#line  1449 "cs.ATG" 
				statement = new YieldStatement(new BreakStatement()); 
			} else SynErr(171);
			Expect(11);
		} else if (la.kind == 100) {
			lexer.NextToken();
			if (StartOf(5)) {
				Expr(
#line  1450 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1450 "cs.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 111) {
			lexer.NextToken();
			if (StartOf(5)) {
				Expr(
#line  1451 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1451 "cs.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (StartOf(5)) {
			StatementExpr(
#line  1454 "cs.ATG" 
out statement);
			Expect(11);
		} else if (la.kind == 113) {
			TryStatement(
#line  1456 "cs.ATG" 
out statement);
		} else if (la.kind == 85) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1458 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1459 "cs.ATG" 
out embeddedStatement);

#line  1459 "cs.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 120) {

#line  1461 "cs.ATG" 
			Statement resourceAcquisitionStmt = null; 
			lexer.NextToken();
			Expect(20);
			ResourceAcquisition(
#line  1463 "cs.ATG" 
out resourceAcquisitionStmt);
			Expect(21);
			EmbeddedStatement(
#line  1464 "cs.ATG" 
out embeddedStatement);

#line  1464 "cs.ATG" 
			statement = new UsingStatement(resourceAcquisitionStmt, embeddedStatement); 
		} else if (la.kind == 118) {
			lexer.NextToken();
			Block(
#line  1466 "cs.ATG" 
out embeddedStatement);

#line  1466 "cs.ATG" 
			statement = new UnsafeStatement(embeddedStatement); 
		} else if (la.kind == 73) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1469 "cs.ATG" 
out type);

#line  1469 "cs.ATG" 
			if (type.PointerNestingLevel == 0) Error("can only fix pointer types");
			List<VariableDeclaration> pointerDeclarators = new List<VariableDeclaration>(1);
			
			Expect(1);

#line  1472 "cs.ATG" 
			string identifier = t.val; 
			Expect(3);
			Expr(
#line  1473 "cs.ATG" 
out expr);

#line  1473 "cs.ATG" 
			pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			while (la.kind == 14) {
				lexer.NextToken();
				Expect(1);

#line  1475 "cs.ATG" 
				identifier = t.val; 
				Expect(3);
				Expr(
#line  1476 "cs.ATG" 
out expr);

#line  1476 "cs.ATG" 
				pointerDeclarators.Add(new VariableDeclaration(identifier, expr)); 
			}
			Expect(21);
			EmbeddedStatement(
#line  1478 "cs.ATG" 
out embeddedStatement);

#line  1478 "cs.ATG" 
			statement = new FixedStatement(type, pointerDeclarators, embeddedStatement); 
		} else SynErr(172);
	}

	void SwitchSections(
#line  1500 "cs.ATG" 
List<SwitchSection> switchSections) {

#line  1502 "cs.ATG" 
		SwitchSection switchSection = new SwitchSection();
		CaseLabel label;
		
		SwitchLabel(
#line  1506 "cs.ATG" 
out label);

#line  1506 "cs.ATG" 
		if (label != null) { switchSection.SwitchLabels.Add(label); } 

#line  1507 "cs.ATG" 
		compilationUnit.BlockStart(switchSection); 
		while (StartOf(25)) {
			if (la.kind == 54 || la.kind == 62) {
				SwitchLabel(
#line  1509 "cs.ATG" 
out label);

#line  1510 "cs.ATG" 
				if (label != null) {
				if (switchSection.Children.Count > 0) {
					// open new section
					compilationUnit.BlockEnd(); switchSections.Add(switchSection);
					switchSection = new SwitchSection();
					compilationUnit.BlockStart(switchSection);
				}
				switchSection.SwitchLabels.Add(label);
				}
				
			} else {
				Statement();
			}
		}

#line  1522 "cs.ATG" 
		compilationUnit.BlockEnd(); switchSections.Add(switchSection); 
	}

	void ForInitializer(
#line  1481 "cs.ATG" 
out List<Statement> initializer) {

#line  1483 "cs.ATG" 
		Statement stmt; 
		initializer = new List<Statement>();
		
		if (
#line  1487 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1487 "cs.ATG" 
out stmt);

#line  1487 "cs.ATG" 
			initializer.Add(stmt);
		} else if (StartOf(5)) {
			StatementExpr(
#line  1488 "cs.ATG" 
out stmt);

#line  1488 "cs.ATG" 
			initializer.Add(stmt);
			while (la.kind == 14) {
				lexer.NextToken();
				StatementExpr(
#line  1488 "cs.ATG" 
out stmt);

#line  1488 "cs.ATG" 
				initializer.Add(stmt);
			}
		} else SynErr(173);
	}

	void ForIterator(
#line  1491 "cs.ATG" 
out List<Statement> iterator) {

#line  1493 "cs.ATG" 
		Statement stmt; 
		iterator = new List<Statement>();
		
		StatementExpr(
#line  1497 "cs.ATG" 
out stmt);

#line  1497 "cs.ATG" 
		iterator.Add(stmt);
		while (la.kind == 14) {
			lexer.NextToken();
			StatementExpr(
#line  1497 "cs.ATG" 
out stmt);

#line  1497 "cs.ATG" 
			iterator.Add(stmt); 
		}
	}

	void GotoStatement(
#line  1575 "cs.ATG" 
out Statement stmt) {

#line  1576 "cs.ATG" 
		Expression expr; stmt = null; 
		Expect(77);
		if (la.kind == 1) {
			lexer.NextToken();

#line  1580 "cs.ATG" 
			stmt = new GotoStatement(t.val); 
			Expect(11);
		} else if (la.kind == 54) {
			lexer.NextToken();
			Expr(
#line  1581 "cs.ATG" 
out expr);
			Expect(11);

#line  1581 "cs.ATG" 
			stmt = new GotoCaseStatement(expr); 
		} else if (la.kind == 62) {
			lexer.NextToken();
			Expect(11);

#line  1582 "cs.ATG" 
			stmt = new GotoCaseStatement(null); 
		} else SynErr(174);
	}

	void StatementExpr(
#line  1602 "cs.ATG" 
out Statement stmt) {

#line  1603 "cs.ATG" 
		Expression expr; 
		Expr(
#line  1605 "cs.ATG" 
out expr);

#line  1608 "cs.ATG" 
		stmt = new ExpressionStatement(expr); 
	}

	void TryStatement(
#line  1532 "cs.ATG" 
out Statement tryStatement) {

#line  1534 "cs.ATG" 
		Statement blockStmt = null, finallyStmt = null;
		List<CatchClause> catchClauses = null;
		
		Expect(113);
		Block(
#line  1538 "cs.ATG" 
out blockStmt);
		if (la.kind == 55) {
			CatchClauses(
#line  1540 "cs.ATG" 
out catchClauses);
			if (la.kind == 72) {
				lexer.NextToken();
				Block(
#line  1540 "cs.ATG" 
out finallyStmt);
			}
		} else if (la.kind == 72) {
			lexer.NextToken();
			Block(
#line  1541 "cs.ATG" 
out finallyStmt);
		} else SynErr(175);

#line  1544 "cs.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
			
	}

	void ResourceAcquisition(
#line  1586 "cs.ATG" 
out Statement stmt) {

#line  1588 "cs.ATG" 
		stmt = null;
		Expression expr;
		
		if (
#line  1593 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1593 "cs.ATG" 
out stmt);
		} else if (StartOf(5)) {
			Expr(
#line  1594 "cs.ATG" 
out expr);

#line  1598 "cs.ATG" 
			stmt = new ExpressionStatement(expr); 
		} else SynErr(176);
	}

	void SwitchLabel(
#line  1525 "cs.ATG" 
out CaseLabel label) {

#line  1526 "cs.ATG" 
		Expression expr = null; label = null; 
		if (la.kind == 54) {
			lexer.NextToken();
			Expr(
#line  1528 "cs.ATG" 
out expr);
			Expect(9);

#line  1528 "cs.ATG" 
			label =  new CaseLabel(expr); 
		} else if (la.kind == 62) {
			lexer.NextToken();
			Expect(9);

#line  1529 "cs.ATG" 
			label =  new CaseLabel(); 
		} else SynErr(177);
	}

	void CatchClauses(
#line  1549 "cs.ATG" 
out List<CatchClause> catchClauses) {

#line  1551 "cs.ATG" 
		catchClauses = new List<CatchClause>();
		
		Expect(55);

#line  1554 "cs.ATG" 
		string identifier;
		Statement stmt;
		TypeReference typeRef;
		
		if (la.kind == 16) {
			Block(
#line  1560 "cs.ATG" 
out stmt);

#line  1560 "cs.ATG" 
			catchClauses.Add(new CatchClause(stmt)); 
		} else if (la.kind == 20) {
			lexer.NextToken();
			ClassType(
#line  1562 "cs.ATG" 
out typeRef, false);

#line  1562 "cs.ATG" 
			identifier = null; 
			if (la.kind == 1) {
				lexer.NextToken();

#line  1563 "cs.ATG" 
				identifier = t.val; 
			}
			Expect(21);
			Block(
#line  1564 "cs.ATG" 
out stmt);

#line  1565 "cs.ATG" 
			catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			while (
#line  1566 "cs.ATG" 
IsTypedCatch()) {
				Expect(55);
				Expect(20);
				ClassType(
#line  1566 "cs.ATG" 
out typeRef, false);

#line  1566 "cs.ATG" 
				identifier = null; 
				if (la.kind == 1) {
					lexer.NextToken();

#line  1567 "cs.ATG" 
					identifier = t.val; 
				}
				Expect(21);
				Block(
#line  1568 "cs.ATG" 
out stmt);

#line  1569 "cs.ATG" 
				catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			}
			if (la.kind == 55) {
				lexer.NextToken();
				Block(
#line  1571 "cs.ATG" 
out stmt);

#line  1571 "cs.ATG" 
				catchClauses.Add(new CatchClause(stmt)); 
			}
		} else SynErr(178);
	}

	void UnaryExpr(
#line  1629 "cs.ATG" 
out Expression uExpr) {

#line  1631 "cs.ATG" 
		TypeReference type = null;
		Expression expr;
		ArrayList  expressions = new ArrayList();
		uExpr = null;
		
		while (StartOf(26) || 
#line  1653 "cs.ATG" 
IsTypeCast()) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  1640 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Plus)); 
			} else if (la.kind == 5) {
				lexer.NextToken();

#line  1641 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Minus)); 
			} else if (la.kind == 24) {
				lexer.NextToken();

#line  1642 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Not)); 
			} else if (la.kind == 27) {
				lexer.NextToken();

#line  1643 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitNot)); 
			} else if (la.kind == 6) {
				lexer.NextToken();

#line  1644 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Star)); 
			} else if (la.kind == 31) {
				lexer.NextToken();

#line  1645 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Increment)); 
			} else if (la.kind == 32) {
				lexer.NextToken();

#line  1646 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Decrement)); 
			} else if (la.kind == 28) {
				lexer.NextToken();

#line  1647 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitWiseAnd)); 
			} else {
				Expect(20);
				Type(
#line  1653 "cs.ATG" 
out type);
				Expect(21);

#line  1653 "cs.ATG" 
				expressions.Add(new CastExpression(type)); 
			}
		}
		PrimaryExpr(
#line  1657 "cs.ATG" 
out expr);

#line  1657 "cs.ATG" 
		for (int i = 0; i < expressions.Count; ++i) {
		Expression nextExpression = i + 1 < expressions.Count ? (Expression)expressions[i + 1] : expr;
		if (expressions[i] is CastExpression) {
			((CastExpression)expressions[i]).Expression = nextExpression;
		} else {
			((UnaryOperatorExpression)expressions[i]).Expression = nextExpression;
		}
		}
		if (expressions.Count > 0) {
			uExpr = (Expression)expressions[0];
		} else {
			uExpr = expr;
		}
		
	}

	void ConditionalOrExpr(
#line  1827 "cs.ATG" 
ref Expression outExpr) {

#line  1828 "cs.ATG" 
		Expression expr;   
		ConditionalAndExpr(
#line  1830 "cs.ATG" 
ref outExpr);
		while (la.kind == 26) {
			lexer.NextToken();
			UnaryExpr(
#line  1830 "cs.ATG" 
out expr);
			ConditionalAndExpr(
#line  1830 "cs.ATG" 
ref expr);

#line  1830 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalOr, expr);  
		}
	}

	void PrimaryExpr(
#line  1674 "cs.ATG" 
out Expression pexpr) {

#line  1676 "cs.ATG" 
		TypeReference type = null;
		List<TypeReference> typeList = null;
		bool isArrayCreation = false;
		Expression expr;
		pexpr = null;
		
		if (la.kind == 112) {
			lexer.NextToken();

#line  1684 "cs.ATG" 
			pexpr = new PrimitiveExpression(true, "true");  
		} else if (la.kind == 71) {
			lexer.NextToken();

#line  1685 "cs.ATG" 
			pexpr = new PrimitiveExpression(false, "false"); 
		} else if (la.kind == 89) {
			lexer.NextToken();

#line  1686 "cs.ATG" 
			pexpr = new PrimitiveExpression(null, "null");  
		} else if (la.kind == 2) {
			lexer.NextToken();

#line  1687 "cs.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
		} else if (
#line  1688 "cs.ATG" 
la.kind == Tokens.Identifier && Peek(1).kind == Tokens.DoubleColon) {
			Expect(1);

#line  1689 "cs.ATG" 
			type = new TypeReference(t.val); 
			Expect(10);

#line  1690 "cs.ATG" 
			pexpr = new TypeReferenceExpression(type); 
			Expect(1);

#line  1691 "cs.ATG" 
			if (type.Type == "global") { type.IsGlobal = true; type.Type = (t.val ?? "?"); } else type.Type += "." + (t.val ?? "?"); 
		} else if (la.kind == 1) {
			lexer.NextToken();

#line  1693 "cs.ATG" 
			pexpr = new IdentifierExpression(t.val); 
		} else if (la.kind == 20) {
			lexer.NextToken();
			Expr(
#line  1695 "cs.ATG" 
out expr);
			Expect(21);

#line  1695 "cs.ATG" 
			pexpr = new ParenthesizedExpression(expr); 
		} else if (StartOf(27)) {

#line  1697 "cs.ATG" 
			string val = null; 
			switch (la.kind) {
			case 51: {
				lexer.NextToken();

#line  1699 "cs.ATG" 
				val = "bool"; 
				break;
			}
			case 53: {
				lexer.NextToken();

#line  1700 "cs.ATG" 
				val = "byte"; 
				break;
			}
			case 56: {
				lexer.NextToken();

#line  1701 "cs.ATG" 
				val = "char"; 
				break;
			}
			case 61: {
				lexer.NextToken();

#line  1702 "cs.ATG" 
				val = "decimal"; 
				break;
			}
			case 65: {
				lexer.NextToken();

#line  1703 "cs.ATG" 
				val = "double"; 
				break;
			}
			case 74: {
				lexer.NextToken();

#line  1704 "cs.ATG" 
				val = "float"; 
				break;
			}
			case 81: {
				lexer.NextToken();

#line  1705 "cs.ATG" 
				val = "int"; 
				break;
			}
			case 86: {
				lexer.NextToken();

#line  1706 "cs.ATG" 
				val = "long"; 
				break;
			}
			case 90: {
				lexer.NextToken();

#line  1707 "cs.ATG" 
				val = "object"; 
				break;
			}
			case 101: {
				lexer.NextToken();

#line  1708 "cs.ATG" 
				val = "sbyte"; 
				break;
			}
			case 103: {
				lexer.NextToken();

#line  1709 "cs.ATG" 
				val = "short"; 
				break;
			}
			case 107: {
				lexer.NextToken();

#line  1710 "cs.ATG" 
				val = "string"; 
				break;
			}
			case 115: {
				lexer.NextToken();

#line  1711 "cs.ATG" 
				val = "uint"; 
				break;
			}
			case 116: {
				lexer.NextToken();

#line  1712 "cs.ATG" 
				val = "ulong"; 
				break;
			}
			case 119: {
				lexer.NextToken();

#line  1713 "cs.ATG" 
				val = "ushort"; 
				break;
			}
			}

#line  1714 "cs.ATG" 
			t.val = ""; 
			Expect(15);
			Expect(1);

#line  1714 "cs.ATG" 
			pexpr = new FieldReferenceExpression(new TypeReferenceExpression(val), t.val); 
		} else if (la.kind == 110) {
			lexer.NextToken();

#line  1716 "cs.ATG" 
			pexpr = new ThisReferenceExpression(); 
		} else if (la.kind == 50) {
			lexer.NextToken();

#line  1718 "cs.ATG" 
			Expression retExpr = new BaseReferenceExpression(); 
			if (la.kind == 15) {
				lexer.NextToken();
				Expect(1);

#line  1720 "cs.ATG" 
				retExpr = new FieldReferenceExpression(retExpr, t.val); 
			} else if (la.kind == 18) {
				lexer.NextToken();
				Expr(
#line  1721 "cs.ATG" 
out expr);

#line  1721 "cs.ATG" 
				List<Expression> indices = new List<Expression>(); if (expr != null) { indices.Add(expr); } 
				while (la.kind == 14) {
					lexer.NextToken();
					Expr(
#line  1722 "cs.ATG" 
out expr);

#line  1722 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(19);

#line  1723 "cs.ATG" 
				retExpr = new IndexerExpression(retExpr, indices); 
			} else SynErr(179);

#line  1724 "cs.ATG" 
			pexpr = retExpr; 
		} else if (la.kind == 88) {
			lexer.NextToken();
			NonArrayType(
#line  1725 "cs.ATG" 
out type);

#line  1726 "cs.ATG" 
			List<Expression> parameters = new List<Expression>(); 
			if (la.kind == 20) {
				lexer.NextToken();

#line  1731 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				if (StartOf(21)) {
					Argument(
#line  1732 "cs.ATG" 
out expr);

#line  1732 "cs.ATG" 
					if (expr != null) { parameters.Add(expr); } 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  1733 "cs.ATG" 
out expr);

#line  1733 "cs.ATG" 
						if (expr != null) { parameters.Add(expr); } 
					}
				}
				Expect(21);

#line  1735 "cs.ATG" 
				pexpr = oce; 
			} else if (la.kind == 18) {
				lexer.NextToken();

#line  1737 "cs.ATG" 
				isArrayCreation = true; ArrayCreateExpression ace = new ArrayCreateExpression(type); pexpr = ace; 

#line  1738 "cs.ATG" 
				int dims = 0; List<int> ranks = new List<int>(); 
				if (la.kind == 14 || la.kind == 19) {
					while (la.kind == 14) {
						lexer.NextToken();

#line  1740 "cs.ATG" 
						dims += 1; 
					}
					Expect(19);

#line  1741 "cs.ATG" 
					ranks.Add(dims); dims = 0; 
					while (la.kind == 18) {
						lexer.NextToken();
						while (la.kind == 14) {
							lexer.NextToken();

#line  1742 "cs.ATG" 
							++dims; 
						}
						Expect(19);

#line  1742 "cs.ATG" 
						ranks.Add(dims); dims = 0; 
					}

#line  1743 "cs.ATG" 
					ace.CreateType.RankSpecifier = ranks.ToArray(); 
					ArrayInitializer(
#line  1744 "cs.ATG" 
out expr);

#line  1744 "cs.ATG" 
					ace.ArrayInitializer = (ArrayInitializerExpression)expr; 
				} else if (StartOf(5)) {
					Expr(
#line  1745 "cs.ATG" 
out expr);

#line  1745 "cs.ATG" 
					if (expr != null) parameters.Add(expr); 
					while (la.kind == 14) {
						lexer.NextToken();

#line  1746 "cs.ATG" 
						dims += 1; 
						Expr(
#line  1747 "cs.ATG" 
out expr);

#line  1747 "cs.ATG" 
						if (expr != null) parameters.Add(expr); 
					}
					Expect(19);

#line  1749 "cs.ATG" 
					ranks.Add(dims); ace.Arguments = parameters; dims = 0; 
					while (la.kind == 18) {
						lexer.NextToken();
						while (la.kind == 14) {
							lexer.NextToken();

#line  1750 "cs.ATG" 
							++dims; 
						}
						Expect(19);

#line  1750 "cs.ATG" 
						ranks.Add(dims); dims = 0; 
					}

#line  1751 "cs.ATG" 
					ace.CreateType.RankSpecifier = ranks.ToArray(); 
					if (la.kind == 16) {
						ArrayInitializer(
#line  1752 "cs.ATG" 
out expr);

#line  1752 "cs.ATG" 
						ace.ArrayInitializer = (ArrayInitializerExpression)expr; 
					}
				} else SynErr(180);
			} else SynErr(181);
		} else if (la.kind == 114) {
			lexer.NextToken();
			Expect(20);
			if (
#line  1757 "cs.ATG" 
NotVoidPointer()) {
				Expect(122);

#line  1757 "cs.ATG" 
				type = new TypeReference("void"); 
			} else if (StartOf(9)) {
				TypeWithRestriction(
#line  1758 "cs.ATG" 
out type, true, true);
			} else SynErr(182);
			Expect(21);

#line  1759 "cs.ATG" 
			pexpr = new TypeOfExpression(type); 
		} else if (la.kind == 62) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1761 "cs.ATG" 
out type);
			Expect(21);

#line  1761 "cs.ATG" 
			pexpr = new DefaultValueExpression(type); 
		} else if (la.kind == 104) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1762 "cs.ATG" 
out type);
			Expect(21);

#line  1762 "cs.ATG" 
			pexpr = new SizeOfExpression(type); 
		} else if (la.kind == 57) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1763 "cs.ATG" 
out expr);
			Expect(21);

#line  1763 "cs.ATG" 
			pexpr = new CheckedExpression(expr); 
		} else if (la.kind == 117) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1764 "cs.ATG" 
out expr);
			Expect(21);

#line  1764 "cs.ATG" 
			pexpr = new UncheckedExpression(expr); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			AnonymousMethodExpr(
#line  1765 "cs.ATG" 
out expr);

#line  1765 "cs.ATG" 
			pexpr = expr; 
		} else SynErr(183);
		while (StartOf(28) || 
#line  1776 "cs.ATG" 
IsGenericFollowedBy(Tokens.Dot) && IsTypeReferenceExpression(pexpr) || 
#line  1785 "cs.ATG" 
IsGenericFollowedBy(Tokens.OpenParenthesis)) {
			if (la.kind == 31 || la.kind == 32) {
				if (la.kind == 31) {
					lexer.NextToken();

#line  1769 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostIncrement); 
				} else if (la.kind == 32) {
					lexer.NextToken();

#line  1770 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostDecrement); 
				} else SynErr(184);
			} else if (la.kind == 47) {
				lexer.NextToken();
				Expect(1);

#line  1773 "cs.ATG" 
				pexpr = new PointerReferenceExpression(pexpr, t.val); 
			} else if (la.kind == 15) {
				lexer.NextToken();
				Expect(1);

#line  1774 "cs.ATG" 
				pexpr = new FieldReferenceExpression(pexpr, t.val);
			} else if (
#line  1776 "cs.ATG" 
IsGenericFollowedBy(Tokens.Dot) && IsTypeReferenceExpression(pexpr)) {
				TypeArgumentList(
#line  1777 "cs.ATG" 
out typeList, false);
				Expect(15);
				Expect(1);

#line  1779 "cs.ATG" 
				pexpr = new FieldReferenceExpression(GetTypeReferenceExpression(pexpr, typeList), t.val);
			} else if (la.kind == 20) {
				lexer.NextToken();

#line  1781 "cs.ATG" 
				List<Expression> parameters = new List<Expression>(); 
				if (StartOf(21)) {
					Argument(
#line  1782 "cs.ATG" 
out expr);

#line  1782 "cs.ATG" 
					if (expr != null) {parameters.Add(expr);} 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  1783 "cs.ATG" 
out expr);

#line  1783 "cs.ATG" 
						if (expr != null) {parameters.Add(expr);} 
					}
				}
				Expect(21);

#line  1784 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters); 
			} else if (
#line  1785 "cs.ATG" 
IsGenericFollowedBy(Tokens.OpenParenthesis)) {
				TypeArgumentList(
#line  1785 "cs.ATG" 
out typeList, false);
				Expect(20);

#line  1786 "cs.ATG" 
				List<Expression> parameters = new List<Expression>(); 
				if (StartOf(21)) {
					Argument(
#line  1787 "cs.ATG" 
out expr);

#line  1787 "cs.ATG" 
					if (expr != null) {parameters.Add(expr);} 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  1788 "cs.ATG" 
out expr);

#line  1788 "cs.ATG" 
						if (expr != null) {parameters.Add(expr);} 
					}
				}
				Expect(21);

#line  1789 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters, typeList); 
			} else {

#line  1791 "cs.ATG" 
				if (isArrayCreation) Error("element access not allow on array creation");
				List<Expression> indices = new List<Expression>();
				
				lexer.NextToken();
				Expr(
#line  1794 "cs.ATG" 
out expr);

#line  1794 "cs.ATG" 
				if (expr != null) { indices.Add(expr); } 
				while (la.kind == 14) {
					lexer.NextToken();
					Expr(
#line  1795 "cs.ATG" 
out expr);

#line  1795 "cs.ATG" 
					if (expr != null) { indices.Add(expr); } 
				}
				Expect(19);

#line  1796 "cs.ATG" 
				pexpr = new IndexerExpression(pexpr, indices); 
			}
		}
	}

	void AnonymousMethodExpr(
#line  1800 "cs.ATG" 
out Expression outExpr) {

#line  1802 "cs.ATG" 
		AnonymousMethodExpression expr = new AnonymousMethodExpression();
		expr.StartLocation = t.Location;
		Statement stmt;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		outExpr = expr;
		
		if (la.kind == 20) {
			lexer.NextToken();
			if (StartOf(10)) {
				FormalParameterList(
#line  1811 "cs.ATG" 
p);

#line  1811 "cs.ATG" 
				expr.Parameters = p; 
			}
			Expect(21);

#line  1813 "cs.ATG" 
			expr.HasParameterList = true; 
		}

#line  1817 "cs.ATG" 
		if (compilationUnit != null) { 
		Block(
#line  1818 "cs.ATG" 
out stmt);

#line  1818 "cs.ATG" 
		expr.Body  = (BlockStatement)stmt; 

#line  1819 "cs.ATG" 
		} else { 
		Expect(16);

#line  1821 "cs.ATG" 
		lexer.SkipCurrentBlock(0); 
		Expect(17);

#line  1823 "cs.ATG" 
		} 

#line  1824 "cs.ATG" 
		expr.EndLocation = t.Location; 
	}

	void TypeArgumentList(
#line  1997 "cs.ATG" 
out List<TypeReference> types, bool canBeUnbound) {

#line  1999 "cs.ATG" 
		types = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(23);
		if (
#line  2004 "cs.ATG" 
canBeUnbound && (la.kind == Tokens.GreaterThan || la.kind == Tokens.Comma)) {

#line  2005 "cs.ATG" 
			types.Add(TypeReference.Null); 
			while (la.kind == 14) {
				lexer.NextToken();

#line  2006 "cs.ATG" 
				types.Add(TypeReference.Null); 
			}
		} else if (StartOf(9)) {
			Type(
#line  2007 "cs.ATG" 
out type);

#line  2007 "cs.ATG" 
			if (type != null) { types.Add(type); } 
			while (la.kind == 14) {
				lexer.NextToken();
				Type(
#line  2008 "cs.ATG" 
out type);

#line  2008 "cs.ATG" 
				if (type != null) { types.Add(type); } 
			}
		} else SynErr(185);
		Expect(22);
	}

	void ConditionalAndExpr(
#line  1833 "cs.ATG" 
ref Expression outExpr) {

#line  1834 "cs.ATG" 
		Expression expr; 
		InclusiveOrExpr(
#line  1836 "cs.ATG" 
ref outExpr);
		while (la.kind == 25) {
			lexer.NextToken();
			UnaryExpr(
#line  1836 "cs.ATG" 
out expr);
			InclusiveOrExpr(
#line  1836 "cs.ATG" 
ref expr);

#line  1836 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalAnd, expr);  
		}
	}

	void InclusiveOrExpr(
#line  1839 "cs.ATG" 
ref Expression outExpr) {

#line  1840 "cs.ATG" 
		Expression expr; 
		ExclusiveOrExpr(
#line  1842 "cs.ATG" 
ref outExpr);
		while (la.kind == 29) {
			lexer.NextToken();
			UnaryExpr(
#line  1842 "cs.ATG" 
out expr);
			ExclusiveOrExpr(
#line  1842 "cs.ATG" 
ref expr);

#line  1842 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseOr, expr);  
		}
	}

	void ExclusiveOrExpr(
#line  1845 "cs.ATG" 
ref Expression outExpr) {

#line  1846 "cs.ATG" 
		Expression expr; 
		AndExpr(
#line  1848 "cs.ATG" 
ref outExpr);
		while (la.kind == 30) {
			lexer.NextToken();
			UnaryExpr(
#line  1848 "cs.ATG" 
out expr);
			AndExpr(
#line  1848 "cs.ATG" 
ref expr);

#line  1848 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.ExclusiveOr, expr);  
		}
	}

	void AndExpr(
#line  1851 "cs.ATG" 
ref Expression outExpr) {

#line  1852 "cs.ATG" 
		Expression expr; 
		EqualityExpr(
#line  1854 "cs.ATG" 
ref outExpr);
		while (la.kind == 28) {
			lexer.NextToken();
			UnaryExpr(
#line  1854 "cs.ATG" 
out expr);
			EqualityExpr(
#line  1854 "cs.ATG" 
ref expr);

#line  1854 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseAnd, expr);  
		}
	}

	void EqualityExpr(
#line  1857 "cs.ATG" 
ref Expression outExpr) {

#line  1859 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		RelationalExpr(
#line  1863 "cs.ATG" 
ref outExpr);
		while (la.kind == 33 || la.kind == 34) {
			if (la.kind == 34) {
				lexer.NextToken();

#line  1866 "cs.ATG" 
				op = BinaryOperatorType.InEquality; 
			} else {
				lexer.NextToken();

#line  1867 "cs.ATG" 
				op = BinaryOperatorType.Equality; 
			}
			UnaryExpr(
#line  1869 "cs.ATG" 
out expr);
			RelationalExpr(
#line  1869 "cs.ATG" 
ref expr);

#line  1869 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void RelationalExpr(
#line  1873 "cs.ATG" 
ref Expression outExpr) {

#line  1875 "cs.ATG" 
		TypeReference type;
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  1880 "cs.ATG" 
ref outExpr);
		while (StartOf(29)) {
			if (StartOf(30)) {
				if (la.kind == 23) {
					lexer.NextToken();

#line  1882 "cs.ATG" 
					op = BinaryOperatorType.LessThan; 
				} else if (la.kind == 22) {
					lexer.NextToken();

#line  1883 "cs.ATG" 
					op = BinaryOperatorType.GreaterThan; 
				} else if (la.kind == 36) {
					lexer.NextToken();

#line  1884 "cs.ATG" 
					op = BinaryOperatorType.LessThanOrEqual; 
				} else if (la.kind == 35) {
					lexer.NextToken();

#line  1885 "cs.ATG" 
					op = BinaryOperatorType.GreaterThanOrEqual; 
				} else SynErr(186);
				UnaryExpr(
#line  1887 "cs.ATG" 
out expr);
				ShiftExpr(
#line  1888 "cs.ATG" 
ref expr);

#line  1889 "cs.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
			} else {
				if (la.kind == 84) {
					lexer.NextToken();
					TypeWithRestriction(
#line  1892 "cs.ATG" 
out type, false, false);
					if (
#line  1893 "cs.ATG" 
la.kind == Tokens.Question && Tokens.CastFollower[Peek(1).kind] == false) {
						NullableQuestionMark(
#line  1894 "cs.ATG" 
ref type);
					}

#line  1895 "cs.ATG" 
					outExpr = new TypeOfIsExpression(outExpr, type); 
				} else if (la.kind == 49) {
					lexer.NextToken();
					TypeWithRestriction(
#line  1897 "cs.ATG" 
out type, false, false);
					if (
#line  1898 "cs.ATG" 
la.kind == Tokens.Question && Tokens.CastFollower[Peek(1).kind] == false) {
						NullableQuestionMark(
#line  1899 "cs.ATG" 
ref type);
					}

#line  1900 "cs.ATG" 
					outExpr = new CastExpression(type, outExpr, CastType.TryCast); 
				} else SynErr(187);
			}
		}
	}

	void ShiftExpr(
#line  1905 "cs.ATG" 
ref Expression outExpr) {

#line  1907 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		AdditiveExpr(
#line  1911 "cs.ATG" 
ref outExpr);
		while (la.kind == 37 || 
#line  1914 "cs.ATG" 
IsShiftRight()) {
			if (la.kind == 37) {
				lexer.NextToken();

#line  1913 "cs.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				Expect(22);
				Expect(22);

#line  1915 "cs.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			UnaryExpr(
#line  1918 "cs.ATG" 
out expr);
			AdditiveExpr(
#line  1918 "cs.ATG" 
ref expr);

#line  1918 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AdditiveExpr(
#line  1922 "cs.ATG" 
ref Expression outExpr) {

#line  1924 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		MultiplicativeExpr(
#line  1928 "cs.ATG" 
ref outExpr);
		while (la.kind == 4 || la.kind == 5) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  1931 "cs.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  1932 "cs.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			UnaryExpr(
#line  1934 "cs.ATG" 
out expr);
			MultiplicativeExpr(
#line  1934 "cs.ATG" 
ref expr);

#line  1934 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void MultiplicativeExpr(
#line  1938 "cs.ATG" 
ref Expression outExpr) {

#line  1940 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		while (la.kind == 6 || la.kind == 7 || la.kind == 8) {
			if (la.kind == 6) {
				lexer.NextToken();

#line  1946 "cs.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else if (la.kind == 7) {
				lexer.NextToken();

#line  1947 "cs.ATG" 
				op = BinaryOperatorType.Divide; 
			} else {
				lexer.NextToken();

#line  1948 "cs.ATG" 
				op = BinaryOperatorType.Modulus; 
			}
			UnaryExpr(
#line  1950 "cs.ATG" 
out expr);

#line  1950 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void TypeParameterConstraintsClauseBase(
#line  2054 "cs.ATG" 
out TypeReference type) {

#line  2055 "cs.ATG" 
		TypeReference t; type = null; 
		if (la.kind == 108) {
			lexer.NextToken();

#line  2057 "cs.ATG" 
			type = TypeReference.StructConstraint; 
		} else if (la.kind == 58) {
			lexer.NextToken();

#line  2058 "cs.ATG" 
			type = TypeReference.ClassConstraint; 
		} else if (la.kind == 88) {
			lexer.NextToken();
			Expect(20);
			Expect(21);

#line  2059 "cs.ATG" 
			type = TypeReference.NewConstraint; 
		} else if (StartOf(9)) {
			Type(
#line  2060 "cs.ATG" 
out t);

#line  2060 "cs.ATG" 
			type = t; 
		} else SynErr(188);
	}


	
	public override void Parse()
	{
		CS();

	}
	
	protected override void SynErr(int line, int col, int errorNumber)
	{
		string s;
		switch (errorNumber) {
			case 0: s = "EOF expected"; break;
			case 1: s = "ident expected"; break;
			case 2: s = "Literal expected"; break;
			case 3: s = "\"=\" expected"; break;
			case 4: s = "\"+\" expected"; break;
			case 5: s = "\"-\" expected"; break;
			case 6: s = "\"*\" expected"; break;
			case 7: s = "\"/\" expected"; break;
			case 8: s = "\"%\" expected"; break;
			case 9: s = "\":\" expected"; break;
			case 10: s = "\"::\" expected"; break;
			case 11: s = "\";\" expected"; break;
			case 12: s = "\"?\" expected"; break;
			case 13: s = "\"??\" expected"; break;
			case 14: s = "\",\" expected"; break;
			case 15: s = "\".\" expected"; break;
			case 16: s = "\"{\" expected"; break;
			case 17: s = "\"}\" expected"; break;
			case 18: s = "\"[\" expected"; break;
			case 19: s = "\"]\" expected"; break;
			case 20: s = "\"(\" expected"; break;
			case 21: s = "\")\" expected"; break;
			case 22: s = "\">\" expected"; break;
			case 23: s = "\"<\" expected"; break;
			case 24: s = "\"!\" expected"; break;
			case 25: s = "\"&&\" expected"; break;
			case 26: s = "\"||\" expected"; break;
			case 27: s = "\"~\" expected"; break;
			case 28: s = "\"&\" expected"; break;
			case 29: s = "\"|\" expected"; break;
			case 30: s = "\"^\" expected"; break;
			case 31: s = "\"++\" expected"; break;
			case 32: s = "\"--\" expected"; break;
			case 33: s = "\"==\" expected"; break;
			case 34: s = "\"!=\" expected"; break;
			case 35: s = "\">=\" expected"; break;
			case 36: s = "\"<=\" expected"; break;
			case 37: s = "\"<<\" expected"; break;
			case 38: s = "\"+=\" expected"; break;
			case 39: s = "\"-=\" expected"; break;
			case 40: s = "\"*=\" expected"; break;
			case 41: s = "\"/=\" expected"; break;
			case 42: s = "\"%=\" expected"; break;
			case 43: s = "\"&=\" expected"; break;
			case 44: s = "\"|=\" expected"; break;
			case 45: s = "\"^=\" expected"; break;
			case 46: s = "\"<<=\" expected"; break;
			case 47: s = "\"->\" expected"; break;
			case 48: s = "\"abstract\" expected"; break;
			case 49: s = "\"as\" expected"; break;
			case 50: s = "\"base\" expected"; break;
			case 51: s = "\"bool\" expected"; break;
			case 52: s = "\"break\" expected"; break;
			case 53: s = "\"byte\" expected"; break;
			case 54: s = "\"case\" expected"; break;
			case 55: s = "\"catch\" expected"; break;
			case 56: s = "\"char\" expected"; break;
			case 57: s = "\"checked\" expected"; break;
			case 58: s = "\"class\" expected"; break;
			case 59: s = "\"const\" expected"; break;
			case 60: s = "\"continue\" expected"; break;
			case 61: s = "\"decimal\" expected"; break;
			case 62: s = "\"default\" expected"; break;
			case 63: s = "\"delegate\" expected"; break;
			case 64: s = "\"do\" expected"; break;
			case 65: s = "\"double\" expected"; break;
			case 66: s = "\"else\" expected"; break;
			case 67: s = "\"enum\" expected"; break;
			case 68: s = "\"event\" expected"; break;
			case 69: s = "\"explicit\" expected"; break;
			case 70: s = "\"extern\" expected"; break;
			case 71: s = "\"false\" expected"; break;
			case 72: s = "\"finally\" expected"; break;
			case 73: s = "\"fixed\" expected"; break;
			case 74: s = "\"float\" expected"; break;
			case 75: s = "\"for\" expected"; break;
			case 76: s = "\"foreach\" expected"; break;
			case 77: s = "\"goto\" expected"; break;
			case 78: s = "\"if\" expected"; break;
			case 79: s = "\"implicit\" expected"; break;
			case 80: s = "\"in\" expected"; break;
			case 81: s = "\"int\" expected"; break;
			case 82: s = "\"interface\" expected"; break;
			case 83: s = "\"internal\" expected"; break;
			case 84: s = "\"is\" expected"; break;
			case 85: s = "\"lock\" expected"; break;
			case 86: s = "\"long\" expected"; break;
			case 87: s = "\"namespace\" expected"; break;
			case 88: s = "\"new\" expected"; break;
			case 89: s = "\"null\" expected"; break;
			case 90: s = "\"object\" expected"; break;
			case 91: s = "\"operator\" expected"; break;
			case 92: s = "\"out\" expected"; break;
			case 93: s = "\"override\" expected"; break;
			case 94: s = "\"params\" expected"; break;
			case 95: s = "\"private\" expected"; break;
			case 96: s = "\"protected\" expected"; break;
			case 97: s = "\"public\" expected"; break;
			case 98: s = "\"readonly\" expected"; break;
			case 99: s = "\"ref\" expected"; break;
			case 100: s = "\"return\" expected"; break;
			case 101: s = "\"sbyte\" expected"; break;
			case 102: s = "\"sealed\" expected"; break;
			case 103: s = "\"short\" expected"; break;
			case 104: s = "\"sizeof\" expected"; break;
			case 105: s = "\"stackalloc\" expected"; break;
			case 106: s = "\"static\" expected"; break;
			case 107: s = "\"string\" expected"; break;
			case 108: s = "\"struct\" expected"; break;
			case 109: s = "\"switch\" expected"; break;
			case 110: s = "\"this\" expected"; break;
			case 111: s = "\"throw\" expected"; break;
			case 112: s = "\"true\" expected"; break;
			case 113: s = "\"try\" expected"; break;
			case 114: s = "\"typeof\" expected"; break;
			case 115: s = "\"uint\" expected"; break;
			case 116: s = "\"ulong\" expected"; break;
			case 117: s = "\"unchecked\" expected"; break;
			case 118: s = "\"unsafe\" expected"; break;
			case 119: s = "\"ushort\" expected"; break;
			case 120: s = "\"using\" expected"; break;
			case 121: s = "\"virtual\" expected"; break;
			case 122: s = "\"void\" expected"; break;
			case 123: s = "\"volatile\" expected"; break;
			case 124: s = "\"while\" expected"; break;
			case 125: s = "??? expected"; break;
			case 126: s = "invalid NamespaceMemberDecl"; break;
			case 127: s = "invalid NonArrayType"; break;
			case 128: s = "invalid AttributeArguments"; break;
			case 129: s = "invalid Expr"; break;
			case 130: s = "invalid TypeModifier"; break;
			case 131: s = "invalid TypeDecl"; break;
			case 132: s = "invalid TypeDecl"; break;
			case 133: s = "invalid IntegralType"; break;
			case 134: s = "invalid FormalParameterList"; break;
			case 135: s = "invalid FormalParameterList"; break;
			case 136: s = "invalid ClassType"; break;
			case 137: s = "invalid ClassMemberDecl"; break;
			case 138: s = "invalid ClassMemberDecl"; break;
			case 139: s = "invalid StructMemberDecl"; break;
			case 140: s = "invalid StructMemberDecl"; break;
			case 141: s = "invalid StructMemberDecl"; break;
			case 142: s = "invalid StructMemberDecl"; break;
			case 143: s = "invalid StructMemberDecl"; break;
			case 144: s = "invalid StructMemberDecl"; break;
			case 145: s = "invalid StructMemberDecl"; break;
			case 146: s = "invalid StructMemberDecl"; break;
			case 147: s = "invalid StructMemberDecl"; break;
			case 148: s = "invalid StructMemberDecl"; break;
			case 149: s = "invalid StructMemberDecl"; break;
			case 150: s = "invalid StructMemberDecl"; break;
			case 151: s = "invalid StructMemberDecl"; break;
			case 152: s = "invalid InterfaceMemberDecl"; break;
			case 153: s = "invalid InterfaceMemberDecl"; break;
			case 154: s = "invalid InterfaceMemberDecl"; break;
			case 155: s = "invalid TypeWithRestriction"; break;
			case 156: s = "invalid TypeWithRestriction"; break;
			case 157: s = "invalid SimpleType"; break;
			case 158: s = "invalid AccessorModifiers"; break;
			case 159: s = "invalid EventAccessorDecls"; break;
			case 160: s = "invalid ConstructorInitializer"; break;
			case 161: s = "invalid OverloadableOperator"; break;
			case 162: s = "invalid AccessorDecls"; break;
			case 163: s = "invalid InterfaceAccessors"; break;
			case 164: s = "invalid InterfaceAccessors"; break;
			case 165: s = "invalid GetAccessorDecl"; break;
			case 166: s = "invalid SetAccessorDecl"; break;
			case 167: s = "invalid VariableInitializer"; break;
			case 168: s = "invalid Statement"; break;
			case 169: s = "invalid AssignmentOperator"; break;
			case 170: s = "invalid EmbeddedStatement"; break;
			case 171: s = "invalid EmbeddedStatement"; break;
			case 172: s = "invalid EmbeddedStatement"; break;
			case 173: s = "invalid ForInitializer"; break;
			case 174: s = "invalid GotoStatement"; break;
			case 175: s = "invalid TryStatement"; break;
			case 176: s = "invalid ResourceAcquisition"; break;
			case 177: s = "invalid SwitchLabel"; break;
			case 178: s = "invalid CatchClauses"; break;
			case 179: s = "invalid PrimaryExpr"; break;
			case 180: s = "invalid PrimaryExpr"; break;
			case 181: s = "invalid PrimaryExpr"; break;
			case 182: s = "invalid PrimaryExpr"; break;
			case 183: s = "invalid PrimaryExpr"; break;
			case 184: s = "invalid PrimaryExpr"; break;
			case 185: s = "invalid TypeArgumentList"; break;
			case 186: s = "invalid RelationalExpr"; break;
			case 187: s = "invalid RelationalExpr"; break;
			case 188: s = "invalid TypeParameterConstraintsClauseBase"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, T,x,x,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, T,x,x,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,T,x,x, T,T,x,x, x,T,T,T, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,T, x,x,T,x, T,x,T,T, T,T,x,T, x,x,x,x, x,x,x},
	{x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, T,T,T,T, T,T,x,T, T,T,T,x, T,T,T,T, x,T,T,T, x,T,T,T, T,T,T,x, x,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,T, x,T,x,x, T,x,T,T, x,T,x,T, x,T,x,T, T,T,T,x, x,T,T,x, x,x,x,T, x,T,T,T, T,x,T,x, T,x,T,x, x,T,x,T, T,T,T,x, x,T,T,T, x,x,T,T, T,x,x,x, x,x,x,T, T,x,T,T, x,T,T,T, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,x,T,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, T,x,T,x, x,x,x,T, x,T,x,T, x,x,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,x,T,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,T,x,x, T,x,T,T, x,T,x,T, x,T,x,T, T,T,T,x, x,T,T,x, x,x,x,T, x,T,T,T, x,x,T,x, T,x,T,x, x,T,x,T, T,T,T,x, x,T,T,T, x,x,T,T, T,x,x,x, x,x,x,T, T,x,T,T, x,T,T,T, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,T,x,x, T,x,T,T, x,T,x,T, x,T,x,T, T,T,T,x, x,T,T,x, x,x,x,T, x,T,T,T, x,x,T,x, T,x,T,x, x,T,x,T, T,T,T,x, x,T,T,T, x,x,T,T, T,x,x,x, x,x,x,T, T,x,T,T, x,T,T,T, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,x,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,x,T,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, T,x,x,x, x,x,x,T, x,T,x,T, x,x,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,x,T,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,T, T,T,T,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,T, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,T,T, x,T,x,T, x,T,x,T, T,T,x,x, x,x,T,x, x,x,x,T, x,T,T,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,T, T,x,x,x, x,x,x,T, T,x,x,T, x,x,T,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,x,T,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, T,T,x,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,T, T,T,T,x, x,T,x,x, x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, T,T,x,T, T,x,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,T,x,x, T,T,x,x, x,T,T,T, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,T,T,x, T,x,x,x, x,x,x,T, x,T,x,T, T,x,x,T, x,x,T,x, T,x,T,T, T,T,x,T, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,x,x, T,T,x,x, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,T, T,T,T,x, x,T,x,x, x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, T,T,x,T, T,x,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,T,x,x, T,T,x,x, x,T,T,T, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,T,x,T, T,T,x,T, x,x,T,x, T,x,T,T, T,T,x,T, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, T,T,x,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,T, T,T,T,x, x,T,x,x, x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, T,T,x,T, T,x,x,T, x,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, T,x,x},
	{x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,T, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x}

	};
} // end Parser

}