// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Editor.Windows, RoslynHighlightingColorizer.cs

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.SyntaxHighlighting
{
	public class SemanticHighlightingColorizer : HighlightingColorizer
	{
		private readonly Workspace _workspace;
		private readonly DocumentId _documentId;

		public SemanticHighlightingColorizer(Workspace workspace, DocumentId documentId)
		{
			_workspace = workspace;
			_documentId = documentId;
		}

		protected override IHighlighter CreateHighlighter(TextView textView, ICSharpCode.AvalonEdit.Document.TextDocument document)
		{
			return new SemanticHighlighter(_workspace, _documentId, document);
		}
	}
}