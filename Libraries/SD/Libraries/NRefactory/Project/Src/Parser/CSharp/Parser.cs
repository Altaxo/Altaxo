
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
	const int maxT = 145;

	const  bool   T            = true;
	const  bool   x            = false;
	

#line  18 "cs.ATG" 


/*

*/

	void CS() {

#line  179 "cs.ATG" 
		lexer.NextToken(); /* get the first token */ 
		while (la.kind == 121) {
			UsingDirective();
		}
		while (
#line  182 "cs.ATG" 
IsGlobalAttrTarget()) {
			GlobalAttributeSection();
		}
		while (StartOf(1)) {
			NamespaceMemberDecl();
		}
		Expect(0);
	}

	void UsingDirective() {

#line  189 "cs.ATG" 
		string qualident = null; TypeReference aliasedType = null;
		
		Expect(121);

#line  192 "cs.ATG" 
		Location startPos = t.Location; 
		Qualident(
#line  193 "cs.ATG" 
out qualident);
		if (la.kind == 3) {
			lexer.NextToken();
			NonArrayType(
#line  194 "cs.ATG" 
out aliasedType);
		}
		Expect(11);

#line  196 "cs.ATG" 
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

#line  212 "cs.ATG" 
		Location startPos = t.Location; 
		Identifier();

#line  213 "cs.ATG" 
		if (t.val != "assembly" && t.val != "module") Error("global attribute target specifier (assembly or module) expected");
		string attributeTarget = t.val;
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		Expect(9);
		Attribute(
#line  218 "cs.ATG" 
out attribute);

#line  218 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  219 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  219 "cs.ATG" 
out attribute);

#line  219 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  221 "cs.ATG" 
		AttributeSection section = new AttributeSection {
		   AttributeTarget = attributeTarget,
		   Attributes = attributes,
		   StartLocation = startPos,
		   EndLocation = t.EndLocation
		};
		compilationUnit.AddChild(section);
		
	}

	void NamespaceMemberDecl() {

#line  322 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		ModifierList m = new ModifierList();
		string qualident;
		
		if (la.kind == 88) {
			lexer.NextToken();

#line  328 "cs.ATG" 
			Location startPos = t.Location; 
			Qualident(
#line  329 "cs.ATG" 
out qualident);

#line  329 "cs.ATG" 
			INode node =  new NamespaceDeclaration(qualident);
			node.StartLocation = startPos;
			compilationUnit.AddChild(node);
			compilationUnit.BlockStart(node);
			
			Expect(16);
			while (la.kind == 121) {
				UsingDirective();
			}
			while (StartOf(1)) {
				NamespaceMemberDecl();
			}
			Expect(17);
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  338 "cs.ATG" 
			node.EndLocation   = t.EndLocation;
			compilationUnit.BlockEnd();
			
		} else if (StartOf(2)) {
			while (la.kind == 18) {
				AttributeSection(
#line  342 "cs.ATG" 
out section);

#line  342 "cs.ATG" 
				attributes.Add(section); 
			}
			while (StartOf(3)) {
				TypeModifier(
#line  343 "cs.ATG" 
m);
			}
			TypeDecl(
#line  344 "cs.ATG" 
m, attributes);
		} else SynErr(146);
	}

	void Qualident(
#line  468 "cs.ATG" 
out string qualident) {
		Identifier();

#line  470 "cs.ATG" 
		qualidentBuilder.Length = 0; qualidentBuilder.Append(t.val); 
		while (
#line  471 "cs.ATG" 
DotAndIdent()) {
			Expect(15);
			Identifier();

#line  471 "cs.ATG" 
			qualidentBuilder.Append('.');
			qualidentBuilder.Append(t.val); 
			
		}

#line  474 "cs.ATG" 
		qualident = qualidentBuilder.ToString(); 
	}

	void NonArrayType(
#line  583 "cs.ATG" 
out TypeReference type) {

#line  585 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (StartOf(4)) {
			ClassType(
#line  590 "cs.ATG" 
out type, false);
		} else if (StartOf(5)) {
			SimpleType(
#line  591 "cs.ATG" 
out name);

#line  591 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 123) {
			lexer.NextToken();
			Expect(6);

#line  592 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(147);
		if (la.kind == 12) {
			NullableQuestionMark(
#line  595 "cs.ATG" 
ref type);
		}
		while (
#line  597 "cs.ATG" 
IsPointer()) {
			Expect(6);

#line  598 "cs.ATG" 
			++pointer; 
		}

#line  600 "cs.ATG" 
		if (type != null) { type.PointerNestingLevel = pointer; } 
	}

	void Identifier() {
		switch (la.kind) {
		case 1: {
			lexer.NextToken();
			break;
		}
		case 126: {
			lexer.NextToken();
			break;
		}
		case 127: {
			lexer.NextToken();
			break;
		}
		case 128: {
			lexer.NextToken();
			break;
		}
		case 129: {
			lexer.NextToken();
			break;
		}
		case 130: {
			lexer.NextToken();
			break;
		}
		case 131: {
			lexer.NextToken();
			break;
		}
		case 132: {
			lexer.NextToken();
			break;
		}
		case 133: {
			lexer.NextToken();
			break;
		}
		case 134: {
			lexer.NextToken();
			break;
		}
		case 135: {
			lexer.NextToken();
			break;
		}
		case 136: {
			lexer.NextToken();
			break;
		}
		case 137: {
			lexer.NextToken();
			break;
		}
		case 138: {
			lexer.NextToken();
			break;
		}
		case 139: {
			lexer.NextToken();
			break;
		}
		case 140: {
			lexer.NextToken();
			break;
		}
		case 141: {
			lexer.NextToken();
			break;
		}
		case 142: {
			lexer.NextToken();
			break;
		}
		case 143: {
			lexer.NextToken();
			break;
		}
		case 144: {
			lexer.NextToken();
			break;
		}
		default: SynErr(148); break;
		}
	}

	void Attribute(
#line  231 "cs.ATG" 
out ASTAttribute attribute) {

#line  232 "cs.ATG" 
		string qualident;
		string alias = null;
		

#line  236 "cs.ATG" 
		Location startPos = la.Location; 
		if (
#line  237 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  238 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  241 "cs.ATG" 
out qualident);

#line  242 "cs.ATG" 
		List<Expression> positional = new List<Expression>();
		List<NamedArgumentExpression> named = new List<NamedArgumentExpression>();
		string name = (alias != null && alias != "global") ? alias + "." + qualident : qualident;
		
		if (la.kind == 20) {
			AttributeArguments(
#line  246 "cs.ATG" 
positional, named);
		}

#line  247 "cs.ATG" 
		attribute = new ASTAttribute(name, positional, named); 
		attribute.StartLocation = startPos;
		attribute.EndLocation = t.EndLocation;
		
	}

	void AttributeArguments(
#line  253 "cs.ATG" 
List<Expression> positional, List<NamedArgumentExpression> named) {

#line  255 "cs.ATG" 
		bool nameFound = false;
		string name = "";
		Expression expr;
		
		Expect(20);
		if (StartOf(6)) {
			if (
#line  263 "cs.ATG" 
IsAssignment()) {

#line  263 "cs.ATG" 
				nameFound = true; 
				Identifier();

#line  264 "cs.ATG" 
				name = t.val; 
				Expect(3);
			}
			Expr(
#line  266 "cs.ATG" 
out expr);

#line  266 "cs.ATG" 
			if (expr != null) {if(name == "") positional.Add(expr);
			else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
			}
			
			while (la.kind == 14) {
				lexer.NextToken();
				if (
#line  274 "cs.ATG" 
IsAssignment()) {

#line  274 "cs.ATG" 
					nameFound = true; 
					Identifier();

#line  275 "cs.ATG" 
					name = t.val; 
					Expect(3);
				} else if (StartOf(6)) {

#line  277 "cs.ATG" 
					if (nameFound) Error("no positional argument after named argument"); 
				} else SynErr(149);
				Expr(
#line  278 "cs.ATG" 
out expr);

#line  278 "cs.ATG" 
				if (expr != null) { if(name == "") positional.Add(expr);
				else { named.Add(new NamedArgumentExpression(name, expr)); name = ""; }
				}
				
			}
		}
		Expect(21);
	}

	void Expr(
#line  1720 "cs.ATG" 
out Expression expr) {

#line  1721 "cs.ATG" 
		expr = null; Expression expr1 = null, expr2 = null; AssignmentOperatorType op; 

#line  1723 "cs.ATG" 
		Location startLocation = la.Location; 
		UnaryExpr(
#line  1724 "cs.ATG" 
out expr);
		if (StartOf(7)) {
			AssignmentOperator(
#line  1727 "cs.ATG" 
out op);
			Expr(
#line  1727 "cs.ATG" 
out expr1);

#line  1727 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (
#line  1728 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			AssignmentOperator(
#line  1729 "cs.ATG" 
out op);
			Expr(
#line  1729 "cs.ATG" 
out expr1);

#line  1729 "cs.ATG" 
			expr = new AssignmentExpression(expr, op, expr1); 
		} else if (StartOf(8)) {
			ConditionalOrExpr(
#line  1731 "cs.ATG" 
ref expr);
			if (la.kind == 13) {
				lexer.NextToken();
				Expr(
#line  1732 "cs.ATG" 
out expr1);

#line  1732 "cs.ATG" 
				expr = new BinaryOperatorExpression(expr, BinaryOperatorType.NullCoalescing, expr1); 
			}
			if (la.kind == 12) {
				lexer.NextToken();
				Expr(
#line  1733 "cs.ATG" 
out expr1);
				Expect(9);
				Expr(
#line  1733 "cs.ATG" 
out expr2);

#line  1733 "cs.ATG" 
				expr = new ConditionalExpression(expr, expr1, expr2);  
			}
		} else SynErr(150);

#line  1736 "cs.ATG" 
		if (expr != null) {
		expr.StartLocation = startLocation;
		expr.EndLocation = t.EndLocation;
		}
		
	}

	void AttributeSection(
#line  287 "cs.ATG" 
out AttributeSection section) {

#line  289 "cs.ATG" 
		string attributeTarget = "";
		List<ASTAttribute> attributes = new List<ASTAttribute>();
		ASTAttribute attribute;
		
		
		Expect(18);

#line  295 "cs.ATG" 
		Location startPos = t.Location; 
		if (
#line  296 "cs.ATG" 
IsLocalAttrTarget()) {
			if (la.kind == 69) {
				lexer.NextToken();

#line  297 "cs.ATG" 
				attributeTarget = "event";
			} else if (la.kind == 101) {
				lexer.NextToken();

#line  298 "cs.ATG" 
				attributeTarget = "return";
			} else {
				Identifier();

#line  299 "cs.ATG" 
				if (t.val != "field"   && t.val != "method" &&
				  t.val != "param" &&
				  t.val != "property" && t.val != "type")
				Error("attribute target specifier (field, event, method, param, property, return or type) expected");
				attributeTarget = t.val;
				
			}
			Expect(9);
		}
		Attribute(
#line  308 "cs.ATG" 
out attribute);

#line  308 "cs.ATG" 
		attributes.Add(attribute); 
		while (
#line  309 "cs.ATG" 
NotFinalComma()) {
			Expect(14);
			Attribute(
#line  309 "cs.ATG" 
out attribute);

#line  309 "cs.ATG" 
			attributes.Add(attribute); 
		}
		if (la.kind == 14) {
			lexer.NextToken();
		}
		Expect(19);

#line  311 "cs.ATG" 
		section = new AttributeSection {
		   AttributeTarget = attributeTarget,
		   Attributes = attributes,
		   StartLocation = startPos,
		   EndLocation = t.EndLocation
		};
		
	}

	void TypeModifier(
#line  670 "cs.ATG" 
ModifierList m) {
		switch (la.kind) {
		case 89: {
			lexer.NextToken();

#line  672 "cs.ATG" 
			m.Add(Modifiers.New, t.Location); 
			break;
		}
		case 98: {
			lexer.NextToken();

#line  673 "cs.ATG" 
			m.Add(Modifiers.Public, t.Location); 
			break;
		}
		case 97: {
			lexer.NextToken();

#line  674 "cs.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			break;
		}
		case 84: {
			lexer.NextToken();

#line  675 "cs.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			break;
		}
		case 96: {
			lexer.NextToken();

#line  676 "cs.ATG" 
			m.Add(Modifiers.Private, t.Location); 
			break;
		}
		case 119: {
			lexer.NextToken();

#line  677 "cs.ATG" 
			m.Add(Modifiers.Unsafe, t.Location); 
			break;
		}
		case 49: {
			lexer.NextToken();

#line  678 "cs.ATG" 
			m.Add(Modifiers.Abstract, t.Location); 
			break;
		}
		case 103: {
			lexer.NextToken();

#line  679 "cs.ATG" 
			m.Add(Modifiers.Sealed, t.Location); 
			break;
		}
		case 107: {
			lexer.NextToken();

#line  680 "cs.ATG" 
			m.Add(Modifiers.Static, t.Location); 
			break;
		}
		case 126: {
			lexer.NextToken();

#line  681 "cs.ATG" 
			m.Add(Modifiers.Partial, t.Location); 
			break;
		}
		default: SynErr(151); break;
		}
	}

	void TypeDecl(
#line  347 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  349 "cs.ATG" 
		TypeReference type;
		List<TypeReference> names;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		string name;
		List<TemplateDefinition> templates;
		
		if (la.kind == 59) {

#line  355 "cs.ATG" 
			m.Check(Modifiers.Classes); 
			lexer.NextToken();

#line  356 "cs.ATG" 
			TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
			templates = newType.Templates;
			compilationUnit.AddChild(newType);
			compilationUnit.BlockStart(newType);
			newType.StartLocation = m.GetDeclarationLocation(t.Location);
			
			newType.Type = Types.Class;
			
			Identifier();

#line  364 "cs.ATG" 
			newType.Name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  367 "cs.ATG" 
templates);
			}
			if (la.kind == 9) {
				ClassBase(
#line  369 "cs.ATG" 
out names);

#line  369 "cs.ATG" 
				newType.BaseTypes = names; 
			}
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  372 "cs.ATG" 
templates);
			}

#line  374 "cs.ATG" 
			newType.BodyStartLocation = t.EndLocation; 
			Expect(16);
			ClassBody();
			Expect(17);
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  378 "cs.ATG" 
			newType.EndLocation = t.Location; 
			compilationUnit.BlockEnd();
			
		} else if (StartOf(9)) {

#line  381 "cs.ATG" 
			m.Check(Modifiers.StructsInterfacesEnumsDelegates); 
			if (la.kind == 109) {
				lexer.NextToken();

#line  382 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.Type = Types.Struct; 
				
				Identifier();

#line  389 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  392 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					StructInterfaces(
#line  394 "cs.ATG" 
out names);

#line  394 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  397 "cs.ATG" 
templates);
				}

#line  400 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				StructBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  402 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 83) {
				lexer.NextToken();

#line  406 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				templates = newType.Templates;
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Interface;
				
				Identifier();

#line  413 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  416 "cs.ATG" 
templates);
				}
				if (la.kind == 9) {
					InterfaceBase(
#line  418 "cs.ATG" 
out names);

#line  418 "cs.ATG" 
					newType.BaseTypes = names; 
				}
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  421 "cs.ATG" 
templates);
				}

#line  423 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				InterfaceBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  425 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else if (la.kind == 68) {
				lexer.NextToken();

#line  429 "cs.ATG" 
				TypeDeclaration newType = new TypeDeclaration(m.Modifier, attributes);
				compilationUnit.AddChild(newType);
				compilationUnit.BlockStart(newType);
				newType.StartLocation = m.GetDeclarationLocation(t.Location);
				newType.Type = Types.Enum;
				
				Identifier();

#line  435 "cs.ATG" 
				newType.Name = t.val; 
				if (la.kind == 9) {
					lexer.NextToken();
					IntegralType(
#line  436 "cs.ATG" 
out name);

#line  436 "cs.ATG" 
					newType.BaseTypes.Add(new TypeReference(name)); 
				}

#line  438 "cs.ATG" 
				newType.BodyStartLocation = t.EndLocation; 
				EnumBody();
				if (la.kind == 11) {
					lexer.NextToken();
				}

#line  440 "cs.ATG" 
				newType.EndLocation = t.Location; 
				compilationUnit.BlockEnd();
				
			} else {
				lexer.NextToken();

#line  444 "cs.ATG" 
				DelegateDeclaration delegateDeclr = new DelegateDeclaration(m.Modifier, attributes);
				templates = delegateDeclr.Templates;
				delegateDeclr.StartLocation = m.GetDeclarationLocation(t.Location);
				
				if (
#line  448 "cs.ATG" 
NotVoidPointer()) {
					Expect(123);

#line  448 "cs.ATG" 
					delegateDeclr.ReturnType = new TypeReference("void", 0, null); 
				} else if (StartOf(10)) {
					Type(
#line  449 "cs.ATG" 
out type);

#line  449 "cs.ATG" 
					delegateDeclr.ReturnType = type; 
				} else SynErr(152);
				Identifier();

#line  451 "cs.ATG" 
				delegateDeclr.Name = t.val; 
				if (la.kind == 23) {
					TypeParameterList(
#line  454 "cs.ATG" 
templates);
				}
				Expect(20);
				if (StartOf(11)) {
					FormalParameterList(
#line  456 "cs.ATG" 
p);

#line  456 "cs.ATG" 
					delegateDeclr.Parameters = p; 
				}
				Expect(21);
				while (la.kind == 127) {
					TypeParameterConstraintsClause(
#line  460 "cs.ATG" 
templates);
				}
				Expect(11);

#line  462 "cs.ATG" 
				delegateDeclr.EndLocation = t.Location;
				compilationUnit.AddChild(delegateDeclr);
				
			}
		} else SynErr(153);
	}

	void TypeParameterList(
#line  2289 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2291 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		Expect(23);
		while (la.kind == 18) {
			AttributeSection(
#line  2295 "cs.ATG" 
out section);

#line  2295 "cs.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  2296 "cs.ATG" 
		templates.Add(new TemplateDefinition(t.val, attributes)); 
		while (la.kind == 14) {
			lexer.NextToken();
			while (la.kind == 18) {
				AttributeSection(
#line  2297 "cs.ATG" 
out section);

#line  2297 "cs.ATG" 
				attributes.Add(section); 
			}
			Identifier();

#line  2298 "cs.ATG" 
			templates.Add(new TemplateDefinition(t.val, attributes)); 
		}
		Expect(22);
	}

	void ClassBase(
#line  477 "cs.ATG" 
out List<TypeReference> names) {

#line  479 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		ClassType(
#line  483 "cs.ATG" 
out typeRef, false);

#line  483 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  484 "cs.ATG" 
out typeRef, false);

