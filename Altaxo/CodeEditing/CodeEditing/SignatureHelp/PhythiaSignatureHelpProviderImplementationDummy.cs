#region Copyright

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
extern alias MCW;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MCW::Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.ExternalAccess.Pythia.Api;

namespace Altaxo.CodeEditing.SignatureHelp
{
  [Export(typeof(Lazy<IPythiaSignatureHelpProviderImplementation>)), Shared]
  internal class LazyPhythiaSignatureHelpProviderImplementationDummy : Lazy<IPythiaSignatureHelpProviderImplementation>
  {

    public LazyPhythiaSignatureHelpProviderImplementationDummy()
      : base(() => new PhythiaSignatureHelpProviderImplementationDummy())
    {
    }
  }

  internal class PhythiaSignatureHelpProviderImplementationDummy : IPythiaSignatureHelpProviderImplementation
  {
    Task<(ImmutableArray<PythiaSignatureHelpItemWrapper> items, int? selectedItemIndex)> IPythiaSignatureHelpProviderImplementation.GetMethodGroupItemsAndSelectionAsync(ImmutableArray<IMethodSymbol> accessibleMethods, Document document, InvocationExpressionSyntax invocationExpression, SemanticModel semanticModel, SymbolInfo currentSymbol, CancellationToken cancellationToken)
    {
      return Task.FromResult((ImmutableArray<PythiaSignatureHelpItemWrapper>.Empty, (int?)null));
    }
  }
}
#endif
