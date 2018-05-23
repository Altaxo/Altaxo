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

using Altaxo.Collections;
using Altaxo.Text.Renderers.Maml;
using Markdig.Renderers;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Text.Renderers
{
	public partial class MamlRenderer : TextRendererBase<MamlRenderer>
	{
		#region Maml topic file handling

		/// <summary>
		/// For the given markdown document, this evaluates all .aml files that are neccessary to store the content.
		/// </summary>
		/// <param name="markdownDocument">The markdown document.</param>
		/// <exception cref="ArgumentException">First block of the markdown document should be a heading block!</exception>
		private void EvaluateMamlFileNames(MarkdownDocument markdownDocument)
		{
			_amlFileList.Clear();
			_indexOfAmlFile = -1;

			// the header titles, entry 0 is the current title for level1, entry [1] is the current title for level 2 and so on
			List<string> headerTitles = new List<string>();

			if (markdownDocument[0] is HeadingBlock hbStart)
				AddMamlFile(hbStart, headerTitles);
			else
				throw new ArgumentException("The first block of the markdown document should be a heading block! Please add a header on top of your markdown document!");

			for (int i = 1; i < markdownDocument.Count; ++i)
			{
				if (markdownDocument[i] is HeadingBlock hb && hb.Level <= SplitLevel)
					AddMamlFile(hb, headerTitles);
			}
		}

		private void AddMamlFile(HeadingBlock headingBlock, List<string> headerTitles)
		{
			var fileName = string.Format("{0}{1:D6}.aml", AmlBaseFileName, _amlFileList.Count);
			var title = ExtractTextContentFrom(headingBlock);
			var levelM1 = headingBlock.Level - 1;

			// List of header titles from level 1 to ... (in order to get Guid)
			for (int i = headerTitles.Count - 1; i >= 0; --i)
				headerTitles.RemoveAt(i);
			headerTitles.Add(title);

			var guid = CreateGuidFromHeaderTitles(headerTitles);
			_amlFileList.Add((fileName, guid, title, headingBlock.Level, headingBlock.Span.Start));
		}

		private string CreateGuidFromHeaderTitles(List<string> headerTitles)
		{
			var stb = new System.Text.StringBuilder();

			for (int i = 0; i < headerTitles.Count; ++i)
			{
				if (i != 0)
					stb.Append(" - ");
				stb.Append(headerTitles[i]);
			}

			byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(stb.ToString());

			byte[] hash = _md5Hasher.ComputeHash(inputBytes);

			// step 2, convert byte array to hex string

			stb.Length = 0;

			for (int i = 0; i < hash.Length; i++)
			{
				stb.Append(hash[i].ToString("X2"));
			}

			return stb.ToString();
		}

		/// <summary>
		/// Try to start a new maml file.
		/// </summary>
		/// <param name="headingBlock">The heading block that might trigger the start of a new maml file..</param>
		/// <returns>True if a new Maml file was started; otherwise, false.</returns>
		public bool TryStartNewMamlFile(HeadingBlock headingBlock)
		{
			if (_indexOfAmlFile < 0 || (_indexOfAmlFile + 1 < _amlFileList.Count && _amlFileList[_indexOfAmlFile + 1].spanStart == headingBlock.Span.Start))
			{
				if (null != this.Writer)
				{
					CloseCurrentMamlFile();
				}

				++_indexOfAmlFile;

				var mamlFile = _amlFileList[_indexOfAmlFile];

				System.IO.Directory.CreateDirectory(Path.GetDirectoryName(mamlFile.fileName));
				var tw = new System.IO.StreamWriter(mamlFile.fileName, false, Encoding.UTF8, 1024);
				this.Writer = tw;

				Push(MamlElements.topic, new[] { new KeyValuePair<string, string>("id", mamlFile.guid), new KeyValuePair<string, string>("revisionNumber", "1") });
				Push(MamlElements.developerConceptualDocument, new[] { new KeyValuePair<string, string>("xmlns", "http://ddue.schemas.microsoft.com/authoring/2003/5"), new KeyValuePair<string, string>("xmlns:xlink", "http://www.w3.org/1999/xlink") });

				Push(MamlElements.introduction);

				bool hasLinkOrOutlineElement = false;
				if (EnableLinkToPreviousSection && _indexOfAmlFile > 0)
				{
					Push(MamlElements.para);
					Write(LinkToPreviousSectionLabelText);
					var prevTopic = _amlFileList[_indexOfAmlFile - 1];
					Push(MamlElements.link, new[] { new KeyValuePair<string, string>("xlink:href", prevTopic.guid) });
					Write(prevTopic.title);
					PopTo(MamlElements.link);

					PopTo(MamlElements.para);

					hasLinkOrOutlineElement = true;
				}

				if (AutoOutline)
				{
					WriteLine("<autoOutline />");
					hasLinkOrOutlineElement = true;
				}

				if(hasLinkOrOutlineElement)
				{
					Push(MamlElements.markup);
					Write("<hr/>");
					PopTo(MamlElements.markup);
				}

				return true;
			}
			else
			{
				return false;
			}
		}

		public void CloseCurrentMamlFile()
		{
			if (null != this.Writer && _currentElementStack.Count > 0)
			{
				if (EnableLinkToNextSection && (_indexOfAmlFile + 1) < _amlFileList.Count )
				{
					if(NumberOfElementsOnStack(MamlElements.introduction)>0)
					{
						// then we are still in the introduction, thus we can place the elements here
						Push(MamlElements.markup);  // Nasty trick: put a horizontal line outside of the section in order to go from the very left to the very right
						Write("<hr/>");             // If this trick won't work anymore, put the hr element inside the content as outcommented below
						PopTo(MamlElements.markup);

					}
					else
					{
						// Pop all content elements except one
						PopToBefore(MamlElements.developerConceptualDocument);

						Push(MamlElements.markup);  // Nasty trick: put a horizontal line outside of the section in order to go from the very left to the very right
						Write("<hr/>");             // If this trick won't work anymore, put the hr element inside the content as outcommented below
						PopTo(MamlElements.markup);

						Push(MamlElements.section);
						Push(MamlElements.content);
					}

					
					// now insert a link to the next section

					// Push(MamlElements.markup); // this is the normal place to put the horizontal line element
					// Write("<hr/>");
					// PopTo(MamlElements.markup);

					Push(MamlElements.para);
					Write(LinkToNextSectionLabelText);
					var nextTopic = _amlFileList[_indexOfAmlFile + 1];
					Push(MamlElements.link, new[] { new KeyValuePair<string, string>("xlink:href", nextTopic.guid) });
					Write(nextTopic.title);
					PopTo(MamlElements.link);
					PopTo(MamlElements.para);
				}

				PopAll();

				this.Writer.Close();
				this.Writer.Dispose();
				this.Writer = TextWriter.Null;
			}
		}

		#endregion Maml topic file handling

		/// <summary>
		/// Removes the old image files. This function will work only if there is a dedicated image folder, i.e. <see cref="ImageFileNames"/> has a value.
		/// </summary>
		public static void RemoveOldContentsOfContentFolder(string fullContentFolderName)
		{
			var dir = new DirectoryInfo(fullContentFolderName);
			if (!dir.Exists)
				return;

			var filesToDelete = new HashSet<string>();
			foreach (var extension in new string[] { ".aml", ".content" })
			{
				filesToDelete.AddRange(dir.GetFiles("*" + extension).Select(x => x.FullName));
			}

			// now delete the files
			foreach (var file in filesToDelete)
				File.Delete(file);
		}
	}
}
