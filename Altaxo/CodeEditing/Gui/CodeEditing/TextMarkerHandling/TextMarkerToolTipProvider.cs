﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

// Originated from: SharpDevelop, AvalonEdit.Addin, src/TextMarkerToolTipProvider.cs

using System;
using System.Linq;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;

namespace Altaxo.Gui.CodeEditing.TextMarkerHandling
{
  internal sealed class TextMarkerToolTipProvider
  {
    private readonly TextMarkerService _textMarkerService;
    private readonly TextEditor _editor;

    public TextMarkerToolTipProvider(TextMarkerService textMarkerService, TextEditor editor)
    {
      _textMarkerService = textMarkerService;
      _editor = editor;
    }

    public void HandleToolTipRequest(ToolTipRequestEventArgs args)
    {
      if (!args.InDocument)
        return;
      var offset = _editor.Document.GetOffset(args.LogicalPosition);

      //FoldingManager foldings = _editor.GetService(typeof(FoldingManager)) as FoldingManager;
      //if (foldings != null)
      //{
      //    var foldingsAtOffset = foldings.GetFoldingsAt(offset);
      //    FoldingSection collapsedSection = foldingsAtOffset.FirstOrDefault(section => section.IsFolded);

      //    if (collapsedSection != null)
      //    {
      //        args.SetToolTip(GetTooltipTextForCollapsedSection(args, collapsedSection));
      //    }
      //}

      var markersAtOffset = _textMarkerService.GetMarkersAtOffset(offset);
      var markerWithToolTip = markersAtOffset.FirstOrDefault(marker => marker.ToolTip != null);
      if (markerWithToolTip != null)
      {
        args.SetToolTip(markerWithToolTip.ToolTip);
      }
    }

    //string GetTooltipTextForCollapsedSection(ToolTipRequestEventArgs args, FoldingSection foldingSection)
    //{
    //    return ToolTipUtils.GetAlignedText(_editor.Document, foldingSection.StartOffset, foldingSection.EndOffset);
    //}
  }
}
