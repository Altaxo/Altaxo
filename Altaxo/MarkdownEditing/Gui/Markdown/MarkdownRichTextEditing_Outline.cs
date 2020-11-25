#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    AltaxoMarkdownEditing
//    Copyright (C) 2018 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//    See the LICENSE.md file in the root of the AltaxoMarkdownEditing library for more information.
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Altaxo.Gui.Markdown
{
  public partial class MarkdownRichTextEditing : UserControl
  {
    /// <summary>
    /// Model class for a tree node of the document outline.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public class OutlineTreeNode : INotifyPropertyChanged
    {
      private string? _text;
      private ObservableCollection<OutlineTreeNode>? _childNodes;
      public event PropertyChangedEventHandler? PropertyChanged;

      public string? Text
      {
        get => _text;
        set
        {
          if (!(_text == value))
          {
            _text = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
          }
        }
      }

      /// <summary>
      /// Gets or sets the line number of the source text (zero based).
      /// </summary>
      /// <value>
      /// The line.
      /// </value>
      public int Line { get; set; }

      [MaybeNull]
      public ObservableCollection<OutlineTreeNode>? ChildNodes
      {
        get => _childNodes;
        set
        {
          if (!object.ReferenceEquals(_childNodes, value))
          {
            _childNodes = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ChildNodes)));
          }
        }
      }

      public OutlineTreeNode AddNewChildNode()
      {
        var n = new OutlineTreeNode();

        if (ChildNodes is null)
          ChildNodes = new ObservableCollection<OutlineTreeNode>();

        ChildNodes.Add(n);

        return n;
      }

      public void TruncateChildNodeCountTo(int count)
      {
        if (_childNodes is not null)
        {
          for (int i = _childNodes.Count - 1; i >= count; --i)
            _childNodes.RemoveAt(i);
        }
      }

      public OutlineTreeNode GetChildAt(int index)
      {
        if (_childNodes is null)
          ChildNodes = new ObservableCollection<OutlineTreeNode>();

        for (int i = _childNodes!.Count; i <= index; ++i)
          _childNodes.Add(new OutlineTreeNode());

        return _childNodes[index];
      }

      public override string ToString()
      {
        return $"{Text} ChildCount={_childNodes?.Count ?? 0}";
      }
    }

    /// <summary>
    /// The root node for showing the outline of the document in a tree view.
    /// Note: the root node itself is not used directly, but its ChildNode collection.
    /// </summary>
    private OutlineTreeNode _outlineRootNode = new OutlineTreeNode();

    /// <summary>
    /// The root node for showing the outline of the document in a tree view.
    /// Note: the root node itself is not used directly, but its ChildNode collection.
    /// </summary>
    public OutlineTreeNode OutlineRootNode { get => _outlineRootNode; }

    private bool _isOutlineWindowVisible;
    public bool IsOutlineWindowVisible
    {
      get
      {
        return _isOutlineWindowVisible;
      }
      set
      {
        if (!(_isOutlineWindowVisible == value))
        {
          _isOutlineWindowVisible = value;
          _guiOutlineGridSplitter.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
          _guiOutline.Visibility = value ? Visibility.Visible : Visibility.Collapsed;

          if (value && _lastMarkdownDocumentProcessed is not null)
          {
            UpdateOutline(_lastMarkdownDocumentProcessed);
          }
        }
      }
    }

    public double _outlineWindowRelativeWidth;
    public double OutlineWindowRelativeWidth
    {
      get
      {
        return _outlineWindowRelativeWidth;
      }
      set
      {
        if (!(_outlineWindowRelativeWidth == value))
        {
          double referenceWidth = ActualWidth - _guiOutlineGridSplitter.ActualWidth;

          if (value > 0 && value < 1)
            _guiOutlineColumn.Width = new GridLength(referenceWidth * value, GridUnitType.Pixel);
          else
            _guiOutlineColumn.Width = GridLength.Auto;
        }
      }
    }

    private void EhOutline_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (_guiOutline.Visibility == Visibility.Visible)
      {
        double referenceWidth = ActualWidth - _guiOutlineGridSplitter.ActualWidth;
        if (_guiOutlineColumn.Width.IsAbsolute)
          _outlineWindowRelativeWidth = _guiOutlineColumn.Width.Value / referenceWidth;
        else
          _outlineWindowRelativeWidth = double.NaN;
      }
    }


    /// <summary>
    /// Gets the children of a heading node.
    /// </summary>
    /// <param name="parent">The parent heading node.
    /// This is a tuple consisting of the Markdown document, the heading level, the index of the node in the Markdown document, and a flag.
    /// The flag is false if the Index points to a real heading node in the Markdown document. The flag is true if the tuple is representing a virtual node,
    /// that does not exist in the markdown document. Virtual nodes are used i) as start node, and ii) to represent missing nodes (example: after heading 1 follows heading 3, thus heading 2 is missing).
    /// </param>
    /// <returns>An enumeration of child nodes of the parent node, with a level that is exactly one greater than the parent level.</returns>
    private IEnumerable<(Markdig.Syntax.MarkdownDocument Doc, int Level, int Index, bool IsVirtualNode)> GetChildren((Markdig.Syntax.MarkdownDocument Doc, int Level, int Index, bool IsVirtualNode) parent)
    {
      int level;
      int startIdx;

      if (parent.IsVirtualNode)
      {
        level = parent.Level;
        startIdx = parent.Index;
      }
      else
      {
        level = ((Markdig.Syntax.HeadingBlock)parent.Doc[parent.Index]).Level;
        startIdx = parent.Index + 1;
      }

      var lastLevel = level;

      for (int i = startIdx; i < parent.Doc.Count; ++i)
      {
        if (parent.Doc[i] is Markdig.Syntax.HeadingBlock h)
        {
          if (h.Level <= level)
          {
            break;
          }
          else if (h.Level == level + 1)
          {
            yield return (parent.Doc, h.Level, i, false);
          }
          else if (lastLevel == level)
          {
            yield return (parent.Doc, level + 1, i, true);
          }
          lastLevel = h.Level;
        }
      }
    }

    public void UpdateOutline(Markdig.Syntax.MarkdownDocument document)
    {
      if (_guiOutline.Visibility != System.Windows.Visibility.Visible)
        return;

      if (document is null)
        return;

      // Determine the minimum header level of the document.
      int minLevel = int.MaxValue;
      foreach (var ele in document.OfType<Markdig.Syntax.HeadingBlock>())
        minLevel = Math.Min(minLevel, ele.Level);

      if (minLevel == int.MaxValue) // if there are no heading paragraphs
      {
        _outlineRootNode.ChildNodes?.Clear(); // clear the outline tree and return.
        return;
      }

      List<(OutlineTreeNode parent, OutlineTreeNode child, int index)>? nodesToDelete = null;

      (Markdig.Syntax.MarkdownDocument Document, int Level, int Index, bool IsVirtualNode) sourceRootNode = (document, minLevel - 1, 0, true);
      ProjectTreeToTree(
        sourceRootNode,
        _outlineRootNode,
        (sn) => GetChildren(sn).GetEnumerator(),
        (dn) => dn.ChildNodes?.GetEnumerator(), // ?? Enumerable.Empty<OutlineTreeNode>().GetEnumerator(),
        (sn, dn) =>
        {
          if (sn.IsVirtualNode)
          {
            var h = sn.Document[sn.Index] as Markdig.Syntax.HeadingBlock;
            dn.Text = string.Empty;
            dn.Line = h?.Line ?? 0;
          }
          else
          {
            var h = (Markdig.Syntax.HeadingBlock)sn.Document[sn.Index];
            dn.Text = ExtractTextContentFrom(h);
            dn.Line = h.Line;
          }
        },
        (parent) => parent.AddNewChildNode(),
        (parent, child, index) => parent.ChildNodes?.RemoveAt(index),
        ref nodesToDelete
        );

      _guiOutline.ItemsSource = _outlineRootNode.ChildNodes;
    }

    private void EhOutline_SelectedItemDoubleClick(object sender, MouseButtonEventArgs e)
    {

      if (!(_guiOutline.SelectedItem is OutlineTreeNode node))
        return;

      _guiEditor.CaretOffset = _guiEditor.Document.GetOffset(node.Line + 1, 0);

      _guiEditor.ScrollTo(node.Line + 1, 0, ICSharpCode.AvalonEdit.Rendering.VisualYPosition.TextTop, _guiEditor.ActualHeight / 2, 1e-3);



      if (_privatViewingConfiguration == ViewingConfiguration.ConfigurationTabbedEditorAndViewer)
      {
        _guiEditorTab.IsSelected = true;
      }
      _guiEditor.Focus();
    }

    public string ExtractTextContentFrom(Markdig.Syntax.LeafBlock leafBlock)
    {
      var result = string.Empty;

      if (leafBlock.Inline is null)
        return result;

      foreach (var il in leafBlock.Inline)
      {
        switch (il)
        {
          case Markdig.Syntax.Inlines.CodeInline childCodeInline:
            result += childCodeInline.Content;
            break;
          case Markdig.Syntax.Inlines.ContainerInline childContainerInline:
            result += ExtractTextContentFrom(childContainerInline);
            break;
          default:
            result += il.ToString();
            break;
        }
      }

      return result;
    }

    public string ExtractTextContentFrom(Markdig.Syntax.Inlines.ContainerInline containerInline)
    {
      var result = string.Empty;



      foreach (var il in containerInline)
      {
        switch (il)
        {
          case Markdig.Syntax.Inlines.CodeInline childCodeInline:
            result += childCodeInline.Content;
            break;
          case Markdig.Syntax.Inlines.ContainerInline childContainerInline:
            result += ExtractTextContentFrom(childContainerInline);
            break;
          default:
            result += il.ToString();
            break;
        }
      }

      return result;
    }

    /// <summary>
    /// Projects a source tree onto a destination tree.
    /// Nodes are updated, added, or deleted in the destination tree to match the source tree.
    /// </summary>
    /// <typeparam name="S">Type of the source node.</typeparam>
    /// <typeparam name="D">Type of the destination node.</typeparam>
    /// <param name="sourceRootNode">The source root node.</param>
    /// <param name="destinationRootNode">The destination root node.</param>
    /// <param name="getSourceChildEnumerator">A function that takes a source node and returns an enumerator to enumerate the children of that node.</param>
    /// <param name="getDestinationChildEnumerator">A function that takes a destination node and returns an enumerator to enumerate the children of that node.</param>
    /// <param name="updateDestinationNodeFromSourceNode">An action that takes a source node and a destination node and updates the destination node according to the contents of the source node.</param>
    /// <param name="createDestinationNode">A function that takes a parent destination node as argument and creates a new destination node as a child of that parent. The return value is the newly created node.</param>
    /// <param name="deleteDestinationNode">An action that deletes a destination node. First argument is the parent node, the second argument is the destination node to delete, 3rd argument is the index of the child node (as obtained from the order of the child node enumeration).</param>
    /// <param name="destinationNodesToDelete">A helper collection to collect destination nodes that have to be deleted. Initially, may be null.</param>
    public static void ProjectTreeToTree<S, D>(
      S sourceRootNode,
      D destinationRootNode,
      Func<S, IEnumerator<S>> getSourceChildEnumerator,
      Func<D, IEnumerator<D>?> getDestinationChildEnumerator,
      Action<S, D> updateDestinationNodeFromSourceNode,
      Func<D, D> createDestinationNode,
      Action<D, D, int> deleteDestinationNode,
      ref List<(D ParentNode, D ChildNode, int Index)>? destinationNodesToDelete)
    {
      updateDestinationNodeFromSourceNode(sourceRootNode, destinationRootNode);

      int destinationNodesToDeleteOriginalCount = destinationNodesToDelete?.Count ?? 0;


      using (var sourceChildEnum = getSourceChildEnumerator(sourceRootNode))
      {
        using (var destChildEnum = getDestinationChildEnumerator(destinationRootNode))
        {
          bool s = sourceChildEnum is not null;
          bool d = destChildEnum is not null;
          for (int idx = 0; ; ++idx)
          {
            if (s)
              s = sourceChildEnum!.MoveNext();

            if (d)
              d = destChildEnum!.MoveNext();
            else
              destChildEnum?.Dispose(); // allows addition of elements without complaining

            if (s)
            {
              var destChildNode = d ? destChildEnum!.Current : createDestinationNode(destinationRootNode);
              ProjectTreeToTree(
                sourceChildEnum!.Current,
                destChildNode,
                getSourceChildEnumerator,
                getDestinationChildEnumerator,
                updateDestinationNodeFromSourceNode,
                createDestinationNode,
                deleteDestinationNode,
                ref destinationNodesToDelete);
            }
            else // if (!s)
            {
              if (d)
              {
                if (destinationNodesToDelete is null)
                  destinationNodesToDelete = new List<(D, D, int)>();

                destinationNodesToDelete.Add((destinationRootNode, destChildEnum!.Current, idx));
              }
              else
              {
                break;
              }
            }
          } // for(;;)
        }
      }

      if (destinationNodesToDelete is not null)
      {
        for (int i = destinationNodesToDelete.Count - 1; i >= destinationNodesToDeleteOriginalCount; --i)
        {
          deleteDestinationNode(destinationNodesToDelete[i].ParentNode, destinationNodesToDelete[i].ChildNode, destinationNodesToDelete[i].Index);
          destinationNodesToDelete.RemoveAt(i);
        }
      }
    }
  }
}
