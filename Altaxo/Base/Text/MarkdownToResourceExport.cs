#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2021 Dr. Dirk Lellinger
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
using System.Collections;
using System.Collections.Generic;
using System.Resources;

namespace Altaxo.Text
{
  /// <summary>
  /// Helper function to prepare the export of a markdown document, including the images, to a .resx resource file.
  /// The images are exported as byte arrays, and the Url of the images in the markdown document is changed, assuming that later on these
  /// images are available as resources.
  /// </summary>
  public static class MarkdownToResourceExport
  {
    /// <summary>
    /// Exports the specified <see cref="TextDocument"/> to an external markdown file.
    /// </summary>
    /// <param name="document">The document to export.</param>
    /// <param name="resourceKeyOfDocument">The resource key of document.</param>
    /// <param name="prefixOfResourceFiles">The prefix of resource files. It is assumed that string resources and image resources reside in the same folder.
    /// Example: If the resource files reside in the source folder /Calc/FitFunctions, you must provide 'Altaxo.Calc.FitFunctions' (Altaxo first because its the namespace of the assembly).
    /// </param>
    public static (string markdownDocument, Dictionary<string, byte[]> Images) Export(
      TextDocument document,
      string resourceKeyOfDocument,
      string prefixOfResourceFiles)
    {
      // Expand the document. This will also set the ReferencedImageUrls
      document = ChildDocumentExpander.ExpandDocumentToNewDocument(document);
      var sourceDoc = new System.Text.StringBuilder(document.SourceText);

      if (document.ReferencedImageUrls is null)
        throw new InvalidProgramException("ReferencedImageUrls should be set before in ExpandDocumentToNewDocument");

      var list = new List<(string Url, int urlSpanStart, int urlSpanEnd)>(document.ReferencedImageUrls);

      list.Sort((x, y) => Comparer<int>.Default.Compare(y.urlSpanEnd, x.urlSpanEnd)); // Note the inverse order of x and y to sort urlSpanEnd descending

      var imageStreamProvider = new ImageStreamProvider();

      // Export images
      var imageDictionary = new Dictionary<string, byte[]>();
      int imageNumber = 0;
      foreach (var (Url, urlSpanStart, urlSpanEnd) in list)
      {
        if (Url.StartsWith("res:"))
          continue; // the image is already a resource, it is not neccessary to export it

        using (var stream = new System.IO.MemoryStream())
        {
          var streamResult = imageStreamProvider.GetImageStream(stream, Url, 4 * 96, Altaxo.Main.ProjectFolder.GetFolderPart(document.Name), document.Images);
          if (streamResult.IsValid)
          {
            ++imageNumber;
            var relativeResourceKeyOfImage = FormattableString.Invariant($"{resourceKeyOfDocument}.Fig{imageNumber}{streamResult.Extension}");
            var absoluteResourceKeyOfImage = FormattableString.Invariant($"{prefixOfResourceFiles}.{relativeResourceKeyOfImage}");


            // now change the url in the markdown text
            var newUrl = $"res:{absoluteResourceKeyOfImage}";
            sourceDoc.Remove(urlSpanStart, 1 + urlSpanEnd - urlSpanStart);
            sourceDoc.Insert(urlSpanStart, newUrl);
            imageDictionary.Add(relativeResourceKeyOfImage, stream.ToArray());
          }
        }
      }

      return (sourceDoc.ToString(), imageDictionary);
    }

    /// <summary>
    /// Exports the specified document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="resourceKeyOfDocument">The resource key of document.</param>
    /// <param name="prefixOfResourceFiles">The prefix of resource files. It is assumed that string resources and image resources reside in the same folder.
    /// Example: If the resource files reside in the source folder /Calc/FitFunctions, you must provide 'Altaxo.Calc.FitFunctions' (Altaxo first because its the namespace of the assembly).
    /// </param>
    /// <param name="stringResources">A dictionary in which the string resources are collected. Key is the relative resource key name, Value is the markdown text.</param>
    /// <param name="imageResources">A dictionary in which the image resources are collected. Key is the relative resource key name, Value is the image as an array of bytes.</param>
    public static void Export(
      TextDocument document,
      string resourceKeyOfDocument,
      string prefixOfResourceFiles,
      IDictionary<string, string> stringResources,
      IDictionary<string, byte[]> imageResources)
    {
      var (markdownText, imageDictionary) = Export(document, resourceKeyOfDocument, prefixOfResourceFiles);

      stringResources.Add(resourceKeyOfDocument, markdownText);
      foreach (var entry in imageDictionary)
        imageResources.Add(entry.Key, entry.Value);
    }

