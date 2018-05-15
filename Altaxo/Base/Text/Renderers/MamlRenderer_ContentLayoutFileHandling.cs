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

using Markdig.Renderers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Altaxo.Text.Renderers
{
	public partial class MamlRenderer : TextRendererBase<MamlRenderer>
	{
		public static string GetContentLayoutFileName(string userChosenfileName)
		{
			if (Path.GetExtension(userChosenfileName).ToLowerInvariant() == ".content")
			{
				return userChosenfileName;
			}
			else if (Path.GetExtension(userChosenfileName).ToLowerInvariant() == ".shfbproj")
			{
				var contentFileName = ExtractContentLayoutFileNameFromShfbproj(userChosenfileName);
				if (!string.IsNullOrEmpty(contentFileName))
					return contentFileName;
			}
			return Path.Combine(Path.GetDirectoryName(userChosenfileName), Path.GetFileNameWithoutExtension(userChosenfileName) + ".content");
		}

		public void WriteContentLayoutFile()
		{
			if (null != this.Writer)
			{
				CloseCurrentMamlFile();
			}

			var (imageTopicFileName, imageTopicFileGuid) = WriteImageTopicFile();
			// Include image topic file at the end of the maml file list with level = 0
			_amlFileList.Add((imageTopicFileName, imageTopicFileGuid.ToString(), "Appendix: Images", 1, 0));

			if (0 == _amlFileList.Count)
				return;

			var tw = new System.IO.StreamWriter(ContentLayoutFileName, false, Encoding.UTF8, 1024);
			this.Writer = tw;

			WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			WriteLine("<Topics>");

			int currentHeadingLevel = 1;
			for (int i = 0; i < _amlFileList.Count; ++i)
			{
				for (; currentHeadingLevel > _amlFileList[i].level; --currentHeadingLevel)
				{
					WriteLine("</Topic>");
				}

				if (i == 0)
					WriteLine(string.Format("<Topic id=\"{0}\" visible=\"True\" title=\"{1}\" isDefault=\"true\" isExpanded=\"true\" isSelected=\"true\">", _amlFileList[i].guid, _amlFileList[i].title));
				else
					WriteLine(string.Format("<Topic id=\"{0}\" visible=\"True\" title=\"{1}\">", _amlFileList[i].guid, _amlFileList[i].title));

				++currentHeadingLevel;
			}

			// Close all open topic tags
			for (; currentHeadingLevel > 1; --currentHeadingLevel)
				WriteLine("</Topic>");

			// Add image topic file at the very end
			//WriteLine(string.Format("<Topic id=\"{0}\" visible=\"True\" title=\"{1}\">", imageTopicFileGuid, "Appendix: Images"));
			//WriteLine("</Topic>");

			Write("</Topics>");

			this.Writer.Close();
			this.Writer.Dispose();
			this.Writer = StreamWriter.Null;
		}
	}
}
