// project created on 09.08.2003 at 10:16
using System;
using System.Collections.Specialized;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

using ICSharpCode.SharpRefactory.PrettyPrinter;
using ICSharpCode.SharpRefactory.Parser;

class MainClass
{
	public static StringCollection SearchDirectory(string directory, string filemask)
	{
		return SearchDirectory(directory, filemask, true);
	}
	
	public static StringCollection SearchDirectory(string directory, string filemask, bool searchSubdirectories)
	{
		StringCollection collection = new StringCollection();
		SearchDirectory(directory, filemask, collection, searchSubdirectories);
		return collection;
	}
	
	/// <summary>
	/// Finds all files which are valid to the mask <code>filemask</code> in the path
	/// <code>directory</code> and all subdirectories (if searchSubdirectories
	/// is true. The found files are added to the StringCollection 
	/// <code>collection</code>.
	/// </summary>
	static void SearchDirectory(string directory, string filemask, StringCollection collection, bool searchSubdirectories)
	{
		try {
			string[] file = Directory.GetFiles(directory, filemask);
			foreach (string f in file) {
				collection.Add(f);
			}
			
			if (searchSubdirectories) {
				string[] dir = Directory.GetDirectories(directory);
				foreach (string d in dir) {
					SearchDirectory(d, filemask, collection, searchSubdirectories);
				}
			}
		} catch (Exception) {
		}
	}
	
	static void PrettyPrintDirectories()
	{
		StringCollection files = SearchDirectory("C:\\b", "*.cs");
		foreach (string fileName in files) {
			Parser p = new Parser();
			Console.Write("Converting : " + fileName);
			p.Parse(new Lexer(new FileReader(fileName)));
			if (p.Errors.count == 0) {
				StreamReader sr = File.OpenText(fileName);
				string content = sr.ReadToEnd();
				sr.Close();
				PrettyPrintVisitor ppv = new PrettyPrintVisitor(content);
				ppv.Visit(p.compilationUnit, null);
				
				StreamWriter sw = new StreamWriter(fileName);
				sw.Write(ppv.Text);
				sw.Close();
				
				Console.WriteLine(" done.");
			} else {
				Console.Write(" Source code errors:");
				Console.WriteLine(p.Errors.ErrorOutput);
			}
		}
		Console.ReadLine();
	}

	public static void Main (string[] args)
	{
//		PrettyPrintDirectories();
		Parser p = new Parser();
		string fileName = "C:\\a.cs";
		Console.Write("Converting : " + fileName);
		p.Parse(new Lexer(new FileReader(fileName)));
		if (p.Errors.count == 0) {
			StreamReader sr = File.OpenText(fileName);
			string content = sr.ReadToEnd();
			sr.Close();
			PrettyPrintVisitor ppv = new PrettyPrintVisitor(content);
			ppv.PrettyPrintOptions.IndentSize = 6;
			ppv.Visit(p.compilationUnit, null);
			
			Console.WriteLine(ppv.Text);
			
			Console.WriteLine(" done.");
		} else {
			Console.Write(" Source code errors:");
			Console.WriteLine(p.Errors.ErrorOutput);
		}
	}
}