    /// <summary>
    /// Exports the specified documents.
    /// </summary>
    /// <param name="documents">The documents as an enumerable of tuples, consisting of the markdown document and the relative resource key
    /// in which it should be stored.</param>
    /// <param name="prefixOfResourceFiles">The prefix of resource files. It is assumed that string resources and image resources reside in the same folder.
    /// Example: If the resource files reside in the source folder /Calc/FitFunctions, you must provide 'Altaxo.Calc.FitFunctions' (Altaxo first because its the namespace of the assembly).
    /// </param>
    /// <param name="stringResources">A dictionary in which the string resources are collected. Key is the relative resource key name, Value is the markdown text.</param>
    /// <param name="imageResources">A dictionary in which the image resources are collected. Key is the relative resource key name, Value is the image as an array of bytes.</param>
    public static void Export(
     IEnumerable<(TextDocument document, string relativeResourceKeyForDocument)> documents,
     string prefixOfResourceFiles,
     IDictionary<string, string> stringResources,
     IDictionary<string, byte[]> imageResources)
    {
      foreach (var (doc, resKey) in documents)
      {
        Export(doc, resKey, prefixOfResourceFiles, stringResources, imageResources);
      }
    }


    /// <summary>
    /// Exports the collected markdown documents and images to the string and the image resource file.
    /// </summary>
    /// <param name="stringDictionary">The string dictionary. Key is the relative resource key, value is the markdown text that should be stored.</param>
    /// <param name="imageDictionary">The image dictionary. Key is the relative resource key, value is the byte array that represents the image.</param>
    /// <param name="prefixOfResourceFiles">The prefix of resource files. It is assumed that string resources and image resources reside in the same folder.
    /// Example: If the resource files reside in the source folder /Calc/FitFunctions, you must provide 'Altaxo.Calc.FitFunctions' (Altaxo first because its the namespace of the assembly).
    /// </param>
    /// <param name="stringResourceReader">The string resource reader.</param>
    /// <param name="stringResourceWriter">The string resource writer.</param>
    /// <param name="storeStringResourcesWithAbsoluteKey">If true, the string resources are stored with the absolute resource key instead of the relative resource key.</param>
    /// <param name="imageResourceReader">The image resource reader.</param>
    /// <param name="imageResourceWriter">The image resource writer.</param>
    /// <param name="storeImageResourcesWithAbsoluteKey">If true, the image resources are stored with the absolute resource key instead of the relative resource key.</param>
    /// <exception cref="ArgumentNullException">
    /// stringResourceReader
    /// or
    /// stringResourceWriter
    /// or
    /// imageResourceReader
    /// or
    /// imageResourceWriter
    /// </exception>
    public static void ExportMarkdownDocumentsToResourceFiles(
      IDictionary<string, string> stringDictionary,
      IDictionary<string, byte[]> imageDictionary,
      string prefixOfResourceFiles,
      IResourceReader stringResourceReader, IResourceWriter stringResourceWriter, bool storeStringResourcesWithAbsoluteKey,
      IResourceReader imageResourceReader, IResourceWriter imageResourceWriter, bool storeImageResourcesWithAbsoluteKey)
    {
      if (stringResourceReader is null)
      {
        throw new ArgumentNullException(nameof(stringResourceReader));
      }
      if (stringResourceWriter is null)
      {
        throw new ArgumentNullException(nameof(stringResourceWriter));
      }
      if (imageResourceReader is null)
      {
        throw new ArgumentNullException(nameof(imageResourceReader));
      }
      if (imageResourceWriter is null)
      {
        throw new ArgumentNullException(nameof(imageResourceWriter));
      }
      var prefixOfResourceFilesWithTrailingDot = prefixOfResourceFiles + ".";



      // at first, write all other resource but left out the resources that should be added afterwards
      foreach (DictionaryEntry entry in stringResourceReader)
      {
        var key = (string)entry.Key;
        var value = entry.Value;
        if (storeStringResourcesWithAbsoluteKey)
        {
          if (!(key.StartsWith(prefixOfResourceFilesWithTrailingDot) && stringDictionary.ContainsKey(key.Substring(prefixOfResourceFilesWithTrailingDot.Length))))
          {
            stringResourceWriter.AddResource(key, value);
          }
        }
        else
        {
          if (!(stringDictionary.ContainsKey(key)))
          {
            stringResourceWriter.AddResource(key, value);
          }
        }
      }


      foreach (DictionaryEntry entry in imageResourceReader)
      {
        var key = (string)entry.Key;
        var value = entry.Value;
        if (storeImageResourcesWithAbsoluteKey)
        {
          if (!(key.StartsWith(prefixOfResourceFilesWithTrailingDot) && imageDictionary.ContainsKey(key.Substring(prefixOfResourceFilesWithTrailingDot.Length))))
          {
            if (value is byte[] ba)
              imageResourceWriter.AddResource(key, ba);
            else
              imageResourceWriter.AddResource(key, value);
          }
        }
        else
        {
          if (!(imageDictionary.ContainsKey(key)))
          {
            if (value is byte[] ba)
              imageResourceWriter.AddResource(key, ba);
            else
              imageResourceWriter.AddResource(key, value);
          }
        }
      }


      // now add the string resources
      {
        var list = new List<string>(stringDictionary.Keys);
        list.Sort();
        foreach (var key in list)
        {
          if (storeStringResourcesWithAbsoluteKey)
            stringResourceWriter.AddResource(prefixOfResourceFilesWithTrailingDot + key, stringDictionary[key]);
          else
            stringResourceWriter.AddResource(key, stringDictionary[key]);
        }
      }
      // now add the image resources
      {
        var list = new List<string>(imageDictionary.Keys);
        list.Sort();
        foreach (var key in list)
        {
          if (storeImageResourcesWithAbsoluteKey)
            imageResourceWriter.AddResource(prefixOfResourceFilesWithTrailingDot + key, imageDictionary[key]);
          else
            imageResourceWriter.AddResource(key, imageDictionary[key]);
        }
      }
    }

