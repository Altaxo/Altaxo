// Copyright Eli Arbel (no explicit copyright notice in original file), Apache License Version 2.0, January 2004

// Originated from: RoslynPad, RoslynPad.Editor.Windows, ICompletionDataEx.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.CodeEditing
{
	public interface ICompletionDataEx : ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData
	{
		bool IsSelected { get; }

		string SortText { get; }
	}
}