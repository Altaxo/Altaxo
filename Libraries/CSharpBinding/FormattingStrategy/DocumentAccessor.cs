using System;
using System.Collections;
using System.IO;

using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace CSharpBinding.FormattingStrategy
{
	public interface IDocumentAccessor
	{
		bool Dirty { get; }
		int LineNumber { get; }
		string Text { get; set; }
		bool Next();
	}
	
	#region DocumentAccessor
	public class DocumentAccessor : IDocumentAccessor
	{
		IDocument doc;
		
		int minLine;
		int maxLine;
		int changedLines = 0;
		
		public DocumentAccessor(IDocument document)
		{
			doc = document;
			this.minLine = 0;
			this.maxLine = doc.TotalNumberOfLines - 1;
		}
		
		public DocumentAccessor(IDocument document, int minLine, int maxLine)
		{
			doc = document;
			this.minLine = minLine;
			this.maxLine = maxLine;
		}
		
		int num = -1;
		bool dirty;
		string text;
		LineSegment line;
		
		public bool Dirty {
			get {
				return dirty;
			}
		}
		
		public int LineNumber {
			get {
				return num;
			}
		}
		
		public int ChangedLines {
			get {
				return changedLines;
			}
		}
		
		bool lineDirty = false;
		
		public string Text {
			get { return text; }
			set {
				if (num < minLine) return;
				text = value;
				dirty = true;
				lineDirty = true;
			}
		}
		
		public bool Next()
		{
			if (lineDirty) {
				doc.Replace(line.Offset, line.Length, text);
				lineDirty = false;
				++changedLines;
			}
			++num;
			if (num > maxLine) return false;
			line = doc.GetLineSegment(num);
			text = doc.GetText(line);
			return true;
		}
	}
	#endregion
	
	#region FileAccessor
	public class FileAccessor : IDisposable, IDocumentAccessor
	{
		public bool Dirty {
			get {
				return dirty;
			}
		}
		
		FileStream f;
		StreamReader r;
		ArrayList lines = new ArrayList();
		bool dirty = false;
		
		System.Text.Encoding encoding;
		
		string filename;
		public FileAccessor(string filename)
		{
			this.filename = filename;
			f = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None);
			// TODO: Auto-detect encoding
			encoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
			r = new StreamReader(f, encoding);
		}
		
		int num = 0;
		
		public int LineNumber {
			get { return num; }
		}
		
		
		string text = "";
		
		public static bool Verbose = false;
		
		public string Text {
			get {
				return text;
			}
			set {
				if (Verbose) {
					Console.WriteLine("old: " + text.Replace("\t", "--->"));
					Console.WriteLine("new: " + value.Replace("\t", "--->"));
				}
				dirty = true;
				text = value;
			}
		}
		
		public bool Next()
		{
			//Console.WriteLine(text.Replace("\t", "--->"));
			if (num > 0) {
				lines.Add(text);
			}
			text = r.ReadLine();
			++num;
			return text != null;
		}
		
		void IDisposable.Dispose()
		{
			Close();
		}
		
		public void Close()
		{
			r.Close();
			f.Close();
			if (dirty) {
				f = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
				using (StreamWriter w = new StreamWriter(f, encoding)) {
					foreach (string line in lines) {
						w.WriteLine(line);
					}
				}
				f.Close();
			}
		}
	}
	#endregion
}
