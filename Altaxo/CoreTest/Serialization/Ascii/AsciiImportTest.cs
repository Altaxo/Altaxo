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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Altaxo.Serialization.Ascii
{
  public class AsciiImportTest
  {
    public class AsciiTestInputBase
    {
      public List<string> LineList = new List<string>();
      public List<List<string>> TokenList = new List<List<string>>();

      public int MaxLines = 40;
      public int MaxColumns = 4;
      public System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;

      public string AsciiText
      {
        get
        {
          return string.Join("\r\n", LineList);
        }
      }

      public void StandardTest(string testComment)
      {
        // now test this document
        var options = AsciiDocumentAnalysisOptions.GetDefaultSystemOptions();
        var bytes = System.Text.Encoding.UTF8.GetBytes(AsciiText);
        var analysis = AsciiDocumentAnalysis.Analyze(null, new System.IO.MemoryStream(bytes), options);
        var separation = analysis.SeparationStrategy;

        Assert.Equal(MaxLines, LineList.Count); // "Test does not produce expected number of lines");

        for (int iLine = 0; iLine < MaxLines; ++iLine)
        {
          var expectedTokens = TokenList[iLine];
          Assert.Equal(MaxColumns, expectedTokens.Count); // string.Format("Test does not produce expected number of tokens in line {0}", iLine));
          var currentTokens = separation.GetTokens(LineList[iLine]).ToArray();
          Assert.Equal(MaxColumns, currentTokens.Length); // string.Format("SeparationStrategy (of type {0}) returns too less or too many tokens in line {1}, ColumnMode={2}", separation.GetType(), iLine, testComment));

          for (int iColumn = 0; iColumn < MaxColumns; ++iColumn)
          {
            var expectedToken = expectedTokens[iColumn].Trim();
            var currentToken = currentTokens[iColumn].Trim();
            Assert.Equal(expectedToken, currentToken); // string.Format("Tokens are different in line {0}, column {1}, ColumnMode={2}, SeparationStrategy={3}", iLine, iColumn, testComment, separation.GetType()));
          }
        }

        var structure = analysis.RecognizedStructure;

        Assert.Equal(MaxColumns, structure.Count);

        for (int iColumn = 0; iColumn < MaxColumns; ++iColumn)
        {
          AsciiColumnInfo info = structure[iColumn];
          Assert.Equal(AsciiColumnType.Int64, info.ColumnType);
        }
      }

      public void SaveToCTempTestTxt()
      {
        using (var wr = System.IO.File.CreateText(@"C:\Temp\Test.txt"))
        {
          wr.Write(AsciiText);
          wr.Flush();
          wr.Close();
        }
      }

      #region Helper functions

      /// <summary>
      /// Calculates x^n by repeated multiplications. The algorithm takes ld(n) multiplications.
      /// This algorithm can also be used with negative n.
      /// </summary>
      /// <param name="x"></param>
      /// <param name="n"></param>
      /// <returns></returns>
      public static long LongTenToThePowerOf(int n)
      {
        if (n < 0)
          throw new ArgumentOutOfRangeException("n<0");

        long value = 1;
        int x = 10;

        do
        {
          if (0 != (n & 1))
            value *= x;  // for odd n

          n >>= 1;
          x *= x;
        } while (n != 0);

        return value;
      }

      public static long GetLongOfWidth(int w)
      {
        return LongTenToThePowerOf(w - 1);
      }

      #endregion Helper functions
    }

    public class AsciiTestInputForTabSpaceSeparation : AsciiTestInputBase
    {
      public int fieldWidth = 13;
      public int tabSize = 4;
      public int iColumnMode;

      public AsciiTestInputForTabSpaceSeparation(int iColumnMode)
      {
        this.iColumnMode = iColumnMode;
      }

      public void BuildAsciiText()
      {
        for (int line = 1; line <= MaxLines; ++line)
        {
          var lineBuilder = new StringBuilder();
          int currentPosition = 0;
          var globColType = iColumnMode;
          var tokens = new List<string>();
          TokenList.Add(tokens);

          for (int c = 1; c <= MaxColumns; ++c) // 4 columns
          {
            var locColType = globColType % 3;
            globColType /= 3;
            string token;

            long value;

            switch (locColType)
            {
              case 0: // leftJustified
                value = line + c + GetLongOfWidth(1 + (line + c) % (fieldWidth - 1));
                token = ToTabFilledLeftAdjusted(lineBuilder, ref currentPosition, value, Culture, fieldWidth, tabSize);
                tokens.Add(token.Trim());
                break;

              case 1: // rightJustified
                value = line + c + GetLongOfWidth(1 + (line + c) % (fieldWidth - 1));
                token = ToTabFilledRightAdjusted(lineBuilder, ref currentPosition, value, Culture, fieldWidth, tabSize);
                tokens.Add(token.Trim());
                break;

              case 2: // just fitting exactly in our cell
                value = line + c + GetLongOfWidth(fieldWidth - 1);
                token = ToTabFilledRightAdjusted(lineBuilder, ref currentPosition, value, Culture, fieldWidth, tabSize);
                tokens.Add(token.Trim());
                break;

              default:
                throw new InvalidProgramException();
            }
          }

          LineList.Add(lineBuilder.ToString());
        }
      }

      #region Helper functions

      public static string ToTabFilledLeftAdjusted(StringBuilder lineBuilder, ref int currentPosition, long x, System.Globalization.CultureInfo ci, int width, int tabSize)
      {
        int destinationPosition = currentPosition + width;
        string a = x.ToString(ci);

        if (a.Length >= width)
          throw new InvalidOperationException("width is less than number x occupies");

        if (lineBuilder.Length > 0 && !char.IsWhiteSpace(lineBuilder[lineBuilder.Length - 1]))
        {
          lineBuilder.Append(' ');
          ++currentPosition;
        }

        {
          lineBuilder.Append(a);
          currentPosition += a.Length;
        }

        FillWithTabsAndSpacesTillDestPosition(lineBuilder, ref currentPosition, destinationPosition, tabSize);

        return a;
      }

      public static string ToTabFilledRightAdjusted(StringBuilder lineBuilder, ref int currentPosition, long x, System.Globalization.CultureInfo ci, int width, int tabSize)
      {
        int destinationPosition = currentPosition + width;
        string a = x.ToString(ci);

        if (a.Length >= width)
          throw new InvalidOperationException("width is less than number x occupies");

        FillWithTabsAndSpacesTillDestPosition(lineBuilder, ref currentPosition, destinationPosition - a.Length, tabSize);

        lineBuilder.Append(a);
        currentPosition += a.Length;

        return a;
      }

      public static void FillWithTabsAndSpacesTillDestPosition(StringBuilder lineBuilder, ref int currentPosition, int destinationPosition, int tabSize)
      {
        if (tabSize > 1)
        {
          int nextPos;
          do
          {
            nextPos = tabSize * ((currentPosition + tabSize) / tabSize);

            if (nextPos <= destinationPosition)
            {
              lineBuilder.Append('\t');
              currentPosition = nextPos;
            }
          } while (nextPos < destinationPosition);
        }

        // fill up the rest with spaces
        for (int i = currentPosition; i < destinationPosition; ++i)
        {
          lineBuilder.Append(' ');
          ++currentPosition;
        }
      }

      #endregion Helper functions
    }

    public class AsciiTestInputForSingleCharSeparation : AsciiTestInputBase
    {
      public char SeparationChar = '\t';
      public int MaxFieldWidth = 13;

      public void BuildAsciiText()
      {
        for (int line = 1; line <= MaxLines; ++line)
        {
          var lineBuilder = new StringBuilder();
          var tokens = new List<string>();
          TokenList.Add(tokens);

          for (int c = 1; c <= MaxColumns; ++c) // 4 columns
          {
            long value = value = line + c + GetLongOfWidth(1 + (line + c) % (MaxFieldWidth));
            string token = value.ToString(Culture);
            lineBuilder.Append(token);
            tokens.Add(token);

            if (c < MaxColumns)
              lineBuilder.Append(SeparationChar);
          }
          LineList.Add(lineBuilder.ToString());
        }
      }
    }

    [Fact]
    public static void Test01_NoTabs()
    {
      for (int iColumnMode = 0; iColumnMode < 81; ++iColumnMode)
      {
        var ascii = new AsciiTestInputForTabSpaceSeparation(iColumnMode) { fieldWidth = 13, tabSize = 1 };
        ascii.BuildAsciiText();
        ascii.StandardTest(iColumnMode.ToString());
      }
    }

    [Fact]
    public static void Test02_TabSize4()
    {
      for (int iColumnMode = 0; iColumnMode < 81; ++iColumnMode)
      {
        var ascii = new AsciiTestInputForTabSpaceSeparation(iColumnMode) { fieldWidth = 13, tabSize = 4 };
        ascii.BuildAsciiText();
        ascii.StandardTest(iColumnMode.ToString());
      }
    }

    [Fact]
    public static void Test03_TabSize8()
    {
      for (int iColumnMode = 0; iColumnMode < 81; ++iColumnMode)
      {
        var ascii = new AsciiTestInputForTabSpaceSeparation(iColumnMode) { fieldWidth = 13, tabSize = 8 };
        ascii.BuildAsciiText();
        ascii.StandardTest(iColumnMode.ToString());
      }
    }

    [Fact]
    public static void Test04_SingleCharTab()
    {
      for (int iColumnMode = 0; iColumnMode < 81; ++iColumnMode)
      {
        var ascii = new AsciiTestInputForSingleCharSeparation { MaxFieldWidth = 11, SeparationChar = '\t' };
        ascii.BuildAsciiText();
        ascii.StandardTest(iColumnMode.ToString());
      }
    }

    [Fact]
    public static void Test05_SingleCharSemicolon()
    {
      for (int iColumnMode = 0; iColumnMode < 81; ++iColumnMode)
      {
        var ascii = new AsciiTestInputForSingleCharSeparation { MaxFieldWidth = 11, SeparationChar = ';' };
        ascii.BuildAsciiText();
        ascii.StandardTest(iColumnMode.ToString());
      }
    }

    [Fact]
    public static void Test06_SingleCharComma()
    {
      for (int iColumnMode = 0; iColumnMode < 81; ++iColumnMode)
      {
        var ascii = new AsciiTestInputForSingleCharSeparation { MaxFieldWidth = 11, SeparationChar = ',' };
        ascii.BuildAsciiText();
        ascii.StandardTest(iColumnMode.ToString());
      }
    }

    [Fact]
    public static void Test04_SingleCharTabGermanCulture()
    {
      for (int iColumnMode = 0; iColumnMode < 81; ++iColumnMode)
      {
        var ascii = new AsciiTestInputForSingleCharSeparation { MaxFieldWidth = 11, SeparationChar = '\t', Culture = System.Globalization.CultureInfo.GetCultureInfo("de") };
        ascii.BuildAsciiText();
        ascii.StandardTest(iColumnMode.ToString());
      }
    }

    [Fact]
    public static void Test05_SingleCharTabGermanCulture2()
    {
      var GermanCulture = System.Globalization.CultureInfo.GetCultureInfo("de");
      var rnd = new Random();

      var stb = new StringBuilder();

      var arr = new double[100, 2];

      for (int i = 0; i < 100; ++i)
      {
        arr[i, 0] = Math.Round(rnd.NextDouble() * 10, 3);
        arr[i, 1] = Math.Round(rnd.NextDouble() * 10, 3);
        stb.Append(arr[i, 0].ToString(GermanCulture));
        stb.Append("\t");
        stb.Append(arr[i, 0].ToString(GermanCulture));
        stb.AppendLine();
      }

      // now test this document
      var options = AsciiDocumentAnalysisOptions.GetOptionsForCultures(System.Globalization.CultureInfo.InvariantCulture, GermanCulture);
      var bytes = System.Text.Encoding.UTF8.GetBytes(stb.ToString());
      var analysis = AsciiDocumentAnalysis.Analyze(null, new System.IO.MemoryStream(bytes), options);
      var separation = analysis.SeparationStrategy;

      Assert.Equal(2, analysis.RecognizedStructure.Count);
      Assert.True(AsciiColumnType.Double == analysis.RecognizedStructure[0].ColumnType);
      Assert.True(AsciiColumnType.Double == analysis.RecognizedStructure[1].ColumnType);

      Assert.True("de" == analysis.NumberFormatCulture.TwoLetterISOLanguageName);
    }
  }
}