    /// <summary>
    /// Exports the markdown documents and their referenced images to the string and the image resource file.
    /// </summary>
    /// <param name="documents">The documents as an enumerable of tuples, consisting of the markdown document and the relative resource key
    /// in which it should be stored.</param>
    /// <param name="prefixOfResourceFiles">The prefix of resource files. It is assumed that string resources and image resources reside in the same folder.
    /// Example: If the resource files reside in the source folder /Calc/FitFunctions, you must provide 'Altaxo.Calc.FitFunctions' (Altaxo first because its the namespace of the assembly).
    /// </param>
    /// <param name="stringResourceReader">The string resource reader.</param>
    /// <param name="stringResourceWriter">The string resource writer.</param>
    /// <param name="storeStringResourcesWithAbsoluteKey">If true, the string resources are stored with the absolute resource key instead of the relative resource key.</param>
    /// <param name="imageResourceReader">The image resource reader.</param>
    /// <param name="imageResourceWriter">The image resource writer.</param>
    /// <param name="storeImageResourcesWithAbsoluteKey">If true, the image resources are stored with the absolute resource key instead of the relative resource key.</param>
    /// <exception cref="ArgumentNullException">
    /// stringResourceReader
    /// or
    /// stringResourceWriter
    /// or
    /// imageResourceReader
    /// or
    /// imageResourceWriter
    /// </exception>
    public static void ExportMarkdownDocumentsToResourceFiles(
      IEnumerable<(TextDocument document, string relativeResourceKeyForDocument)> documents,
      string prefixOfResourceFiles,
      IResourceReader stringResourceReader, IResourceWriter stringResourceWriter, bool storeStringResourcesWithAbsoluteKey,
      IResourceReader imageResourceReader, IResourceWriter imageResourceWriter, bool storeImageResourcesWithAbsoluteKey)
    {
      var stringDictionary = new Dictionary<string, string>();
      var imageDictionary = new Dictionary<string, byte[]>();

      Export(documents, prefixOfResourceFiles, stringDictionary, imageDictionary);

      ExportMarkdownDocumentsToResourceFiles(
        stringDictionary, imageDictionary,
        prefixOfResourceFiles,
        stringResourceReader, stringResourceWriter, storeStringResourcesWithAbsoluteKey,
        imageResourceReader, imageResourceWriter, storeImageResourcesWithAbsoluteKey);
    }

    /// <summary>
    /// Exports the markdown document and its referenced images to the string and the image resource file.
    /// </summary>
    /// <param name="document">The markdown document to export.</param>
    /// <param name="resourceKeyOfDocument">The relative resource key in which the markdown text should be stored.</param>
    /// <param name="prefixOfResourceFiles">The prefix of resource files. It is assumed that string resources and image resources reside in the same folder.
    /// Example: If the resource files reside in the source folder /Calc/FitFunctions, you must provide 'Altaxo.Calc.FitFunctions' (Altaxo first because its the namespace of the assembly).
    /// </param>
    /// <param name="stringResourceReader">The string resource reader.</param>
    /// <param name="stringResourceWriter">The string resource writer.</param>
    /// <param name="storeStringResourcesWithAbsoluteKey">If true, the string resources are stored with the absolute resource key instead of the relative resource key.</param>
    /// <param name="imageResourceReader">The image resource reader.</param>
    /// <param name="imageResourceWriter">The image resource writer.</param>
    /// <param name="storeImageResourcesWithAbsoluteKey">If true, the image resources are stored with the absolute resource key instead of the relative resource key.</param>
    /// <exception cref="ArgumentNullException">
    /// stringResourceReader
    /// or
    /// stringResourceWriter
    /// or
    /// imageResourceReader
    /// or
    /// imageResourceWriter
    /// </exception>
    public static void ExportMarkdownDocumentToResourceFile(TextDocument document,
      string resourceKeyOfDocument,
      string prefixOfResourceFiles,
      IResourceReader stringResourceReader, IResourceWriter stringResourceWriter, bool storeStringResourcesWithAbsoluteKey,
      IResourceReader imageResourceReader, IResourceWriter imageResourceWriter, bool storeImageResourcesWithAbsoluteKey)
    {
      var stringDictionary = new Dictionary<string, string>();
      var imageDictionary = new Dictionary<string, byte[]>();

      Export(document, resourceKeyOfDocument, prefixOfResourceFiles, stringDictionary, imageDictionary);

      ExportMarkdownDocumentsToResourceFiles(
        stringDictionary, imageDictionary,
        prefixOfResourceFiles,
        stringResourceReader, stringResourceWriter, storeStringResourcesWithAbsoluteKey,
        imageResourceReader, imageResourceWriter, storeImageResourcesWithAbsoluteKey);
    }
  }
}
