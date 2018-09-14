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

using System;
using System.Collections.Generic;
using Altaxo.Main.Services;

namespace Altaxo.AddInItems
{
  /// <summary>
  /// Supports sorting codons using InsertBefore/InsertAfter
  /// </summary>
  internal static class TopologicalSort
  {
    private sealed class Node
    {
      internal Codon codon;
      internal bool visited;
      internal List<Node> previous = new List<Node>();

      internal void Visit(List<Codon> output)
      {
        if (visited)
          return;
        visited = true;
        foreach (Node n in previous)
          n.Visit(output);
        output.Add(codon);
      }
    }

    public static List<Codon> Sort(IEnumerable<IEnumerable<Codon>> codonInput)
    {
      // Step 1: create nodes for graph
      var nameToNodeDict = new Dictionary<string, Node>();
      var allNodes = new List<Node>();
      foreach (IEnumerable<Codon> codonList in codonInput)
      {
        // create entries to preserve order within
        Node previous = null;
        foreach (Codon codon in codonList)
        {
          var node = new Node
          {
            codon = codon
          };
          if (!string.IsNullOrEmpty(codon.Id))
            nameToNodeDict[codon.Id] = node;
          // add implicit edges
          if (previous != null)
            node.previous.Add(previous);

          allNodes.Add(node);
          previous = node;
        }
      }
      // Step 2: create edges from InsertBefore/InsertAfter values
      foreach (Node node in allNodes)
      {
        if (!string.IsNullOrEmpty(node.codon.InsertBefore))
        {
          foreach (string beforeReference in node.codon.InsertBefore.Split(','))
          {
            if (nameToNodeDict.TryGetValue(beforeReference, out var referencedNode))
            {
              referencedNode.previous.Add(node);
            }
            else
            {
              Current.Log.WarnFormatted("Codon ({0}) specified in the insertbefore of the {1} codon does not exist!", beforeReference, node.codon);
            }
          }
        }
        if (!string.IsNullOrEmpty(node.codon.InsertAfter))
        {
          foreach (string afterReference in node.codon.InsertAfter.Split(','))
          {
            if (nameToNodeDict.TryGetValue(afterReference, out var referencedNode))
            {
              node.previous.Add(referencedNode);
            }
            else
            {
              Current.Log.WarnFormatted("Codon ({0}) specified in the insertafter of the {1} codon does not exist!", afterReference, node.codon);
            }
          }
        }
      }
      // Step 3: Perform Topological Sort
      var output = new List<Codon>();
      foreach (Node node in allNodes)
      {
        node.Visit(output);
      }
      return output;
    }
  }
}
