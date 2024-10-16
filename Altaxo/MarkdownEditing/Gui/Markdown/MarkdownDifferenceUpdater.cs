﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    AltaxoMarkdownEditing
//    Copyright (C) 2018 Dr. Dirk Lellinger
//    This source file is licensed under the MIT license.
//    See the LICENSE.md file in the root of the AltaxoMarkdownEditing library for more information.
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Wpf;

namespace Altaxo.Gui.Markdown
{
  /// <summary>
  /// Incremental changer of a <see cref="FlowDocument"/> by comparing an old version of a markdown text with a new version, and changing the affected parts of the <see cref="FlowDocument"/>.
  /// </summary>
  public class MarkdownDifferenceUpdater
  {
    #region Members given in constructor

    /// <summary>
    /// The old source text. It is assumed that the FlowDocument at the beginning of the task corresponds to this source text.
    /// </summary>
    protected string OldSourceText { get; private set; }

    /// <summary>
    /// The markdown document of the old source text.  It is assumed that the FlowDocument at the beginning of the task corresponds to this document.
    /// </summary>
    protected MarkdownDocument OldDocument { get; set; }

    /// <summary>
    /// The new source text.
    /// </summary>
    public string NewSourceText { get; private set; }

    /// <summary>
    /// The update sequence number of the new source text.
    /// </summary>
    public long NewSourceTextUsn { get; private set; }

    /// <summary>
    /// The markdown document of the new source text.
    /// </summary>
    public MarkdownDocument? NewDocument { get; private set; }

    /// <summary>
    /// Gets the markdown pipeline used for parsing the source text.
    /// </summary>
    protected MarkdownPipeline Pipeline { get; private set; }

    /// <summary>
    /// Get the styles used for rendering.
    /// </summary>
    /// <value>
    /// The styles.
    /// </value>
    protected IStyles Styles { get; private set; }

    protected IWpfImageProvider? ImageProvider { get; private set; }

    /// <summary>
    /// The flow document to change.
    /// </summary>
    protected FlowDocument FlowDocument { get; private set; }

    /// <summary>
    /// The dispatcher which is needed because the last part of the task has to run in Gui context.
    /// </summary>
    protected Dispatcher Dispatcher { get; private set; }

    /// <summary>
    /// The cancellation token, used to cancel the task if neccessary.
    /// </summary>
    private CancellationToken _cancellationToken;

    /// <summary>
    /// At the end of the task, when already in Gui context, this action is responsible for setting back
    /// the current version of the source text and the parsed markdown whereever they are stored.
    /// </summary>
    protected Action<string, long, MarkdownDocument> NewTextAndDocumentSetter { get; private set; }

    /// <summary>
    /// This action sets a flag to true when an update of the flow document is in progress.
    /// This helps our MarkdownEditing controll to distinguish between TextChanged events coming from an update of the FlowDocument
    /// and TextChanged events coming from user input, e.g. correction of spelling errors.
    /// </summary>
    protected Action<bool> SetFlowDocumentUpdateInProgressFlag { get; private set; }

    #endregion Members given in constructor

    #region Operational members

    /// <summary>
    /// The list of changed markdig toplevel elements (in comparison from old to new parsed document).
    /// </summary>
    private List<MarkdownObject> _listOfChangedMarkdig;

    /// <summary>
    /// The dictionary that translates the markdown object of the old markdig document into the elements of the new parsed markdig document.
    /// </summary>
    private Dictionary<MarkdownObject, MarkdownObject> _dictionaryOldToNew;

