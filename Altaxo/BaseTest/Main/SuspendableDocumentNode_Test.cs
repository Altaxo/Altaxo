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
using System.Threading.Tasks;
using Xunit;

namespace AltaxoTest.Main
{
  using Altaxo.Main;

  
  public class SuspendableDocumentNode_Test
  {
    private class SuspendTester
    {
      private System.Random _rnd = new Random();

      private ISuspendableByToken _doc;
      private double _probabilityToResumeTemporarily;
      private ISuspendToken _suspendToken;

      /// <summary>
      /// Initializes a new instance of the <see cref="SuspendTester"/> class.
      /// </summary>
      /// <param name="doc">The document which should be suspended/resumed.</param>
      /// <param name="probabilityToResumeTemporarily">The probability to resume temporarily (0..1).</param>
      public SuspendTester(ISuspendableByToken doc, double probabilityToResumeTemporarily)
      {
        _doc = doc;
        _probabilityToResumeTemporarily = probabilityToResumeTemporarily;
      }

      /// <summary>
      /// Suspend the document and resumes it periodically, for a time span of 1 second.
      /// Sometimes, depending on the probabilityToResumeTemporarily, the document is resumed temporarily.
      /// </summary>
      public void Worker()
      {
        var start = DateTime.UtcNow;
        do
        {
          System.Threading.Thread.Sleep(1);

          if (_suspendToken is null)
          {
            _suspendToken = _doc.SuspendGetToken();
          }
          else
          {
            var rnd = _rnd.NextDouble();

            if (rnd < _probabilityToResumeTemporarily)
            {
              _suspendToken.ResumeCompleteTemporarily();
            }
            else
            {
              _suspendToken.Dispose();
              _suspendToken = null;
            }
          }
        } while ((DateTime.UtcNow - start).TotalSeconds < 1);

        if (_suspendToken is not null)
        {
          _suspendToken.Dispose();
          _suspendToken = null;
        }
      }
    }

    private class TestNodeWithoutChild_SuspendableDocumentLeafNode : SuspendableDocumentLeafNodeWithEventArgs
    {
      public int _NumberOfSuspended;
      public int _NumberOfResumes;
      public int _NumberOfResumesSilently;

      protected override void OnSuspended()
      {
        System.Threading.Interlocked.Increment(ref _NumberOfSuspended);
        base.OnSuspended();
      }

      protected override void OnResume()
      {
        System.Threading.Interlocked.Increment(ref _NumberOfResumes);
        base.OnResume();
      }

      protected override void OnResumeSilently()
      {
        System.Threading.Interlocked.Increment(ref _NumberOfResumesSilently);
        base.OnResumeSilently();
      }
    }

    private class TestNodeWithoutChild_SuspendableDocumentNode : SuspendableDocumentNodeWithEventArgs
    {
      private SuspendableDocumentLeafNode _child;

      public int _NumberOfSuspended;
      public int _NumberOfResumes;
      public int _NumberOfResumesSilently;

      public int NumberOfChildTokens { get { return _suspendTokensOfChilds is null ? 0 : _suspendTokensOfChilds.Count; } }

      public TestNodeWithoutChild_SuspendableDocumentNode()
      {
      }

      public TestNodeWithoutChild_SuspendableDocumentNode(SuspendableDocumentLeafNode child)
      {
        _child = child;
      }

      protected override IEnumerable<DocumentNodeAndName> GetDocumentNodeChildrenWithName()
      {
        if (_child is not null)
          yield return new DocumentNodeAndName(_child, () => _child = null, "Child");
      }

      protected override void OnSuspended()
      {
        System.Threading.Interlocked.Increment(ref _NumberOfSuspended);
        base.OnSuspended();
      }

      protected override void OnResume(int eventCount)
      {
        System.Threading.Interlocked.Increment(ref _NumberOfResumes);
        base.OnResume(eventCount);
      }

      protected override void OnResumeSilently(int eventCount)
      {
        System.Threading.Interlocked.Increment(ref _NumberOfResumesSilently);
        base.OnResumeSilently(eventCount);
      }
    }

    /// <summary>
    /// Test a model suspendable document node for multiple suspend/resume actions.
    /// </summary>
    [Fact]
    public void Test001_MultipleSuspendThreads_SuspendableDocumentNode()
    {
      const int NumberOfRuns = 5;
      const int NumberOfTesters = 20;

      for (int run = 0; run < NumberOfRuns; ++run)
      {
        var node = new TestNodeWithoutChild_SuspendableDocumentNode();

        var tester = new SuspendTester[NumberOfTesters];
        for (int i = 0; i < NumberOfTesters; ++i)
        {
          tester[i] = new SuspendTester(node, 0.2);
        }

        Parallel.ForEach(tester, test => test.Worker());

        Assert.False(node.IsSuspended);
        Assert.False(node.IsSuspendedOrResumeInProgress);
        Assert.True(node._NumberOfSuspended > 0);
        Assert.True(node._NumberOfResumesSilently == 0);
        Assert.Equal(node._NumberOfSuspended, node._NumberOfResumes);
      }
    }

