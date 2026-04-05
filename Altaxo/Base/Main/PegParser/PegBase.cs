#region Copyright

/////////////////////////////////////////////////////////////////////////////
//   Author:Martin.Holzherr;Date:20080922;Context:"PEG Support for C#";Licence:CPOL
//   <<History>>
//   20080922;V1.0 created
//   20080929;UTF16BE;Added UTF16BE read support to <<FileLoader.LoadFile(out string src)>>
//   <</History>>
//
//    modified for Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    Altaxo is free software; you can redistribute it and/or modify
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

/* Changes made for Altaxo:
 * Uses of member errOut_ is secured by if not null since its not guaranted that errOut_ is always not null
 * Parameterless constructor of PegException is replaced by constructor which takes an error message, and calls to this constructor are provided with a meaningful message
*/

#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Altaxo.Main.PegParser
{
  #region Input File Support

  /// <summary>
  /// Specifies the expected encoding class of an input file.
  /// </summary>
  public enum EncodingClass
  {
    /// <summary>
    /// Expects Unicode text.
    /// </summary>
    unicode,

    /// <summary>
    /// Expects UTF-8 text.
    /// </summary>
    utf8,

    /// <summary>
    /// Expects binary data.
    /// </summary>
    binary,

    /// <summary>
    /// Expects ASCII text.
    /// </summary>
    ascii
  };

  /// <summary>
  /// Specifies how Unicode encoding should be detected.
  /// </summary>
  public enum UnicodeDetection
  {
    /// <summary>
    /// Unicode detection is not applicable.
    /// </summary>
    notApplicable,

    /// <summary>
    /// Detects Unicode by using a byte order mark.
    /// </summary>
    BOM,

    /// <summary>
    /// Detects Unicode by assuming the first character is ASCII.
    /// </summary>
    FirstCharIsAscii
  };

  /// <summary>
  /// Loads text or binary input files and determines their encoding.
  /// </summary>
  public class FileLoader
  {
    /// <summary>
    /// Represents the supported file encodings.
    /// </summary>
    public enum FileEncoding
    {
      /// <summary>
      /// No encoding could be determined.
      /// </summary>
      none,

      /// <summary>
      /// ASCII encoding.
      /// </summary>
      ascii,

      /// <summary>
      /// Binary data.
      /// </summary>
      binary,

      /// <summary>
      /// UTF-8 encoding.
      /// </summary>
      utf8,

      /// <summary>
      /// Unicode encoding.
      /// </summary>
      unicode,

      /// <summary>
      /// UTF-16 big-endian encoding.
      /// </summary>
      utf16be,

      /// <summary>
      /// UTF-16 little-endian encoding.
      /// </summary>
      utf16le,

      /// <summary>
      /// UTF-32 little-endian encoding.
      /// </summary>
      utf32le,

      /// <summary>
      /// UTF-32 big-endian encoding.
      /// </summary>
      utf32be,

      /// <summary>
      /// Unicode encoding detected by byte order mark.
      /// </summary>
      uniCodeBOM
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="FileLoader"/> class.
    /// </summary>
    /// <param name="encodingClass">The expected encoding class.</param>
    /// <param name="detection">The Unicode detection mode.</param>
    /// <param name="path">The file path.</param>
    public FileLoader(EncodingClass encodingClass, UnicodeDetection detection, string path)
    {
      encoding_ = GetEncoding(encodingClass, detection, path);
      path_ = path;
    }

    /// <summary>
    /// Determines whether the loaded file is treated as binary.
    /// </summary>
    /// <returns><c>true</c> if the file is binary; otherwise, <c>false</c>.</returns>
    public bool IsBinaryFile()
    {
      return encoding_ == FileEncoding.binary;
    }

    /// <summary>
    /// Loads the file as binary data.
    /// </summary>
    /// <param name="src">The loaded bytes.</param>
    /// <returns><c>true</c> if the file could be loaded as binary data; otherwise, <c>false</c>.</returns>
    public bool LoadFile(out byte[] src)
    {
      src = null;
      if (!IsBinaryFile())
        return false;
      using (var brdr = new BinaryReader(File.Open(path_, FileMode.Open, FileAccess.Read)))
      {
        src = brdr.ReadBytes((int)brdr.BaseStream.Length);
        return true;
      }
    }

    /// <summary>
    /// Loads the file as text.
    /// </summary>
    /// <param name="src">The loaded text.</param>
    /// <returns><c>true</c> if the file could be loaded as text; otherwise, <c>false</c>.</returns>
    public bool LoadFile(out string src)
    {
      src = null;
      Encoding textEncoding = FileEncodingToTextEncoding();
      if (textEncoding is null)
      {
        if (encoding_ == FileEncoding.binary)
          return false;
        using (var rd = new StreamReader(path_, true))
        {
          src = rd.ReadToEnd();
          return true;
        }
      }
      else
      {
        if (encoding_ == FileEncoding.utf16be)//UTF16BE
        {
          using (var brdr = new BinaryReader(File.Open(path_, FileMode.Open, FileAccess.Read)))
          {
            byte[] bytes = brdr.ReadBytes((int)brdr.BaseStream.Length);
            var s = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
              char c = (char)(bytes[i] << 8 | bytes[i + 1]);
              s.Append(c);
            }
            src = s.ToString();
            return true;
          }
        }
        else
        {
          using (var rd = new StreamReader(path_, textEncoding))
          {
            src = rd.ReadToEnd();
            return true;
          }
        }
      }
    }

    private Encoding FileEncodingToTextEncoding()
    {
      switch (encoding_)
      {
        case FileEncoding.utf8:
          return new UTF8Encoding();
        case FileEncoding.utf32be:
        case FileEncoding.utf32le:
          return new UTF32Encoding();
        case FileEncoding.unicode:
        case FileEncoding.utf16be:
        case FileEncoding.utf16le:
          return new UnicodeEncoding();
        case FileEncoding.ascii:
          return new ASCIIEncoding();
        case FileEncoding.binary:
        case FileEncoding.uniCodeBOM:
          return null;
        default:
          throw new InvalidProgramException();
      }
    }

    private static FileEncoding DetermineUnicodeWhenFirstCharIsAscii(string path)
    {
      using (var br = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)))
      {
        byte[] startBytes = br.ReadBytes(4);
        if (startBytes.Length == 0)
          return FileEncoding.none;
        if (startBytes.Length == 1 || startBytes.Length == 3)
          return FileEncoding.utf8;
        if (startBytes.Length == 2 && startBytes[0] != 0)
          return FileEncoding.utf16le;
        if (startBytes.Length == 2 && startBytes[0] == 0)
          return FileEncoding.utf16be;
        if (startBytes[0] == 0 && startBytes[1] == 0)
          return FileEncoding.utf32be;
        if (startBytes[0] == 0 && startBytes[1] != 0)
          return FileEncoding.utf16be;
        if (startBytes[0] != 0 && startBytes[1] == 0)
          return FileEncoding.utf16le;
        return FileEncoding.utf8;
      }
    }

    private FileEncoding GetEncoding(EncodingClass encodingClass, UnicodeDetection detection, string path)
    {
      switch (encodingClass)
      {
        case EncodingClass.ascii:
          return FileEncoding.ascii;
        case EncodingClass.unicode:
          {
            if (detection == UnicodeDetection.FirstCharIsAscii)
            {
              return DetermineUnicodeWhenFirstCharIsAscii(path);
            }
            else if (detection == UnicodeDetection.BOM)
            {
              return FileEncoding.uniCodeBOM;
            }
            else
              return FileEncoding.unicode;
          }
        case EncodingClass.utf8:
          return FileEncoding.utf8;
        case EncodingClass.binary:
          return FileEncoding.binary;
      }
      return FileEncoding.none;
    }

    private string path_;

    /// <summary>
    /// Gets the detected file encoding.
    /// </summary>
    public readonly FileEncoding encoding_;
  }

  #endregion Input File Support

  #region Error handling

  /// <summary>
  /// Exception that indicates a parsing error.
  /// </summary>
  public class PegException : System.Exception
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="PegException"/> class.
    /// </summary>
    /// <param name="message">The parsing error message.</param>
    public PegException(string message)
      : base("Parsing error: " + message ?? string.Empty)
    {
    }
  }

  /// <summary>
  /// Stores line information for parser error reporting.
  /// </summary>
  public struct PegError
  {
    internal SortedList<int, int> lineStarts;

    private void AddLineStarts(string s, int first, int last, ref int lineNo, out int colNo)
    {
      colNo = 2;
      for (int i = first + 1; i <= last; ++i, ++colNo)
      {
        if (s[i - 1] == '\n')
        {
          lineStarts[i] = ++lineNo;
          colNo = 1;
        }
      }
      --colNo;
    }

    /// <summary>
    /// Gets the line and column that correspond to the specified position.
    /// </summary>
    /// <param name="s">The source text.</param>
    /// <param name="pos">The position in the source text.</param>
    /// <param name="lineNo">Receives the line number.</param>
    /// <param name="colNo">Receives the column number.</param>
    public void GetLineAndCol(string s, int pos, out int lineNo, out int colNo)
    {
      for (int i = lineStarts.Count(); i > 0; --i)
      {
        KeyValuePair<int, int> curLs = lineStarts.ElementAt(i - 1);
        if (curLs.Key == pos)
        {
          lineNo = curLs.Value;
          colNo = 1;
          return;
        }
        if (curLs.Key < pos)
        {
          lineNo = curLs.Value;
          AddLineStarts(s, curLs.Key, pos, ref lineNo, out colNo);
          return;
        }
      }
      lineNo = 1;
      AddLineStarts(s, 0, pos, ref lineNo, out colNo);
    }
  }

  #endregion Error handling

  #region Syntax/Parse-Tree related classes

  /// <summary>
  /// Defines special node identifiers used by the parse tree.
  /// </summary>
  public enum ESpecialNodes
  {
    /// <summary>
    /// Identifies a fatal parser node.
    /// </summary>
    eFatal = -10001,

    /// <summary>
    /// Identifies an anonymous nonterminal node.
    /// </summary>
    eAnonymNTNode = -1000,

    /// <summary>
    /// Identifies an anonymous abstract syntax tree node.
    /// </summary>
    eAnonymASTNode = -1001,

    /// <summary>
    /// Identifies an anonymous character node.
    /// </summary>
    eAnonymousNode = -100
  }

  /// <summary>
  /// Defines the phases used when creating parse tree nodes.
  /// </summary>
  public enum ECreatorPhase
  {
    /// <summary>
    /// Creates the node.
    /// </summary>
    eCreate,

    /// <summary>
    /// Completes creation of an existing node.
    /// </summary>
    eCreationComplete,

    /// <summary>
    /// Creates and completes the node in one step.
    /// </summary>
    eCreateAndComplete
  }

  /// <summary>
  /// Represents a begin and end position in the source string.
  /// </summary>
  public struct PegBegEnd//indices into the source string
  {
    /// <summary>
    /// Gets the length of the matched range.
    /// </summary>
    public int Length
    {
      get { return posEnd_ - posBeg_; }
    }

    /// <summary>
    /// Gets the referenced substring from the specified source text.
    /// </summary>
    /// <param name="src">The source text.</param>
    /// <returns>The referenced substring.</returns>
    public string GetAsString(string src)
    {
      if (!(src.Length >= posEnd_))
        throw new InvalidProgramException();

      return src.Substring(posBeg_, Length);
    }

    /// <summary>
    /// The inclusive start position.
    /// </summary>
    public int posBeg_;

    /// <summary>
    /// The exclusive end position.
    /// </summary>
    public int posEnd_;
  }

  /// <summary>
  /// Represents a node in the parse tree.
  /// </summary>
  public class PegNode : ICloneable
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PegNode"/> class.
    /// </summary>
    /// <param name="parent">The parent node.</param>
    /// <param name="id">The node identifier.</param>
    /// <param name="match">The matched source range.</param>
    /// <param name="child">The first child node.</param>
    /// <param name="next">The next sibling node.</param>
    public PegNode(PegNode parent, int id, PegBegEnd match, PegNode child, PegNode next)
    {
      parent_ = parent;
      id_ = id;
      child_ = child;
      next_ = next;
      match_ = match;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PegNode"/> class.
    /// </summary>
    /// <param name="parent">The parent node.</param>
    /// <param name="id">The node identifier.</param>
    /// <param name="match">The matched source range.</param>
    /// <param name="child">The first child node.</param>
    public PegNode(PegNode parent, int id, PegBegEnd match, PegNode child)
      : this(parent, id, match, child, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PegNode"/> class.
    /// </summary>
    /// <param name="parent">The parent node.</param>
    /// <param name="id">The node identifier.</param>
    /// <param name="match">The matched source range.</param>
    public PegNode(PegNode parent, int id, PegBegEnd match)
      : this(parent, id, match, null, null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PegNode"/> class.
    /// </summary>
    /// <param name="parent">The parent node.</param>
    /// <param name="id">The node identifier.</param>
    public PegNode(PegNode parent, int id)
      : this(parent, id, new PegBegEnd(), null, null)
    {
    }

    #endregion Constructors

    #region Public Members

    /// <summary>
    /// Gets the matched source text for this node.
    /// </summary>
    /// <param name="s">The full source text.</param>
    /// <returns>The matched source text.</returns>
    public virtual string GetAsString(string s)
    {
      return match_.GetAsString(s);
    }

    /// <summary>
    /// Creates a copy of this node and its descendants.
    /// </summary>
    /// <returns>A cloned node.</returns>
    public virtual PegNode Clone()
    {
      var clone = new PegNode(parent_, id_, match_);
      CloneSubTrees(clone);
      return clone;
    }

    #endregion Public Members

    #region Protected Members

    /// <summary>
    /// Clones the child and sibling subtrees into the specified node.
    /// </summary>
    /// <param name="clone">The node that receives the cloned subtrees.</param>
    protected void CloneSubTrees(PegNode clone)
    {
      PegNode child = null, next = null;
      if (child_ is not null)
      {
        child = child_.Clone();
        child.parent_ = clone;
      }
      if (next_ is not null)
      {
        next = next_.Clone();
        next.parent_ = clone;
      }
      clone.child_ = child;
      clone.next_ = next;
    }

    #endregion Protected Members

    #region Data Members

    /// <summary>
    /// The node identifier.
    /// </summary>
    public int id_;

    /// <summary>
    /// The parent node.
    /// </summary>
    public PegNode parent_;

    /// <summary>
    /// The first child node.
    /// </summary>
    public PegNode child_;

    /// <summary>
    /// The next sibling node.
    /// </summary>
    public PegNode next_;

    /// <summary>
    /// The matched source range.
    /// </summary>
    public PegBegEnd match_;

    #endregion Data Members

    #region ICloneable Members

    /// <inheritdoc/>
    object ICloneable.Clone()
    {
      return Clone();
    }

    #endregion ICloneable Members
  }

  internal struct PegTree
  {
    internal enum AddPolicy { eAddAsChild, eAddAsSibling };

    internal PegNode root_;
    internal PegNode cur_;
    internal AddPolicy addPolicy;
  }

  /// <summary>
  /// Defines the operations required to print a parse tree node.
  /// </summary>
  public abstract class PrintNode
  {
    /// <summary>
    /// Gets the maximum line length used for printing.
    /// </summary>
    /// <returns>The maximum line length.</returns>
    public abstract int LenMaxLine();

    /// <summary>
    /// Determines whether the specified node is a leaf.
    /// </summary>
    /// <param name="p">The node to inspect.</param>
    /// <returns><c>true</c> if the node is a leaf; otherwise, <c>false</c>.</returns>
    public abstract bool IsLeaf(PegNode p);

    /// <summary>
    /// Determines whether the specified node should be skipped during printing.
    /// </summary>
    /// <param name="p">The node to inspect.</param>
    /// <returns><c>true</c> if the node should be skipped; otherwise, <c>false</c>.</returns>
    public virtual bool IsSkip(PegNode p)
    {
      return false;
    }

    /// <summary>
    /// Prints the beginning of a non-leaf node.
    /// </summary>
    /// <param name="p">The node to print.</param>
    /// <param name="bAlignVertical">Whether child nodes are aligned vertically.</param>
    /// <param name="nOffsetLineBeg">The current line offset.</param>
    /// <param name="nLevel">The current tree level.</param>
    public abstract void PrintNodeBeg(PegNode p, bool bAlignVertical, ref int nOffsetLineBeg, int nLevel);

    /// <summary>
    /// Prints the end of a non-leaf node.
    /// </summary>
    /// <param name="p">The node to print.</param>
    /// <param name="bAlignVertical">Whether child nodes are aligned vertically.</param>
    /// <param name="nOffsetLineBeg">The current line offset.</param>
    /// <param name="nLevel">The current tree level.</param>
    public abstract void PrintNodeEnd(PegNode p, bool bAlignVertical, ref int nOffsetLineBeg, int nLevel);

    /// <summary>
    /// Gets the printed length of the beginning of a non-leaf node.
    /// </summary>
    /// <param name="p">The node to inspect.</param>
    /// <returns>The printed length.</returns>
    public abstract int LenNodeBeg(PegNode p);

    /// <summary>
    /// Gets the printed length of the end of a non-leaf node.
    /// </summary>
    /// <param name="p">The node to inspect.</param>
    /// <returns>The printed length.</returns>
    public abstract int LenNodeEnd(PegNode p);

    /// <summary>
    /// Prints a leaf node.
    /// </summary>
    /// <param name="p">The node to print.</param>
    /// <param name="nOffsetLineBeg">The current line offset.</param>
    /// <param name="bAlignVertical">Whether output is vertically aligned.</param>
    public abstract void PrintLeaf(PegNode p, ref int nOffsetLineBeg, bool bAlignVertical);

    /// <summary>
    /// Gets the printed length of a leaf node.
    /// </summary>
    /// <param name="p">The node to inspect.</param>
    /// <returns>The printed length.</returns>
    public abstract int LenLeaf(PegNode p);

    /// <summary>
    /// Gets the printed distance to the next node.
    /// </summary>
    /// <param name="p">The current node.</param>
    /// <param name="bAlignVertical">Whether output is vertically aligned.</param>
    /// <param name="nOffsetLineBeg">The current line offset.</param>
    /// <param name="nLevel">The current tree level.</param>
    /// <returns>The printed distance.</returns>
    public abstract int LenDistNext(PegNode p, bool bAlignVertical, ref int nOffsetLineBeg, int nLevel);

    /// <summary>
    /// Prints the separator to the next node.
    /// </summary>
    /// <param name="p">The current node.</param>
    /// <param name="bAlignVertical">Whether output is vertically aligned.</param>
    /// <param name="nOffsetLineBeg">The current line offset.</param>
    /// <param name="nLevel">The current tree level.</param>
    public abstract void PrintDistNext(PegNode p, bool bAlignVertical, ref int nOffsetLineBeg, int nLevel);
  }

  /// <summary>
  /// Prints parse trees in a textual representation.
  /// </summary>
  public class TreePrint : PrintNode
  {
    #region Data Members

    /// <summary>
    /// Resolves the display name for a node.
    /// </summary>
    /// <param name="node">The node to name.</param>
    /// <returns>The display name.</returns>
    public delegate string GetNodeName(PegNode node);

    private string src_;
    private TextWriter treeOut_;
    private int nMaxLineLen_;
    private bool bVerbose_;
    private GetNodeName GetNodeName_;

    #endregion Data Members

    #region Methods

    /// <summary>
    /// Initializes a new instance of the <see cref="TreePrint"/> class.
    /// </summary>
    /// <param name="treeOut">The text writer that receives the output.</param>
    /// <param name="src">The source text.</param>
    /// <param name="nMaxLineLen">The maximum output line length.</param>
    /// <param name="GetNodeName">The callback used to resolve node names.</param>
    /// <param name="bVerbose">Whether verbose output should be generated.</param>
    public TreePrint(TextWriter treeOut, string src, int nMaxLineLen, GetNodeName GetNodeName, bool bVerbose)
    {
      treeOut_ = treeOut;
      nMaxLineLen_ = nMaxLineLen;
      bVerbose_ = bVerbose;
      GetNodeName_ = GetNodeName;
      src_ = src;
    }

    /// <summary>
    /// Prints the specified tree.
    /// </summary>
    /// <param name="parent">The root node to print.</param>
    /// <param name="nOffsetLineBeg">The indentation offset.</param>
    /// <param name="nLevel">The current tree level.</param>
    public void PrintTree(PegNode parent, int nOffsetLineBeg, int nLevel)
    {
      if (IsLeaf(parent))
      {
        PrintLeaf(parent, ref nOffsetLineBeg, false);
        treeOut_.Flush();
        return;
      }
      bool bAlignVertical =
          DetermineLineLength(parent, nOffsetLineBeg) > LenMaxLine();
      PrintNodeBeg(parent, bAlignVertical, ref nOffsetLineBeg, nLevel);
      int nOffset = nOffsetLineBeg;
      for (PegNode p = parent.child_; p is not null; p = p.next_)
      {
        if (IsSkip(p))
          continue;

        if (IsLeaf(p))
        {
          PrintLeaf(p, ref nOffsetLineBeg, bAlignVertical);
        }
        else
        {
          PrintTree(p, nOffsetLineBeg, nLevel + 1);
        }
        if (bAlignVertical)
        {
          nOffsetLineBeg = nOffset;
        }
        while (p.next_ is not null && IsSkip(p.next_))
          p = p.next_;

        if (p.next_ is not null)
        {
          PrintDistNext(p, bAlignVertical, ref nOffsetLineBeg, nLevel);
        }
      }
      PrintNodeEnd(parent, bAlignVertical, ref nOffsetLineBeg, nLevel);
      treeOut_.Flush();
    }

    private int DetermineLineLength(PegNode parent, int nOffsetLineBeg)
    {
      int nLen = LenNodeBeg(parent);
      PegNode p;
      for (p = parent.child_; p is not null; p = p.next_)
      {
        if (IsSkip(p))
          continue;
        if (IsLeaf(p))
        {
          nLen += LenLeaf(p);
        }
        else
        {
          nLen += DetermineLineLength(p, nOffsetLineBeg);
        }
        if (nLen + nOffsetLineBeg > LenMaxLine())
        {
          return nLen + nOffsetLineBeg;
        }
      }
      nLen += LenNodeEnd(p);
      return nLen;
    }

    /// <inheritdoc/>
    public override int LenMaxLine()
    {
      return nMaxLineLen_;
    }

    /// <inheritdoc/>
    public override void
        PrintNodeBeg(PegNode p, bool bAlignVertical, ref int nOffsetLineBeg, int nLevel)
    {
      PrintIdAsName(p);
      treeOut_.Write("<");
      if (bAlignVertical)
      {
        treeOut_.WriteLine();
        treeOut_.Write(new string(' ', nOffsetLineBeg += 2));
      }
      else
      {
        ++nOffsetLineBeg;
      }
    }

    /// <inheritdoc/>
    public override void
        PrintNodeEnd(PegNode p, bool bAlignVertical, ref int nOffsetLineBeg, int nLevel)
    {
      if (bAlignVertical)
      {
        treeOut_.WriteLine();
        treeOut_.Write(new string(' ', nOffsetLineBeg -= 2));
      }
      treeOut_.Write('>');
      if (!bAlignVertical)
      {
        ++nOffsetLineBeg;
      }
    }

    /// <inheritdoc/>
    public override int LenNodeBeg(PegNode p)
    {
      return LenIdAsName(p) + 1;
    }

    /// <inheritdoc/>
    public override int LenNodeEnd(PegNode p)
    {
      return 1;
    }

    /// <inheritdoc/>
    public override void PrintLeaf(PegNode p, ref int nOffsetLineBeg, bool bAlignVertical)
    {
      if (bVerbose_)
      {
        PrintIdAsName(p);
        treeOut_.Write('<');
      }
      int len = p.match_.posEnd_ - p.match_.posBeg_;
      treeOut_.Write("'");
      if (len > 0)
      {
        treeOut_.Write(src_.Substring(p.match_.posBeg_, p.match_.posEnd_ - p.match_.posBeg_));
      }
      treeOut_.Write("'");
      if (bVerbose_)
        treeOut_.Write('>');
    }

    /// <inheritdoc/>
    public override int LenLeaf(PegNode p)
    {
      int nLen = p.match_.posEnd_ - p.match_.posBeg_ + 2;
      if (bVerbose_)
        nLen += LenIdAsName(p) + 2;
      return nLen;
    }

    /// <inheritdoc/>
    public override bool IsLeaf(PegNode p)
    {
      return p.child_ is null;
    }

    /// <inheritdoc/>
    public override void
        PrintDistNext(PegNode p, bool bAlignVertical, ref int nOffsetLineBeg, int nLevel)
    {
      if (bAlignVertical)
      {
        treeOut_.WriteLine();
        treeOut_.Write(new string(' ', nOffsetLineBeg));
      }
      else
      {
        treeOut_.Write(' ');
        ++nOffsetLineBeg;
      }
    }

    /// <inheritdoc/>
    public override int
        LenDistNext(PegNode p, bool bAlignVertical, ref int nOffsetLineBeg, int nLevel)
    {
      return 1;
    }

    private int LenIdAsName(PegNode p)
    {
      string name = GetNodeName_(p);
      return name.Length;
    }

    private void PrintIdAsName(PegNode p)
    {
      string name = GetNodeName_(p);
      treeOut_.Write(name);
    }

    #endregion Methods
  }

  #endregion Syntax/Parse-Tree related classes

  #region Parsers

  /// <summary>
  /// Provides the common functionality for PEG parsers.
  /// </summary>
  public abstract class PegBaseParser
  {
    #region Data Types

    /// <summary>
    /// Represents a parser matcher function.
    /// </summary>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public delegate bool Matcher();

    /// <summary>
    /// Represents a callback that creates parse tree nodes.
    /// </summary>
    /// <param name="ePhase">The creation phase.</param>
    /// <param name="parentOrCreated">The parent node or the node being completed.</param>
    /// <param name="id">The node identifier.</param>
    /// <returns>The created node, or <see langword="null"/>.</returns>
    public delegate PegNode Creator(ECreatorPhase ePhase, PegNode parentOrCreated, int id);

    #endregion Data Types

    #region Data members

    /// <summary>
    /// The length of the current source.
    /// </summary>
    protected int srcLen_;

    /// <summary>
    /// The current parser position.
    /// </summary>
    protected int pos_;

    /// <summary>
    /// A value indicating whether tree creation is muted.
    /// </summary>
    protected bool bMute_;

    /// <summary>
    /// The writer used for parser diagnostics.
    /// </summary>
    protected TextWriter errOut_;

    /// <summary>
    /// The callback used to create tree nodes.
    /// </summary>
    protected Creator nodeCreator_;
    private PegTree tree;

    #endregion Data members

    /// <summary>
    /// Gets the rule name for the specified identifier.
    /// </summary>
    /// <param name="id">The rule identifier.</param>
    /// <returns>The rule name.</returns>
    public virtual string GetRuleNameFromId(int id)
    {//normally overridden
      switch (id)
      {
        case (int)ESpecialNodes.eFatal:
          return "FATAL";
        case (int)ESpecialNodes.eAnonymNTNode:
          return "Nonterminal";
        case (int)ESpecialNodes.eAnonymASTNode:
          return "ASTNode";
        case (int)ESpecialNodes.eAnonymousNode:
          return "Node";
        default:
          return id.ToString();
      }
    }

    /// <summary>
    /// Gets the file loading properties expected by the parser.
    /// </summary>
    /// <param name="encoding">Receives the expected encoding class.</param>
    /// <param name="detection">Receives the Unicode detection strategy.</param>
    public virtual void GetProperties(out EncodingClass encoding, out UnicodeDetection detection)
    {
      encoding = EncodingClass.ascii;
      detection = UnicodeDetection.notApplicable;
    }

    /// <summary>
    /// Creates a default parse tree node.
    /// </summary>
    /// <param name="phase">The creation phase.</param>
    /// <param name="parentOrCreated">The parent node or created node.</param>
    /// <param name="id">The node identifier.</param>
    /// <returns>The created node, or <see langword="null"/> when no node is created.</returns>
    protected PegNode DefaultNodeCreator(ECreatorPhase phase, PegNode parentOrCreated, int id)
    {
      if (phase == ECreatorPhase.eCreate || phase == ECreatorPhase.eCreateAndComplete)
        return new PegNode(parentOrCreated, id);
      else
        return null;
    }

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PegBaseParser"/> class.
    /// </summary>
    /// <param name="errOut">The writer used for parser diagnostics.</param>
    public PegBaseParser(TextWriter errOut)
    {
      srcLen_ = pos_ = 0;
      errOut_ = errOut;
      nodeCreator_ = DefaultNodeCreator;
    }

    #endregion Constructors

    #region Reinitialization, TextWriter access,Tree Access

    /// <summary>
    /// Reinitializes the parser.
    /// </summary>
    /// <param name="Fout">The writer used for parser diagnostics.</param>
    public void Construct(TextWriter Fout)
    {
      srcLen_ = pos_ = 0;
      bMute_ = false;
      SetErrorDestination(Fout);
      ResetTree();
    }

    /// <summary>
    /// Resets the current parser position to the beginning of the source.
    /// </summary>
    public void Rewind()
    {
      pos_ = 0;
    }

    /// <summary>
    /// Sets the destination for parser diagnostics.
    /// </summary>
    /// <param name="errOut">The writer used for parser diagnostics.</param>
    public void SetErrorDestination(TextWriter errOut)
    {
      errOut_ = errOut is null ? new StreamWriter(System.Console.OpenStandardError())
          : errOut;
    }

    #endregion Reinitialization, TextWriter access,Tree Access

    #region Tree root access, Tree Node generation/display

    /// <summary>
    /// Gets the root node of the parse tree.
    /// </summary>
    /// <returns>The root node, or <see langword="null"/>.</returns>
    public PegNode GetRoot()
    {
      return tree.root_;
    }

    /// <summary>
    /// Clears the parse tree.
    /// </summary>
    public void ResetTree()
    {
      tree.root_ = null;
      tree.cur_ = null;
      tree.addPolicy = PegTree.AddPolicy.eAddAsChild;
    }

    private void AddTreeNode(int nId, PegTree.AddPolicy newAddPolicy, Creator createNode, ECreatorPhase ePhase)
    {
      if (bMute_)
        return;
      if (tree.root_ is null)
      {
        tree.root_ = tree.cur_ = createNode(ePhase, tree.cur_, nId);
      }
      else if (tree.addPolicy == PegTree.AddPolicy.eAddAsChild)
      {
        tree.cur_ = tree.cur_.child_ = createNode(ePhase, tree.cur_, nId);
      }
      else
      {
        tree.cur_ = tree.cur_.next_ = createNode(ePhase, tree.cur_.parent_, nId);
      }
      tree.addPolicy = newAddPolicy;
    }

    private void RestoreTree(PegNode prevCur, PegTree.AddPolicy prevPolicy)
    {
      if (bMute_)
        return;
      if (prevCur is null)
      {
        tree.root_ = null;
      }
      else if (prevPolicy == PegTree.AddPolicy.eAddAsChild)
      {
        prevCur.child_ = null;
      }
      else
      {
        prevCur.next_ = null;
      }
      tree.cur_ = prevCur;
      tree.addPolicy = prevPolicy;
    }

    /// <summary>
    /// Matches character data and stores it as an anonymous tree node.
    /// </summary>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool TreeChars(Matcher toMatch)
    {
      return TreeCharsWithId((int)ESpecialNodes.eAnonymousNode, toMatch);
    }

    /// <summary>
    /// Matches character data and stores it as an anonymous tree node.
    /// </summary>
    /// <param name="nodeCreator">The node creator to use.</param>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool TreeChars(Creator nodeCreator, Matcher toMatch)
    {
      return TreeCharsWithId(nodeCreator, (int)ESpecialNodes.eAnonymousNode, toMatch);
    }

    /// <summary>
    /// Matches character data and stores it using the specified node identifier.
    /// </summary>
    /// <param name="nId">The node identifier.</param>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool TreeCharsWithId(int nId, Matcher toMatch)
    {
      return TreeCharsWithId(nodeCreator_, nId, toMatch);
    }

    /// <summary>
    /// Matches character data and stores it using the specified creator and node identifier.
    /// </summary>
    /// <param name="nodeCreator">The node creator to use.</param>
    /// <param name="nId">The node identifier.</param>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool TreeCharsWithId(Creator nodeCreator, int nId, Matcher toMatch)
    {
      int pos = pos_;
      if (toMatch())
      {
        if (!bMute_)
        {
          AddTreeNode(nId, PegTree.AddPolicy.eAddAsSibling, nodeCreator, ECreatorPhase.eCreateAndComplete);
          tree.cur_.match_.posBeg_ = pos;
          tree.cur_.match_.posEnd_ = pos_;
        }
        return true;
      }
      return false;
    }

    /// <summary>
    /// Matches a nonterminal and stores it in the parse tree.
    /// </summary>
    /// <param name="nRuleId">The rule identifier.</param>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool TreeNT(int nRuleId, Matcher toMatch)
    {
      return TreeNT(nodeCreator_, nRuleId, toMatch);
    }

    /// <summary>
    /// Matches a nonterminal and stores it in the parse tree.
    /// </summary>
    /// <param name="nodeCreator">The node creator to use.</param>
    /// <param name="nRuleId">The rule identifier.</param>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool TreeNT(Creator nodeCreator, int nRuleId, Matcher toMatch)
    {
      if (bMute_)
        return toMatch();
      PegNode prevCur = tree.cur_, ruleNode;
      PegTree.AddPolicy prevPolicy = tree.addPolicy;
      int posBeg = pos_;
      AddTreeNode(nRuleId, PegTree.AddPolicy.eAddAsChild, nodeCreator, ECreatorPhase.eCreate);
      ruleNode = tree.cur_;
      bool bMatches = toMatch();
      if (!bMatches)
        RestoreTree(prevCur, prevPolicy);
      else
      {
        ruleNode.match_.posBeg_ = posBeg;
        ruleNode.match_.posEnd_ = pos_;
        tree.cur_ = ruleNode;
        tree.addPolicy = PegTree.AddPolicy.eAddAsSibling;
        nodeCreator(ECreatorPhase.eCreationComplete, ruleNode, nRuleId);
      }
      return bMatches;
    }

    /// <summary>
    /// Matches an abstract syntax tree node and stores it in the parse tree.
    /// </summary>
    /// <param name="nRuleId">The rule identifier.</param>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool TreeAST(int nRuleId, Matcher toMatch)
    {
      return TreeAST(nodeCreator_, nRuleId, toMatch);
    }

    /// <summary>
    /// Matches an abstract syntax tree node and stores it in the parse tree.
    /// </summary>
    /// <param name="nodeCreator">The node creator to use.</param>
    /// <param name="nRuleId">The rule identifier.</param>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool TreeAST(Creator nodeCreator, int nRuleId, Matcher toMatch)
    {
      if (bMute_)
        return toMatch();
      bool bMatches = TreeNT(nodeCreator, nRuleId, toMatch);
      if (bMatches)
      {
        if (tree.cur_.child_ is not null && tree.cur_.child_.next_ is null && tree.cur_.parent_ is not null)
        {
          if (tree.cur_.parent_.child_ == tree.cur_)
          {
            tree.cur_.parent_.child_ = tree.cur_.child_;
            tree.cur_.child_.parent_ = tree.cur_.parent_;
            tree.cur_ = tree.cur_.child_;
          }
          else
          {
            PegNode prev;
            for (prev = tree.cur_.parent_.child_; prev is not null && prev.next_ != tree.cur_; prev = prev.next_)
            {
            }
            if (prev is not null)
            {
              prev.next_ = tree.cur_.child_;
              tree.cur_.child_.parent_ = tree.cur_.parent_;
              tree.cur_ = tree.cur_.child_;
            }
          }
        }
      }
      return bMatches;
    }

    /// <summary>
    /// Matches an anonymous nonterminal and stores it in the parse tree.
    /// </summary>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool TreeNT(Matcher toMatch)
    {
      return TreeNT((int)ESpecialNodes.eAnonymNTNode, toMatch);
    }

    /// <summary>
    /// Matches an anonymous nonterminal and stores it in the parse tree.
    /// </summary>
    /// <param name="nodeCreator">The node creator to use.</param>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool TreeNT(Creator nodeCreator, Matcher toMatch)
    {
      return TreeNT(nodeCreator, (int)ESpecialNodes.eAnonymNTNode, toMatch);
    }

    /// <summary>
    /// Matches an anonymous abstract syntax tree node and stores it in the parse tree.
    /// </summary>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool TreeAST(Matcher toMatch)
    {
      return TreeAST((int)ESpecialNodes.eAnonymASTNode, toMatch);
    }

    /// <summary>
    /// Matches an anonymous abstract syntax tree node and stores it in the parse tree.
    /// </summary>
    /// <param name="nodeCreator">The node creator to use.</param>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool TreeAST(Creator nodeCreator, Matcher toMatch)
    {
      return TreeAST(nodeCreator, (int)ESpecialNodes.eAnonymASTNode, toMatch);
    }

    /// <summary>
    /// Converts the specified tree node to a display string.
    /// </summary>
    /// <param name="node">The node to convert.</param>
    /// <returns>The display string.</returns>
    public virtual string TreeNodeToString(PegNode node)
    {
      return GetRuleNameFromId(node.id_);
    }

    /// <summary>
    /// Sets the callback used to create parse tree nodes.
    /// </summary>
    /// <param name="nodeCreator">The node creator callback.</param>
    public void SetNodeCreator(Creator nodeCreator)
    {
      if (nodeCreator is null)
        throw new InvalidProgramException();

      nodeCreator_ = nodeCreator;
    }

    #endregion Tree root access, Tree Node generation/display

    #region PEG  e1 e2 .. ; &e1 ; !e1 ;  e? ; e* ; e+ ; e{a,b} ; .

    /// <summary>
    /// Performs a positive lookahead with tree rollback support.
    /// </summary>
    /// <param name="pegSequence">The matcher to evaluate.</param>
    /// <returns><c>true</c> if the matcher succeeds; otherwise, <c>false</c>.</returns>
    public bool And(Matcher pegSequence)
    {
      PegNode prevCur = tree.cur_;
      PegTree.AddPolicy prevPolicy = tree.addPolicy;
      int pos0 = pos_;
      bool bMatches = pegSequence();
      if (!bMatches)
      {
        pos_ = pos0;
        RestoreTree(prevCur, prevPolicy);
      }
      return bMatches;
    }

    /// <summary>
    /// Performs a lookahead match without consuming input.
    /// </summary>
    /// <param name="toMatch">The matcher to evaluate.</param>
    /// <returns><c>true</c> if the matcher succeeds; otherwise, <c>false</c>.</returns>
    public bool Peek(Matcher toMatch)
    {
      int pos0 = pos_;
      bool prevMute = bMute_;
      bMute_ = true;
      bool bMatches = toMatch();
      bMute_ = prevMute;
      pos_ = pos0;
      return bMatches;
    }

    /// <summary>
    /// Performs a negative lookahead match without consuming input.
    /// </summary>
    /// <param name="toMatch">The matcher to evaluate.</param>
    /// <returns><c>true</c> if the matcher fails; otherwise, <c>false</c>.</returns>
    public bool Not(Matcher toMatch)
    {
      int pos0 = pos_;
      bool prevMute = bMute_;
      bMute_ = true;
      bool bMatches = toMatch();
      bMute_ = prevMute;
      pos_ = pos0;
      return !bMatches;
    }

    /// <summary>
    /// Matches one or more repetitions.
    /// </summary>
    /// <param name="toRepeat">The matcher to repeat.</param>
    /// <returns><c>true</c> if at least one repetition succeeds; otherwise, <c>false</c>.</returns>
    public bool PlusRepeat(Matcher toRepeat)
    {
      int i;
      for (i = 0; ; ++i)
      {
        int pos0 = pos_;
        if (!toRepeat())
        {
          pos_ = pos0;
          break;
        }
      }
      return i > 0;
    }

    /// <summary>
    /// Matches zero or more repetitions.
    /// </summary>
    /// <param name="toRepeat">The matcher to repeat.</param>
    /// <returns>Always <c>true</c>.</returns>
    public bool OptRepeat(Matcher toRepeat)
    {
      for (; ; )
      {
        int pos0 = pos_;
        if (!toRepeat())
        {
          pos_ = pos0;
          return true;
        }
      }
    }

    /// <summary>
    /// Optionally matches the specified expression.
    /// </summary>
    /// <param name="toMatch">The matcher to evaluate.</param>
    /// <returns>Always <c>true</c>.</returns>
    public bool Option(Matcher toMatch)
    {
      int pos0 = pos_;
      if (!toMatch())
        pos_ = pos0;
      return true;
    }

    /// <summary>
    /// Matches the specified expression an exact number of times.
    /// </summary>
    /// <param name="count">The required repetition count.</param>
    /// <param name="toRepeat">The matcher to repeat.</param>
    /// <returns><c>true</c> if all repetitions succeed; otherwise, <c>false</c>.</returns>
    public bool ForRepeat(int count, Matcher toRepeat)
    {
      PegNode prevCur = tree.cur_;
      PegTree.AddPolicy prevPolicy = tree.addPolicy;
      int pos0 = pos_;
      int i;
      for (i = 0; i < count; ++i)
      {
        if (!toRepeat())
        {
          pos_ = pos0;
          RestoreTree(prevCur, prevPolicy);
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Matches the specified expression within the provided repetition range.
    /// </summary>
    /// <param name="lower">The minimum repetition count.</param>
    /// <param name="upper">The maximum repetition count.</param>
    /// <param name="toRepeat">The matcher to repeat.</param>
    /// <returns><c>true</c> if the number of successful repetitions is within range; otherwise, <c>false</c>.</returns>
    public bool ForRepeat(int lower, int upper, Matcher toRepeat)
    {
      PegNode prevCur = tree.cur_;
      PegTree.AddPolicy prevPolicy = tree.addPolicy;
      int pos0 = pos_;
      int i;
      for (i = 0; i < upper; ++i)
      {
        if (!toRepeat())
          break;
      }
      if (i < lower)
      {
        pos_ = pos0;
        RestoreTree(prevCur, prevPolicy);
        return false;
      }
      return true;
    }

    /// <summary>
    /// Matches any single source element.
    /// </summary>
    /// <returns><c>true</c> if input was available; otherwise, <c>false</c>.</returns>
    public bool Any()
    {
      if (pos_ < srcLen_)
      {
        ++pos_;
        return true;
      }
      return false;
    }

    #endregion PEG  e1 e2 .. ; &e1 ; !e1 ;  e? ; e* ; e+ ; e{a,b} ; .
  }

  /// <summary>
  /// Provides PEG parsing support for binary input.
  /// </summary>
  public class PegByteParser : PegBaseParser
  {
    #region Data members

    /// <summary>
    /// The current binary source.
    /// </summary>
    protected byte[] src_;
    private PegError errors;

    #endregion Data members

    #region PEG optimizations

    /// <summary>
    /// Represents an optimized byte set matcher.
    /// </summary>
    public sealed class BytesetData
    {
      /// <summary>
      /// Represents an inclusive byte range.
      /// </summary>
      public struct Range
      {
        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> struct.
        /// </summary>
        /// <param name="low">The lower bound.</param>
        /// <param name="high">The upper bound.</param>
        public Range(byte low, byte high)
        {
          this.low = low;
          this.high = high;
        }

        /// <summary>
        /// The lower bound.
        /// </summary>
        public byte low;

        /// <summary>
        /// The upper bound.
        /// </summary>
        public byte high;
      }

      private System.Collections.BitArray charSet_;
      private bool bNegated_;

      /// <summary>
      /// Initializes a new instance of the <see cref="BytesetData"/> class.
      /// </summary>
      /// <param name="b">The bit set that defines the allowed bytes.</param>
      public BytesetData(System.Collections.BitArray b)
        : this(b, false)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BytesetData"/> class.
      /// </summary>
      /// <param name="b">The bit set that defines the allowed bytes.</param>
      /// <param name="bNegated">Whether the bit set should be negated.</param>
      public BytesetData(System.Collections.BitArray b, bool bNegated)
      {
        charSet_ = new System.Collections.BitArray(b);
        bNegated_ = bNegated;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BytesetData"/> class.
      /// </summary>
      /// <param name="r">The byte ranges to include.</param>
      /// <param name="c">The individual bytes to include.</param>
      public BytesetData(Range[] r, byte[] c)
        : this(r, c, false)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="BytesetData"/> class.
      /// </summary>
      /// <param name="r">The byte ranges to include.</param>
      /// <param name="c">The individual bytes to include.</param>
      /// <param name="bNegated">Whether the resulting set should be negated.</param>
      public BytesetData(Range[] r, byte[] c, bool bNegated)
      {
        int max = 0;
        if (r is not null)
          foreach (Range val in r)
            if (val.high > max)
              max = val.high;
        if (c is not null)
          foreach (int val in c)
            if (val > max)
              max = val;
        charSet_ = new System.Collections.BitArray(max + 1, false);
        if (r is not null)
        {
          foreach (Range val in r)
          {
            for (int i = val.low; i <= val.high; ++i)
            {
              charSet_[i] = true;
            }
          }
        }
        if (c is not null)
          foreach (int val in c)
            charSet_[val] = true;
        bNegated_ = bNegated;
      }

      /// <summary>
      /// Determines whether the specified byte matches the set.
      /// </summary>
      /// <param name="c">The byte to test.</param>
      /// <returns><c>true</c> if the byte matches; otherwise, <c>false</c>.</returns>
      public bool Matches(byte c)
      {
        bool bMatches = c < charSet_.Length && charSet_[c];
        if (bNegated_)
          return !bMatches;
        else
          return bMatches;
      }
    }

    /*     public class BytesetData
                 {
                         public struct Range
                         {
                                 public Range(byte low, byte high) { this.low = low; this.high = high; }
                                 public byte low;
                                 public byte high;
                         }
                         protected System.Collections.BitArray charSet_;
                         bool bNegated_;
                         public BytesetData(System.Collections.BitArray b, bool bNegated)
                         {
                                 charSet_ = new System.Collections.BitArray(b);
                                 bNegated_ = bNegated;
                         }
                         public BytesetData(byte[] c, bool bNegated)
                         {
                                 int max = 0;
                                 foreach (int val in c) if (val > max) max = val;
                                 charSet_ = new System.Collections.BitArray(max + 1, false);
                                 foreach (int val in c) charSet_[val] = true;
                                 bNegated_ = bNegated;
                         }
                         public BytesetData(Range[] r, byte[] c, bool bNegated)
                         {
                                 int max = 0;
                                 foreach (Range val in r) if (val.high > max) max = val.high;
                                 foreach (int val in c) if (val > max) max = val;
                                 charSet_ = new System.Collections.BitArray(max + 1, false);
                                 foreach (Range val in r)
                                 {
                                         for (int i = val.low; i <= val.high; ++i)
                                         {
                                                 charSet_[i] = true;
                                         }
                                 }
                                 foreach (int val in c) charSet_[val] = true;
                         }

                         public bool Matches(byte c)
                         {
                                 bool bMatches = c < charSet_.Length && charSet_[(int)c];
                                 if (bNegated_) return !bMatches;
                                 else return bMatches;
                         }
                 }*/

    #endregion PEG optimizations

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PegByteParser"/> class.
    /// </summary>
    public PegByteParser()
      : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PegByteParser"/> class.
    /// </summary>
    /// <param name="src">The initial source data.</param>
    public PegByteParser(byte[] src)
      : base(null)
    {
      SetSource(src);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PegByteParser"/> class.
    /// </summary>
    /// <param name="src">The initial source data.</param>
    /// <param name="errOut">The writer used for parser diagnostics.</param>
    public PegByteParser(byte[] src, TextWriter errOut)
      : base(errOut)
    {
      SetSource(src);
    }

    #endregion Constructors

    #region Reinitialization, Source Code access, TextWriter access,Tree Access

    /// <summary>
    /// Reinitializes the parser with the specified source and error writer.
    /// </summary>
    /// <param name="src">The source data.</param>
    /// <param name="Fout">The writer used for parser diagnostics.</param>
    public void Construct(byte[] src, TextWriter Fout)
    {
      base.Construct(Fout);
      SetSource(src);
    }

    /// <summary>
    /// Sets the current source data.
    /// </summary>
    /// <param name="src">The source data.</param>
    public void SetSource(byte[] src)
    {
      if (src is null)
        src = new byte[0];
      src_ = src;
      srcLen_ = src.Length;
      errors.lineStarts = new SortedList<int, int>
      {
        [0] = 1
      };
    }

    /// <summary>
    /// Gets the current source data.
    /// </summary>
    /// <returns>The current source data.</returns>
    public byte[] GetSource()
    {
      return src_;
    }

    #endregion Reinitialization, Source Code access, TextWriter access,Tree Access

    #region Setting host variables

    /// <summary>
    /// Captures the bytes matched by the specified matcher.
    /// </summary>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <param name="into">Receives the matched bytes.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool Into(Matcher toMatch, out byte[] into)
    {
      int pos = pos_;
      if (toMatch())
      {
        int nLen = pos_ - pos;
        into = new byte[nLen];
        for (int i = 0; i < nLen; ++i)
        {
          into[i] = src_[i + pos];
        }
        return true;
      }
      else
      {
        into = null;
        return false;
      }
    }

    /// <summary>
    /// Captures the source range matched by the specified matcher.
    /// </summary>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <param name="begEnd">Receives the matched range.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool Into(Matcher toMatch, out PegBegEnd begEnd)
    {
      begEnd.posBeg_ = pos_;
      bool bMatches = toMatch();
      begEnd.posEnd_ = pos_;
      return bMatches;
    }

    /// <summary>
    /// Captures the bytes matched by the specified matcher and converts them to an integer.
    /// </summary>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <param name="into">Receives the converted integer.</param>
    /// <returns><c>true</c> if the match and conversion succeed; otherwise, <c>false</c>.</returns>
    public bool Into(Matcher toMatch, out int into)
    {
      into = 0;
      if (!Into(toMatch, out byte[] s))
        return false;
      into = 0;
      for (int i = 0; i < s.Length; ++i)
      {
        into <<= 8;
        into |= s[i];
      }
      return true;
    }

    /// <summary>
    /// Captures the bytes matched by the specified matcher and converts them to a double.
    /// </summary>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <param name="into">Receives the converted double value.</param>
    /// <returns><c>true</c> if the match and conversion succeed; otherwise, <c>false</c>.</returns>
    public bool Into(Matcher toMatch, out double into)
    {
      into = 0.0;
      if (!Into(toMatch, out byte[] s))
        return false;
      System.Text.Encoding encoding = System.Text.Encoding.UTF8;
      string sAsString = encoding.GetString(s);
      if (!double.TryParse(sAsString, out into))
        return false;
      return true;
    }

    /// <summary>
    /// Reads the specified bit range into an integer.
    /// </summary>
    /// <param name="lowBitNo">The first bit number.</param>
    /// <param name="highBitNo">The last bit number.</param>
    /// <param name="into">Receives the extracted value.</param>
    /// <returns><c>true</c> if a byte was available; otherwise, <c>false</c>.</returns>
    public bool BitsInto(int lowBitNo, int highBitNo, out int into)
    {
      if (pos_ < srcLen_)
      {
        into = (src_[pos_] >> (lowBitNo - 1)) & ((1 << highBitNo) - 1);
        ++pos_;
        return true;
      }
      into = 0;
      return false;
    }

    /// <summary>
    /// Reads the specified bit range into an integer and tests it against a byte set.
    /// </summary>
    /// <param name="lowBitNo">The first bit number.</param>
    /// <param name="highBitNo">The last bit number.</param>
    /// <param name="toMatch">The byte set to test.</param>
    /// <param name="into">Receives the extracted value.</param>
    /// <returns><c>true</c> if the extracted value matches; otherwise, <c>false</c>.</returns>
    public bool BitsInto(int lowBitNo, int highBitNo, BytesetData toMatch, out int into)
    {
      if (pos_ < srcLen_)
      {
        byte value = (byte)((src_[pos_] >> (lowBitNo - 1)) & ((1 << highBitNo) - 1));
        ++pos_;
        into = value;
        return toMatch.Matches(value);
      }
      into = 0;
      return false;
    }

    #endregion Setting host variables

    #region Error handling

    private void LogOutMsg(string sErrKind, string sMsg)
    {
      if (errOut_ is not null)
      {
        errOut_.WriteLine("<{0}>{1}:{2}", pos_, sErrKind, sMsg);
        errOut_.Flush();
      }
    }

    /// <summary>
    /// Reports a fatal parser error.
    /// </summary>
    /// <param name="sMsg">The error message.</param>
    /// <returns>This method does not return.</returns>
    public virtual bool Fatal(string sMsg)
    {
      LogOutMsg("FATAL", sMsg);
      throw new PegException(sMsg);
    }

    /// <summary>
    /// Reports a parser warning.
    /// </summary>
    /// <param name="sMsg">The warning message.</param>
    /// <returns>Always <c>true</c>.</returns>
    public bool Warning(string sMsg)
    {
      LogOutMsg("WARNING", sMsg);
      return true;
    }

    #endregion Error handling

    #region PEG Bit level equivalents for PEG e1 ; &e1 ; !e1; e1:into ;

    /// <summary>
    /// Matches the specified bit range against an exact value.
    /// </summary>
    public bool Bits(int lowBitNo, int highBitNo, byte toMatch)
    {
      if (pos_ < srcLen_ && ((src_[pos_] >> (lowBitNo - 1)) & ((1 << highBitNo) - 1)) == toMatch)
      {
        ++pos_;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Matches the specified bit range against a byte set.
    /// </summary>
    public bool Bits(int lowBitNo, int highBitNo, BytesetData toMatch)
    {
      if (pos_ < srcLen_)
      {
        byte value = (byte)((src_[pos_] >> (lowBitNo - 1)) & ((1 << highBitNo) - 1));
        ++pos_;
        return toMatch.Matches(value);
      }
      return false;
    }

    /// <summary>
    /// Tests whether the specified bit range matches an exact value without consuming input.
    /// </summary>
    public bool PeekBits(int lowBitNo, int highBitNo, byte toMatch)
    {
      return pos_ < srcLen_ && ((src_[pos_] >> (lowBitNo - 1)) & ((1 << highBitNo) - 1)) == toMatch;
    }

    /// <summary>
    /// Tests whether the specified bit range does not match an exact value without consuming input.
    /// </summary>
    public bool NotBits(int lowBitNo, int highBitNo, byte toMatch)
    {
      return !(pos_ < srcLen_ && ((src_[pos_] >> (lowBitNo - 1)) & ((1 << highBitNo) - 1)) == toMatch);
    }

    /// <summary>
    /// Reads the specified bit range into an integer.
    /// </summary>
    public bool IntoBits(int lowBitNo, int highBitNo, out int val)
    {
      return BitsInto(lowBitNo, highBitNo, out val);
    }

    /// <summary>
    /// Reads the specified bit range into an integer and tests it against a byte set.
    /// </summary>
    public bool IntoBits(int lowBitNo, int highBitNo, BytesetData toMatch, out int val)
    {
      return BitsInto(lowBitNo, highBitNo, out val);
    }

    /// <summary>
    /// Matches the specified single bit against an exact value.
    /// </summary>
    public bool Bit(int bitNo, byte toMatch)
    {
      if (pos_ < srcLen_ && ((src_[pos_] >> (bitNo - 1)) & 1) == toMatch)
      {
        ++pos_;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Tests whether the specified single bit matches an exact value without consuming input.
    /// </summary>
    public bool PeekBit(int bitNo, byte toMatch)
    {
      return pos_ < srcLen_ && ((src_[pos_] >> (bitNo - 1)) & 1) == toMatch;
    }

    /// <summary>
    /// Tests whether the specified single bit does not match an exact value without consuming input.
    /// </summary>
    public bool NotBit(int bitNo, byte toMatch)
    {
      return !(pos_ < srcLen_ && ((src_[pos_] >> (bitNo - 1)) & 1) == toMatch);
    }

    #endregion PEG Bit level equivalents for PEG e1 ; &e1 ; !e1; e1:into ;

    #region PEG '<Literal>' / '<Literal>'/i / [low1-high1,low2-high2..] / [<CharList>]

    /// <summary>
    /// Matches the specified byte literal.
    /// </summary>
    public bool Char(byte c1)
    {
      if (pos_ < srcLen_ && src_[pos_] == c1)
      { ++pos_; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified two-byte literal.
    /// </summary>
    public bool Char(byte c1, byte c2)
    {
      if (pos_ + 1 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2)
      { pos_ += 2; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified three-byte literal.
    /// </summary>
    public bool Char(byte c1, byte c2, byte c3)
    {
      if (pos_ + 2 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2
          && src_[pos_ + 2] == c3)
      { pos_ += 3; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified four-byte literal.
    /// </summary>
    public bool Char(byte c1, byte c2, byte c3, byte c4)
    {
      if (pos_ + 3 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2
          && src_[pos_ + 2] == c3
          && src_[pos_ + 3] == c4)
      { pos_ += 4; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified five-byte literal.
    /// </summary>
    public bool Char(byte c1, byte c2, byte c3, byte c4, byte c5)
    {
      if (pos_ + 4 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2
          && src_[pos_ + 2] == c3
          && src_[pos_ + 3] == c4
          && src_[pos_ + 4] == c5)
      { pos_ += 5; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified six-byte literal.
    /// </summary>
    public bool Char(byte c1, byte c2, byte c3, byte c4, byte c5, byte c6)
    {
      if (pos_ + 5 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2
          && src_[pos_ + 2] == c3
          && src_[pos_ + 3] == c4
          && src_[pos_ + 4] == c5
          && src_[pos_ + 5] == c6)
      { pos_ += 6; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified seven-byte literal.
    /// </summary>
    public bool Char(byte c1, byte c2, byte c3, byte c4, byte c5, byte c6, byte c7)
    {
      if (pos_ + 6 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2
          && src_[pos_ + 2] == c3
          && src_[pos_ + 3] == c4
          && src_[pos_ + 4] == c5
          && src_[pos_ + 5] == c6
          && src_[pos_ + 6] == c7)
      { pos_ += 7; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified eight-byte literal.
    /// </summary>
    public bool Char(byte c1, byte c2, byte c3, byte c4, byte c5, byte c6, byte c7, byte c8)
    {
      if (pos_ + 7 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2
          && src_[pos_ + 2] == c3
          && src_[pos_ + 3] == c4
          && src_[pos_ + 4] == c5
          && src_[pos_ + 5] == c6
          && src_[pos_ + 6] == c7
          && src_[pos_ + 7] == c8)
      { pos_ += 8; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified byte sequence.
    /// </summary>
    public bool Char(byte[] s)
    {
      int sLength = s.Length;
      if (pos_ + sLength > srcLen_)
        return false;
      for (int i = 0; i < sLength; ++i)
      {
        if (s[i] != src_[pos_ + i])
          return false;
      }
      pos_ += sLength;
      return true;
    }

    /// <summary>
    /// Converts an ASCII lowercase byte to uppercase.
    /// </summary>
    public static byte ToUpper(byte c)
    {
      if (c >= 97 && c <= 122)
        return (byte)(c - 32);
      else
        return c;
    }

    /// <summary>
    /// Matches the specified byte literal case-insensitively.
    /// </summary>
    public bool IChar(byte c1)
    {
      if (pos_ < srcLen_ && ToUpper(src_[pos_]) == c1)
      { ++pos_; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified two-byte literal case-insensitively.
    /// </summary>
    public bool IChar(byte c1, byte c2)
    {
      if (pos_ + 1 < srcLen_
          && ToUpper(src_[pos_]) == ToUpper(c1)
          && ToUpper(src_[pos_ + 1]) == ToUpper(c2))
      { pos_ += 2; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified three-byte literal case-insensitively.
    /// </summary>
    public bool IChar(byte c1, byte c2, byte c3)
    {
      if (pos_ + 2 < srcLen_
          && ToUpper(src_[pos_]) == ToUpper(c1)
          && ToUpper(src_[pos_ + 1]) == ToUpper(c2)
          && ToUpper(src_[pos_ + 2]) == ToUpper(c3))
      { pos_ += 3; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified four-byte literal case-insensitively.
    /// </summary>
    public bool IChar(byte c1, byte c2, byte c3, byte c4)
    {
      if (pos_ + 3 < srcLen_
          && ToUpper(src_[pos_]) == ToUpper(c1)
          && ToUpper(src_[pos_ + 1]) == ToUpper(c2)
          && ToUpper(src_[pos_ + 2]) == ToUpper(c3)
          && ToUpper(src_[pos_ + 3]) == ToUpper(c4))
      { pos_ += 4; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified five-byte literal case-insensitively.
    /// </summary>
    public bool IChar(byte c1, byte c2, byte c3, byte c4, byte c5)
    {
      if (pos_ + 4 < srcLen_
          && ToUpper(src_[pos_]) == ToUpper(c1)
          && ToUpper(src_[pos_ + 1]) == ToUpper(c2)
          && ToUpper(src_[pos_ + 2]) == ToUpper(c3)
          && ToUpper(src_[pos_ + 3]) == ToUpper(c4)
          && ToUpper(src_[pos_ + 4]) == ToUpper(c5))
      { pos_ += 5; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified six-byte literal case-insensitively.
    /// </summary>
    public bool IChar(byte c1, byte c2, byte c3, byte c4, byte c5, byte c6)
    {
      if (pos_ + 5 < srcLen_
          && ToUpper(src_[pos_]) == ToUpper(c1)
          && ToUpper(src_[pos_ + 1]) == ToUpper(c2)
          && ToUpper(src_[pos_ + 2]) == ToUpper(c3)
          && ToUpper(src_[pos_ + 3]) == ToUpper(c4)
          && ToUpper(src_[pos_ + 4]) == ToUpper(c5)
          && ToUpper(src_[pos_ + 5]) == ToUpper(c6))
      { pos_ += 6; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified seven-byte literal case-insensitively.
    /// </summary>
    public bool IChar(byte c1, byte c2, byte c3, byte c4, byte c5, byte c6, byte c7)
    {
      if (pos_ + 6 < srcLen_
          && ToUpper(src_[pos_]) == ToUpper(c1)
          && ToUpper(src_[pos_ + 1]) == ToUpper(c2)
          && ToUpper(src_[pos_ + 2]) == ToUpper(c3)
          && ToUpper(src_[pos_ + 3]) == ToUpper(c4)
          && ToUpper(src_[pos_ + 4]) == ToUpper(c5)
          && ToUpper(src_[pos_ + 5]) == ToUpper(c6)
          && ToUpper(src_[pos_ + 6]) == ToUpper(c7))
      { pos_ += 7; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified byte sequence case-insensitively.
    /// </summary>
    public bool IChar(byte[] s)
    {
      int sLength = s.Length;
      if (pos_ + sLength > srcLen_)
        return false;
      for (int i = 0; i < sLength; ++i)
      {
        if (s[i] != ToUpper(src_[pos_ + i]))
          return false;
      }
      pos_ += sLength;
      return true;
    }

    /// <summary>
    /// Matches a byte within the specified inclusive range.
    /// </summary>
    public bool In(byte c0, byte c1)
    {
      if (pos_ < srcLen_
          && src_[pos_] >= c0 && src_[pos_] <= c1)
      {
        ++pos_;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Matches a byte within either of the specified inclusive ranges.
    /// </summary>
    public bool In(byte c0, byte c1, byte c2, byte c3)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        if (c >= c0 && c <= c1
            || c >= c2 && c <= c3)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches a byte within any of the specified inclusive ranges.
    /// </summary>
    public bool In(byte c0, byte c1, byte c2, byte c3, byte c4, byte c5)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        if (c >= c0 && c <= c1
            || c >= c2 && c <= c3
            || c >= c4 && c <= c5)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches a byte within any of the specified inclusive ranges.
    /// </summary>
    public bool In(byte c0, byte c1, byte c2, byte c3, byte c4, byte c5, byte c6, byte c7)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        if (c >= c0 && c <= c1
            || c >= c2 && c <= c3
            || c >= c4 && c <= c5
            || c >= c6 && c <= c7)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches a byte within any of the inclusive ranges defined by the array.
    /// </summary>
    public bool In(byte[] s)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        for (int i = 0; i < s.Length - 1; i += 2)
        {
          if (c >= s[i] && c <= s[i + 1])
          {
            ++pos_;
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Matches a byte outside all inclusive ranges defined by the array.
    /// </summary>
    public bool NotIn(byte[] s)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        for (int i = 0; i < s.Length - 1; i += 2)
        {
          if (c >= s[i] && c <= s[i + 1])
            return false;
        }
        ++pos_;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified bytes.
    /// </summary>
    public bool OneOf(byte c0, byte c1)
    {
      if (pos_ < srcLen_
          && (src_[pos_] == c0 || src_[pos_] == c1))
      {
        ++pos_;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified bytes.
    /// </summary>
    public bool OneOf(byte c0, byte c1, byte c2)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        if (c == c0 || c == c1 || c == c2)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified bytes.
    /// </summary>
    public bool OneOf(byte c0, byte c1, byte c2, byte c3)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        if (c == c0 || c == c1 || c == c2 || c == c3)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified bytes.
    /// </summary>
    public bool OneOf(byte c0, byte c1, byte c2, byte c3, byte c4)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        if (c == c0 || c == c1 || c == c2 || c == c3 || c == c4)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified bytes.
    /// </summary>
    public bool OneOf(byte c0, byte c1, byte c2, byte c3, byte c4, byte c5)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        if (c == c0 || c == c1 || c == c2 || c == c3 || c == c4 || c == c5)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified bytes.
    /// </summary>
    public bool OneOf(byte c0, byte c1, byte c2, byte c3, byte c4, byte c5, byte c6)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        if (c == c0 || c == c1 || c == c2 || c == c3 || c == c4 || c == c5 || c == c6)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified bytes.
    /// </summary>
    public bool OneOf(byte c0, byte c1, byte c2, byte c3, byte c4, byte c5, byte c6, byte c7)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        if (c == c0 || c == c1 || c == c2 || c == c3 || c == c4 || c == c5 || c == c6 || c == c7)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches any byte contained in the specified array.
    /// </summary>
    public bool OneOf(byte[] s)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        for (int i = 0; i < s.Length; ++i)
        {
          if (c == s[i])
          { ++pos_; return true; }
        }
      }
      return false;
    }

    /// <summary>
    /// Matches any byte not contained in the specified array.
    /// </summary>
    public bool NotOneOf(byte[] s)
    {
      if (pos_ < srcLen_)
      {
        byte c = src_[pos_];
        for (int i = 0; i < s.Length; ++i)
        {
          if (c == s[i])
          { return false; }
        }
        return true;
      }
      return false;
    }

    /// <summary>
    /// Matches any byte contained in the specified optimized byte set.
    /// </summary>
    public bool OneOf(BytesetData bset)
    {
      if (pos_ < srcLen_ && bset.Matches(src_[pos_]))
      {
        ++pos_;
        return true;
      }
      return false;
    }

    #endregion PEG '<Literal>' / '<Literal>'/i / [low1-high1,low2-high2..] / [<CharList>]
  }

  /// <summary>
  /// Provides PEG parsing support for character input.
  /// </summary>
  public class PegCharParser : PegBaseParser
  {
    #region Data members

    /// <summary>
    /// The current character source.
    /// </summary>
    protected string src_;
    private PegError errors;

    #endregion Data members

    #region PEG optimizations

    /// <summary>
    /// Represents an optimized character set matcher.
    /// </summary>
    public sealed class OptimizedCharset
    {
      /// <summary>
      /// Represents an inclusive character range.
      /// </summary>
      public struct Range
      {
        /// <summary>
        /// Initializes a new instance of the <see cref="Range"/> struct.
        /// </summary>
        /// <param name="low">The lower bound.</param>
        /// <param name="high">The upper bound.</param>
        public Range(char low, char high)
        {
          this.low = low;
          this.high = high;
        }

        /// <summary>
        /// The lower bound.
        /// </summary>
        public char low;

        /// <summary>
        /// The upper bound.
        /// </summary>
        public char high;
      }

      private System.Collections.BitArray charSet_;
      private bool bNegated_;

      /// <summary>
      /// Initializes a new instance of the <see cref="OptimizedCharset"/> class.
      /// </summary>
      /// <param name="b">The bit set that defines the allowed characters.</param>
      public OptimizedCharset(System.Collections.BitArray b)
        : this(b, false)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="OptimizedCharset"/> class.
      /// </summary>
      /// <param name="b">The bit set that defines the allowed characters.</param>
      /// <param name="bNegated">Whether the bit set should be negated.</param>
      public OptimizedCharset(System.Collections.BitArray b, bool bNegated)
      {
        charSet_ = new System.Collections.BitArray(b);
        bNegated_ = bNegated;
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="OptimizedCharset"/> class.
      /// </summary>
      /// <param name="r">The character ranges to include.</param>
      /// <param name="c">The individual characters to include.</param>
      public OptimizedCharset(Range[] r, char[] c)
        : this(r, c, false)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="OptimizedCharset"/> class.
      /// </summary>
      /// <param name="r">The character ranges to include.</param>
      /// <param name="c">The individual characters to include.</param>
      /// <param name="bNegated">Whether the resulting set should be negated.</param>
      public OptimizedCharset(Range[] r, char[] c, bool bNegated)
      {
        int max = 0;
        if (r is not null)
          foreach (Range val in r)
            if (val.high > max)
              max = val.high;
        if (c is not null)
          foreach (int val in c)
            if (val > max)
              max = val;
        charSet_ = new System.Collections.BitArray(max + 1, false);
        if (r is not null)
        {
          foreach (Range val in r)
          {
            for (int i = val.low; i <= val.high; ++i)
            {
              charSet_[i] = true;
            }
          }
        }
        if (c is not null)
          foreach (int val in c)
            charSet_[val] = true;
        bNegated_ = bNegated;
      }

      /// <summary>
      /// Determines whether the specified character matches the set.
      /// </summary>
      /// <param name="c">The character to test.</param>
      /// <returns><c>true</c> if the character matches; otherwise, <c>false</c>.</returns>
      public bool Matches(char c)
      {
        bool bMatches = c < charSet_.Length && charSet_[c];
        if (bNegated_)
          return !bMatches;
        else
          return bMatches;
      }
    }

    /// <summary>
    /// Represents an optimized literal matcher.
    /// </summary>
    public sealed class OptimizedLiterals
    {
      internal class Trie
      {
        internal Trie(char cThis, int nIndex, string[] literals)
        {
          cThis_ = cThis;
          char cMax = char.MinValue;
          cMin_ = char.MaxValue;
          var followChars = new HashSet<char>();

          foreach (string literal in literals)
          {
            if (literal is null || nIndex > literal.Length)
              continue;
            if (nIndex == literal.Length)
            {
              bLitEnd_ = true;
              continue;
            }
            char c = literal[nIndex];
            followChars.Add(c);
            if (c < cMin_)
              cMin_ = c;
            if (c > cMax)
              cMax = c;
          }
          if (followChars.Count == 0)
          {
            children_ = null;
          }
          else
          {
            children_ = new Trie[(cMax - cMin_) + 1];
            foreach (char c in followChars)
            {
              var subLiterals = new List<string>();
              foreach (string s in literals)
              {
                if (nIndex >= s.Length)
                  continue;
                if (c == s[nIndex])
                {
                  subLiterals.Add(s);
                }
              }
              children_[c - cMin_] = new Trie(c, nIndex + 1, subLiterals.ToArray());
            }
          }
        }

        internal char cThis_;           //character stored in this node
        internal bool bLitEnd_;         //end of literal

        internal char cMin_;            //first valid character in children
        internal Trie[] children_;      //contains the successor node of cThis_;
      }

      internal Trie literalsRoot;

      /// <summary>
      /// Initializes a new instance of the <see cref="OptimizedLiterals"/> class.
      /// </summary>
      /// <param name="litAlternatives">The literal alternatives to match.</param>
      public OptimizedLiterals(string[] litAlternatives)
      {
        literalsRoot = new Trie('\u0000', 0, litAlternatives);
      }
    }

    #endregion PEG optimizations

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PegCharParser"/> class.
    /// </summary>
    public PegCharParser()
      : this("")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PegCharParser"/> class.
    /// </summary>
    /// <param name="src">The initial source text.</param>
    public PegCharParser(string src)
      : base(null)
    {
      SetSource(src);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PegCharParser"/> class.
    /// </summary>
    /// <param name="src">The initial source text.</param>
    /// <param name="errOut">The writer used for parser diagnostics.</param>
    public PegCharParser(string src, TextWriter errOut)
      : base(errOut)
    {
      SetSource(src);
      nodeCreator_ = DefaultNodeCreator;
    }

    #endregion Constructors

    #region Overrides

    /// <inheritdoc/>
    public override string TreeNodeToString(PegNode node)
    {
      string label = base.TreeNodeToString(node);
      if (node.id_ == (int)ESpecialNodes.eAnonymousNode)
      {
        string value = node.GetAsString(src_);
        if (value.Length < 32)
          label += " <" + value + ">";
        else
          label += " <" + value.Substring(0, 29) + "...>";
      }
      return label;
    }

    #endregion Overrides

    #region Reinitialization, Source Code access, TextWriter access,Tree Access

    /// <summary>
    /// Reinitializes the parser with the specified source and error writer.
    /// </summary>
    /// <param name="src">The source text.</param>
    /// <param name="Fout">The writer used for parser diagnostics.</param>
    public void Construct(string src, TextWriter Fout)
    {
      base.Construct(Fout);
      SetSource(src);
    }

    /// <summary>
    /// Sets the current source text.
    /// </summary>
    /// <param name="src">The source text.</param>
    public void SetSource(string src)
    {
      if (src is null)
        src = "";
      src_ = src;
      srcLen_ = src.Length;
      pos_ = 0;
      errors.lineStarts = new SortedList<int, int>
      {
        [0] = 1
      };
    }

    /// <summary>
    /// Gets the current source text.
    /// </summary>
    /// <returns>The current source text.</returns>
    public string GetSource()
    {
      return src_;
    }

    #endregion Reinitialization, Source Code access, TextWriter access,Tree Access

    #region Setting host variables

    /// <summary>
    /// Captures the text matched by the specified matcher.
    /// </summary>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <param name="into">Receives the matched text.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool Into(Matcher toMatch, out string into)
    {
      int pos = pos_;
      if (toMatch())
      {
        into = src_.Substring(pos, pos_ - pos);
        return true;
      }
      else
      {
        into = "";
        return false;
      }
    }

    /// <summary>
    /// Captures the source range matched by the specified matcher.
    /// </summary>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <param name="begEnd">Receives the matched range.</param>
    /// <returns><c>true</c> if the match succeeds; otherwise, <c>false</c>.</returns>
    public bool Into(Matcher toMatch, out PegBegEnd begEnd)
    {
      begEnd.posBeg_ = pos_;
      bool bMatches = toMatch();
      begEnd.posEnd_ = pos_;
      return bMatches;
    }

    /// <summary>
    /// Captures the text matched by the specified matcher and converts it to an integer.
    /// </summary>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <param name="into">Receives the converted integer.</param>
    /// <returns><c>true</c> if the match and conversion succeed; otherwise, <c>false</c>.</returns>
    public bool Into(Matcher toMatch, out int into)
    {
      into = 0;
      if (!Into(toMatch, out string s))
        return false;
      if (!int.TryParse(s, out into))
        return false;
      return true;
    }

    /// <summary>
    /// Captures the text matched by the specified matcher and converts it to a double.
    /// </summary>
    /// <param name="toMatch">The matcher to execute.</param>
    /// <param name="into">Receives the converted double value.</param>
    /// <returns><c>true</c> if the match and conversion succeed; otherwise, <c>false</c>.</returns>
    public bool Into(Matcher toMatch, out double into)
    {
      into = 0.0;
      if (!Into(toMatch, out string s))
        return false;
      if (!double.TryParse(s, out into))
        return false;
      return true;
    }

    #endregion Setting host variables

    #region Error handling

    private void LogOutMsg(string sErrKind, string sMsg)
    {
      errors.GetLineAndCol(src_, pos_, out var lineNo, out var colNo);
      if (errOut_ is not null)
      {
        errOut_.WriteLine("<{0},{1}>{2}:{3}", lineNo, colNo, sErrKind, sMsg);
        errOut_.Flush();
      }
    }

    /// <summary>
    /// Reports a fatal parser error.
    /// </summary>
    /// <param name="sMsg">The error message.</param>
    /// <returns>This method does not return.</returns>
    public virtual bool Fatal(string sMsg)
    {
      LogOutMsg("FATAL", sMsg);
      throw new PegException(sMsg);
      //return false;
    }

    /// <summary>
    /// Reports a parser warning.
    /// </summary>
    /// <param name="sMsg">The warning message.</param>
    /// <returns>Always <c>true</c>.</returns>
    public bool Warning(string sMsg)
    {
      LogOutMsg("WARNING", sMsg);
      return true;
    }

    #endregion Error handling

    #region PEG  optimized version of  e* ; e+

    /// <summary>
    /// Matches zero or more characters from the specified optimized character set.
    /// </summary>
    public bool OptRepeat(OptimizedCharset charset)
    {
      for (; pos_ < srcLen_ && charset.Matches(src_[pos_]); ++pos_)
        ;
      return true;
    }

    /// <summary>
    /// Matches one or more characters from the specified optimized character set.
    /// </summary>
    public bool PlusRepeat(OptimizedCharset charset)
    {
      int pos0 = pos_;
      for (; pos_ < srcLen_ && charset.Matches(src_[pos_]); ++pos_)
        ;
      return pos_ > pos0;
    }

    #endregion PEG  optimized version of  e* ; e+

    #region PEG '<Literal>' / '<Literal>'/i / [low1-high1,low2-high2..] / [<CharList>]

    /// <summary>
    /// Matches the specified character literal.
    /// </summary>
    public bool Char(char c1)
    {
      if (pos_ < srcLen_ && src_[pos_] == c1)
      { ++pos_; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified two-character literal.
    /// </summary>
    public bool Char(char c1, char c2)
    {
      if (pos_ + 1 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2)
      { pos_ += 2; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified three-character literal.
    /// </summary>
    public bool Char(char c1, char c2, char c3)
    {
      if (pos_ + 2 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2
          && src_[pos_ + 2] == c3)
      { pos_ += 3; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified four-character literal.
    /// </summary>
    public bool Char(char c1, char c2, char c3, char c4)
    {
      if (pos_ + 3 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2
          && src_[pos_ + 2] == c3
          && src_[pos_ + 3] == c4)
      { pos_ += 4; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified five-character literal.
    /// </summary>
    public bool Char(char c1, char c2, char c3, char c4, char c5)
    {
      if (pos_ + 4 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2
          && src_[pos_ + 2] == c3
          && src_[pos_ + 3] == c4
          && src_[pos_ + 4] == c5)
      { pos_ += 5; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified six-character literal.
    /// </summary>
    public bool Char(char c1, char c2, char c3, char c4, char c5, char c6)
    {
      if (pos_ + 5 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2
          && src_[pos_ + 2] == c3
          && src_[pos_ + 3] == c4
          && src_[pos_ + 4] == c5
          && src_[pos_ + 5] == c6)
      { pos_ += 6; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified seven-character literal.
    /// </summary>
    public bool Char(char c1, char c2, char c3, char c4, char c5, char c6, char c7)
    {
      if (pos_ + 6 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2
          && src_[pos_ + 2] == c3
          && src_[pos_ + 3] == c4
          && src_[pos_ + 4] == c5
          && src_[pos_ + 5] == c6
          && src_[pos_ + 6] == c7)
      { pos_ += 7; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified eight-character literal.
    /// </summary>
    public bool Char(char c1, char c2, char c3, char c4, char c5, char c6, char c7, char c8)
    {
      if (pos_ + 7 < srcLen_
          && src_[pos_] == c1
          && src_[pos_ + 1] == c2
          && src_[pos_ + 2] == c3
          && src_[pos_ + 3] == c4
          && src_[pos_ + 4] == c5
          && src_[pos_ + 5] == c6
          && src_[pos_ + 6] == c7
          && src_[pos_ + 7] == c8)
      { pos_ += 8; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified string literal.
    /// </summary>
    public bool Char(string s)
    {
      int sLength = s.Length;
      if (pos_ + sLength > srcLen_)
        return false;
      for (int i = 0; i < sLength; ++i)
      {
        if (s[i] != src_[pos_ + i])
          return false;
      }
      pos_ += sLength;
      return true;
    }

    /// <summary>
    /// Matches the specified character literal case-insensitively.
    /// </summary>
    public bool IChar(char c1)
    {
      if (pos_ < srcLen_ && char.ToUpper(src_[pos_]) == c1)
      { ++pos_; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified two-character literal case-insensitively.
    /// </summary>
    public bool IChar(char c1, char c2)
    {
      if (pos_ + 1 < srcLen_
          && char.ToUpper(src_[pos_]) == char.ToUpper(c1)
          && char.ToUpper(src_[pos_ + 1]) == char.ToUpper(c2))
      { pos_ += 2; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified three-character literal case-insensitively.
    /// </summary>
    public bool IChar(char c1, char c2, char c3)
    {
      if (pos_ + 2 < srcLen_
          && char.ToUpper(src_[pos_]) == char.ToUpper(c1)
          && char.ToUpper(src_[pos_ + 1]) == char.ToUpper(c2)
          && char.ToUpper(src_[pos_ + 2]) == char.ToUpper(c3))
      { pos_ += 3; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified four-character literal case-insensitively.
    /// </summary>
    public bool IChar(char c1, char c2, char c3, char c4)
    {
      if (pos_ + 3 < srcLen_
          && char.ToUpper(src_[pos_]) == char.ToUpper(c1)
          && char.ToUpper(src_[pos_ + 1]) == char.ToUpper(c2)
          && char.ToUpper(src_[pos_ + 2]) == char.ToUpper(c3)
          && char.ToUpper(src_[pos_ + 3]) == char.ToUpper(c4))
      { pos_ += 4; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified five-character literal case-insensitively.
    /// </summary>
    public bool IChar(char c1, char c2, char c3, char c4, char c5)
    {
      if (pos_ + 4 < srcLen_
          && char.ToUpper(src_[pos_]) == char.ToUpper(c1)
          && char.ToUpper(src_[pos_ + 1]) == char.ToUpper(c2)
          && char.ToUpper(src_[pos_ + 2]) == char.ToUpper(c3)
          && char.ToUpper(src_[pos_ + 3]) == char.ToUpper(c4)
          && char.ToUpper(src_[pos_ + 4]) == char.ToUpper(c5))
      { pos_ += 5; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified six-character literal case-insensitively.
    /// </summary>
    public bool IChar(char c1, char c2, char c3, char c4, char c5, char c6)
    {
      if (pos_ + 5 < srcLen_
          && char.ToUpper(src_[pos_]) == char.ToUpper(c1)
          && char.ToUpper(src_[pos_ + 1]) == char.ToUpper(c2)
          && char.ToUpper(src_[pos_ + 2]) == char.ToUpper(c3)
          && char.ToUpper(src_[pos_ + 3]) == char.ToUpper(c4)
          && char.ToUpper(src_[pos_ + 4]) == char.ToUpper(c5)
          && char.ToUpper(src_[pos_ + 5]) == char.ToUpper(c6))
      { pos_ += 6; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified seven-character literal case-insensitively.
    /// </summary>
    public bool IChar(char c1, char c2, char c3, char c4, char c5, char c6, char c7)
    {
      if (pos_ + 6 < srcLen_
          && char.ToUpper(src_[pos_]) == char.ToUpper(c1)
          && char.ToUpper(src_[pos_ + 1]) == char.ToUpper(c2)
          && char.ToUpper(src_[pos_ + 2]) == char.ToUpper(c3)
          && char.ToUpper(src_[pos_ + 3]) == char.ToUpper(c4)
          && char.ToUpper(src_[pos_ + 4]) == char.ToUpper(c5)
          && char.ToUpper(src_[pos_ + 5]) == char.ToUpper(c6)
          && char.ToUpper(src_[pos_ + 6]) == char.ToUpper(c7))
      { pos_ += 7; return true; }
      return false;
    }

    /// <summary>
    /// Matches the specified string literal case-insensitively.
    /// </summary>
    public bool IChar(string s)
    {
      int sLength = s.Length;
      if (pos_ + sLength > srcLen_)
        return false;
      for (int i = 0; i < sLength; ++i)
      {
        if (s[i] != char.ToUpper(src_[pos_ + i]))
          return false;
      }
      pos_ += sLength;
      return true;
    }

    /// <summary>
    /// Matches a character within the specified inclusive range.
    /// </summary>
    public bool In(char c0, char c1)
    {
      if (pos_ < srcLen_
          && src_[pos_] >= c0 && src_[pos_] <= c1)
      {
        ++pos_;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Matches a character within either of the specified inclusive ranges.
    /// </summary>
    public bool In(char c0, char c1, char c2, char c3)
    {
      if (pos_ < srcLen_)
      {
        char c = src_[pos_];
        if (c >= c0 && c <= c1
            || c >= c2 && c <= c3)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches a character within any of the specified inclusive ranges.
    /// </summary>
    public bool In(char c0, char c1, char c2, char c3, char c4, char c5)
    {
      if (pos_ < srcLen_)
      {
        char c = src_[pos_];
        if (c >= c0 && c <= c1
            || c >= c2 && c <= c3
            || c >= c4 && c <= c5)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches a character within any of the specified inclusive ranges.
    /// </summary>
    public bool In(char c0, char c1, char c2, char c3, char c4, char c5, char c6, char c7)
    {
      if (pos_ < srcLen_)
      {
        char c = src_[pos_];
        if (c >= c0 && c <= c1
            || c >= c2 && c <= c3
            || c >= c4 && c <= c5
            || c >= c6 && c <= c7)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches a character within any of the inclusive ranges defined by the string.
    /// </summary>
    public bool In(string s)
    {
      if (pos_ < srcLen_)
      {
        char c = src_[pos_];
        for (int i = 0; i < s.Length - 1; i += 2)
        {
          if (!(c >= s[i] && c <= s[i + 1]))
            return false;
        }
        ++pos_;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Matches a character outside all inclusive ranges defined by the string.
    /// </summary>
    public bool NotIn(string s)
    {
      if (pos_ < srcLen_)
      {
        char c = src_[pos_];
        for (int i = 0; i < s.Length - 1; i += 2)
        {
          if (c >= s[i] && c <= s[i + 1])
            return false;
        }
        ++pos_;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified characters.
    /// </summary>
    public bool OneOf(char c0, char c1)
    {
      if (pos_ < srcLen_
          && (src_[pos_] == c0 || src_[pos_] == c1))
      {
        ++pos_;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified characters.
    /// </summary>
    public bool OneOf(char c0, char c1, char c2)
    {
      if (pos_ < srcLen_)
      {
        char c = src_[pos_];
        if (c == c0 || c == c1 || c == c2)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified characters.
    /// </summary>
    public bool OneOf(char c0, char c1, char c2, char c3)
    {
      if (pos_ < srcLen_)
      {
        char c = src_[pos_];
        if (c == c0 || c == c1 || c == c2 || c == c3)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified characters.
    /// </summary>
    public bool OneOf(char c0, char c1, char c2, char c3, char c4)
    {
      if (pos_ < srcLen_)
      {
        char c = src_[pos_];
        if (c == c0 || c == c1 || c == c2 || c == c3 || c == c4)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified characters.
    /// </summary>
    public bool OneOf(char c0, char c1, char c2, char c3, char c4, char c5)
    {
      if (pos_ < srcLen_)
      {
        char c = src_[pos_];
        if (c == c0 || c == c1 || c == c2 || c == c3 || c == c4 || c == c5)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified characters.
    /// </summary>
    public bool OneOf(char c0, char c1, char c2, char c3, char c4, char c5, char c6)
    {
      if (pos_ < srcLen_)
      {
        char c = src_[pos_];
        if (c == c0 || c == c1 || c == c2 || c == c3 || c == c4 || c == c5 || c == c6)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified characters.
    /// </summary>
    public bool OneOf(char c0, char c1, char c2, char c3, char c4, char c5, char c6, char c7)
    {
      if (pos_ < srcLen_)
      {
        char c = src_[pos_];
        if (c == c0 || c == c1 || c == c2 || c == c3 || c == c4 || c == c5 || c == c6 || c == c7)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches any character contained in the specified string.
    /// </summary>
    public bool OneOf(string s)
    {
      if (pos_ < srcLen_)
      {
        if (s.IndexOf(src_[pos_]) != -1)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches any character not contained in the specified string.
    /// </summary>
    public bool NotOneOf(string s)
    {
      if (pos_ < srcLen_)
      {
        if (s.IndexOf(src_[pos_]) == -1)
        {
          ++pos_;
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Matches any character contained in the specified optimized character set.
    /// </summary>
    public bool OneOf(OptimizedCharset cset)
    {
      if (pos_ < srcLen_ && cset.Matches(src_[pos_]))
      {
        ++pos_;
        return true;
      }
      return false;
    }

    /// <summary>
    /// Matches one of the specified optimized literal alternatives.
    /// </summary>
    public bool OneOfLiterals(OptimizedLiterals litAlt)
    {
      OptimizedLiterals.Trie node = litAlt.literalsRoot;
      int matchPos = pos_ - 1;
      for (int pos = pos_; pos < srcLen_; ++pos)
      {
        if (node.bLitEnd_)
          matchPos = pos;
        char c = src_[pos];
        if (node.children_ is null
            || c < node.cMin_ || c > node.cMin_ + node.children_.Length - 1
            || node.children_[c - node.cMin_] is null)
        {
          break;
        }
        node = node.children_[c - node.cMin_];
      }
      if (matchPos >= pos_)
      {
        pos_ = matchPos;
        return true;
      }
      else
        return false;
    }

    #endregion PEG '<Literal>' / '<Literal>'/i / [low1-high1,low2-high2..] / [<CharList>]
  }

  #endregion Parsers
}