    #endregion Operational members

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownDifferenceUpdater"/> class.
    /// </summary>
    /// <param name="oldSourceText">The previous version of the source text.</param>
    /// <param name="oldDocument">The previous version of the parsed markdown document.</param>
    /// <param name="pipeline">The markdown pipeline used to process the source text.</param>
    /// <param name="styles">The markdown styles to be used for the rendering.</param>
    /// <param name="imageProvider">The image provider.</param>
    /// <param name="newSourceText">The new version of the source text.</param>
    /// <param name="newSourceTextUsn">The serial number of the new source text.</param>
    /// <param name="flowDocument">The flow document to render to.</param>
    /// <param name="dispatcher">The dispatcher (used here to process some actions in Gui context).</param>
    /// <param name="newDocumentSetter">
    /// Action that is called at the end of the processing in Gui context. It is intended to set the new new document in the class that is calling this function.
    /// The arguments are: i) the new source text, ii) the serial number of the new source text, and iii) the parsed markdown document.
    /// </param>
    /// <param name="setFlowDocumentUpdateInProgressFlag">
    /// Action that is called in Gui context. It is intended to signal the class that is calling this function exactly when the flow document is updated.
    /// This action is called twice: at the beginning of the update with the argument set to true, and at the end of the update with the argument set to false.
    /// Argument is a flag that tells if the update of the flow document is in progress now.
    /// </param>
    /// <param name="cancellationToken">Cancellation token that can be used to cancel the update, e.g. if the user has entered new text.</param>
    public MarkdownDifferenceUpdater(string oldSourceText, MarkdownDocument oldDocument, MarkdownPipeline pipeline, IStyles styles, IWpfImageProvider? imageProvider, string newSourceText, long newSourceTextUsn, FlowDocument flowDocument, Dispatcher dispatcher, Action<string, long, MarkdownDocument> newDocumentSetter, Action<bool> setFlowDocumentUpdateInProgressFlag, CancellationToken cancellationToken)
    {
      OldSourceText = oldSourceText;
      OldDocument = oldDocument;
      Pipeline = pipeline;
      Styles = styles;
      ImageProvider = imageProvider;
      NewSourceText = newSourceText;
      NewSourceTextUsn = newSourceTextUsn;
      FlowDocument = flowDocument;
      Dispatcher = dispatcher;
      NewTextAndDocumentSetter = newDocumentSetter;
      SetFlowDocumentUpdateInProgressFlag = setFlowDocumentUpdateInProgressFlag;
      _cancellationToken = cancellationToken;
      _dictionaryOldToNew = new Dictionary<MarkdownObject, MarkdownObject>();
      _listOfChangedMarkdig = new List<MarkdownObject>();
    }

