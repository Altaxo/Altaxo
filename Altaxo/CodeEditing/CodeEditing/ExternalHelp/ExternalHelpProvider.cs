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

#if !NoExternalHelp
//extern alias MCW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.ExternalAccess.Pythia.Api;
using Microsoft.CodeAnalysis.LanguageService;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Shared.Utilities;
using Roslyn.Utilities;

namespace Altaxo.CodeEditing.ExternalHelp
{
  public class ExternalHelpProvider : IExternalHelpProvider
  {
    public async Task<ExternalHelpItem> GetExternalHelpItem(Document document, int position, CancellationToken cancellationToken)
    {
      var tree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
      var token = await tree.GetTouchingTokenAsync(position, cancellationToken, findInsideTrivia: false).ConfigureAwait(false);

      if (!token.Span.IntersectsWith(position))
        return null; // nothing found

      var model = await BindTokenAsync(document, token, cancellationToken);

      if (model.Item2.Count == 0)
        return null;

      var desc = model.Item2[0];

      if (desc.Kind == SymbolKind.Parameter && model.Item2.Count >= 2) // for a parameter, we rather want to have the type of the parameter
        desc = model.Item2[1];

      if (!desc.CanBeReferencedByName && !desc.IsConstructor())
        return null;

      if (desc.DeclaredAccessibility != Microsoft.CodeAnalysis.Accessibility.NotApplicable && desc.DeclaredAccessibility < Microsoft.CodeAnalysis.Accessibility.Protected)
        return null; // only types that are at least protected should be reported

      var assemblySymbol = desc.ContainingAssembly;
      var assemblyIdentity = assemblySymbol.Identity;
      int numberOfGenericArguments = 0;

      var namespaceName = desc.ContainingNamespace.GetNameParts();

      IReadOnlyList<string> typeName = desc.ContainingType?.GetNameParts();
      string memberName = desc.Name;

      if (desc is INamedTypeSymbol typeSymbol)
      {
        typeName = typeSymbol.GetNameParts();
        memberName = null;
      }
      else if (desc is ILocalSymbol lsymb)
      {
        typeName = lsymb.Type.GetNameParts();
        var args = lsymb.Type.GetTypeArguments();
        numberOfGenericArguments = args.Length;
        memberName = null;
        assemblySymbol = lsymb.Type.ContainingAssembly;
        assemblyIdentity = assemblySymbol.Identity;
      }
      else if (desc.IsConstructor())
      {
        if (desc is IMethodSymbol msym)
        {
          var receiverType = msym.ReceiverType;
          numberOfGenericArguments = receiverType.GetTypeArguments().Length;
        }
      }

      char typeChar = 'T'; // if it is not a field, a method, or a property, then it is a type
      if (desc.Kind == SymbolKind.Event)
        typeChar = 'E';
      else if (desc.Kind == SymbolKind.Field)
        typeChar = 'F';
      else if (desc.Kind == SymbolKind.Method)
        typeChar = 'M';
      else if (desc.Kind == SymbolKind.Property)
        typeChar = 'P';

      return new ExternalHelpItem(
        assemblyIdentity: assemblyIdentity,
        typeNameParts: typeName,
        memberName: memberName,
        symbolTypeChar: typeChar,
        isConstructor: desc.IsConstructor(),
        numberOfGenericArguments: numberOfGenericArguments
        );
    }

    private async Task<ValueTuple<SemanticModel, IList<ISymbol>>> BindTokenAsync(
      Document document,
      SyntaxToken token,
      CancellationToken cancellationToken)
    {
      var semanticModel = await document.GetSemanticModelForNodeAsync(token.Parent, cancellationToken).ConfigureAwait(false);
      var enclosingType = semanticModel.GetEnclosingNamedType(token.SpanStart, cancellationToken);

      //var symbols = semanticModel.GetSymbols(token, document.Project.Solution.Workspace, bindLiteralsToUnderlyingType: true, cancellationToken: cancellationToken);

      var semanticInfo = semanticModel.GetSemanticInfo(token, document.Project.Solution.Services, cancellationToken: cancellationToken);
      IEnumerable<ISymbol> symbols = semanticInfo.GetSymbols(includeType: true);

      var bindableParent = document.GetLanguageService<ISyntaxFactsService>().TryGetBindableParent(token);
      var overloads = semanticModel.GetMemberGroup(bindableParent, cancellationToken);

      symbols = symbols.Where(IsOk)
          .Where(s => IsAccessible(s, enclosingType))
          .Concat(overloads)
          .Distinct(SymbolEquivalenceComparer.Instance);

      if (symbols.Any())
      {
        var typeParameter = symbols.First() as ITypeParameterSymbol;
        return new ValueTuple<SemanticModel, IList<ISymbol>>(
            semanticModel,
            typeParameter != null && typeParameter.TypeParameterKind == TypeParameterKind.Cref
                ? SpecializedCollections.EmptyList<ISymbol>()
                : symbols.ToList());
      }

      // Couldn't bind the token to specific symbols.  If it's an operator, see if we can at
      // least bind it to a type.
      var syntaxFacts = document.Project.LanguageServices.GetService<ISyntaxFactsService>();
      if (syntaxFacts.IsOperator(token))
      {
        var typeInfo = semanticModel.GetTypeInfo(token.Parent, cancellationToken);
        if (IsOk(typeInfo.Type))
        {
          return new ValueTuple<SemanticModel, IList<ISymbol>>(semanticModel, new List<ISymbol>(1) { typeInfo.Type });
        }
      }

      return ValueTuple.Create(semanticModel, SpecializedCollections.EmptyList<ISymbol>());
    }

    private static bool IsOk(ISymbol symbol)
    {
      return symbol != null && !symbol.IsErrorType() && !symbol.IsAnonymousFunction();
    }

    private static bool IsAccessible(ISymbol symbol, INamedTypeSymbol within)
    {
      return within == null || symbol.IsAccessibleWithin(within);
    }
  }
}
#endif