#line  484 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void TypeParameterConstraintsClause(
#line  2302 "cs.ATG" 
List<TemplateDefinition> templates) {

#line  2303 "cs.ATG" 
		string name = ""; TypeReference type; 
		Expect(127);
		Identifier();

#line  2306 "cs.ATG" 
		name = t.val; 
		Expect(9);
		TypeParameterConstraintsClauseBase(
#line  2308 "cs.ATG" 
out type);

#line  2309 "cs.ATG" 
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
#line  2318 "cs.ATG" 
out type);

#line  2319 "cs.ATG" 
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

#line  488 "cs.ATG" 
		AttributeSection section; 
		while (StartOf(12)) {

#line  490 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (!(StartOf(13))) {SynErr(154); lexer.NextToken(); }
			while (la.kind == 18) {
				AttributeSection(
#line  494 "cs.ATG" 
out section);

#line  494 "cs.ATG" 
				attributes.Add(section); 
			}
			MemberModifiers(
#line  495 "cs.ATG" 
m);
			ClassMemberDecl(
#line  496 "cs.ATG" 
m, attributes);
		}
	}

	void StructInterfaces(
#line  500 "cs.ATG" 
out List<TypeReference> names) {

#line  502 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  506 "cs.ATG" 
out typeRef, false);

#line  506 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  507 "cs.ATG" 
out typeRef, false);

#line  507 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void StructBody() {

#line  511 "cs.ATG" 
		AttributeSection section; 
		Expect(16);
		while (StartOf(14)) {

#line  514 "cs.ATG" 
			List<AttributeSection> attributes = new List<AttributeSection>();
			ModifierList m = new ModifierList();
			
			while (la.kind == 18) {
				AttributeSection(
#line  517 "cs.ATG" 
out section);

#line  517 "cs.ATG" 
				attributes.Add(section); 
			}
			MemberModifiers(
#line  518 "cs.ATG" 
m);
			StructMemberDecl(
#line  519 "cs.ATG" 
m, attributes);
		}
		Expect(17);
	}

	void InterfaceBase(
#line  524 "cs.ATG" 
out List<TypeReference> names) {

#line  526 "cs.ATG" 
		TypeReference typeRef;
		names = new List<TypeReference>();
		
		Expect(9);
		TypeName(
#line  530 "cs.ATG" 
out typeRef, false);

#line  530 "cs.ATG" 
		if (typeRef != null) { names.Add(typeRef); } 
		while (la.kind == 14) {
			lexer.NextToken();
			TypeName(
#line  531 "cs.ATG" 
out typeRef, false);

#line  531 "cs.ATG" 
			if (typeRef != null) { names.Add(typeRef); } 
		}
	}

	void InterfaceBody() {
		Expect(16);
		while (StartOf(15)) {
			while (!(StartOf(16))) {SynErr(155); lexer.NextToken(); }
			InterfaceMemberDecl();
		}
		Expect(17);
	}

	void IntegralType(
#line  692 "cs.ATG" 
out string name) {

#line  692 "cs.ATG" 
		name = ""; 
		switch (la.kind) {
		case 102: {
			lexer.NextToken();

#line  694 "cs.ATG" 
			name = "sbyte"; 
			break;
		}
		case 54: {
			lexer.NextToken();

#line  695 "cs.ATG" 
			name = "byte"; 
			break;
		}
		case 104: {
			lexer.NextToken();

#line  696 "cs.ATG" 
			name = "short"; 
			break;
		}
		case 120: {
			lexer.NextToken();

#line  697 "cs.ATG" 
			name = "ushort"; 
			break;
		}
		case 82: {
			lexer.NextToken();

#line  698 "cs.ATG" 
			name = "int"; 
			break;
		}
		case 116: {
			lexer.NextToken();

#line  699 "cs.ATG" 
			name = "uint"; 
			break;
		}
		case 87: {
			lexer.NextToken();

#line  700 "cs.ATG" 
			name = "long"; 
			break;
		}
		case 117: {
			lexer.NextToken();

#line  701 "cs.ATG" 
			name = "ulong"; 
			break;
		}
		case 57: {
			lexer.NextToken();

#line  702 "cs.ATG" 
			name = "char"; 
			break;
		}
		default: SynErr(156); break;
		}
	}

	void EnumBody() {

#line  540 "cs.ATG" 
		FieldDeclaration f; 
		Expect(16);
		if (StartOf(17)) {
			EnumMemberDecl(
#line  543 "cs.ATG" 
out f);

#line  543 "cs.ATG" 
			compilationUnit.AddChild(f); 
			while (
#line  544 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				EnumMemberDecl(
#line  545 "cs.ATG" 
out f);

#line  545 "cs.ATG" 
				compilationUnit.AddChild(f); 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);
	}

	void Type(
#line  551 "cs.ATG" 
out TypeReference type) {
		TypeWithRestriction(
#line  553 "cs.ATG" 
out type, true, false);
	}

	void FormalParameterList(
#line  614 "cs.ATG" 
List<ParameterDeclarationExpression> parameter) {

#line  617 "cs.ATG" 
		ParameterDeclarationExpression p;
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		
		while (la.kind == 18) {
			AttributeSection(
#line  622 "cs.ATG" 
out section);

#line  622 "cs.ATG" 
			attributes.Add(section); 
		}
		if (StartOf(18)) {
			FixedParameter(
#line  624 "cs.ATG" 
out p);

#line  624 "cs.ATG" 
			bool paramsFound = false;
			p.Attributes = attributes;
			parameter.Add(p);
			
			while (la.kind == 14) {
				lexer.NextToken();

#line  629 "cs.ATG" 
				attributes = new List<AttributeSection>(); if (paramsFound) Error("params array must be at end of parameter list"); 
				while (la.kind == 18) {
					AttributeSection(
#line  630 "cs.ATG" 
out section);

#line  630 "cs.ATG" 
					attributes.Add(section); 
				}
				if (StartOf(18)) {
					FixedParameter(
#line  632 "cs.ATG" 
out p);

#line  632 "cs.ATG" 
					p.Attributes = attributes; parameter.Add(p); 
				} else if (la.kind == 95) {
					ParameterArray(
#line  633 "cs.ATG" 
out p);

#line  633 "cs.ATG" 
					paramsFound = true; p.Attributes = attributes; parameter.Add(p); 
				} else SynErr(157);
			}
		} else if (la.kind == 95) {
			ParameterArray(
#line  636 "cs.ATG" 
out p);

#line  636 "cs.ATG" 
			p.Attributes = attributes; parameter.Add(p); 
		} else SynErr(158);
	}

	void ClassType(
#line  684 "cs.ATG" 
out TypeReference typeRef, bool canBeUnbound) {

#line  685 "cs.ATG" 
		TypeReference r; typeRef = null; 
		if (StartOf(19)) {
			TypeName(
#line  687 "cs.ATG" 
out r, canBeUnbound);

#line  687 "cs.ATG" 
			typeRef = r; 
		} else if (la.kind == 91) {
			lexer.NextToken();

#line  688 "cs.ATG" 
			typeRef = new TypeReference("object"); 
		} else if (la.kind == 108) {
			lexer.NextToken();

#line  689 "cs.ATG" 
			typeRef = new TypeReference("string"); 
		} else SynErr(159);
	}

	void TypeName(
#line  2232 "cs.ATG" 
out TypeReference typeRef, bool canBeUnbound) {

#line  2233 "cs.ATG" 
		List<TypeReference> typeArguments = null;
		string alias = null;
		string qualident;
		
		if (
#line  2238 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  2239 "cs.ATG" 
			alias = t.val; 
			Expect(10);
		}
		Qualident(
#line  2242 "cs.ATG" 
out qualident);
		if (la.kind == 23) {
			TypeArgumentList(
#line  2243 "cs.ATG" 
out typeArguments, canBeUnbound);
		}

#line  2245 "cs.ATG" 
		if (alias == null) {
		typeRef = new TypeReference(qualident, typeArguments);
		} else if (alias == "global") {
			typeRef = new TypeReference(qualident, typeArguments);
			typeRef.IsGlobal = true;
		} else {
			typeRef = new TypeReference(alias + "." + qualident, typeArguments);
		}
		
		while (
#line  2254 "cs.ATG" 
DotAndIdent()) {
			Expect(15);

#line  2255 "cs.ATG" 
			typeArguments = null; 
			Qualident(
#line  2256 "cs.ATG" 
out qualident);
			if (la.kind == 23) {
				TypeArgumentList(
#line  2257 "cs.ATG" 
out typeArguments, canBeUnbound);
			}

#line  2258 "cs.ATG" 
			typeRef = new InnerClassTypeReference(typeRef, qualident, typeArguments); 
		}
	}

	void MemberModifiers(
#line  705 "cs.ATG" 
ModifierList m) {
		while (StartOf(20)) {
			switch (la.kind) {
			case 49: {
				lexer.NextToken();

#line  708 "cs.ATG" 
				m.Add(Modifiers.Abstract, t.Location); 
				break;
			}
			case 71: {
				lexer.NextToken();

#line  709 "cs.ATG" 
				m.Add(Modifiers.Extern, t.Location); 
				break;
			}
			case 84: {
				lexer.NextToken();

#line  710 "cs.ATG" 
				m.Add(Modifiers.Internal, t.Location); 
				break;
			}
			case 89: {
				lexer.NextToken();

#line  711 "cs.ATG" 
				m.Add(Modifiers.New, t.Location); 
				break;
			}
			case 94: {
				lexer.NextToken();

#line  712 "cs.ATG" 
				m.Add(Modifiers.Override, t.Location); 
				break;
			}
			case 96: {
				lexer.NextToken();

#line  713 "cs.ATG" 
				m.Add(Modifiers.Private, t.Location); 
				break;
			}
			case 97: {
				lexer.NextToken();

#line  714 "cs.ATG" 
				m.Add(Modifiers.Protected, t.Location); 
				break;
			}
			case 98: {
				lexer.NextToken();

#line  715 "cs.ATG" 
				m.Add(Modifiers.Public, t.Location); 
				break;
			}
			case 99: {
				lexer.NextToken();

#line  716 "cs.ATG" 
				m.Add(Modifiers.ReadOnly, t.Location); 
				break;
			}
			case 103: {
				lexer.NextToken();

#line  717 "cs.ATG" 
				m.Add(Modifiers.Sealed, t.Location); 
				break;
			}
			case 107: {
				lexer.NextToken();

#line  718 "cs.ATG" 
				m.Add(Modifiers.Static, t.Location); 
				break;
			}
			case 74: {
				lexer.NextToken();

#line  719 "cs.ATG" 
				m.Add(Modifiers.Fixed, t.Location); 
				break;
			}
			case 119: {
				lexer.NextToken();

#line  720 "cs.ATG" 
				m.Add(Modifiers.Unsafe, t.Location); 
				break;
			}
			case 122: {
				lexer.NextToken();

#line  721 "cs.ATG" 
				m.Add(Modifiers.Virtual, t.Location); 
				break;
			}
			case 124: {
				lexer.NextToken();

#line  722 "cs.ATG" 
				m.Add(Modifiers.Volatile, t.Location); 
				break;
			}
			case 126: {
				lexer.NextToken();

#line  723 "cs.ATG" 
				m.Add(Modifiers.Partial, t.Location); 
				break;
			}
			}
		}
	}

	void ClassMemberDecl(
#line  1032 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  1033 "cs.ATG" 
		Statement stmt = null; 
		if (StartOf(21)) {
			StructMemberDecl(
#line  1035 "cs.ATG" 
m, attributes);
		} else if (la.kind == 27) {

#line  1036 "cs.ATG" 
			m.Check(Modifiers.Destructors); Location startPos = t.Location; 
			lexer.NextToken();
			Identifier();

#line  1037 "cs.ATG" 
			DestructorDeclaration d = new DestructorDeclaration(t.val, m.Modifier, attributes); 
			d.Modifier = m.Modifier;
			d.StartLocation = m.GetDeclarationLocation(startPos);
			
			Expect(20);
			Expect(21);

#line  1041 "cs.ATG" 
			d.EndLocation = t.EndLocation; 
			if (la.kind == 16) {
				Block(
#line  1041 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(160);

#line  1042 "cs.ATG" 
			d.Body = (BlockStatement)stmt;
			compilationUnit.AddChild(d);
			
		} else SynErr(161);
	}

	void StructMemberDecl(
#line  727 "cs.ATG" 
ModifierList m, List<AttributeSection> attributes) {

#line  729 "cs.ATG" 
		string qualident = null;
		TypeReference type;
		Expression expr;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		Statement stmt = null;
		List<VariableDeclaration> variableDeclarators = new List<VariableDeclaration>();
		List<TemplateDefinition> templates = new List<TemplateDefinition>();
		TypeReference explicitInterface = null;
		bool isExtensionMethod = false;
		
		if (la.kind == 60) {

#line  740 "cs.ATG" 
			m.Check(Modifiers.Constants); 
			lexer.NextToken();

#line  741 "cs.ATG" 
			Location startPos = t.Location; 
			Type(
#line  742 "cs.ATG" 
out type);
			Identifier();

#line  742 "cs.ATG" 
			FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier | Modifiers.Const);
			fd.StartLocation = m.GetDeclarationLocation(startPos);
			VariableDeclaration f = new VariableDeclaration(t.val);
			SafeAdd(fd, fd.Fields, f);
			
			Expect(3);
			Expr(
#line  747 "cs.ATG" 
out expr);

#line  747 "cs.ATG" 
			f.Initializer = expr; 
			while (la.kind == 14) {
				lexer.NextToken();
				Identifier();

#line  748 "cs.ATG" 
				f = new VariableDeclaration(t.val);
				SafeAdd(fd, fd.Fields, f);
				
				Expect(3);
				Expr(
#line  751 "cs.ATG" 
out expr);

#line  751 "cs.ATG" 
				f.Initializer = expr; 
			}
			Expect(11);

#line  752 "cs.ATG" 
			fd.EndLocation = t.EndLocation; compilationUnit.AddChild(fd); 
		} else if (
#line  756 "cs.ATG" 
NotVoidPointer()) {

#line  756 "cs.ATG" 
			m.Check(Modifiers.PropertysEventsMethods); 
			Expect(123);

#line  757 "cs.ATG" 
			Location startPos = t.Location; 
			if (
#line  758 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
				TypeName(
#line  759 "cs.ATG" 
out explicitInterface, false);

#line  760 "cs.ATG" 
				if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
				 } 
			} else if (StartOf(19)) {
				Identifier();

#line  763 "cs.ATG" 
				qualident = t.val; 
			} else SynErr(162);
			if (la.kind == 23) {
				TypeParameterList(
#line  766 "cs.ATG" 
templates);
			}
			Expect(20);
			if (la.kind == 111) {
				lexer.NextToken();

#line  769 "cs.ATG" 
				isExtensionMethod = true; /* C# 3.0 */ 
			}
			if (StartOf(11)) {
				FormalParameterList(
#line  770 "cs.ATG" 
p);
			}
			Expect(21);

#line  771 "cs.ATG" 
			MethodDeclaration methodDeclaration = new MethodDeclaration {
			Name = qualident,
			Modifier = m.Modifier,
			TypeReference = new TypeReference("void"),
			Parameters = p,
			Attributes = attributes,
			StartLocation = m.GetDeclarationLocation(startPos),
			EndLocation = t.EndLocation,
			Templates = templates,
			IsExtensionMethod = isExtensionMethod
			};
			if (explicitInterface != null)
				methodDeclaration.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
			compilationUnit.AddChild(methodDeclaration);
			compilationUnit.BlockStart(methodDeclaration);
			
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  789 "cs.ATG" 
templates);
			}
			if (la.kind == 16) {
				Block(
#line  791 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(163);

#line  791 "cs.ATG" 
			compilationUnit.BlockEnd();
			methodDeclaration.Body  = (BlockStatement)stmt;
			
		} else if (la.kind == 69) {

#line  795 "cs.ATG" 
			m.Check(Modifiers.PropertysEventsMethods); 
			lexer.NextToken();

#line  797 "cs.ATG" 
			EventDeclaration eventDecl = new EventDeclaration {
			Modifier = m.Modifier, 
			Attributes = attributes,
			StartLocation = t.Location
			};
			compilationUnit.AddChild(eventDecl);
			compilationUnit.BlockStart(eventDecl);
			EventAddRegion addBlock = null;
			EventRemoveRegion removeBlock = null;
			
			Type(
#line  807 "cs.ATG" 
out type);

#line  807 "cs.ATG" 
			eventDecl.TypeReference = type; 
			if (
#line  808 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
				TypeName(
#line  809 "cs.ATG" 
out explicitInterface, false);

#line  810 "cs.ATG" 
				qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface); 

#line  811 "cs.ATG" 
				eventDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident)); 
			} else if (StartOf(19)) {
				Identifier();

#line  813 "cs.ATG" 
				qualident = t.val; 
			} else SynErr(164);

#line  815 "cs.ATG" 
			eventDecl.Name = qualident; eventDecl.EndLocation = t.EndLocation; 
			if (la.kind == 3) {
				lexer.NextToken();
				Expr(
#line  816 "cs.ATG" 
out expr);

#line  816 "cs.ATG" 
				eventDecl.Initializer = expr; 
			}
			if (la.kind == 16) {
				lexer.NextToken();

#line  817 "cs.ATG" 
				eventDecl.BodyStart = t.Location; 
				EventAccessorDecls(
#line  818 "cs.ATG" 
out addBlock, out removeBlock);
				Expect(17);

#line  819 "cs.ATG" 
				eventDecl.BodyEnd   = t.EndLocation; 
			}
			if (la.kind == 11) {
				lexer.NextToken();
			}

