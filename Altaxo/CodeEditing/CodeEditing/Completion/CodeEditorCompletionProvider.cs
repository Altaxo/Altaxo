#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2017 Dr. Dirk Lellinger
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
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.CodeAnalysis.Text;

namespace Altaxo.CodeEditing.Completion
{
	using SnippetHandling;
	using SignatureHelp;

	internal sealed class CodeEditorCompletionProvider : ICodeEditorCompletionProvider
	{
		private readonly DocumentId _documentId;
		private readonly RoslynHost _roslynHost;
		private readonly AltaxoSnippetInfoService _snippetService;

		public CodeEditorCompletionProvider(DocumentId documentId, RoslynHost roslynHost)
		{
			_documentId = documentId;
			_roslynHost = roslynHost;
			_snippetService = (AltaxoSnippetInfoService)_roslynHost.GetService<ICSharpEditSnippetInfoService>();
		}

		public async Task<CompletionResult> GetCompletionData(int position, char? triggerChar, bool useSignatureHelp)
		{
			IList<ICompletionDataEx> completionData = null;
			IOverloadProviderEx overloadProvider = null;

			var document = _roslynHost.GetDocument(_documentId);
			if (useSignatureHelp || triggerChar != null)
			{
				var signatureHelpProvider = _roslynHost.GetService<ISignatureHelpProvider>();
				var isSignatureHelp = useSignatureHelp || signatureHelpProvider.IsTriggerCharacter(triggerChar.Value);
				if (isSignatureHelp)
				{
					var signatureHelp = await signatureHelpProvider.GetItemsAsync(
							document,
							position,
							new SignatureHelpTriggerInfo(
									useSignatureHelp
											? SignatureHelpTriggerReason.InvokeSignatureHelpCommand
											: SignatureHelpTriggerReason.TypeCharCommand, triggerChar))
							.ConfigureAwait(false);
					if (signatureHelp != null)
					{
						overloadProvider = new RoslynOverloadProvider(signatureHelp);
					}
				}
			}

			if (overloadProvider == null)
			{
				var completionService = CompletionService.GetService(document);
				var completionTrigger = GetCompletionTrigger(triggerChar);
				var data = await completionService.GetCompletionsAsync(
						document,
						position,
						completionTrigger
						).ConfigureAwait(false);
				if (data != null && data.Items.Any())
				{
					var helper = CompletionHelper.GetHelper(document, completionService);
					var text = await document.GetTextAsync().ConfigureAwait(false);
					var textSpanToText = new Dictionary<TextSpan, string>();

					completionData = data.Items
							.Where(item => MatchesFilterText(helper, item, text, textSpanToText))
							.Select(item => new RoslynCompletionData(document, item, triggerChar, _snippetService.SnippetManager))
									.ToArray<ICompletionDataEx>();
				}
				else
				{
					completionData = Array.Empty<ICompletionDataEx>();
				}
			}

			return new CompletionResult(completionData, overloadProvider);
		}

		private static bool MatchesFilterText(CompletionHelper helper, CompletionItem item, SourceText text, Dictionary<TextSpan, string> textSpanToText)
		{
			var filterText = GetFilterText(item, text, textSpanToText);
			if (string.IsNullOrEmpty(filterText)) return true;
			return helper.MatchesFilterText(item, filterText);
		}

		private static string GetFilterText(CompletionItem item, SourceText text, Dictionary<TextSpan, string> textSpanToText)
		{
			var textSpan = item.Span;
			string filterText;
			if (!textSpanToText.TryGetValue(textSpan, out filterText))
			{
				filterText = text.GetSubText(textSpan).ToString();
				textSpanToText[textSpan] = filterText;
			}
			return filterText;
		}

		private static CompletionTrigger GetCompletionTrigger(char? triggerChar)
		{
			return triggerChar != null
					? CompletionTrigger.CreateInsertionTrigger(triggerChar.Value)
					: CompletionTrigger.Default;
		}
	}
}