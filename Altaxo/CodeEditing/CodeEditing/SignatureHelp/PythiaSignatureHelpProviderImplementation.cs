﻿#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

#if !NoCompletion && !NoSignatureHelp
using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.ExternalAccess.Pythia.Api;

namespace Altaxo.CodeEditing.SignatureHelp;

[Export(typeof(IPythiaSignatureHelpProviderImplementation))]
internal class PythiaSignatureHelpProviderImplementation : IPythiaSignatureHelpProviderImplementation
{
  public Task<(ImmutableArray<PythiaSignatureHelpItemWrapper> items, int? selectedItemIndex)> GetMethodGroupItemsAndSelectionAsync(ImmutableArray<IMethodSymbol> accessibleMethods, Document document, InvocationExpressionSyntax invocationExpression, SemanticModel semanticModel, SymbolInfo currentSymbol, CancellationToken cancellationToken)
  {
    return Task.FromResult((ImmutableArray<PythiaSignatureHelpItemWrapper>.Empty, (int?)null));
  }
}
#endif