#line  822 "cs.ATG" 
			compilationUnit.BlockEnd();
			eventDecl.AddRegion = addBlock;
			eventDecl.RemoveRegion = removeBlock;
			
		} else if (
#line  828 "cs.ATG" 
IdentAndLPar()) {

#line  828 "cs.ATG" 
			m.Check(Modifiers.Constructors | Modifiers.StaticConstructors); 
			Identifier();

#line  829 "cs.ATG" 
			string name = t.val; Location startPos = t.Location; 
			Expect(20);
			if (StartOf(11)) {

#line  829 "cs.ATG" 
				m.Check(Modifiers.Constructors); 
				FormalParameterList(
#line  830 "cs.ATG" 
p);
			}
			Expect(21);

#line  832 "cs.ATG" 
			ConstructorInitializer init = null;  
			if (la.kind == 9) {

#line  833 "cs.ATG" 
				m.Check(Modifiers.Constructors); 
				ConstructorInitializer(
#line  834 "cs.ATG" 
out init);
			}

#line  836 "cs.ATG" 
			ConstructorDeclaration cd = new ConstructorDeclaration(name, m.Modifier, p, init, attributes); 
			cd.StartLocation = startPos;
			cd.EndLocation   = t.EndLocation;
			
			if (la.kind == 16) {
				Block(
#line  841 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();
			} else SynErr(165);

#line  841 "cs.ATG" 
			cd.Body = (BlockStatement)stmt; compilationUnit.AddChild(cd); 
		} else if (la.kind == 70 || la.kind == 80) {

#line  844 "cs.ATG" 
			m.Check(Modifiers.Operators);
			if (m.isNone) Error("at least one modifier must be set"); 
			bool isImplicit = true;
			Location startPos = Location.Empty;
			
			if (la.kind == 80) {
				lexer.NextToken();

#line  849 "cs.ATG" 
				startPos = t.Location; 
			} else {
				lexer.NextToken();

#line  849 "cs.ATG" 
				isImplicit = false; startPos = t.Location; 
			}
			Expect(92);
			Type(
#line  850 "cs.ATG" 
out type);

#line  850 "cs.ATG" 
			TypeReference operatorType = type; 
			Expect(20);
			Type(
#line  851 "cs.ATG" 
out type);
			Identifier();

#line  851 "cs.ATG" 
			string varName = t.val; 
			Expect(21);

#line  852 "cs.ATG" 
			Location endPos = t.Location; 
			if (la.kind == 16) {
				Block(
#line  853 "cs.ATG" 
out stmt);
			} else if (la.kind == 11) {
				lexer.NextToken();

#line  853 "cs.ATG" 
				stmt = null; 
			} else SynErr(166);

#line  856 "cs.ATG" 
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
			parameters.Add(new ParameterDeclarationExpression(type, varName));
			OperatorDeclaration operatorDeclaration = new OperatorDeclaration {
				Modifier = m.Modifier,
				Attributes = attributes, 
				Parameters = parameters, 
				TypeReference = operatorType,
				ConversionType = isImplicit ? ConversionType.Implicit : ConversionType.Explicit,
				Body = (BlockStatement)stmt,
				StartLocation = m.GetDeclarationLocation(startPos),
				EndLocation = endPos
			};
			compilationUnit.AddChild(operatorDeclaration);
			
		} else if (StartOf(22)) {
			TypeDecl(
#line  873 "cs.ATG" 
m, attributes);
		} else if (StartOf(10)) {
			Type(
#line  875 "cs.ATG" 
out type);

#line  875 "cs.ATG" 
			Location startPos = t.Location;  
			if (la.kind == 92) {

#line  877 "cs.ATG" 
				OverloadableOperatorType op;
				m.Check(Modifiers.Operators);
				if (m.isNone) Error("at least one modifier must be set");
				
				lexer.NextToken();
				OverloadableOperator(
#line  881 "cs.ATG" 
out op);

#line  881 "cs.ATG" 
				TypeReference firstType, secondType = null; string secondName = null; 
				Expect(20);
				Type(
#line  882 "cs.ATG" 
out firstType);
				Identifier();

#line  882 "cs.ATG" 
				string firstName = t.val; 
				if (la.kind == 14) {
					lexer.NextToken();
					Type(
#line  883 "cs.ATG" 
out secondType);
					Identifier();

#line  883 "cs.ATG" 
					secondName = t.val; 
				} else if (la.kind == 21) {
				} else SynErr(167);

#line  891 "cs.ATG" 
				Location endPos = t.Location; 
				Expect(21);
				if (la.kind == 16) {
					Block(
#line  892 "cs.ATG" 
out stmt);
				} else if (la.kind == 11) {
					lexer.NextToken();
				} else SynErr(168);

#line  894 "cs.ATG" 
				List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>();
				parameters.Add(new ParameterDeclarationExpression(firstType, firstName));
				if (secondType != null) {
					parameters.Add(new ParameterDeclarationExpression(secondType, secondName));
				}
				OperatorDeclaration operatorDeclaration = new OperatorDeclaration {
					Modifier = m.Modifier,
					Attributes = attributes,
					Parameters = parameters,
					TypeReference = type,
					OverloadableOperator = op,
					Body = (BlockStatement)stmt,
					StartLocation = m.GetDeclarationLocation(startPos),
					EndLocation = endPos
				};
				compilationUnit.AddChild(operatorDeclaration);
				
			} else if (
#line  913 "cs.ATG" 
IsVarDecl()) {

#line  914 "cs.ATG" 
				m.Check(Modifiers.Fields);
				FieldDeclaration fd = new FieldDeclaration(attributes, type, m.Modifier);
				fd.StartLocation = m.GetDeclarationLocation(startPos); 
				
				if (
#line  918 "cs.ATG" 
m.Contains(Modifiers.Fixed)) {
					VariableDeclarator(
#line  919 "cs.ATG" 
variableDeclarators);
					Expect(18);
					Expr(
#line  921 "cs.ATG" 
out expr);

#line  921 "cs.ATG" 
					if (variableDeclarators.Count > 0)
					variableDeclarators[variableDeclarators.Count-1].FixedArrayInitialization = expr; 
					Expect(19);
					while (la.kind == 14) {
						lexer.NextToken();
						VariableDeclarator(
#line  925 "cs.ATG" 
variableDeclarators);
						Expect(18);
						Expr(
#line  927 "cs.ATG" 
out expr);

#line  927 "cs.ATG" 
						if (variableDeclarators.Count > 0)
						variableDeclarators[variableDeclarators.Count-1].FixedArrayInitialization = expr; 
						Expect(19);
					}
				} else if (StartOf(19)) {
					VariableDeclarator(
#line  932 "cs.ATG" 
variableDeclarators);
					while (la.kind == 14) {
						lexer.NextToken();
						VariableDeclarator(
#line  933 "cs.ATG" 
variableDeclarators);
					}
				} else SynErr(169);
				Expect(11);

#line  935 "cs.ATG" 
				fd.EndLocation = t.EndLocation; fd.Fields = variableDeclarators; compilationUnit.AddChild(fd); 
			} else if (la.kind == 111) {

#line  938 "cs.ATG" 
				m.Check(Modifiers.Indexers); 
				lexer.NextToken();
				Expect(18);
				FormalParameterList(
#line  939 "cs.ATG" 
p);
				Expect(19);

#line  939 "cs.ATG" 
				Location endLocation = t.EndLocation; 
				Expect(16);

#line  940 "cs.ATG" 
				IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
				indexer.StartLocation = startPos;
				indexer.EndLocation   = endLocation;
				indexer.BodyStart     = t.Location;
				PropertyGetRegion getRegion;
				PropertySetRegion setRegion;
				
				AccessorDecls(
#line  947 "cs.ATG" 
out getRegion, out setRegion);
				Expect(17);

#line  948 "cs.ATG" 
				indexer.BodyEnd    = t.EndLocation;
				indexer.GetRegion = getRegion;
				indexer.SetRegion = setRegion;
				compilationUnit.AddChild(indexer);
				
			} else if (
#line  953 "cs.ATG" 
IsIdentifierToken(la)) {
				if (
#line  954 "cs.ATG" 
IsExplicitInterfaceImplementation()) {
					TypeName(
#line  955 "cs.ATG" 
out explicitInterface, false);

#line  956 "cs.ATG" 
					if (la.kind != Tokens.Dot || Peek(1).kind != Tokens.This) {
					qualident = TypeReference.StripLastIdentifierFromType(ref explicitInterface);
					 } 
				} else if (StartOf(19)) {
					Identifier();

#line  959 "cs.ATG" 
					qualident = t.val; 
				} else SynErr(170);

#line  961 "cs.ATG" 
				Location qualIdentEndLocation = t.EndLocation; 
				if (la.kind == 16 || la.kind == 20 || la.kind == 23) {
					if (la.kind == 20 || la.kind == 23) {

#line  965 "cs.ATG" 
						m.Check(Modifiers.PropertysEventsMethods); 
						if (la.kind == 23) {
							TypeParameterList(
#line  967 "cs.ATG" 
templates);
						}
						Expect(20);
						if (la.kind == 111) {
							lexer.NextToken();

#line  969 "cs.ATG" 
							isExtensionMethod = true; 
						}
						if (StartOf(11)) {
							FormalParameterList(
#line  970 "cs.ATG" 
p);
						}
						Expect(21);

#line  972 "cs.ATG" 
						MethodDeclaration methodDeclaration = new MethodDeclaration {
						Name = qualident,
						Modifier = m.Modifier,
						TypeReference = type,
						Parameters = p, 
						Attributes = attributes
						};
						if (explicitInterface != null)
							methodDeclaration.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
						methodDeclaration.StartLocation = m.GetDeclarationLocation(startPos);
						methodDeclaration.EndLocation   = t.EndLocation;
						methodDeclaration.IsExtensionMethod = isExtensionMethod;
						methodDeclaration.Templates = templates;
						compilationUnit.AddChild(methodDeclaration);
						                                      
						while (la.kind == 127) {
							TypeParameterConstraintsClause(
#line  987 "cs.ATG" 
templates);
						}
						if (la.kind == 16) {
							Block(
#line  988 "cs.ATG" 
out stmt);
						} else if (la.kind == 11) {
							lexer.NextToken();
						} else SynErr(171);

#line  988 "cs.ATG" 
						methodDeclaration.Body  = (BlockStatement)stmt; 
					} else {
						lexer.NextToken();

#line  991 "cs.ATG" 
						PropertyDeclaration pDecl = new PropertyDeclaration(qualident, type, m.Modifier, attributes); 
						if (explicitInterface != null)
						pDecl.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, qualident));
						      pDecl.StartLocation = m.GetDeclarationLocation(startPos);
						      pDecl.EndLocation   = qualIdentEndLocation;
						      pDecl.BodyStart   = t.Location;
						      PropertyGetRegion getRegion;
						      PropertySetRegion setRegion;
						   
						AccessorDecls(
#line  1000 "cs.ATG" 
out getRegion, out setRegion);
						Expect(17);

#line  1002 "cs.ATG" 
						pDecl.GetRegion = getRegion;
						pDecl.SetRegion = setRegion;
						pDecl.BodyEnd = t.EndLocation;
						compilationUnit.AddChild(pDecl);
						
					}
				} else if (la.kind == 15) {

#line  1010 "cs.ATG" 
					m.Check(Modifiers.Indexers); 
					lexer.NextToken();
					Expect(111);
					Expect(18);
					FormalParameterList(
#line  1011 "cs.ATG" 
p);
					Expect(19);

#line  1012 "cs.ATG" 
					IndexerDeclaration indexer = new IndexerDeclaration(type, p, m.Modifier, attributes);
					indexer.StartLocation = m.GetDeclarationLocation(startPos);
					indexer.EndLocation   = t.EndLocation;
					if (explicitInterface != null)
					indexer.InterfaceImplementations.Add(new InterfaceImplementation(explicitInterface, "this"));
					      PropertyGetRegion getRegion;
					      PropertySetRegion setRegion;
					    
					Expect(16);

#line  1020 "cs.ATG" 
					Location bodyStart = t.Location; 
					AccessorDecls(
#line  1021 "cs.ATG" 
out getRegion, out setRegion);
					Expect(17);

#line  1022 "cs.ATG" 
					indexer.BodyStart = bodyStart;
					indexer.BodyEnd   = t.EndLocation;
					indexer.GetRegion = getRegion;
					indexer.SetRegion = setRegion;
					compilationUnit.AddChild(indexer);
					
				} else SynErr(172);
			} else SynErr(173);
		} else SynErr(174);
	}

	void InterfaceMemberDecl() {

#line  1049 "cs.ATG" 
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
#line  1062 "cs.ATG" 
out section);

#line  1062 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 89) {
			lexer.NextToken();

#line  1063 "cs.ATG" 
			mod = Modifiers.New; startLocation = t.Location; 
		}
		if (
#line  1066 "cs.ATG" 
NotVoidPointer()) {
			Expect(123);

#line  1066 "cs.ATG" 
			if (startLocation.X == -1) startLocation = t.Location; 
			Identifier();

#line  1067 "cs.ATG" 
			name = t.val; 
			if (la.kind == 23) {
				TypeParameterList(
#line  1068 "cs.ATG" 
templates);
			}
			Expect(20);
			if (StartOf(11)) {
				FormalParameterList(
#line  1069 "cs.ATG" 
parameters);
			}
			Expect(21);
			while (la.kind == 127) {
				TypeParameterConstraintsClause(
#line  1070 "cs.ATG" 
templates);
			}
			Expect(11);

#line  1072 "cs.ATG" 
			MethodDeclaration md = new MethodDeclaration {
			Name = name, Modifier = mod, TypeReference = new TypeReference("void"), 
			Parameters = parameters, Attributes = attributes, Templates = templates,
			StartLocation = startLocation, EndLocation = t.EndLocation
			};
			compilationUnit.AddChild(md);
			
		} else if (StartOf(23)) {
			if (StartOf(10)) {
				Type(
#line  1080 "cs.ATG" 
out type);

#line  1080 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				if (StartOf(19)) {
					Identifier();

#line  1082 "cs.ATG" 
					name = t.val; Location qualIdentEndLocation = t.EndLocation; 
					if (la.kind == 20 || la.kind == 23) {
						if (la.kind == 23) {
							TypeParameterList(
#line  1086 "cs.ATG" 
templates);
						}
						Expect(20);
						if (StartOf(11)) {
							FormalParameterList(
#line  1087 "cs.ATG" 
parameters);
						}
						Expect(21);
						while (la.kind == 127) {
							TypeParameterConstraintsClause(
#line  1089 "cs.ATG" 
templates);
						}
						Expect(11);

#line  1090 "cs.ATG" 
						MethodDeclaration md = new MethodDeclaration {
						Name = name, Modifier = mod, TypeReference = type,
						Parameters = parameters, Attributes = attributes, Templates = templates,
						StartLocation = startLocation, EndLocation = t.EndLocation
						};
						compilationUnit.AddChild(md);
						
					} else if (la.kind == 16) {

#line  1098 "cs.ATG" 
						PropertyDeclaration pd = new PropertyDeclaration(name, type, mod, attributes); compilationUnit.AddChild(pd); 
						lexer.NextToken();

#line  1099 "cs.ATG" 
						Location bodyStart = t.Location;
						InterfaceAccessors(
#line  1099 "cs.ATG" 
out getBlock, out setBlock);
						Expect(17);

#line  1099 "cs.ATG" 
						pd.GetRegion = getBlock; pd.SetRegion = setBlock; pd.StartLocation = startLocation; pd.EndLocation = qualIdentEndLocation; pd.BodyStart = bodyStart; pd.BodyEnd = t.EndLocation; 
					} else SynErr(175);
				} else if (la.kind == 111) {
					lexer.NextToken();
					Expect(18);
					FormalParameterList(
#line  1102 "cs.ATG" 
parameters);
					Expect(19);

#line  1102 "cs.ATG" 
					Location bracketEndLocation = t.EndLocation; 

#line  1102 "cs.ATG" 
					IndexerDeclaration id = new IndexerDeclaration(type, parameters, mod, attributes); compilationUnit.AddChild(id); 
					Expect(16);

#line  1103 "cs.ATG" 
					Location bodyStart = t.Location;
					InterfaceAccessors(
#line  1103 "cs.ATG" 
out getBlock, out setBlock);
					Expect(17);

#line  1103 "cs.ATG" 
					id.GetRegion = getBlock; id.SetRegion = setBlock; id.StartLocation = startLocation;  id.EndLocation = bracketEndLocation; id.BodyStart = bodyStart; id.BodyEnd = t.EndLocation;
				} else SynErr(176);
			} else {
				lexer.NextToken();

#line  1106 "cs.ATG" 
				if (startLocation.X == -1) startLocation = t.Location; 
				Type(
#line  1107 "cs.ATG" 
out type);
				Identifier();

#line  1108 "cs.ATG" 
				EventDeclaration ed = new EventDeclaration {
				TypeReference = type, Name = t.val, Modifier = mod, Attributes = attributes
				};
				compilationUnit.AddChild(ed);
				
				Expect(11);

#line  1113 "cs.ATG" 
				ed.StartLocation = startLocation; ed.EndLocation = t.EndLocation; 
			}
		} else SynErr(177);
	}

	void EnumMemberDecl(
#line  1118 "cs.ATG" 
out FieldDeclaration f) {

#line  1120 "cs.ATG" 
		Expression expr = null;
		List<AttributeSection> attributes = new List<AttributeSection>();
		AttributeSection section = null;
		VariableDeclaration varDecl = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1126 "cs.ATG" 
out section);

#line  1126 "cs.ATG" 
			attributes.Add(section); 
		}
		Identifier();

