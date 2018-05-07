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
using Altaxo.Text.Renderers.Maml.Extensions;
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
		private MarkdownDocument _markdownDocument;

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

		private List<Maml.MamlElement> _currentElementStack = new List<MamlElement>();

		private List<(string fileName, string guid, string title, int level, int spanStart)> _mamlFileList = new List<(string fileName, string guid, string title, int level, int spanStart)>();
		private int _indexOfMamlFile;

		public MamlRenderer() : base(TextWriter.Null)
		{
			// Default block renderers
			ObjectRenderers.Add(new CodeBlockRenderer());
			ObjectRenderers.Add(new ListRenderer());
			ObjectRenderers.Add(new HeadingRenderer());
			//ObjectRenderers.Add(new HtmlBlockRenderer());
			ObjectRenderers.Add(new ParagraphRenderer());
			ObjectRenderers.Add(new QuoteBlockRenderer());
			ObjectRenderers.Add(new ThematicBreakRenderer());

			// Default inline renderers
			ObjectRenderers.Add(new AutolinkInlineRenderer());
			ObjectRenderers.Add(new CodeInlineRenderer());
			ObjectRenderers.Add(new DelimiterInlineRenderer());
			ObjectRenderers.Add(new EmphasisInlineRenderer());
			ObjectRenderers.Add(new LineBreakInlineRenderer());
			//ObjectRenderers.Add(new HtmlInlineRenderer());
			ObjectRenderers.Add(new HtmlEntityInlineRenderer());
			ObjectRenderers.Add(new LinkInlineRenderer());
			ObjectRenderers.Add(new LiteralInlineRenderer());

			// Extension renderers
			ObjectRenderers.Add(new TableRenderer());
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

		public IDictionary<string, string> OldToNewImageUris { get; set; }

		#endregion Properties

		#region Maml content file handling

		/// <summary>
		/// For the given markdown document, this evaluates all .aml files that are neccessary to store the content.
		/// </summary>
		/// <param name="markdownDocument">The markdown document.</param>
		/// <exception cref="ArgumentException">First block of the markdown document should be a heading block!</exception>
		private void EvaluateMamlFileNames(MarkdownDocument markdownDocument)
		{
			_mamlFileList.Clear();
			_indexOfMamlFile = -1;

			if (markdownDocument[0] is HeadingBlock hbStart)
				AddMamlFile(hbStart);
			else
				throw new ArgumentException("The first block of the markdown document should be a heading block! Please add a header on top of your markdown document!");

			for (int i = 1; i < markdownDocument.Count; ++i)
			{
				if (markdownDocument[i] is HeadingBlock hb && hb.Level <= _splitLevel)
					AddMamlFile(hb);
			}
		}

		private void AddMamlFile(HeadingBlock headingBlock)
		{
			var fileName = _fullPathBaseFileName + _mamlFileList.Count.ToString() + ".aml";
			var guid = Guid.NewGuid();
			var title = ExtractTextContentFrom(headingBlock);
			_mamlFileList.Add((fileName, guid.ToString(), title, headingBlock.Level, headingBlock.Span.Start));
		}

		public void TryStartNewMamlFile(HeadingBlock headingBlock)
		{
			if (_indexOfMamlFile < 0 || (_indexOfMamlFile + 1 < _mamlFileList.Count && _mamlFileList[_indexOfMamlFile + 1].spanStart == headingBlock.Span.Start))
			{
				++_indexOfMamlFile;

				if (null != this.Writer)
				{
					CloseCurrentMamlFile();
				}

				var mamlFile = _mamlFileList[_indexOfMamlFile];

				var tw = new System.IO.StreamWriter(mamlFile.fileName, false, Encoding.UTF8, 1024);
				this.Writer = tw;

				Push(MamlElements.topic, new[] { new KeyValuePair<string, string>("id", mamlFile.guid), new KeyValuePair<string, string>("revisionNumber", "1") });
				Push(MamlElements.developerConceptualDocument, new[] { new KeyValuePair<string, string>("xmlns", "http://ddue.schemas.microsoft.com/authoring/2003/5"), new KeyValuePair<string, string>("xmlns:xlink", "http://www.w3.org/1999/xlink") });

				Push(MamlElements.introduction);

				if (_autoOutline)
					WriteLine("<autoOutline />");

				// TODO here insert introductory text

				PopTo(MamlElements.introduction);
			}
		}

		public void CloseCurrentMamlFile()
		{
			if (null != this.Writer)
			{
				PopAll();
				this.Writer.Close();
				this.Writer.Dispose();
				this.Writer = TextWriter.Null;
			}
		}

		#endregion Maml content file handling

		#region Maml image topic files handling

		/// <summary>
		/// Writes a file which contains all referenced images in native resolution (without using width and height attributes).
		/// To include this file helps ensure that all referenced images will be included into the help file.
		/// </summary>
		/// <returns>The guid of this .aml file.</returns>
		public Guid WriteImageTopicFile()
		{
			var fileName = _fullPathBaseFileName + "_Images.aml";
			var tw = new System.IO.StreamWriter(fileName, false, Encoding.UTF8, 1024);
			this.Writer = tw;

			var guid = Guid.NewGuid();
			Push(MamlElements.topic, new[] { new KeyValuePair<string, string>("id", guid.ToString()), new KeyValuePair<string, string>("revisionNumber", "1") });
			Push(MamlElements.developerConceptualDocument, new[] { new KeyValuePair<string, string>("xmlns", "http://ddue.schemas.microsoft.com/authoring/2003/5"), new KeyValuePair<string, string>("xmlns:xlink", "http://www.w3.org/1999/xlink") });
			Push(MamlElements.introduction);
			Write("This page contains all images used in this help file in native resolution. The ordering of the images is arbitrary.");
			PopTo(MamlElements.introduction);
			Push(MamlElements.section);
			Push(MamlElements.title);
			Write("Appendix: All images in native resolution");
			EnsureLine();
			PopTo(MamlElements.title);
			Push(MamlElements.content);

			// all links to all images here
			foreach (var entry in OldToNewImageUris)
			{
				var localUrl = System.IO.Path.GetFileNameWithoutExtension(entry.Value);

				Push(MamlElements.para);

				Push(MamlElements.mediaLinkInline);

				Push(MamlElements.image, new[] { new KeyValuePair<string, string>("xlink:href", localUrl) });

				PopTo(MamlElements.para);
			}

			PopAll();

			this.Writer.Close();
			this.Writer.Dispose();
			this.Writer = StreamWriter.Null;

			return guid;
		}

		#endregion Maml image topic files handling

		#region Content file creation

		public void WriteContentFile()
		{
			if (null != this.Writer)
			{
				CloseCurrentMamlFile();
			}

			var imageTopicFileGuid = WriteImageTopicFile();

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

			// Add image topic file at the very end
			WriteLine(string.Format("<Topic id=\"{0}\" visible=\"True\" title=\"{1}\">", imageTopicFileGuid, "Appendix: Images"));
			WriteLine("</Topic>");

			Write("</Topics>");

			this.Writer.Close();
			this.Writer.Dispose();
			this.Writer = StreamWriter.Null;
		}

		#endregion Content file creation

		/// <summary>
		/// Enumerates all objects in a markdown parse tree recursively, starting with the given element.
		/// </summary>
		/// <param name="startElement">The start element.</param>
		/// <returns>All text element (the given text element and all its childs).</returns>
		public static IEnumerable<Markdig.Syntax.MarkdownObject> EnumerateAllMarkdownObjectsRecursively(Markdig.Syntax.MarkdownObject startElement)
		{
			yield return startElement;
			var childList = GetChildList(startElement);
			if (null != childList)
			{
				foreach (var child in GetChildList(startElement))
				{
					foreach (var childAndSub in EnumerateAllMarkdownObjectsRecursively(child))
						yield return childAndSub;
				}
			}
		}

		/// <summary>
		/// Gets the childs of a markdown object. Null is returned if no childs were to be found.
		/// </summary>
		/// <param name="parent">The markdown object from which to get the childs.</param>
		/// <returns>The childs of the given markdown object, or null.</returns>
		public static IEnumerable<Markdig.Syntax.MarkdownObject> GetChilds(Markdig.Syntax.MarkdownObject parent)
		{
			if (parent is Markdig.Syntax.LeafBlock leafBlock)
				return leafBlock.Inline;
			else if (parent is Markdig.Syntax.Inlines.ContainerInline containerInline)
				return containerInline;
			else if (parent is Markdig.Syntax.ContainerBlock containerBlock)
				return containerBlock;
			else
				return null;
		}

		/// <summary>
		/// Gets the childs of a markdown object. Null is returned if no childs were to be found.
		/// </summary>
		/// <param name="parent">The markdown object from which to get the childs.</param>
		/// <returns>The childs of the given markdown object, or null.</returns>
		public static IReadOnlyList<Markdig.Syntax.MarkdownObject> GetChildList(Markdig.Syntax.MarkdownObject parent)
		{
			if (parent is Markdig.Syntax.LeafBlock leafBlock)
				return leafBlock.Inline?.ToArray<Markdig.Syntax.MarkdownObject>();
			else if (parent is Markdig.Syntax.Inlines.ContainerInline containerInline)
				return containerInline.ToArray<Markdig.Syntax.MarkdownObject>();
			else if (parent is Markdig.Syntax.ContainerBlock containerBlock)
				return containerBlock;
			else
				return null;
		}

		public (string fileGuid, string address) FindFragmentLink(string url)
		{
			if (url.StartsWith("#"))
				url = url.Substring(1);

			// for now, we have to go through the entire FlowDocument in search for a markdig tag that
			// (i) contains HtmlAttributes, and (ii) the HtmlAttibutes has the Id that is our url

			foreach (var mdo in EnumerateAllMarkdownObjectsRecursively(_markdownDocument))
			{
				var attr = (Markdig.Renderers.Html.HtmlAttributes)mdo.GetData(typeof(Markdig.Renderers.Html.HtmlAttributes));
				if (null != attr && attr.Id == url)
				{
					// markdown element found, now we need to know in which file it is
					var prevFile = _mamlFileList.First();
					foreach (var file in _mamlFileList.Skip(1))
					{
						if (file.spanStart > mdo.Span.End)
							break;
						prevFile = file;
					}

					return (prevFile.guid, url);
				}
			}

			return (null, null);
		}

		public override object Render(MarkdownObject markdownObject)
		{
			object result = null;

			if (null == markdownObject)
				throw new ArgumentNullException(nameof(markdownObject));

			if (markdownObject is MarkdownDocument markdownDocument)
			{
				_markdownDocument = markdownDocument;

				if (markdownDocument.Count == 0)
					return base.Render(markdownDocument);

				EvaluateMamlFileNames(markdownDocument);

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

		public void Push(Maml.MamlElement mamlElement, IEnumerable<KeyValuePair<string, string>> attributes)
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
		/// Writes the content escaped for Maml.
		/// </summary>
		/// <param name="slice">The slice.</param>
		/// <param name="softEscape">Only escape &lt; and &amp;</param>
		/// <returns>This instance</returns>
		public void WriteEscape(StringSlice slice, bool softEscape = false)
		{
			WriteEscape(ref slice, softEscape);
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
