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
    /// Extracts the file name of the content layout file (extension: .content)  from the Sandcastle help file builder project.
    /// </summary>
    /// <param name="shfbProjectFileName">Name of the Sandcastle help file builder project file.</param>
    /// <returns>The full name of the content file (if it could be extracted from the project file), or null.</returns>
    public static string? ExtractContentLayoutFileNameFromShfbproj(string shfbProjectFileName)
    {
      var doc = new XmlDocument();
      //Load the the document with the last book node.
      doc.Load(shfbProjectFileName);

      XmlNode currNode = doc.DocumentElement.FirstChild;
      while (null != currNode)
      {
        if (currNode.Name == "ItemGroup" && currNode.FirstChild?.Name == "ContentLayout")
        {
          var clFileName = currNode.FirstChild.Attributes["Include"];
          return Path.Combine(Path.GetDirectoryName(shfbProjectFileName) ?? throw new InvalidOperationException($"Unable to get directory name of file {shfbProjectFileName}"), clFileName.Value);
        }

        currNode = currNode.NextSibling;
      }

      return null;
    }

    /// <summary>
    /// Updates the Sandcastle help file builder project file. This update removes old .aml files and old referenced images and
    /// replaces them with new .aml files and image files.
    /// </summary>
    /// <param name="shfbProjectFileName">Sandcastle help file builder project file.</param>
    /// <param name="contentLayoutFileName">Name of the content layout file (extension: .content).</param>
    /// <param name="amlFileNames">Enumeration of all .aml files that should be included in the Sandcastle help file builder project.</param>
    /// <param name="imageFileNames">Enumeration of the names of all image files that should be included in the Sandcastle help file builder project.</param>
    public static void UpdateShfbproj(string shfbProjectFileName, string contentLayoutFileName, IEnumerable<string> amlFileNames, IEnumerable<string> imageFileNames)
    {
      XmlNode? contentLayoutNode = null;
      XmlNode? amlFilesNode = null;
      XmlNode? imageFilesNode = null;

      string projectDirectory = Path.GetDirectoryName(shfbProjectFileName) ?? throw new InvalidOperationException($"Unable to get directory of file name {shfbProjectFileName}");

      var doc = new XmlDocument();
      //Load the the document with the last book node.
      doc.Load(shfbProjectFileName);

      XmlNode currNode = doc.DocumentElement.FirstChild;
      while (null != currNode)
      {
        if (currNode.Name == "ItemGroup" && currNode.FirstChild?.Name == "ContentLayout")
        {
          contentLayoutNode = currNode;
        }
        if (currNode.Name == "ItemGroup" && currNode.FirstChild?.Name == "Image")
        {
          imageFilesNode = currNode;
        }
        if (currNode.Name == "ItemGroup" && currNode.FirstChild?.Name == "None")
        {
          amlFilesNode = currNode;
        }

        currNode = currNode.NextSibling;
      }

      if (null == contentLayoutNode)
      {
        var itemGroup = doc.CreateElement("ItemGroup", doc.DocumentElement.NamespaceURI);

        doc.DocumentElement.AppendChild(itemGroup);
        contentLayoutNode = itemGroup;
      }

      if (null == amlFilesNode && amlFileNames.Any())
      {
        var itemGroup = doc.CreateElement("ItemGroup", doc.DocumentElement.NamespaceURI);
        doc.DocumentElement.AppendChild(itemGroup);
        amlFilesNode = itemGroup;
      }

      if (null == imageFilesNode && imageFileNames.Any())
      {
        var itemGroup = doc.CreateElement("ItemGroup", doc.DocumentElement.NamespaceURI);
        doc.DocumentElement.AppendChild(itemGroup);
        imageFilesNode = itemGroup;
      }

      if (null != contentLayoutNode)
      {
        contentLayoutNode.RemoveAll();

        var layoutNode = doc.CreateElement("ContentLayout", doc.DocumentElement.NamespaceURI);
        var inclAttr = doc.CreateAttribute("Include");
        inclAttr.Value = GetFileNameRelativeTo(contentLayoutFileName, projectDirectory);
        layoutNode.Attributes.Append(inclAttr);
        contentLayoutNode.AppendChild(layoutNode);
      }

      if (null != amlFilesNode)
      {
        amlFilesNode.RemoveAll();

        foreach (var amlFileName in amlFileNames)
        {
          var noneNode = doc.CreateElement("None", doc.DocumentElement.NamespaceURI);
          var inclAttr = doc.CreateAttribute("Include");
          inclAttr.Value = GetFileNameRelativeTo(amlFileName, projectDirectory);
          noneNode.Attributes.Append(inclAttr);
          amlFilesNode.AppendChild(noneNode);
        }
      }

      if (null != imageFilesNode)
      {
        imageFilesNode.RemoveAll();

        foreach (var imageFileName in imageFileNames)
        {
          var imgNode = doc.CreateElement("Image", doc.DocumentElement.NamespaceURI);
          var inclAttr = doc.CreateAttribute("Include");
          inclAttr.Value = GetFileNameRelativeTo(imageFileName, projectDirectory);
          imgNode.Attributes.Append(inclAttr);

          var imgId = doc.CreateElement("ImageId", doc.DocumentElement.NamespaceURI);
          imgId.InnerText = Path.GetFileNameWithoutExtension(imageFileName);
          imgNode.AppendChild(imgId);

          var altText = doc.CreateElement("AlternateText", doc.DocumentElement.NamespaceURI);
          altText.InnerText = Path.GetFileNameWithoutExtension(imageFileName);
          imgNode.AppendChild(altText);

          imageFilesNode.AppendChild(imgNode);
        }
      }

      // Finally, save the sandcastle help file builder project
      doc.Save(shfbProjectFileName);
    }

    /// <summary>
    /// Gets the file name relative to a directory. The returned relative file name is HTML friendly.
    /// </summary>
    /// <param name="fullFileName">Full name of the file.</param>
    /// <param name="baseDirectory">The full name of the directory.</param>
    /// <returns>The path name relative to the provided directory. Backslashes are replaced with slashes to conform with HTML style.</returns>
    public static string GetFileNameRelativeTo(string fullFileName, string baseDirectory)
    {
      if (!Path.IsPathRooted(fullFileName))
        throw new ArgumentException("Path is not rooted", nameof(fullFileName));

      var dir = Path.GetDirectoryName(fullFileName) ?? throw new InvalidOperationException($"Unable to get directory of file name {fullFileName}");

      if (!dir.StartsWith(baseDirectory))
        throw new ArgumentException("File must be in the base directory or in a subdirectory", nameof(fullFileName));

      int addLength = baseDirectory.EndsWith("" + Path.DirectorySeparatorChar) ? 0 : 1;

      return fullFileName.Substring(baseDirectory.Length + addLength).Replace('\\', '/');
    }
  }
}
