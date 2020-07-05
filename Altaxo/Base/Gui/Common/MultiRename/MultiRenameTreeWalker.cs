#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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
using System.Text;
using Altaxo.Main.PegParser;

namespace Altaxo.Gui.Common.MultiRename
{
  internal class MultiRenameTreeWalker
  {
    private string _sourceText;
    private MultiRenameData _renameData;
    private PegNode _tree;

    public MultiRenameTreeWalker(string sourceText, MultiRenameData data)
    {
      _sourceText = sourceText;
      _renameData = data;
      var parser = new MultiRenameParser(data);
      parser.SetSource(_sourceText);

      bool bMatches = parser.MainSentence();
      _tree = parser.GetRoot();
    }

    public IMultiRenameElement VisitTree()
    {
      var result = new MultiRenameElementCollection();

      var child = _tree.child_;
      int startLiteral = 0;

      while (null != child)
      {
        int startChild = child.match_.posBeg_;
        if (startChild > startLiteral)
        {
          result.Elements.Add(new MultiRenameLiteralElement(_sourceText.Substring(startLiteral, startChild - startLiteral)));
        }
        startLiteral = child.match_.posEnd_;

        switch (child.id_)
        {
          case (int)EAltaxo_MultiRename.EscBracket:
            result.Elements.Add(HandleEscBracket(child));
            break;

          case (int)EAltaxo_MultiRename.IntegerTemplate:
            result.Elements.Add(HandleIntegerTemplate(child));
            break;

          case (int)EAltaxo_MultiRename.StringTemplate:
            result.Elements.Add(HandleStringTemplate(child));
            break;

          case (int)EAltaxo_MultiRename.DateTimeTemplate:
            result.Elements.Add(HandleDateTimeTemplate(child));
            break;

          case (int)EAltaxo_MultiRename.ArrayTemplate:
            result.Elements.Add(HandleArrayTemplate(child));
            break;
        }

        child = child.next_;
      }

      if (_sourceText.Length > startLiteral)
        result.Elements.Add(new MultiRenameLiteralElement(_sourceText.Substring(startLiteral, _sourceText.Length - startLiteral)));

      return result;
    }

    private IMultiRenameElement HandleEscBracket(PegNode node)
    {
      return new MultiRenameLiteralElement("[");
    }

    private IMultiRenameElement HandleIntegerTemplate(PegNode node)
    {
      var childNode = node.child_;
      if (childNode == null)
        throw new ArgumentNullException("childNode");
      string shortCut = _sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length);
      int numberOfDigits = 0;
      int offset = 0;
      int step = 1;

      while (null != (childNode = childNode.next_))
      {
        switch (childNode.id_)
        {
          case (int)EAltaxo_MultiRename.IntArgNumberOfDigits:
            numberOfDigits = int.Parse(_sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length));
            break;

          case (int)EAltaxo_MultiRename.IntArg1st:
            offset = int.Parse(_sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length));
            break;

          case (int)EAltaxo_MultiRename.IntArg2nd:
            step = int.Parse(_sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length));
            break;
        }
      }

      return new MultiRenameIntegerElement(_renameData, shortCut, numberOfDigits, offset, step);
    }

    private IMultiRenameElement HandleStringTemplate(PegNode node)
    {
      var childNode = node.child_;
      if (childNode == null)
        throw new ArgumentNullException("childNode");
      string shortCut = _sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length);

      int start = 0;
      int last = -1;

      while (null != (childNode = childNode.next_))
      {
        switch (childNode.id_)
        {
          case (int)EAltaxo_MultiRename.IntArgOnly:
            start = last = int.Parse(_sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length));
            break;

          case (int)EAltaxo_MultiRename.IntArg1st:
            start = int.Parse(_sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length));
            break;

          case (int)EAltaxo_MultiRename.IntArg2nd:
            last = int.Parse(_sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length));
            break;
        }
      }

      return new MultiRenameStringElement(_renameData, shortCut, start, last);
    }

    private IMultiRenameElement HandleDateTimeTemplate(PegNode node)
    {
      string? dateTimeFormat = null;
      bool useUtcTime = false;
      var childNode = node.child_;
      if (childNode == null)
        throw new ArgumentNullException("childNode");
      string shortCut = _sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length);

      while (null != (childNode = childNode.next_))
      {
        switch (childNode.id_)
        {
          case (int)EAltaxo_MultiRename.StringContent:
            dateTimeFormat = GetEscStringText(childNode);
            break;

          case (int)EAltaxo_MultiRename.DateTimeKind:
            useUtcTime = 'u' == char.ToLowerInvariant(_sourceText[childNode.match_.posBeg_]);
            break;
        }
      }

      return new MultiRenameDateTimeElement(_renameData, shortCut, dateTimeFormat, useUtcTime);
    }

    private IMultiRenameElement HandleArrayTemplate(PegNode node)
    {
      var childNode = node.child_;
      if (childNode == null)
        throw new ArgumentNullException("childNode");
      string shortCut = _sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length);

      int start = 0;
      int last = -1;
      string separator = "\\";

      while (null != (childNode = childNode.next_))
      {
        switch (childNode.id_)
        {
          case (int)EAltaxo_MultiRename.StringContent:
            separator = GetEscStringText(childNode);
            break;

          case (int)EAltaxo_MultiRename.IntArgOnly:
            start = last = int.Parse(_sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length));
            break;

          case (int)EAltaxo_MultiRename.IntArg1st:
            start = int.Parse(_sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length));
            break;

          case (int)EAltaxo_MultiRename.IntArg2nd:
            last = int.Parse(_sourceText.Substring(childNode.match_.posBeg_, childNode.match_.Length));
            break;
        }
      }

      return new MultiRenameArrayElement(_renameData, shortCut, start, last, separator);
    }

    private string GetText(PegNode node)
    {
      return _sourceText.Substring(node.match_.posBeg_, node.match_.Length);
    }

    private string GetEscStringText(PegNode node)
    {
      return TransformToLiteral(GetText(node));
    }

    private string TransformToLiteral(string escString)
    {
      var stb = new StringBuilder();


      for (int i = 0; i < escString.Length; ++i)
      {
        if (escString[i] != '\\')
        {
          stb.Append(escString[i]);
          continue;
        }
        else if ((i + 5) < escString.Length &&
                  'u' == char.ToLower(escString[i + 1]) &&
                   int.TryParse(escString.Substring(i + 2, 4), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out var charValue)
                )
        {
          stb.Append((char)charValue);
          i += 5;
          continue;
        }
        else if ((i + 1) < escString.Length)
        {
          stb.Append(escString[i]);
          i += 1;
        }
      }

      return stb.ToString();
    }
  } // end class TreeWalker
}