#line  1127 "cs.ATG" 
		f = new FieldDeclaration(attributes);
		varDecl         = new VariableDeclaration(t.val);
		f.Fields.Add(varDecl);
		f.StartLocation = t.Location;
		
		if (la.kind == 3) {
			lexer.NextToken();
			Expr(
#line  1132 "cs.ATG" 
out expr);

#line  1132 "cs.ATG" 
			varDecl.Initializer = expr; 
		}
	}

	void TypeWithRestriction(
#line  556 "cs.ATG" 
out TypeReference type, bool allowNullable, bool canBeUnbound) {

#line  558 "cs.ATG" 
		string name;
		int pointer = 0;
		type = null;
		
		if (StartOf(4)) {
			ClassType(
#line  563 "cs.ATG" 
out type, canBeUnbound);
		} else if (StartOf(5)) {
			SimpleType(
#line  564 "cs.ATG" 
out name);

#line  564 "cs.ATG" 
			type = new TypeReference(name); 
		} else if (la.kind == 123) {
			lexer.NextToken();
			Expect(6);

#line  565 "cs.ATG" 
			pointer = 1; type = new TypeReference("void"); 
		} else SynErr(178);

#line  566 "cs.ATG" 
		List<int> r = new List<int>(); 
		if (
#line  568 "cs.ATG" 
allowNullable && la.kind == Tokens.Question) {
			NullableQuestionMark(
#line  568 "cs.ATG" 
ref type);
		}
		while (
#line  570 "cs.ATG" 
IsPointerOrDims()) {

#line  570 "cs.ATG" 
			int i = 0; 
			if (la.kind == 6) {
				lexer.NextToken();

#line  571 "cs.ATG" 
				++pointer; 
			} else if (la.kind == 18) {
				lexer.NextToken();
				while (la.kind == 14) {
					lexer.NextToken();

#line  572 "cs.ATG" 
					++i; 
				}
				Expect(19);

#line  572 "cs.ATG" 
				r.Add(i); 
			} else SynErr(179);
		}

#line  575 "cs.ATG" 
		if (type != null) {
		type.RankSpecifier = r.ToArray();
		type.PointerNestingLevel = pointer;
		  }
		
	}

	void SimpleType(
#line  603 "cs.ATG" 
out string name) {

#line  604 "cs.ATG" 
		name = String.Empty; 
		if (StartOf(24)) {
			IntegralType(
#line  606 "cs.ATG" 
out name);
		} else if (la.kind == 75) {
			lexer.NextToken();

#line  607 "cs.ATG" 
			name = "float"; 
		} else if (la.kind == 66) {
			lexer.NextToken();

#line  608 "cs.ATG" 
			name = "double"; 
		} else if (la.kind == 62) {
			lexer.NextToken();

#line  609 "cs.ATG" 
			name = "decimal"; 
		} else if (la.kind == 52) {
			lexer.NextToken();

#line  610 "cs.ATG" 
			name = "bool"; 
		} else SynErr(180);
	}

	void NullableQuestionMark(
#line  2263 "cs.ATG" 
ref TypeReference typeRef) {

#line  2264 "cs.ATG" 
		List<TypeReference> typeArguments = new List<TypeReference>(1); 
		Expect(12);

#line  2268 "cs.ATG" 
		if (typeRef != null) typeArguments.Add(typeRef);
		typeRef = new TypeReference("System.Nullable", typeArguments);
		
	}

	void FixedParameter(
#line  640 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  642 "cs.ATG" 
		TypeReference type;
		ParameterModifiers mod = ParameterModifiers.In;
		Location start = t.Location;
		
		if (la.kind == 93 || la.kind == 100) {
			if (la.kind == 100) {
				lexer.NextToken();

#line  648 "cs.ATG" 
				mod = ParameterModifiers.Ref; 
			} else {
				lexer.NextToken();

#line  649 "cs.ATG" 
				mod = ParameterModifiers.Out; 
			}
		}
		Type(
#line  651 "cs.ATG" 
out type);
		Identifier();

#line  651 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, mod); p.StartLocation = start; p.EndLocation = t.Location; 
	}

	void ParameterArray(
#line  654 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  655 "cs.ATG" 
		TypeReference type; 
		Expect(95);
		Type(
#line  657 "cs.ATG" 
out type);
		Identifier();

#line  657 "cs.ATG" 
		p = new ParameterDeclarationExpression(type, t.val, ParameterModifiers.Params); 
	}

	void AccessorModifiers(
#line  660 "cs.ATG" 
out ModifierList m) {

#line  661 "cs.ATG" 
		m = new ModifierList(); 
		if (la.kind == 96) {
			lexer.NextToken();

#line  663 "cs.ATG" 
			m.Add(Modifiers.Private, t.Location); 
		} else if (la.kind == 97) {
			lexer.NextToken();

#line  664 "cs.ATG" 
			m.Add(Modifiers.Protected, t.Location); 
			if (la.kind == 84) {
				lexer.NextToken();

#line  665 "cs.ATG" 
				m.Add(Modifiers.Internal, t.Location); 
			}
		} else if (la.kind == 84) {
			lexer.NextToken();

#line  666 "cs.ATG" 
			m.Add(Modifiers.Internal, t.Location); 
			if (la.kind == 97) {
				lexer.NextToken();

#line  667 "cs.ATG" 
				m.Add(Modifiers.Protected, t.Location); 
			}
		} else SynErr(181);
	}

	void Block(
#line  1251 "cs.ATG" 
out Statement stmt) {
		Expect(16);

#line  1253 "cs.ATG" 
		BlockStatement blockStmt = new BlockStatement();
		blockStmt.StartLocation = t.Location;
		compilationUnit.BlockStart(blockStmt);
		if (!ParseMethodBodies) lexer.SkipCurrentBlock(0);
		
		while (StartOf(25)) {
			Statement();
		}
		while (!(la.kind == 0 || la.kind == 17)) {SynErr(182); lexer.NextToken(); }
		Expect(17);

#line  1261 "cs.ATG" 
		stmt = blockStmt;
		blockStmt.EndLocation = t.EndLocation;
		compilationUnit.BlockEnd();
		
	}

	void EventAccessorDecls(
#line  1189 "cs.ATG" 
out EventAddRegion addBlock, out EventRemoveRegion removeBlock) {

#line  1190 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		Statement stmt;
		addBlock = null;
		removeBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1197 "cs.ATG" 
out section);

#line  1197 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 130) {

#line  1199 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); 
			AddAccessorDecl(
#line  1200 "cs.ATG" 
out stmt);

#line  1200 "cs.ATG" 
			attributes = new List<AttributeSection>(); addBlock.Block = (BlockStatement)stmt; 
			while (la.kind == 18) {
				AttributeSection(
#line  1201 "cs.ATG" 
out section);

#line  1201 "cs.ATG" 
				attributes.Add(section); 
			}
			RemoveAccessorDecl(
#line  1202 "cs.ATG" 
out stmt);

#line  1202 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; 
		} else if (la.kind == 131) {
			RemoveAccessorDecl(
#line  1204 "cs.ATG" 
out stmt);

#line  1204 "cs.ATG" 
			removeBlock = new EventRemoveRegion(attributes); removeBlock.Block = (BlockStatement)stmt; attributes = new List<AttributeSection>(); 
			while (la.kind == 18) {
				AttributeSection(
#line  1205 "cs.ATG" 
out section);

#line  1205 "cs.ATG" 
				attributes.Add(section); 
			}
			AddAccessorDecl(
#line  1206 "cs.ATG" 
out stmt);

#line  1206 "cs.ATG" 
			addBlock = new EventAddRegion(attributes); addBlock.Block = (BlockStatement)stmt; 
		} else SynErr(183);
	}

	void ConstructorInitializer(
#line  1281 "cs.ATG" 
out ConstructorInitializer ci) {

#line  1282 "cs.ATG" 
		Expression expr; ci = new ConstructorInitializer(); 
		Expect(9);
		if (la.kind == 51) {
			lexer.NextToken();

#line  1286 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.Base; 
		} else if (la.kind == 111) {
			lexer.NextToken();

#line  1287 "cs.ATG" 
			ci.ConstructorInitializerType = ConstructorInitializerType.This; 
		} else SynErr(184);
		Expect(20);
		if (StartOf(26)) {
			Argument(
#line  1290 "cs.ATG" 
out expr);

#line  1290 "cs.ATG" 
			if (expr != null) { ci.Arguments.Add(expr); } 
			while (la.kind == 14) {
				lexer.NextToken();
				Argument(
#line  1290 "cs.ATG" 
out expr);

#line  1290 "cs.ATG" 
				if (expr != null) { ci.Arguments.Add(expr); } 
			}
		}
		Expect(21);
	}

	void OverloadableOperator(
#line  1302 "cs.ATG" 
out OverloadableOperatorType op) {

#line  1303 "cs.ATG" 
		op = OverloadableOperatorType.None; 
		switch (la.kind) {
		case 4: {
			lexer.NextToken();

#line  1305 "cs.ATG" 
			op = OverloadableOperatorType.Add; 
			break;
		}
		case 5: {
			lexer.NextToken();

#line  1306 "cs.ATG" 
			op = OverloadableOperatorType.Subtract; 
			break;
		}
		case 24: {
			lexer.NextToken();

#line  1308 "cs.ATG" 
			op = OverloadableOperatorType.Not; 
			break;
		}
		case 27: {
			lexer.NextToken();

#line  1309 "cs.ATG" 
			op = OverloadableOperatorType.BitNot; 
			break;
		}
		case 31: {
			lexer.NextToken();

#line  1311 "cs.ATG" 
			op = OverloadableOperatorType.Increment; 
			break;
		}
		case 32: {
			lexer.NextToken();

#line  1312 "cs.ATG" 
			op = OverloadableOperatorType.Decrement; 
			break;
		}
		case 113: {
			lexer.NextToken();

#line  1314 "cs.ATG" 
			op = OverloadableOperatorType.IsTrue; 
			break;
		}
		case 72: {
			lexer.NextToken();

#line  1315 "cs.ATG" 
			op = OverloadableOperatorType.IsFalse; 
			break;
		}
		case 6: {
			lexer.NextToken();

#line  1317 "cs.ATG" 
			op = OverloadableOperatorType.Multiply; 
			break;
		}
		case 7: {
			lexer.NextToken();

#line  1318 "cs.ATG" 
			op = OverloadableOperatorType.Divide; 
			break;
		}
		case 8: {
			lexer.NextToken();

#line  1319 "cs.ATG" 
			op = OverloadableOperatorType.Modulus; 
			break;
		}
		case 28: {
			lexer.NextToken();

#line  1321 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseAnd; 
			break;
		}
		case 29: {
			lexer.NextToken();

#line  1322 "cs.ATG" 
			op = OverloadableOperatorType.BitwiseOr; 
			break;
		}
		case 30: {
			lexer.NextToken();

#line  1323 "cs.ATG" 
			op = OverloadableOperatorType.ExclusiveOr; 
			break;
		}
		case 37: {
			lexer.NextToken();

#line  1325 "cs.ATG" 
			op = OverloadableOperatorType.ShiftLeft; 
			break;
		}
		case 33: {
			lexer.NextToken();

#line  1326 "cs.ATG" 
			op = OverloadableOperatorType.Equality; 
			break;
		}
		case 34: {
			lexer.NextToken();

#line  1327 "cs.ATG" 
			op = OverloadableOperatorType.InEquality; 
			break;
		}
		case 23: {
			lexer.NextToken();

#line  1328 "cs.ATG" 
			op = OverloadableOperatorType.LessThan; 
			break;
		}
		case 35: {
			lexer.NextToken();

#line  1329 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThanOrEqual; 
			break;
		}
		case 36: {
			lexer.NextToken();

#line  1330 "cs.ATG" 
			op = OverloadableOperatorType.LessThanOrEqual; 
			break;
		}
		case 22: {
			lexer.NextToken();

#line  1331 "cs.ATG" 
			op = OverloadableOperatorType.GreaterThan; 
			if (la.kind == 22) {
				lexer.NextToken();

#line  1331 "cs.ATG" 
				op = OverloadableOperatorType.ShiftRight; 
			}
			break;
		}
		default: SynErr(185); break;
		}
	}

	void VariableDeclarator(
#line  1244 "cs.ATG" 
List<VariableDeclaration> fieldDeclaration) {

#line  1245 "cs.ATG" 
		Expression expr = null; 
		Identifier();

#line  1247 "cs.ATG" 
		VariableDeclaration f = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1248 "cs.ATG" 
out expr);

#line  1248 "cs.ATG" 
			f.Initializer = expr; 
		}

#line  1248 "cs.ATG" 
		fieldDeclaration.Add(f); 
	}

	void AccessorDecls(
#line  1136 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1138 "cs.ATG" 
		List<AttributeSection> attributes = new List<AttributeSection>(); 
		AttributeSection section;
		getBlock = null;
		setBlock = null; 
		ModifierList modifiers = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1145 "cs.ATG" 
out section);

#line  1145 "cs.ATG" 
			attributes.Add(section); 
		}
		if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
			AccessorModifiers(
#line  1146 "cs.ATG" 
out modifiers);
		}
		if (la.kind == 128) {
			GetAccessorDecl(
#line  1148 "cs.ATG" 
out getBlock, attributes);

#line  1149 "cs.ATG" 
			if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(27)) {

#line  1150 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1151 "cs.ATG" 
out section);

#line  1151 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
					AccessorModifiers(
#line  1152 "cs.ATG" 
out modifiers);
				}
				SetAccessorDecl(
#line  1153 "cs.ATG" 
out setBlock, attributes);

