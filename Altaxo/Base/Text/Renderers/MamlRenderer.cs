#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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

using Altaxo.Text.Renderers.Maml;
using Altaxo.Text.Renderers.Maml.Inlines;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Text.Renderers
{
	/// <summary>
	/// Renderer for a Markdown <see cref="MarkdownDocument"/> object that renders into one or multiple MAML files (MAML = Microsoft Assisted Markup Language).
	/// </summary>
	/// <seealso cref="TextRendererBase{T}" />
	public class MamlRenderer : TextRendererBase<MamlRenderer>
	{
		/// <summary>
		/// The header level where to split the output into different MAML files.
		/// 0 = render in only one file. 1 = Split at header level 1, 2 = split at header level 2, and so on.
		/// </summary>
		private int _splitLevel;

		/// <summary>
		/// If true, an outline of the content will be included at the top of every Maml file.
		/// </summary>
		private bool _autoOutline;

		/// <summary>
		/// The basic full path file name of a Maml file. To this name there will be appended (i) a number, and (ii) the extension ".maml".
		/// </summary>
		private string _fullPathBaseFileName;

		public bool EnableHtmlEscape { get; set; }

		private int _numberOfMamlFile;

		// operational members
		private Guid _currentTopicGuid;

		private List<Maml.MamlElement> _currentElementStack = new List<MamlElement>();

		private List<(string fileName, string guid, string title, int level, int spanStart)> _mamlFileList = new List<(string fileName, string guid, string title, int level, int spanStart)>();

		public MamlRenderer() : base(TextWriter.Null)
		{
			// Default block renderers
			ObjectRenderers.Add(new CodeBlockRenderer());
			ObjectRenderers.Add(new ListRenderer());
			ObjectRenderers.Add(new HeadingRenderer());
			//ObjectRenderers.Add(new HtmlBlockRenderer());
			ObjectRenderers.Add(new ParagraphRenderer());
			ObjectRenderers.Add(new QuoteBlockRenderer());
			//ObjectRenderers.Add(new ThematicBreakRenderer());

			// Default inline renderers
			//ObjectRenderers.Add(new AutolinkInlineRenderer());
			ObjectRenderers.Add(new CodeInlineRenderer());
			//ObjectRenderers.Add(new DelimiterInlineRenderer());
			ObjectRenderers.Add(new EmphasisInlineRenderer());
			//ObjectRenderers.Add(new LineBreakInlineRenderer());
			//ObjectRenderers.Add(new HtmlInlineRenderer());
			//ObjectRenderers.Add(new HtmlEntityInlineRenderer());
			//ObjectRenderers.Add(new LinkInlineRenderer());
			ObjectRenderers.Add(new LiteralInlineRenderer());
		}

		#region Properties

		public int SplitLevel
		{
			get
			{
				return _splitLevel;
			}
			set
			{
				_splitLevel = value;
			}
		}

		public bool AutoOutline
		{
			get
			{
				return _autoOutline;
			}
			set
			{
				_autoOutline = value;
			}
		}

		public string FullPathBaseFileName
		{
			get
			{
				return _fullPathBaseFileName;
			}
			set
			{
				_fullPathBaseFileName = value;
			}
		}

		#endregion Properties

		public void StartNewMamlFile(string title, int headingLevel, int spanStart)
		{
			if (null != this.Writer)
			{
				CloseCurrentMamlFile();
			}

			++_numberOfMamlFile;
			var fileName = _fullPathBaseFileName + _numberOfMamlFile.ToString() + ".aml";
			var tw = new System.IO.StreamWriter(fileName, false, Encoding.UTF8, 1024);
			this.Writer = tw;

			_currentTopicGuid = Guid.NewGuid();
			WriteLine(string.Format("<topic id=\"{0}\" revisionNumber=\"1\">", _currentTopicGuid));
			WriteLine("<developerConceptualDocument xmlns=\"http://ddue.schemas.microsoft.com/authoring/2003/5\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">");
			WriteLine("<introduction>");

			if (_autoOutline)
				WriteLine("<autoOutline />");

			// TODO here insert introductory text

			Write("</introduction>");

			_mamlFileList.Add((fileName, _currentTopicGuid.ToString(), title, headingLevel, spanStart));
		}

		public void CloseCurrentMamlFile()
		{
			if (null != this.Writer)
			{
				PopAll();

				WriteLine("</developerConceptualDocument>");
				WriteLine("</topic>");
				this.Writer.Close();
				this.Writer.Dispose();
				this.Writer = TextWriter.Null;
			}
		}

		public void WriteContentFile()
		{
			if (null != this.Writer)
			{
				CloseCurrentMamlFile();
			}

			if (0 == _mamlFileList.Count)
				return;

			var fileName = _fullPathBaseFileName + ".content";
			var tw = new System.IO.StreamWriter(fileName, false, Encoding.UTF8, 1024);
			this.Writer = tw;

			WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			WriteLine("<Topics>");

			int startingHeadingLevel = _mamlFileList[0].level;
			int previousHeadingLevel = startingHeadingLevel - 1;
			for (int i = 0; i < _mamlFileList.Count; ++i)
			{
				for (int j = previousHeadingLevel; j >= _mamlFileList[i].level; --j)
					WriteLine("</Topic>");

				if (i == 0)
					WriteLine(string.Format("<Topic id=\"{0}\" visible=\"True\" title=\"{1}\" isDefault=\"true\" isExpanded=\"true\" isSelected=\"true\">", _mamlFileList[i].guid, _mamlFileList[i].title));
				else
					WriteLine(string.Format("<Topic id=\"{0}\" visible=\"True\" title=\"{1}\">", _mamlFileList[i].guid, _mamlFileList[i].title));

				if ((_mamlFileList[i].level - previousHeadingLevel) > 0) // For sublevels increase the heading level at maximum by one
					++previousHeadingLevel; // because jumping for instance from 1 to 3, i.e. jumping more than one level is not supported
				else
					previousHeadingLevel = _mamlFileList[i].level;
			}

			// Close all open topic tags
			for (int j = previousHeadingLevel; j >= startingHeadingLevel; --j)
				WriteLine("</Topic>");

			Write("</Topics>");

			this.Writer.Close();
			this.Writer.Dispose();
			this.Writer = StreamWriter.Null;
		}

		public override object Render(MarkdownObject markdownObject)
		{
			object result = null;

			if (markdownObject is MarkdownDocument markdownDocument)
			{
				if (markdownDocument.Count == 0)
					return base.Render(markdownDocument);

				if (!(markdownDocument[0] is HeadingBlock))
				{
					// if the first block of the markdown document starts with something but an header, we have to start a section manually
				}

				base.Render(markdownObject);
			}
			else
			{
				result = base.Render(markdownObject);
			}

			// At the end, write the content file

			WriteContentFile();

			return result;
		}

		public void Push(Maml.MamlElement mamlElement)
		{
			Push(mamlElement, null);
		}

		public void Push(Maml.MamlElement mamlElement, Dictionary<string, string> attributes)
		{
			_currentElementStack.Add(mamlElement);

			if (!mamlElement.IsInlineElement)
				WriteLine();

			Write("<");
			Write(mamlElement.Name);

			if (null != attributes)
			{
				foreach (var att in attributes)
				{
					Write(" ");
					Write(att.Key);
					Write("=\"");
					Write(att.Value);
					Write("\"");
				}
			}

			Write(">");

			if (!mamlElement.IsInlineElement)
				WriteLine();
		}

		public Maml.MamlElement Pop()
		{
			if (_currentElementStack.Count <= 0)
				throw new InvalidOperationException("Pop from an empty stack");

			var ele = _currentElementStack[_currentElementStack.Count - 1];
			_currentElementStack.RemoveAt(_currentElementStack.Count - 1);

			Write("</");
			Write(ele.Name);
			Write(">");

			if (!ele.IsInlineElement)
				WriteLine();

			return ele;
		}

		public void PopAll()
		{
			while (_currentElementStack.Count > 0)
				Pop();
		}

		public void PopTo(Maml.MamlElement mamlElement)
		{
			Maml.MamlElement ele = null;
			while (_currentElementStack.Count > 0)
			{
				ele = Pop();
				if (ele == mamlElement)
					break;
			}

			if (ele != mamlElement)
				throw new InvalidOperationException("Could not pop to Maml element " + mamlElement.Name);
		}

		public void PopToBefore(Maml.MamlElement mamlElement)
		{
			while (_currentElementStack.Count > 0)
			{
				if (_currentElementStack[_currentElementStack.Count - 1] == mamlElement)
					break;

				Pop();
			}

			if (_currentElementStack.Count == 0)
				throw new InvalidOperationException("Could not pop to before element " + mamlElement.Name);
		}

		public bool ElementStackContains(Maml.MamlElement mamlElement)
		{
			return _currentElementStack.Contains(mamlElement);
		}

		public int NumberOfElementsOnStack(Maml.MamlElement mamlElement)
		{
			int result = 0;
			for (int i = _currentElementStack.Count - 1; i >= 0; --i)
				if (_currentElementStack[i] == mamlElement)
					++result;

			return result;
		}

		/// <summary>
		/// Writes the content escaped for XAML.
		/// </summary>
		/// <param name="slice">The slice.</param>
		/// <param name="softEscape">Only escape &lt; and &amp;</param>
		/// <returns>This instance</returns>
		public void WriteEscape(ref StringSlice slice, bool softEscape = false)
		{
			if (slice.Start > slice.End)
			{
				return;
			}
			WriteEscape(slice.Text, slice.Start, slice.Length, softEscape);
		}

		/// <summary>
		/// Writes the content escaped for Maml.
		/// </summary>
		/// <param name="content">The content.</param>
		public void WriteEscape(string content)
		{
			if (!string.IsNullOrEmpty(content))
				WriteEscape(content, 0, content.Length);
		}

		/// <summary>
		/// Writes the content escaped for Maml.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="length">The length.</param>
		/// <param name="softEscape">Only escape &lt; and &amp;</param>
		public void WriteEscape(string content, int offset, int length, bool softEscape = false)
		{
			if (string.IsNullOrEmpty(content) || length == 0)
				return;

			var end = offset + length;
			var previousOffset = offset;
			for (; offset < end; offset++)
			{
				switch (content[offset])
				{
					case '<':
						Write(content, previousOffset, offset - previousOffset);
						if (EnableHtmlEscape)
						{
							Write("&lt;");
						}
						previousOffset = offset + 1;
						break;

					case '>':
						if (!softEscape)
						{
							Write(content, previousOffset, offset - previousOffset);
							if (EnableHtmlEscape)
							{
								Write("&gt;");
							}
							previousOffset = offset + 1;
						}
						break;

					case '&':
						Write(content, previousOffset, offset - previousOffset);
						if (EnableHtmlEscape)
						{
							Write("&amp;");
						}
						previousOffset = offset + 1;
						break;

					case '"':
						if (!softEscape)
						{
							Write(content, previousOffset, offset - previousOffset);
							if (EnableHtmlEscape)
							{
								Write("&quot;");
							}
							previousOffset = offset + 1;
						}
						break;
				}
			}

			Write(content, previousOffset, end - previousOffset);
		}

		/// <summary>
		/// Writes the lines of a <see cref="LeafBlock"/>
		/// </summary>
		/// <param name="leafBlock">The leaf block.</param>
		/// <param name="writeEndOfLines">if set to <c>true</c> write end of lines.</param>
		/// <param name="escape">if set to <c>true</c> escape the content for XAML</param>
		/// <param name="softEscape">Only escape &lt; and &amp;</param>
		/// <returns>This instance</returns>
		public void WriteLeafRawLines(LeafBlock leafBlock, bool writeEndOfLines, bool escape, bool softEscape = false)
		{
			if (leafBlock == null)
				throw new ArgumentNullException(nameof(leafBlock));

			if (leafBlock.Lines.Lines != null)
			{
				var lines = leafBlock.Lines;
				var slices = lines.Lines;
				for (var i = 0; i < lines.Count; i++)
				{
					if (!writeEndOfLines && i > 0)
					{
						WriteLine();
					}
					if (escape)
					{
						WriteEscape(ref slices[i].Slice, softEscape);
					}
					else
					{
						Write(ref slices[i].Slice);
					}
					if (writeEndOfLines)
					{
						WriteLine();
					}
				}
			}
		}

		public string ExtractTextContentFrom(LeafBlock leafBlock)
		{
			var result = string.Empty;

			if (null == leafBlock.Inline)
				return result;

			foreach (var il in leafBlock.Inline)
			{
				result += il.ToString();
			}

			return result;
		}
	}
}
