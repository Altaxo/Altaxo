// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, EditorFeatures, GoToDefinition/OGoToDefinitionService.cs

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Navigation;
using Microsoft.CodeAnalysis.Options;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.GoToDefinition
{
	internal interface IGoToDefinitionService : ILanguageService
	{
		/// <summary>
		/// Finds the definitions for the symbol at the specific position in the document.
		/// </summary>
		Task<IEnumerable<INavigableItem>> FindDefinitionsAsync(Document document, int position, CancellationToken cancellationToken);

		/// <summary>
		/// Finds the definitions for the symbol at the specific position in the document and then
		/// navigates to them.
		/// </summary>
		/// <returns>True if navigating to the definition of the symbol at the provided position succeeds.  False, otherwise.</returns>
		bool TryGoToDefinition(Document document, int position, CancellationToken cancellationToken);
	}
}