#line  1154 "cs.ATG" 
				if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (la.kind == 129) {
			SetAccessorDecl(
#line  1157 "cs.ATG" 
out setBlock, attributes);

#line  1158 "cs.ATG" 
			if (modifiers != null) {setBlock.Modifier = modifiers.Modifier; } 
			if (StartOf(28)) {

#line  1159 "cs.ATG" 
				attributes = new List<AttributeSection>(); modifiers = null; 
				while (la.kind == 18) {
					AttributeSection(
#line  1160 "cs.ATG" 
out section);

#line  1160 "cs.ATG" 
					attributes.Add(section); 
				}
				if (la.kind == 84 || la.kind == 96 || la.kind == 97) {
					AccessorModifiers(
#line  1161 "cs.ATG" 
out modifiers);
				}
				GetAccessorDecl(
#line  1162 "cs.ATG" 
out getBlock, attributes);

#line  1163 "cs.ATG" 
				if (modifiers != null) {getBlock.Modifier = modifiers.Modifier; } 
			}
		} else if (StartOf(19)) {
			Identifier();

#line  1165 "cs.ATG" 
			Error("get or set accessor declaration expected"); 
		} else SynErr(186);
	}

	void InterfaceAccessors(
#line  1210 "cs.ATG" 
out PropertyGetRegion getBlock, out PropertySetRegion setBlock) {

#line  1212 "cs.ATG" 
		AttributeSection section;
		List<AttributeSection> attributes = new List<AttributeSection>();
		getBlock = null; setBlock = null;
		PropertyGetSetRegion lastBlock = null;
		
		while (la.kind == 18) {
			AttributeSection(
#line  1218 "cs.ATG" 
out section);

#line  1218 "cs.ATG" 
			attributes.Add(section); 
		}

#line  1219 "cs.ATG" 
		Location startLocation = la.Location; 
		if (la.kind == 128) {
			lexer.NextToken();

#line  1221 "cs.ATG" 
			getBlock = new PropertyGetRegion(null, attributes); 
		} else if (la.kind == 129) {
			lexer.NextToken();

#line  1222 "cs.ATG" 
			setBlock = new PropertySetRegion(null, attributes); 
		} else SynErr(187);
		Expect(11);

#line  1225 "cs.ATG" 
		if (getBlock != null) { getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; }
		if (setBlock != null) { setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; }
		attributes = new List<AttributeSection>(); 
		if (la.kind == 18 || la.kind == 128 || la.kind == 129) {
			while (la.kind == 18) {
				AttributeSection(
#line  1229 "cs.ATG" 
out section);

#line  1229 "cs.ATG" 
				attributes.Add(section); 
			}

#line  1230 "cs.ATG" 
			startLocation = la.Location; 
			if (la.kind == 128) {
				lexer.NextToken();

#line  1232 "cs.ATG" 
				if (getBlock != null) Error("get already declared");
				                 else { getBlock = new PropertyGetRegion(null, attributes); lastBlock = getBlock; }
				              
			} else if (la.kind == 129) {
				lexer.NextToken();

#line  1235 "cs.ATG" 
				if (setBlock != null) Error("set already declared");
				                 else { setBlock = new PropertySetRegion(null, attributes); lastBlock = setBlock; }
				              
			} else SynErr(188);
			Expect(11);

#line  1240 "cs.ATG" 
			if (lastBlock != null) { lastBlock.StartLocation = startLocation; lastBlock.EndLocation = t.EndLocation; } 
		}
	}

	void GetAccessorDecl(
#line  1169 "cs.ATG" 
out PropertyGetRegion getBlock, List<AttributeSection> attributes) {

#line  1170 "cs.ATG" 
		Statement stmt = null; 
		Expect(128);

#line  1173 "cs.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1174 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(189);

#line  1175 "cs.ATG" 
		getBlock = new PropertyGetRegion((BlockStatement)stmt, attributes); 

#line  1176 "cs.ATG" 
		getBlock.StartLocation = startLocation; getBlock.EndLocation = t.EndLocation; 
	}

	void SetAccessorDecl(
#line  1179 "cs.ATG" 
out PropertySetRegion setBlock, List<AttributeSection> attributes) {

#line  1180 "cs.ATG" 
		Statement stmt = null; 
		Expect(129);

#line  1183 "cs.ATG" 
		Location startLocation = t.Location; 
		if (la.kind == 16) {
			Block(
#line  1184 "cs.ATG" 
out stmt);
		} else if (la.kind == 11) {
			lexer.NextToken();
		} else SynErr(190);

#line  1185 "cs.ATG" 
		setBlock = new PropertySetRegion((BlockStatement)stmt, attributes); 

#line  1186 "cs.ATG" 
		setBlock.StartLocation = startLocation; setBlock.EndLocation = t.EndLocation; 
	}

	void AddAccessorDecl(
#line  1267 "cs.ATG" 
out Statement stmt) {

#line  1268 "cs.ATG" 
		stmt = null;
		Expect(130);
		Block(
#line  1271 "cs.ATG" 
out stmt);
	}

	void RemoveAccessorDecl(
#line  1274 "cs.ATG" 
out Statement stmt) {

#line  1275 "cs.ATG" 
		stmt = null;
		Expect(131);
		Block(
#line  1278 "cs.ATG" 
out stmt);
	}

	void VariableInitializer(
#line  1294 "cs.ATG" 
out Expression initializerExpression) {

#line  1295 "cs.ATG" 
		TypeReference type = null; Expression expr = null; initializerExpression = null; 
		if (StartOf(6)) {
			Expr(
#line  1297 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 16) {
			CollectionInitializer(
#line  1298 "cs.ATG" 
out initializerExpression);
		} else if (la.kind == 106) {
			lexer.NextToken();
			Type(
#line  1299 "cs.ATG" 
out type);
			Expect(18);
			Expr(
#line  1299 "cs.ATG" 
out expr);
			Expect(19);

#line  1299 "cs.ATG" 
			initializerExpression = new StackAllocExpression(type, expr); 
		} else SynErr(191);
	}

	void Statement() {

#line  1440 "cs.ATG" 
		TypeReference type;
		Expression expr;
		Statement stmt = null;
		Location startPos = la.Location;
		
		while (!(StartOf(29))) {SynErr(192); lexer.NextToken(); }
		if (
#line  1449 "cs.ATG" 
IsLabel()) {
			Identifier();

#line  1449 "cs.ATG" 
			compilationUnit.AddChild(new LabelStatement(t.val)); 
			Expect(9);
			Statement();
		} else if (la.kind == 60) {
			lexer.NextToken();
			Type(
#line  1452 "cs.ATG" 
out type);

#line  1452 "cs.ATG" 
			LocalVariableDeclaration var = new LocalVariableDeclaration(type, Modifiers.Const); string ident = null; var.StartLocation = t.Location; 
			Identifier();

#line  1453 "cs.ATG" 
			ident = t.val; 
			Expect(3);
			Expr(
#line  1454 "cs.ATG" 
out expr);

#line  1454 "cs.ATG" 
			SafeAdd(var, var.Variables, new VariableDeclaration(ident, expr)); 
			while (la.kind == 14) {
				lexer.NextToken();
				Identifier();

#line  1455 "cs.ATG" 
				ident = t.val; 
				Expect(3);
				Expr(
#line  1455 "cs.ATG" 
out expr);

#line  1455 "cs.ATG" 
				SafeAdd(var, var.Variables, new VariableDeclaration(ident, expr)); 
			}
			Expect(11);

#line  1456 "cs.ATG" 
			compilationUnit.AddChild(var); 
		} else if (
#line  1459 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1459 "cs.ATG" 
out stmt);
			Expect(11);

#line  1459 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else if (StartOf(30)) {
			EmbeddedStatement(
#line  1461 "cs.ATG" 
out stmt);

#line  1461 "cs.ATG" 
			compilationUnit.AddChild(stmt); 
		} else SynErr(193);

#line  1467 "cs.ATG" 
		if (stmt != null) {
		stmt.StartLocation = startPos;
		stmt.EndLocation = t.EndLocation;
		}
		
	}

	void Argument(
#line  1334 "cs.ATG" 
out Expression argumentexpr) {

#line  1336 "cs.ATG" 
		Expression expr;
		FieldDirection fd = FieldDirection.None;
		
		if (la.kind == 93 || la.kind == 100) {
			if (la.kind == 100) {
				lexer.NextToken();

#line  1341 "cs.ATG" 
				fd = FieldDirection.Ref; 
			} else {
				lexer.NextToken();

#line  1342 "cs.ATG" 
				fd = FieldDirection.Out; 
			}
		}
		Expr(
#line  1344 "cs.ATG" 
out expr);

#line  1345 "cs.ATG" 
		argumentexpr = fd != FieldDirection.None ? argumentexpr = new DirectionExpression(fd, expr) : expr; 
	}

	void CollectionInitializer(
#line  1365 "cs.ATG" 
out Expression outExpr) {

#line  1367 "cs.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(16);

#line  1371 "cs.ATG" 
		initializer.StartLocation = t.Location; 
		if (StartOf(31)) {
			VariableInitializer(
#line  1372 "cs.ATG" 
out expr);

#line  1373 "cs.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			while (
#line  1374 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				VariableInitializer(
#line  1375 "cs.ATG" 
out expr);

#line  1376 "cs.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);

#line  1380 "cs.ATG" 
		initializer.EndLocation = t.Location; outExpr = initializer; 
	}

	void AssignmentOperator(
#line  1348 "cs.ATG" 
out AssignmentOperatorType op) {

#line  1349 "cs.ATG" 
		op = AssignmentOperatorType.None; 
		if (la.kind == 3) {
			lexer.NextToken();

#line  1351 "cs.ATG" 
			op = AssignmentOperatorType.Assign; 
		} else if (la.kind == 38) {
			lexer.NextToken();

#line  1352 "cs.ATG" 
			op = AssignmentOperatorType.Add; 
		} else if (la.kind == 39) {
			lexer.NextToken();

#line  1353 "cs.ATG" 
			op = AssignmentOperatorType.Subtract; 
		} else if (la.kind == 40) {
			lexer.NextToken();

#line  1354 "cs.ATG" 
			op = AssignmentOperatorType.Multiply; 
		} else if (la.kind == 41) {
			lexer.NextToken();

#line  1355 "cs.ATG" 
			op = AssignmentOperatorType.Divide; 
		} else if (la.kind == 42) {
			lexer.NextToken();

#line  1356 "cs.ATG" 
			op = AssignmentOperatorType.Modulus; 
		} else if (la.kind == 43) {
			lexer.NextToken();

#line  1357 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseAnd; 
		} else if (la.kind == 44) {
			lexer.NextToken();

#line  1358 "cs.ATG" 
			op = AssignmentOperatorType.BitwiseOr; 
		} else if (la.kind == 45) {
			lexer.NextToken();

#line  1359 "cs.ATG" 
			op = AssignmentOperatorType.ExclusiveOr; 
		} else if (la.kind == 46) {
			lexer.NextToken();

#line  1360 "cs.ATG" 
			op = AssignmentOperatorType.ShiftLeft; 
		} else if (
#line  1361 "cs.ATG" 
la.kind == Tokens.GreaterThan && Peek(1).kind == Tokens.GreaterEqual) {
			Expect(22);
			Expect(35);

#line  1362 "cs.ATG" 
			op = AssignmentOperatorType.ShiftRight; 
		} else SynErr(194);
	}

	void CollectionOrObjectInitializer(
#line  1383 "cs.ATG" 
out Expression outExpr) {

#line  1385 "cs.ATG" 
		Expression expr = null;
		CollectionInitializerExpression initializer = new CollectionInitializerExpression();
		
		Expect(16);

#line  1389 "cs.ATG" 
		initializer.StartLocation = t.Location; 
		if (StartOf(31)) {
			ObjectPropertyInitializerOrVariableInitializer(
#line  1390 "cs.ATG" 
out expr);

#line  1391 "cs.ATG" 
			if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			while (
#line  1392 "cs.ATG" 
NotFinalComma()) {
				Expect(14);
				ObjectPropertyInitializerOrVariableInitializer(
#line  1393 "cs.ATG" 
out expr);

#line  1394 "cs.ATG" 
				if (expr != null) { initializer.CreateExpressions.Add(expr); } 
			}
			if (la.kind == 14) {
				lexer.NextToken();
			}
		}
		Expect(17);

#line  1398 "cs.ATG" 
		initializer.EndLocation = t.Location; outExpr = initializer; 
	}

	void ObjectPropertyInitializerOrVariableInitializer(
#line  1401 "cs.ATG" 
out Expression expr) {

#line  1402 "cs.ATG" 
		expr = null; 
		if (
#line  1404 "cs.ATG" 
IdentAndAsgn()) {
			Identifier();

#line  1406 "cs.ATG" 
			NamedArgumentExpression nae = new NamedArgumentExpression(t.val, null);
			nae.StartLocation = t.Location;
			Expression r = null; 
			Expect(3);
			if (la.kind == 16) {
				CollectionOrObjectInitializer(
#line  1410 "cs.ATG" 
out r);
			} else if (StartOf(31)) {
				VariableInitializer(
#line  1411 "cs.ATG" 
out r);
			} else SynErr(195);

#line  1412 "cs.ATG" 
			nae.Expression = r; nae.EndLocation = t.EndLocation; expr = nae; 
		} else if (StartOf(31)) {
			VariableInitializer(
#line  1414 "cs.ATG" 
out expr);
		} else SynErr(196);
	}

	void LocalVariableDecl(
#line  1418 "cs.ATG" 
out Statement stmt) {

#line  1420 "cs.ATG" 
		TypeReference type;
		VariableDeclaration      var = null;
		LocalVariableDeclaration localVariableDeclaration; 
		
		Type(
#line  1425 "cs.ATG" 
out type);

#line  1425 "cs.ATG" 
		localVariableDeclaration = new LocalVariableDeclaration(type); localVariableDeclaration.StartLocation = t.Location; 
		LocalVariableDeclarator(
#line  1426 "cs.ATG" 
out var);

#line  1426 "cs.ATG" 
		SafeAdd(localVariableDeclaration, localVariableDeclaration.Variables, var); 
		while (la.kind == 14) {
			lexer.NextToken();
			LocalVariableDeclarator(
#line  1427 "cs.ATG" 
out var);

#line  1427 "cs.ATG" 
			SafeAdd(localVariableDeclaration, localVariableDeclaration.Variables, var); 
		}

#line  1428 "cs.ATG" 
		stmt = localVariableDeclaration; 
	}

	void LocalVariableDeclarator(
#line  1431 "cs.ATG" 
out VariableDeclaration var) {

#line  1432 "cs.ATG" 
		Expression expr = null; 
		Identifier();

#line  1434 "cs.ATG" 
		var = new VariableDeclaration(t.val); 
		if (la.kind == 3) {
			lexer.NextToken();
			VariableInitializer(
#line  1435 "cs.ATG" 
out expr);

#line  1435 "cs.ATG" 
			var.Initializer = expr; 
		}
	}

	void EmbeddedStatement(
#line  1474 "cs.ATG" 
out Statement statement) {

#line  1476 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;
		

#line  1482 "cs.ATG" 
		Location startLocation = la.Location; 
		if (la.kind == 16) {
			Block(
#line  1484 "cs.ATG" 
out statement);
		} else if (la.kind == 11) {
			lexer.NextToken();

#line  1487 "cs.ATG" 
			statement = new EmptyStatement(); 
		} else if (
#line  1490 "cs.ATG" 
UnCheckedAndLBrace()) {

#line  1490 "cs.ATG" 
			Statement block; bool isChecked = true; 
			if (la.kind == 58) {
				lexer.NextToken();
			} else if (la.kind == 118) {
				lexer.NextToken();

#line  1491 "cs.ATG" 
				isChecked = false;
			} else SynErr(197);
			Block(
#line  1492 "cs.ATG" 
out block);

#line  1492 "cs.ATG" 
			statement = isChecked ? (Statement)new CheckedStatement(block) : (Statement)new UncheckedStatement(block); 
		} else if (la.kind == 79) {
			IfStatement(
#line  1495 "cs.ATG" 
out statement);
		} else if (la.kind == 110) {
			lexer.NextToken();

#line  1497 "cs.ATG" 
			List<SwitchSection> switchSections = new List<SwitchSection>(); 
			Expect(20);
			Expr(
#line  1498 "cs.ATG" 
out expr);
			Expect(21);
			Expect(16);
			SwitchSections(
#line  1499 "cs.ATG" 
switchSections);
			Expect(17);

#line  1500 "cs.ATG" 
			statement = new SwitchStatement(expr, switchSections); 
		} else if (la.kind == 125) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1503 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1504 "cs.ATG" 
out embeddedStatement);

#line  1505 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.Start);
		} else if (la.kind == 65) {
			lexer.NextToken();
			EmbeddedStatement(
#line  1507 "cs.ATG" 
out embeddedStatement);
			Expect(125);
			Expect(20);
			Expr(
#line  1508 "cs.ATG" 
out expr);
			Expect(21);
			Expect(11);

#line  1509 "cs.ATG" 
			statement = new DoLoopStatement(expr, embeddedStatement, ConditionType.While, ConditionPosition.End); 
		} else if (la.kind == 76) {
			lexer.NextToken();

#line  1511 "cs.ATG" 
			List<Statement> initializer = null; List<Statement> iterator = null; 
			Expect(20);
			if (StartOf(6)) {
				ForInitializer(
#line  1512 "cs.ATG" 
out initializer);
			}
			Expect(11);
			if (StartOf(6)) {
				Expr(
#line  1513 "cs.ATG" 
out expr);
			}
			Expect(11);
			if (StartOf(6)) {
				ForIterator(
#line  1514 "cs.ATG" 
out iterator);
			}
			Expect(21);
			EmbeddedStatement(
#line  1515 "cs.ATG" 
out embeddedStatement);

#line  1515 "cs.ATG" 
			statement = new ForStatement(initializer, expr, iterator, embeddedStatement); 
		} else if (la.kind == 77) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1517 "cs.ATG" 
out type);
			Identifier();

#line  1517 "cs.ATG" 
			string varName = t.val; 
			Expect(81);
			Expr(
#line  1518 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1519 "cs.ATG" 
out embeddedStatement);

#line  1520 "cs.ATG" 
			statement = new ForeachStatement(type, varName , expr, embeddedStatement); 
		} else if (la.kind == 53) {
			lexer.NextToken();
			Expect(11);

#line  1523 "cs.ATG" 
			statement = new BreakStatement(); 
		} else if (la.kind == 61) {
			lexer.NextToken();
			Expect(11);

#line  1524 "cs.ATG" 
			statement = new ContinueStatement(); 
		} else if (la.kind == 78) {
			GotoStatement(
#line  1525 "cs.ATG" 
out statement);
		} else if (
#line  1527 "cs.ATG" 
IsYieldStatement()) {
			Expect(132);
			if (la.kind == 101) {
				lexer.NextToken();
				Expr(
#line  1528 "cs.ATG" 
out expr);

#line  1528 "cs.ATG" 
				statement = new YieldStatement(new ReturnStatement(expr)); 
			} else if (la.kind == 53) {
				lexer.NextToken();

#line  1529 "cs.ATG" 
				statement = new YieldStatement(new BreakStatement()); 
			} else SynErr(198);
			Expect(11);
		} else if (la.kind == 101) {
			lexer.NextToken();
			if (StartOf(6)) {
				Expr(
#line  1532 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1532 "cs.ATG" 
			statement = new ReturnStatement(expr); 
		} else if (la.kind == 112) {
			lexer.NextToken();
			if (StartOf(6)) {
				Expr(
#line  1533 "cs.ATG" 
out expr);
			}
			Expect(11);

#line  1533 "cs.ATG" 
			statement = new ThrowStatement(expr); 
		} else if (StartOf(6)) {
			StatementExpr(
#line  1536 "cs.ATG" 
out statement);
			while (!(la.kind == 0 || la.kind == 11)) {SynErr(199); lexer.NextToken(); }
			Expect(11);
		} else if (la.kind == 114) {
			TryStatement(
#line  1539 "cs.ATG" 
out statement);
		} else if (la.kind == 86) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1542 "cs.ATG" 
out expr);
			Expect(21);
			EmbeddedStatement(
#line  1543 "cs.ATG" 
out embeddedStatement);

#line  1543 "cs.ATG" 
			statement = new LockStatement(expr, embeddedStatement); 
		} else if (la.kind == 121) {

#line  1546 "cs.ATG" 
			Statement resourceAcquisitionStmt = null; 
			lexer.NextToken();
			Expect(20);
			ResourceAcquisition(
#line  1548 "cs.ATG" 
out resourceAcquisitionStmt);
			Expect(21);
			EmbeddedStatement(
#line  1549 "cs.ATG" 
out embeddedStatement);

#line  1549 "cs.ATG" 
			statement = new UsingStatement(resourceAcquisitionStmt, embeddedStatement); 
		} else if (la.kind == 119) {
			lexer.NextToken();
			Block(
#line  1552 "cs.ATG" 
out embeddedStatement);

#line  1552 "cs.ATG" 
			statement = new UnsafeStatement(embeddedStatement); 
		} else if (la.kind == 74) {

#line  1554 "cs.ATG" 
			Statement pointerDeclarationStmt = null; 
			lexer.NextToken();
			Expect(20);
			ResourceAcquisition(
#line  1556 "cs.ATG" 
out pointerDeclarationStmt);
			Expect(21);
			EmbeddedStatement(
#line  1557 "cs.ATG" 
out embeddedStatement);

#line  1557 "cs.ATG" 
			statement = new FixedStatement(pointerDeclarationStmt, embeddedStatement); 
		} else SynErr(200);

