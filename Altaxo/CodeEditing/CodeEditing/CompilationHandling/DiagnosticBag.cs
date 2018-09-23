// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Originated from: Roslyn, Compilers, Core/Portable/Diagnostic/DiagnosticBag.cs

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting;

namespace Altaxo.CodeEditing.CompilationHandling
{
  public class DiagnosticBag
  {
    private ConcurrentQueue<Diagnostic> _lazyBag;

    public bool IsEmptyWithoutResolution
    {
      get
      {
        var bag = _lazyBag;
        return bag == null || bag.IsEmpty;
      }
    }

    private ConcurrentQueue<Diagnostic> Bag
    {
      get
      {
        var bag = _lazyBag;
        if (bag != null)
        {
          return bag;
        }

        var newBag = new ConcurrentQueue<Diagnostic>();
        return Interlocked.CompareExchange(ref _lazyBag, newBag, null) ?? newBag;
      }
    }

    public void AddRange<T>(ImmutableArray<T> diagnostics) where T : Diagnostic
    {
      if (!diagnostics.IsDefaultOrEmpty)
      {
        var bag = Bag;
        foreach (var t in diagnostics)
        {
          bag.Enqueue(t);
        }
      }
    }

    public IEnumerable<Diagnostic> AsEnumerable()
    {
      var bag = Bag;

      var foundVoid = bag.Any(diagnostic => diagnostic.Severity == DiagnosticSeverityVoid);

      return foundVoid
          ? AsEnumerableFiltered()
          : bag;
    }

    internal void Clear()
    {
      var bag = _lazyBag;
      if (bag != null)
      {
        _lazyBag = null;
      }
    }

    private static DiagnosticSeverity DiagnosticSeverityVoid => ~DiagnosticSeverity.Info;

    private IEnumerable<Diagnostic> AsEnumerableFiltered()
    {
      return Bag.Where(diagnostic => diagnostic.Severity != DiagnosticSeverityVoid);
    }
  }
}