    /// <summary>
    /// Test a model suspendable document leaf node for multiple suspend/resume actions.
    /// </summary>
    [Fact]
    public void Test002_MultipleSuspendThreads_SuspendableDocumentLeafNode()
    {
      const int NumberOfRuns = 5;
      const int NumberOfTesters = 20;

      for (int run = 0; run < NumberOfRuns; ++run)
      {
        var node = new TestNodeWithoutChild_SuspendableDocumentLeafNode();

        var tester = new SuspendTester[NumberOfTesters];
        for (int i = 0; i < NumberOfTesters; ++i)
        {
          tester[i] = new SuspendTester(node, 0.2);
        }

        Parallel.ForEach(tester, test => test.Worker());

        Assert.False(node.IsSuspended);
        Assert.True(node._NumberOfSuspended > 0);
        Assert.True(node._NumberOfResumesSilently == 0);
        Assert.Equal(node._NumberOfSuspended, node._NumberOfResumes);
      }
    }

    /// <summary>
    /// Test a model suspendable document node with one child for multiple suspend/resume actions of the child.
    /// </summary>
    [Fact]
    public void Test003_MultipleSuspendThreads_SuspendableDocumentLeafNode()
    {
      const int NumberOfRuns = 5;
      const int NumberOfTesters = 20;

      for (int run = 0; run < NumberOfRuns; ++run)
      {
        var childNode = new TestNodeWithoutChild_SuspendableDocumentLeafNode();
        var parentNode = new TestNodeWithoutChild_SuspendableDocumentNode(childNode);

        using (var parentSuspendToken = parentNode.SuspendGetToken())
        {
          var tester = new SuspendTester[NumberOfTesters];
          for (int i = 0; i < NumberOfTesters; ++i)
          {
            tester[i] = new SuspendTester(childNode, 0.2);
          }

          Parallel.ForEach(tester, test => test.Worker());

          Assert.True(parentNode.IsSuspended);
          Assert.True(parentNode._NumberOfSuspended > 0);
          Assert.True(parentNode._NumberOfResumesSilently == 0);
          Assert.Equal(parentNode._NumberOfSuspended - 1, parentNode._NumberOfResumes);

          parentSuspendToken.Resume();
        }

        Assert.False(childNode.IsSuspended);
        Assert.True(childNode._NumberOfSuspended > 0);
        Assert.True(childNode._NumberOfResumesSilently == 0);
        Assert.Equal(childNode._NumberOfSuspended, childNode._NumberOfResumes);

        Assert.False(parentNode.IsSuspended);
        Assert.True(parentNode._NumberOfSuspended > 0);
        Assert.True(parentNode._NumberOfResumesSilently == 0);
        Assert.Equal(parentNode._NumberOfSuspended, parentNode._NumberOfResumes);
      }
    }

    /// <summary>
    /// Test a model suspendable document node with one child for multiple suspend/resume actions of the child and of the parent.
    /// </summary>
    [Fact]
    public void Test004_MultipleSuspendThreads_SuspendableDocumentLeafNode()
    {
      const int NumberOfRuns = 5;
      const int NumberOfTesters = 20;

      for (int run = 0; run < NumberOfRuns; ++run)
      {
        var childNode = new TestNodeWithoutChild_SuspendableDocumentLeafNode();
        var parentNode = new TestNodeWithoutChild_SuspendableDocumentNode(childNode);

        using (var parentSuspendToken = parentNode.SuspendGetToken())
        {
          var tester = new SuspendTester[NumberOfTesters];
          for (int i = 0; i < NumberOfTesters; ++i)
          {
            tester[i] = new SuspendTester(i % 2 == 0 ? childNode : (ISuspendableByToken)parentNode, 0.2);
          }

          Parallel.ForEach(tester, test => test.Worker());

          Assert.True(parentNode.IsSuspended);
          Assert.True(parentNode._NumberOfSuspended > 0);
          Assert.True(parentNode._NumberOfResumesSilently == 0);
          Assert.Equal(parentNode._NumberOfSuspended - 1, parentNode._NumberOfResumes);

          parentSuspendToken.Resume();
        }

        Assert.False(childNode.IsSuspended);
        Assert.True(childNode._NumberOfSuspended > 0);
        Assert.True(childNode._NumberOfResumesSilently == 0);
        Assert.Equal(childNode._NumberOfSuspended, childNode._NumberOfResumes);

        Assert.False(parentNode.IsSuspended);
        Assert.True(parentNode._NumberOfSuspended > 0);
        Assert.True(parentNode._NumberOfResumesSilently == 0);
        Assert.Equal(parentNode._NumberOfSuspended, parentNode._NumberOfResumes);
        Assert.Equal(0, parentNode.NumberOfChildTokens);
      }
    }
  }
}
