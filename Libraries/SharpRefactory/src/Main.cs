// project created on 09.08.2003 at 10:16
using System;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

using ICSharpCode.SharpRefactory.PrettyPrinter;
using ICSharpCode.SharpRefactory.Parser;


class MainClass
{
//	static void A()
//	{
//		Lexer lexer = new Lexer(new ICSharpCode.SharpRefactory.Lexer.StringReader("(int)i"));
//		for (int i = 0; i < 10; ++i) {
//			Console.WriteLine(i + " ----> " + lexer.Peek(i).kind);
//		}
//		lexer.NextToken();
//		Console.WriteLine("1." + lexer.LookAhead.kind + " -- " + Tokens.OpenParenthesis);
//		lexer.NextToken();
//		Console.WriteLine("2." + lexer.LookAhead.kind + " -- " + Tokens.Int);
//		lexer.NextToken();
//		Console.WriteLine("3." + lexer.LookAhead.kind + " -- " + Tokens.CloseParenthesis);
//		lexer.NextToken();
//		Console.WriteLine("4." + lexer.LookAhead.kind + " -- " + Tokens.Identifier);
//		lexer.NextToken();
//		Console.WriteLine("5." + lexer.LookAhead.kind + " -- " + Tokens.EOF);
//	}
	
	public static void Main (string[] args)
	{
//		Lexer lexer = new Lexer(new FileReader("C:\\test.cs"));
//		Token t = lexer.NextToken();
//		while (t != null && t.kind != Tokens.EOF) {
//			Console.WriteLine("Token : {0}, value={1}, literalValue={2}", t.kind, t.val, t.literalValue);
//			t = lexer.NextToken();
//		}
//		if (t != null) {
//			Console.WriteLine("Token : {0}, value={1}, literalValue={2}", t.kind, t.val, t.literalValue);
//		} else {
//			Console.WriteLine("NULL");
//		}
		
		Parser p = new Parser();
		p.Parse(new Lexer(new FileReader("C:\\Main.cs")));
		if (p.Errors.count == 0) {
			LookupTableVisitor lookupTableVisitor = new LookupTableVisitor();
			lookupTableVisitor.Visit(p.compilationUnit, null);
			
//			new DebugASTVisitor().Visit(p.compilationUnit, null);
//			PrettyPrintVisitor ppv = new PrettyPrintVisitor();
//			ppv.Visit(p.compilationUnit, null);
//			Console.WriteLine(ppv.Text.ToString());
		} else {
			Console.WriteLine("Source code errors:");
			Console.WriteLine(p.Errors.ErrorOutput);
		}
			
//		
//		Scanner.Init(fileName);
//		Console.WriteLine("Parsing source file {0}", fileName);
//		Parser.Parse();
//		
		
//			
//			CodeDOMVisitor cdom = new CodeDOMVisitor();
//			
//			cdom.Visit(Parser.compilationUnit, null);
//			
//			Microsoft.CSharp.CSharpCodeProvider provider = new CSharpCodeProvider();
//			// Call the CodeDomProvider.CreateGenerator() method to obtain an ICodeGenerator from the provider.
//			System.CodeDom.Compiler.ICodeGenerator generator = provider.CreateGenerator();
//			generator.GenerateCodeFromCompileUnit(cdom.codeCompileUnit, Console.Out, null);
//			
//			VBNetVisitor vbv = new VBNetVisitor();
//			
//			vbv.Visit(Parser.compilationUnit, null);
//			StreamWriter sw = new StreamWriter(@"C:\testform.vb");
//			sw.Write(vbv.SourceText.ToString());
//			sw.Close();
//			Console.WriteLine("converted.");
	}
}
