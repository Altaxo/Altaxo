// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Drawing;
using System.Collections;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Internal.Project;
using SharpDevelop.Internal.Parser;
using CSharpBinding.Parser.SharpDevelopTree;
using ICSharpCode.SharpRefactory.Parser;

namespace CSharpBinding.Parser
{
	public class TParser : IParser
	{
		///<summary>IParser Interface</summary> 
		string[] lexerTags;
		public string[] LexerTags {
//// Alex: get accessor
			get {
				return lexerTags;
			}
			set {
				lexerTags = value;
			}
		}
		public IExpressionFinder ExpressionFinder {
			get {
				return new ExpressionFinder();
			}
		}
		public bool CanParse(string fileName)
		{
			return System.IO.Path.GetExtension(fileName).ToUpper() == ".CS";
		}
		public bool CanParse(IProject project)
		{
			return project.ProjectType == "C#";
		}
		
		void RetrieveRegions(CompilationUnit cu, SpecialTracker tracker)
		{
			for (int i = 0; i < tracker.CurrentSpecials.Count; ++i) {
				PreProcessingDirective directive = tracker.CurrentSpecials[i] as PreProcessingDirective;
				if (directive != null) {
					if (directive.Cmd == "#region") {
						int deep = 1; 
						for (int j = i + 1; j < tracker.CurrentSpecials.Count; ++j) {
							PreProcessingDirective nextDirective = tracker.CurrentSpecials[j] as PreProcessingDirective;
							if (nextDirective != null) {
								switch (nextDirective.Cmd) {
									case "#region":
										++deep;
										break;
									case "#endregion":
										--deep;
										if (deep == 0) {
											cu.FoldingRegions.Add(new FoldingRegion(directive.Arg.Trim(), new DefaultRegion(directive.Start, new Point(nextDirective.End.X, nextDirective.End.Y))));
											goto end;
										}
										break;
								}
							}
						}
						end: ;
					}
				}
			}
		}
		
		public ICompilationUnitBase Parse(string fileName)
		{
			ICSharpCode.SharpRefactory.Parser.Parser p = new ICSharpCode.SharpRefactory.Parser.Parser();
			
			Lexer lexer = new Lexer(new FileReader(fileName));
			lexer.SpecialCommentTags = lexerTags;
			p.Parse(lexer);
			
			CSharpVisitor visitor = new CSharpVisitor();
			visitor.Visit(p.compilationUnit, null);
			visitor.Cu.FileName = fileName;
			visitor.Cu.ErrorsDuringCompile = p.Errors.count > 0;
			RetrieveRegions(visitor.Cu, lexer.SpecialTracker);
			AddCommentTags(visitor.Cu, lexer.TagComments);
			return visitor.Cu;
		}
		
		public ICompilationUnitBase Parse(string fileName, string fileContent)
		{
			ICSharpCode.SharpRefactory.Parser.Parser p = new ICSharpCode.SharpRefactory.Parser.Parser();
			
			Lexer lexer = new Lexer(new StringReader(fileContent));
			lexer.SpecialCommentTags = lexerTags;
			p.Parse(lexer);
			
			CSharpVisitor visitor = new CSharpVisitor();
			visitor.Visit(p.compilationUnit, null);
			visitor.Cu.FileName = fileName;
			visitor.Cu.ErrorsDuringCompile = p.Errors.count > 0;
			visitor.Cu.Tag = p.compilationUnit;
			RetrieveRegions(visitor.Cu, lexer.SpecialTracker);
			AddCommentTags(visitor.Cu, lexer.TagComments);
			return visitor.Cu;
		}
		
		void AddCommentTags(ICompilationUnit cu, ArrayList tagComments)
		{
			foreach (ICSharpCode.SharpRefactory.Parser.TagComment tagComment in tagComments) {
				DefaultRegion tagRegion = new DefaultRegion(tagComment.StartPosition.Y, tagComment.StartPosition.X);
				SharpDevelop.Internal.Parser.Tag tag = new SharpDevelop.Internal.Parser.Tag(tagComment.Tag, tagRegion);
				tag.CommentString = tagComment.CommentText;
				cu.TagComments.Add(tag);
			}
		}
		
		
		public ArrayList CtrlSpace(IParserService parserService, int caretLine, int caretColumn, string fileName)
		{
			ArrayList result = new Resolver().CtrlSpace(parserService, caretLine, caretColumn, fileName);
			return result;
		}
		
		public ResolveResult Resolve(IParserService parserService, string expression, int caretLineNumber, int caretColumn, string fileName, string fileContent)
		{
			return new Resolver().Resolve(parserService, expression, caretLineNumber, caretColumn, fileName, fileContent);
		}
		
		///////// IParser Interface END
	}
}
