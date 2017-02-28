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

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing.Folding
{
	/// <summary>
	/// Responsible for the folding markers in the text editor.
	/// </summary>
	public class SyntaxTreeFoldingStrategy
	{
		protected bool _isFirstUpdate = true;

		/// <summary>
		/// Central routine of the folding strategy. Uses the provided syntax tree in <paramref name="parseInfo"/>
		/// to calculate all folding positions.
		/// </summary>
		/// <param name="parseInfo">The syntax tree of a document.</param>
		/// <returns>Enumeration of foldings.</returns>
		public virtual IEnumerable<NewFolding> GetNewFoldings(SyntaxTree parseInfo)
		{
			var newFoldMarkers = new List<NewFolding>();

			if (parseInfo != null)
			{
				var root = parseInfo.GetRoot();
				// 1st) add foldings for all class declarations
				foreach (var classInfo in root.DescendantNodes().OfType<Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax>())
				{
					newFoldMarkers.Add(new NewFolding(classInfo.Identifier.Span.End, classInfo.Span.End));
				}

				// 2nd) add foldings for #region / #endregion pairs
				var regionStack = new Stack<Microsoft.CodeAnalysis.SyntaxTrivia>();

				foreach (var trivia in root.DescendantTrivia())
				{
					if (trivia.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.RegionDirectiveTrivia))
					{
						regionStack.Push(trivia);
					}
					else if (trivia.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.EndRegionDirectiveTrivia) && regionStack.Count > 0)
					{
						var regionStartTrivia = regionStack.Pop();

						var name = regionStartTrivia.ToFullString().Trim();
						if (regionStartTrivia.HasStructure && regionStartTrivia.GetStructure() is Microsoft.CodeAnalysis.CSharp.Syntax.RegionDirectiveTriviaSyntax node)
						{
							name = node.EndOfDirectiveToken.ToFullString().Trim();
						}

						var newFolding = new NewFolding(regionStartTrivia.Span.Start, trivia.Span.End)
						{
							DefaultClosed = _isFirstUpdate,
							Name = name
						};
						newFoldMarkers.Add(newFolding);
					}
				}
			}
			return newFoldMarkers.OrderBy(f => f.StartOffset);
		}
	}
}