#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2019 Dr. Dirk Lellinger
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altaxo.Graph;
using Altaxo.Main;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Altaxo.Text
{
  public static class FigureRenumerator
  {
    /// <summary>
    /// Renumerates all figures, all elements that are enclosed in ^^^ and that have a figure caption.
    /// The links to those figures are updated, too.
    /// </summary>
    /// <param name="documentText">The document text.</param>
    /// <returns>The modified document text, with renumerated figure captions and updated links.</returns>
    public static string RenumerateFigures(string documentText)
    {
      // first parse the document with markdig
      var pipeline = new MarkdownPipelineBuilder();
      pipeline = MarkdownUtilities.UseSupportedExtensions(pipeline);
      var builtPipeline = pipeline.Build();
      var markdownDocument = Markdig.Markdown.Parse(documentText, builtPipeline);

      return RenumerateFigures(markdownDocument, documentText);
    }


    /// <summary>
    /// Renumerates all figures, all elements that are enclosed in ^^^ and that have a figure caption.
    /// The links to those figures are updated, too.
    /// </summary>
    /// <param name="document">The parsed markdown document.</param>
    /// <param name="documentText">The document text.</param>
    /// <returns>The modified document text, with renumerated figure captions and updated links.</returns>
    public static string RenumerateFigures(MarkdownDocument document, string documentText)
    {
      // Get the list of all captions with the positions of the caption id and the position of the number
      var captionList = GetCaptionList(document);

      // Now parse through all links
      var linkList = GetLinkList(document, captionList);


      // now attach a new number to each figure caption
      // we store the numbers in a separate list

      var captionNumbers = GetCaptionNumberList(captionList);

      // now we have everything in order to modify the text
      // for this we have to collect all the positions where text has to be changed,
      // and we start then the modification at the end of the text

      var modificationList = new List<((int Position, int Count) Number, bool isLink, int Index)>();

      for (int i = 0; i < captionList.Count; ++i)
      {
        modificationList.Add((captionList[i].Number, false, i));
      }
      for (int i = 0; i < linkList.Count; ++i)
      {
        modificationList.Add((linkList[i].Number, true, i));
      }

      modificationList.Sort((x, y) => Comparer<int>.Default.Compare(y.Number.Position, x.Number.Position));


      // now we can change the text according to the modification list

      var stb = new StringBuilder(documentText);

      foreach (var entry in modificationList)
      {
        int number = 0;
        if (entry.isLink)
        {
          var link = linkList[entry.Index];
          number = captionNumbers[link.CaptionListIndex];
        }
        else
        {
          number = captionNumbers[entry.Index];
        }

        if (entry.Number.Position >= 0) // if position is negative, the number is missing in the caption or in the link
        {
          stb.Remove(entry.Number.Position, entry.Number.Count);
          stb.Insert(entry.Number.Position, number.ToString());
        }
      }

      return stb.ToString();
    }

    /// <summary>
    /// Gets a list of integers with the same length as the <paramref name="captionList"/>. The numbers in the returned list are the number of the
    /// figures. Each category of figure has its own numbering.
    /// </summary>
    /// <param name="captionList">The caption list.</param>
    /// <returns>List of integers with the same length as the <paramref name="captionList"/> which gives the figure number for each figure in the caption list. </returns>
    public static List<int> GetCaptionNumberList(List<((string Name, int Position, int Count) Category, (int Position, int Count) Number, Markdig.Extensions.Figures.Figure Figure, Markdig.Extensions.Figures.FigureCaption FigureCaption)> captionList)
    {
      var dictCategoryNumber = new Dictionary<string, int>();
      var captionNumbers = new List<int>(captionList.Count);

      foreach (var cap in captionList)
      {
        if (!dictCategoryNumber.TryGetValue(cap.Category.Name, out var number))
        {
          number = 0;
          dictCategoryNumber.Add(cap.Category.Name, number);
        }

        ++number;
        dictCategoryNumber[cap.Category.Name] = number;

        captionNumbers.Add(number);
      }

      return captionNumbers;
    }

    /// <summary>
    /// Gets a list containing all links that point to figures, and that have a number inside the link text that is
    /// considered as the number of the figure the link is referring to.
    /// </summary>
    /// <param name="document">The markdown document.</param>
    /// <param name="captionList">The caption list, as retrieved by <see cref="GetCaptionList(MarkdownDocument)"/>.</param>
    /// <returns>A list containing tuples. Each tuple contains the following elements:
    /// The CaptionListIndex is the index in the <paramref name="captionList"/> that the link is referring to.
    /// The Number tuple contains the position and the length of the number string in the link.
    /// The Link element is the markdown link itself.</returns>
    public static List<(int CaptionListIndex, (int Position, int Count) Number, Markdig.Syntax.Inlines.LinkInline Link)> GetLinkList(MarkdownDocument document, List<((string Name, int Position, int Count) Category, (int Position, int Count) Number, Markdig.Extensions.Figures.Figure Figure, Markdig.Extensions.Figures.FigureCaption FigureCaption)> captionList)
    {
      var linkList = new List<(int CaptionListIndex, (int Position, int Count) Number, Markdig.Syntax.Inlines.LinkInline Link)>();
      foreach (var link in MarkdownUtilities.EnumerateAllMarkdownObjectsRecursively(document).OfType<Markdig.Syntax.Inlines.LinkInline>())
      {
        if (link.IsImage)
          continue;

        if (!link.Url.StartsWith("#"))
          continue;

        // look if the link can be resolved to be in one of the figures

        int captionListIndex = -1;
        for (int i = 0; i < captionList.Count; ++i)
        {
          var fig = captionList[i];
          if (MarkdownUtilities.IsLinkInElement(link.Url, fig.Figure))
          {
            captionListIndex = i;
            break;
          }
        }

        if (captionListIndex < 0)
          continue;

        var linkNumber = ExtractNumberFromLink(link);
        if (linkNumber.Position < 0)
          continue;

        linkList.Add((captionListIndex, linkNumber, link));
      }

      return linkList;
    }

    /// <summary>
    /// Gets a list with all figure captions of the document, where i) there is exactly one figure caption in the figure,
    /// ii) a figure identifier (such as 'Figure') can be clearly identified, and iii) a number can be identified in that caption.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns>A tuple. The category tuple contains the caption identifier (such as 'Figure') along with its position and length.
    /// The Number tuple contains the position and length of the number string in the document.
    /// The 3rd element of the tuple is the figure which contains the caption.
    /// The last element of the tuple is the figure caption.</returns>
    public static List<((string Name, int Position, int Count) Category, (int Position, int Count) Number, Markdig.Extensions.Figures.Figure Figure, Markdig.Extensions.Figures.FigureCaption FigureCaption)> GetCaptionList(MarkdownDocument document)
    {
      var captionList = new List<((string Name, int Position, int Count) Category, (int Position, int Count) Number, Markdig.Extensions.Figures.Figure Figure, Markdig.Extensions.Figures.FigureCaption FigureCaption)>();

      foreach (var figure in MarkdownUtilities.EnumerateAllMarkdownObjectsRecursively(document).OfType<Markdig.Extensions.Figures.Figure>())
      {
        var figureCaption = MarkdownUtilities.EnumerateAllMarkdownObjectsRecursively(figure).OfType<Markdig.Extensions.Figures.FigureCaption>().SingleOrDefault();

        if (null == figureCaption)
          continue;

        var (category, digits) = ExtractCategoryAndNumber(figureCaption);

        if (!string.IsNullOrEmpty(category.Name))
        {
          captionList.Add((category, digits, figure, figureCaption));
        }
      }

      return captionList;
    }



    /// <summary>
    /// Extracts the category and number from a figure caption.
    /// </summary>
    /// <param name="caption">The figure caption.</param>
    /// <returns>A tuple, consisting of two tuples: Category and Digits. The Category tuple contains the category name, as well as the postion and count of the category name in the source text.
    /// the tuple Digits contains the position and count of the digits that are considered as the figure number.</returns>
    public static ((string Name, int Position, int Count) Category, (int Position, int Count) Digits) ExtractCategoryAndNumber(Markdig.Extensions.Figures.FigureCaption caption)
    {
      var words = new List<(string Text, int Position)>();
      ExtractTextContentFrom(caption, words);

      // we consider the first letters, up to the first occurence of a space or a digit, as the figure name

      if (words.Count == 0)
        return ((null, -1, 0), (-1, 0));

      string figureName = null;


      int i;
      string text;
      int textPos = -1;

      for (int w = 0; w < words.Count; ++w)
      {
        (text, textPos) = Trim(words[0]);
        if (!string.IsNullOrEmpty(text))
        {
          int idxOfNonLetterChar = text.Length;
          for (i = 0; i < text.Length; ++i)
          {
            if (!char.IsLetter(text[i]))
            {
              idxOfNonLetterChar = i;
              break;
            }
          }
          figureName = text.Substring(0, idxOfNonLetterChar);

          if (text.Length > 0)
            break;
        }
      }
      if (string.IsNullOrEmpty(figureName))
      {
        return ((null, -1, 0), (-1, 0));
      }

      int namePosition = textPos;
      int nameCount = figureName.Length;

      // Examples for valid Figure captions that should be recognized (always the 74, and either Figure or Fig)
      // Figure 74 this is
      // Figure 2.74 this is
      // Fig.74 this is
      // Fig. 74 this is
      // Figure 74: this is
      // Figure 23.74: this is
      // Fig.74: this is
      // Fig. 74: this is

      int idxWord = 0;
      int positionLastDigit = -1;
      int positionFirstDigit = -1;
      int subWordsConsidered = 0;
      text = words[idxWord].Text;
      textPos = words[idxWord].Position;
      for (; ; )
      {
        (text, textPos) = Trim((text, textPos));
        if (!string.IsNullOrEmpty(text))
        {
          ++subWordsConsidered;
          var idx = text.IndexOfAny(new char[] { ' ', '\t', '\r', '\n' });
          if (idx < 0)
            idx = text.Length;
          var subWord = text.Substring(0, idx);
          var idx2 = subWord.LastIndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
          if (idx2 >= 0)
          {
            int idx3 = idx2;
            for (int k = idx2; k >= 0; --k)
            {
              if (char.IsDigit(subWord[k]))
                idx3 = k;
              else
                break;
            }
            positionFirstDigit = textPos + idx3;
            positionLastDigit = textPos + idx2;
            break;
          }

          if (subWordsConsidered >= 2)
          {
            break;
          }

          text = text.Substring(idx, text.Length - idx);
          textPos += idx;
        }


        if (string.IsNullOrEmpty(text))
        {
          // Proceed to next word
          ++idxWord;
          if (idxWord >= words.Count)
            break;

          text = words[idxWord].Text;
          textPos = words[idxWord].Position;
        }
      }

      return ((figureName, namePosition, nameCount), (positionFirstDigit, positionLastDigit - positionFirstDigit + 1));
    }


    /// <summary>
    /// Extracts the text position of the number from a link.
    /// Note: if the link text contains more than one number, only the position of the last number is returned.
    /// </summary>
    /// <param name="link">The link.</param>
    /// <returns>A tuple containing the position and count of the number in the link. If the link does not contain a number,
    /// the tuple (-1, 0) is returned.</returns>
    public static (int Position, int Count) ExtractNumberFromLink(Markdig.Syntax.Inlines.LinkInline link)
    {
      var words = new List<(string Text, int Position)>();
      ExtractTextContentFrom(link, words);

      // we consider the first letters, up to the first occurence of a space or a digit, as the figure name

      if (words.Count == 0)
        return (-1, 0);

      // in the link, we search for the last occurrence of digits

      int positionLastDigit = -1;
      int positionFirstDigit = -1;
      for (int idxWord = words.Count - 1; idxWord >= 0; --idxWord)
      {
        var (text, textPos) = Trim(words[idxWord]);
        if (string.IsNullOrEmpty(text))
          continue;

        var idx2 = text.LastIndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });

        if (idx2 < 0)
          continue;

        int idx3 = idx2;
        for (int k = idx2; k >= 0; --k)
        {
          if (char.IsDigit(text[k]))
            idx3 = k;
          else
            break;
        }
        positionFirstDigit = textPos + idx3;
        positionLastDigit = textPos + idx2;
        break;
      }

      return (positionFirstDigit, positionLastDigit - positionFirstDigit + 1);
    }


    /// <summary>
    /// Helper function that works like <see cref="string.TrimStart"/> but additionally tracks the position.
    /// </summary>
    /// <param name="text">The text tuple, consisting of the text and the position of that text in the source text.</param>
    /// <returns>The text trimmed at the start, and the position of the trimmed text in the source text.</returns>
    public static (string Text, int Position) TrimStart((string Text, int Position) text)
    {
      for (int i = 0; i < text.Text.Length; ++i)
      {
        if (!char.IsWhiteSpace(text.Text[i]))
          return (text.Text.Substring(i, text.Text.Length - i), text.Position + i);
      }
      return (string.Empty, text.Text.Length + text.Position);
    }

    /// <summary>
    /// Helper function that works like <see cref="string.Trim()"/> but additionally tracks the position.
    /// </summary>
    /// <param name="text">The text tuple, consisting of the text and the position of that text in the source text.</param>
    /// <returns>The text trimmed, and the position of the trimmed text in the source text.</returns>
    public static (string Text, int Position) Trim((string Text, int Position) text)
    {
      for (int i = 0; i < text.Text.Length; ++i)
      {
        if (!char.IsWhiteSpace(text.Text[i]))
          return (text.Text.Substring(i, text.Text.Length - i).TrimEnd(), text.Position + i);
      }
      return (string.Empty, text.Text.Length + text.Position);
    }




    /// <summary>
    /// Gets the childs of a markdown object. Null is returned if no childs were to be found.
    /// </summary>
    /// <param name="parent">The markdown object from which to get the childs.</param>
    /// <returns>The childs of the given markdown object, or null.</returns>
    public static IReadOnlyList<MarkdownObject> GetChilds(MarkdownObject parent)
    {
      if (parent is LeafBlock leafBlock)
        return leafBlock.Inline?.ToArray<MarkdownObject>();
      else if (parent is Markdig.Syntax.Inlines.ContainerInline containerInline)
        return containerInline.ToArray<MarkdownObject>();
      else if (parent is ContainerBlock containerBlock)
        return containerBlock;
      else
        return null;
    }

    /// <summary>
    /// Extracts the text content from a <see cref="Markdig.Syntax.LeafBlock"/>.
    /// </summary>
    /// <param name="leafBlock">The leaf block to extract the text from.</param>
    /// <param name="result">The result. Contains one or more text fragments bundled with its position in the source text.</param>
    /// <exception cref="NotImplementedException"></exception>
    public static void ExtractTextContentFrom(Markdig.Syntax.LeafBlock leafBlock, List<(string Text, int Position)> result)
    {
      if (null == leafBlock.Inline)
        return;

      foreach (var il in leafBlock.Inline)
      {
        switch (il)
        {
          case Markdig.Syntax.Inlines.CodeInline childCodeInline:
            result.Add((childCodeInline.Content, childCodeInline.Span.Start));
            break;
          case Markdig.Syntax.Inlines.ContainerInline childContainerInline:
            ExtractTextContentFrom(childContainerInline, result);
            break;
          case Markdig.Syntax.Inlines.LiteralInline literalInline:
            result.Add((literalInline.Content.ToString(), literalInline.Span.Start));
            break;
          default:
            throw new NotImplementedException();
        }
      }
    }

    /// <summary>
    /// Extracts the text content from a <see cref="Markdig.Syntax.Inlines.ContainerInline"/>.
    /// </summary>
    /// <param name="containerInline">The container inline to extract the text from.</param>
    /// <param name="result">The result. Contains one or more text fragments bundled with its position in the source text.</param>
    /// <exception cref="NotImplementedException"></exception>
    public static void ExtractTextContentFrom(Markdig.Syntax.Inlines.ContainerInline containerInline, List<(string Text, int Position)> result)
    {
      foreach (var il in containerInline)
      {
        switch (il)
        {
          case Markdig.Syntax.Inlines.CodeInline childCodeInline:
            result.Add((childCodeInline.Content, childCodeInline.Span.Start));
            break;
          case Markdig.Syntax.Inlines.ContainerInline childContainerInline:
            ExtractTextContentFrom(childContainerInline, result);
            break;
          case Markdig.Syntax.Inlines.LiteralInline literalInline:
            result.Add((literalInline.Content.ToString(), literalInline.Span.Start));
            break;
          default:
            throw new NotImplementedException();
        }
      }
    }
  }
}
