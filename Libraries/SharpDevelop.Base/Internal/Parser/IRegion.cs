// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
namespace SharpDevelop.Internal.Parser
{
	public interface IRegion: IComparable
	{
		int BeginLine {
			get;
		}

		int BeginColumn {
			get;
		}

		int EndColumn {
			get;
			set;
		}

		int EndLine {
			get;
			set;
		}

		bool IsInside(int row, int column);
	}
}
