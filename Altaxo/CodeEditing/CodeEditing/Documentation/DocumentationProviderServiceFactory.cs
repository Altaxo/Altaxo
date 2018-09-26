// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, Workspaces, Core/Desktop/Workspace/Host/Documentation/DocumentationProviderServiceFactory.cs

using System;
using System.Collections.Concurrent;
using System.Composition;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.Host.Mef;

namespace Altaxo.CodeEditing.Documentation
{
  [ExportWorkspaceServiceFactory(typeof(IDocumentationProviderService), ServiceLayer.Host), Shared]
  internal sealed class DocumentationProviderServiceFactory : IWorkspaceServiceFactory
  {
    public IWorkspaceService CreateService(HostWorkspaceServices workspaceServices)
    {
      return new DocumentationProviderService();
    }

    internal sealed class DocumentationProviderService : IDocumentationProviderService
    {
      private readonly ConcurrentDictionary<string, DocumentationProvider> _assemblyPathToDocumentationProviderMap =
          new ConcurrentDictionary<string, DocumentationProvider>();

      public DocumentationProvider GetDocumentationProvider(string assemblyPath)
      {
        if (assemblyPath == null)
        {
          throw new ArgumentNullException(nameof(assemblyPath));
        }

        assemblyPath = Path.ChangeExtension(assemblyPath, "xml");
        if (!_assemblyPathToDocumentationProviderMap.TryGetValue(assemblyPath, out var provider))
        {
          provider = _assemblyPathToDocumentationProviderMap.GetOrAdd(assemblyPath, _path => XmlDocumentationProvider.CreateFromFile(_path));
        }

        return provider;
      }
    }
  }
}