#line  1559 "cs.ATG" 
		if (statement != null) {
		statement.StartLocation = startLocation;
		statement.EndLocation = t.EndLocation;
		}
		
	}

	void IfStatement(
#line  1566 "cs.ATG" 
out Statement statement) {

#line  1568 "cs.ATG" 
		Expression expr = null;
		Statement embeddedStatement = null;
		statement = null;
		
		Expect(79);
		Expect(20);
		Expr(
#line  1574 "cs.ATG" 
out expr);
		Expect(21);
		EmbeddedStatement(
#line  1575 "cs.ATG" 
out embeddedStatement);

#line  1576 "cs.ATG" 
		Statement elseStatement = null; 
		if (la.kind == 67) {
			lexer.NextToken();
			EmbeddedStatement(
#line  1577 "cs.ATG" 
out elseStatement);
		}

#line  1578 "cs.ATG" 
		statement = elseStatement != null ? new IfElseStatement(expr, embeddedStatement, elseStatement) : new IfElseStatement(expr, embeddedStatement); 

#line  1579 "cs.ATG" 
		if (elseStatement is IfElseStatement && (elseStatement as IfElseStatement).TrueStatement.Count == 1) {
		/* else if-section (otherwise we would have a BlockStatment) */
		(statement as IfElseStatement).ElseIfSections.Add(
		             new ElseIfSection((elseStatement as IfElseStatement).Condition,
		                               (elseStatement as IfElseStatement).TrueStatement[0]));
		(statement as IfElseStatement).ElseIfSections.AddRange((elseStatement as IfElseStatement).ElseIfSections);
		(statement as IfElseStatement).FalseStatement = (elseStatement as IfElseStatement).FalseStatement;
		}
		
	}

	void SwitchSections(
#line  1609 "cs.ATG" 
List<SwitchSection> switchSections) {

#line  1611 "cs.ATG" 
		SwitchSection switchSection = new SwitchSection();
		CaseLabel label;
		
		SwitchLabel(
#line  1615 "cs.ATG" 
out label);

#line  1615 "cs.ATG" 
		if (label != null) { switchSection.SwitchLabels.Add(label); } 

#line  1616 "cs.ATG" 
		compilationUnit.BlockStart(switchSection); 
		while (StartOf(32)) {
			if (la.kind == 55 || la.kind == 63) {
				SwitchLabel(
#line  1618 "cs.ATG" 
out label);

#line  1619 "cs.ATG" 
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

#line  1631 "cs.ATG" 
		compilationUnit.BlockEnd(); switchSections.Add(switchSection); 
	}

	void ForInitializer(
#line  1590 "cs.ATG" 
out List<Statement> initializer) {

#line  1592 "cs.ATG" 
		Statement stmt; 
		initializer = new List<Statement>();
		
		if (
#line  1596 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1596 "cs.ATG" 
out stmt);

#line  1596 "cs.ATG" 
			initializer.Add(stmt);
		} else if (StartOf(6)) {
			StatementExpr(
#line  1597 "cs.ATG" 
out stmt);

#line  1597 "cs.ATG" 
			initializer.Add(stmt);
			while (la.kind == 14) {
				lexer.NextToken();
				StatementExpr(
#line  1597 "cs.ATG" 
out stmt);

#line  1597 "cs.ATG" 
				initializer.Add(stmt);
			}
		} else SynErr(201);
	}

	void ForIterator(
#line  1600 "cs.ATG" 
out List<Statement> iterator) {

#line  1602 "cs.ATG" 
		Statement stmt; 
		iterator = new List<Statement>();
		
		StatementExpr(
#line  1606 "cs.ATG" 
out stmt);

#line  1606 "cs.ATG" 
		iterator.Add(stmt);
		while (la.kind == 14) {
			lexer.NextToken();
			StatementExpr(
#line  1606 "cs.ATG" 
out stmt);

#line  1606 "cs.ATG" 
			iterator.Add(stmt); 
		}
	}

	void GotoStatement(
#line  1684 "cs.ATG" 
out Statement stmt) {

#line  1685 "cs.ATG" 
		Expression expr; stmt = null; 
		Expect(78);
		if (StartOf(19)) {
			Identifier();

#line  1689 "cs.ATG" 
			stmt = new GotoStatement(t.val); 
			Expect(11);
		} else if (la.kind == 55) {
			lexer.NextToken();
			Expr(
#line  1690 "cs.ATG" 
out expr);
			Expect(11);

#line  1690 "cs.ATG" 
			stmt = new GotoCaseStatement(expr); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(11);

#line  1691 "cs.ATG" 
			stmt = new GotoCaseStatement(null); 
		} else SynErr(202);
	}

	void StatementExpr(
#line  1711 "cs.ATG" 
out Statement stmt) {

#line  1712 "cs.ATG" 
		Expression expr; 
		Expr(
#line  1714 "cs.ATG" 
out expr);

#line  1717 "cs.ATG" 
		stmt = new ExpressionStatement(expr); 
	}

	void TryStatement(
#line  1641 "cs.ATG" 
out Statement tryStatement) {

#line  1643 "cs.ATG" 
		Statement blockStmt = null, finallyStmt = null;
		List<CatchClause> catchClauses = null;
		
		Expect(114);
		Block(
#line  1647 "cs.ATG" 
out blockStmt);
		if (la.kind == 56) {
			CatchClauses(
#line  1649 "cs.ATG" 
out catchClauses);
			if (la.kind == 73) {
				lexer.NextToken();
				Block(
#line  1649 "cs.ATG" 
out finallyStmt);
			}
		} else if (la.kind == 73) {
			lexer.NextToken();
			Block(
#line  1650 "cs.ATG" 
out finallyStmt);
		} else SynErr(203);

#line  1653 "cs.ATG" 
		tryStatement = new TryCatchStatement(blockStmt, catchClauses, finallyStmt);
			
	}

	void ResourceAcquisition(
#line  1695 "cs.ATG" 
out Statement stmt) {

#line  1697 "cs.ATG" 
		stmt = null;
		Expression expr;
		
		if (
#line  1702 "cs.ATG" 
IsLocalVarDecl()) {
			LocalVariableDecl(
#line  1702 "cs.ATG" 
out stmt);
		} else if (StartOf(6)) {
			Expr(
#line  1703 "cs.ATG" 
out expr);

#line  1707 "cs.ATG" 
			stmt = new ExpressionStatement(expr); 
		} else SynErr(204);
	}

	void SwitchLabel(
#line  1634 "cs.ATG" 
out CaseLabel label) {

#line  1635 "cs.ATG" 
		Expression expr = null; label = null; 
		if (la.kind == 55) {
			lexer.NextToken();
			Expr(
#line  1637 "cs.ATG" 
out expr);
			Expect(9);

#line  1637 "cs.ATG" 
			label =  new CaseLabel(expr); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(9);

#line  1638 "cs.ATG" 
			label =  new CaseLabel(); 
		} else SynErr(205);
	}

	void CatchClauses(
#line  1658 "cs.ATG" 
out List<CatchClause> catchClauses) {

#line  1660 "cs.ATG" 
		catchClauses = new List<CatchClause>();
		
		Expect(56);

#line  1663 "cs.ATG" 
		string identifier;
		Statement stmt;
		TypeReference typeRef;
		
		if (la.kind == 16) {
			Block(
#line  1669 "cs.ATG" 
out stmt);

#line  1669 "cs.ATG" 
			catchClauses.Add(new CatchClause(stmt)); 
		} else if (la.kind == 20) {
			lexer.NextToken();
			ClassType(
#line  1671 "cs.ATG" 
out typeRef, false);

#line  1671 "cs.ATG" 
			identifier = null; 
			if (StartOf(19)) {
				Identifier();

#line  1672 "cs.ATG" 
				identifier = t.val; 
			}
			Expect(21);
			Block(
#line  1673 "cs.ATG" 
out stmt);

#line  1674 "cs.ATG" 
			catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			while (
#line  1675 "cs.ATG" 
IsTypedCatch()) {
				Expect(56);
				Expect(20);
				ClassType(
#line  1675 "cs.ATG" 
out typeRef, false);

#line  1675 "cs.ATG" 
				identifier = null; 
				if (StartOf(19)) {
					Identifier();

#line  1676 "cs.ATG" 
					identifier = t.val; 
				}
				Expect(21);
				Block(
#line  1677 "cs.ATG" 
out stmt);

#line  1678 "cs.ATG" 
				catchClauses.Add(new CatchClause(typeRef, identifier, stmt)); 
			}
			if (la.kind == 56) {
				lexer.NextToken();
				Block(
#line  1680 "cs.ATG" 
out stmt);

#line  1680 "cs.ATG" 
				catchClauses.Add(new CatchClause(stmt)); 
			}
		} else SynErr(206);
	}

	void UnaryExpr(
#line  1744 "cs.ATG" 
out Expression uExpr) {

#line  1746 "cs.ATG" 
		TypeReference type = null;
		Expression expr = null;
		ArrayList expressions = new ArrayList();
		uExpr = null;
		
		while (StartOf(33) || 
#line  1768 "cs.ATG" 
IsTypeCast()) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  1755 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Plus)); 
			} else if (la.kind == 5) {
				lexer.NextToken();

#line  1756 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Minus)); 
			} else if (la.kind == 24) {
				lexer.NextToken();

#line  1757 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Not)); 
			} else if (la.kind == 27) {
				lexer.NextToken();

#line  1758 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.BitNot)); 
			} else if (la.kind == 6) {
				lexer.NextToken();

#line  1759 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Dereference)); 
			} else if (la.kind == 31) {
				lexer.NextToken();

#line  1760 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Increment)); 
			} else if (la.kind == 32) {
				lexer.NextToken();

#line  1761 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.Decrement)); 
			} else if (la.kind == 28) {
				lexer.NextToken();

#line  1762 "cs.ATG" 
				expressions.Add(new UnaryOperatorExpression(UnaryOperatorType.AddressOf)); 
			} else {
				Expect(20);
				Type(
#line  1768 "cs.ATG" 
out type);
				Expect(21);

#line  1768 "cs.ATG" 
				expressions.Add(new CastExpression(type)); 
			}
		}
		if (
#line  1773 "cs.ATG" 
LastExpressionIsUnaryMinus(expressions) && IsMostNegativeIntegerWithoutTypeSuffix()) {
			Expect(2);

#line  1776 "cs.ATG" 
			expressions.RemoveAt(expressions.Count - 1);
			if (t.literalValue is uint) {
				expr = new PrimitiveExpression(int.MinValue, int.MinValue.ToString());
			} else if (t.literalValue is ulong) {
				expr = new PrimitiveExpression(long.MinValue, long.MinValue.ToString());
			} else {
				throw new Exception("t.literalValue must be uint or ulong");
			}
			
		} else if (StartOf(34)) {
			PrimaryExpr(
#line  1785 "cs.ATG" 
out expr);
		} else SynErr(207);

#line  1787 "cs.ATG" 
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
#line  2103 "cs.ATG" 
ref Expression outExpr) {

#line  2104 "cs.ATG" 
		Expression expr;   
		ConditionalAndExpr(
#line  2106 "cs.ATG" 
ref outExpr);
		while (la.kind == 26) {
			lexer.NextToken();
			UnaryExpr(
#line  2106 "cs.ATG" 
out expr);
			ConditionalAndExpr(
#line  2106 "cs.ATG" 
ref expr);

#line  2106 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalOr, expr);  
		}
	}

	void PrimaryExpr(
#line  1804 "cs.ATG" 
out Expression pexpr) {

#line  1806 "cs.ATG" 
		TypeReference type = null;
		Expression expr;
		pexpr = null;
		

#line  1811 "cs.ATG" 
		Location startLocation = la.Location; 
		if (la.kind == 113) {
			lexer.NextToken();

#line  1813 "cs.ATG" 
			pexpr = new PrimitiveExpression(true, "true");  
		} else if (la.kind == 72) {
			lexer.NextToken();

#line  1814 "cs.ATG" 
			pexpr = new PrimitiveExpression(false, "false"); 
		} else if (la.kind == 90) {
			lexer.NextToken();

#line  1815 "cs.ATG" 
			pexpr = new PrimitiveExpression(null, "null");  
		} else if (la.kind == 2) {
			lexer.NextToken();

#line  1816 "cs.ATG" 
			pexpr = new PrimitiveExpression(t.literalValue, t.val);  
		} else if (
#line  1817 "cs.ATG" 
StartOfQueryExpression()) {
			QueryExpression(
#line  1818 "cs.ATG" 
out pexpr);
		} else if (
#line  1819 "cs.ATG" 
IdentAndDoubleColon()) {
			Identifier();

#line  1820 "cs.ATG" 
			type = new TypeReference(t.val); 
			Expect(10);

#line  1821 "cs.ATG" 
			pexpr = new TypeReferenceExpression(type); 
			Identifier();

#line  1822 "cs.ATG" 
			if (type.Type == "global") { type.IsGlobal = true; type.Type = (t.val ?? "?"); } else type.Type += "." + (t.val ?? "?"); 
		} else if (StartOf(19)) {
			Identifier();

#line  1826 "cs.ATG" 
			pexpr = new IdentifierExpression(t.val); 
			if (la.kind == 48 || 
#line  1829 "cs.ATG" 
IsGenericInSimpleNameOrMemberAccess()) {
				if (la.kind == 48) {
					ShortedLambdaExpression(
#line  1828 "cs.ATG" 
(IdentifierExpression)pexpr, out pexpr);
				} else {

#line  1830 "cs.ATG" 
					List<TypeReference> typeList; 
					TypeArgumentList(
#line  1831 "cs.ATG" 
out typeList, false);

#line  1832 "cs.ATG" 
					((IdentifierExpression)pexpr).TypeArguments = typeList; 
				}
			}
		} else if (
#line  1834 "cs.ATG" 
IsLambdaExpression()) {
			LambdaExpression(
#line  1835 "cs.ATG" 
out pexpr);
		} else if (la.kind == 20) {
			lexer.NextToken();
			Expr(
#line  1838 "cs.ATG" 
out expr);
			Expect(21);

#line  1838 "cs.ATG" 
			pexpr = new ParenthesizedExpression(expr); 
		} else if (StartOf(35)) {

#line  1841 "cs.ATG" 
			string val = null; 
			switch (la.kind) {
			case 52: {
				lexer.NextToken();

#line  1842 "cs.ATG" 
				val = "bool"; 
				break;
			}
			case 54: {
				lexer.NextToken();

#line  1843 "cs.ATG" 
				val = "byte"; 
				break;
			}
			case 57: {
				lexer.NextToken();

#line  1844 "cs.ATG" 
				val = "char"; 
				break;
			}
			case 62: {
				lexer.NextToken();

#line  1845 "cs.ATG" 
				val = "decimal"; 
				break;
			}
			case 66: {
				lexer.NextToken();

#line  1846 "cs.ATG" 
				val = "double"; 
				break;
			}
			case 75: {
				lexer.NextToken();

#line  1847 "cs.ATG" 
				val = "float"; 
				break;
			}
			case 82: {
				lexer.NextToken();

#line  1848 "cs.ATG" 
				val = "int"; 
				break;
			}
			case 87: {
				lexer.NextToken();

#line  1849 "cs.ATG" 
				val = "long"; 
				break;
			}
			case 91: {
				lexer.NextToken();

#line  1850 "cs.ATG" 
				val = "object"; 
				break;
			}
			case 102: {
				lexer.NextToken();

#line  1851 "cs.ATG" 
				val = "sbyte"; 
				break;
			}
			case 104: {
				lexer.NextToken();

#line  1852 "cs.ATG" 
				val = "short"; 
				break;
			}
			case 108: {
				lexer.NextToken();

#line  1853 "cs.ATG" 
				val = "string"; 
				break;
			}
			case 116: {
				lexer.NextToken();

#line  1854 "cs.ATG" 
				val = "uint"; 
				break;
			}
			case 117: {
				lexer.NextToken();

#line  1855 "cs.ATG" 
				val = "ulong"; 
				break;
			}
			case 120: {
				lexer.NextToken();

#line  1856 "cs.ATG" 
				val = "ushort"; 
				break;
			}
			}
			MemberAccess(
#line  1858 "cs.ATG" 
out pexpr, new TypeReferenceExpression(val) { StartLocation = t.Location, EndLocation = t.EndLocation } );
		} else if (la.kind == 111) {
			lexer.NextToken();

#line  1861 "cs.ATG" 
			pexpr = new ThisReferenceExpression(); 
		} else if (la.kind == 51) {
			lexer.NextToken();

#line  1863 "cs.ATG" 
			pexpr = new BaseReferenceExpression(); 
		} else if (la.kind == 89) {
			NewExpression(
#line  1866 "cs.ATG" 
out pexpr);
		} else if (la.kind == 115) {
			lexer.NextToken();
			Expect(20);
			if (
#line  1870 "cs.ATG" 
NotVoidPointer()) {
				Expect(123);

#line  1870 "cs.ATG" 
				type = new TypeReference("void"); 
			} else if (StartOf(10)) {
				TypeWithRestriction(
#line  1871 "cs.ATG" 
out type, true, true);
			} else SynErr(208);
			Expect(21);

#line  1873 "cs.ATG" 
			pexpr = new TypeOfExpression(type); 
		} else if (la.kind == 63) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1875 "cs.ATG" 
out type);
			Expect(21);

#line  1875 "cs.ATG" 
			pexpr = new DefaultValueExpression(type); 
		} else if (la.kind == 105) {
			lexer.NextToken();
			Expect(20);
			Type(
#line  1876 "cs.ATG" 
out type);
			Expect(21);

#line  1876 "cs.ATG" 
			pexpr = new SizeOfExpression(type); 
		} else if (la.kind == 58) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1877 "cs.ATG" 
out expr);
			Expect(21);

#line  1877 "cs.ATG" 
			pexpr = new CheckedExpression(expr); 
		} else if (la.kind == 118) {
			lexer.NextToken();
			Expect(20);
			Expr(
#line  1878 "cs.ATG" 
out expr);
			Expect(21);

#line  1878 "cs.ATG" 
			pexpr = new UncheckedExpression(expr); 
		} else if (la.kind == 64) {
			lexer.NextToken();
			AnonymousMethodExpr(
#line  1879 "cs.ATG" 
out expr);

#line  1879 "cs.ATG" 
			pexpr = expr; 
		} else SynErr(209);

