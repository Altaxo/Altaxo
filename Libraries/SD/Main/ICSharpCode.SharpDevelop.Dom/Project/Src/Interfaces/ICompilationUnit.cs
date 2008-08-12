﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1661 $</version>
// </file>

using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public interface ICompilationUnit
	{
		string FileName {
			get;
			set;
		}
		
		bool ErrorsDuringCompile {
			get;
			set;
		}
		
		object Tag {
			get;
			set;
		}
		
		IProjectContent ProjectContent {
			get;
		}
		
		List<IUsing> Usings {
			get;
		}
		
		List<IAttribute> Attributes {
			get;
		}
		
		List<IClass> Classes {
			get;
		}
		
		List<IComment> MiscComments {
			get;
		}
		
		List<IComment> DokuComments {
			get;
		}
		
		List<TagComment> TagComments {
			get;
		}
		
		List<FoldingRegion> FoldingRegions {
			get;
		}
		
		
		/// <summary>
		/// Returns the innerst class in which the carret currently is, returns null
		/// if the carret is outside any class boundaries.
		/// </summary>
		IClass GetInnermostClass(int caretLine, int caretColumn);
		
		List<IClass> GetOuterClasses(int caretLine, int caretColumn);
	}
}