    /// <summary>
    /// Parses the new source text (given in the constructor), and updates the flow document.
    /// </summary>
    public void Parse()
    {
      if (_cancellationToken.IsCancellationRequested)
        return;

      // TODO make parse cancellable, at least in the first level
      NewDocument = Markdig.Markdown.Parse(NewSourceText, Pipeline);

      if (_cancellationToken.IsCancellationRequested)
        return;

      LinkReferenceTrackerPostProcessor.TrackLinks(NewDocument, NewSourceTextUsn, ImageProvider);

      if (_cancellationToken.IsCancellationRequested)
        return;

      var (indexOfFirstDifference, indexOfLastDifferenceInOld, indexOfLastDifferenceInNew) = FindFirstAndLastChangedIndexInSourceText();

      if (_cancellationToken.IsCancellationRequested)
        return;

      // Dictionary that translates the markdown objects of the old parsed doc into markdown objects of the new parsed doc
      _dictionaryOldToNew = new Dictionary<MarkdownObject, MarkdownObject>();
      int indexOfFirstTopLevelBlockChanged = TranslateOldToNewFirstPart(_dictionaryOldToNew, indexOfFirstDifference, OldDocument, NewDocument);

      if (_cancellationToken.IsCancellationRequested)
        return;

      // Dictionary that translates the markdown objects of the old parsed doc into markdown objects of the new parsed doc
      var (indexOfLastTopLevelBlockChangedOld, indexOfLastTopLevelBlockChangedNew) = TranslateOldToNewLastPart(_dictionaryOldToNew, indexOfLastDifferenceInOld, indexOfLastDifferenceInNew, OldDocument, NewDocument);

      if (_cancellationToken.IsCancellationRequested)
        return;

      // create a list of changed markdig elements
      _listOfChangedMarkdig = new List<MarkdownObject>();
      for (int i = indexOfFirstTopLevelBlockChanged; i <= indexOfLastTopLevelBlockChangedNew; ++i)
        _listOfChangedMarkdig.Add(NewDocument[i]);

      if (_cancellationToken.IsCancellationRequested)
        return;

      // the next part involves changes to the FlowDocument. That's why it is not cancellable any more, and needs to be executed in Gui context
      Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => UpdateTheFlowDocument()));
    }

    /// <summary>
    /// Updates the flow document, i.e. deletes the superfluous items, creates new TextElements, and inserts them in the FlowDocument.
    /// Finally, it will exchange the current (so far) source text and markdig document in the Gui control. Since everything has to
    /// be done in the Gui context, there should be no race condition.
    /// </summary>
    private void UpdateTheFlowDocument()
    {
      // this is the very last opportunity to cancel the update, if it was for some time waiting in the message queue...

      if (_cancellationToken.IsCancellationRequested)
        return;

      SetFlowDocumentUpdateInProgressFlag?.Invoke(true);

      // the next lines must be called in the context of Gui,
      // and can not be cancelled, since that would
      // mess up the FlowDocument

      // Exchange the tags in the flow document
      ExchangeMarkdigTagsInFlowDocument(_dictionaryOldToNew, FlowDocument.Blocks);

      // Get the insertion and deletion positions in FlowDocument.Blocks
      var (firstLevelTextElementToInsertBefore, firstLevelTextElementToInsertAfter, firstLevelTextElementsToDelete) = GetTextElementInsertionAndDeletionPositions();

      // create new TextElements for the changed span of top level blocks - this seems to be the most CPU intensive call
      var newListOfTextElements = ListOfMarkdownObjectsToListOfTextElements(_listOfChangedMarkdig, Pipeline, Styles, ImageProvider); // create new TextElements

      // now delete the changed top level blocks of FlowDocument and insert or add the newly created ones
      DeleteOldAndInsertNewElementsInFlowDocument(firstLevelTextElementsToDelete, newListOfTextElements, firstLevelTextElementToInsertBefore, firstLevelTextElementToInsertAfter);

      SetFlowDocumentUpdateInProgressFlag?.Invoke(false);

      // exchange the current source text and parsed markdig in the text editor
      NewTextAndDocumentSetter(NewSourceText, NewSourceTextUsn, NewDocument!);
    }

    /// <summary>
    /// Deletes the old top level blocks in <see cref="FlowDocument"/> and inserts new elements.
    /// </summary>
    /// <param name="firstLevelTextElementsToDelete">The first level text elements to delete in the <see cref="FlowDocument"/>.</param>
    /// <param name="firstLevelTextElementsToInsert">The first level text elements to insert in <see cref="FlowDocument"/>. The insert position depends on the next two parameters</param>
    /// <param name="firstLevelTextElementToInsertBefore">The first level text element to insert before. Can be null.</param>
    /// <param name="firstLevelTextElementToInsertAfter">The first level text element to insert after. Can be null.</param>
    private void DeleteOldAndInsertNewElementsInFlowDocument(List<System.Windows.Documents.Block> firstLevelTextElementsToDelete, IList<TextElement> firstLevelTextElementsToInsert, System.Windows.Documents.Block? firstLevelTextElementToInsertBefore, System.Windows.Documents.Block? firstLevelTextElementToInsertAfter)
    {
      var blocks = FlowDocument.Blocks;
      foreach (var textEle in firstLevelTextElementsToDelete)
        blocks.Remove(textEle);

      if (firstLevelTextElementsToInsert.Count > 0)
      {
        if (firstLevelTextElementToInsertAfter is not null)
        {
          for (int i = firstLevelTextElementsToInsert.Count - 1; i >= 0; --i)
            blocks.InsertAfter(firstLevelTextElementToInsertAfter, (System.Windows.Documents.Block)firstLevelTextElementsToInsert[i]);
        }
        else if (firstLevelTextElementToInsertBefore is not null)
        {
          for (int i = 0; i < firstLevelTextElementsToInsert.Count; ++i)
            blocks.InsertBefore(firstLevelTextElementToInsertBefore, (System.Windows.Documents.Block)firstLevelTextElementsToInsert[i]);
        }
        else
        {
          // the insert position could not be determined, because none of the old blocks have been deleted
          // we have to decide either (i) the blocks have to be added before all the other blocks, or
          // (ii) the blocks have to be added after all the other blocks
          // we do this using the markdig tags of the last block to insert, and the first of the old blocks

          var lastToInsert = firstLevelTextElementsToInsert[firstLevelTextElementsToInsert.Count - 1];
          var firstBlock = blocks.FirstBlock;
          var lastToInsertTag = lastToInsert?.Tag as MarkdownObject;
          var firstBlockTag = firstBlock?.Tag as MarkdownObject;

          if (lastToInsertTag is not null &&
              firstBlockTag is not null &&
              (lastToInsertTag!.Span.End <= firstBlockTag.Span.Start)
            )
          {
            // is added at start
            for (int i = 0; i < firstLevelTextElementsToInsert.Count; ++i)
              blocks.InsertBefore(firstBlock, (System.Windows.Documents.Block)firstLevelTextElementsToInsert[i]);
          }
          else
          {
            for (int i = 0; i < firstLevelTextElementsToInsert.Count; ++i)
              blocks.Add((System.Windows.Documents.Block)firstLevelTextElementsToInsert[i]);
          }
        }
      }
    }

    /// <summary>
    /// Gets the insertion and deletion positions of the top level blocks of <see cref="FlowDocument"/>.
    /// </summary>
    /// <returns>A tuple, consisting of positions either to insert before, to insert after, and the top level blocks to delete.</returns>
    private (System.Windows.Documents.Block? firstLevelTextElementToInsertBefore,
              System.Windows.Documents.Block? firstLevelTextElementToInsertAfter,
              List<System.Windows.Documents.Block> firstLevelTextElementsToDelete
            ) GetTextElementInsertionAndDeletionPositions()
    {
      // find the first block in FlowDocument that has to be exchanged

      var firstLevelTextElementsToDelete = new List<System.Windows.Documents.Block>();
      System.Windows.Documents.Block? firstLevelTextElementToInsertBefore = null;
      System.Windows.Documents.Block? firstLevelTextElementToInsertAfter = null;
      System.Windows.Documents.Block? previousBlockWithTag = null;
      foreach (var blk in FlowDocument.Blocks)
      {
        if (blk.Tag is null) // blk.Tag==null indicates that this is a block that has been changed and should be removed (during exchanging the of the tags the tag of this block is assigned null)
        {
          if (firstLevelTextElementToInsertAfter is null)
          {
            firstLevelTextElementToInsertAfter = previousBlockWithTag;
          }
          firstLevelTextElementToInsertBefore = null;
          firstLevelTextElementsToDelete.Add(blk);
        }
        else
        {
          if (firstLevelTextElementToInsertBefore is null && firstLevelTextElementsToDelete.Count > 0)
          {
            firstLevelTextElementToInsertBefore = blk;
          }
          previousBlockWithTag = blk; // assign previous block here to make sure it is not in the list of elements to delete
        }
      }

      return (firstLevelTextElementToInsertBefore, firstLevelTextElementToInsertAfter, firstLevelTextElementsToDelete);
    }

    /// <summary>
    /// Finds the first and last changed index in source text by comparing and old and a new version of the source text.
    /// </summary>
    /// <returns>The index of the first difference of the two source texts, and the indices of the last difference in the source text (for the old and the new source texts).</returns>
    private (int IndexOfFirstDifference, int IndexOfLastDifferenceInOld, int IndexOfLastDifferenceInNew) FindFirstAndLastChangedIndexInSourceText()
    {
      // Compare old source text with new source text

      string oldS = OldSourceText;
      string newS = NewSourceText;

      int i;
      int len = Math.Min(OldSourceText.Length, NewSourceText.Length);
      for (i = 0; i < len; ++i)
        if (oldS[i] != newS[i])
          break;

      int indexOfFirstDifference = i;

      int j;
      for (i = oldS.Length - 1, j = newS.Length - 1; i >= 0 && j >= 0; --i, --j)
        if (oldS[i] != newS[j])
          break;

      int indexOfLastDifferenceInOld = i;
      int indexOfLastDifferenceInNew = j;

      return (indexOfFirstDifference, indexOfLastDifferenceInOld, indexOfLastDifferenceInNew);
    }

    /// <summary>
    /// Makes a recursion through the beginnings of the old and the new markdig documents until this top level block, where the both source texts differ.
    /// During recursion, the translation dictionary from old to new markdig objects will be filled.
    /// </summary>
    /// <param name="dictOldToNew">The dictionary that translates markdig objects in the old markdig document to the objects in the new markdig document.</param>
    /// <param name="indexOfFirstDifference">The index of the first difference in the source text (this index is valid for both old and new source text).</param>
    /// <param name="oldBlocks">List of markdig objects from the old document. During the top level call, this are the top level blocks of the old markdig document.</param>
    /// <param name="newBlocks">List of markdig objects from the new document. During the top level call, this are the top level blocks of the new markdig document.</param>
    /// <returns>First index of the top level blocks (of old and new markdig document), that differs (in comparsion of old and new).</returns>
    private int TranslateOldToNewFirstPart(Dictionary<MarkdownObject, MarkdownObject> dictOldToNew, int indexOfFirstDifference, IReadOnlyList<MarkdownObject> oldBlocks, IReadOnlyList<MarkdownObject> newBlocks)
    {
      var len = Math.Min(oldBlocks.Count, newBlocks.Count);
      int i;
      for (i = 0; i < len; ++i)
      {
        var blockO = oldBlocks[i];
        var blockN = newBlocks[i];

        if (blockN.Span.End >= indexOfFirstDifference)
          return i;

        if (blockN.Span.End != blockO.Span.End)
          return i;

        if (blockN.GetType() != blockO.GetType())
          return i;
        var childsO = GetChilds(blockO);
        var childsN = GetChilds(blockN);
        if (childsO is not null && childsN is not null)
        {
          if (0 <= TranslateOldToNewFirstPart(dictOldToNew, indexOfFirstDifference, childsO, childsN))
            return i;
        }

        dictOldToNew.Add(blockO, blockN);
      }

      return i;
    }

    /// <summary>
    /// Makes a recursion through the old and the new markdig documents beginning from the end upwards, up to the top level block, where both source texts differ.
    /// During recursion, the translation dictionary from old to new markdig objects will be filled.
    /// </summary>
    /// <param name="dictOldToNew">The dictionary that translates markdig objects in the old markdig document to the objects in the new markdig document.</param>
    /// <param name="indexOfLastDifferenceO">The index of the last difference in the old source text.</param>
    /// <param name="indexOfLastDifferenceN">The index of the last difference in the new source text.</param>
    /// <param name="oldBlocks">List of markdig objects from the old document. During the top level call, this are the top level blocks of the old markdig document.</param>
    /// <param name="newBlocks">List of markdig objects from the new document. During the top level call, this are the top level blocks of the new markdig document.</param>
    /// <returns>Last index of the top level blocks (of old and new markdig document), that differs (in comparsion of old and new).</returns>
    private (int firstLevelIndexOld, int firstLevelIndexNew) TranslateOldToNewLastPart(Dictionary<MarkdownObject, MarkdownObject> dictOldToNew, int indexOfLastDifferenceO, int indexOfLastDifferenceN, IReadOnlyList<MarkdownObject> oldBlocks, IReadOnlyList<MarkdownObject> newBlocks)
    {
      int i, j;
      for (i = oldBlocks.Count - 1, j = newBlocks.Count - 1; i >= 0 && j >= 0; --i, --j)
      {
        var blockO = oldBlocks[i];
        var blockN = newBlocks[j];

        if (blockN.Span.Start <= indexOfLastDifferenceN)
          return (i, j);

        if (blockN.Span.Start != blockO.Span.Start + (indexOfLastDifferenceN - indexOfLastDifferenceO))
          return (i, j);

        if (blockN.GetType() != blockO.GetType())
          return (i, j);

        var childsO = GetChilds(blockO);
        var childsN = GetChilds(blockN);
        if (childsO is not null && childsN is not null)
        {
          var (ii, jj) = TranslateOldToNewLastPart(dictOldToNew, indexOfLastDifferenceO, indexOfLastDifferenceN, childsO, childsN);
          if (ii >= 0 || jj >= 0)
            return (i, j);
        }

        // 2019-05-09 Try to find out why sometimes blockO is already in the dictionary
        if (dictOldToNew.TryGetValue(blockO, out var alreadyPresentBlockN))
        {
          if (!object.ReferenceEquals(alreadyPresentBlockN, blockN))
            throw new InvalidOperationException();
        }
        else
        {
          dictOldToNew.Add(blockO, blockN);
        }
      }

      return (i, j);
    }

    #region Exchange of Markdig tags in FlowDocument

    /// <summary>
    /// Exchanges recursively the markdig tags in flow document, using the dictionary that translates old to new markdig objects.
    /// For tags no dictionary entry is found, the tag is set to null. This will be the case for all TextElements that belong to the changed area of source code.
    /// </summary>
    /// <param name="oldToNewDict">The dictionary that translates old to new markdig objects.</param>
    /// <param name="textElements">The text elements. During the top level call, this are the elements of the Blocks collection of the <see cref="FlowDocument"/>.</param>
    public void ExchangeMarkdigTagsInFlowDocument(Dictionary<MarkdownObject, MarkdownObject> oldToNewDict, System.Collections.IEnumerable textElements)
    {
      foreach (TextElement t in textElements)
      {
        var childs = GetChilds(t);
        if (childs is not null)
        {
          ExchangeMarkdigTagsInFlowDocument(oldToNewDict, childs);
        }

        if (t.Tag is MarkdownObject mdo)
        {
          if (oldToNewDict.TryGetValue(mdo, out var newMdo))
            t.Tag = newMdo;
          else
            t.Tag = null;
        }
      }
    }

    #endregion Exchange of Markdig tags in FlowDocument

    #region Markdig syntax tree

    /// <summary>
    /// Gets the childs of a markdown object. Null is returned if no childs were to be found.
    /// </summary>
    /// <param name="parent">The markdown object from which to get the childs.</param>
    /// <returns>The childs of the given markdown object, or null.</returns>
    public static IReadOnlyList<MarkdownObject>? GetChilds(MarkdownObject parent)
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

    #endregion Markdig syntax tree

    #region Flowdocument tree

    /// <summary>
    /// Gets the childs of a <see cref="TextElement"/>. Null is returned if no childs were to be found.
    /// </summary>
    /// <param name="parent">The <see cref="TextElement"/> from which to get the childs.</param>
    /// <returns>The childs of the given <see cref="TextElement"/>, or null.</returns>
    public static System.Collections.IList? GetChilds(TextElement parent)
    {
      if (parent is Paragraph para)
      {
        return para.Inlines;
      }
      else if (parent is List list)
      {
        return list.ListItems;
      }
      else if (parent is ListItem listItem)
      {
        return listItem.Blocks;
      }
      else if (parent is Span span)
      {
        return span.Inlines;
      }
      else if (parent is Section section)
      {
        return section.Blocks;
      }
      return null;
    }

    /// <summary>
    /// Converts a list of (top level) <see cref="MarkdownObject"/>s to a list of (top level) <see cref="TextElement"/>s.
    /// </summary>
    /// <param name="markdownObjects">List of top level markdown objects.</param>
    /// <param name="pipeline">The pipeline used for the conversion.</param>
    /// <returns>The list of (top level) <see cref="TextElement"/>s as the result of the conversion.</returns>
    /// <exception cref="System.ArgumentNullException">if markdown variable is null</exception>
    public static IList<TextElement> ListOfMarkdownObjectsToListOfTextElements(IList<MarkdownObject> markdownObjects, MarkdownPipeline pipeline, IStyles styles, IWpfImageProvider? imageProvider)
    {
      var p = new Paragraph();
      styles.ApplyParagraphStyle(p);

      var result = new TextElementList() { FontSize = p.FontSize };
      var renderer = new Markdig.Renderers.WpfRenderer(result, styles) { ImageProvider = imageProvider };

      pipeline.Setup(renderer);
      renderer.Render(markdownObjects);

      return result;
    }

    /// <summary>
    /// A list of <see cref="TextElement"/>s, that can be used instead of a <see cref="FlowDocument"/> as top level element for Wpf rendering.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.List{System.Windows.Documents.TextElement}" />
    /// <seealso cref="System.Windows.Markup.IAddChild" />
    private class TextElementList : List<TextElement>, System.Windows.Markup.IAddChild
    {
      public void AddChild(object value)
      {
        Add((TextElement)value);
      }

      public void AddText(string text)
      {
        throw new NotImplementedException();
      }

      /// <summary>
      /// Gets or sets the size of the font. It is important to set the fontsize to the same value as the markdown
      /// document, because otherwise formulas can not determine their font size.
      /// </summary>
      public double FontSize { get; set; } = 16;
    }

    #endregion Flowdocument tree
  }
}