#line  1881 "cs.ATG" 
		if (pexpr != null) {
		pexpr.StartLocation = startLocation;
		pexpr.EndLocation = t.EndLocation;
		}
		
		while (StartOf(36)) {
			if (la.kind == 31 || la.kind == 32) {

#line  1887 "cs.ATG" 
				startLocation = la.Location; 
				if (la.kind == 31) {
					lexer.NextToken();

#line  1889 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostIncrement); 
				} else if (la.kind == 32) {
					lexer.NextToken();

#line  1890 "cs.ATG" 
					pexpr = new UnaryOperatorExpression(pexpr, UnaryOperatorType.PostDecrement); 
				} else SynErr(210);
			} else if (la.kind == 47) {
				PointerMemberAccess(
#line  1893 "cs.ATG" 
out pexpr, pexpr);
			} else if (la.kind == 15) {
				MemberAccess(
#line  1894 "cs.ATG" 
out pexpr, pexpr);
			} else if (la.kind == 20) {
				lexer.NextToken();

#line  1898 "cs.ATG" 
				List<Expression> parameters = new List<Expression>(); 

#line  1899 "cs.ATG" 
				pexpr = new InvocationExpression(pexpr, parameters); 
				if (StartOf(26)) {
					Argument(
#line  1900 "cs.ATG" 
out expr);

#line  1900 "cs.ATG" 
					SafeAdd(pexpr, parameters, expr); 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  1901 "cs.ATG" 
out expr);

#line  1901 "cs.ATG" 
						SafeAdd(pexpr, parameters, expr); 
					}
				}
				Expect(21);
			} else {

#line  1907 "cs.ATG" 
				List<Expression> indices = new List<Expression>();
				pexpr = new IndexerExpression(pexpr, indices);
				
				lexer.NextToken();
				Expr(
#line  1910 "cs.ATG" 
out expr);

#line  1910 "cs.ATG" 
				SafeAdd(pexpr, indices, expr); 
				while (la.kind == 14) {
					lexer.NextToken();
					Expr(
#line  1911 "cs.ATG" 
out expr);

#line  1911 "cs.ATG" 
					SafeAdd(pexpr, indices, expr); 
				}
				Expect(19);

#line  1914 "cs.ATG" 
				if (pexpr != null) {
				pexpr.StartLocation = startLocation;
				pexpr.EndLocation = t.EndLocation;
				}
				
			}
		}
	}

	void QueryExpression(
#line  2339 "cs.ATG" 
out Expression outExpr) {

#line  2340 "cs.ATG" 
		QueryExpression q = new QueryExpression(); outExpr = q; q.StartLocation = la.Location; 
		QueryExpressionFromClause fromClause;
		
		QueryExpressionFromClause(
#line  2344 "cs.ATG" 
out fromClause);

#line  2344 "cs.ATG" 
		q.FromClause = fromClause; 
		QueryExpressionBody(
#line  2345 "cs.ATG" 
q);

#line  2346 "cs.ATG" 
		q.EndLocation = t.EndLocation; 
	}

	void ShortedLambdaExpression(
#line  2027 "cs.ATG" 
IdentifierExpression ident, out Expression pexpr) {

#line  2028 "cs.ATG" 
		LambdaExpression lambda = new LambdaExpression(); pexpr = lambda; 
		Expect(48);

#line  2033 "cs.ATG" 
		lambda.StartLocation = ident.StartLocation;
		lambda.Parameters.Add(new ParameterDeclarationExpression(null, ident.Identifier));
		lambda.Parameters[0].StartLocation = ident.StartLocation;
		lambda.Parameters[0].EndLocation = ident.EndLocation;
		
		LambdaExpressionBody(
#line  2038 "cs.ATG" 
lambda);
	}

	void TypeArgumentList(
#line  2273 "cs.ATG" 
out List<TypeReference> types, bool canBeUnbound) {

#line  2275 "cs.ATG" 
		types = new List<TypeReference>();
		TypeReference type = null;
		
		Expect(23);
		if (
#line  2280 "cs.ATG" 
canBeUnbound && (la.kind == Tokens.GreaterThan || la.kind == Tokens.Comma)) {

#line  2281 "cs.ATG" 
			types.Add(TypeReference.Null); 
			while (la.kind == 14) {
				lexer.NextToken();

#line  2282 "cs.ATG" 
				types.Add(TypeReference.Null); 
			}
		} else if (StartOf(10)) {
			Type(
#line  2283 "cs.ATG" 
out type);

#line  2283 "cs.ATG" 
			if (type != null) { types.Add(type); } 
			while (la.kind == 14) {
				lexer.NextToken();
				Type(
#line  2284 "cs.ATG" 
out type);

#line  2284 "cs.ATG" 
				if (type != null) { types.Add(type); } 
			}
		} else SynErr(211);
		Expect(22);
	}

	void LambdaExpression(
#line  2007 "cs.ATG" 
out Expression outExpr) {

#line  2009 "cs.ATG" 
		LambdaExpression lambda = new LambdaExpression();
		lambda.StartLocation = la.Location;
		ParameterDeclarationExpression p;
		outExpr = lambda;
		
		Expect(20);
		if (StartOf(10)) {
			LambdaExpressionParameter(
#line  2017 "cs.ATG" 
out p);

#line  2017 "cs.ATG" 
			SafeAdd(lambda, lambda.Parameters, p); 
			while (la.kind == 14) {
				lexer.NextToken();
				LambdaExpressionParameter(
#line  2019 "cs.ATG" 
out p);

#line  2019 "cs.ATG" 
				SafeAdd(lambda, lambda.Parameters, p); 
			}
		}
		Expect(21);
		Expect(48);
		LambdaExpressionBody(
#line  2024 "cs.ATG" 
lambda);
	}

	void MemberAccess(
#line  1922 "cs.ATG" 
out Expression expr, Expression target) {

#line  1923 "cs.ATG" 
		List<TypeReference> typeList; 

#line  1925 "cs.ATG" 
		if (ShouldConvertTargetExpressionToTypeReference(target)) {
		TypeReference type = GetTypeReferenceFromExpression(target);
		if (type != null) {
			target = new TypeReferenceExpression(type) { StartLocation = t.Location, EndLocation = t.EndLocation };
		}
		}
		t.val = ""; // required for TypeReferenceExpressionTests.StandaloneIntReferenceExpression hack
		
		Expect(15);
		Identifier();

#line  1935 "cs.ATG" 
		expr = new MemberReferenceExpression(target, t.val); 
		if (
#line  1936 "cs.ATG" 
IsGenericInSimpleNameOrMemberAccess()) {
			TypeArgumentList(
#line  1937 "cs.ATG" 
out typeList, false);

#line  1938 "cs.ATG" 
			((MemberReferenceExpression)expr).TypeArguments = typeList; 
		}
	}

	void NewExpression(
#line  1954 "cs.ATG" 
out Expression pexpr) {

#line  1955 "cs.ATG" 
		pexpr = null;
		List<Expression> parameters = new List<Expression>();
		TypeReference type = null;
		Expression expr;
		
		Expect(89);
		if (StartOf(10)) {
			NonArrayType(
#line  1962 "cs.ATG" 
out type);
		}
		if (la.kind == 16 || la.kind == 20) {
			if (la.kind == 20) {

#line  1968 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				lexer.NextToken();

#line  1969 "cs.ATG" 
				if (type == null) Error("Cannot use an anonymous type with arguments for the constructor"); 
				if (StartOf(26)) {
					Argument(
#line  1970 "cs.ATG" 
out expr);

#line  1970 "cs.ATG" 
					SafeAdd(oce, parameters, expr); 
					while (la.kind == 14) {
						lexer.NextToken();
						Argument(
#line  1971 "cs.ATG" 
out expr);

#line  1971 "cs.ATG" 
						SafeAdd(oce, parameters, expr); 
					}
				}
				Expect(21);

#line  1973 "cs.ATG" 
				pexpr = oce; 
				if (la.kind == 16) {
					CollectionOrObjectInitializer(
#line  1974 "cs.ATG" 
out expr);

#line  1974 "cs.ATG" 
					oce.ObjectInitializer = (CollectionInitializerExpression)expr; 
				}
			} else {

#line  1975 "cs.ATG" 
				ObjectCreateExpression oce = new ObjectCreateExpression(type, parameters); 
				CollectionOrObjectInitializer(
#line  1976 "cs.ATG" 
out expr);

#line  1976 "cs.ATG" 
				oce.ObjectInitializer = (CollectionInitializerExpression)expr; 

#line  1977 "cs.ATG" 
				pexpr = oce; 
			}
		} else if (la.kind == 18) {
			lexer.NextToken();

#line  1982 "cs.ATG" 
			ArrayCreateExpression ace = new ArrayCreateExpression(type);
			/* we must not change RankSpecifier on the null type reference*/
			if (ace.CreateType.IsNull) { ace.CreateType = new TypeReference(""); }
			pexpr = ace;
			int dims = 0; List<int> ranks = new List<int>();
			
			if (la.kind == 14 || la.kind == 19) {
				while (la.kind == 14) {
					lexer.NextToken();

#line  1989 "cs.ATG" 
					dims += 1; 
				}
				Expect(19);

#line  1990 "cs.ATG" 
				ranks.Add(dims); dims = 0; 
				while (la.kind == 18) {
					lexer.NextToken();
					while (la.kind == 14) {
						lexer.NextToken();

#line  1991 "cs.ATG" 
						++dims; 
					}
					Expect(19);

#line  1991 "cs.ATG" 
					ranks.Add(dims); dims = 0; 
				}

#line  1992 "cs.ATG" 
				ace.CreateType.RankSpecifier = ranks.ToArray(); 
				CollectionInitializer(
#line  1993 "cs.ATG" 
out expr);

#line  1993 "cs.ATG" 
				ace.ArrayInitializer = (CollectionInitializerExpression)expr; 
			} else if (StartOf(6)) {
				Expr(
#line  1994 "cs.ATG" 
out expr);

#line  1994 "cs.ATG" 
				if (expr != null) parameters.Add(expr); 
				while (la.kind == 14) {
					lexer.NextToken();

#line  1995 "cs.ATG" 
					dims += 1; 
					Expr(
#line  1996 "cs.ATG" 
out expr);

#line  1996 "cs.ATG" 
					if (expr != null) parameters.Add(expr); 
				}
				Expect(19);

#line  1998 "cs.ATG" 
				ranks.Add(dims); ace.Arguments = parameters; dims = 0; 
				while (la.kind == 18) {
					lexer.NextToken();
					while (la.kind == 14) {
						lexer.NextToken();

#line  1999 "cs.ATG" 
						++dims; 
					}
					Expect(19);

#line  1999 "cs.ATG" 
					ranks.Add(dims); dims = 0; 
				}

#line  2000 "cs.ATG" 
				ace.CreateType.RankSpecifier = ranks.ToArray(); 
				if (la.kind == 16) {
					CollectionInitializer(
#line  2001 "cs.ATG" 
out expr);

#line  2001 "cs.ATG" 
					ace.ArrayInitializer = (CollectionInitializerExpression)expr; 
				}
			} else SynErr(212);
		} else SynErr(213);
	}

	void AnonymousMethodExpr(
#line  2070 "cs.ATG" 
out Expression outExpr) {

#line  2072 "cs.ATG" 
		AnonymousMethodExpression expr = new AnonymousMethodExpression();
		expr.StartLocation = t.Location;
		BlockStatement stmt;
		List<ParameterDeclarationExpression> p = new List<ParameterDeclarationExpression>();
		outExpr = expr;
		
		if (la.kind == 20) {
			lexer.NextToken();
			if (StartOf(11)) {
				FormalParameterList(
#line  2081 "cs.ATG" 
p);

#line  2081 "cs.ATG" 
				expr.Parameters = p; 
			}
			Expect(21);

#line  2083 "cs.ATG" 
			expr.HasParameterList = true; 
		}
		BlockInsideExpression(
#line  2085 "cs.ATG" 
out stmt);

#line  2085 "cs.ATG" 
		expr.Body  = stmt; 

#line  2086 "cs.ATG" 
		expr.EndLocation = t.Location; 
	}

	void PointerMemberAccess(
#line  1942 "cs.ATG" 
out Expression expr, Expression target) {

#line  1943 "cs.ATG" 
		List<TypeReference> typeList; 
		Expect(47);
		Identifier();

#line  1947 "cs.ATG" 
		expr = new PointerReferenceExpression(target, t.val); 
		if (
#line  1948 "cs.ATG" 
IsGenericInSimpleNameOrMemberAccess()) {
			TypeArgumentList(
#line  1949 "cs.ATG" 
out typeList, false);

#line  1950 "cs.ATG" 
			((MemberReferenceExpression)expr).TypeArguments = typeList; 
		}
	}

	void LambdaExpressionParameter(
#line  2041 "cs.ATG" 
out ParameterDeclarationExpression p) {

#line  2042 "cs.ATG" 
		Location start = la.Location; p = null;
		TypeReference type;
		
		if (
#line  2046 "cs.ATG" 
Peek(1).kind == Tokens.Comma || Peek(1).kind == Tokens.CloseParenthesis) {
			Identifier();

#line  2048 "cs.ATG" 
			p = new ParameterDeclarationExpression(null, t.val);
			p.StartLocation = start; p.EndLocation = t.EndLocation;
			
		} else if (StartOf(10)) {
			Type(
#line  2051 "cs.ATG" 
out type);
			Identifier();

#line  2053 "cs.ATG" 
			p = new ParameterDeclarationExpression(type, t.val);
			p.StartLocation = start; p.EndLocation = t.EndLocation;
			
		} else SynErr(214);
	}

	void LambdaExpressionBody(
#line  2059 "cs.ATG" 
LambdaExpression lambda) {

#line  2060 "cs.ATG" 
		Expression expr; BlockStatement stmt; 
		if (la.kind == 16) {
			BlockInsideExpression(
#line  2063 "cs.ATG" 
out stmt);

#line  2063 "cs.ATG" 
			lambda.StatementBody = stmt; 
		} else if (StartOf(6)) {
			Expr(
#line  2064 "cs.ATG" 
out expr);

#line  2064 "cs.ATG" 
			lambda.ExpressionBody = expr; 
		} else SynErr(215);

#line  2066 "cs.ATG" 
		lambda.EndLocation = t.EndLocation; 

#line  2067 "cs.ATG" 
		lambda.ExtendedEndLocation = la.Location; 
	}

	void BlockInsideExpression(
#line  2089 "cs.ATG" 
out BlockStatement outStmt) {

#line  2090 "cs.ATG" 
		Statement stmt = null; outStmt = null; 

#line  2094 "cs.ATG" 
		if (compilationUnit != null) { 
		Block(
#line  2095 "cs.ATG" 
out stmt);

#line  2095 "cs.ATG" 
		outStmt = (BlockStatement)stmt; 

#line  2096 "cs.ATG" 
		} else { 
		Expect(16);

#line  2098 "cs.ATG" 
		lexer.SkipCurrentBlock(0); 
		Expect(17);

#line  2100 "cs.ATG" 
		} 
	}

	void ConditionalAndExpr(
#line  2109 "cs.ATG" 
ref Expression outExpr) {

#line  2110 "cs.ATG" 
		Expression expr; 
		InclusiveOrExpr(
#line  2112 "cs.ATG" 
ref outExpr);
		while (la.kind == 25) {
			lexer.NextToken();
			UnaryExpr(
#line  2112 "cs.ATG" 
out expr);
			InclusiveOrExpr(
#line  2112 "cs.ATG" 
ref expr);

#line  2112 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.LogicalAnd, expr);  
		}
	}

	void InclusiveOrExpr(
#line  2115 "cs.ATG" 
ref Expression outExpr) {

#line  2116 "cs.ATG" 
		Expression expr; 
		ExclusiveOrExpr(
#line  2118 "cs.ATG" 
ref outExpr);
		while (la.kind == 29) {
			lexer.NextToken();
			UnaryExpr(
#line  2118 "cs.ATG" 
out expr);
			ExclusiveOrExpr(
#line  2118 "cs.ATG" 
ref expr);

#line  2118 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseOr, expr);  
		}
	}

	void ExclusiveOrExpr(
#line  2121 "cs.ATG" 
ref Expression outExpr) {

#line  2122 "cs.ATG" 
		Expression expr; 
		AndExpr(
#line  2124 "cs.ATG" 
ref outExpr);
		while (la.kind == 30) {
			lexer.NextToken();
			UnaryExpr(
#line  2124 "cs.ATG" 
out expr);
			AndExpr(
#line  2124 "cs.ATG" 
ref expr);

#line  2124 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.ExclusiveOr, expr);  
		}
	}

	void AndExpr(
#line  2127 "cs.ATG" 
ref Expression outExpr) {

#line  2128 "cs.ATG" 
		Expression expr; 
		EqualityExpr(
#line  2130 "cs.ATG" 
ref outExpr);
		while (la.kind == 28) {
			lexer.NextToken();
			UnaryExpr(
#line  2130 "cs.ATG" 
out expr);
			EqualityExpr(
#line  2130 "cs.ATG" 
ref expr);

#line  2130 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, BinaryOperatorType.BitwiseAnd, expr);  
		}
	}

	void EqualityExpr(
#line  2133 "cs.ATG" 
ref Expression outExpr) {

#line  2135 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		RelationalExpr(
#line  2139 "cs.ATG" 
ref outExpr);
		while (la.kind == 33 || la.kind == 34) {
			if (la.kind == 34) {
				lexer.NextToken();

#line  2142 "cs.ATG" 
				op = BinaryOperatorType.InEquality; 
			} else {
				lexer.NextToken();

#line  2143 "cs.ATG" 
				op = BinaryOperatorType.Equality; 
			}
			UnaryExpr(
#line  2145 "cs.ATG" 
out expr);
			RelationalExpr(
#line  2145 "cs.ATG" 
ref expr);

#line  2145 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void RelationalExpr(
#line  2149 "cs.ATG" 
ref Expression outExpr) {

#line  2151 "cs.ATG" 
		TypeReference type;
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		ShiftExpr(
#line  2156 "cs.ATG" 
ref outExpr);
		while (StartOf(37)) {
			if (StartOf(38)) {
				if (la.kind == 23) {
					lexer.NextToken();

#line  2158 "cs.ATG" 
					op = BinaryOperatorType.LessThan; 
				} else if (la.kind == 22) {
					lexer.NextToken();

#line  2159 "cs.ATG" 
					op = BinaryOperatorType.GreaterThan; 
				} else if (la.kind == 36) {
					lexer.NextToken();

#line  2160 "cs.ATG" 
					op = BinaryOperatorType.LessThanOrEqual; 
				} else if (la.kind == 35) {
					lexer.NextToken();

#line  2161 "cs.ATG" 
					op = BinaryOperatorType.GreaterThanOrEqual; 
				} else SynErr(216);
				UnaryExpr(
#line  2163 "cs.ATG" 
out expr);
				ShiftExpr(
#line  2164 "cs.ATG" 
ref expr);

#line  2165 "cs.ATG" 
				outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
			} else {
				if (la.kind == 85) {
					lexer.NextToken();
					TypeWithRestriction(
#line  2168 "cs.ATG" 
out type, false, false);
					if (
#line  2169 "cs.ATG" 
la.kind == Tokens.Question && !IsPossibleExpressionStart(Peek(1).kind)) {
						NullableQuestionMark(
#line  2170 "cs.ATG" 
ref type);
					}

#line  2171 "cs.ATG" 
					outExpr = new TypeOfIsExpression(outExpr, type); 
				} else if (la.kind == 50) {
					lexer.NextToken();
					TypeWithRestriction(
#line  2173 "cs.ATG" 
out type, false, false);
					if (
#line  2174 "cs.ATG" 
la.kind == Tokens.Question && !IsPossibleExpressionStart(Peek(1).kind)) {
						NullableQuestionMark(
#line  2175 "cs.ATG" 
ref type);
					}

#line  2176 "cs.ATG" 
					outExpr = new CastExpression(type, outExpr, CastType.TryCast); 
				} else SynErr(217);
			}
		}
	}

	void ShiftExpr(
#line  2181 "cs.ATG" 
ref Expression outExpr) {

#line  2183 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		AdditiveExpr(
#line  2187 "cs.ATG" 
ref outExpr);
		while (la.kind == 37 || 
#line  2190 "cs.ATG" 
IsShiftRight()) {
			if (la.kind == 37) {
				lexer.NextToken();

#line  2189 "cs.ATG" 
				op = BinaryOperatorType.ShiftLeft; 
			} else {
				Expect(22);
				Expect(22);

#line  2191 "cs.ATG" 
				op = BinaryOperatorType.ShiftRight; 
			}
			UnaryExpr(
#line  2194 "cs.ATG" 
out expr);
			AdditiveExpr(
#line  2194 "cs.ATG" 
ref expr);

#line  2194 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void AdditiveExpr(
#line  2198 "cs.ATG" 
ref Expression outExpr) {

#line  2200 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		MultiplicativeExpr(
#line  2204 "cs.ATG" 
ref outExpr);
		while (la.kind == 4 || la.kind == 5) {
			if (la.kind == 4) {
				lexer.NextToken();

#line  2207 "cs.ATG" 
				op = BinaryOperatorType.Add; 
			} else {
				lexer.NextToken();

#line  2208 "cs.ATG" 
				op = BinaryOperatorType.Subtract; 
			}
			UnaryExpr(
#line  2210 "cs.ATG" 
out expr);
			MultiplicativeExpr(
#line  2210 "cs.ATG" 
ref expr);

#line  2210 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr);  
		}
	}

	void MultiplicativeExpr(
#line  2214 "cs.ATG" 
ref Expression outExpr) {

#line  2216 "cs.ATG" 
		Expression expr;
		BinaryOperatorType op = BinaryOperatorType.None;
		
		while (la.kind == 6 || la.kind == 7 || la.kind == 8) {
			if (la.kind == 6) {
				lexer.NextToken();

#line  2222 "cs.ATG" 
				op = BinaryOperatorType.Multiply; 
			} else if (la.kind == 7) {
				lexer.NextToken();

#line  2223 "cs.ATG" 
				op = BinaryOperatorType.Divide; 
			} else {
				lexer.NextToken();

#line  2224 "cs.ATG" 
				op = BinaryOperatorType.Modulus; 
			}
			UnaryExpr(
#line  2226 "cs.ATG" 
out expr);

#line  2226 "cs.ATG" 
			outExpr = new BinaryOperatorExpression(outExpr, op, expr); 
		}
	}

	void TypeParameterConstraintsClauseBase(
#line  2330 "cs.ATG" 
out TypeReference type) {

#line  2331 "cs.ATG" 
		TypeReference t; type = null; 
		if (la.kind == 109) {
			lexer.NextToken();

#line  2333 "cs.ATG" 
			type = TypeReference.StructConstraint; 
		} else if (la.kind == 59) {
			lexer.NextToken();

#line  2334 "cs.ATG" 
			type = TypeReference.ClassConstraint; 
		} else if (la.kind == 89) {
			lexer.NextToken();
			Expect(20);
			Expect(21);

#line  2335 "cs.ATG" 
			type = TypeReference.NewConstraint; 
		} else if (StartOf(10)) {
			Type(
#line  2336 "cs.ATG" 
out t);

#line  2336 "cs.ATG" 
			type = t; 
		} else SynErr(218);
	}

	void QueryExpressionFromClause(
#line  2349 "cs.ATG" 
out QueryExpressionFromClause fc) {

#line  2350 "cs.ATG" 
		fc = new QueryExpressionFromClause(); fc.StartLocation = la.Location; 
		
		Expect(137);
		QueryExpressionFromOrJoinClause(
#line  2354 "cs.ATG" 
fc);

#line  2355 "cs.ATG" 
		fc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionBody(
#line  2385 "cs.ATG" 
QueryExpression q) {

#line  2386 "cs.ATG" 
		QueryExpressionFromClause fromClause;     QueryExpressionWhereClause whereClause;
		QueryExpressionLetClause letClause;       QueryExpressionJoinClause joinClause;
		QueryExpressionOrderClause orderClause;
		QueryExpressionSelectClause selectClause; QueryExpressionGroupClause groupClause;
		QueryExpressionIntoClause intoClause;
		
		while (StartOf(39)) {
			if (la.kind == 137) {
				QueryExpressionFromClause(
#line  2393 "cs.ATG" 
out fromClause);

#line  2393 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, fromClause); 
			} else if (la.kind == 127) {
				QueryExpressionWhereClause(
#line  2394 "cs.ATG" 
out whereClause);

#line  2394 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, whereClause); 
			} else if (la.kind == 141) {
				QueryExpressionLetClause(
#line  2395 "cs.ATG" 
out letClause);

#line  2395 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, letClause); 
			} else if (la.kind == 142) {
				QueryExpressionJoinClause(
#line  2396 "cs.ATG" 
out joinClause);

#line  2396 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, joinClause); 
			} else {
				QueryExpressionOrderByClause(
#line  2397 "cs.ATG" 
out orderClause);

#line  2397 "cs.ATG" 
				SafeAdd<QueryExpressionClause>(q, q.MiddleClauses, orderClause); 
			}
		}
		if (la.kind == 133) {
			QueryExpressionSelectClause(
#line  2399 "cs.ATG" 
out selectClause);

#line  2399 "cs.ATG" 
			q.SelectOrGroupClause = selectClause; 
		} else if (la.kind == 134) {
			QueryExpressionGroupClause(
#line  2400 "cs.ATG" 
out groupClause);

#line  2400 "cs.ATG" 
			q.SelectOrGroupClause = groupClause; 
		} else SynErr(219);
		if (la.kind == 136) {
			QueryExpressionIntoClause(
#line  2402 "cs.ATG" 
out intoClause);

#line  2402 "cs.ATG" 
			q.IntoClause = intoClause; 
		}
	}

	void QueryExpressionFromOrJoinClause(
#line  2375 "cs.ATG" 
QueryExpressionFromOrJoinClause fjc) {

#line  2376 "cs.ATG" 
		TypeReference type; Expression expr; 

#line  2378 "cs.ATG" 
		fjc.Type = null; 
		if (
#line  2379 "cs.ATG" 
IsLocalVarDecl()) {
			Type(
#line  2379 "cs.ATG" 
out type);

#line  2379 "cs.ATG" 
			fjc.Type = type; 
		}
		Identifier();

#line  2380 "cs.ATG" 
		fjc.Identifier = t.val; 
		Expect(81);
		Expr(
#line  2382 "cs.ATG" 
out expr);

#line  2382 "cs.ATG" 
		fjc.InExpression = expr; 
	}

	void QueryExpressionJoinClause(
#line  2358 "cs.ATG" 
out QueryExpressionJoinClause jc) {

#line  2359 "cs.ATG" 
		jc = new QueryExpressionJoinClause(); jc.StartLocation = la.Location; 
		Expression expr;
		
		Expect(142);
		QueryExpressionFromOrJoinClause(
#line  2364 "cs.ATG" 
jc);
		Expect(143);
		Expr(
#line  2366 "cs.ATG" 
out expr);

#line  2366 "cs.ATG" 
		jc.OnExpression = expr; 
		Expect(144);
		Expr(
#line  2368 "cs.ATG" 
out expr);

#line  2368 "cs.ATG" 
		jc.EqualsExpression = expr; 
		if (la.kind == 136) {
			lexer.NextToken();
			Identifier();

#line  2370 "cs.ATG" 
			jc.IntoIdentifier = t.val; 
		}

#line  2372 "cs.ATG" 
		jc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionWhereClause(
#line  2405 "cs.ATG" 
out QueryExpressionWhereClause wc) {

#line  2406 "cs.ATG" 
		Expression expr; wc = new QueryExpressionWhereClause(); wc.StartLocation = la.Location; 
		Expect(127);
		Expr(
#line  2409 "cs.ATG" 
out expr);

#line  2409 "cs.ATG" 
		wc.Condition = expr; 

#line  2410 "cs.ATG" 
		wc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionLetClause(
#line  2413 "cs.ATG" 
out QueryExpressionLetClause wc) {

#line  2414 "cs.ATG" 
		Expression expr; wc = new QueryExpressionLetClause(); wc.StartLocation = la.Location; 
		Expect(141);
		Identifier();

#line  2417 "cs.ATG" 
		wc.Identifier = t.val; 
		Expect(3);
		Expr(
#line  2419 "cs.ATG" 
out expr);

#line  2419 "cs.ATG" 
		wc.Expression = expr; 

#line  2420 "cs.ATG" 
		wc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionOrderByClause(
#line  2423 "cs.ATG" 
out QueryExpressionOrderClause oc) {

#line  2424 "cs.ATG" 
		QueryExpressionOrdering ordering; oc = new QueryExpressionOrderClause(); oc.StartLocation = la.Location; 
		Expect(140);
		QueryExpressionOrdering(
#line  2427 "cs.ATG" 
out ordering);

#line  2427 "cs.ATG" 
		SafeAdd(oc, oc.Orderings, ordering); 
		while (la.kind == 14) {
			lexer.NextToken();
			QueryExpressionOrdering(
#line  2429 "cs.ATG" 
out ordering);

#line  2429 "cs.ATG" 
			SafeAdd(oc, oc.Orderings, ordering); 
		}

#line  2431 "cs.ATG" 
		oc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionSelectClause(
#line  2444 "cs.ATG" 
out QueryExpressionSelectClause sc) {

#line  2445 "cs.ATG" 
		Expression expr; sc = new QueryExpressionSelectClause(); sc.StartLocation = la.Location; 
		Expect(133);
		Expr(
#line  2448 "cs.ATG" 
out expr);

#line  2448 "cs.ATG" 
		sc.Projection = expr; 

#line  2449 "cs.ATG" 
		sc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionGroupClause(
#line  2452 "cs.ATG" 
out QueryExpressionGroupClause gc) {

#line  2453 "cs.ATG" 
		Expression expr; gc = new QueryExpressionGroupClause(); gc.StartLocation = la.Location; 
		Expect(134);
		Expr(
#line  2456 "cs.ATG" 
out expr);

#line  2456 "cs.ATG" 
		gc.Projection = expr; 
		Expect(135);
		Expr(
#line  2458 "cs.ATG" 
out expr);

#line  2458 "cs.ATG" 
		gc.GroupBy = expr; 

#line  2459 "cs.ATG" 
		gc.EndLocation = t.EndLocation; 
	}

	void QueryExpressionIntoClause(
#line  2462 "cs.ATG" 
out QueryExpressionIntoClause ic) {

#line  2463 "cs.ATG" 
		ic = new QueryExpressionIntoClause(); ic.StartLocation = la.Location; 
		Expect(136);
		Identifier();

#line  2466 "cs.ATG" 
		ic.IntoIdentifier = t.val; 

#line  2467 "cs.ATG" 
		ic.ContinuedQuery = new QueryExpression(); 

#line  2468 "cs.ATG" 
		ic.ContinuedQuery.StartLocation = la.Location; 
		QueryExpressionBody(
#line  2469 "cs.ATG" 
ic.ContinuedQuery);

#line  2470 "cs.ATG" 
		ic.ContinuedQuery.EndLocation = t.EndLocation; 

#line  2471 "cs.ATG" 
		ic.EndLocation = t.EndLocation; 
	}

	void QueryExpressionOrdering(
#line  2434 "cs.ATG" 
out QueryExpressionOrdering ordering) {

#line  2435 "cs.ATG" 
		Expression expr; ordering = new QueryExpressionOrdering(); ordering.StartLocation = la.Location; 
		Expr(
#line  2437 "cs.ATG" 
out expr);

#line  2437 "cs.ATG" 
		ordering.Criteria = expr; 
		if (la.kind == 138 || la.kind == 139) {
			if (la.kind == 138) {
				lexer.NextToken();

#line  2438 "cs.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Ascending; 
			} else {
				lexer.NextToken();

#line  2439 "cs.ATG" 
				ordering.Direction = QueryExpressionOrderingDirection.Descending; 
			}
		}

#line  2441 "cs.ATG" 
		ordering.EndLocation = t.EndLocation; 
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
			case 48: s = "\"=>\" expected"; break;
			case 49: s = "\"abstract\" expected"; break;
			case 50: s = "\"as\" expected"; break;
			case 51: s = "\"base\" expected"; break;
			case 52: s = "\"bool\" expected"; break;
			case 53: s = "\"break\" expected"; break;
			case 54: s = "\"byte\" expected"; break;
			case 55: s = "\"case\" expected"; break;
			case 56: s = "\"catch\" expected"; break;
			case 57: s = "\"char\" expected"; break;
			case 58: s = "\"checked\" expected"; break;
			case 59: s = "\"class\" expected"; break;
			case 60: s = "\"const\" expected"; break;
			case 61: s = "\"continue\" expected"; break;
			case 62: s = "\"decimal\" expected"; break;
			case 63: s = "\"default\" expected"; break;
			case 64: s = "\"delegate\" expected"; break;
			case 65: s = "\"do\" expected"; break;
			case 66: s = "\"double\" expected"; break;
			case 67: s = "\"else\" expected"; break;
			case 68: s = "\"enum\" expected"; break;
			case 69: s = "\"event\" expected"; break;
			case 70: s = "\"explicit\" expected"; break;
			case 71: s = "\"extern\" expected"; break;
			case 72: s = "\"false\" expected"; break;
			case 73: s = "\"finally\" expected"; break;
			case 74: s = "\"fixed\" expected"; break;
			case 75: s = "\"float\" expected"; break;
			case 76: s = "\"for\" expected"; break;
			case 77: s = "\"foreach\" expected"; break;
			case 78: s = "\"goto\" expected"; break;
			case 79: s = "\"if\" expected"; break;
			case 80: s = "\"implicit\" expected"; break;
			case 81: s = "\"in\" expected"; break;
			case 82: s = "\"int\" expected"; break;
			case 83: s = "\"interface\" expected"; break;
			case 84: s = "\"internal\" expected"; break;
			case 85: s = "\"is\" expected"; break;
			case 86: s = "\"lock\" expected"; break;
			case 87: s = "\"long\" expected"; break;
			case 88: s = "\"namespace\" expected"; break;
			case 89: s = "\"new\" expected"; break;
			case 90: s = "\"null\" expected"; break;
			case 91: s = "\"object\" expected"; break;
			case 92: s = "\"operator\" expected"; break;
			case 93: s = "\"out\" expected"; break;
			case 94: s = "\"override\" expected"; break;
			case 95: s = "\"params\" expected"; break;
			case 96: s = "\"private\" expected"; break;
			case 97: s = "\"protected\" expected"; break;
			case 98: s = "\"public\" expected"; break;
			case 99: s = "\"readonly\" expected"; break;
			case 100: s = "\"ref\" expected"; break;
			case 101: s = "\"return\" expected"; break;
			case 102: s = "\"sbyte\" expected"; break;
			case 103: s = "\"sealed\" expected"; break;
			case 104: s = "\"short\" expected"; break;
			case 105: s = "\"sizeof\" expected"; break;
			case 106: s = "\"stackalloc\" expected"; break;
			case 107: s = "\"static\" expected"; break;
			case 108: s = "\"string\" expected"; break;
			case 109: s = "\"struct\" expected"; break;
			case 110: s = "\"switch\" expected"; break;
			case 111: s = "\"this\" expected"; break;
			case 112: s = "\"throw\" expected"; break;
			case 113: s = "\"true\" expected"; break;
			case 114: s = "\"try\" expected"; break;
			case 115: s = "\"typeof\" expected"; break;
			case 116: s = "\"uint\" expected"; break;
			case 117: s = "\"ulong\" expected"; break;
			case 118: s = "\"unchecked\" expected"; break;
			case 119: s = "\"unsafe\" expected"; break;
			case 120: s = "\"ushort\" expected"; break;
			case 121: s = "\"using\" expected"; break;
			case 122: s = "\"virtual\" expected"; break;
			case 123: s = "\"void\" expected"; break;
			case 124: s = "\"volatile\" expected"; break;
			case 125: s = "\"while\" expected"; break;
			case 126: s = "\"partial\" expected"; break;
			case 127: s = "\"where\" expected"; break;
			case 128: s = "\"get\" expected"; break;
			case 129: s = "\"set\" expected"; break;
			case 130: s = "\"add\" expected"; break;
			case 131: s = "\"remove\" expected"; break;
			case 132: s = "\"yield\" expected"; break;
			case 133: s = "\"select\" expected"; break;
			case 134: s = "\"group\" expected"; break;
			case 135: s = "\"by\" expected"; break;
			case 136: s = "\"into\" expected"; break;
			case 137: s = "\"from\" expected"; break;
			case 138: s = "\"ascending\" expected"; break;
			case 139: s = "\"descending\" expected"; break;
			case 140: s = "\"orderby\" expected"; break;
			case 141: s = "\"let\" expected"; break;
			case 142: s = "\"join\" expected"; break;
			case 143: s = "\"on\" expected"; break;
			case 144: s = "\"equals\" expected"; break;
			case 145: s = "??? expected"; break;
			case 146: s = "invalid NamespaceMemberDecl"; break;
			case 147: s = "invalid NonArrayType"; break;
			case 148: s = "invalid Identifier"; break;
			case 149: s = "invalid AttributeArguments"; break;
			case 150: s = "invalid Expr"; break;
			case 151: s = "invalid TypeModifier"; break;
			case 152: s = "invalid TypeDecl"; break;
			case 153: s = "invalid TypeDecl"; break;
			case 154: s = "this symbol not expected in ClassBody"; break;
			case 155: s = "this symbol not expected in InterfaceBody"; break;
			case 156: s = "invalid IntegralType"; break;
			case 157: s = "invalid FormalParameterList"; break;
			case 158: s = "invalid FormalParameterList"; break;
			case 159: s = "invalid ClassType"; break;
			case 160: s = "invalid ClassMemberDecl"; break;
			case 161: s = "invalid ClassMemberDecl"; break;
			case 162: s = "invalid StructMemberDecl"; break;
			case 163: s = "invalid StructMemberDecl"; break;
			case 164: s = "invalid StructMemberDecl"; break;
			case 165: s = "invalid StructMemberDecl"; break;
			case 166: s = "invalid StructMemberDecl"; break;
			case 167: s = "invalid StructMemberDecl"; break;
			case 168: s = "invalid StructMemberDecl"; break;
			case 169: s = "invalid StructMemberDecl"; break;
			case 170: s = "invalid StructMemberDecl"; break;
			case 171: s = "invalid StructMemberDecl"; break;
			case 172: s = "invalid StructMemberDecl"; break;
			case 173: s = "invalid StructMemberDecl"; break;
			case 174: s = "invalid StructMemberDecl"; break;
			case 175: s = "invalid InterfaceMemberDecl"; break;
			case 176: s = "invalid InterfaceMemberDecl"; break;
			case 177: s = "invalid InterfaceMemberDecl"; break;
			case 178: s = "invalid TypeWithRestriction"; break;
			case 179: s = "invalid TypeWithRestriction"; break;
			case 180: s = "invalid SimpleType"; break;
			case 181: s = "invalid AccessorModifiers"; break;
			case 182: s = "this symbol not expected in Block"; break;
			case 183: s = "invalid EventAccessorDecls"; break;
			case 184: s = "invalid ConstructorInitializer"; break;
			case 185: s = "invalid OverloadableOperator"; break;
			case 186: s = "invalid AccessorDecls"; break;
			case 187: s = "invalid InterfaceAccessors"; break;
			case 188: s = "invalid InterfaceAccessors"; break;
			case 189: s = "invalid GetAccessorDecl"; break;
			case 190: s = "invalid SetAccessorDecl"; break;
			case 191: s = "invalid VariableInitializer"; break;
			case 192: s = "this symbol not expected in Statement"; break;
			case 193: s = "invalid Statement"; break;
			case 194: s = "invalid AssignmentOperator"; break;
			case 195: s = "invalid ObjectPropertyInitializerOrVariableInitializer"; break;
			case 196: s = "invalid ObjectPropertyInitializerOrVariableInitializer"; break;
			case 197: s = "invalid EmbeddedStatement"; break;
			case 198: s = "invalid EmbeddedStatement"; break;
			case 199: s = "this symbol not expected in EmbeddedStatement"; break;
			case 200: s = "invalid EmbeddedStatement"; break;
			case 201: s = "invalid ForInitializer"; break;
			case 202: s = "invalid GotoStatement"; break;
			case 203: s = "invalid TryStatement"; break;
			case 204: s = "invalid ResourceAcquisition"; break;
			case 205: s = "invalid SwitchLabel"; break;
			case 206: s = "invalid CatchClauses"; break;
			case 207: s = "invalid UnaryExpr"; break;
			case 208: s = "invalid PrimaryExpr"; break;
			case 209: s = "invalid PrimaryExpr"; break;
			case 210: s = "invalid PrimaryExpr"; break;
			case 211: s = "invalid TypeArgumentList"; break;
			case 212: s = "invalid NewExpression"; break;
			case 213: s = "invalid NewExpression"; break;
			case 214: s = "invalid LambdaExpressionParameter"; break;
			case 215: s = "invalid LambdaExpressionBody"; break;
			case 216: s = "invalid RelationalExpr"; break;
			case 217: s = "invalid RelationalExpr"; break;
			case 218: s = "invalid TypeParameterConstraintsClauseBase"; break;
			case 219: s = "invalid QueryExpressionBody"; break;

			default: s = "error " + errorNumber; break;
		}
		this.Errors.Error(line, col, s);
	}
	
	private bool StartOf(int s)
	{
		return set[s, lexer.LookAhead.kind];
	}
	
	static bool[,] set = {
	{T,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,T,T,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,x,T,T, T,T,T,T, T,x,T,T, T,x,T,T, x,T,T,T, x,x,T,x, T,T,T,T, x,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, T,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,T,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,T,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,T,x,T, x,x,x,x, T,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,T, x,x,T,T, x,x,x,x, T,x,T,T, T,x,x,T, x,T,x,T, x,x,T,x, T,T,T,T, x,x,T,T, T,x,x,T, T,T,x,x, x,x,x,x, T,T,x,T, T,x,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,x,x, T,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,T,x, T,T,T,T, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,T, T,x,T,x, T,x,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,T,T, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,T,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,T, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,T,x,x, x,x,x,x, T,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{T,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,T,T,x, x,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,T,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,T,T,x, T,T,T,x, x,x,x,T, x,x,x,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, x,T,T,x, T,T,T,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,T, x,x,T,x, x,x,T,T, x,T,T,T, x,x,x,x, x,x,x,x, x,T,T,x, T,T,x,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,T,x, x,T,T,x, x,x,T,T, T,x,T,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,T,T, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, T,x,x,T, x,T,x,T, T,T,T,x, T,x,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,T,x,x, x,x,T,x, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, T,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x},
	{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,x,x}

	};
} // end Parser

}