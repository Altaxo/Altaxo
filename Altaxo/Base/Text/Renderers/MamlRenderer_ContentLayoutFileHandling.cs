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

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Markdig.Renderers;

namespace Altaxo.Text.Renderers
{
  public partial class MamlRenderer : TextRendererBase<MamlRenderer>
  {
    /// <summary>
    /// Gets the name of the content layout file, depending on the file name the user has chosen to be the output file.
    /// If the user has chosen a .content file, then this name is returned. If the user has chosen a SHFBPROJ file, then the name of the .content
    /// file is extracted from this project file. If all this fails, the name of the content layout file is synthezized from the user chosen file name.
    /// </summary>
    /// <returns>The full name of the content layout file.</returns>
    public string GetContentLayoutFileName()
    {
      if (Path.GetExtension(ProjectOrContentFileName).ToLowerInvariant() == ".content")
      {
        return ProjectOrContentFileName;
      }
      else if (Path.GetExtension(ProjectOrContentFileName).ToLowerInvariant() == ".shfbproj")
      {
        var contentFileName = ExtractContentLayoutFileNameFromShfbproj(ProjectOrContentFileName);
        if (!string.IsNullOrEmpty(contentFileName))
          return contentFileName;
      }
      if (!string.IsNullOrEmpty(ContentFileNameBase))
        return Path.Combine(BasePathName, ContentFolderName, ContentFileNameBase + ".content");
      else
        return Path.Combine(BasePathName, Path.GetFileNameWithoutExtension(ProjectOrContentFileName) + ".content");
    }

    /// <summary>
    /// Writes the content layout file, i.e. the sections as indicated by the heading levels are written in a XML tree structure to be read
    /// by Sandcastle help file builder.
    /// </summary>
    public void WriteContentLayoutFile()
    {
      if (Writer is not null)
      {
        CloseCurrentMamlFile();
      }

      var (imageTopicFileName, imageTopicFileGuid) = WriteImageTopicFile(); // writes the image topic file to disc

      // Note the following convention:
      // If there are multiple files with a Level 1 header level, then we add the image topic file as Level 1 file, too
      // But if there is only one Level 1 header level file, then we add the image topic file as Level 2 file, so that is becomes a sub-node of the first level header topic
      var minimumLevel = _amlFileList.Min(x => x.level);
      var numberOfMinimumLevelTopics = _amlFileList.Count(x => x.level == minimumLevel);
      // Include image topic file at the end of the maml file list with level = 0
      _amlFileList.Add((imageTopicFileName, imageTopicFileGuid, "Appendix: Images", numberOfMinimumLevelTopics == 1 ? minimumLevel + 1 : minimumLevel, 0));

      if (0 == _amlFileList.Count)
        return;

      var tw = new System.IO.StreamWriter(ContentLayoutFileName, false, Encoding.UTF8, 1024);
      Writer = tw;

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

      Writer.Close();
      Writer.Dispose();
      Writer = StreamWriter.Null;
    }
  }
}
